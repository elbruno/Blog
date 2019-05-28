import face_recognition
import cv2
import numpy as np

video_capture = cv2.VideoCapture(0)

while True:
    ret, frame = video_capture.read()
    rgb_frame = frame[:, :, ::-1]

    face_landmarks_list = face_recognition.face_landmarks(rgb_frame)
    for face_landmarks in face_landmarks_list:

        for facial_feature in face_landmarks.keys():
            pts = np.array([face_landmarks[facial_feature]], np.int32) 
            pts = pts.reshape((-1,1,2))
            cv2.polylines(frame, [pts], False, (0,255,0))

    cv2.imshow('Video', frame)

    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

video_capture.release()
cv2.destroyAllWindows()
