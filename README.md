JFTP
====

JFTP (JSON File Transfer Protocol) is intended as a less painful replacement for good old FTP, particularly in situations where you would have to build the FTP client into your application. The basic idea is that a server provides file information as a JSON string and you then download the file (either all at once or bit by bit) via HTTP. I suppose it might be less efficient, but would you rather wait an extra couple milliseconds for a file to load or an extra month for trying to make FTP work in your application?

I have mostly been working on the server so far. The last push involved setting up authentication methods set up (currently, we have Nancy's Forms and Stateless authentication modes working), and the next one will--theoretically--have them hooked up to a data store of some kind that can be used to run the server.

Once that's out of the way, the intent is to create a simple library to communicate with the server.
