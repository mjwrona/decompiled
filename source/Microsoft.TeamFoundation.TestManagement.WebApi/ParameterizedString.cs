// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.ParameterizedString
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  public class ParameterizedString
  {
    public const string ElementName = "parameterizedString";
    public const string IsformattedAttribute = "isformatted";
    private const string TitleElementName = "action";
    private const string ExpectedResultElementName = "expected";
    private const string TextParameterName = "text";
    private const string ParameterElementName = "parameter";
    private const string OutputParameterElementName = "outputparameter";
    private const char FirstChar = '@';
    private const char IgnoreValueChar = '?';
    private const string IgnoreValueString = "@?";
    internal const string ParameterString = "@";
    private const string parameterCharacters = "[\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}\\p{Nd}\\p{Pc}\\p{Pd}\\p{Mn}\\p{Mc}\\p{Cf}_@]";
    private const string nonParameterCharacters = "[^\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}\\p{Nd}\\p{Pc}\\p{Pd}\\p{Mn}\\p{Mc}\\p{Cf}_@]";
    private static Regex s_parameterRefRegex = new Regex("(^@\\??[\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}\\p{Nd}\\p{Pc}\\p{Pd}\\p{Mn}\\p{Mc}\\p{Cf}_@]+)|([^\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}\\p{Nd}\\p{Pc}\\p{Pd}\\p{Mn}\\p{Mc}\\p{Cf}_@]@\\??[\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}\\p{Nd}\\p{Pc}\\p{Pd}\\p{Mn}\\p{Mc}\\p{Cf}_@]+)");
    private string m_flowDocumentFormattedString;
    private static ParameterizedString s_empty = new ParameterizedString();
    private ParameterizedStringPart[] m_parts;

    public ParameterizedString() => this.m_flowDocumentFormattedString = string.Empty;

    public ParameterizedString(string value)
    {
      value = value.Replace("\r\n", "<P>").Replace("\n", "<P>").Replace("\r", "<P>");
      this.m_flowDocumentFormattedString = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "<DIV><P>{0}</P></DIV>", (object) value);
    }

    public ParameterizedString(XmlReader reader)
    {
      this.m_flowDocumentFormattedString = string.Empty;
      this.FromXml(reader);
    }

    public static implicit operator ParameterizedString(string value) => new ParameterizedString(value);

    public static implicit operator string(ParameterizedString value) => value.ToString();

    public override string ToString() => this.m_flowDocumentFormattedString;

    public static ParameterizedString Empty => ParameterizedString.s_empty;

    public void FromXml(XmlReader reader)
    {
      if (reader.Name == "parameterizedString")
      {
        List<ParameterizedStringPart> parts = new List<ParameterizedStringPart>();
        if (reader.IsEmptyElement)
        {
          this.InitializeFromParts((IEnumerable<ParameterizedStringPart>) parts);
          reader.Read();
        }
        else
        {
          string attribute = reader.GetAttribute("isformatted");
          if (attribute != null && attribute != "true")
          {
            reader.Read();
            while (reader.NodeType == XmlNodeType.Element)
            {
              ParameterizedStringPart parameterizedStringPart;
              switch (reader.Name)
              {
                case "parameter":
                  parameterizedStringPart = new ParameterizedStringPart(reader.ReadElementContentAsString(), false);
                  break;
                case "outputparameter":
                  parameterizedStringPart = new ParameterizedStringPart(reader.ReadElementContentAsString(), true);
                  break;
                case "text":
                  string literalText = reader.ReadElementContentAsString();
                  if (string.IsNullOrEmpty(literalText))
                    literalText = " ";
                  parameterizedStringPart = new ParameterizedStringPart(literalText);
                  break;
                default:
                  throw new XmlException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Unexpected element name {0}.", (object) reader.Name));
              }
              parts.Add(parameterizedStringPart);
            }
            this.InitializeFromParts((IEnumerable<ParameterizedStringPart>) parts);
            reader.ReadEndElement();
          }
          else
          {
            this.m_flowDocumentFormattedString = reader.ReadElementContentAsString();
            this.Parse();
          }
        }
      }
      else
      {
        if (!(reader.Name == "action") && !(reader.Name == "expected"))
          return;
        this.m_flowDocumentFormattedString = reader.ReadElementContentAsString();
        this.Parse();
      }
    }

    public void ToXml(XmlWriter writer)
    {
      writer.WriteStartElement("parameterizedString");
      writer.WriteAttributeString("isformatted", "true");
      string documentFormattedString = this.m_flowDocumentFormattedString;
      writer.WriteString(documentFormattedString);
      writer.WriteEndElement();
    }

    private void InitializeFromParts(IEnumerable<ParameterizedStringPart> parts)
    {
      List<ParameterizedStringPart> parameterizedStringPartList = new List<ParameterizedStringPart>();
      StringBuilder stringBuilder = new StringBuilder();
      foreach (ParameterizedStringPart part in parts)
      {
        parameterizedStringPartList.Add(part);
        if (part.IsParameter)
        {
          if (part.IgnoreValue)
            stringBuilder.Append("@?");
          else
            stringBuilder.Append("@");
          stringBuilder.Append(part.ParameterName);
        }
        else
          stringBuilder.Append(part.LiteralValue);
      }
      this.m_flowDocumentFormattedString = stringBuilder.ToString();
      this.m_parts = parameterizedStringPartList.ToArray();
    }

    private void Parse()
    {
      string documentFormattedString = this.m_flowDocumentFormattedString;
      List<ParameterizedStringPart> parameterizedStringPartList = new List<ParameterizedStringPart>();
      if (!string.IsNullOrEmpty(documentFormattedString))
      {
        int startIndex = 0;
        for (Match match = ParameterizedString.s_parameterRefRegex.Match(documentFormattedString); match.Success; match = match.NextMatch())
        {
          int num = match.Value.IndexOf('@');
          string parameterName = num == -1 || num >= match.Value.Length - 1 ? match.Value : match.Value.Substring(num + 1);
          if (parameterName.Trim('@', '?').Length != 0)
          {
            bool flag = false;
            int index = match.Index;
            if (startIndex < index)
            {
              string str = documentFormattedString.Substring(startIndex, index - startIndex);
              parameterizedStringPartList.Add(new ParameterizedStringPart(str + match.Value[0].ToString()));
              flag = true;
            }
            if (!flag && parameterizedStringPartList.Count > 0)
              parameterizedStringPartList.Add(new ParameterizedStringPart(match.Value[0].ToString()));
            bool ignoreValue = parameterName[0] == '?';
            if (ignoreValue)
              parameterizedStringPartList.Add(new ParameterizedStringPart(parameterName.Substring(1), ignoreValue));
            else
              parameterizedStringPartList.Add(new ParameterizedStringPart(parameterName, ignoreValue));
            startIndex = index + match.Length;
          }
        }
        if (startIndex < documentFormattedString.Length)
        {
          string literalText = documentFormattedString.Substring(startIndex);
          parameterizedStringPartList.Add(new ParameterizedStringPart(literalText));
        }
      }
      this.m_parts = parameterizedStringPartList.ToArray();
    }
  }
}
