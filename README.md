# CSharpDecompiler
CSharpDecompiler is a crossplatform console tool for decompiling .dll files to .cs, based on ILSpy.

Usage examples (on Mac):

`mono CSharpDecompiler.exe MyApp.dll` // decompiles MyApp.dll to console

`mono CSharpDecompiler.exe assemblies -p --regex='^(?!System\.|Mono\.)' --output=src` // decompiles all .dll files from /assemblies directory that don't start with System. or Mono. to /src in parallel (multithreaded) mode.

`mono CSharpDecompiler.exe MyApp.dll /?` // show help
