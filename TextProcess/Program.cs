using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using TextProcess;

var NarrowMapping = new Dictionary<int, int>();
var MenuGlyphTable = Settings.FE8MenuProfile;
var SerifGlyphTable = Settings.FE8SerifProfile;
var Dictionary = new Dictionary<int, (string, byte[])>();
var Alnum = new HashSet<int>();

var CurrentDirectory = Directory.GetCurrentDirectory();
var AssemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

int CharToInt(string any) {
    if (any.StartsWith("0x")) {
        return Convert.ToInt32(any, 16);
    } else if (any.Length == 1) {
        return any[0];
    } else if (int.TryParse(any, out var value)) {
        return value;
    }
    return any[0];
}

int ToInt(string any) {
    if (any.StartsWith("0x")) {
        return Convert.ToInt32(any, 16);
    }
    return Convert.ToInt32(any);
}

string FindFile(string file, params string[] paths) {
    foreach (var path in paths) {
        if (File.Exists(Path.Join(path, file))) {
            return Path.Join(path, file);
        }
    }
    throw new FileNotFoundException($"{file} doesn't exist!");
}

byte[] StringToBytes(string str) {
    var replacements = Regex.Matches(str, @"\[([^\[\]]*)\]").Select(x => ControlCodesHelper.Parse(x.Groups[1].Value)).ToArray();
    str = Regex.Replace(str, @"\[[^\[\]]*\]", "\n");
    var bytes = Encoding.ASCII.GetBytes(str);
    using var stream = new MemoryStream();
    var writer = new BinaryWriter(stream);
    var size = 0;
    var count = 0;
    foreach (var b in bytes) {
        if (b == '\n') {
            if (replacements[count][0] == 0) break;
            size += replacements[count].Length;
            writer.Write(replacements[count]);
            count += 1;
        } else {
            size += 1;
            writer.Write(b);
        }
    }
    return stream.GetBuffer()[..size];
}

byte[] ToNarrow(byte[] buffer, bool isMenu) {
    var newBuffer = new byte[buffer.Length];
    for (var i = 0; i < buffer.Length; i++) {
        // modifying menu numbers is a very bad idea so no narrow menu numbers
        if (isMenu && char.IsNumber((char) buffer[i])) {
            newBuffer[i] = buffer[i];
        } else if (isMenu && Settings.NarrowMenuPunctuations && !char.IsLetter((char) buffer[i]) && !IsSpace(buffer[i])) {
            newBuffer[i] = buffer[i];
        } else if (NarrowMapping.ContainsKey(buffer[i])) {
            newBuffer[i] = (byte) NarrowMapping[buffer[i]];
        } else {
            newBuffer[i] = buffer[i];
        }
    }
    return newBuffer;
}


var space = (int)' ';
var narrowSpace = NarrowMapping.ContainsKey(space) ? NarrowMapping[space] : space;

bool IsSpace(int x) {
    return x == space || x == narrowSpace;
}

bool IsNewLine(int x) {
    return x is (int)ControlCodes.NL or (int)ControlCodes.NL2;
}

void LineBreak(List<byte> bytes, int i, ref int lineCount, ref int offset) {
    while (IsSpace(bytes.LastOrDefault())) {
        bytes.RemoveAt(bytes.Count - 1);
    }
    if (lineCount >= i) {
        bytes.Add((byte) ControlCodes.A);
        lineCount = 0;
    }
    bytes.Add((byte) ControlCodes.NL);
    lineCount += 1;
    offset = 0;
}

byte[] BuildDialogue(byte[] buffer, bool isMenu, int lineSize, bool multiLine, int clickInterval) {
    var offset = 0;
    var lineCount = 1;

    var word = new List<byte>();
    var result = new List<byte>();
    var table = isMenu ? MenuGlyphTable : SerifGlyphTable;
    foreach (var b in buffer) {
        if (Alnum.Contains(b)) {
            word.Add(b);
            continue;
        }
        var (width, _) = MeasureString(word, isMenu);
        if (width + offset > lineSize && multiLine) {
            LineBreak(result, clickInterval, ref lineCount, ref offset);
        }
        result.AddRange(word);
        word.Clear();
        offset += width;
        if (IsNewLine(b)) {
            lineCount += 1;
            offset = 0;
        } else if (b == (byte)ControlCodes.A) {
            lineCount = 0;
        }
        if (offset == 0 && IsSpace(b)) continue;
        width = table[b];
        if (width + offset > lineSize && multiLine) {
            LineBreak(result, clickInterval, ref lineCount, ref offset);
        }
        offset += width;
        result.Add(b);
    }
    var (w, _) = MeasureString(word, isMenu);
    if (w + offset > lineSize && multiLine) {
        LineBreak(result, clickInterval, ref lineCount, ref offset);
    }
    result.AddRange(word);
    while (result.Count > 0 && (IsNewLine(result[^1]) || IsSpace(result[^1]))) {
        result.RemoveAt(result.Count - 1);
    }
    if (result.LastOrDefault() != (byte) ControlCodes.A && clickInterval < 10) {
        result.Add((byte) ControlCodes.A);
    }
    result.Add(0);
    return result.ToArray();
}

(int, int) MeasureString(IList<byte> str, bool isMenu) {
    if (str.Count == 0) { return (0, 0); }
    var font = isMenu ? MenuGlyphTable : SerifGlyphTable;
    var width = 0;
    var height = 1;
    var offset = 0;
    foreach (var b in str) {
        if (IsNewLine(b)) {
            width = width > offset ? width : offset;
            offset = 0;
            height += 1;
        } else {
            offset += font[b];
        }
    }
    width = width > offset ? width : offset;
    return (width, height);
}


void WarnOverflow(string s, int i) {
    ErrorReport.ReportWarning($"Text overflow at {s} 0x{i:X}.");
}

void WarnTextEmpty(byte[] bytes, string s) {
    if (bytes.Length < 2 && !string.IsNullOrWhiteSpace(s)) {
        ErrorReport.ReportWarning($"{s} empty or missing.");
    }
}

void WarnOverwrite(int i, string post) {
    if (Dictionary.ContainsKey(i)) {
        var prev = Dictionary[i].Item1;
        if (string.IsNullOrWhiteSpace(prev)) {
            prev = $"0x{i:X}";
        }
        if (string.IsNullOrWhiteSpace(post)) {
            post = $"0x{i:X}";
        }
        ErrorReport.ReportWarning($"Text Entry 0x{i:X} overwritten, from {prev} to {post}");
    }
}

void DumpString(int index, string name, byte[] buffer, TextFormat textFormat, NarrowFormat narrowFormat) {
    
    var normalDialog = BuildDialogue(buffer, textFormat.IsMenuFont(), textFormat.LineWidth(), 
        textFormat.LineCount() != 1, textFormat.ClickInterval());
    if (NarrowMapping.Count == 0 || narrowFormat == NarrowFormat.Never) {
        WarnTextEmpty(normalDialog, name);
        WarnOverwrite(index, name);
        Dictionary[index] = (name, normalDialog);
        return;
    }
    var narrowVersion = ToNarrow(buffer, textFormat.IsMenuFont());
    var narrowDialog = BuildDialogue(narrowVersion, textFormat.IsMenuFont(), textFormat.LineWidth(), 
        textFormat.LineCount() != 1, textFormat.ClickInterval());
    var (normalW, normalH) = MeasureString(normalDialog, textFormat.IsMenuFont());
    var (narrowW, narrowH) = MeasureString(narrowDialog, textFormat.IsMenuFont());
    if (narrowW > textFormat.LineWidth() || narrowH > textFormat.LineCount()) {
        WarnOverflow(name, index);
    }
    WarnTextEmpty(normalDialog, name);
    WarnOverwrite(index, name);
    switch (narrowFormat) {
        case NarrowFormat.Always:
            Dictionary[index] = (name, narrowDialog);
            break;
        case NarrowFormat.WhenNecessary when normalW > textFormat.LineWidth() || normalH > textFormat.LineCount():
            Dictionary[index] = (name, narrowDialog);
            break;
        case NarrowFormat.WhenNecessary:
            Dictionary[index] = (name, normalDialog);
            break;
        case NarrowFormat.ReduceLineCount:
            Dictionary[index] = (name, narrowH < normalH ? narrowDialog: (normalW > textFormat.LineWidth() ? narrowDialog: normalDialog));
            break;
    }
}

void SetOption(string option, string value) {
    try {
        if (option == "UseDefaultNarrowFontProfile") {
            MenuGlyphTable = Settings.NarrowFontMenuProfile;
            SerifGlyphTable = Settings.NarrowFontSerifProfile;
            return;
        }
        if (option == "AllowNarrowMenuPunctuations") {
            Settings.NarrowMenuPunctuations = true;
            return;
        }
        if (option.EndsWith("NarrowWhen")) {
            var textFormat = Enum.Parse<TextFormat>(option[..^10]);
            var narrowFormat = Enum.Parse<NarrowFormat>(value);
            Settings.NarrowFormats[textFormat] = narrowFormat;
            return;
        }
    } catch (Exception) { }
    ErrorReport.ReportError($"Invalid option {option} {value}.");
}

void ParseFile(string fileName, ref int ptr) {
    var dir = Path.GetDirectoryName(fileName);
    var lines = File.ReadAllLines(fileName);
    var buffer = new List<byte>();
    var entryName = "";
    var parsing = -1;
    var textFormat = TextFormat.Any;
    var narrowFormat = NarrowFormat.Never;
    ErrorReport.FileName = Path.GetFileName(fileName);
    for (var l = 0; l < lines.Length; l++) {
        ErrorReport.LineNumber = l + 1;
        var line = lines[l].Trim();
        var commentIndex = line.IndexOf("//", StringComparison.Ordinal);
        if (commentIndex >= 0) {
            (line, var comment) = (line[..commentIndex], line[commentIndex..]);
            var idx = comment.ToUpper().IndexOf("TODO:", StringComparison.Ordinal);
            if (idx > 0) {
                ErrorReport.ReportToDo(comment[(idx + 5)..]);
            }
        }
        if (string.IsNullOrWhiteSpace(line)) {
            continue;
        }
        if (Regex.IsMatch(line, @"^\[.*\]\s*=.*$")) {
            if (parsing != -1 && buffer.Count != 0) {
                DumpString(parsing, entryName, buffer.ToArray(), textFormat, narrowFormat);
                ptr += 1;
            }
            parsing = -1;
            entryName = "";
            buffer.Clear();
            var match = Regex.Match(line, @"^\[(.*)\]\s*=\s*(.*)$");
            var varName = match.Groups[1].Value;
            var bytes = StringToBytes(match.Groups[2].Value);
            ControlCodesHelper.Aliases[varName] = bytes;
            continue;
        }
        if (line.StartsWith("#")) {
            if (parsing != -1 && buffer.Count != 0) {
                DumpString(parsing, entryName, buffer.ToArray(), textFormat, narrowFormat);
                ptr += 1;
            }
            parsing = -1;
            entryName = "";
            buffer.Clear();
            if (Regex.IsMatch(line, @"^#define\s+\w+\s*\w*$")) {
                var matchOption = Regex.Match(line, @"^#define\s+(\w+)\s*(\w*)$");
                var option = matchOption.Groups[1].Value;
                var value = matchOption.Groups[2].Value;
                SetOption(option, value);
                continue;
            }
            if (Regex.IsMatch(line, @"^#include\s+"".*""$")) {
                var file = Regex.Match(line, @"^#include\s+""(.*)""$").Groups[1].Value;
                file = FindFile(file, dir!, CurrentDirectory, AssemblyDirectory!);
                ParseFile(file, ref ptr);
                continue;
            }
            var match = Regex.Match(line, @"^#([#IWDSNT])?\s*(((0x|0X)?[a-fA-F0-9]+)?(\s+|([\^*])?$))?(\w+)?\s*([\^*])?");
            if (match.Success) {
                var textType = match.Groups[1].Value;
                var id = match.Groups[3].Value;
                entryName = match.Groups[7].Value;
                var forceNarrow = match.Groups[8].Length > 0;
                (textFormat, narrowFormat) = TextFormatHelper.FromCode(textType);
                if (forceNarrow) {
                    narrowFormat = NarrowFormat.Always;
                }
                if (id.Length > 0) {
                    parsing = ToInt(id);
                    ptr = parsing;
                } else {
                    parsing = ptr;
                }
                continue;
            }
            ErrorReport.ReportWarning($"Unable to parse statement: {line}");
        } else {
            if (parsing != -1) {
                var bytes = StringToBytes(line);
                buffer.AddRange(bytes);
            } else {
                ErrorReport.ReportSevereWarning($"Orphaned text: {line}");
            }
        }
    }
    if (parsing != -1) {
        DumpString(parsing, entryName, buffer.ToArray(), textFormat, narrowFormat);
        ptr += 1;
    }
}
const string alnumString = "01234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ'";
Alnum = alnumString.Select(x => (int)x).ToHashSet();

var commandLineArgs = args.ToList();
if (commandLineArgs.Count < 2) {
    var isExe = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ".exe" : "";
    Console.WriteLine($"TextProcess{isExe} text_buildfile.txt output.event [--narrow-mapping file] [--rom fe8_with_font_edits.gba] [--portraits portraits_definitions.event]");
    Environment.Exit(0);
}
var input = commandLineArgs[0];
var output = commandLineArgs[1];
var narrowIndex = commandLineArgs.IndexOf("--narrow-mapping");
if (narrowIndex > 0) {
    var narrowFile = FindFile(commandLineArgs[narrowIndex + 1], CurrentDirectory, AssemblyDirectory!);
    var lines = File.ReadAllLines(narrowFile).Where(x => !string.IsNullOrWhiteSpace(x));
    NarrowMapping = lines.
        Select(x => x.Split(new []{' ', ','}, StringSplitOptions.RemoveEmptyEntries)).
        ToDictionary(x => CharToInt(x[0]), x => CharToInt(x[1]));
    Alnum.UnionWith(alnumString.Where(x => NarrowMapping.ContainsKey(x)).Select(x => NarrowMapping[x]));
}

var romRef = commandLineArgs.IndexOf("--rom");
var (menuGlyphTableOffset, serifGlyphTableOffset) = (0x58C7EC, 0x58F6F4);
if (romRef > 0) {
    MenuGlyphTable = new int[256];
    SerifGlyphTable = new int[256];
    var rom = FindFile(commandLineArgs[romRef + 1], CurrentDirectory, AssemblyDirectory!);
    var romBuffer = File.ReadAllBytes(rom);
    for (var i = 0x20; i < 255; i++) {
        var offset = menuGlyphTableOffset + 4 * i;
        var ptr = romBuffer[offset] + (romBuffer[offset + 1] << 8) + (romBuffer[offset + 2] << 16) + (romBuffer[offset + 3] << 24);
        MenuGlyphTable[i] = ptr == 0 ? 0 : romBuffer[ptr - 0x8000000 + 5];
    }
    for (var i = 0x20; i < 255; i++) {
        var offset = serifGlyphTableOffset + 4 * i;
        var ptr = romBuffer[offset] + (romBuffer[offset + 1] << 8) + (romBuffer[offset + 2] << 16) + (romBuffer[offset + 3] << 24);
        SerifGlyphTable[i] = ptr == 0 ? 0 : romBuffer[ptr - 0x8000000 + 5];
    }
    MenuGlyphTable[0x80] = SerifGlyphTable[0x80] = 0;
}

var portraitsRef = commandLineArgs.IndexOf("--portraits");
if (portraitsRef > 0) {
    var portraitsEvent = FindFile(commandLineArgs[romRef + 1], CurrentDirectory, AssemblyDirectory!);
    var eventFile = File.ReadAllText(portraitsEvent);
    var matches = Regex.Matches(eventFile, @"^\s*#define\s+(\w+)(Portrait|Mug)\s+((0x)?\d+)\s*$");
    foreach (Match x in matches) {
        ControlCodesHelper.PortraitIDs[x.Captures[0].Value] = (byte)ToInt(x.Captures[2].Value);
    }
}
var defaultText = 0xD49;
ParseFile(FindFile(input, CurrentDirectory, AssemblyDirectory!), ref defaultText);
Console.WriteLine($"TextProcess finished at 0x{defaultText:X}!");
using StreamWriter file = new(output);
file.WriteLine($"// {Path.GetFileName(output)} generated by TextProcess");
file.WriteLine("// Do NOT modify");
file.WriteLine("// Requires a version of EA that supports raw BASE64");
file.WriteLine();
foreach (var (key, entry) in Dictionary) {
    var (name, data) = entry;
    var labelName = $"__text_data_0x{key:x}_{name.ToLower()}";
    file.WriteLine($"ALIGN 4");
    file.WriteLine($"{labelName}:");
    file.WriteLine($"BASE64 \"{Convert.ToBase64String(data)}\"");
    file.WriteLine($"setText({key}, {labelName})");
    if (!string.IsNullOrWhiteSpace(name)) {
        file.WriteLine($"#define {name} 0x{key:X}");
    }
    file.WriteLine();
}