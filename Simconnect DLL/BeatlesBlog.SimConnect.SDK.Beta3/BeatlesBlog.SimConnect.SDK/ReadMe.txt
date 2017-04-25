
                 BeatlesBlog.SimConnect.SDK Beta 2 ReadMe File




                          *** Legal Disclaimer ***

    Microsoft Corp has no connection with this SDK product, or any of its 
        contents.  This product, and its contents, are Copyright 2009 
                               Beatle's Blog.

                        *** End Legal Disclaimer ***




Welcome to Beta 2

Fixes/changes since Beta 1

 * IsLocalRunning() function would fault on any machine that hadn't also ran ESP 
      v1 at some point - sorry about that :->
 * Returned data that contained a dwoutof or dwArrayCount value wasn't being
      handled correctly when there was no data, resulting in an array out of 
      bounds error
 * Fixed the code that receives facility data to correctly allocate the 
      SIMCONNECT_DATA_FACILITY_Xxx objects within the returned array
 * Added sanity checks to the data parsing code to check for "shortened" return
      structures.
 * Changed the void return type on almost all API functions to be an Int32.  The
      return value is the PacketID of the packet sent for this API call.


Overview

Enclosed is the SDK for the Beatle's Blog Managed SimConnect client library.
This SDK can be used to build both Desktop clients (Console, WinForm, & WPF) and
SilverLight clients.  The client libraries included are built using the Any CPU
setting, so you should use this setting in your own projects, unless some other
requirement of your client needs a different setting.  The client libraries 
included are written in 100% managed code with no depencencies on any existing
libraries/DLLs/etc (except for a minimal number of .Net FrameWork libraries).

Benefits of new client library:

 * No reliance on SimConnect.MSI or the native client library it installs.
 * Ability to compile projects using the Any CPU build setting.
 * Ability to build SilverLight based web clients.
 * Simpler Data Structure Definition/Registration system using custom attributes.
 * Overloads provided for those APIs where appropriate allowing use of default
      values like the native library has.
 * Ability to pass an instance object to a RequestDataOnSimObject call, allowing
      the instance to be auto updated as new requested data is received.
 * Working support for Client Data API functions
 * No SimConnect.CFG file required/used, Server Name and Port provided in the
      SimConnect.Open(...) call.
 * In general, more .Net friendly :->.
 * Uses a Ships-With model i.e. you ship the library with your application, 
      installed in the same directory as your application.  An alternate option
      is to use something like ILMerge.exe to embed the client library directly
      into your application.
 * Client libraries are compatible with FSX SP2, FSX + Acceleration pack, and
      ESP v1.  Client libraries will NOT connect to FSX RTM or FSX SP1.




A note on Silverlight Support


The SilverLight runtime imposes several restrictions on socket communications.  
SilverLight applications can only open a port in the range 4502 through 4534, 
so you will need to modify your SimConnect.XML file to open one or more IPv4 
ports in this range.  Here is a sample SimConnect.XML file:

<?xml version="1.0" encoding="Windows-1252"?>

<SimBase.Document Type="SimConnect" version="1,0">
  <Descr>SimConnect Server Configuration</Descr>
  <Filename>SimConnect.xml</Filename>
  <Disabled>False</Disabled>

  <!-- Global (remote) IPv4 Server Configuration (SilverLight compatible) -->
  <SimConnect.Comm>
    <Disabled>False</Disabled>
    <Protocol>IPv4</Protocol>
    <Scope>global</Scope>
    <MaxClients>64</MaxClients>
    <Address>0.0.0.0</Address>
    <Port>4504</Port>
  </SimConnect.Comm>

</SimBase.Document>


Another restriction is that in order to make a cross-domain connection (which 
would be any SimConnect connection), the machine you are trying to connect to 
(the machine that Flight Simulator/ESP is running on) must be running a 
Silverlight Policy File server on port 943.  See the Samples section below for
a simple Policy File server that fills this requirement.




Client Library Files


Libs\BeatlesBlog.SimConnect.DLL

This is the main desktop version of the client library built using .Net 3.5.  If
you are building a desktop application, this is the version you most likely want
to use.



Libs\BeatlesBlog.SimConnect20.DLL

This is an alternate version of the desktop client built using .Net 2.0.  This
version does not include Named Pipe support (as .Net 2.0 doesn't provide this
support).  If some other requirement of your application limits you to only
using .Net 2.0, then you should use this version of the library instead.



Libs\BeatlesBlog.SimConnectSL.DLL

This is a SilverLight version of the Client library built using SilverLight 2.0,
but the library should be usable in a SilverLight 3.0 project.




Solution Files


Samples\SamplesBasic.sln 

Solution file contains the 3 basic desktop samples, one each for Console, 
WinForm, and WPF.  This solution file is compatible with the Visual Studio C# 
Express product and doesn't require any additional items be installed in order 
to build.



Samples\Samples.sln 

Solution file contains all of the available sample projects.  Some of these have 
extra requirements (SilverLight tools, Silverlight ToolKit, Virtual Earth 
SilverLight Map Control CTP, Virtual Earth 3D Map control).




Basic Sample List (those in SamplesBasic.sln)


Samples\Console\SimConnectTestConsole

This sample uses the MENU version of the Text(...) function to display a 
heirarchical set of selection dialogs that allow testing several SimConnect
functions.  Built using the .Net framework 3.5.



Samples\WinForm\SimConnectTestWinForm

This sample displays all of the Aircraft (User and AI) in a detail mode ListView
showing Lat, Lon, Alt, Pitch, Bank, Heading.  Menu allows connecting to either a
local SimConnect instance (if one is available) or a remote SimConnect instance.
Built using the .Net framework 3.5.



Samples\WPF\SimConnectTestWPF

This sample displays all of the Aircraft (User and AI) in a data bound ListBox
using a custom DataTemplate to display Lat, Lon, Alt, Pitch, Bank, Heading in 
Degrees and Radians/Feet and Meters.  Built using the .Net frameowrk 3.5.




Enhanced Sample List (those only in Samples.sln)


Samples\WinForm\WinForm20Map

This sample is a WinForm application built using .Net 2.0 and the 
BeatlesBlog.SimConnect20.DLL version of the client library.  This sample also
uses a Virtual Earth 3D (VE3D) map control, which you can install by going to 
http://www.bing.com/maps and clicking on the 3D button in the map nav bar.  Once
connected, the application creates a custom Camera Controller that provides a 
psuedo "cockpit view" of the user aircraft Lat, Lon, Alt, Pitch, Bank, Heading.



Samples\WPF\ClientDataTest

This sample shows how to use the Client Data API functionality in the client 
library.  Built using .Net 3.5



Samples\SilverLight\SimConnectTestSilverlight

This sample is basically the same as the SimConnectTestWPF sample, but written
as a SilverLight application.  There is also an associated ASP.Net webpage 
project.  Built using SilverLight 2.0.



Samples\SilverLight\MovingMapSL\MovingMapSL

This sample uses the CTP Virtual Earth Silverlight Map control, available from
http://connect.microsoft.com.  There is also an associated ASP.Net webpage 
project.  Built using SilverLight 2.0.



Samples\SilverLight\SilverLightPolicyServer

This is the sample Silverlight Policy Server mentioned above.  You must be 
running this, or a similar utility, on the same machine as Flight Simulator / 
ESP in order for Silverlight Clients to be able to connect to it.




Documentation

At the moment, the documentation consist of this readme file and the sample
projects included in this SDK.  Please keep an eye on my blog
(http://beatlesblog.spaces.live.com) for future posts related to the new client
library.  Please use the FSDeveloper SimConnect forum 
(http://www.fsdeveloper.com/forum/forumdisplay.php?f=60) for questions, 
suggestions, and bug reports.