// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Common.XamlHelper
// Assembly: Microsoft.TeamFoundation.Build.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AD9C54FA-787C-49B8-AA73-C4A6EF8CE391
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build.Common.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Microsoft.TeamFoundation.Build.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class XamlHelper
  {
    private const string XamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml";
    private const string SystemNamespace = "clr-namespace:System;assembly=mscorlib";
    private const string SystemCollectionsGenericNamespace = "clr-namespace:System.Collections.Generic;assembly=mscorlib";

    public static string Save(IDictionary<string, string> parameters)
    {
      XmlWriterSettings settings = new XmlWriterSettings()
      {
        Indent = true,
        IndentChars = "  ",
        OmitXmlDeclaration = true
      };
      using (StringWriter output = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture))
      {
        using (XmlWriter xmlWriter = XmlWriter.Create((TextWriter) output, settings))
        {
          xmlWriter.WriteStartDocument();
          xmlWriter.WriteStartElement("", "Dictionary", "clr-namespace:System.Collections.Generic;assembly=mscorlib");
          xmlWriter.WriteAttributeString("x", "TypeArguments", "http://schemas.microsoft.com/winfx/2006/xaml", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:String, {0}:Object", (object) "s"));
          xmlWriter.WriteAttributeString("xmlns", "x", (string) null, "http://schemas.microsoft.com/winfx/2006/xaml");
          xmlWriter.WriteAttributeString("xmlns", "s", (string) null, "clr-namespace:System;assembly=mscorlib");
          foreach (KeyValuePair<string, string> parameter in (IEnumerable<KeyValuePair<string, string>>) parameters)
          {
            xmlWriter.WriteStartElement((string) null, "String", "clr-namespace:System;assembly=mscorlib");
            xmlWriter.WriteAttributeString("x", "Key", "http://schemas.microsoft.com/winfx/2006/xaml", parameter.Key);
            xmlWriter.WriteAttributeString("xml", "space", (string) null, "preserve");
            xmlWriter.WriteString(parameter.Value);
            xmlWriter.WriteEndElement();
          }
          xmlWriter.WriteEndElement();
          xmlWriter.WriteEndDocument();
          xmlWriter.Flush();
          return output.ToString();
        }
      }
    }

    public static Dictionary<string, string> LoadPartial(string xaml)
    {
      if (!string.IsNullOrEmpty(xaml))
      {
        XmlReaderSettings settings = new XmlReaderSettings()
        {
          IgnoreComments = true,
          IgnoreWhitespace = true,
          DtdProcessing = DtdProcessing.Prohibit,
          XmlResolver = (XmlResolver) null
        };
        using (StringReader input = new StringReader(xaml))
        {
          using (XmlReader xmlReader = XmlReader.Create((TextReader) input, settings))
          {
            try
            {
              xmlReader.Read();
              if (xmlReader.NodeType == XmlNodeType.XmlDeclaration)
                xmlReader.Read();
              if (XamlHelper.IsKnownNode(xmlReader, XmlNodeType.Element, "clr-namespace:System.Collections.Generic;assembly=mscorlib", "Dictionary"))
              {
                xmlReader.Read();
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                while (xmlReader.NodeType == XmlNodeType.Element)
                {
                  if (XamlHelper.IsKnownNode(xmlReader, XmlNodeType.Element, "clr-namespace:System;assembly=mscorlib", "String") || XamlHelper.IsKnownNode(xmlReader, XmlNodeType.Element, "http://schemas.microsoft.com/winfx/2006/xaml", "String"))
                  {
                    if (xmlReader.MoveToAttribute("Key", "http://schemas.microsoft.com/winfx/2006/xaml"))
                    {
                      string key = xmlReader.Value;
                      if (!xmlReader.IsEmptyElement)
                      {
                        xmlReader.Read();
                        if (xmlReader.NodeType == XmlNodeType.Text)
                          dictionary.Add(key, xmlReader.Value);
                      }
                    }
                    while (!XamlHelper.IsKnownNode(xmlReader, XmlNodeType.EndElement, "clr-namespace:System;assembly=mscorlib", "String") && !XamlHelper.IsKnownNode(xmlReader, XmlNodeType.EndElement, "http://schemas.microsoft.com/winfx/2006/xaml", "String"))
                      xmlReader.Read();
                    if (xmlReader.NodeType == XmlNodeType.EndElement)
                      xmlReader.ReadEndElement();
                  }
                  else
                    xmlReader.ReadOuterXml();
                }
                return dictionary;
              }
            }
            catch (XmlException ex)
            {
            }
          }
        }
      }
      return (Dictionary<string, string>) null;
    }

    private static bool IsKnownNode(
      XmlReader xmlReader,
      XmlNodeType nodeType,
      string ns,
      string localName)
    {
      return xmlReader.NodeType == nodeType && XamlNamespaceComparer.Equals(xmlReader.NamespaceURI, ns) && string.Equals(xmlReader.LocalName, localName, StringComparison.Ordinal);
    }
  }
}
