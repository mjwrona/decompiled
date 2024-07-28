// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.HtmlCssParser
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  internal static class HtmlCssParser
  {
    private static readonly string[] _colors = new string[140]
    {
      "aliceblue",
      "antiquewhite",
      "aqua",
      "aquamarine",
      "azure",
      "beige",
      "bisque",
      "black",
      "blanchedalmond",
      "blue",
      "blueviolet",
      "brown",
      "burlywood",
      "cadetblue",
      "chartreuse",
      "chocolate",
      "coral",
      "cornflowerblue",
      "cornsilk",
      "crimson",
      "cyan",
      "darkblue",
      "darkcyan",
      "darkgoldenrod",
      "darkgray",
      "darkgreen",
      "darkkhaki",
      "darkmagenta",
      "darkolivegreen",
      "darkorange",
      "darkorchid",
      "darkred",
      "darksalmon",
      "darkseagreen",
      "darkslateblue",
      "darkslategray",
      "darkturquoise",
      "darkviolet",
      "deeppink",
      "deepskyblue",
      "dimgray",
      "dodgerblue",
      "firebrick",
      "floralwhite",
      "forestgreen",
      "fuchsia",
      "gainsboro",
      "ghostwhite",
      "gold",
      "goldenrod",
      "gray",
      "green",
      "greenyellow",
      "honeydew",
      "hotpink",
      "indianred",
      "indigo",
      "ivory",
      "khaki",
      "lavender",
      "lavenderblush",
      "lawngreen",
      "lemonchiffon",
      "lightblue",
      "lightcoral",
      "lightcyan",
      "lightgoldenrodyellow",
      "lightgreen",
      "lightgrey",
      "lightpink",
      "lightsalmon",
      "lightseagreen",
      "lightskyblue",
      "lightslategray",
      "lightsteelblue",
      "lightyellow",
      "lime",
      "limegreen",
      "linen",
      "magenta",
      "maroon",
      "mediumaquamarine",
      "mediumblue",
      "mediumorchid",
      "mediumpurple",
      "mediumseagreen",
      "mediumslateblue",
      "mediumspringgreen",
      "mediumturquoise",
      "mediumvioletred",
      "midnightblue",
      "mintcream",
      "mistyrose",
      "moccasin",
      "navajowhite",
      "navy",
      "oldlace",
      "olive",
      "olivedrab",
      "orange",
      "orangered",
      "orchid",
      "palegoldenrod",
      "palegreen",
      "paleturquoise",
      "palevioletred",
      "papayawhip",
      "peachpuff",
      "peru",
      "pink",
      "plum",
      "powderblue",
      "purple",
      "red",
      "rosybrown",
      "royalblue",
      "saddlebrown",
      "salmon",
      "sandybrown",
      "seagreen",
      "seashell",
      "sienna",
      "silver",
      "skyblue",
      "slateblue",
      "slategray",
      "snow",
      "springgreen",
      "steelblue",
      "tan",
      "teal",
      "thistle",
      "tomato",
      "turquoise",
      "violet",
      "wheat",
      "white",
      "whitesmoke",
      "yellow",
      "yellowgreen"
    };
    private static readonly string[] _systemColors = new string[28]
    {
      "activeborder",
      "activecaption",
      "appworkspace",
      "background",
      "buttonface",
      "buttonhighlight",
      "buttonshadow",
      "buttontext",
      "captiontext",
      "graytext",
      "highlight",
      "highlighttext",
      "inactiveborder",
      "inactivecaption",
      "inactivecaptiontext",
      "infobackground",
      "infotext",
      "menu",
      "menutext",
      "scrollbar",
      "threeddarkshadow",
      "threedface",
      "threedhighlight",
      "threedlightshadow",
      "threedshadow",
      "window",
      "windowframe",
      "windowtext"
    };
    private static readonly string[] _fontGenericFamilies = new string[5]
    {
      "serif",
      "sans-serif",
      "monospace",
      "cursive",
      "fantasy"
    };
    private static readonly string[] _fontStyles = new string[3]
    {
      "normal",
      "italic",
      "oblique"
    };
    private static readonly string[] _fontVariants = new string[2]
    {
      "normal",
      "small-caps"
    };
    private static readonly string[] _fontWeights = new string[13]
    {
      "normal",
      "bold",
      "bolder",
      "lighter",
      "100",
      "200",
      "300",
      "400",
      "500",
      "600",
      "700",
      "800",
      "900"
    };
    private static readonly string[] _fontAbsoluteSizes = new string[7]
    {
      "xx-small",
      "x-small",
      "small",
      "medium",
      "large",
      "x-large",
      "xx-large"
    };
    private static readonly string[] _fontRelativeSizes = new string[2]
    {
      "larger",
      "smaller"
    };
    private static readonly string[] _fontSizeUnits = new string[9]
    {
      "px",
      "mm",
      "cm",
      "in",
      "pt",
      "pc",
      "em",
      "ex",
      "%"
    };
    private static readonly string[] _flowDirections = new string[2]
    {
      "rtl",
      "ltr"
    };
    private static readonly string[] _listStyleTypes = new string[9]
    {
      "disc",
      "circle",
      "square",
      "decimal",
      "lower-roman",
      "upper-roman",
      "lower-alpha",
      "upper-alpha",
      "none"
    };
    private static readonly string[] _listStylePositions = new string[2]
    {
      "inside",
      "outside"
    };
    private static readonly string[] _textDecorations = new string[5]
    {
      "none",
      "underline",
      "overline",
      "line-through",
      "blink"
    };
    private static readonly string[] _textTransforms = new string[4]
    {
      "none",
      "capitalize",
      "uppercase",
      "lowercase"
    };
    private static readonly string[] _textAligns = new string[4]
    {
      "left",
      "right",
      "center",
      "justify"
    };
    private static readonly string[] _verticalAligns = new string[8]
    {
      "baseline",
      "sub",
      "super",
      "top",
      "text-top",
      "middle",
      "bottom",
      "text-bottom"
    };
    private static readonly string[] _floats = new string[3]
    {
      "left",
      "right",
      "none"
    };
    private static readonly string[] _clears = new string[4]
    {
      "none",
      "left",
      "right",
      "both"
    };
    private static readonly string[] _borderStyles = new string[9]
    {
      "none",
      "dotted",
      "dashed",
      "solid",
      "double",
      "groove",
      "ridge",
      "inset",
      "outset"
    };
    private static string[] _blocks = new string[4]
    {
      "block",
      "inline",
      "list-item",
      "none"
    };

    internal static void GetElementPropertiesFromCssAttributes(
      XmlElement htmlElement,
      string elementName,
      CssStylesheet stylesheet,
      Hashtable localProperties,
      List<XmlElement> sourceContext)
    {
      string style = stylesheet.GetStyle(elementName, sourceContext);
      string attribute = HtmlToXamlConverter.GetAttribute(htmlElement, "style");
      string str1 = style ?? (string) null;
      if (attribute != null)
        str1 = str1 == null ? attribute : str1 + ";" + attribute;
      if (str1 == null)
        return;
      string str2 = str1;
      char[] chArray1 = new char[1]{ ';' };
      foreach (string str3 in str2.Split(chArray1))
      {
        char[] chArray2 = new char[1]{ ':' };
        string[] strArray = str3.Split(chArray2);
        if (strArray.Length == 2)
        {
          string lowerInvariant1 = strArray[0].Trim().ToLowerInvariant();
          string lowerInvariant2 = HtmlToXamlConverter.UnQuote(strArray[1].Trim()).ToLowerInvariant();
          int nextIndex = 0;
          if (lowerInvariant1 != null)
          {
            switch (lowerInvariant1.Length)
            {
              case 3:
                if (lowerInvariant1 == "dir")
                {
                  HtmlCssParser.ParseCssDir(lowerInvariant2, ref nextIndex, localProperties);
                  continue;
                }
                continue;
              case 4:
                if (lowerInvariant1 == "font")
                {
                  HtmlCssParser.ParseCssFont(lowerInvariant2, localProperties);
                  continue;
                }
                continue;
              case 5:
                switch (lowerInvariant1[2])
                {
                  case 'd':
                    if (lowerInvariant1 == "width")
                      goto label_86;
                    else
                      continue;
                  case 'e':
                    if (lowerInvariant1 == "clear")
                    {
                      HtmlCssParser.ParseCssClear(lowerInvariant2, ref nextIndex, localProperties);
                      continue;
                    }
                    continue;
                  case 'l':
                    if (lowerInvariant1 == "color")
                    {
                      HtmlCssParser.ParseCssColor(lowerInvariant2, ref nextIndex, localProperties, "color");
                      continue;
                    }
                    continue;
                  case 'o':
                    if (lowerInvariant1 == "float")
                    {
                      HtmlCssParser.ParseCssFloat(lowerInvariant2, ref nextIndex, localProperties);
                      continue;
                    }
                    continue;
                  default:
                    continue;
                }
              case 6:
                switch (lowerInvariant1[0])
                {
                  case 'b':
                    if (lowerInvariant1 == "border")
                    {
                      HtmlCssParser.ParseCssBorder(lowerInvariant2, ref nextIndex, localProperties);
                      continue;
                    }
                    continue;
                  case 'h':
                    if (lowerInvariant1 == "height")
                      goto label_86;
                    else
                      continue;
                  case 'm':
                    if (lowerInvariant1 == "margin")
                    {
                      HtmlCssParser.ParseCssRectangleProperty(lowerInvariant2, ref nextIndex, localProperties, lowerInvariant1);
                      continue;
                    }
                    continue;
                  default:
                    continue;
                }
              case 7:
                switch (lowerInvariant1[0])
                {
                  case 'd':
                    if (lowerInvariant1 == "display")
                      continue;
                    continue;
                  case 'p':
                    if (lowerInvariant1 == "padding")
                    {
                      HtmlCssParser.ParseCssRectangleProperty(lowerInvariant2, ref nextIndex, localProperties, lowerInvariant1);
                      continue;
                    }
                    continue;
                  default:
                    continue;
                }
              case 9:
                if (lowerInvariant1 == "font-size")
                {
                  HtmlCssParser.ParseCssSize(lowerInvariant2, ref nextIndex, localProperties, "font-size", true);
                  continue;
                }
                continue;
              case 10:
                switch (lowerInvariant1[0])
                {
                  case 'b':
                    switch (lowerInvariant1)
                    {
                      case "background":
                        HtmlCssParser.ParseCssColor(lowerInvariant2, ref nextIndex, localProperties, "background");
                        continue;
                      default:
                        continue;
                    }
                  case 'f':
                    if (lowerInvariant1 == "font-style")
                    {
                      HtmlCssParser.ParseCssFontStyle(lowerInvariant2, ref nextIndex, localProperties);
                      continue;
                    }
                    continue;
                  case 'm':
                    if (lowerInvariant1 == "margin-top")
                      goto label_88;
                    else
                      continue;
                  case 't':
                    if (lowerInvariant1 == "text-align")
                    {
                      HtmlCssParser.ParseCssTextAlign(lowerInvariant2, ref nextIndex, localProperties);
                      continue;
                    }
                    continue;
                  default:
                    continue;
                }
              case 11:
                switch (lowerInvariant1[0])
                {
                  case 'b':
                    if (lowerInvariant1 == "border-left")
                      continue;
                    continue;
                  case 'f':
                    switch (lowerInvariant1)
                    {
                      case "font-family":
                        break;
                      case "font-weight":
                        HtmlCssParser.ParseCssFontWeight(lowerInvariant2, ref nextIndex, localProperties);
                        continue;
                      default:
                        continue;
                    }
                    break;
                  case 'l':
                    if (lowerInvariant1 == "line-height")
                    {
                      HtmlCssParser.ParseCssSize(lowerInvariant2, ref nextIndex, localProperties, "line-height", true);
                      continue;
                    }
                    continue;
                  case 'm':
                    if (lowerInvariant1 == "margin-left")
                      goto label_88;
                    else
                      continue;
                  case 'p':
                    if (lowerInvariant1 == "padding-top")
                      goto label_90;
                    else
                      continue;
                  case 't':
                    if (lowerInvariant1 == "text-indent")
                    {
                      HtmlCssParser.ParseCssSize(lowerInvariant2, ref nextIndex, localProperties, "text-indent", false);
                      continue;
                    }
                    continue;
                  default:
                    continue;
                }
              case 12:
                switch (lowerInvariant1[9])
                {
                  case 'a':
                    if (lowerInvariant1 == "font-variant")
                    {
                      HtmlCssParser.ParseCssFontVariant(lowerInvariant2, ref nextIndex, localProperties);
                      continue;
                    }
                    continue;
                  case 'd':
                    if (lowerInvariant1 == "border-width")
                      break;
                    continue;
                  case 'e':
                    if (lowerInvariant1 == "padding-left")
                      goto label_90;
                    else
                      continue;
                  case 'g':
                    switch (lowerInvariant1)
                    {
                      case "margin-right":
                        goto label_88;
                      default:
                        continue;
                    }
                  case 'i':
                    if (lowerInvariant1 == "word-spacing")
                      continue;
                    continue;
                  case 'l':
                    if (lowerInvariant1 == "border-color")
                      break;
                    continue;
                  case 'y':
                    if (lowerInvariant1 == "border-style")
                      break;
                    continue;
                  default:
                    continue;
                }
                HtmlCssParser.ParseCssRectangleProperty(lowerInvariant2, ref nextIndex, localProperties, lowerInvariant1);
                continue;
              case 13:
                switch (lowerInvariant1[0])
                {
                  case 'b':
                    if (lowerInvariant1 == "border-bottom")
                      continue;
                    continue;
                  case 'm':
                    if (lowerInvariant1 == "margin-bottom")
                      goto label_88;
                    else
                      continue;
                  case 'p':
                    if (lowerInvariant1 == "padding-right")
                      goto label_90;
                    else
                      continue;
                  default:
                    continue;
                }
              case 14:
                switch (lowerInvariant1[0])
                {
                  case 'l':
                    if (lowerInvariant1 == "letter-spacing")
                      continue;
                    continue;
                  case 'p':
                    if (lowerInvariant1 == "padding-bottom")
                      goto label_90;
                    else
                      continue;
                  case 't':
                    if (lowerInvariant1 == "text-transform")
                    {
                      HtmlCssParser.ParseCssTextTransform(lowerInvariant2, ref nextIndex, localProperties);
                      continue;
                    }
                    continue;
                  case 'v':
                    if (lowerInvariant1 == "vertical-align")
                    {
                      HtmlCssParser.ParseCssVerticalAlign(lowerInvariant2, ref nextIndex, localProperties);
                      continue;
                    }
                    continue;
                  default:
                    continue;
                }
              case 15:
                if (lowerInvariant1 == "text-decoration")
                {
                  HtmlCssParser.ParseCssTextDecoration(lowerInvariant2, ref nextIndex, localProperties);
                  continue;
                }
                continue;
              case 16:
                switch (lowerInvariant1[11])
                {
                  case 'c':
                    switch (lowerInvariant1)
                    {
                      case "background-color":
                        HtmlCssParser.ParseCssColor(lowerInvariant2, ref nextIndex, localProperties, "background-color");
                        continue;
                      default:
                        continue;
                    }
                  case 's':
                    if (lowerInvariant1 == "border-top-style")
                      continue;
                    continue;
                  case 'w':
                    if (lowerInvariant1 == "border-top-width")
                      continue;
                    continue;
                  default:
                    continue;
                }
              case 17:
                switch (lowerInvariant1[12])
                {
                  case 'c':
                    if (lowerInvariant1 == "border-left-color")
                      continue;
                    continue;
                  case 's':
                    if (lowerInvariant1 == "border-left-style")
                      continue;
                    continue;
                  case 'w':
                    if (lowerInvariant1 == "border-left-width")
                      continue;
                    continue;
                  default:
                    continue;
                }
              case 18:
                switch (lowerInvariant1[13])
                {
                  case 'c':
                    if (lowerInvariant1 == "border-right-color")
                      continue;
                    continue;
                  case 's':
                    if (lowerInvariant1 == "border-right-style")
                      continue;
                    continue;
                  case 'w':
                    if (lowerInvariant1 == "border-right-width")
                      continue;
                    continue;
                  default:
                    continue;
                }
              case 19:
                switch (lowerInvariant1[14])
                {
                  case 'c':
                    if (lowerInvariant1 == "border-bottom-color")
                      continue;
                    continue;
                  case 's':
                    if (lowerInvariant1 == "border-bottom-style")
                      continue;
                    continue;
                  case 'w':
                    if (lowerInvariant1 == "border-bottom-width")
                      continue;
                    continue;
                  default:
                    continue;
                }
              case 21:
                if (lowerInvariant1 == "mso-ascii-font-family")
                  break;
                continue;
              default:
                continue;
            }
            HtmlCssParser.ParseCssFontFamily(lowerInvariant2, ref nextIndex, localProperties);
            continue;
label_86:
            HtmlCssParser.ParseCssSize(lowerInvariant2, ref nextIndex, localProperties, lowerInvariant1, true);
            continue;
label_88:
            HtmlCssParser.ParseCssSize(lowerInvariant2, ref nextIndex, localProperties, lowerInvariant1, true);
            continue;
label_90:
            HtmlCssParser.ParseCssSize(lowerInvariant2, ref nextIndex, localProperties, lowerInvariant1, true);
          }
        }
      }
    }

    private static void ParseWhiteSpace(string styleValue, ref int nextIndex)
    {
      while (nextIndex < styleValue.Length && char.IsWhiteSpace(styleValue[nextIndex]))
        ++nextIndex;
    }

    private static bool ParseWord(string word, string styleValue, ref int nextIndex)
    {
      HtmlCssParser.ParseWhiteSpace(styleValue, ref nextIndex);
      for (int index = 0; index < word.Length; ++index)
      {
        if (nextIndex + index >= styleValue.Length || (int) word[index] != (int) styleValue[nextIndex + index])
          return false;
      }
      if (nextIndex + word.Length < styleValue.Length && char.IsLetterOrDigit(styleValue[nextIndex + word.Length]))
        return false;
      nextIndex += word.Length;
      return true;
    }

    private static string ParseWordEnumeration(
      string[] words,
      string styleValue,
      ref int nextIndex)
    {
      for (int index = 0; index < words.Length; ++index)
      {
        if (HtmlCssParser.ParseWord(words[index], styleValue, ref nextIndex))
          return words[index];
      }
      return (string) null;
    }

    private static void ParseWordEnumeration(
      string[] words,
      string styleValue,
      ref int nextIndex,
      Hashtable localProperties,
      string attributeName)
    {
      string wordEnumeration = HtmlCssParser.ParseWordEnumeration(words, styleValue, ref nextIndex);
      if (wordEnumeration == null)
        return;
      localProperties[(object) attributeName] = (object) wordEnumeration;
    }

    private static string ParseCssSize(
      string styleValue,
      ref int nextIndex,
      bool mustBeNonNegative)
    {
      HtmlCssParser.ParseWhiteSpace(styleValue, ref nextIndex);
      int num = nextIndex;
      if (nextIndex < styleValue.Length && styleValue[nextIndex] == '-')
        ++nextIndex;
      if (nextIndex >= styleValue.Length || !char.IsDigit(styleValue[nextIndex]))
        return (string) null;
      while (nextIndex < styleValue.Length && (char.IsDigit(styleValue[nextIndex]) || styleValue[nextIndex] == '.'))
        ++nextIndex;
      string str1 = styleValue.Substring(num, nextIndex - num);
      string str2 = HtmlCssParser.ParseWordEnumeration(HtmlCssParser._fontSizeUnits, styleValue, ref nextIndex) ?? "px";
      return mustBeNonNegative && styleValue[num] == '-' ? "0" : str1 + str2;
    }

    private static void ParseCssSize(
      string styleValue,
      ref int nextIndex,
      Hashtable localValues,
      string propertyName,
      bool mustBeNonNegative)
    {
      string cssSize = HtmlCssParser.ParseCssSize(styleValue, ref nextIndex, mustBeNonNegative);
      if (cssSize == null)
        return;
      localValues[(object) propertyName] = (object) cssSize;
    }

    private static string ParseCssColor(string styleValue, ref int nextIndex)
    {
      HtmlCssParser.ParseWhiteSpace(styleValue, ref nextIndex);
      string cssColor = (string) null;
      if (nextIndex < styleValue.Length)
      {
        int startIndex = nextIndex;
        char c = styleValue[nextIndex];
        if (c == '#')
        {
          ++nextIndex;
          while (nextIndex < styleValue.Length)
          {
            char upper = char.ToUpper(styleValue[nextIndex]);
            if ('0' <= upper && upper <= '9' || 'A' <= upper && upper <= 'F')
              ++nextIndex;
            else
              break;
          }
          if (nextIndex > startIndex + 1)
            cssColor = styleValue.Substring(startIndex, nextIndex - startIndex);
        }
        else if (string.Equals(styleValue.Substring(nextIndex, 3), "rbg", StringComparison.OrdinalIgnoreCase))
        {
          while (nextIndex < styleValue.Length && styleValue[nextIndex] != ')')
            ++nextIndex;
          if (nextIndex < styleValue.Length)
            ++nextIndex;
          cssColor = "gray";
        }
        else if (char.IsLetter(c))
        {
          cssColor = HtmlCssParser.ParseWordEnumeration(HtmlCssParser._colors, styleValue, ref nextIndex);
          if (cssColor == null)
          {
            cssColor = HtmlCssParser.ParseWordEnumeration(HtmlCssParser._systemColors, styleValue, ref nextIndex);
            if (cssColor != null)
              cssColor = "black";
          }
        }
      }
      return cssColor;
    }

    private static void ParseCssColor(
      string styleValue,
      ref int nextIndex,
      Hashtable localValues,
      string propertyName)
    {
      string cssColor = HtmlCssParser.ParseCssColor(styleValue, ref nextIndex);
      if (cssColor == null)
        return;
      localValues[(object) propertyName] = (object) cssColor;
    }

    private static void ParseCssFont(string styleValue, Hashtable localProperties)
    {
      int nextIndex = 0;
      HtmlCssParser.ParseCssFontStyle(styleValue, ref nextIndex, localProperties);
      HtmlCssParser.ParseCssFontVariant(styleValue, ref nextIndex, localProperties);
      HtmlCssParser.ParseCssFontWeight(styleValue, ref nextIndex, localProperties);
      HtmlCssParser.ParseCssSize(styleValue, ref nextIndex, localProperties, "font-size", true);
      HtmlCssParser.ParseWhiteSpace(styleValue, ref nextIndex);
      if (nextIndex < styleValue.Length && styleValue[nextIndex] == '/')
      {
        ++nextIndex;
        HtmlCssParser.ParseCssSize(styleValue, ref nextIndex, localProperties, "line-height", true);
      }
      HtmlCssParser.ParseCssFontFamily(styleValue, ref nextIndex, localProperties);
    }

    private static void ParseCssFontStyle(
      string styleValue,
      ref int nextIndex,
      Hashtable localProperties)
    {
      HtmlCssParser.ParseWordEnumeration(HtmlCssParser._fontStyles, styleValue, ref nextIndex, localProperties, "font-style");
    }

    private static void ParseCssFontVariant(
      string styleValue,
      ref int nextIndex,
      Hashtable localProperties)
    {
      HtmlCssParser.ParseWordEnumeration(HtmlCssParser._fontVariants, styleValue, ref nextIndex, localProperties, "font-variant");
    }

    private static void ParseCssFontWeight(
      string styleValue,
      ref int nextIndex,
      Hashtable localProperties)
    {
      HtmlCssParser.ParseWordEnumeration(HtmlCssParser._fontWeights, styleValue, ref nextIndex, localProperties, "font-weight");
    }

    private static void ParseCssFontFamily(
      string styleValue,
      ref int nextIndex,
      Hashtable localProperties)
    {
      string str1 = (string) null;
      while (nextIndex < styleValue.Length)
      {
        string str2 = HtmlCssParser.ParseWordEnumeration(HtmlCssParser._fontGenericFamilies, styleValue, ref nextIndex);
        if (str2 == null)
        {
          if (nextIndex < styleValue.Length && (styleValue[nextIndex] == '"' || styleValue[nextIndex] == '\''))
          {
            char ch = styleValue[nextIndex];
            ++nextIndex;
            int startIndex = nextIndex;
            while (nextIndex < styleValue.Length && (int) styleValue[nextIndex] != (int) ch)
              ++nextIndex;
            str2 = "\"" + styleValue.Substring(startIndex, nextIndex - startIndex) + "\"";
          }
          if (str2 == null)
          {
            int startIndex = nextIndex;
            while (nextIndex < styleValue.Length && styleValue[nextIndex] != ',' && styleValue[nextIndex] != ';')
              ++nextIndex;
            if (nextIndex > startIndex)
            {
              str2 = styleValue.Substring(startIndex, nextIndex - startIndex).Trim();
              if (str2.Length == 0)
                str2 = (string) null;
            }
          }
        }
        HtmlCssParser.ParseWhiteSpace(styleValue, ref nextIndex);
        if (nextIndex < styleValue.Length && styleValue[nextIndex] == ',')
          ++nextIndex;
        if (str2 != null)
        {
          if (str1 == null && str2.Length > 0)
          {
            if (str2[0] == '"' || str2[0] == '\'')
              str2 = str2.Substring(1, str2.Length - 2);
            if (str2[str2.Length - 1] == '"')
              str2 = str2.Substring(0, str2.Length - 1);
            str1 = str2;
          }
        }
        else
          break;
      }
      if (str1 == null)
        return;
      localProperties[(object) "font-family"] = (object) str1;
    }

    private static void ParseCssListStyle(string styleValue, Hashtable localProperties)
    {
      int nextIndex = 0;
      while (nextIndex < styleValue.Length)
      {
        string cssListStyleType = HtmlCssParser.ParseCssListStyleType(styleValue, ref nextIndex);
        if (cssListStyleType != null)
        {
          localProperties[(object) "list-style-type"] = (object) cssListStyleType;
        }
        else
        {
          string listStylePosition = HtmlCssParser.ParseCssListStylePosition(styleValue, ref nextIndex);
          if (listStylePosition != null)
          {
            localProperties[(object) "list-style-position"] = (object) listStylePosition;
          }
          else
          {
            string cssListStyleImage = HtmlCssParser.ParseCssListStyleImage(styleValue, ref nextIndex);
            if (cssListStyleImage == null)
              break;
            localProperties[(object) "list-style-image"] = (object) cssListStyleImage;
          }
        }
      }
    }

    private static string ParseCssListStyleType(string styleValue, ref int nextIndex) => HtmlCssParser.ParseWordEnumeration(HtmlCssParser._listStyleTypes, styleValue, ref nextIndex);

    private static string ParseCssListStylePosition(string styleValue, ref int nextIndex) => HtmlCssParser.ParseWordEnumeration(HtmlCssParser._listStylePositions, styleValue, ref nextIndex);

    private static string ParseCssListStyleImage(string styleValue, ref int nextIndex) => (string) null;

    private static void ParseCssTextDecoration(
      string styleValue,
      ref int nextIndex,
      Hashtable localProperties)
    {
      for (int index = 1; index < HtmlCssParser._textDecorations.Length; ++index)
        localProperties[(object) ("text-decoration-" + HtmlCssParser._textDecorations[index])] = (object) "false";
      while (nextIndex < styleValue.Length)
      {
        string wordEnumeration = HtmlCssParser.ParseWordEnumeration(HtmlCssParser._textDecorations, styleValue, ref nextIndex);
        if (wordEnumeration == null || string.Equals(wordEnumeration, "none", StringComparison.OrdinalIgnoreCase))
          break;
        localProperties[(object) ("text-decoration-" + wordEnumeration)] = (object) "true";
      }
    }

    private static void ParseCssTextTransform(
      string styleValue,
      ref int nextIndex,
      Hashtable localProperties)
    {
      HtmlCssParser.ParseWordEnumeration(HtmlCssParser._textTransforms, styleValue, ref nextIndex, localProperties, "text-transform");
    }

    private static void ParseCssTextAlign(
      string styleValue,
      ref int nextIndex,
      Hashtable localProperties)
    {
      HtmlCssParser.ParseWordEnumeration(HtmlCssParser._textAligns, styleValue, ref nextIndex, localProperties, "text-align");
    }

    private static void ParseCssVerticalAlign(
      string styleValue,
      ref int nextIndex,
      Hashtable localProperties)
    {
      HtmlCssParser.ParseWordEnumeration(HtmlCssParser._verticalAligns, styleValue, ref nextIndex, localProperties, "vertical-align");
    }

    private static void ParseCssFloat(
      string styleValue,
      ref int nextIndex,
      Hashtable localProperties)
    {
      HtmlCssParser.ParseWordEnumeration(HtmlCssParser._floats, styleValue, ref nextIndex, localProperties, "float");
    }

    private static void ParseCssClear(
      string styleValue,
      ref int nextIndex,
      Hashtable localProperties)
    {
      HtmlCssParser.ParseWordEnumeration(HtmlCssParser._clears, styleValue, ref nextIndex, localProperties, "clear");
    }

    private static bool ParseCssRectangleProperty(
      string styleValue,
      ref int nextIndex,
      Hashtable localProperties,
      string propertyName)
    {
      string str1 = string.Equals(propertyName, "border-color", StringComparison.OrdinalIgnoreCase) ? HtmlCssParser.ParseCssColor(styleValue, ref nextIndex) : (string.Equals(propertyName, "border-style", StringComparison.OrdinalIgnoreCase) ? HtmlCssParser.ParseCssBorderStyle(styleValue, ref nextIndex) : HtmlCssParser.ParseCssSize(styleValue, ref nextIndex, true));
      if (str1 == null)
        return false;
      localProperties[(object) (propertyName + "-top")] = (object) str1;
      localProperties[(object) (propertyName + "-bottom")] = (object) str1;
      localProperties[(object) (propertyName + "-right")] = (object) str1;
      localProperties[(object) (propertyName + "-left")] = (object) str1;
      string str2 = string.Equals(propertyName, "border-color", StringComparison.OrdinalIgnoreCase) ? HtmlCssParser.ParseCssColor(styleValue, ref nextIndex) : (string.Equals(propertyName, "border-style", StringComparison.OrdinalIgnoreCase) ? HtmlCssParser.ParseCssBorderStyle(styleValue, ref nextIndex) : HtmlCssParser.ParseCssSize(styleValue, ref nextIndex, true));
      if (str2 != null)
      {
        localProperties[(object) (propertyName + "-right")] = (object) str2;
        localProperties[(object) (propertyName + "-left")] = (object) str2;
        string str3 = string.Equals(propertyName, "border-color", StringComparison.OrdinalIgnoreCase) ? HtmlCssParser.ParseCssColor(styleValue, ref nextIndex) : (string.Equals(propertyName, "border-style", StringComparison.OrdinalIgnoreCase) ? HtmlCssParser.ParseCssBorderStyle(styleValue, ref nextIndex) : HtmlCssParser.ParseCssSize(styleValue, ref nextIndex, true));
        if (str3 != null)
        {
          localProperties[(object) (propertyName + "-bottom")] = (object) str3;
          string str4 = string.Equals(propertyName, "border-color", StringComparison.OrdinalIgnoreCase) ? HtmlCssParser.ParseCssColor(styleValue, ref nextIndex) : (string.Equals(propertyName, "border-style", StringComparison.OrdinalIgnoreCase) ? HtmlCssParser.ParseCssBorderStyle(styleValue, ref nextIndex) : HtmlCssParser.ParseCssSize(styleValue, ref nextIndex, true));
          if (str4 != null)
            localProperties[(object) (propertyName + "-left")] = (object) str4;
        }
      }
      return true;
    }

    private static void ParseCssBorder(
      string styleValue,
      ref int nextIndex,
      Hashtable localProperties)
    {
      do
        ;
      while (HtmlCssParser.ParseCssRectangleProperty(styleValue, ref nextIndex, localProperties, "border-width") || HtmlCssParser.ParseCssRectangleProperty(styleValue, ref nextIndex, localProperties, "border-style") || HtmlCssParser.ParseCssRectangleProperty(styleValue, ref nextIndex, localProperties, "border-color"));
    }

    private static string ParseCssBorderStyle(string styleValue, ref int nextIndex) => HtmlCssParser.ParseWordEnumeration(HtmlCssParser._borderStyles, styleValue, ref nextIndex);

    private static void ParseCssBackground(
      string styleValue,
      ref int nextIndex,
      Hashtable localValues)
    {
    }

    private static void ParseCssDir(
      string styleValue,
      ref int nextIndex,
      Hashtable localProperties)
    {
      HtmlCssParser.ParseWordEnumeration(HtmlCssParser._flowDirections, styleValue, ref nextIndex, localProperties, "dir");
    }
  }
}
