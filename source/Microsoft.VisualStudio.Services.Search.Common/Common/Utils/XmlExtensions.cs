// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Utils.XmlExtensions
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;
using System.IO;
using System.Xml;

namespace Microsoft.VisualStudio.Services.Search.Common.Utils
{
  public static class XmlExtensions
  {
    public static void AddAttribute(
      this XmlDocument xmlDoc,
      XmlNode xmlNode,
      string attrKey,
      string attrVal)
    {
      if (xmlDoc == null)
        throw new ArgumentNullException(nameof (xmlDoc));
      if (xmlNode == null)
        throw new ArgumentNullException(nameof (xmlNode));
      XmlAttribute attribute = xmlDoc.CreateAttribute(attrKey);
      attribute.Value = attrVal;
      xmlNode.Attributes.Append(attribute);
    }

    public static string GetAttributeValue(this XmlNode data, string attributeName)
    {
      if (data == null || data.Attributes == null || data.Attributes[attributeName] == null)
        throw new SearchServiceException((string) null, (Exception) new InvalidDataException("Invalid xml data node"));
      return data.Attributes[attributeName].Value;
    }
  }
}
