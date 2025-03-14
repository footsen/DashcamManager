
Please write a C# application to manage my dashcam's stored files. I'll manage it using MS Visual Studio 2022. The script can use http calls to query the dashcam. The dashcam's IP is 192.168.5.36.

It needs these 3 functions: 
List all files sorting by file creating time (command 3015), and let me select which ones to download or delete.
Download selected files, and save them to d:\localdashcam. show a progress bar while downloading.
Delete selected files (command 4003).

Here's the command for listing files:
List all files sorting by file creating time. This command only is executed in playback mode only.
Command: http://192.168.5.36/?custom=1&cmd=3015
Parameter: Null

Here's the command to delete a file:
Command: http://192.168.5.36/?custom=1&cmd=4003&str=[path to mp4]
Parameter: string format would be the same as file path


Jesse,

The Cuba doc says that I shouldn't bring any personal electronic items. I'm concerned about being semi-offline for 2.5 months, considering that many of my home accounts are secured with Google Authenticator and my main personal email is through a Microsoft work subscription (chriscorlett.com domain), which I don't think will play nice with OpenNet.  

What do people who are posted there do? I can't imagine that they forego electronics for a full tour. Maybe there's a way to use my laptop at the embassy, or agree to carry it with me everywhere I go? Or I could buy a cheap laptop and plan to reimage it when I depart (that was a recommended approach when I posted in Beijing).

Is there anyone I can ask for more information about this?

Thanks,
Chris



This is an application that lets you manage your Viofo dashcam's video files. I developed it for use with my A229 Pro dashcam.

Features:

Loads a list of all of the movies and photos on your dashcam, including front and rear, parking front and rear, and "locked" videos as well. I don't have an interior camera, but I expect that it would pick these videos up as well. It also handles photos that you take with the dashcam.

Clicking on column headers lets you sort the list by file name, video date/time, and file size. 

Select one or more files and press the download button to copy these files to your local PC.

Select one or more files and press the delete button to remove them from your dashcam's SD card.

Getting started:

Connect on your home network using the dashcam's "Wi-Fi Station Mode". You need to enable station IP settings in the mobile app first to let the cam connect to your network. Then turn on Wi-Fi Station Mode by pressing and holding the MIC + WiFi buttons together for a few seconds. The dashcam will announce that station mode Wi-Fi is enabled. Pro tip: run an extension cord with a USB charger to your car, and connect your dashcam through that. Then you can play with Station Mode and not have to worry about draining your battery!



Retrieving the list of files and videos is done using this command:
http://[dashcam IP]/?custom=1&cmd=3015

Deleting a file is done using this command:
http://[dashcam IP]/?custom=1&cmd=4003&str=[path to file being deleted]

Good post:
https://dashcamtalk.com/forum/threads/how-to-access-the-a129-over-wi-fi-without-the-viofo-app.37279/post-445948






I am creating a windows forms application using Visual Studio 2022. There are two a couple of 







