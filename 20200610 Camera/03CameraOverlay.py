# Bruno Capuano 2020
# display the camera feed using OpenCV
# add a bottom image overlay, using a background image

import time
import cv2

# Camera Settings
camera_Width  = 640 # 1024 # 1280 # 640
camera_Heigth = 480 # 780  # 960  # 480
frameSize = (camera_Width, camera_Heigth)
video_capture = cv2.VideoCapture(1)
time.sleep(1.0)


# load bottom img
background = cv2.imread('Bottom03.png')
background = cv2.resize(background, frameSize)

while True:
    ret, frameOrig = video_capture.read()
    frame = cv2.resize(frameOrig, frameSize)

    img = cv2.addWeighted(background, 1, frame, 1, 0)
   
    cv2.imshow('@elbruno - Logo Overlay', img)

    # key controller
    key = cv2.waitKey(1) & 0xFF    
    if key == ord("q"):
        break


video_capture.release()
cv2.destroyAllWindows()