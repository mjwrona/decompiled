// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.TfsNamespaceResolver
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using System.Collections.Generic;
using System.Xml;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class TfsNamespaceResolver : IXmlNamespaceResolver
  {
    public IDictionary<string, string> GetNamespacesInScope(XmlNamespaceScope scope)
    {
      if (scope == XmlNamespaceScope.All || scope == XmlNamespaceScope.Local)
        return (IDictionary<string, string>) new Dictionary<string, string>()
        {
          {
            "tb1",
            "http://schemas.microsoft.com/TeamFoundation/2010/Build"
          },
          {
            "xml",
            "http://www.w3.org/2000/xmlns/"
          }
        };
      return (IDictionary<string, string>) new Dictionary<string, string>()
      {
        {
          "tb1",
          "http://schemas.microsoft.com/TeamFoundation/2010/Build"
        }
      };
    }

    public string LookupNamespace(string prefix)
    {
      switch (prefix)
      {
        case "tb1":
          return "http://schemas.microsoft.com/TeamFoundation/2010/Build";
        case "xml":
          return "http://www.w3.org/2000/xmlns/";
        default:
          return string.Empty;
      }
    }

    public string LookupPrefix(string namespaceName)
    {
      switch (namespaceName)
      {
        case "http://schemas.microsoft.com/TeamFoundation/2010/Build":
          return "tb1";
        case "http://www.w3.org/2000/xmlns/":
          return "xml";
        default:
          return string.Empty;
      }
    }
  }
}
