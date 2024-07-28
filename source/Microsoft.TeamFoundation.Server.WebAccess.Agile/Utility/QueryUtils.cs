// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.Utility.QueryUtils
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.Utility
{
  internal static class QueryUtils
  {
    internal static IDictionary GetDefaultQueryContext(
      this IVssRequestContext requestContext,
      string projectName)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<string>(projectName, nameof (projectName));
      Hashtable defaultQueryContext = new Hashtable((IEqualityComparer) StringComparer.OrdinalIgnoreCase);
      defaultQueryContext[(object) "me"] = (object) requestContext.GetUserIdentity().DisplayName;
      defaultQueryContext[(object) "project"] = (object) projectName;
      return (IDictionary) defaultQueryContext;
    }

    internal static WorkItemStateCategory[] GetQueryStates(bool showInProgress) => showInProgress ? new WorkItemStateCategory[3]
    {
      WorkItemStateCategory.Proposed,
      WorkItemStateCategory.InProgress,
      WorkItemStateCategory.Resolved
    } : new WorkItemStateCategory[1]
    {
      WorkItemStateCategory.Proposed
    };
  }
}
