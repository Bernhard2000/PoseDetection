import time

import cv2
import mediapipe as mp
from statistics import mean
import numpy as np
from matplotlib import pyplot as pit
import time
import socket
import queue

clientSock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
UDP_IP_ADDRESS = "127.0.0.1"
UDP_PORT_NO = 64039
Message = "0"

class poseDetector():
    def __init__(self, mode= False, complexity = 0, smooth = True, detectionConfidence = 0.7, trackingConfidence = 0.4):
        self.mode = mode
        self.complexity = complexity
        self.smooth = smooth
        self.detectionConfidence = detectionConfidence
        self.trackingConfidence = trackingConfidence
        self.mpPose = mp.solutions.pose
        self.mpDraw = mp.solutions.drawing_utils
        self.pose = self.mpPose.Pose(self.mode, self.complexity, self.smooth, self.detectionConfidence, self.trackingConfidence)

    def findPose(self, img, draw = True):
        imgRGB = cv2.cvtColor(img, cv2.COLOR_BGR2RGB)
        self.results = self.pose.process(imgRGB)
        if self.results.pose_landmarks:
            if draw:
                if(self.results.pose_landmarks):
                    self.mpDraw.draw_landmarks(img, self.results.pose_landmarks, self.mpPose.POSE_CONNECTIONS)
                    #self.mpDraw.draw_landmarks(img, self.results.face_landmarks, self.mpPose.FACE_CONNECTIONS)
                    #self.mpDraw.draw_landmarks(img, self.results.left_hand_landmarks, self.mpPose.HAND_CONNECTIONS)
                    #self.mpDraw.draw_landmarks(img, self.results.right_hand_landmarks, self.mpPose.HAND_CONNECTIONS)


        return img

    def findPosition(self, img, draw=True):
        lmList = []
        if self.results.pose_landmarks:
            for id, lm in enumerate(self.results.pose_landmarks.landmark):
                h, w, c = img.shape
                cx, cy = lm.x * h, lm.y * w
                lmList.append([id, lm.x, lm.y, lm.z])
                if draw:
                    cv2.circle(img, (cx,cy), 10, (255,0,0), cv2.FILLED)
        return lmList


def main():
    cap = cv2.VideoCapture(0)
    detector = poseDetector()

    startTime = 0
    stopTime = 0
    frametimes = []
    frametimes.append(30)

    while True:
        startTime = time.time()
        if len(frametimes) > 20:
            del frametimes[0]
        success, img = cap.read()
        if success:
            img = detector.findPose(img)
            lmList = detector.findPosition(img, False)
            print(lmList)
            fps = 1 / mean(frametimes)
            cv2.putText(img, str(int(fps)), (70, 50), cv2.FONT_HERSHEY_PLAIN, 3, (255, 0, 0), 3)
            cv2.imshow("Image", img)

            Message = ""
            for i in lmList:
                values = [str(j) for j in i]
                for j in values:
                    Message = Message + j + ','
                Message = Message[:len(Message) - 1]
                Message = Message + ';'


            clientSock.sendto(Message.encode(), (UDP_IP_ADDRESS, UDP_PORT_NO))
            stopTime = time.time()
            frametimes.append((stopTime - startTime) + 0.001)
        cv2.waitKey(1)


if __name__ == "__main__":
    main()
