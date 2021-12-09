using System;

namespace TextProcess {
    public enum TextFormat {
        Any,  // no wrapping, no narrow
        Dialog, //"T" no narrow, wrap at 160px
        Dialog3, //"X" no narrow, wrap at 160px
        Item, //"I" item name, menu font, 52 px, 1 row, auto-narrow, overflow warning
        Name, //"N" character/class name, menu font, 46 px, 1 row, auto-narrow, overflow warning
        WeaponDescription, //"W" 160 px, 1 row, auto-narrow, overflow warning
        Description, //"D" 160 px, 2 rows, auto-narrow, overflow warning
        SkillDescription, //"S" 160 px, 3 rows, auto-narrow, overflow warning
    }
    
    public enum NarrowFormat {
        Always, Never, WhenNecessary, ReduceLineCount
    }

    public static class TextFormatHelper {

        public static int LineCount(this TextFormat format) {
            return format switch {
                TextFormat.Item => 1,
                TextFormat.Name => 1,
                TextFormat.WeaponDescription => 1,
                TextFormat.Description => 2,
                TextFormat.SkillDescription => 3,
                _ => 9999,
            };
        }
        
        public static int LineWidth(this TextFormat format) {
            return format switch {
                TextFormat.Name => 46,
                TextFormat.Item => 56,
                TextFormat.Any => 9999,
                _ => 160,
            };
        }
        
        public static bool IsMenuFont(this TextFormat format) {
            return format switch {
                TextFormat.Name => true,
                TextFormat.Item => true,
                _ => false,
            };
        }
        
        public static bool IsSerifFont(this TextFormat format) {
            return format switch {
                TextFormat.Name => false,
                TextFormat.Item => false,
                _ => true,
            };
        }

        public static int ClickInterval(this TextFormat format) {
            return format switch {
                TextFormat.Dialog => 2,
                TextFormat.Dialog3 => 3,
                _ => 9999,
            };
        }

        public static (TextFormat, NarrowFormat) FromCode(string code) {
            var textformat = code switch {
                "#" => TextFormat.Any,
                "" => TextFormat.Any,
                "T" => TextFormat.Dialog,
                "X" => TextFormat.Dialog3,
                "I" => TextFormat.Item,
                "N" => TextFormat.Name,
                "W" => TextFormat.WeaponDescription,
                "D" => TextFormat.Description,
                "S" => TextFormat.SkillDescription,
                _ => throw new ArgumentException($"Unknown Control Code {code}.")
            };
            return (textformat, Settings.NarrowFormats[textformat]);
        }
        
    }
}