# Anode
An emulator for the Nintendo Entertainment System

## Localisation support
This program is only built with British English as a language, however the addition of other languages is planned for the future.

## Platform support
This program is built and tested on a Windows x64 system. Whilst the program should be compatible with x32 and ARM64, I can't guarantee that this is the case.<br>
Builds are only created as .EXE, and the program is built in the .NET framework, so support for Linux and MacOS may not be available.<br>

The emulator is built for the .NET framework 4.7.2. It may not work or function correctly with older versions or without the framework entirely.<br>

Furthermore, running this wit the JIT compiler or debugging mode can cause framerate drops. It is recommended to use a pre-compiled build rather than the source if you want to run the emulator at full speed.

## Emulation and legality
Whilst it is legal to use - and for me to create - this emulator, that only applies if the ROMs are legal. I am not responsible for the ROMs used by any users on this emulator. For legal reasons, these ROMs should be from your own NES game library and obtained via a cartridge dumper, or alternatively from open-source ROMs. This emulator is only intended to work with legal ROMs.<br>
This emulator does not come with any games or test programs.

## Credits
.gitignore: https://github.com/github/gitignore/blob/main/VisualStudio.gitignore<br>
Tutorial for basics: https://www.patreon.com/posts/making-your-nes-137873901<br>
Info on CPU instructions: https://www.masswerk.at/6502/6502_instruction_set.html<br>
CPU Details: https://www.princeton.edu/~mae412/HANDOUTS/Datasheets/6502.pdf<br>
Window Focus check: https://stackoverflow.com/questions/7162834/determine-if-current-application-is-activated-has-focus<br>

## Understanding the version system for releases (2026.2.1+)
The version is split into 2 sections, with different parts within each section. Here, a different letter represents each part. The sections are separated by dashes:<br>
`yyyy.x.z-M.mb`<br>
There is no 2nd section in full releases, only in beta and alpha builds.<br>
### First section (`yyyy.x`)
`yyyy` is the year of release, or intended release year.<br>
`x` is the release number in the year.<br>
`z` is the patch number of the release. This is incremented when small features are changed and small bugs are fixed<br><br>
Example: `2026.3` would be the 3rd full release in 2026, and the first patch.
### Second section (`M.mb`)
`M` is the Major portion. This is incremented when large features and changes are added.<br>
`m` is the minor portion. This is incremented when small features are added and bugs are fixed.<br>
`b` is the status of the version. This can be `b` for beta or `a` for alpha.<br><br>
Beta builds come first, and have the most bugs and issues. Alpha builds are closer to the release window, when there's only a few issues to work out, and the code is more stable.<br><br>
Example: `2.4b` would have had 2 major changes, 4 minor changes, and be in the beta status.

## Licenses
On version 2026.3-1.0b or later, the software is licensed under the MIT license.<br>
Versions prior to 2026.3-1.0b are licensed with my own license, ECL 2.0. (Albeit with the mistake of saying "Copyleft" in the about screen, which isn't correct.<br><br>

Whilst ECL 2.0 had its own stricter limitations, these limitations are shared between the two licenses:
- The developer assumes no responsibility for any damage or harm caused by the software
- The software is provided "as is", and comes without warranty of any kind
On versions 2026.3-1.0b and later, you may:
- Copy the software
- Freely modify it, as long as the changes do not remove any license texts or remove the existing copyright statement (if you wish to add your own copyright for your code, amend it before or after the original statement.
- Distribute the software
- Sublicense the software

## Access to the software
This software is originally provided as open-source free software, and there is no paid version that officially exists or is affiliated with the developer. By downloading a copy of this software, you own that copy until it has been removed - you do not need to buy the software to own a copy.

## Accuracy info (as of 2026.3-0.0b)
Of all the NROM based ROMs I have tested (including separate tests in test suites):
- 215 pass
- 152 fail
- 6 OK
