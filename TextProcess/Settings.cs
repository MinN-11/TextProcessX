using System.Collections.Generic;

namespace TextProcess {
    public static class Settings {
        public static bool NarrowMenuPunctuations = true;

        public static Dictionary<TextFormat, NarrowFormat> NarrowFormats = new() {
            [TextFormat.Any] = NarrowFormat.Never,
            [TextFormat.Dialog] = NarrowFormat.Never,
            [TextFormat.Dialog3] = NarrowFormat.Never,
            [TextFormat.Description] = NarrowFormat.WhenNecessary,
            [TextFormat.Name] = NarrowFormat.WhenNecessary,
            [TextFormat.Item] = NarrowFormat.WhenNecessary,
            [TextFormat.WeaponDescription] = NarrowFormat.WhenNecessary,
            [TextFormat.SkillDescription] = NarrowFormat.WhenNecessary,
        };

        public static int[] FE8MenuProfile = new[] {
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 4, 4,
            6, 6, 6, 6, 2, 4, 4, 6, 6, 2, 4, 2, 5, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 2, 2, 5, 4, 5, 6, 6, 5, 5, 5, 5, 5,
            5, 5, 5, 2, 5, 5, 5, 6, 5, 5, 5, 6, 5, 5, 6, 5, 6, 6, 6, 6, 5, 3, 6, 3, 4, 4, 2, 6, 5, 5, 5, 5, 4, 5, 5,
            2, 4, 5, 2, 6, 5, 5, 5, 5, 4, 5, 4, 5, 6, 6, 6, 5, 5, 4, 2, 4, 6, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            7, 0, 0, 0, 0, 3, 2, 5, 4, 0, 0, 0, 0, 0, 0, 0, 7, 0, 0, 0, 0, 4, 0, 0, 0, 0, 0, 0, 0, 0, 5, 6, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4, 6, 0, 0, 0, 6, 5, 5, 5, 0, 5, 0, 0, 6, 5, 5, 5, 5, 3, 3, 4, 4, 0, 5,
            5, 5, 5, 0, 5, 0, 0, 5, 5, 5, 5, 0, 0, 6, 6, 6, 6, 0, 6, 0, 0, 5, 5, 5, 5, 5, 3, 3, 4, 4, 0, 5, 5, 5, 5,
            0, 5, 0, 0, 5, 5, 5, 5, 0, 0, 0
        };

        public static int[] FE8SerifProfile = new[] {
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4, 3, 5,
            8, 6, 8, 7, 2, 4, 4, 8, 8, 2, 4, 2, 7, 6, 4, 6, 6, 7, 6, 6, 6, 6, 6, 2, 2, 6, 6, 6, 7, 8, 6, 6, 6, 6, 6,
            6, 6, 6, 4, 6, 6, 5, 8, 6, 6, 6, 7, 6, 7, 6, 6, 6, 8, 6, 6, 6, 3, 8, 3, 5, 4, 2, 6, 5, 5, 5, 5, 5, 7, 5,
            2, 3, 5, 2, 6, 5, 5, 5, 5, 4, 5, 5, 5, 6, 6, 6, 4, 5, 4, 2, 4, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            8, 0, 0, 0, 0, 2, 2, 5, 5, 0, 0, 0, 0, 0, 0, 0, 8, 0, 0, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 6, 5, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5, 5, 0, 0, 0, 7, 6, 6, 6, 0, 6, 0, 0, 6, 6, 6, 6, 6, 4, 4, 4, 4, 0, 6,
            6, 6, 6, 0, 6, 0, 0, 6, 6, 6, 6, 0, 0, 7, 6, 6, 6, 0, 6, 0, 0, 5, 5, 5, 5, 5, 3, 3, 4, 4, 0, 5, 5, 5, 5,
            0, 5, 0, 0, 5, 5, 5, 5, 0, 0, 0
        };

        public static int[] NarrowFontMenuProfile = new[] {
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 4, 4,
            6, 6, 6, 6, 2, 4, 4, 6, 6, 2, 4, 2, 5, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 2, 2, 5, 4, 5, 6, 6, 5, 5, 5, 5, 5,
            5, 5, 5, 2, 5, 5, 5, 6, 5, 5, 5, 6, 5, 5, 6, 5, 6, 6, 6, 6, 5, 3, 6, 3, 4, 4, 2, 6, 5, 5, 5, 5, 4, 5, 5,
            2, 4, 5, 2, 6, 5, 5, 5, 5, 4, 5, 4, 5, 6, 6, 6, 5, 5, 4, 2, 4, 6, 6, 0, 5, 4, 4, 4, 4, 4, 4, 4, 3, 4, 4,
            4, 4, 4, 3, 4, 3, 2, 5, 4, 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 6, 4, 4, 4,
            4, 4, 4, 4, 0, 0, 0, 0, 0, 0, 0, 4, 6, 2, 0, 0, 6, 5, 5, 5, 0, 5, 0, 0, 6, 5, 5, 5, 5, 3, 3, 4, 4, 0, 5,
            5, 5, 5, 0, 5, 0, 0, 5, 5, 5, 5, 0, 0, 6, 6, 6, 6, 0, 6, 0, 0, 5, 5, 5, 5, 5, 3, 3, 4, 4, 0, 5, 5, 5, 5,
            0, 5, 0, 0, 5, 5, 5, 5, 0, 0, 0
        };

        public static int[] NarrowFontSerifProfile = new[] {
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4, 3, 5,
            8, 6, 8, 7, 2, 4, 4, 8, 8, 2, 4, 2, 7, 6, 4, 6, 6, 7, 6, 6, 6, 6, 6, 2, 2, 6, 6, 6, 7, 8, 6, 6, 6, 6, 6,
            6, 6, 6, 4, 6, 6, 5, 8, 6, 6, 6, 7, 6, 7, 6, 6, 6, 8, 6, 6, 6, 3, 8, 3, 5, 4, 2, 6, 5, 5, 5, 5, 5, 7, 5,
            2, 3, 5, 2, 6, 5, 5, 5, 5, 4, 5, 5, 5, 6, 6, 6, 4, 5, 14, 15, 17, 7, 17, 0, 4, 4, 4, 4, 4, 4, 5, 4, 4, 4,
            4, 4, 4, 4, 4, 4, 2, 2, 5, 5, 0, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 3, 5, 5, 5, 5, 5, 5, 5, 6, 6, 5, 5, 5,
            5, 4, 4, 4, 5, 13, 13, 13, 13, 13, 12, 12, 13, 5, 2, 0, 0, 7, 4, 5, 5, 5, 5, 5, 5, 5, 5, 5, 2, 2, 2, 4, 4,
            4, 4, 4, 8, 17, 3, 5, 6, 0, 0, 6, 6, 6, 6, 0, 0, 7, 6, 6, 6, 0, 6, 0, 0, 5, 5, 5, 5, 5, 3, 3, 4, 4, 0, 5,
            5, 5, 5, 0, 5, 0, 0, 5, 5, 5, 5, 0, 0, 0
        };
    }
}