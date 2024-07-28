// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.WorkItemIdentityExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public static class WorkItemIdentityExtensions
  {
    public static WitIdentityRef ToWitIdentityRef(this WorkItemIdentity workItemIdentity)
    {
      if (workItemIdentity == null)
        return (WitIdentityRef) null;
      string str = (string) null;
      if (workItemIdentity.HasPermission)
        str = workItemIdentity.DistinctDisplayName;
      else if (workItemIdentity.IdentityRef != null)
        str = workItemIdentity.IdentityRef.Descriptor.ToString() == null ? workItemIdentity.IdentityRef.DisplayName : string.Format("{0} <desc:{1}>", (object) workItemIdentity.IdentityRef.DisplayName, (object) workItemIdentity.IdentityRef.Descriptor);
      return new WitIdentityRef(workItemIdentity.SecurityToken)
      {
        DistinctDisplayName = str,
        IdentityRef = workItemIdentity.IdentityRef
      };
    }
  }
}
