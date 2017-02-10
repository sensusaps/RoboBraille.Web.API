# RoboBraille.Web.API

Open source version of the RoboBraille Web Service. http://www.robobraille.org/ Developed by Sensus ApS.

# Introduction

RoboBraille is a web service capable of automatically transforming documents into a variety of alternate formats for the visually and reading impaired.
RoboBraille has been available as web application for many years. Now, RoboBraille is available as a programmable Web Api. The objective is to provide developers with the basic support for creating their own web server implementation of the API. The Web API infrastructure provides the backbone on which the RoboBraille conversions take place.
The RoboBraille Web Api was developed at Sensus ApS. Sensus is a research-based consultancy organisation specialising in accessibility, inclusion, information technology and disability. The RoboBraille Web Api project is available as an open source Web Api application, however the technologies used by the application are each subject to their own licensing, terms and conditions. All of the tools used, referenced or mentioned in the application are simply provided as examples, any similar tools can be used instead. Sensus ApS does not endorse any of the technologies or tools used, referenced or mentioned, nor is it responsible for any misuse. 

# Motivation

The project that is part of Prosperity4All (P4All). It is a European project funded under the FP7 programme. The main project website: http://www.prosperity4all.eu. More resources that describes the project are available at: https://wiki.gpii.net/w/Main_Page 
 
High level description of the solution: https://wiki.gpii.net/w/Document_Transformation_Infrastructure
 
The end result will be an open source version of RoboBraille that others might be able to implement, provided they also got all the commercial software etc., that Sensus is using, or use open source equivalents instead. 

# Documentation

RoboBraille offers five different categories of services:
* Braille services
* Audio services
* Daisy conversion services
* E-book services
* Accessibility services

## Braille services

Transcription of documents to and from contracted and uncontracted Braille in accordance with the Braille codes for Bulgarian, Danish, British English, American English, French, German, Greek, Hungarian, Icelandic, Italian, Norwegian, Portuguese, Romanian, Slovenian and Spanish. The documents can furthermore be formatted and paginated, and delivered as ready-to-emboss files in a variety of digital Braille formats.

## Audio and Daisy conversion services

Conversion into plain MP3 files and well as DAISY Talking Books, including DAISY books with spoken math. The audio conversion features currently include high-quality voices for Arabic, Arabic/English bilingual, Bulgarian, British and American English, Danish, Dutch, German, Greenlandic, French, Hungarian, Italian, Lithuanian, Polish, Portuguese, Romanian, Slovenian, and Castilian and Latin American Spanish.

## E-book services

Documents can be converted into both EPUB and Mobi Pocket (Amazon Kindle) e-book formats. Furthermore, EPUB may be converted into Mobi Pocket and vice versa. To accommodate users with low vision, the base line of the body text in an e-book may be raised to allow for more appropriate text scaling in mainstream e-book readers. RoboBraille can also be converted into EPUB3 e-books with media overlays.

## Accessibility services

Otherwise inaccessible documents such as image files in gif, tiff, jpg, bmp, pcx, dcx, j2k, jp2, jpx, djv and image-only pdf, as well as all types of pdf files can be converted to more accessible formats including tagged pdf, doc, docx, Word xml, xls, xlsx, csv, text, rtf and html. The service furthermore supports conversion of Microsoft Office documents into tagged pdf and Microsoft PowerPoint presentations into rtf files and web-projects.

# Versioning

## Release V1.01

Following the stable release. Certain improvements have been added, including adding a DELETE method in the Controller and better Amara integration

## Release V1

The Release V1 is the first stable version release of the project. The project has been tested. 

## Beta V2

Beta V2 Changes:
-improved testing, by adding dependency injection to Controller and Repository classes
-new features:
    --added frameworks for uploading videos to Amara and downloading subtitles
    --added templates for Document Structure Recognition, Document Accessibility Checking and Language to Language Translation
-improved efficiency of conversions
-added more information to the database

## Beta V1

Beta V1 was the original public Beta release.

# Installation

Clone the repository to your local machine, do not build it yet. Once downloaded the solution will need to be configured according to the Installation step below in order to build and run successfully. 

## Running the solution

Before running the solution on your local IIS server, the following components and configurations need to be installed in order to achieve the full functionality of the RoboBraille Web API solution.

## Prerequisites

### Windows server

The solution requires a Windows machine with minimum Windows 2008 Server Edition and a running IIS. .NET 4.0 and Visual Studio Runtimes 2012 The solution is build using .NET 4.0 and for running components such as Tessaract it is required to install Visual Studio Runtime 2012.
Create the bin folder in the RoboBraille.WebApi project and add the necessary dlls (the ones mentioned below).

### Folder Configurations

The directory paths must correspond to the key-value pairs set inside the web.config of the solution. The current folder configuration can be found in the source code under the folder called “Working Directory”. Change the local disk and source path as appropriate. There are 4 values in the web.config file that need to be mapped correctly.
    <add key="tessdatapath" value="" />
    <add key="calibrepath" value="C:\calibreportable\calibre portable\calibre" />
    <add key="BinDirectory" value="" />
    <add key="FileDirectory" value="C:\RoboBrailleWebApi\" />
    <add key="DistDirectory" value="C:\...\dist\" />
tessdatapath must point to the folder containing  the training data for Tesseract OCR (only necessary if Tesseract is used as part of the solution)
calibrepath must point to the installation path of the Calibre setup (only necessary if Calibre is used as part of the solution)
BinDirectory must point to the project’s bin directory, also when publishing to the server it must point to the bin directory of the published solution)
FileDirectory corresponds to the folder called WorkingDirectory within the downloaded RoboBrailleWebAPI solution, FileDirectory must point to the WorkingDirectory.
DistDirectory corresponds to the directory where RoboBraille Web API can publish files to the web in order to be sent as further requests to other API’s or any other similar actions (currently it is used to POST videos to the Amara API)

### Sensus SB4

Sensus SB4 is commercial braille conversion dynamic link library (dll) designed and maintained by Sensus. It produces high quality and accurate braille text. This software can be bought at www.sensus.dk. Please contact Sensus for purchasing and licensing information and sensus@sensus.dk. Once purchased the SB4.dll needs to be placed in the bin directory of your solution together with the Sensus.Braille.dll and only the Sensus.SB4.dll should be added as a reference within the Web API solution (in Visual Studio, right click on the RoboBraille.WebApi project, select “Add/Reference” and browse to your bin directory to add the “Sensus.SB4.dll”). [this can also be omitted in the final document and only specify: “further instructions provided upon purchasing”]

### Liblouis

Download a stable build of Liblouis from the provided download link (or alternatively build and configure the Liblouis dll). Place the liblouis.dll in the bin directory of the solution and add a table subdirectory containing all the necessary conversion tables for Liblouis, please consult the Liblouis website for further information. 
Download link: http://liblouis.org/downloads/ 

### Abbyy

Abbyy Recognition Server needs to be installed and configured as a running web service on your local network and added as a service reference to the solution. For more details please check: https://www.abbyy.com/en/recognition-server/ If using Abbyy, remember to change the RoboBraille.WebApi web.config file with the correct AbbyyOCRServer IP address and the correct bindings to the SoapService of the Abbyy Recognition Server.

### Windows Speech and Installed Voices

By default the application will use the English voices installed and configured on your local windows server. Additional voices can be installed and configured.
Install eSpeak to see voices. (also used by the daisy pipeline).
Optional: You can see installed voices using eSpeak.

### Messaging

Make sure Message Queuing is installed and enabled. In Control Panel under “Programs/Programs and Features/Turn Windows features on or off”, select the check box for “Microsoft Message Queue (MSMQ) Server” in order to enable it. Follow the provided link for more details: https://technet.microsoft.com/en-us/library/cc730960.aspx  
In Control Panel, under “System and Security/Administrative Tools/Services” Message Queuing (and optionally Message Queuing Triggers - depends on the windows version) must have “Status: Running” and “Startup Type: Automatic”.
Erlang must be installed in order for the messaging system to work. Installation Link: http://www.erlang.org/download.html 
Messaging must be enabled in the Services settings of your windows machine. The RabbitMQ messaging client must also be installed. You can install RabbitMQ from the following link: https://www.rabbitmq.com/
If your DAISY component is running on a separate machine, clustering must also be enabled and configured: https://www.rabbitmq.com/clustering.html
In order to configure and manage your RabbitMQ cluster it is advisable to enable the management console of rabbit mq: https://www.rabbitmq.com/management.html  

### Daisy Pipeline 1 and 2, Lame, ImageMagick

Please follow the “Prerequisites” step before going through either the “Quick Run” or “Extensive Setup”.

#### Prerequisites 

Follow the instructions to download Java and run the .exe to install Java on your machine. Once you installed Java on your machine, you will need to set environment variables to point to correct installation directories.
Assuming you have installed Java in C:\Program Files\java\jdk directory, go to this directory and copy the path
Right-click on 'My Computer' and select Properties/Advanced system settings/Environment variables/system variables
Choose the 'Path' variable and press the edit button. At the end of this line, make sure there is a “;”,Paste in the path to your java directory, and end with a “;”. Now do the same with the jre folder from your java directory. Example: at the end of the Path variable change your path to read ; C:\Program Files\Java\jdk1.8.0_60\bin; C:\Program Files\Java\jdk1.8.0_60\jre;
Add the felix.jar from the “WorkingDirectory\felix-framework-5.2.0\bin” folder to your Path environment variable following the same instructions.
(e.g. ; C:\Users\Administrator\Source\Repos\RoboBrailleWebApi\WorkingDirectory\felix-framework-5.2.0\bin;)
Install lame: http://lame.sourceforge.net/download.php (for windows users this is recomended: http://lame.buanzo.org/Lame_v3.99.3_for_Windows.exe ).
Install ImageMagick: http://www.imagemagick.org/. Be aware that only version 6.9.3-Q16 has been tested and is known to work!
Install eSpeak: http://espeak.sourceforge.net/download.html
There are two options for running the Daisy conversion. Either by following the “Quick Run” or by doing the “Extensive Setup”. The “Quick Run” uses the “WorkingDirectory” in which the DaisyConversionRPC is present along with a complete version of the Daisy Pipeline 1 and the Daisy Pipeline 2.

#### Quick Run

For a quick run of the Daisy Conversion component simply run the “WorkingDirectory\DaisyConversionRPC\DaisyConversionRPC.exe” executable. The source code for this executable can be found in the github folder under the “DaisyConversionRPC” project. Note that the messaging step must be finished in order to run the executable. 

#### Extensive Setup

If you do not want to follow the “Quick Run” and wish to build the entire process from scratch please read the following carefully. First build the project “DaisyConversionRPC” from the github source code. Then follow the configuration steps provided by Daisy in order to set up a running service of Daisy Pipeline 1 and 2, links are provided below:
DAISY Pipeline 1: http://www.daisy.org/pipeline/download
DAISY Pipeline 2: https://code.google.com/p/daisy-pipeline/downloads/detail?name=pipeline2-1.6.zip 

Optional step: The current configuration allows for installing the Daisy related conversions to a separate machine. This configuration requires installing Erlang and RabbitMQ on your Windows server and on an additional windows machine, configuring a messaging cluster between those two machines. 

### Calibre (Portable preferred)

The portable version is preferred.
Use the provided calibre portable installation in the “WorkingDirectory” folder. Or download and install Calibre. For server installation the portable version is preferred and add a reference in the project's web.config to the calibre.exe installation path. 
Link: 
http://calibre-ebook.com/download 

### Microsoft Office 2013

In order to convert Microsoft Office documents an installation of Office 2013 must be present on the server.

### Tesseract 3.02 language data files for required files

Tesseract 3.02 comes installed as a nuget package in the solution. It is used as an open source alternative for some of the ABBYY OCR conversion capabilities. Support for multiple languages must be installed and configured within the solution as needed. Currently supported languages are English and Danish. 
Tesseract source for languages: 
https://github.com/tesseract-ocr/tesseract 
Tesseract .NET:
https://github.com/charlesw/tesseract 

### Database setup and connection

Use MS Sql Server to create a database called “RoboBrailleJobDB” and run the latest version of the script called “RoboBrailleJobDB-(version).sql” from the folder named “Database Script”. After that run the “demo-user.sql” script to add a default user. The current configuration uses a MS SQL Server database with code first migrations enabled. 
Please check the following tutorial for help in configuring the solution http://www.asp.net/web-api/overview/data/using-web-api-with-entity-framework/part-3. 
If not setup automatically, check inside the web.config file that the connection string with the appropriate Data Source exists with the naming “RoboBrailleJobDB”.

## Building the solution

Open and build with Visual Studio 2013 or newer. Make sure all the prerequisites are met (database connection, folder configurations, etc.). First start the DaisyConversionRPC project (or directly the .exe). Build and run your solution within Visual Studio (alternatively publish to IIS server). 

## Testing the solution

User testing can be done by following the test guides present in the solution folder under RoboBraille.TestCases
The solution uses Swagger [http://swagger.io/] to show an overview of the Web API functionality. To see this on your local installation use the following path:
http://{url}:{port}/swagger/ui/index

## Authentication

The authentication can be disabled if it is not needed by removing the [Authorize] attribute above the Post method of each of the API Controllers. The RoboBraille API uses HAWK authentication. Which is a Token based authentication mechanism. Every POST request must be accompanied by an “Authorization” header. See https://github.com/hueniverse/hawk for more details

# Release Notes

This solution is an open source version of the existing robobraille service: http://www.robobraille.org/ . It is meant to be a framework fro managing document conversions and it must be custom tailored to fit individual needs and the conversion components need to be installed and configured. The solution is available as is. For additional consultancy and help please contact sensus (at) sensus.dk 

# History

The RoboBraille service was invented by Lars Ballieu Christensen and Svend Thougaard, and has won several international awards since 2007. As a result of benevolence and cooperation with financial and professional partners, the service has gained a foothold in a number of countries across the globe; primarily institutions dealing with those who are visually impaired, dyslexic or illiterate. In May 2005, Synscenter Refsnæs applied to the EU Commission for funding of project, wherein the RoboBraille service could be localised and tested in Ireland, Great Britain, Italy, Portugal and Cyprus. The project that was implemented in 2006-2007 and was a notable success in the following: that a range of new speech accounts were added to the RoboBraille service (British English, Italian, Portuguese and Greek); that RoboBraille became capable of producing Braille in English, Italian, Portuguese and Greek; that the RoboBraille user interface became customisable in the aforementioned languages; extended the language scope of the service to include speech in both French and Lithuanian.
The Web API solution is a reimplementation of the existing robobraille functionality as an open source project. Therefore not all components can be used out of the box, for some third party components (such as ABBY), licenses must be bought.

# Acknowledgements

The solution was developed at Sensus by a team of developers (Vlad Paul Cosma, Milad Ruben Soro and Saju Sathyan), led by Lars Ballieu Christensen. The solution has been tested by: Yannis Syrimpeis and Tsakou Gianna.
For any questions regarding the implementation and development of the solution please send an email to: paul(at)sensus.dk.

# Donate

You can help improve the RoboBraille Web Api by making a donation to RoboBraille. We appreciate your support.http://robobraille.org/donate.

# License

Copyright 2017-2016 Sensus ApS (http://www.sensus.dk/)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
You may obtain a copy of the License at

  http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and
limitations under the License.
