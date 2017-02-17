A WPF chat application for [Pilot's Life](http://thepilotslife.com/)

## Advantages over Webchat provided by pilots life:
* **Nick Alert Feature:**  The app has nick alert feature. That means if any one takes your name in chat it will alert you with a beep sound and you can have custom line colors for the lines containing your nickalert. You can have any nickalert you wish and unlimited nickalerts!. 
Can be useful for admins and other peeps. For example if an admin has chat app opened and there is hacker in game and a person calls admin with his/her nickalert then it will alert them and they don't need to be ingame for that!
* **Chat log:** The chat that is done while chat app is opened can be logged. The chat logs are stored in _log/chat.txt_ inside the App folder. And of-course this option is toggle-able.
* **Smoother Experience:** The chat is fetched from API every 0.3 second giving a smooth chat experience where as in the web-chat page its fetched every 5 second.

## Pics of application in action:
Login screen :

<img src="https://i.gyazo.com/13a56ccbed128131338cdd1e9605709c.png" width="500" >

Image of main app with nickalert set as **_'the'_** :

<img src="https://i.gyazo.com/bac18aa111d189f856cd1f5923715a29.png" width="500" >

Settings window:

<img src="https://i.gyazo.com/3f47c800712168218ff84074691540af.png" width="500" >

**You need to click Save button at bottom of nickalert box in settings window after editing/adding/deleting nickalert info. Same goes with delay between fetching online player list, click save button beside the text box to save it.**

## External libraries being used
* [libcurl](http://curl.haxx.se/libcurl/)
* [CurlSharp by masroore](https://github.com/masroore/CurlSharp)
* [Extended WPF toolkit](http://wpftoolkit.codeplex.com/)
* [zlib](http://www.zlib.net/)
* [libssh](https://www.libssh.org/)

## License 
This project is licensed under the terms of the [MIT license](https://en.wikipedia.org/wiki/MIT_License)