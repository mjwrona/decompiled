// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.TokenType
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

namespace Microsoft.Ajax.Utilities
{
  internal enum TokenType
  {
    Error = -1, // 0xFFFFFFFF
    None = 0,
    Space = 1,
    CommentOpen = 2,
    CommentClose = 3,
    Includes = 4,
    DashMatch = 5,
    PrefixMatch = 6,
    SuffixMatch = 7,
    SubstringMatch = 8,
    String = 9,
    Identifier = 10, // 0x0000000A
    Hash = 11, // 0x0000000B
    ImportSymbol = 12, // 0x0000000C
    PageSymbol = 13, // 0x0000000D
    MediaSymbol = 14, // 0x0000000E
    FontFaceSymbol = 15, // 0x0000000F
    CharacterSetSymbol = 16, // 0x00000010
    AtKeyword = 17, // 0x00000011
    ImportantSymbol = 18, // 0x00000012
    NamespaceSymbol = 19, // 0x00000013
    KeyFramesSymbol = 20, // 0x00000014
    RelativeLength = 21, // 0x00000015
    AbsoluteLength = 22, // 0x00000016
    Resolution = 23, // 0x00000017
    Angle = 24, // 0x00000018
    Time = 25, // 0x00000019
    Frequency = 26, // 0x0000001A
    Speech = 27, // 0x0000001B
    Dimension = 28, // 0x0000001C
    Percentage = 29, // 0x0000001D
    Number = 30, // 0x0000001E
    Uri = 31, // 0x0000001F
    Function = 32, // 0x00000020
    Not = 33, // 0x00000021
    UnicodeRange = 34, // 0x00000022
    ProgId = 35, // 0x00000023
    Character = 36, // 0x00000024
    Comment = 37, // 0x00000025
    TopLeftCornerSymbol = 38, // 0x00000026
    TopLeftSymbol = 39, // 0x00000027
    TopCenterSymbol = 40, // 0x00000028
    TopRightSymbol = 41, // 0x00000029
    TopRightCornerSymbol = 42, // 0x0000002A
    BottomLeftCornerSymbol = 43, // 0x0000002B
    BottomLeftSymbol = 44, // 0x0000002C
    BottomCenterSymbol = 45, // 0x0000002D
    BottomRightSymbol = 46, // 0x0000002E
    BottomRightCornerSymbol = 47, // 0x0000002F
    LeftTopSymbol = 48, // 0x00000030
    LeftMiddleSymbol = 49, // 0x00000031
    LeftBottomSymbol = 50, // 0x00000032
    RightTopSymbol = 51, // 0x00000033
    RightMiddleSymbol = 52, // 0x00000034
    RightBottomSymbol = 53, // 0x00000035
    AspNetBlock = 54, // 0x00000036
    ReplacementToken = 55, // 0x00000037
  }
}
