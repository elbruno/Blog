# Bruno Capuano 2020
# display the camera feed using OpenCV

import time
import cv2

# Camera Settings
camera_Width  = 640 # 1024 # 1280 # 640
camera_Heigth = 480 # 780  # 960  # 480
frameSize = (camera_Width, camera_Heigth)
video_capture = cv2.VideoCapture(1)
time.sleep(1.0)

while True:
    ret, frameOrig = video_capture.read()
    frame = cv2.resize(frameOrig, frameSize)
  
    cv2.imshow('@elbruno - Camera', frame)

    # key controller
    key = cv2.waitKey(1) & 0xFF    
    if key == ord("q"):
        break

video_capture.release()
cv2.destroyAllWindows()