// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.ReadGroupMembershipComponent6
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
  internal class ReadGroupMembershipComponent6 : ReadGroupMembershipComponent5
  {
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
        if (this.RequestContext.ExecutionEnvironment.IsHostedDeployment && childrenQuery == QueryMembership.None && parentsQuery == QueryMembership.None)
        {
          if (identityIds != null && identityIds.Count<Guid>() == 1)
            this.PrepareReadScopeVisibility(scopeId, identityIds.First<Guid>());
          else
            this.PrepareReadScopeVisibilities(scopeId, identityIds);
          ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
          resultCollection.AddBinder<GroupMembership>((ObjectBinder<GroupMembership>) new ReadGroupMembershipsComponentBase.ParentColumns());
          resultCollection.AddBinder<GroupMembership>((ObjectBinder<GroupMembership>) new ReadGroupMembershipsComponentBase.ChildrenColumns());
          if (returnVisibleIdentities)
            resultCollection.AddBinder<ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData>((ObjectBinder<ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData>) new ReadGroupMembershipsComponentBase.ScopeFilteredIdentitiesColumns());
          return resultCollection;
        }
        return this.ReadGroupMembership3ConfigHelper.CanUseReadGroupMembership3(this.RequestContext) && this.RequestContext.ExecutionEnvironment.IsHostedDeployment ? this.ExecuteRGM3(identityIds, childrenQuery, parentsQuery, includeRestricted, minInactivatedTime, scopeId, returnVisibleIdentities, useXtpProc, new long?(minSequenceId), inScopeMembershipsOnly) : this.ExecuteRGM2(identityIds, childrenQuery, parentsQuery, includeRestricted, minInactivatedTime, scopeId, returnVisibleIdentities, useXtpProc, minSequenceId, inScopeMembershipsOnly);
      }
      finally
      {
        this.TraceLeave(4703118, nameof (ReadMemberships));
      }
    }

    private ResultCollection ExecuteRGM2(
      IEnumerable<Guid> identityIds,
      QueryMembership childrenQuery,
      QueryMembership parentsQuery,
      bool includeRestricted,
      int? minInactivatedTime,
      Guid scopeId,
      bool returnVisibleIdentities,
      bool useXtpProc,
      long minSequenceId,
      bool inScopeMembershipsOnly)
    {
      this.PrepareReadMemberships(identityIds, childrenQuery, parentsQuery, includeRestricted, minInactivatedTime, scopeId, returnVisibleIdentities, useXtpProc, new long?(minSequenceId));
      if (this.RequestContext.ExecutionEnvironment.IsHostedDeployment)
        this.BindBoolean("@inScopeMembershipsOnly", inScopeMembershipsOnly);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<GroupMembership>((ObjectBinder<GroupMembership>) new ReadGroupMembershipsComponentBase.ParentColumns());
      resultCollection.AddBinder<GroupMembership>((ObjectBinder<GroupMembership>) new ReadGroupMembershipsComponentBase.ChildrenColumns());
      if (returnVisibleIdentities)
        resultCollection.AddBinder<ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData>((ObjectBinder<ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData>) new ReadGroupMembershipsComponentBase.ScopeFilteredIdentitiesColumns());
      return resultCollection;
    }

    protected ResultCollection ExecuteRGM3(
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
      if (childrenQuery == QueryMembership.None)
        return parentsQuery == QueryMembership.Direct ? (inScopeMembershipsOnly ? (ResultCollection) this.ExecuteRGM3ParentsDirectOnlyInScopeOnly(identityIds, includeRestricted, scopeId, returnVisibleIdentities, minSequenceId) : (ResultCollection) this.ExecuteRGM3ParentsDirectOnlyNoScope(identityIds, includeRestricted, scopeId, returnVisibleIdentities, minSequenceId)) : (inScopeMembershipsOnly ? (useXtpProc ? (ResultCollection) this.ExecuteRGM3ParentsExpandedUpInScopeOnlyXtp(identityIds, includeRestricted, scopeId, returnVisibleIdentities, minSequenceId) : (ResultCollection) this.ExecuteRGM3ParentsExpandedUpInScopeOnly(identityIds, includeRestricted, scopeId, returnVisibleIdentities, minSequenceId)) : (useXtpProc ? (ResultCollection) this.ExecuteRGM3ParentsExpandedUpNoScopeXtp(identityIds, includeRestricted, scopeId, returnVisibleIdentities, minSequenceId) : (ResultCollection) this.ExecuteRGM3ParentsExpandedUpNoScope(identityIds, includeRestricted, scopeId, returnVisibleIdentities, minSequenceId)));
      if (parentsQuery != 0 | returnVisibleIdentities)
        return (ResultCollection) this.ExecuteRGM3WrapParentsThenChildren(identityIds, childrenQuery, parentsQuery, includeRestricted, minInactivatedTime, scopeId, returnVisibleIdentities, inScopeMembershipsOnly, minSequenceId);
      if (childrenQuery == QueryMembership.Direct)
        return (ResultCollection) this.ExecuteRGMChildrenDirectOnly(identityIds, includeRestricted, minInactivatedTime, minSequenceId);
      return useXtpProc ? (ResultCollection) this.ExecuteRGM3ChildrenExpandedDownXtp(identityIds, includeRestricted, minInactivatedTime, minSequenceId) : (ResultCollection) this.ExecuteRGM3ChildrenExpandedDown(identityIds, includeRestricted, minInactivatedTime, minSequenceId);
    }

    private ResultCollectionWithNullObjectBinder<GroupMembership> ExecuteRGM3ParentsDirectOnlyInScopeOnly(
      IEnumerable<Guid> identityIds,
      bool includeRestricted,
      Guid scopeId,
      bool returnVisibleIdentities,
      long? minSequenceId)
    {
      this.PrepareStoredProcedure("prc_RGM3_Parents_DirectOnly_InScopeOnly");
      this.BindMemoGuidTable("@identityIds", identityIds);
      this.BindBoolean("@includeRestricted", includeRestricted);
      this.BindGuid("@scopeId", scopeId);
      this.BindBoolean("@returnVisibleIdentities", returnVisibleIdentities);
      if (minSequenceId.HasValue)
        this.BindLong("@minSequenceId", minSequenceId.Value);
      ResultCollectionWithNullObjectBinder<GroupMembership> nullObjectBinder = new ResultCollectionWithNullObjectBinder<GroupMembership>((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      nullObjectBinder.AddBinder<GroupMembership>((ObjectBinder<GroupMembership>) new ReadGroupMembershipsComponentBase.ParentColumns());
      nullObjectBinder.AddBinder<GroupMembership>((ObjectBinder<GroupMembership>) new NullObjectBinder<GroupMembership>());
      if (returnVisibleIdentities)
        nullObjectBinder.AddBinder<ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData>((ObjectBinder<ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData>) new ReadGroupMembershipsComponentBase.ScopeFilteredIdentitiesColumns());
      return nullObjectBinder;
    }

    private ResultCollectionWithNullObjectBinder<GroupMembership> ExecuteRGM3ParentsDirectOnlyNoScope(
      IEnumerable<Guid> identityIds,
      bool includeRestricted,
      Guid scopeId,
      bool returnVisibleIdentities,
      long? minSequenceId)
    {
      this.PrepareStoredProcedure("prc_RGM3_Parents_DirectOnly_NoScope");
      this.BindMemoGuidTable("@identityIds", identityIds);
      this.BindBoolean("@includeRestricted", includeRestricted);
      this.BindGuid("@scopeId", scopeId);
      this.BindBoolean("@returnVisibleIdentities", returnVisibleIdentities);
      if (minSequenceId.HasValue)
        this.BindLong("@minSequenceId", minSequenceId.Value);
      ResultCollectionWithNullObjectBinder<GroupMembership> nullObjectBinder = new ResultCollectionWithNullObjectBinder<GroupMembership>((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      nullObjectBinder.AddBinder<GroupMembership>((ObjectBinder<GroupMembership>) new ReadGroupMembershipsComponentBase.ParentColumns());
      nullObjectBinder.AddBinder<GroupMembership>((ObjectBinder<GroupMembership>) new NullObjectBinder<GroupMembership>());
      if (returnVisibleIdentities)
        nullObjectBinder.AddBinder<ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData>((ObjectBinder<ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData>) new ReadGroupMembershipsComponentBase.ScopeFilteredIdentitiesColumns());
      return nullObjectBinder;
    }

    private ResultCollectionWithNullObjectBinder<GroupMembership> ExecuteRGM3ParentsExpandedUpInScopeOnlyXtp(
      IEnumerable<Guid> identityIds,
      bool includeRestricted,
      Guid scopeId,
      bool returnVisibleIdentities,
      long? minSequenceId)
    {
      this.PrepareStoredProcedure("prc_RGM3_Parents_ExpandedUp_InScopeOnly_xtp");
      this.BindMemoGuidTable("@identityIds", identityIds);
      this.BindBoolean("@includeRestricted", includeRestricted);
      this.BindGuid("@scopeId", scopeId);
      this.BindBoolean("@returnVisibleIdentities", returnVisibleIdentities);
      if (minSequenceId.HasValue)
        this.BindLong("@minSequenceId", minSequenceId.Value);
      ResultCollectionWithNullObjectBinder<GroupMembership> nullObjectBinder = new ResultCollectionWithNullObjectBinder<GroupMembership>((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      nullObjectBinder.AddBinder<GroupMembership>((ObjectBinder<GroupMembership>) new ReadGroupMembershipsComponentBase.ParentColumns());
      nullObjectBinder.AddBinder<GroupMembership>((ObjectBinder<GroupMembership>) new NullObjectBinder<GroupMembership>());
      if (returnVisibleIdentities)
        nullObjectBinder.AddBinder<ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData>((ObjectBinder<ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData>) new ReadGroupMembershipsComponentBase.ScopeFilteredIdentitiesColumns());
      return nullObjectBinder;
    }

    private ResultCollectionWithNullObjectBinder<GroupMembership> ExecuteRGM3ParentsExpandedUpInScopeOnly(
      IEnumerable<Guid> identityIds,
      bool includeRestricted,
      Guid scopeId,
      bool returnVisibleIdentities,
      long? minSequenceId)
    {
      this.PrepareStoredProcedure("prc_RGM3_Parents_ExpandedUp_InScopeOnly");
      this.BindMemoGuidTable("@identityIds", identityIds);
      this.BindBoolean("@includeRestricted", includeRestricted);
      this.BindGuid("@scopeId", scopeId);
      this.BindBoolean("@returnVisibleIdentities", returnVisibleIdentities);
      if (minSequenceId.HasValue)
        this.BindLong("@minSequenceId", minSequenceId.Value);
      ResultCollectionWithNullObjectBinder<GroupMembership> nullObjectBinder = new ResultCollectionWithNullObjectBinder<GroupMembership>((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      nullObjectBinder.AddBinder<GroupMembership>((ObjectBinder<GroupMembership>) new ReadGroupMembershipsComponentBase.ParentColumns());
      nullObjectBinder.AddBinder<GroupMembership>((ObjectBinder<GroupMembership>) new NullObjectBinder<GroupMembership>());
      if (returnVisibleIdentities)
        nullObjectBinder.AddBinder<ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData>((ObjectBinder<ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData>) new ReadGroupMembershipsComponentBase.ScopeFilteredIdentitiesColumns());
      return nullObjectBinder;
    }

    private ResultCollectionWithNullObjectBinder<GroupMembership> ExecuteRGM3ParentsExpandedUpNoScopeXtp(
      IEnumerable<Guid> identityIds,
      bool includeRestricted,
      Guid scopeId,
      bool returnVisibleIdentities,
      long? minSequenceId)
    {
      this.PrepareStoredProcedure("prc_RGM3_Parents_ExpandedUp_NoScope_xtp");
      this.BindMemoGuidTable("@identityIds", identityIds);
      this.BindBoolean("@includeRestricted", includeRestricted);
      this.BindGuid("@scopeId", scopeId);
      this.BindBoolean("@returnVisibleIdentities", returnVisibleIdentities);
      if (minSequenceId.HasValue)
        this.BindLong("@minSequenceId", minSequenceId.Value);
      ResultCollectionWithNullObjectBinder<GroupMembership> nullObjectBinder = new ResultCollectionWithNullObjectBinder<GroupMembership>((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      nullObjectBinder.AddBinder<GroupMembership>((ObjectBinder<GroupMembership>) new ReadGroupMembershipsComponentBase.ParentColumns());
      nullObjectBinder.AddBinder<GroupMembership>((ObjectBinder<GroupMembership>) new NullObjectBinder<GroupMembership>());
      if (returnVisibleIdentities)
        nullObjectBinder.AddBinder<ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData>((ObjectBinder<ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData>) new ReadGroupMembershipsComponentBase.ScopeFilteredIdentitiesColumns());
      return nullObjectBinder;
    }

    private ResultCollectionWithNullObjectBinder<GroupMembership> ExecuteRGM3ParentsExpandedUpNoScope(
      IEnumerable<Guid> identityIds,
      bool includeRestricted,
      Guid scopeId,
      bool returnVisibleIdentities,
      long? minSequenceId)
    {
      this.PrepareStoredProcedure("prc_RGM3_Parents_ExpandedUp_NoScope");
      this.BindMemoGuidTable("@identityIds", identityIds);
      this.BindBoolean("@includeRestricted", includeRestricted);
      this.BindGuid("@scopeId", scopeId);
      this.BindBoolean("@returnVisibleIdentities", returnVisibleIdentities);
      if (minSequenceId.HasValue)
        this.BindLong("@minSequenceId", minSequenceId.Value);
      ResultCollectionWithNullObjectBinder<GroupMembership> nullObjectBinder = new ResultCollectionWithNullObjectBinder<GroupMembership>((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      nullObjectBinder.AddBinder<GroupMembership>((ObjectBinder<GroupMembership>) new ReadGroupMembershipsComponentBase.ParentColumns());
      nullObjectBinder.AddBinder<GroupMembership>((ObjectBinder<GroupMembership>) new NullObjectBinder<GroupMembership>());
      if (returnVisibleIdentities)
        nullObjectBinder.AddBinder<ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData>((ObjectBinder<ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData>) new ReadGroupMembershipsComponentBase.ScopeFilteredIdentitiesColumns());
      return nullObjectBinder;
    }

    private ResultCollectionWithNullObjectBinder<GroupMembership> ExecuteRGM3WrapParentsThenChildren(
      IEnumerable<Guid> identityIds,
      QueryMembership childrenQuery,
      QueryMembership parentsQuery,
      bool includeRestricted,
      int? minInactivatedTime,
      Guid scopeId,
      bool returnVisibleIdentities,
      bool inScopeMembershipsOnly,
      long? minSequenceId)
    {
      this.PrepareStoredProcedure("prc_RGM3_Wrap_ParentsThenChildren");
      this.BindMemoGuidTable("@identityIds", identityIds);
      this.BindInt("@childrenQuery", (int) childrenQuery);
      this.BindInt("@parentsQuery", (int) parentsQuery);
      this.BindBoolean("@includeRestricted", includeRestricted);
      this.BindNullableInt("@minInactivatedTime", minInactivatedTime.GetValueOrDefault(), 0);
      this.BindGuid("@scopeId", scopeId);
      this.BindBoolean("@returnVisibleIdentities", returnVisibleIdentities);
      this.BindBoolean("@inScopeMembershipsOnly", inScopeMembershipsOnly);
      if (minSequenceId.HasValue)
        this.BindLong("@minSequenceId", minSequenceId.Value);
      ResultCollectionWithNullObjectBinder<GroupMembership> nullObjectBinder = new ResultCollectionWithNullObjectBinder<GroupMembership>((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      if (parentsQuery == QueryMembership.None)
        nullObjectBinder.AddBinder<GroupMembership>((ObjectBinder<GroupMembership>) new NullObjectBinder<GroupMembership>());
      else
        nullObjectBinder.AddBinder<GroupMembership>((ObjectBinder<GroupMembership>) new ReadGroupMembershipsComponentBase.ParentColumns());
      if (childrenQuery == QueryMembership.None)
        nullObjectBinder.AddBinder<GroupMembership>((ObjectBinder<GroupMembership>) new NullObjectBinder<GroupMembership>());
      else
        nullObjectBinder.AddBinder<GroupMembership>((ObjectBinder<GroupMembership>) new ReadGroupMembershipsComponentBase.ChildrenColumns());
      if (returnVisibleIdentities)
        nullObjectBinder.AddBinder<ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData>((ObjectBinder<ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData>) new ReadGroupMembershipsComponentBase.ScopeFilteredIdentitiesColumns());
      return nullObjectBinder;
    }

    private ResultCollectionWithNullObjectBinder<GroupMembership> ExecuteRGMChildrenDirectOnly(
      IEnumerable<Guid> identityIds,
      bool includeRestricted,
      int? minInactivatedTime,
      long? minSequenceId)
    {
      this.PrepareStoredProcedure("prc_RGM3_Children_DirectOnly");
      this.BindMemoGuidTable("@identityIds", identityIds);
      this.BindBoolean("@includeRestricted", includeRestricted);
      this.BindNullableInt("@minInactivatedTime", minInactivatedTime.GetValueOrDefault(), 0);
      if (minSequenceId.HasValue)
        this.BindLong("@minSequenceId", minSequenceId.Value);
      ResultCollectionWithNullObjectBinder<GroupMembership> nullObjectBinder = new ResultCollectionWithNullObjectBinder<GroupMembership>((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      nullObjectBinder.AddBinder<GroupMembership>((ObjectBinder<GroupMembership>) new NullObjectBinder<GroupMembership>());
      nullObjectBinder.AddBinder<GroupMembership>((ObjectBinder<GroupMembership>) new ReadGroupMembershipsComponentBase.ChildrenColumns());
      return nullObjectBinder;
    }

    private ResultCollectionWithNullObjectBinder<GroupMembership> ExecuteRGM3ChildrenExpandedDownXtp(
      IEnumerable<Guid> identityIds,
      bool includeRestricted,
      int? minInactivatedTime,
      long? minSequenceId)
    {
      this.PrepareStoredProcedure("prc_RGM3_Children_ExpandedDown_xtp");
      this.BindMemoGuidTable("@identityIds", identityIds);
      this.BindBoolean("@includeRestricted", includeRestricted);
      this.BindNullableInt("@minInactivatedTime", minInactivatedTime.GetValueOrDefault(), 0);
      if (minSequenceId.HasValue)
        this.BindLong("@minSequenceId", minSequenceId.Value);
      ResultCollectionWithNullObjectBinder<GroupMembership> nullObjectBinder = new ResultCollectionWithNullObjectBinder<GroupMembership>((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      nullObjectBinder.AddBinder<GroupMembership>((ObjectBinder<GroupMembership>) new NullObjectBinder<GroupMembership>());
      nullObjectBinder.AddBinder<GroupMembership>((ObjectBinder<GroupMembership>) new ReadGroupMembershipsComponentBase.ChildrenColumns());
      return nullObjectBinder;
    }

    private ResultCollectionWithNullObjectBinder<GroupMembership> ExecuteRGM3ChildrenExpandedDown(
      IEnumerable<Guid> identityIds,
      bool includeRestricted,
      int? minInactivatedTime,
      long? minSequenceId)
    {
      this.PrepareStoredProcedure("prc_RGM3_Children_ExpandedDown");
      this.BindMemoGuidTable("@identityIds", identityIds);
      this.BindBoolean("@includeRestricted", includeRestricted);
      this.BindNullableInt("@minInactivatedTime", minInactivatedTime.GetValueOrDefault(), 0);
      if (minSequenceId.HasValue)
        this.BindLong("@minSequenceId", minSequenceId.Value);
      ResultCollectionWithNullObjectBinder<GroupMembership> nullObjectBinder = new ResultCollectionWithNullObjectBinder<GroupMembership>((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      nullObjectBinder.AddBinder<GroupMembership>((ObjectBinder<GroupMembership>) new NullObjectBinder<GroupMembership>());
      nullObjectBinder.AddBinder<GroupMembership>((ObjectBinder<GroupMembership>) new ReadGroupMembershipsComponentBase.ChildrenColumns());
      return nullObjectBinder;
    }
  }
}
