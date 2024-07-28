// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.GroupComponent16
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class GroupComponent16 : GroupComponent15
  {
    public override ResultCollection ReadMemberships(
      IEnumerable<Guid> identityIds,
      QueryMembership childrenQuery,
      QueryMembership parentsQuery,
      bool includeRestricted,
      int? minInactivatedTime,
      Guid scopeId,
      bool returnVisibleIdentities,
      bool useXtpProc = false,
      long minSequenceId = -1,
      bool inScopeMembershipsOnly = false)
    {
      try
      {
        this.TraceEnter(4703110, nameof (ReadMemberships));
        if (this.RequestContext.ExecutionEnvironment.IsHostedDeployment && childrenQuery == QueryMembership.None && parentsQuery == QueryMembership.None)
        {
          if (identityIds != null && identityIds.Count<Guid>() == 1)
            this.PrepareReadScopeVisibility(scopeId, identityIds.First<Guid>());
          else
            this.PrepareReadScopeVisibilities(scopeId, identityIds);
        }
        else
          this.PrepareReadMemberships(identityIds, childrenQuery, parentsQuery, includeRestricted, minInactivatedTime, scopeId, returnVisibleIdentities, useXtpProc, new long?(minSequenceId));
        ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<GroupMembership>((ObjectBinder<GroupMembership>) new ReadGroupMembershipsComponentBase.ParentColumns());
        resultCollection.AddBinder<GroupMembership>((ObjectBinder<GroupMembership>) new ReadGroupMembershipsComponentBase.ChildrenColumns());
        if (returnVisibleIdentities)
          resultCollection.AddBinder<ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData>((ObjectBinder<ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData>) new ReadGroupMembershipsComponentBase.ScopeFilteredIdentitiesColumns());
        return resultCollection;
      }
      finally
      {
        this.TraceLeave(4703119, nameof (ReadMemberships));
      }
    }
  }
}
