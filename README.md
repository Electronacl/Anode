# Anode
An emulator for the Nintendo Entertainment System

## Project goals
My aim with this emulator is for an accurate NES system compatible with both PAL and NTSC. Just to make it a little easier at first, the emulator will only support NTSC, but PAL support is going to be added later.

## Known issues
Check the "issues" tab - as it's unstable at the moment, there might be too many to list here.

## Localisation support
This program is only built with British English as a language, however the addition of other languages is planned for the future.

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
The version is split into 3 sections, with different parts within each section. Here, a different letter represents each part. The sections are separated by dashes:<br>
`yyyy.x.z-M.mb-s`<br>
There is no 2nd or 3rd section in full releases, only in beta and alpha builds.<br>
### First section (`yyyy.x`)
`yyyy` is the year of release, or intended release year.<br>
`x` is the release number in the year.<br>
`z` is the patch number of the release. This is incremented when small features are changed and small bugs are fixed<br><br>
Example: `2026.3` would be the 3rd full release in 2026, and the first patch.
### Second section (`M.mb`)
`M` is the Major portion. This is incremented when large features and changes are added.<br>
`m` is the minor portion. This is incremented when small features are added and bugs are fixed.<br>
`b` is the status of the version. This can be `b` for beta or `a` for alpha.<br><br>
Beta builds come first, and have the most bugs and issues. Alpha builds are closer to the release window, when there's only a few issues to work out.<br><br>
Example: `2.4b` would have had 2 major changes, 4 minor changes, and be in the beta status.
### Last section (`s`)
`s` denotes the stability of the build. This can be `s` for stable or `xs` for unstable.<br><br>
If you're looking for accuracy and optimisation, use the stable builds only (unless there isn't one available for the version you're looking to use).<br>
If you want the latest features only, use the unstable builds.

## Accuracy info (as of 2026.2-s)
### AccuracyCoin
<img width="267" height="248" alt="image" src="https://github.com/user-attachments/assets/603a654d-3343-4a8c-8a23-26479e9cfaec" /><br>
### nestest
*Official opcodes*<br>
<img width="262" height="247" alt="image" src="https://github.com/user-attachments/assets/a3da22f5-b6ef-4635-be0b-ebc3fb264313" /><br>
*Unofficial opcodes*<br>
<img width="265" height="251" alt="image" src="https://github.com/user-attachments/assets/ffaa854f-c85d-441c-a295-8773a5398c06" /><br>

## Licenses
This project is licensed under my own license, _Electronacl Content License V2.0_. 
As per the requirements, here are the things I must state
- No AI was used in the creation of the project.
