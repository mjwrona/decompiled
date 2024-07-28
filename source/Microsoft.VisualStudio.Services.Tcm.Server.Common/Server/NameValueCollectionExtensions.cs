// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.NameValueCollectionExtensions
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System.Collections.Generic;
using System.Collections.Specialized;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal static class NameValueCollectionExtensions
  {
    public static Dictionary<string, string> ToDictionary(
      this NameValueCollection nameValueCollection)
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      foreach (string allKey in nameValueCollection.AllKeys)
        dictionary.Add(allKey, nameValueCollection[allKey]);
      return dictionary;
    }
  }
}
