// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.ProcessParameterHelper
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Microsoft.TeamFoundation.Build.Server
{
  internal static class ProcessParameterHelper
  {
    private const string SystemNamespace = "clr-namespace:System;assembly=mscorlib";
    private const string SystemCollectionsGenericNamespace = "clr-namespace:System.Collections.Generic;assembly=mscorlib";
    private const string TeamBuildWorkflowNamespace = "clr-namespace:Microsoft.TeamFoundation.Build.Workflow.Activities;assembly=Microsoft.TeamFoundation.Build.Workflow";
    private const string WorkflowNamespace = "http://schemas.microsoft.com/netfx/2009/xaml/activities";
    private const string XamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml";
    private const string XamlSchemaNamespace = "http://schemas.microsoft.com/netfx/2008/xaml/schema";
    private const string VariableNodeName = "Variable";
    private const string ActivityElementName = "Activity";
    private const string SchemaMembersNodeName = "Members";
    private const string SchemaPropertyNodeName = "Property";
    private const string ClassNodeName = "Class";

    public static string ExtractProcessParameters(
      string processWorkflow,
      out IDictionary<string, string> parameters)
    {
      bool flag = false;
      XmlReaderSettings settings1 = new XmlReaderSettings()
      {
        IgnoreComments = true,
        IgnoreWhitespace = true,
        DtdProcessing = DtdProcessing.Prohibit,
        XmlResolver = (XmlResolver) null
      };
      XmlWriterSettings settings2 = new XmlWriterSettings()
      {
        Indent = true,
        IndentChars = "  ",
        OmitXmlDeclaration = true
      };
      parameters = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.Ordinal);
      using (StringWriter output = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture))
      {
        using (XmlWriter xmlWriter = XmlWriter.Create((TextWriter) output, settings2))
        {
          using (StringReader input = new StringReader(processWorkflow))
          {
            using (XmlReader xmlReader = XmlReader.Create((TextReader) input, settings1))
            {
              try
              {
                do
                  ;
                while (xmlReader.Read() && !ProcessParameterHelper.IsKnownNode(xmlReader, XmlNodeType.Element, "http://schemas.microsoft.com/netfx/2009/xaml/activities", "Activity"));
                if (ProcessParameterHelper.IsKnownNode(xmlReader, XmlNodeType.Element, "http://schemas.microsoft.com/netfx/2009/xaml/activities", "Activity"))
                {
                  xmlWriter.WriteStartElement(xmlReader.Prefix, xmlReader.LocalName, xmlReader.NamespaceURI);
                  string str = (string) null;
                  string clrNamespace = (string) null;
                  if (xmlReader.HasAttributes)
                  {
                    List<ProcessParameterHelper.BufferedAttribute> bufferedAttributeList = new List<ProcessParameterHelper.BufferedAttribute>();
                    while (xmlReader.MoveToNextAttribute())
                    {
                      if (ProcessParameterHelper.IsKnownNode(xmlReader, XmlNodeType.Attribute, "http://schemas.microsoft.com/winfx/2006/xaml", "Class"))
                      {
                        str = xmlReader.Value;
                        int length = str.LastIndexOf('.');
                        if (length >= 0)
                        {
                          clrNamespace = str.Substring(0, length);
                          str = str.Substring(length + 1);
                        }
                      }
                      else
                        bufferedAttributeList.Add(new ProcessParameterHelper.BufferedAttribute()
                        {
                          LocalName = xmlReader.LocalName,
                          NamespaceURI = xmlReader.NamespaceURI,
                          Value = xmlReader.Value
                        });
                      xmlWriter.WriteAttributeString(xmlReader.Prefix, xmlReader.LocalName, xmlReader.NamespaceURI, xmlReader.Value);
                    }
                    foreach (ProcessParameterHelper.BufferedAttribute bufferedAttribute in bufferedAttributeList)
                    {
                      if (ProcessParameterHelper.IsClassNamespace(bufferedAttribute.NamespaceURI, clrNamespace))
                      {
                        string localName = bufferedAttribute.LocalName;
                        if (localName.StartsWith(str, StringComparison.Ordinal))
                        {
                          string key = localName.Substring(str.Length + 1);
                          parameters[key] = bufferedAttribute.Value;
                        }
                      }
                    }
                  }
                  xmlReader.MoveToElement();
                  xmlReader.Read();
                  if (ProcessParameterHelper.IsKnownNode(xmlReader, XmlNodeType.Element, "http://schemas.microsoft.com/winfx/2006/xaml", "Members"))
                  {
                    flag = true;
                    xmlWriter.WriteStartElement(xmlReader.LocalName, xmlReader.NamespaceURI);
                    xmlReader.Read();
                    while (ProcessParameterHelper.IsKnownNode(xmlReader, XmlNodeType.Element, "http://schemas.microsoft.com/winfx/2006/xaml", "Property"))
                    {
                      if (xmlReader.MoveToAttribute("Name"))
                      {
                        if (!parameters.ContainsKey(xmlReader.Value))
                          parameters.Add(xmlReader.Value, (string) null);
                        xmlReader.MoveToElement();
                        xmlWriter.WriteNode(xmlReader, true);
                      }
                    }
                    xmlReader.ReadEndElement();
                    xmlWriter.WriteEndElement();
                  }
                  if (flag)
                  {
                    while (xmlReader.NodeType == XmlNodeType.Element)
                    {
                      if (ProcessParameterHelper.IsClassNamespace(xmlReader.NamespaceURI, clrNamespace))
                      {
                        string localName1 = xmlReader.LocalName;
                        if (localName1.StartsWith(str, StringComparison.Ordinal))
                        {
                          string key = localName1.Substring(str.Length + 1);
                          if (parameters.TryGetValue(key, out string _))
                          {
                            string localName2 = xmlReader.LocalName;
                            string namespaceUri = xmlReader.NamespaceURI;
                            xmlWriter.WriteStartElement(xmlReader.Prefix, xmlReader.LocalName, xmlReader.NamespaceURI);
                            xmlWriter.WriteAttributes(xmlReader, true);
                            bool isEmptyElement = xmlReader.IsEmptyElement;
                            if (xmlReader.Read() && !isEmptyElement)
                            {
                              switch (xmlReader.NodeType)
                              {
                                case XmlNodeType.Element:
                                  XmlDocument xmlDocument = new XmlDocument(xmlReader.NameTable);
                                  xmlDocument.Load(xmlReader);
                                  using (XmlReader reader = (XmlReader) new XmlNodeReader((XmlNode) xmlDocument.DocumentElement))
                                    xmlWriter.WriteNode(reader, true);
                                  parameters[key] = xmlDocument.OuterXml;
                                  break;
                                case XmlNodeType.Text:
                                  xmlWriter.WriteString(xmlReader.Value);
                                  parameters[key] = xmlReader.Value;
                                  xmlReader.Read();
                                  break;
                              }
                              while (!ProcessParameterHelper.IsKnownNode(xmlReader, XmlNodeType.EndElement, namespaceUri, localName2))
                                xmlReader.Read();
                              xmlReader.ReadEndElement();
                            }
                            xmlWriter.WriteEndElement();
                            continue;
                          }
                        }
                      }
                      xmlReader.Skip();
                    }
                    xmlWriter.WriteEndElement();
                    xmlWriter.Flush();
                  }
                }
              }
              catch (XmlException ex)
              {
                flag = false;
              }
              return flag ? output.ToString() : (string) null;
            }
          }
        }
      }
    }

    public static string CreateProcessParameters(IDictionary<string, string> parameters) => XamlHelper.Save(parameters);

    public static string ExtractProcessParameter(string processWorkflow, string parameterName)
    {
      try
      {
        Dictionary<string, string> dictionary = XamlHelper.LoadPartial(processWorkflow);
        if (dictionary != null)
        {
          if (dictionary.ContainsKey(parameterName))
            return dictionary[parameterName];
        }
      }
      catch
      {
      }
      return (string) null;
    }

    private static bool IsKnownNode(
      XmlReader xmlReader,
      XmlNodeType nodeType,
      string ns,
      string localName)
    {
      return xmlReader.NodeType == nodeType && XamlNamespaceComparer.Equals(xmlReader.NamespaceURI, ns) && string.Equals(xmlReader.LocalName, localName, StringComparison.Ordinal);
    }

    private static bool IsClassNamespace(string ns, string clrNamespace) => XamlNamespaceComparer.Equals(ns, "clr-namespace:" + clrNamespace);

    private sealed class BufferedAttribute
    {
      public string NamespaceURI { get; set; }

      public string LocalName { get; set; }

      public string Value { get; set; }
    }
  }
}
