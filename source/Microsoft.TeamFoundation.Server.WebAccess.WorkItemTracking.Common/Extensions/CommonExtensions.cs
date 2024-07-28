// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Extensions.CommonExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Extensions
{
  public static class CommonExtensions
  {
    public static string BuildQueryString(this NameValueCollection queryParameters)
    {
      if (queryParameters == null)
        return string.Empty;
      List<string> stringList = new List<string>();
      foreach (string allKey in queryParameters.AllKeys)
      {
        foreach (string stringToEscape in queryParameters.GetValues(allKey))
          stringList.Add(allKey + "=" + Uri.EscapeDataString(stringToEscape));
      }
      return string.Join("&", stringList.ToArray());
    }
  }
}
