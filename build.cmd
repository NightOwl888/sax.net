@echo off

set builddir="%CD%\build\"

rmdir /s /q %builddir%
mkdir %builddir%

%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild Sax.Net.sln /t:Clean;Build /p:Configuration=Release /v:m /nr:false

xcopy "%CD%\src\Sax\bin\Release\Sax.Net.*" %builddir% /Y /R /V