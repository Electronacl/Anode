# Anode
An emulator for the Nintendo Entertainment System

## Project goals
My aim with this emulator is for an accurate NES system compatible with both PAL and NTSC.

## Known issues
- APU is only partially implemented, and has no sounds
- Inaccuracies with some specifics (check the AccuracyCoin results below)
- No Y scroll support
- No mapper, alternative expansions, or alternative console support
- Incorrect PPU flags cause some graphical bugs
- PAL support remains untested

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
This project is licensed under my own license, _Electronacl Content License V2.0_. 
As per the requirements, here are the things I must state
- No AI was used in the creation of the project.

<br>
Whilst there shouldn't be anything that *could* damage your PC with this program, I can't guaruntee that things like ROMs, TASes (when I add them), or modifications may cause damage. Even if it is from the core program, I don't assume responsibility for any damage that may be caused by this program. (This is for legal reasons)

## Accuracy info (as of 2026.3-0.0b)
### AccuracyCoin
<img width="263" height="245" alt="image" src="https://github.com/user-attachments/assets/0ce1a396-6eaf-45fc-ada8-ea26dd00c864" /><br>
### nestest
*Official opcodes*<br>
<img width="262" height="247" alt="image" src="https://github.com/user-attachments/assets/a3da22f5-b6ef-4635-be0b-ebc3fb264313" /><br>
*Unofficial opcodes*<br>
<img width="265" height="251" alt="image" src="https://github.com/user-attachments/assets/ffaa854f-c85d-441c-a295-8773a5398c06" /><br>

### Wondering why a test no longer passes?
Sometimes, when fixing some bugs, it gets rid of false positives. This can actually cause the score to stay the same, or even go down for a new release. This doesn't necessarily mean it's less accurate on the newer release, in fact it may be _more_ accurate, so take existing test results with a little pinch of salt whilst the emulator is still in a slightly unstable early phase.<br>

Another cause for this is the ROMs that I use. Some of them update regularly to implement new features and bugfixes based on discoveries with hardware specifics. As I aim to use new versions of the test ROMs, the results above may have the ROM changed from what is shown on Anode's previous updates, which can cause the score to decrease.
