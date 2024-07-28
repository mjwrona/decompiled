// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Promote.WitPromoteExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Promote
{
  internal static class WitPromoteExtensions
  {
    private const string c_SeenGlobalListsKey = "PromoteSeenGlobalLists";

    internal static ISet<string> GetSeenGlobalListNames(this IVssRequestContext requestContext)
    {
      object obj;
      ISet<string> seenGlobalListNames;
      if (requestContext.Items.TryGetValue("PromoteSeenGlobalLists", out obj))
      {
        seenGlobalListNames = (ISet<string>) obj;
      }
      else
      {
        seenGlobalListNames = (ISet<string>) new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        requestContext.Items["PromoteSeenGlobalLists"] = (object) seenGlobalListNames;
      }
      return seenGlobalListNames;
    }
  }
}
