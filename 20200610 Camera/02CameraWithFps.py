# Bruno Capuano 2020
# display the camera feed using OpenCV
# display FPS

import time
import cv2

# Camera Settings
camera_Width  = 640 # 1024 # 1280 # 640
camera_Heigth = 480 # 780  # 960  # 480
frameSize = (camera_Width, camera_Heigth)
video_capture = cv2.VideoCapture(1)
time.sleep(1.0)

while True:
    start_time = time.time()
    ret, frameOrig = video_capture.read()
    frame = cv2.resize(frameOrig, frameSize)
  
    if (time.time() - start_time ) > 0:
        fpsInfo = "FPS: " + str(1.0 / (time.time() - start_time)) # FPS = 1 / time to process loop
        font = cv2.FONT_HERSHEY_DUPLEX
        cv2.putText(frame, fpsInfo, (10, 20), font, 0.4, (255, 255, 255), 1)

    cv2.imshow('@elbruno - Camera FPS', frame)

    # key controller
    key = cv2.waitKey(1) & 0xFF    
    if key == ord("q"):
        break

video_capture.release()
cv2.destroyAllWindows()