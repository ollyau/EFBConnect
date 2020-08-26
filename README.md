# EFB Connect
A utility that shares Microsoft Flight Simulator position, attitude, and traffic information with ForeFlight.

## Compatibility
EFB Connect is compatible with Microsoft Flight Simulator (2020), Microsoft Flight Simulator X, Microsoft ESP, and Lockheed Martin Prepar3D.  EFB Connect sends UDP packets on port 49002 as described in the [ForeFlight - Simulator GPS, Traffic, Attitude Integration](https://www.foreflight.com/support/network-gps/) documentation and should be compatible with ForeFlight v5.2 and newer.

## Prerequisites
EFB Connect targets the [.NET Framework 4.5](https://www.microsoft.com/en-us/download/details.aspx?id=42643) and has no other prerequisites.  EFB Connect uses Tim Gregson's Managed SimConnect SDK and does not require FSUIPC.

## Usage
To begin using EFB Connect, simply launch the program's executable while Flight Simulator is running.  EFB Connect will establish a SimConnect connection upon launch and begin broadcasting data immediately.

As an alternative to using a UDP broadcast, there is an option to select your device's IP address to only send packets to a single device.
