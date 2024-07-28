// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TcmArgumentValidator
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using System;
using System.Xml;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal static class TcmArgumentValidator
  {
    internal static void CheckNull(object arg, string name)
    {
      if (arg == null)
        throw new ArgumentNullException(name, ServerResources.NullArgument);
    }

    internal static bool ValidateXml(XmlNode node, bool allowNull = false)
    {
      if (node == null & allowNull)
        return true;
      try
      {
        XmlDocument xmlDocument = new XmlDocument();
        using (XmlTextReader reader = new XmlTextReader(node.OuterXml, XmlNodeType.Element, (XmlParserContext) null))
        {
          reader.DtdProcessing = DtdProcessing.Prohibit;
          reader.XmlResolver = (XmlResolver) null;
          xmlDocument.Load((XmlReader) reader);
        }
      }
      catch
      {
        return false;
      }
      return true;
    }
  }
}
