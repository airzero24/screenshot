# Screenshot Utility
This is a quick project I threw together for an assessment. Needed a .NET assembly to take screenshots and have the ability to set an interval and take screenshots for a specified timer, and then zip up all the files and delete the originals. This is a combination of some code from [@djhohnstein](https://twitter.com/djhohnstein) and [@CptJesus](https://twitter.com/CptJesus)'s [zipstuff](https://github.com/rvazarkar/ZipStuff).

Screenshot files are saved in the format of `<yyyyMMddHHmmss>-<GUID>.jpeg` in the directory `C:\Users\<username>\AppData\Local\Temp`. When using a timer, all the screenshot files will be collected and zipped up into a file named `<yyyyMMdd>.zip` in the same Temp directory.
