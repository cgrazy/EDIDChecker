Simple c# .net core project to check Extended Display Identification Data.

###Usage

```text
dotnet \<pathToProject\>/EDIDChecker.dll [ -f \<file\> | -e \<edid\> | [ -w \<width\> -h \<height\> ] \
-f \<file\>: path to a simple ASCII encoded text file containing the EDID information string.  \
-e \<edid\>: EDID information string.  \
-w \<width\>: optional. A possible width to be checked in the EDID structure.  \ 
-h \<height\>: optional. The corresponding height of the screen to be checked in the EDID structure. 
```

#####Example:
dotnet \<path\>/bin/Debug/net8.0/EDIDChecker.dll -f ~/\<folder\>/edid_benq.txt -w 3180 -h 2140