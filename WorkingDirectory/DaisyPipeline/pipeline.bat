@echo off

rem This is the path to Java. Edit this variable to suite your needs.
set JAVA=java

echo.
%JAVA% -classpath "pipeline.jar";"." org.daisy.pipeline.ui.CommandLineUI %1 %2 %3 %4 %5 %6 %7 %8 %9
echo.
