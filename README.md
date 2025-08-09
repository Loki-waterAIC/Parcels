# Parcels

This is a project based off the Parsels AutoDesk Univeristy Plugin Class.

The project instructions can be found here:

- [Create Your First AutoCAD Plug-In by Ben Rand](https://www.autodesk.com/autodesk-university/class/Create-Your-First-AutoCAD-Plug-2020?msockid=330dfa63d9e16669310defb5d8bf671a)

This project specifically follows the handout found here:

- [Class_Handout_SD467609_Ben_Rand.pdf [Download]](https://static.au-uw2-prd.autodesk.com/Class_Handout_SD467609_Ben_Rand.pdf)

_Also found in the repo files with notes and markups_

## Requirements:

- AutoCAD - any non free version
- Visual Studio 2019+ (community version or enterprise)

## Instructions:

Follow along with the PDF.

If not following along in the PDF, start from commands.cs

## NOTES

Make sure the files you want to use, Commands.cs and Active.cs, are specified in the .csproj file.

The .csproj file is what specifies what files .NET should use and which ones it should ignore. Think of it like a make file.

```xml
<ItemGroup>
    <Compile Include="Active.cs" />
    <Compile Include="Commands.cs" />
    <Compile Include="Properties\AssemblyInfo.cs" />
</ItemGroup>
```
