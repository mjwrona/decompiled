// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.GroupComponent12
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
  internal class GroupComponent12 : GroupComponent11
  {
    public override ResultCollection ReadGroupBySid(Guid scopeId, string groupSid)
    {
      try
      {
        this.TraceEnter(4703090, nameof (ReadGroupBySid));
        this.PrepareStoredProcedure("prc_ReadGroupBySid");
        this.BindGuid("@scopeId", scopeId);
        this.BindString("@groupSid", groupSid, 256, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<GroupComponent.GroupIdentityData>((ObjectBinder<GroupComponent.GroupIdentityData>) this.GetGroupIdentitiesColumns());
        return resultCollection;
      }
      finally
      {
        this.TraceLeave(4703099, nameof (ReadGroupBySid));
      }
    }

    public override ResultCollection ReadGroupById(Guid scopeId, Guid groupId)
    {
      try
      {
        this.TraceEnter(4703090, nameof (ReadGroupById));
        this.PrepareStoredProcedure("prc_ReadGroupById");
        this.BindGuid("@scopeId", scopeId);
        this.BindGuid("@groupId", groupId);
        ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<GroupComponent.GroupIdentityData>((ObjectBinder<GroupComponent.GroupIdentityData>) this.GetGroupIdentitiesColumns());
        return resultCollection;
      }
      finally
      {
        this.TraceLeave(4703099, nameof (ReadGroupById));
      }
    }

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
          this.PrepareReadMemberships(identityIds, childrenQuery, parentsQuery, includeRestricted, minInactivatedTime, scopeId, returnVisibleIdentities, useXtpProc, new long?());
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

    protected void PrepareReadMemberships(
      IEnumerable<Guid> identityIds,
      QueryMembership childrenQuery,
      QueryMembership parentsQuery,
      bool includeRestricted,
      int? minInactivatedTime,
      Guid scopeId,
      bool returnVisibleIdentities,
      bool useXtpProc,
      long? minSequenceId)
    {
      if (!this.RequestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        this.PrepareStoredProcedure("prc_ReadGroupMembership");
        this.BindGuidTable("@identityIds", identityIds);
      }
      else
      {
        if (useXtpProc)
        {
          this.PrepareStoredProcedure("prc_ReadGroupMembership2_wrap");
          this.BindMemoGuidTable("@identityIds", identityIds);
        }
        else
        {
          this.PrepareStoredProcedure("prc_ReadGroupMembership2");
          this.BindGuidTable("@identityIds", identityIds);
        }
        if (minSequenceId.HasValue)
          this.BindLong("@minSequenceId", minSequenceId.Value);
      }
      this.BindInt("@childrenQuery", (int) childrenQuery);
      this.BindInt("@parentsQuery", (int) parentsQuery);
      this.BindBoolean("@includeRestricted", includeRestricted);
      this.BindNullableInt("@minInactivatedTime", minInactivatedTime.GetValueOrDefault(), 0);
      this.BindGuid("@scopeId", scopeId);
      this.BindBoolean("@returnVisibleIdentities", returnVisibleIdentities);
    }

    protected void PrepareReadScopeVisibility(Guid scopeId, Guid identityId)
    {
      this.PrepareStoredProcedure("prc_ReadScopeVisibility");
      this.BindGuid("@scopeId", scopeId);
      this.BindGuid("@identityId", identityId);
    }

    protected void PrepareReadScopeVisibilities(Guid scopeId, IEnumerable<Guid> identityIds)
    {
      this.PrepareStoredProcedure("prc_ReadScopeVisibilities");
      this.BindGuid("@scopeId", scopeId);
      this.BindGuidTable("@identityIds", identityIds);
    }
  }
}
