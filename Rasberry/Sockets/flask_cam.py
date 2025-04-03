import threading
import cv2
from flask import Flask, Response
from cam import WebcamVideoStream
from flask_basicauth import BasicAuth
import os
import signal

app = Flask(__name__)
app.config['BASIC_AUTH_USERNAME'] = 'pi'
app.config['BASIC_AUTH_PASSWORD'] = 'pi'
app.config['BASIC_AUTH_FORCE'] = True

basic_auth = BasicAuth(app)
active_connections = 0
camera = None  # Instance unique de la caméra
lock = threading.Lock()  # Sécuriser l'accès au compteur


def gen():
    global active_connections, camera
    with lock:
        active_connections += 1
        if camera is None:
            camera = WebcamVideoStream().start()  # Démarre la caméra si elle est éteinte
            print("Camera started")
        print("Active connections:", active_connections)
    try:
        while True:
            if camera.stopped:
                break
            frame = camera.read()
            ret, jpeg = cv2.imencode('.jpg', frame)
            if jpeg is not None:
                yield (b'--frame\r\n'
                       b'Content-Type: image/jpeg\r\n\r\n' + jpeg.tobytes() + b'\r\n\r\n')
            else:
                print("frame is none")
    except GeneratorExit:
        pass  # L'utilisateur a quitté

@app.route('/video_feed')
@basic_auth.required
def video_feed():
    return Response(gen(), mimetype='multipart/x-mixed-replace; boundary=frame')

# New route to shut down the server
@app.route('/shutdown', methods=['GET'])
@basic_auth.required
def shutdown():
    """Shuts down the Flask server."""
    os.kill(os.getpid(), signal.SIGTERM)  # Terminate the Flask process
    return "Server shutting down..."

if __name__ == '__main__':
    app.run(host='0.0.0.0', debug=True, threaded=True)


# http://127.0.0.1:5000/video_feed