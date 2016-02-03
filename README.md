# CSharpDecompiler
CSharpDecompiler is a crossplatform console app for decompiling .dll files to .cs, based on ILSpy.

Usage examples (on Mac):

// decompiles MyApp.dll to console
`mono CSharpDecompiler.exe MyApp.dll`

// decompiles all .dll files from /assemblies directory that don't start with System. or Mono. to /src in parallel (multithreaded) mode.
`mono CSharpDecompiler.exe assemblies -p --regex='^(?!System\.|Mono\.)' --output=src`

// show help
`mono CSharpDecompiler.exe MyApp.dll /?`
