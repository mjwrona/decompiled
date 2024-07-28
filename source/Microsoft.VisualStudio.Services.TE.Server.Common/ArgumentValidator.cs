// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestExecution.Server.ArgumentValidator
// Assembly: Microsoft.VisualStudio.Services.TE.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BC2680F-A5FB-41BE-A4CF-F78BF7AC3E02
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.TE.Server.Common.dll

using System;
using System.Xml;

namespace Microsoft.TeamFoundation.TestExecution.Server
{
  internal static class ArgumentValidator
  {
    internal static void CheckNull(object arg, string name)
    {
      if (arg == null)
        throw new ArgumentNullException(name, TestExecutionServiceResources.NullArgument);
    }

    internal static void CheckStringNull(string arg, string name)
    {
      if (string.IsNullOrEmpty(arg))
        throw new ArgumentNullException(name, TestExecutionServiceResources.NullArgument);
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
