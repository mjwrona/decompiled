// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.JSKeyword
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

namespace Microsoft.Ajax.Utilities
{
  internal sealed class JSKeyword
  {
    private JSKeyword m_next;
    private JSToken m_token;
    private string m_name;
    private int m_length;

    private JSKeyword(JSToken token, string name)
      : this(token, name, (JSKeyword) null)
    {
    }

    private JSKeyword(JSToken token, string name, JSKeyword next)
    {
      this.m_name = name;
      this.m_token = token;
      this.m_length = this.m_name.Length;
      this.m_next = next;
    }

    internal static string CanBeIdentifier(JSToken keyword)
    {
      switch (keyword)
      {
        case JSToken.Super:
          return "super";
        case JSToken.Module:
          return "module";
        case JSToken.Let:
          return "let";
        case JSToken.Implements:
          return "implements";
        case JSToken.Interface:
          return "interface";
        case JSToken.Package:
          return "package";
        case JSToken.Private:
          return "private";
        case JSToken.Protected:
          return "protected";
        case JSToken.Public:
          return "public";
        case JSToken.Static:
          return "static";
        case JSToken.Yield:
          return "yield";
        case JSToken.Native:
          return "native";
        case JSToken.Get:
          return "get";
        case JSToken.Set:
          return "set";
        default:
          return (string) null;
      }
    }

    internal JSToken GetKeyword(string source, int startPosition, int wordLength)
    {
      for (JSKeyword jsKeyword = this; jsKeyword != null; jsKeyword = jsKeyword.m_next)
      {
        if (wordLength == jsKeyword.m_length)
        {
          int num = string.CompareOrdinal(jsKeyword.m_name, 0, source, startPosition, wordLength);
          if (num == 0)
            return jsKeyword.m_token;
          if (num > 0)
            return JSToken.Identifier;
        }
        else if (wordLength < jsKeyword.m_length)
          return JSToken.Identifier;
      }
      return JSToken.Identifier;
    }

    internal static JSKeyword[] InitKeywords() => new JSKeyword[26]
    {
      null,
      new JSKeyword(JSToken.Break, "break"),
      new JSKeyword(JSToken.Case, "case", new JSKeyword(JSToken.Catch, "catch", new JSKeyword(JSToken.Class, "class", new JSKeyword(JSToken.Const, "const", new JSKeyword(JSToken.Continue, "continue"))))),
      new JSKeyword(JSToken.Do, "do", new JSKeyword(JSToken.FirstOperator, "delete", new JSKeyword(JSToken.Default, "default", new JSKeyword(JSToken.Debugger, "debugger")))),
      new JSKeyword(JSToken.Else, "else", new JSKeyword(JSToken.Enum, "enum", new JSKeyword(JSToken.Export, "export", new JSKeyword(JSToken.Extends, "extends")))),
      new JSKeyword(JSToken.For, "for", new JSKeyword(JSToken.False, "false", new JSKeyword(JSToken.Finally, "finally", new JSKeyword(JSToken.Function, "function")))),
      new JSKeyword(JSToken.Get, "get"),
      null,
      new JSKeyword(JSToken.If, "if", new JSKeyword(JSToken.In, "in", new JSKeyword(JSToken.Import, "import", new JSKeyword(JSToken.Interface, "interface", new JSKeyword(JSToken.Implements, "implements", new JSKeyword(JSToken.InstanceOf, "instanceof")))))),
      null,
      null,
      new JSKeyword(JSToken.Let, "let"),
      null,
      new JSKeyword(JSToken.New, "new", new JSKeyword(JSToken.Null, "null", new JSKeyword(JSToken.Native, "native"))),
      null,
      new JSKeyword(JSToken.Public, "public", new JSKeyword(JSToken.Package, "package", new JSKeyword(JSToken.Private, "private", new JSKeyword(JSToken.Protected, "protected")))),
      null,
      new JSKeyword(JSToken.Return, "return"),
      new JSKeyword(JSToken.Set, "set", new JSKeyword(JSToken.Super, "super", new JSKeyword(JSToken.Static, "static", new JSKeyword(JSToken.Switch, "switch")))),
      new JSKeyword(JSToken.Try, "try", new JSKeyword(JSToken.This, "this", new JSKeyword(JSToken.True, "true", new JSKeyword(JSToken.Throw, "throw", new JSKeyword(JSToken.TypeOf, "typeof"))))),
      null,
      new JSKeyword(JSToken.Var, "var", new JSKeyword(JSToken.Void, "void")),
      new JSKeyword(JSToken.With, "with", new JSKeyword(JSToken.While, "while")),
      null,
      new JSKeyword(JSToken.Yield, "yield"),
      null
    };
  }
}
