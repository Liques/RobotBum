# RobotBum
RobotBum is application that tracks airplanes and sends messages to Twitter or a webserver. Works on Windows, Linux and Mac OS X, using .NET Core.

![alt tag](https://github.com/Liques/RobotBum/raw/master/example.PNG)


Note: The application, for a while, is not running on RaspberryPI devices, even if it's running the latest versions of Windows 10 or Linux. The framework we are using (.NET Core) is not supporting ARM devices yet, but they will release support to ARM devices next months.

 - [Download page](https://github.com/Liques/RobotBum/releases)

##How to Install

###Step 1: 

If you don't have a ADS-B reciever or you already have an instance running ModeSMixer2, even if it's not running in your machine, please skip to the step 2.

If you have your own ADS-B reciever you have to install ***ModeSMixer2***. This program, as their website says, has ability to receive data across the network from one or more sources, such as instances of dump1090, rtl1090, modesdeco2, ADSB# or any other Mode S decoding application. This program is free, run in most operational systems. 

- [Download link page for ModeSMixer2](http://xdeco.org/?page_id=30)
- [How to install and run ModeSMixer2](http://xdeco.org/?page_id=48)

###Step 2: 

1. Make sure that you already have an instance ModeSMixer2 running and the machine where you are installing Robot Bum have access to it webserver.
2. Download the lastest code of RobotBum or clone this repository 
3. Unzip the file.
4. If you are on windows, open DOS Command Prompt or if you are on Linux or Mac OS, make sure that you are on terminal or on a SSH client.
5. Navigate to the folder that you unziped.

###Step 3: 
Type the following line, replacing the parameters:
  
  ```
  dotnet run  -AirportICAO [YourClosestAirportICAO] -ModeSMixerURL [YourModeXMixer2ServerLink] -URLServerFolder server
  ```
  
Example:
```
 dotnet run  
        -AirportICAO KJFK 
        -ModeSMixerURL http://mymodesmixerserver.ddns.net:8081 
        -URLServerFolder server
```

If you run and have no error, **it's running!** The application is writing some HTML pages, you can see these files in the "server" folder. If you have Apache2, IIS or any webserver installed on your machine, you just need to change the -URLServerFolder as the example below:

```
 dotnet run  
        -AirportICAO KJFK 
        -ModeSMixerURL http://mymodesmixerserver.ddns.net:8081 
        -URLServerFolder /var/www/html 
```

It's the enough to run Robot Bum. But if you want to filter some traffic or if you want to post the messages on Twitter, Robot Bum have a lot options.

## List of command-line options

|     Command                              |Value| 
|---------------------------------|--------------------------------------------------------| 
|  -AirportICAO            | text (four characters)                                             | 
|  -ModeSMixerURL         | text                                             | 
|  -TwitterAccessToken            | text                                             | 
|  -URLServerFolder               | text                                             |
|  -User               | text                                             |
|  -Password               | text                                             |
|                        |                                                   | 
|  -TwitterConsumerKey            | text                                             | 
|  -TwitterConsumerSecret         | text                                             | 
|  -TwitterAccessToken            | text                                             | 
|  -TwitterAccessTokenSecret      | text                                             |  
|                                 |                                                        | 
|  -ShowAllHeavyWeightAirplanes   | true/false                                             | 
|  -ShowAllMediumWeightAirplanes  | true/false                                             | 
|  -ShowAllLowWeightAirplanes     | true/false                                             | 
|                                 |                                                        | 
|                                 |                                                        | 
|  -AvoidAllHeavyWeightAirplanes  | true/false                                             | 
|  -AvoidAllMediumWeightAirplanes | true/false                                             | 
|  -AvoidAllLowWeightAirplanes    | true/false                                             | 
|                                 |                                                        | 
|  -ShowAllCruisesHeavyWeight     | true/false                                             | 
|  -AvoidAllFlightsStartingWith   | Ex.: "EK,DAL,AIS"                                | 
|  -ShowAllFlightStartingWith     | Ex.: "EK,DAL,AIS"                                | 
|  -AvoidAllModelsStartingWith    | Ex.: "A38,B77,B76"                               | 
|  -ShowAllModelsStartingWith     | Ex.: "A38,B77,B76"                               | 
|  -ShowHelicopters               | true/false                                             | 
|  -MessageLanguage               |  Supported languages: en,en-PIRATE,pt,pt-BR,zh-HANS,he | 
