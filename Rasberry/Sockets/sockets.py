import socket
import threading

HEADER = 64
PORT = 5050
SERVER = socket.gethostbyname(socket.gethostname())
ADDR = (SERVER, PORT)
FORMAT = 'utf-8'
DISCONNECT_MESSAGE = "!DISCONNECT"

server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
server.bind(ADDR)


def handle_client(conn, addr):
    print(f"[New Connection] {addr} connected.")
    connected = True
    while connected:
        msg_length = conn.recv(HEADER).decode(FORMAT)
        if msg_length:

            send(conn,f"[{addr}] Connection réussite!")
            if msg_length == DISCONNECT_MESSAGE:
                connected = False
            print(f"[{addr}] {msg_length}")

    conn.close()

def start():
    server.listen()
    print(f"[Listening] server is listening on {SERVER}")
    while True:
        conn, addr = server.accept()
        thread = threading.Thread(target=handle_client, args=(conn, addr))
        thread.start()
        print(f"[Active Connections] {threading.activeCount() - 1}")



def send(conn,msg):
    message = msg.encode(FORMAT)

    conn.send(message)
print("[Strating] server is strating")
start()