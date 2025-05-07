from robot_hat import Pin, ADC, PWM, Servo, fileDB, Grayscale_Module, Ultrasonic, utils, Music, TTS
from picarx import Picarx
import time
import socket
import threading
import pygame
import subprocess
import requests
import os

from gpiozero import Device 
from gpiozero.pins.pigpio import PiGPIOFactory
Device.pin_factory = PiGPIOFactory()

# Initialisation sécurisée de l'audio pygame
try:
    os.environ["SDL_AUDIODRIVER"] = "pulseaudio"
    pygame.mixer.init()
    pygame.mixer.music.load("/home/robot/AppRobot/Rasberry/musics/rollin.mp3")
except pygame.error as e:
    print(f"[AUDIO] Impossible d'initialiser l'audio : {e}")
    print("[AUDIO] Passage en mode silencieux (dummy audio).")
    os.environ["SDL_AUDIODRIVER"] = "dummy"
    try:
        pygame.mixer.init()
    except Exception as e2:
        print(f"[AUDIO] Échec du mode dummy aussi : {e2}")




HEADER = 64
PORT = 5050
SERVER = "0.0.0.0"
ADDR = (SERVER, PORT)
FORMAT = 'utf-8'
DISCONNECT_MESSAGE = "!DISCONNECT"


active_conn = None

trig = 'D2'
echo = 'D3'

# ultrasonic = Ultrasonic(Pin(trig), Pin(echo, mode=Pin.IN, pull=Pin.PULL_DOWN))

px = Picarx()

SafeDistance = 40
DangerDistance = 20

is_moving = False
last_state = None

picarx_dir_servo = -7.6
picarx_cam_pan_servo = -10.4
picarx_cam_tilt_servo = -14.4
picarx_dir_motor = [1, 1]

line_reference = [1055, 1088, 768]
cliff_reference = [464, 441, 329]

line_following = False
line_thread = None
last_state = "stop"
last_seen_direction = None

# music = Music()

# music.music_set_volume(90)

# config_file = '/opt/picar-x/picar-x.conf'
# config = fileDB(config_file, 777, 'root')

# cali_dir_value = config.get('picarx_dir_motor', default_value='[1, 1]')
# cali_dir_value = [int(i.strip()) for i in cali_dir_value.strip().strip('[]').split(',')]

# cali_speed_value = config.get('picarx_speed_motor', default_value='[0, 0]')
# cali_speed_value = [int(i.strip()) for i in cali_speed_value.strip().strip('[]').split(',')]

# dir_cali_val = float(config.get('picarx_dir_servo', default_value=0))


flask_process = None

def start_flask_cam():
    """Start the Flask camera server in a subprocess."""
    global flask_process
    if flask_process is None:
        flask_process = subprocess.Popen(
            ["python3", "/home/robot/AppRobot/Rasberry/Sockets/cam/flask_cam.py"],
            stdout=subprocess.PIPE,
            stderr=subprocess.PIPE
        )
        print("[INFO] Flask camera server started.")
        time.sleep(2)

def stop_flask_cam():
    """Gracefully stop the Flask camera server."""
    global flask_process
    if flask_process is not None:
        try:
            requests.get("http://localhost:8080/shutdown", auth=('pi', 'pi'))
            flask_process.terminate()
            flask_process.wait()
        except Exception as e:
            print(f"[ERROR] Failed to stop Flask server: {e}")
        finally:
            flask_process = None
            print("[INFO] Flask camera server stopped.")

def go_straight():
    px.set_dir_servo_angle(picarx_dir_servo)


def getDistance():
    global last_state, active_conn
    while True:
        if is_moving:

            distance = round(px.ultrasonic.read())
            while distance == -2:
                time.sleep(0.05)
                distance = round(px.ultrasonic.read())

            print("distance: ",distance)

            if distance < DangerDistance and last_state != "danger":
                backward()
                time.sleep(1)
                stop()

                last_state = "danger"

            elif distance <= SafeDistance and last_state != "warning":
                if active_conn:
                    send(active_conn, "Attention, zone de danger!")
                last_state = "warning"

            elif distance >= SafeDistance and last_state != "safe":
                last_state = "safe"

        time.sleep(0.1)


distance_thread = threading.Thread(target=getDistance, daemon=True)
distance_thread.start()

def start_moving():
    global is_moving
    is_moving = True

def stop_moving():
    global is_moving
    is_moving = False

def jouerHorn():
    # music.sound_play('/home/robot/AppRobot/Rasberry/sounds/car-horn.wav')
    time.sleep(0.05)

def jouerMusic():

    pygame.mixer.music.play()

def arreterMusic():

    pygame.mixer.music.stop()
    # music.music_play('/home/robot/AppRobot/Rasberry/musics/music.mp3')
    # time.sleep(5)
    # music.music_stop()


def follow_line():
    global line_following, last_state

    print("[SUIVI] Suivi de ligne démarré.")
    time_lost = 0
    lost_threshold = 1.5  

    while line_following:
        gm_val_list = px.get_grayscale_data()
        gm_state = px.get_line_status(gm_val_list)

        print(f"[SUIVI] gm_val_list = {gm_val_list}, gm_state = {gm_state}")

        if gm_state == [0, 0, 0]:
            time_lost += 0.05
            print(f"[SUIVI] Ligne perdue depuis {time_lost:.2f}s")

            if time_lost >= lost_threshold:
                print("[SUIVI] Ligne définitivement perdue. Arrêt.")
                line_following = False
                stop()
                if active_conn:
                    send(active_conn, "Suivi interrompu : ligne perdue.")
                break

            # tentative de récupération
            if last_seen_direction == 'right':
                px.set_dir_servo_angle(20)
                move_backward()
            elif last_seen_direction == 'left':
                px.set_dir_servo_angle(-20)
                move_backward()
            else:
                px.set_dir_servo_angle(0)
                move_backward()

            time.sleep(0.05)
            continue
        else:
            time_lost = 0  # ligne retrouvée

        if gm_state[1] == 1 and gm_state[0] == 0 and gm_state[2] == 0:
            px.set_dir_servo_angle(0)
            move_forward()
            last_state = 'forward'
            last_seen_direction = "center"
        elif gm_state[0] == 1 and gm_state[1] == 0:
            px.set_dir_servo_angle(-20)
            move_forward()
            last_state = 'right'
            last_seen_direction = "left"
        elif gm_state[2] == 1 and gm_state[1] == 0:
            px.set_dir_servo_angle(20)
            move_forward()
            last_state = 'left'
            last_seen_direction = "right"
        else:
            # ligne probablement centrée ou en fourche : avancer droit par défaut
            px.set_dir_servo_angle(0)
            move_forward()
            last_state = 'forward'
            last_seen_direction = "center"

        time.sleep(0.05)

    print("[SUIVI] Thread de suivi de ligne terminé.")

def start_line_following():
    global line_following, line_thread
    if not line_following:
        line_following = True
        line_thread = threading.Thread(target=follow_line)
        line_thread.daemon = True
        line_thread.start()

def stop_line_following():
    global line_following
    line_following = False
    stop()

def move_forward():
    start_moving()

    px.set_motor_speed(1, 10) # ici on compense la vitesse du moteur gauche parce que sinon pour une raison X la roue gauche tourne moin vite que la roue droite
    px.set_motor_speed(2, -9) # on inverse la vitesse pour que le moteur tourne dans l'autre sens parce que les moteurs sont placés en miroir sur le chassis

def move_backward():
    start_moving()

    px.set_motor_speed(1, -10)
    px.set_motor_speed(2, 9)

def move_rotation_left():
    start_moving()

    px.set_motor_speed(1, -10)
    px.set_motor_speed(2, -9)


def move_rotation_right():
    start_moving()

    px.set_motor_speed(1, 10)
    px.set_motor_speed(2, 9)

def turn_right(angle):
    px.set_dir_servo_angle(angle)

def turn_left(angle):
    px.set_dir_servo_angle(-angle)


def stop():
    stop_moving()

    px.set_dir_servo_angle(0)
    px.set_motor_speed(1, 0)
    px.set_motor_speed(2, 0)

def forward():
    go_straight()
    move_forward()


def backward():
    go_straight()
    move_backward()


def forward_right():
    go_straight()
    turn_right(28)
    move_forward()


def forward_left():
    go_straight()
    turn_left(28)
    move_forward()


def backward_right():
    go_straight()
    turn_right(28)
    move_backward()


def backward_left():
    go_straight()
    turn_left(28)
    move_backward()


def rotation_left():
    go_straight()
    turn_left(40)
    move_rotation_left()


def rotation_right():
    go_straight()
    turn_right(40)
    move_rotation_right()



function_mapping = {
    'forward' : forward,
    'backward' : backward,
    'forward_right' : forward_right,
    'forward_left' : forward_left,
    'backward_left' : backward_left,
    'backward_right' : backward_right,
    'stop' : stop,
    'music_on' : jouerMusic,
    'music_off' : arreterMusic,
    'rotation_left' : rotation_left,
    'rotation_right' : rotation_right,
    'camera_on': start_flask_cam,
    'camera_off': stop_flask_cam,
    'start_follow_line' : start_line_following,
    'stop_follow_line' : stop_line_following
}

server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
server.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
server.bind(ADDR)

def handle_client(conn, addr):
    print(f"[New Connection] {addr} connected.")
    global active_conn
    active_conn = conn

    connected = True
    while connected:
        msg = conn.recv(HEADER).decode(FORMAT)
        if msg:

            if msg == DISCONNECT_MESSAGE:
                connected = False
            else:
                send(conn,"-_-")
                if msg in function_mapping:
                    function_mapping[msg]()
                else:
                    send(conn,"Salut c'est moi ton robot prefere!")



            print(f"[{addr}] {msg}")

    conn.close()

def start():
    server.listen()
    print(f"[Listening] server is listening on {SERVER}")
    print("[READY] Serveur TCP actif, en attente de commandes")

    while True:
        conn, addr = server.accept()
        thread = threading.Thread(target=handle_client, args=(conn, addr))
        thread.start()
        print(f"[Active Connections] {threading.activeCount() - 1}")


def send(conn, msg):

    message = msg.encode(FORMAT)
    conn.send(message)
    time.sleep(0.1)


print("[Strating] server is strating")

start()