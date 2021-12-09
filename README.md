# TextProcessX
Advanced text processor for buildfiles.

## What's new
### New Features
TextProcessX provides auto newline, auto add `[A]`, auto narrow and overflow warning.

The lack of dependency on calling ParseFile multiple times provides a significant performance boost. Text building and parsing should cost less than 1 second on most PCs.

[X] is no longer required as text no longer fall through different entries.

### Backwards Compatability
TextProcessX is mostly backwards compatable to classic TextProcess and even Bly's extension, however there are a few minor changes.

`[var] = something` can be called anywhere, but using `[A] [B]` as `[A] = [B]` is no longer supported.

Using `#` as line comment is no longer supported since text no longer fall through. Use `//` instead. *(Probably won't cause an error unless the comment is in the middle of a text block)*

Leading and trailling spaces will be trimmed, use `[0x20]` instead if necessary.

`ParseDefinitions.txt` is no longer given special treatment so please add

	#include "ParseDefinitions.txt" 
to the start of your text_buildfile to use your definitions normally.

### Additional Requirements

A version of EA Core that supports raw **BASE64**

## Command Line Arguments
    TextProcess text_buildfile.txt InstallTextData.event --narrow-mapping narrow_mapping.csv --rom reference.gba --portraits PortraitInstaller.event
    
### Breakdown:
--narrow-mapping, a csv file, required for narrow font features.

Each line represents the non-narrow version and narrow version of a character.

Valid lines:

	a, 0x81
	0x20, 0x79
	32, 127

Sample csv files are provided in the project.
	
--rom, if specified, we will use the glyph data stored in a rom to calculate text size so we can perform auto-narrow. If not specified, we will use default FE8 glyph sizes. To use the standard narrowfont glyph sizes, at the start of *text_buildfile.txt* specify 

	#define UseDefaultNarrowFontProfile

--portraits, an Event Assembler event, if specified, we will automatically import definitions like `#define {NAME}Portrait` or  `#define {NAME}Mug`.
For example if 

	#define TimmyMug 0x14
	
**[LoadTimmy]** will become 

	[LoadPortrait][0x14][0x1]

## New Formatting Options

Traditionally TextProcess has 2 syntax for text:

	## Name
	# 0x1234 (Name)
	
Now the syntax goes

	#(T/X/I/N/W/D/S/#) (0x1234) (Name)
	
T/X/I/N/W/D/S determine which type of text this is

	#T: Text, or dialogue in general, 160px serif font
	#X: eXtension, dialog with 3 line dialogue patch installed
	#I: Item Name, 56px menu font
	#N: (Character/Class) Name, 46px menu font
	#W: Weapon Description, 160px serif font
	#D: Description: 160px serif font, x2 lines
	#S: Skill Description: 160px serif font, x3 lines
	#/## (Default): The text will not be altered in any way.

T and X will try to add `[N]`s and `[A]`s if necessary. The program will not break existing formats and only add `[N]`s and `[A]`s if overflows occur.

I/N/W/D/S will try to narrow the text if an overflow occurs.
D/S will add [N]s if necessary

Examples 


	#I IronSwordName
	Iron Sword
	
	#W IronSwordDesc
	A sword made of Iron.
	



