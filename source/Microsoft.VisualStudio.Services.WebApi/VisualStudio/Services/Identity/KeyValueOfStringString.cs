// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.KeyValueOfStringString
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Xml;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class KeyValueOfStringString
  {
    public string Key { get; set; }

    public string Value { get; set; }

    internal static KeyValueOfStringString FromXml(
      IServiceProvider serviceProvider,
      XmlReader reader)
    {
      KeyValueOfStringString valueOfStringString = new KeyValueOfStringString();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          string name = reader.Name;
        }
      }
      reader.Read();
      if (!isEmptyElement)
      {
        while (reader.NodeType == XmlNodeType.Element)
        {
          switch (reader.Name)
          {
            case "Key":
              valueOfStringString.Key = XmlUtility.StringFromXmlElement(reader);
              continue;
            case "Value":
              valueOfStringString.Value = XmlUtility.StringFromXmlElement(reader);
              continue;
            default:
              reader.ReadOuterXml();
              continue;
          }
        }
        reader.ReadEndElement();
      }
      return valueOfStringString;
    }
  }
}
