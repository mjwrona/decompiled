// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.CssStylesheet
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  internal class CssStylesheet
  {
    private List<CssStylesheet.StyleDefinition> _styleDefinitions;

    public CssStylesheet(XmlElement htmlElement)
    {
      if (htmlElement == null)
        return;
      this.DiscoverStyleDefinitions(htmlElement);
    }

    public void DiscoverStyleDefinitions(XmlElement htmlElement)
    {
      if (string.Equals(htmlElement.LocalName, "link", StringComparison.OrdinalIgnoreCase))
        return;
      if (!string.Equals(htmlElement.LocalName, "style"))
      {
        for (XmlNode htmlElement1 = htmlElement.FirstChild; htmlElement1 != null; htmlElement1 = htmlElement1.NextSibling)
        {
          if (htmlElement1 is XmlElement)
            this.DiscoverStyleDefinitions((XmlElement) htmlElement1);
        }
      }
      else
      {
        StringBuilder stringBuilder = new StringBuilder();
        XmlNode xmlNode = htmlElement.FirstChild;
        while (true)
        {
          switch (xmlNode)
          {
            case null:
              goto label_13;
            case XmlText _:
            case XmlComment _:
              stringBuilder.Append(this.RemoveComments(xmlNode.Value));
              break;
          }
          xmlNode = xmlNode.NextSibling;
        }
label_13:
        int index = 0;
        while (index < stringBuilder.Length)
        {
          int startIndex = index;
          while (index < stringBuilder.Length && stringBuilder[index] != '{')
            ++index;
          if (index < stringBuilder.Length)
          {
            int num = index;
            while (index < stringBuilder.Length && stringBuilder[index] != '}')
              ++index;
            if (index - num > 2)
              this.AddStyleDefinition(stringBuilder.ToString(startIndex, num - startIndex), stringBuilder.ToString(num + 1, index - num - 2));
            if (index < stringBuilder.Length)
              ++index;
          }
        }
      }
    }

    private string RemoveComments(string text)
    {
      int length = text.IndexOf("/*");
      if (length < 0)
        return text;
      int num = text.IndexOf("*/", length + 2);
      return num < 0 ? text.Substring(0, length) : text.Substring(0, length) + " " + this.RemoveComments(text.Substring(num + 2));
    }

    public void AddStyleDefinition(string selector, string definition)
    {
      selector = selector.Trim().ToLowerInvariant();
      definition = definition.Trim().ToLowerInvariant();
      if (selector.Length == 0 || definition.Length == 0)
        return;
      if (this._styleDefinitions == null)
        this._styleDefinitions = new List<CssStylesheet.StyleDefinition>();
      string str1 = selector;
      char[] chArray = new char[1]{ ',' };
      foreach (string str2 in str1.Split(chArray))
      {
        string selector1 = str2.Trim();
        if (selector1.Length > 0)
          this._styleDefinitions.Add(new CssStylesheet.StyleDefinition(selector1, definition));
      }
    }

    public string GetStyle(string elementName, List<XmlElement> sourceContext)
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (this._styleDefinitions != null)
      {
        for (int index1 = this._styleDefinitions.Count - 1; index1 >= 0; --index1)
        {
          string[] strArray = this._styleDefinitions[index1].Selector.Split(' ');
          int index2 = strArray.Length - 1;
          int count = sourceContext.Count;
          if (this.MatchSelectorLevel(strArray[index2].Trim(), sourceContext[sourceContext.Count - 1]))
          {
            foreach (string propertySetter in (IEnumerable<string>) this._styleDefinitions[index1].PropertySetters)
            {
              string key = ((IEnumerable<string>) propertySetter.Split(':')).First<string>();
              if (!dictionary.ContainsKey(key))
                dictionary[key] = propertySetter;
            }
          }
        }
      }
      return string.Join(";", dictionary.Values.ToArray<string>());
    }

    private bool MatchSelectorLevel(string selectorLevel, XmlElement xmlElement)
    {
      if (selectorLevel.Length == 0)
        return false;
      int length1 = selectorLevel.IndexOf('.');
      int length2 = selectorLevel.IndexOf('#');
      string b1 = (string) null;
      string b2 = (string) null;
      string a = (string) null;
      if (length1 >= 0)
      {
        if (length1 > 0)
          a = selectorLevel.Substring(0, length1);
        b1 = selectorLevel.Substring(length1 + 1);
      }
      else if (length2 >= 0)
      {
        if (length2 > 0)
          a = selectorLevel.Substring(0, length2);
        b2 = selectorLevel.Substring(length2 + 1);
      }
      else
        a = selectorLevel;
      return (a == null || string.Equals(a, xmlElement.LocalName, StringComparison.OrdinalIgnoreCase)) && (b2 == null || string.Equals(HtmlToXamlConverter.GetAttribute(xmlElement, "id"), b2, StringComparison.OrdinalIgnoreCase)) && (b1 == null || string.Equals(HtmlToXamlConverter.GetAttribute(xmlElement, "class"), b1, StringComparison.OrdinalIgnoreCase));
    }

    private class StyleDefinition
    {
      private List<string> _propertySetters = new List<string>();

      public StyleDefinition(string selector, string definition)
      {
        this.Selector = selector;
        this.Definition = definition;
        if (string.IsNullOrEmpty(definition))
          return;
        this._propertySetters.AddRange(((IEnumerable<string>) definition.Split(';')).Select<string, string>((Func<string, string>) (str => str.Trim())));
      }

      public string Selector { get; private set; }

      public string Definition { get; private set; }

      public IList<string> PropertySetters => (IList<string>) this._propertySetters;
    }
  }
}
