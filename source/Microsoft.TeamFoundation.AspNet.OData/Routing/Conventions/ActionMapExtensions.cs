// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Routing.Conventions.ActionMapExtensions
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Interfaces;

namespace Microsoft.AspNet.OData.Routing.Conventions
{
  internal static class ActionMapExtensions
  {
    public static string FindMatchingAction(
      this IWebApiActionMap actionMap,
      params string[] targetActionNames)
    {
      foreach (string targetActionName in targetActionNames)
      {
        if (actionMap.Contains(targetActionName))
          return targetActionName;
      }
      return (string) null;
    }
  }
}
