import threading
import cv2
from flask import Flask, Response, render_template
from cam import WebcamVideoStream
from flask_basicauth import BasicAuth
import os
import signal
import time

app = Flask(__name__)
app.config['BASIC_AUTH_USERNAME'] = 'pi'
app.config['BASIC_AUTH_PASSWORD'] = 'pi'
app.config['BASIC_AUTH_FORCE'] = True

basic_auth = BasicAuth(app)
active_connections = 0
camera = None  # Instance unique de la caméra
lock = threading.Lock()  # Sécuriser l'accès au compteur

@app.route('/')
@basic_auth.required
def index():
    return render_template('index.html')
 
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

            frame = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)

            frame = cv2.resize(frame, (640,480))
            ret, jpeg = cv2.imencode('.jpg', frame)
            if ret:
                yield (b'--frame\r\n'
                       b'Content-Type: image/jpeg\r\n\r\n' + jpeg.tobytes() + b'\r\n\r\n')
            else:
                print("frame is none")

            time.sleep(0.05)
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
    app.run(host='0.0.0.0',port=8080, debug=True, threaded=True)


# http://127.0.0.1:5000/video_feed