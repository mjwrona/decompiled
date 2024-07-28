// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Identity.WorkItemAadIdentityCreator
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Identity
{
  internal class WorkItemAadIdentityCreator
  {
    private AadIdentityHelper m_aadIdentityHelper;

    public WorkItemAadIdentityCreator()
      : this(new AadIdentityHelper())
    {
    }

    public WorkItemAadIdentityCreator(AadIdentityHelper witIdentityHelper) => this.m_aadIdentityHelper = witIdentityHelper;

    public virtual IdentityScopeInfo EnsureAadEntities(
      IVssRequestContext requestContext,
      IList<Guid> aadObjectIds,
      bool isGroups)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IList<Guid>>(aadObjectIds, nameof (aadObjectIds));
      IdentityScopeInfo identityScopeInfo = new IdentityScopeInfo();
      if (!aadObjectIds.Any<Guid>())
        return identityScopeInfo;
      ITeamFoundationWorkItemTrackingMetadataService service = requestContext.GetService<ITeamFoundationWorkItemTrackingMetadataService>();
      IdentityScopeInfo identityInScope = this.m_aadIdentityHelper.GetIdentityInScope(requestContext, aadObjectIds, isGroups);
      if (identityInScope.InScopeIdentitiesMap.Any<KeyValuePair<Guid, Microsoft.VisualStudio.Services.Identity.Identity>>())
        service.SyncIdentities(requestContext, (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identityInScope.InScopeIdentitiesMap.Values, nameof (EnsureAadEntities));
      return identityInScope;
    }
  }
}
