from robot_hat import Pin, ADC, PWM, Servo, fileDB, Grayscale_Module, Ultrasonic, utils, Music, TTS
import time
import socket
import threading

HEADER = 64
PORT = 5050
SERVER = "0.0.0.0"
ADDR = (SERVER, PORT)
FORMAT = 'utf-8'
DISCONNECT_MESSAGE = "!DISCONNECT"

dir_left = Pin('D4')
pwm_left = PWM('P13')

dir_right = Pin('D5')
pwm_right = PWM('P12')

steering_servo = Servo('P2')

picarx_dir_servo = -7.2

picarx_cam_pan_servo = -10.4

picarx_cam_tilt_servo = -14.4

picarx_dir_motor = [1, 1]

line_reference = [1055, 1088, 768]

cliff_reference = [464, 441, 329]

music = Music()
tts = TTS()

music.music_set_volume(90)

jouer = False
# config_file = '/opt/picar-x/picar-x.conf'
# config = fileDB(config_file, 777, 'root')

# cali_dir_value = config.get('picarx_dir_motor', default_value='[1, 1]')
# cali_dir_value = [int(i.strip()) for i in cali_dir_value.strip().strip('[]').split(',')]

# cali_speed_value = config.get('picarx_speed_motor', default_value='[0, 0]')
# cali_speed_value = [int(i.strip()) for i in cali_speed_value.strip().strip('[]').split(',')]

# dir_cali_val = float(config.get('picarx_dir_servo', default_value=0))

pwm_left.period(4095)
pwm_right.period(4095)
pwm_left.prescaler(10)
pwm_right.prescaler(10)

def jouerHorn():
    music.sound_play('/home/robot/AppRobot/Rasberry/sounds/car-horn.wav')
    time.sleep(0.05)

def jouerMusic():
    music.music_play('/home/robot/AppRobot/Rasberry/musics/music.mp3')
    time.sleep(5)
    music.music_stop()


def move_forward(speed):

    dir_left.low()
    dir_right.high()

    left_speed = speed
    right_speed = speed

    left_speed = max(0, min(100, left_speed))
    right_speed = max(0, min(100, right_speed))

    pwm_left.pulse_width_percent(left_speed)
    pwm_right.pulse_width_percent(right_speed)

def move_backward(speed):

    dir_left.high()
    dir_right.low()


    pwm_left.pulse_width_percent(speed)
    pwm_right.pulse_width_percent(speed)


def turn_right(angle):
    steering_servo.angle(angle)

def turn_left(angle):
    steering_servo.angle(-angle)

def go_straight():
    steering_servo.angle(picarx_dir_servo)

def stop():
    for _ in range(2):
        pwm_left.pulse_width_percent(0)
        pwm_right.pulse_width_percent(0)
        time.sleep(0.002)

def forward():
    go_straight()
    move_forward(90)


def backward():
    go_straight()
    move_backward(90)


def forward_right():
    go_straight()
    turn_right(35)
    move_forward(90)


def forward_left():
    go_straight()
    turn_left(35)
    move_forward(90)


def backward_right():
    go_straight()
    turn_right(35)
    move_backward(90)


def backward_left():
    go_straight()
    turn_left(35)
    move_backward(90)


function_mapping = {
    'forward' : forward,
    'backward' : backward,
    'forward_right' : forward_right,
    'forward_left' : forward_left,
    'backward_left' : backward_left,
    'backward_right' : backward_right,
    'stop' : stop,
    'music' : jouerMusic
}

server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
server.bind(ADDR)

def handle_client(conn, addr):
    print(f"[New Connection] {addr} connected.")
    connected = True
    while connected:
        msg = conn.recv(HEADER).decode(FORMAT)
        if msg:

            if msg == DISCONNECT_MESSAGE:
                connected = False
            else:
                if msg in function_mapping:
                    function_mapping[msg]()
                    send(conn,f"[{addr}] Commande reussite!")
                else:
                    send(conn,f"[{addr}] Connection reussite!")
            print(f"[{addr}] {msg}")

    conn.close()

def start():
    server.listen()
    print(f"[Listening] server is listening on {SERVER}")
    while True:
        conn, addr = server.accept()
        thread = threading.Thread(target=handle_client, args=(conn, addr))
        thread.start()
        print(f"[Active Connections] {threading.activeCount() - 1}")


def send(conn, msg):

    message = msg.encode(FORMAT)

    conn.send(message)


print("[Strating] server is strating")

start()