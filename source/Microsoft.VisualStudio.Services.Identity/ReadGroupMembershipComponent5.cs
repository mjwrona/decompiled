// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.ReadGroupMembershipComponent5
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity.SqlComponent;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class ReadGroupMembershipComponent5 : ReadGroupMembershipComponent4
  {
    protected readonly ReadGroupMembership3ConfigHelper ReadGroupMembership3ConfigHelper;

    public ReadGroupMembershipComponent5() => this.ReadGroupMembership3ConfigHelper = new ReadGroupMembership3ConfigHelper();

    protected override void Initialize(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      Guid dataspaceIdentifier,
      int serviceVersion,
      DatabaseConnectionType connectionType,
      ITFLogger logger)
    {
      this.ReadGroupMembership3ConfigHelper.CanUseReadGroupMembership3(requestContext);
      base.Initialize(requestContext, dataspaceCategory, dataspaceIdentifier, serviceVersion, connectionType, logger);
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
        this.TraceEnter(4703111, nameof (ReadMemberships));
        bool flag = false;
        if (this.RequestContext.ExecutionEnvironment.IsHostedDeployment && childrenQuery == QueryMembership.None && parentsQuery == QueryMembership.None)
        {
          if (identityIds != null && identityIds.Count<Guid>() == 1)
            this.PrepareReadScopeVisibility(scopeId, identityIds.First<Guid>());
          else
            this.PrepareReadScopeVisibilities(scopeId, identityIds);
        }
        else if (this.ReadGroupMembership3ConfigHelper.CanUseReadGroupMembership3(this.RequestContext))
        {
          flag = true;
          this.PrepareReadMemberships3(identityIds, childrenQuery, parentsQuery, includeRestricted, minInactivatedTime, scopeId, returnVisibleIdentities, useXtpProc, new long?(minSequenceId), inScopeMembershipsOnly);
        }
        else
        {
          this.PrepareReadMemberships(identityIds, childrenQuery, parentsQuery, includeRestricted, minInactivatedTime, scopeId, returnVisibleIdentities, useXtpProc, new long?(minSequenceId));
          if (this.RequestContext.ExecutionEnvironment.IsHostedDeployment)
            this.BindBoolean("@inScopeMembershipsOnly", inScopeMembershipsOnly);
        }
        ResultCollectionWithNullObjectBinder<GroupMembership> nullObjectBinder = new ResultCollectionWithNullObjectBinder<GroupMembership>((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        if (parentsQuery == QueryMembership.None & flag)
          nullObjectBinder.AddBinder<GroupMembership>((ObjectBinder<GroupMembership>) new NullObjectBinder<GroupMembership>());
        else
          nullObjectBinder.AddBinder<GroupMembership>((ObjectBinder<GroupMembership>) new ReadGroupMembershipsComponentBase.ParentColumns());
        if (childrenQuery == QueryMembership.None & flag)
          nullObjectBinder.AddBinder<GroupMembership>((ObjectBinder<GroupMembership>) new NullObjectBinder<GroupMembership>());
        else
          nullObjectBinder.AddBinder<GroupMembership>((ObjectBinder<GroupMembership>) new ReadGroupMembershipsComponentBase.ChildrenColumns());
        if (returnVisibleIdentities)
          nullObjectBinder.AddBinder<ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData>((ObjectBinder<ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData>) new ReadGroupMembershipsComponentBase.ScopeFilteredIdentitiesColumns());
        return (ResultCollection) nullObjectBinder;
      }
      finally
      {
        this.TraceLeave(4703118, nameof (ReadMemberships));
      }
    }

    private string GetReadMemberships3ProcName(
      QueryMembership childrenQuery,
      QueryMembership parentsQuery,
      bool returnVisibleIdentities,
      bool useXtpProc,
      bool inScopeMembershipsOnly)
    {
      if (childrenQuery == QueryMembership.None)
        return parentsQuery == QueryMembership.Direct ? (inScopeMembershipsOnly ? "prc_ReadGroupMembership3_Parents_DirectOnly_InScopeOnly" : "prc_ReadGroupMembership3_Parents_DirectOnly_NoScope") : (inScopeMembershipsOnly ? (useXtpProc ? "prc_ReadGroupMembership3_Parents_ExpandedUp_InScopeOnly_xtp" : "prc_ReadGroupMembership3_Parents_ExpandedUp_InScopeOnly") : (useXtpProc ? "prc_ReadGroupMembership3_Parents_ExpandedUp_NoScope_xtp" : "prc_ReadGroupMembership3_Parents_ExpandedUp_NoScope"));
      if (parentsQuery != 0 | returnVisibleIdentities)
        return "prc_ReadGroupMembership3_Wrap_ParentsThenChildren";
      if (childrenQuery == QueryMembership.Direct)
        return "prc_ReadGroupMembership3_Children_DirectOnly";
      int num = useXtpProc ? 1 : 0;
      return "prc_ReadGroupMembership3_Children_ExpandedDown_xtp";
    }

    protected void PrepareReadMemberships3(
      IEnumerable<Guid> identityIds,
      QueryMembership childrenQuery,
      QueryMembership parentsQuery,
      bool includeRestricted,
      int? minInactivatedTime,
      Guid scopeId,
      bool returnVisibleIdentities,
      bool useXtpProc,
      long? minSequenceId,
      bool inScopeMembershipsOnly)
    {
      if (!this.RequestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        this.PrepareStoredProcedure("prc_ReadGroupMembership");
        this.BindGuidTable("@identityIds", identityIds);
      }
      else
      {
        this.PrepareStoredProcedure(this.GetReadMemberships3ProcName(childrenQuery, parentsQuery, returnVisibleIdentities, useXtpProc, inScopeMembershipsOnly));
        this.BindMemoGuidTable("@identityIds", identityIds);
        if (minSequenceId.HasValue)
          this.BindLong("@minSequenceId", minSequenceId.Value);
      }
      if (childrenQuery != QueryMembership.None)
        this.BindNullableInt("@minInactivatedTime", minInactivatedTime.GetValueOrDefault(), 0);
      if (!(parentsQuery != 0 | returnVisibleIdentities))
        return;
      if (childrenQuery != QueryMembership.None)
      {
        this.BindInt("@childrenQuery", (int) childrenQuery);
        this.BindInt("@parentsQuery", (int) parentsQuery);
        this.BindBoolean("@inScopeMembershipsOnly", inScopeMembershipsOnly);
      }
      this.BindBoolean("@includeRestricted", includeRestricted);
      this.BindGuid("@scopeId", scopeId);
      this.BindBoolean("@returnVisibleIdentities", returnVisibleIdentities);
    }
  }
}
