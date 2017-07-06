# Syncthing Service Launcher Wrapper

### Welcome to the XMPie - Syncthing Service Launcher Wrapper.

Syncthing is a **continuous file synchronization program**.  
The service launch wrapper, enables a simple activation of Syncthing as a windows service. This capability is important when running Syncthing on windows servers, since they normally operate without an interactive user.

The C# wrapper redirects Syncthing console output to a textual file. 
In addition it sets the Syncthing flags to enable a UI less Syncthing.

## Purpose of the wrapper

1. Provide a simple installation of Syncthing as a service.

2. Provide simple monitoring of Syncthing operation by checking if the service is running.

3. Provide a simple uninstall of the service.

## Things to consider when using this wrapper

1. The wrapper is intended for use on windows servers. It does not provide any additional functionality to Syncthing.

2. Security access is granted according to the Windows user that is running the service. 
   IT can easily configure a user to run the service rather then the dedault Local System identity.

## Building

Building the project can be done either via MSBUILD command line or via opening the solution in Visual Studio 2015 or above.

All output is copied to the SyncthingServiceLauncher output folder. 
Please copy/build the latest Syncthing.exe file to the output folder.

> ## The following files makeup a complete install package
> Syncthing.exe  
> SyncThingServiceLauncher.dll  
> XMPie.Assembly.Utils.dll  
> XMPie.Service.Launcher.exe  
> Install.bat  
> Uninstall.bat  

## Installation

Copy the generated output folder to **%ProgramFiles%\Syncthing** or any other prefered location.  
Run the **Install.bat** file
Configure Syncthing with the web UI.  
Default url is [127.0.0.1:8384](http://127.0.0.1:8384/).  
Check the log.txt file for **GUI and API** if default url does not work.

## Monitoring

Open the services snap-in, (services.msc) and located the "Syncthing Service".
Verify that the service is running.  
If the service is stopped for some reason, make sure that the Log-on user is set up correctly.

## Uninstall

Go to the installation folder  
Run the **Uninstall.bat** file