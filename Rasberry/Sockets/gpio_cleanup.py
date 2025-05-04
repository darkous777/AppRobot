import os
import time
import RPi.GPIO as GPIO

print("[CLEANUP] Libération des GPIO...")
try:
    GPIO.setmode(GPIO.BCM)
    GPIO.cleanup()
    print("[CLEANUP] GPIO libérés avec succès.")
except Exception as e:
    print(f"[CLEANUP] Erreur lors du cleanup : {e}")
