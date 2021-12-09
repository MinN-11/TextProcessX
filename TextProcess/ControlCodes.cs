using System;
using System.Collections.Generic;
using System.Globalization;

namespace TextProcess {
    public enum ControlCodes {
        X = 0x0,
        NL = 0x1,
        NL2 = 0x2,
        A = 0x3,
        Pause = 0x4,
        Pause2 = 0x5,
        Pause3 = 0x6,
        Pause4 = 0x7,
        FarLeft = 0x8,
        MidLeft = 0x9,
        Left = 0xA,
        Right = 0xB,
        MidRight = 0xC,
        FarRight = 0xD,
        FarFarLeft = 0xE,
        FarFarRight = 0xF,
        OpenFarLeft = 0x8,
        OpenMidLeft = 0x9,
        OpenLeft = 0xA,
        OpenRight = 0xB,
        OpenMidRight = 0xC,
        OpenFarRight = 0xD,
        OpenFarFarLeft = 0xE,
        OpenFarFarRight = 0xF,
        LoadFace = 0x10,
        LoadPortrait = 0x10,
        ClearFace = 0x11,
        ClearPortrait = 0x11,
        NormalPrintFE6 = 0x12, 
        FastPrintFE6 = 0x13,
        CloseSpeechFast = 0x14,
        CloseSpeechSlow = 0x15,
        ToggleMouthMove = 0x16,
        ToggleSmile = 0x17,
        Yes = 0x18,
        No = 0x19,
        Buy = 0x1A,
        Sell = 0x1A,
        ShopContinue = 0x1B,
        SendToBack = 0x1C,
        FastPrint = 0x1D,
        Pad = 0x1F,
    }

    public enum ComplexControlCodes {
        LoadOverworldFaces = 0x04,
        Events = 0x04,
        G = 0x05,
        MoveFarLeft = 0x0A,
        MoveMidLeft = 0x0B,
        MoveLeft = 0x0C,
        MoveRight = 0x0D,
        MoveMidRight = 0x0E,
        MoveFarRight = 0x0F,
        MoveFarFarLeft = 0x10,
        MoveFarFarRight = 0x11,
        EnableBlinking = 0x16,
        DelayBlinking = 0x18,
        PauseBlinking = 0x19,
        DisableBlinking = 0x1B,
        OpenEyes = 0x1C,
        CloseEyes = 0x1D,
        HalfCloseEyes = 0x1E,
        Wink = 0x1F,
        Tact = 0x20,
        ToggleRed = 0x21,
        Red = 0x21,
        Item = 0x22,
        SetName = 0x23,
        ToggleColorInvert = 0x25,
    }


    public static class ControlCodesHelper {
        public static readonly Dictionary<string, ControlCodes> CommonAliases = new() {
            ["N"] = ControlCodes.NL,
            ["2NL"] = ControlCodes.NL2,
            ["."] = ControlCodes.Pad,
            [".."] = ControlCodes.Pause,
            ["..."] = ControlCodes.Pause,
            ["...."] = ControlCodes.Pause,
            ["....."] = ControlCodes.Pause2,
            ["......"] = ControlCodes.Pause3,
            ["......."] = ControlCodes.Pause4,
        };

        public static readonly Dictionary<string, byte[]> Aliases = new() { };
        public static readonly Dictionary<string, byte> PortraitIDs = new() { };

        public static byte[] Parse(string controlCode) {
            var hc = controlCode.StartsWith("0x") || controlCode.StartsWith("0X") ? controlCode[2..] : controlCode; 
            if (Enum.TryParse<ControlCodes>(controlCode, out var result)) {
                return new[] {(byte) result};
            } else if (Enum.TryParse<ComplexControlCodes>(controlCode, out var result2)) {
                return new[] {(byte)0x80, (byte) result};
            } else if (CommonAliases.ContainsKey(controlCode)) {
                return new[] {(byte) CommonAliases[controlCode]};
            } else if (Aliases.ContainsKey(controlCode)) {
                return Aliases[controlCode];
            } else if (controlCode.StartsWith("Load")){
                if (PortraitIDs.ContainsKey(controlCode[4..])) {
                    return new[] {(byte) ControlCodes.LoadFace, PortraitIDs[controlCode], (byte)0x1};
                }
                ErrorReport.ReportError($"Unknown Portrait ID {controlCode}");
                return new [] {(byte) ControlCodes.Pad};
            } else if (int.TryParse(controlCode, out var r)) {
                return new[] {(byte) r};
            } else if (int.TryParse(hc, NumberStyles.HexNumber, null, out var h)) {
                return new[] {(byte) h};
            }
            ErrorReport.ReportError($"Unknown Control Code {controlCode}");
            return new [] {(byte) ControlCodes.Pad};
        }
    }
}