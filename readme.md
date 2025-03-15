This is an application that lets you manage your Viofo dashcam's video files. I developed it for use with my A229 Pro dashcam.

Features:

Loads a list of all of the movies and photos on your dashcam, including front and rear, parking front and rear, and "locked" videos as well. I don't have an interior camera, but I expect that it would pick these videos up as well. It also handles photos that you take with the dashcam.

Clicking on column headers lets you sort the list by file name, video date/time, and file size. 

Select one or more files and press the download button to copy these files to your local PC.

Select one or more files and press the delete button to remove them from your dashcam's SD card.

Getting started:

Install the app. 

Connect on your home network using the dashcam's "Wi-Fi Station Mode". You need to enable station IP settings in the mobile app first to let the cam connect to your network. Then turn on Wi-Fi Station Mode by pressing and holding the MIC + WiFi buttons together for a few seconds. The dashcam will announce that station mode Wi-Fi is enabled. Pro tip: run an extension cord with a USB charger to your car, and connect your dashcam through that. Then you can play with Station Mode and not have to worry about draining your battery!

You can choose the IP address for your dashcam, and the folder where you'll store downloaded videos and photos:

![Alt text](https://github.com/user-attachments/assets/b511ecc4-c5a1-4750-a6b6-e0fe2c2ce6f2)

The main application window lets you select videos and photos, then download or delete them.

![Alt text](https://github.com/user-attachments/assets/adcd0320-f583-4f11-9a51-18cf0e08f5d3)


Retrieving the list of files and videos is done using this command:
http://[dashcam IP]/?custom=1&cmd=3015

Deleting a file is done using this command:
http://[dashcam IP]/?custom=1&cmd=4003&str=[path to file being deleted]

Good post that describes possible http commands:
https://dashcamtalk.com/forum/threads/how-to-access-the-a129-over-wi-fi-without-the-viofo-app.37279/post-445948



