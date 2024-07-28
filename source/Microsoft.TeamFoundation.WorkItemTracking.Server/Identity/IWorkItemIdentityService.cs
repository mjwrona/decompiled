// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Identity.IWorkItemIdentityService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.UpdateState;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Identity
{
  [DefaultServiceImplementation(typeof (WorkItemIdentityService))]
  internal interface IWorkItemIdentityService : IVssFrameworkService
  {
    ResolvedIdentityNamesInfo ResolveIdentityFields(
      WorkItemTrackingRequestContext witRequestContext,
      IEnumerable<WorkItemUpdateState> workItems,
      bool bypassRules);

    ResolvedIdentityNamesInfo ResolveIdentityFields(
      WorkItemTrackingRequestContext witRequestContext,
      IDictionary<int, object> updates,
      IDictionary<int, object> latestValues,
      out IEnumerable<WorkItemFieldInvalidException> exceptions);

    ResolvedIdentityNamesInfo ResolveIdentityNames(
      WorkItemTrackingRequestContext witRequestContext,
      IEnumerable<string> names,
      bool includeInactiveIdentitiesForQuery,
      bool syncMissingIdentities = true);

    ResolvedIdentityNamesInfo ResolveIdentityNames(
      WorkItemTrackingRequestContext witRequestContext,
      IEnumerable<string> names,
      IEnumerable<WorkItemIdentity> identities,
      bool includeInactiveIdentitiesForQuery,
      bool syncMissingIdentities = true);

    IEnumerable<IdentityRef> SearchIdentities(
      IVssRequestContext requestContext,
      string searchTerm,
      SearchIdentityType identityType = SearchIdentityType.All);
  }
}
