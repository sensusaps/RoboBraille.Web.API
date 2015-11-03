#!/bin/bash
############################################
# Simple launch script for the command line
# version of the Daisy Pipeline   
############################################

# This is the path to Java. Edit this variable to suite your needs.
JAVA=`which java 2> /dev/null`
#JAVA=/opt/jre1.5.0_04/bin/java




# Test java version
if [ -n "$JAVA" ]
then
  JAVAVERSION=`$JAVA -version 2>&1 | grep version | cut -d '"' -f 2 | cut -d '.' -f 1-2`
  JAVAMAJOR=`echo $JAVAVERSION | cut -d '.' -f 1`
  JAVAMINOR=`echo $JAVAVERSION | cut -d '.' -f 2`
  if [ $JAVAMAJOR -lt 1 -o $JAVAMINOR -lt 5 ]
  then    
    echo
    echo "Error: This application requires Java 5 (or later) to run. Only" 
    echo "       version $JAVAVERSION was found on your system path. Make sure" 
    echo "       Java 5 (or later) is installed on the system and edit this"
    echo "       pipeline.sh script to insert the path to the 'java' command."  
    echo
    echo "       Current (incorrect) java path: $JAVA"
    echo
    exit 1
  fi
else
  echo
  echo "Error: Java 5 was not found on your system path. Make sure Java 5"
  echo "       (or later) is installed on the system and edit this"
  echo "       pipelins.sh script to insert the path to the 'java' command."  
  echo
  exit 1
fi



# Set classpath
DIR=`dirname $0`
CP=$DIR/pipeline.jar:$DIR

# Execute Daisy Pipeline
$JAVA -classpath $CP org.daisy.pipeline.ui.CommandLineUI "$@"
