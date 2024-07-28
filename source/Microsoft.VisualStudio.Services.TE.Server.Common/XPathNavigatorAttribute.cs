// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestExecution.Server.XPathNavigatorAttribute
// Assembly: Microsoft.VisualStudio.Services.TE.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BC2680F-A5FB-41BE-A4CF-F78BF7AC3E02
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.TE.Server.Common.dll

using System.Xml.XPath;

namespace Microsoft.TeamFoundation.TestExecution.Server
{
  public static class XPathNavigatorAttribute
  {
    public static T GetAttributeValue<T>(
      this XPathNavigator node,
      string attributeName,
      ParseDelegate<T> parseDelegate)
    {
      T result;
      return parseDelegate(node.GetAttribute(attributeName, string.Empty), out result) ? result : default (T);
    }
  }
}
