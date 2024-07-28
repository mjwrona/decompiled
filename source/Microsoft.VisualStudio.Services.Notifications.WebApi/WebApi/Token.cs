// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.WebApi.Token
// Assembly: Microsoft.VisualStudio.Services.Notifications.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF217E0A-7730-437B-BE9F-877363CB7392
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.WebApi.dll

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.VisualStudio.Services.Notifications.WebApi
{
  public abstract class Token
  {
    protected byte m_type;
    protected string m_spelling;
    protected string m_queriableValue;
    protected bool m_isXPath;
    protected bool m_hasMacros;
    protected bool m_isCaseInsensitve;
    public const byte IntLiteral = 0;
    public const byte StringLiteral = 1;
    public const byte DateLiteral = 2;
    public const byte Null = 3;
    public const byte Today = 4;
    public const byte OPOr = 5;
    public const byte OPAnd = 6;
    public const byte OPNot = 7;
    public const byte OPLT = 8;
    public const byte OPGT = 9;
    public const byte OPLTE = 10;
    public const byte OPGTE = 11;
    public const byte OPEqual = 12;
    public const byte OPNotEqual = 13;
    public const byte OPUnder = 14;
    public const byte OPMatch = 15;
    public const byte OPLike = 16;
    public const byte OPDynamic = 17;
    public const byte OPAdd = 18;
    public const byte OPSubtract = 19;
    public const byte ParenLeft = 20;
    public const byte ParenRight = 21;
    public const byte OPDate = 22;
    public const byte True = 23;
    public const byte False = 24;
    public const byte OPNotMatch = 25;
    public const byte OPUnderPath = 26;
    public const byte OPContainsValue = 27;
    public const byte OPDoesNotContainValue = 28;
    public const byte OPMemberOf = 29;
    public static readonly string[] spellings = new string[30]
    {
      "<IntLiteral>",
      "<StringLiteral>",
      "<DateLiteral>",
      "null",
      "today",
      "or",
      "and",
      "not",
      "<",
      ">",
      "<=",
      ">=",
      "=",
      "<>",
      "under",
      "match",
      "like",
      "dynamic",
      "+",
      "-",
      "(",
      ")",
      "date",
      "true",
      "false",
      "notmatch",
      "underpath",
      "containsValue",
      "doesNotContainValue",
      "memberOf"
    };
    protected static char[] replacementChars = new char[2]
    {
      '\v',
      '\a'
    };
    protected const string SingleQuoteValue = "\"'\"";
    protected const string DoubleQuoteValue = "'\"'";
    protected const string SingleQuoteCharValue = "'";
    protected const string DoubleQuoteCharValue = "\"";

    public Token()
      : this(string.Empty)
    {
    }

    public Token(string spelling)
    {
      this.m_type = (byte) 3;
      this.Spelling = spelling;
    }

    public Token(byte type, string spelling)
    {
      this.m_type = type;
      this.Spelling = spelling;
    }

    public byte TokenType
    {
      get => this.m_type;
      set => this.m_type = value;
    }

    public virtual string Spelling
    {
      get => this.m_spelling;
      protected set => this.m_spelling = value;
    }

    public string QueriableValue => this.m_queriableValue;

    public virtual string EscapeSpecialChatactersIfNeeded(
      bool UseSingleQuoteChar = true,
      bool replaceSpecialCharsWithToken = true)
    {
      return this.Spelling;
    }

    public bool IsXPathExpression => this.m_isXPath;

    public bool HasMacros => this.m_hasMacros;

    public string EvaluateToken(Dictionary<string, string> macros)
    {
      string token = this.m_queriableValue;
      if (this.m_hasMacros && macros != null)
      {
        foreach (string key in macros.Keys)
          token = this.ReplaceString(key, macros[key], token);
      }
      return token;
    }

    internal string ReplaceString(string match, string value, string token = null)
    {
      if (token == null)
        token = this.m_queriableValue;
      return !this.m_isCaseInsensitve ? token.Replace(match, value) : this.ReplaceString(match, value?.ToLower(), token, StringComparison.OrdinalIgnoreCase);
    }

    internal string ReplaceString(
      string match,
      string value,
      string token,
      StringComparison comparison)
    {
      int num = token.IndexOf(match, comparison);
      if (num < 0)
        return token;
      StringBuilder stringBuilder = new StringBuilder();
      int startIndex1 = 0;
      int startIndex2;
      for (; num != -1; num = token.IndexOf(match, startIndex2, comparison))
      {
        stringBuilder.Append(token.Substring(startIndex1, num - startIndex1));
        stringBuilder.Append(value);
        startIndex2 = num + match.Length;
        startIndex1 = startIndex2;
      }
      stringBuilder.Append(token.Substring(startIndex1));
      return stringBuilder.ToString();
    }

    protected virtual string Unescape() => this.m_spelling;

    public virtual string EvaluatePathFunctions() => this.m_spelling;

    public bool IsBoolOperator()
    {
      switch (this.m_type)
      {
        case 8:
        case 9:
        case 10:
        case 11:
        case 12:
        case 13:
        case 14:
        case 15:
        case 16:
        case 17:
        case 25:
        case 26:
        case 27:
        case 28:
        case 29:
          return true;
        default:
          return false;
      }
    }

    public override string ToString() => this.m_spelling != null ? this.m_spelling : Token.spellings[(int) this.m_type];

    public static string GetOperatorString(byte op) => op < (byte) 0 || (int) op >= Token.spellings.Length ? "invalidOp" : Token.spellings[(int) op];
  }
}
