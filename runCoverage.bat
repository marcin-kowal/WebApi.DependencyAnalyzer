@echo off

SET dotnet="C:/Program Files/dotnet/dotnet.exe"  
SET opencover=%UserProfile%\.nuget\packages\opencover\4.6.519\tools\OpenCover.Console.exe  
SET reportgenerator=%UserProfile%\.nuget\packages\reportgenerator\3.0.2\tools\ReportGenerator.exe

REM SET targetargs="test"
SET targetargs="test -f netcoreapp2.0 -c Debug WebApi.DependencyAnalyzer.Engine.Tests/WebApi.DependencyAnalyzer.Engine.Tests.csproj"
SET filter="+[WebApi*]* -[WebApi.DependencyAnalyzer.Engine.Tests*]*"
SET coveragefile=coverage.xml
SET coveragedir=Coverage 
SET searchdir=WebApi.DependencyAnalyzer.Engine.Tests/bin/Debug/netcoreapp2.0

REM delete old results
del %coveragefile%
rmdir /s /q %coveragedir%

REM Run code coverage analysis  
%opencover% -target:%dotnet% -targetargs:%targetargs% -output:%coveragefile% -filter:%filter% -searchdirs:%searchdir% -oldStyle -register:user -mergeoutput -hideskipped:File 

REM Generate the report  
%reportgenerator% -targetdir:%coveragedir% -reporttypes:Html;Badges -reports:%coveragefile% -verbosity:Error

REM Open the report  
start "report" "%coveragedir%\index.htm" 