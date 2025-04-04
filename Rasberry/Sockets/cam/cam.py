import cv2
from threading import Thread
import time

class WebcamVideoStream:
    def __init__(self, src = 0,width = 640,height = 480):
        print("init")
        self.stream = cv2.VideoCapture(src)

        self.stream.set(cv2.CAP_PROP_FOURCC, cv2.VideoWriter_fourcc(*'MJPG'))

        self.stream.set(3,width)
        self.stream.set(4,height)


        (self.grabbed, self.frame) = self.stream.read()
        self.stopped = False
        time.sleep(2.0)

    def start(self):
        print("start thread")
        t = Thread(target=self.update, args=())
        t.daemon = True
        t.start()
        return self

    def update(self):
        print("read")
        while True:
            if self.stopped:
                return

            (self.grabbed, self.frame) = self.stream.read()

    def read(self):
        return self.frame

    def stop(self):
        self.stopped = True