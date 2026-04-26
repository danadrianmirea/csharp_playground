REM publish by putting everything in the binary
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true