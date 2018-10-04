# metar

METAR is simple software which runs in the background which uses the https://avwx.docs.apiary.io API to query any airport code of your choice. Once queried, it checks to ensure that it's not going to double over by removing the metadata information which reflects when you last queried, and then it will read out the METAR information over your Default Playback Device. If you change your default playback device while the software is running, you'll need to relaunch it.

Feel free to head over to the release tab and give it a shot - make your necessary configuration changes (There isn't much - mainly due to the fact that this software is stupidly simple and has pretty code).

https://github.com/cainkilgore/metar/releases
