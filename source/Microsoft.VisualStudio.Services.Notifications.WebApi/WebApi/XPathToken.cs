// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.WebApi.XPathToken
// Assembly: Microsoft.VisualStudio.Services.Notifications.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF217E0A-7730-437B-BE9F-877363CB7392
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.WebApi.dll

using System;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.VisualStudio.Services.Notifications.WebApi
{
  public class XPathToken : Token
  {
    private bool m_hasConstEval;

    public XPathToken()
    {
    }

    public XPathToken(string spelling)
      : base(spelling)
    {
    }

    public XPathToken(byte type, string spelling)
      : base(type, spelling)
    {
    }

    public override string Spelling
    {
      get => this.m_spelling;
      protected set
      {
        this.m_spelling = value;
        this.m_hasMacros = false;
        if (!string.IsNullOrEmpty(this.m_spelling))
        {
          this.m_hasMacros = this.m_spelling.Contains("@@") || this.m_spelling.IndexOfAny(Token.replacementChars) > -1;
          this.m_hasConstEval = this.RequiresEval(this.m_spelling);
          this.m_isCaseInsensitve = this.IsCaseInsensitve(this.m_spelling);
          if (XPathSubscriptionExpression.ParseLastXPathFilter(this.m_spelling, out string _, out string _, out string _))
            this.m_isXPath = true;
        }
        this.m_queriableValue = this.Unescape();
      }
    }

    public override string EvaluatePathFunctions()
    {
      if (string.IsNullOrEmpty(this.m_queriableValue) || !this.m_hasConstEval)
        return this.m_spelling;
      XPathNavigator navigator = new XmlDocument().CreateNavigator();
      string[] strArray = this.m_queriableValue.Split(new char[1]
      {
        '='
      }, 2);
      strArray[0] = this.XPathToStringLiteral(strArray[0], navigator);
      if (strArray.Length == 2)
        strArray[1] = this.XPathToStringLiteral(strArray[1], navigator);
      return string.Join("=", strArray);
    }

    private string XPathToStringLiteral(string value, XPathNavigator navigator)
    {
      try
      {
        if (!this.RequiresEval(value))
          return value;
        object obj = navigator.Evaluate(value);
        return obj is string ? "'" + ((string) obj).Replace("'", "@@SingleQuote@@").Replace("\"", "@@DoubleQuote@@") + "'" : value;
      }
      catch
      {
        return value;
      }
    }

    protected override string Unescape()
    {
      string str = this.m_spelling;
      if (this.m_hasMacros)
        str = str.Replace("@@SQBDQ@@", "\"'\"").Replace("@@DQBSQ@@", "'\"'").Replace("@@SingleQuote@@", "'").Replace("@@DoubleQuote@@", "\"");
      return str;
    }

    public override string EscapeSpecialChatactersIfNeeded(
      bool UseSingleQuoteChar = true,
      bool replaceSpecialCharsWithToken = true)
    {
      string spelling = this.Spelling;
      string oldValue = UseSingleQuoteChar ? "'" : "\"";
      int num = UseSingleQuoteChar ? 1 : 0;
      string str = !replaceSpecialCharsWithToken ? (UseSingleQuoteChar ? "\"'\"" : "'\"'") : (UseSingleQuoteChar ? "@@SQBDQ@@" : "@@DQBSQ@@");
      if (!spelling.Contains(oldValue))
        return oldValue + spelling + oldValue;
      string newValue = string.Format("{0},{1},{0}", (object) oldValue, (object) str);
      return string.Format("concat({0}{1}{0})", (object) oldValue, (object) spelling.Replace(oldValue, newValue));
    }

    private bool RequiresEval(string xpathExpression) => xpathExpression.IndexOf("concat(", StringComparison.OrdinalIgnoreCase) >= 0;

    private bool IsCaseInsensitve(string xpathExpression) => xpathExpression.IndexOf("translate(", StringComparison.OrdinalIgnoreCase) >= 0;
  }
}
