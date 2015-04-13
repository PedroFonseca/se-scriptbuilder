# Space Engineers In-Game Scripting Compiler

This project builds Space Engineers in-game scripts from an existing Visual Studio project.

## Usage

```ScriptBuilder <target folder> <script name>```

## How It Works

It just scans the entire target folder for all .cs files. Within the file it looks for any content in a SpaceEngineers region. These will all be appended into the output script file.

A file in the root of the target called ```UserConfig.cs`` will be used first. This allows you to define any config settings you want users to be able to change and they'll be at the top of the script.

The generated script is given the name you specify and can be loaded in from the Workshop menu.
