// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.QueryTypeExtensionMethods
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal static class QueryTypeExtensionMethods
  {
    internal static bool IsOneHopQuery(this QueryType queryType)
    {
      switch (queryType)
      {
        case QueryType.LinksOneHopMustContain:
        case QueryType.LinksOneHopMayContain:
        case QueryType.LinksOneHopDoesNotContain:
          return true;
        default:
          return false;
      }
    }
  }
}
