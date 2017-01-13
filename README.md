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

## Prerequisites

### Windows server

The solution requires a Windows machine with minimum Windows 2008 Server Edition and a running IIS. 
.NET 4.0 and Visual Studio Runtimes 2012
The solution is build using .NET 4.0 and for running components such as Tessaract it is required to install Visual Studio Runtime 2012

### Sensus SB4
Sensus SB4 is commercial braille conversion library designed and maintained by Sensus. It produces high quality and accurate braille text. This software can be bought at www.sensus.dk. Please contact Sensus for purchasing and licensing information and sensus(at)sensus.dk

### Liblouis
Build and configure liblouis to be in the bin directory of the solution and add a tables subdirectory containing all the necessary conversion tables for liblouis, please consult the liblouis website for further information.
Download link: http://liblouis.org/downloads/

### Abbyy
Abbyy needs to be installed and configured as a running web service on your local network and added as a service reference to the solution.
For more details please check: https://www.abbyy.com/en/recognition-server/
If using abbyy, remember to change the RoboBraille.WebApi web.config file with the correct AbbyyOCRServer IP address and the correct bindings to the SoapService of the Abbyy Recognition Server.

### Windows Speech and Installed Voices
By default the application will use the english voices installed and configured on your local windows server. Additional voices can be installed and configured.

### Messaging
Make sure Message Queuing is installed and enabled: https://technet.microsoft.com/en-us/library/cc730960.aspx  In the local services settings of Windows enable Message Queuing and Message Queuing Triggers. 

#### Erlang
Erlang must be installed in order for the messaging system to work.
Installation Link: http://www.erlang.org/download.html

#### Rabbitmq (with optional clustering)
Messaging must be enabled in the Services settings of your windows machine. The RabbitMQ messaging client must also be installed and configured. If your Daisy component is running on a separate machine, clustering must also be enabled and configured. 
Useful links: https://www.rabbitmq.com/
https://www.rabbitmq.com/management.html
https://www.rabbitmq.com/clustering.html

### Daisy Pipeline 1 and 2, Lame, ImageMagick
Prerequisites: Install Java and add it to the Path environment variable. Add the felix.jar from the “WorkingDirectory\felix-framework-5.2.0\bin” folder to your Path environment variable.
Quick Run: 
For a quick setup of the Daisy Conversion component simply run the “WorkingDirectory\DaisyConversionRPC\DaisyConversionRPC.exe” executable. Note that the messaging step must be finished in order to run the executable.
Extensive Setup:
The current configuration allows for installing the Daisy related conversions to a separate machine. This configuration requires installing Erlang and RabbitMQ on your Windows server and on an additional windows machine, configuring a messaging cluster between those two machines and adding running the components. 
Pipeline 1: http://www.daisy.org/pipeline/download
Pipeline 2: https://code.google.com/p/daisy-pipeline/downloads/detail?name=pipeline2-1.6.zip
Lame: http://lame.sourceforge.net/download.php
ImageMagick: http://www.imagemagick.org/script/binary-releases.php
Please follow the configuration steps provided by Daisy in order to set up a running service and test it before adding it to the solution.

### Calibre (Portable preferred)
Quick setup: Use the provided calibre portable installation in the “WorkingDirectory” folder. Or download and install Calibre. For server installation the portable version is preferred and add a reference in the project's web.config to the calibre.exe installation path.
Link: http://calibre-ebook.com/download

### Microsoft Office 2013
In order to convert Microsoft Office documents an installation of Office 2013 must be present on the server.

### Tesseract 3.02 language data files for required files
Tessaract 3.02 comes installed as a nuget package in the solution. It is used as an open source alternative for some of the ABBYY OCR conversion capabilities. Support for multiple languages must be installed and configured within the solution as needed. Currently supported languages are English and Danish. 
Tessaract source for languages: https://github.com/tesseract-ocr/tesseract
Tessaract .NET: https://github.com/charlesw/tesseract

### Database setup and connection
Quick setup: Use MS Sql Server to create a database called “RoboBrailleJobDB” and run the script from the folder named “Database Script”. Please make sure that there is at least one entry in the ServiceUsers table and that it has the following UserID: “d2b97532-e8c5-e411-8270-f0def103cfd0”.
The current configuration uses a MS SQL Server database with code first migrations enabled. Please check the following tutorial for help in configuring the solution http://www.asp.net/web-api/overview/data/using-web-api-with-entity-framework/part-3.  
Inside the RoboBraille.WebApi project make sure to add your connection string with the appropriate Data Source and keep the naming “RoboBrailleJobDB”.

### Folder Configurations
A specific folder must be used for temporarily storing the input files these directory paths must correspond to the web.config paths of the solution. 
The current folder configuration can be found in the source code under the folder called “Working Directory”. Change the local disk and source path as appropriate.

## Running the solution
Clone the repository to your local machine. Open and build with Visual Studio 2013 or newer. Make sure all the prerequisites are met (database connection, folder configurations, etc.).
First start the DaisyConversionRPC project (or directly the .exe).
Build and run your solution within Visual Studio or on your IIS Server.

## Testing the solution
User testing can be done by following the test guides present in the solution folder under RoboBraille.TestCases.

# Release Notes
This solution is an open source version of the existing robobraille service: http://www.robobraille.org/ . It is available as is, it must be configured and tailored to each user's individual needs. For additional consultancy and help please contact sensus (at) sensus.dk 

# History
The RoboBraille service was invented by Lars Ballieu Christensen and Svend Thougaard, and has won several international awards since 2007. As a result of benevolence and cooperation with financial and professional partners, the service has gained a foothold in a number of countries across the globe; primarily institutions dealing with those who are visually impaired, dyslexic or illiterate. In May 2005, Synscenter Refsnæs applied to the EU Commission for funding of project, wherein the RoboBraille service could be localised and tested in Ireland, Great Britain, Italy, Portugal and Cyprus. The project that was implemented in 2006-2007 and was a notable success in the following: that a range of new speech accounts were added to the RoboBraille service (British English, Italian, Portuguese and Greek); that RoboBraille became capable of producing Braille in English, Italian, Portuguese and Greek; that the RoboBraille user interface became customisable in the aforementioned languages; extended the language scope of the service to include speech in both French and Lithuanian.
The Web API solution is a reimplementation of the existing robobraille functionality as an open source project. Therefore not all components can be used out of the box, for some third party components (such as ABBY), licenses must be bought.

# Acknowledgements
The solution was developed at Sensus by a team of developers (Vlad Paul Cosma, Milad Ruben Soro and Saju Sathyan), led by Lars Ballieu Christensen. The solution has been tested by: Yannis Syrimpeis and Tsakou Gianna.
For any questions regarding the implementation and development of the solution please send an email to: paul(at)sensus.dk.

# Donate
You can help improve the RoboBraille Web Api by making a donation to RoboBraille. We appreciate your support.http://robobraille.org/donate.

# License
Copyright 2016 Sensus ApS (http://www.sensus.dk/)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
You may obtain a copy of the License at

  http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and
limitations under the License.
