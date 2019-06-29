cd %~dp0

set _SRC_BASE=G:\Dev\websocket-sharp\Example2
set _SRC_=%_SRC_BASE%\xs2clientprotocol
set protoc_exe="G:\ToolBase\protoc-3.0.0-win32\bin\protoc.exe"
set protoc_exe="G:\GitHub\google\protobuf\solution\Debug\protoc.exe"

@rem %protoc_exe% -I=%_SRC_BASE% --proto_path=%_SRC_% --cpp_out=%_SRC_% %_SRC_%\data.proto
@rem %protoc_exe% -I=%_SRC_BASE% --proto_path=%_SRC_% --csharp_out=%_SRC_% %_SRC_%\common.proto
@rem %protoc_exe% -I=%_SRC_BASE% --proto_path=%_SRC_% --csharp_out=%_SRC_% %_SRC_%\sc2api.proto

@rem pause

for %%i in ( *.proto ) do (
%protoc_exe% -I=%_SRC_BASE% --proto_path=%_SRC_% --csharp_out=%_SRC_% %%i
)

pause