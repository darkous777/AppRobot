# import pigpio
# import time

# pi = pigpio.pi()

# MOTOR1_DIR = 23
# MOTOR2_DIR = 24

# MOTOR1_PWM = 13
# MOTOR2_PWM = 12


# pi.set_mode(MOTOR1_DIR, pigpio.OUTPUT)
# pi.set_mode(MOTOR1_PWM, pigpio.OUTPUT)
# pi.set_mode(MOTOR2_DIR, pigpio.OUTPUT)
# pi.set_mode(MOTOR2_PWM, pigpio.OUTPUT)


# def move_forward(speed =128):
#     pi.write(MOTOR1_DIR, 1)
#     pi.set_PWM_dutycycle(MOTOR1_PWM, speed)


#     pi.write(MOTOR2_DIR, 1)
#     pi.set_PWM_dutycycle(MOTOR2_PWM, speed)

# def stop():

#     pi.set_PWM_dutycycle(MOTOR1_PWM, 0)
#     pi.set_PWM_dutycycle(MOTOR2_PWM, 0)

# try:
#     move_forward()
#     time.sleep(2)
#     stop()

# finally:
#     pi.stop()

from robot_hat import Pin, ADC, PWM, Servo, fileDB, Grayscale_Module, Ultrasonic, utils
import time




dir_left = Pin('D4')
pwm_left = PWM('P13')

dir_right = Pin('D5')
pwm_right = PWM('P12')

steering_servo = Servo('P2')

pwm_left.period(4095)
pwm_right.period(4095)
pwm_left.prescaler(10)
pwm_right.prescaler(10)



def move_forward(speed):

    dir_left.low()
    dir_right.high()


    pwm_left.pulse_width_percent(speed)
    pwm_right.pulse_width_percent(speed)

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
    steering_servo.angle(0)

def stop():

    pwm_left.pulse_width_percent(0)
    pwm_right.pulse_width_percent(0)

go_straight()
move_forward(90)
time.sleep(1)

turn_left(30)
move_forward(90)
time.sleep(1)

turn_right(30)
move_forward(90)
time.sleep(1)

stop()


# MOTOR1_DIR = 'D5'
# MOTOR2_DIR = 'D4'

# MOTOR1_PWM = 'P13'
# MOTOR2_PWM = 'P12'


# GPIO.setmode(GPIO.BCM)
# GPIO.setup(MOTOR1_DIR, GPIO.OUT)
# GPIO.setup(MOTOR2_DIR, GPIO.OUT)

# motor1_pwm = PWMOutputDevice(MOTOR1_PWM)
# motor2_pwm = PWMOutputDevice(MOTOR2_PWM)


# def move_forward(speed = 0.5):
#     GPIO.output(MOTOR1_DIR, GPIO.HIGH)
#     motor1_pwm.value = speed


#     GPIO.output(MOTOR2_DIR, GPIO.HIGH)
#     motor2_pwm.value = speed

# def stop():

#     motor1_pwm.value = 0
#     motor2_pwm.value = 0

# try:
#     move_forward(0.9)
#     time.sleep(2)
#     stop()

# finally:
#     GPIO.cleanup()
