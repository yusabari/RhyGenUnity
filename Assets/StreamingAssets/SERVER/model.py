import os, sys
import librosa
import matplotlib.pyplot as plt
from PIL import Image
import numpy as np
import tensorflow as tf
from keras.preprocessing.image import load_img, img_to_array
from keras.utils import to_categorical
from keras.layers import Lambda
from keras.callbacks import EarlyStopping
from keras.layers import Conv2D, MaxPooling2D, GlobalAveragePooling2D, Dense, Dropout, LSTM, TimeDistributed, Reshape, Flatten, Input, LSTM, Masking, GRU, Embedding, Layer
from keras.models import Sequential
from sklearn.model_selection import train_test_split
from keras.utils import plot_model

def plot_and_save_spectrogram_for_each_measure(audio_path, output_path, bpm):
    # Load the audio file
    y, sr = librosa.load(audio_path)
    
    # Calculate the duration of one beat in seconds
    beat_duration = 60.0 / float(bpm)
    
    # Calculate the number of samples per beat
    samples_per_beat = int(beat_duration * sr)
    
    # Calculate the number of beats in one measure (assuming 4/4 time signature)
    beats_per_measure = 4
    
    # Calculate the number of samples per measure
    samples_per_measure = samples_per_beat * beats_per_measure
    
    # Calculate the total number of measures in the audio
    num_measures = int(len(y) / samples_per_measure)
    
    for i in range(num_measures):
        # Extract the samples for the current measure
        start_sample = i * samples_per_measure
        end_sample = start_sample + samples_per_measure
        y_measure = y[start_sample:end_sample]
        
        # Compute the spectrogram
        D = np.abs(librosa.stft(y_measure))
        DB = librosa.amplitude_to_db(D, ref=np.max)
        
        # Plot the spectrogram
        plt.figure(figsize=(10, 4), frameon=False)
        librosa.display.specshow(DB, sr=sr, x_axis='time', y_axis='log')
        plt.axis("off")
        
        # Save the plot to a PNG file
        png_filename = str(i).zfill(3) + '.png'
        plt.savefig(output_path + png_filename, bbox_inches = 'tight', pad_inches=0)
        plt.close()
        
        # Resize the image to 100x100 using Pillow
        with Image.open(output_path + png_filename) as img:
            img = img.resize((200, 200))
            img.save(output_path + png_filename)
    
    return num_measures

def load_and_preprocess_image(image_path, target_size=(200, 200)):
    # 이미지를 흑백으로 로드
    img = load_img(image_path, color_mode='grayscale', target_size=target_size)
    # 이미지를 numpy 배열로 변환
    img_array = img_to_array(img)
    # 이미지를 [0, 1] 범위로 정규화
    img_array = img_array / 255.0
    return img_array

class STE_RoundingLayer(Layer):
    def __init__(self, trainable=False, **kwargs):
        super(STE_RoundingLayer, self).__init__(**kwargs)
        self.trainable = trainable

    def call(self, inputs):
        # 순전파에서는 반올림, 역전파에서는 identity (기울기 전달)
        return inputs + tf.stop_gradient(tf.round(inputs) - inputs)
def checkLoaded():
    global isLoaded
    return isLoaded

def make_prediction(input_audio, input_bpm):
    num_png = plot_and_save_spectrogram_for_each_measure(input_audio, os.getcwd() + "/tmp/", input_bpm)
    print(f"Number of PNG files created: {num_png}")

    result = ""

    for id in range(num_png):
        img = np.expand_dims(load_and_preprocess_image(os.getcwd() + "/tmp/" + str(id).zfill(3) + ".png"), axis=0)
        stepresult = np.squeeze(model.predict(img, verbose=None))

        for i in range(8):
            if i < 6:
                result += "#" + str(id).zfill(3) + "1" + str(i + 1) + ":"
                # print("#" + str(i).zfill(3) + "1" + str(i + 1) + ":", end="")
            else:
                result += "#" + str(id).zfill(3) + "1" + str(i + 2) + ":"
                # print("#" + str(i).zfill(3) + "1" + str(i + 2) + ":", end="")
            for d in stepresult[i]:
                d = round(d)
                if d == -1: 
                    break
                else:
                    result += str(d).zfill(2)
                    # print(str(d).zfill(2), end='')
            result += "n"
        result += "n"

    return result

custom_objects = {"STE_RoundingLayer": STE_RoundingLayer, "MSE": tf.keras.losses.MeanSquaredError}

with tf.keras.utils.custom_object_scope(custom_objects):
    model_path = os.path.dirname(os.path.abspath(os.path.dirname(__file__))) + '/SERVER/model/model.h5'
    print("Loading TensorFlow model...")
    model = tf.keras.models.load_model(model_path)
    print("Model loaded.")
    print(make_prediction(sys.argv[1], sys.argv[2]))
    exit()