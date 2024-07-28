// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.GroupComponent9
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class GroupComponent9 : GroupComponent8
  {
    public override ResultCollection GetChanges(
      long sequenceId,
      string everyoneSid,
      Guid scopeId,
      out Guid everyoneId,
      out long lastSequenceId,
      bool includeIdentities = false,
      bool getScopedGroupChanges = false)
    {
      try
      {
        this.TraceEnter(4704500, nameof (GetChanges));
        this.PrepareStoredProcedure("prc_GetGroupChanges");
        this.BindInt("@sequenceId", GroupComponent.ReduceMaxLongToMaxInt(sequenceId));
        this.BindString("@everyoneSid", everyoneSid, 256, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        this.BindBoolean("@includeIdentities", includeIdentities);
        this.BindBoolean("@useGroupAudit", this.RequestContext.ExecutionEnvironment.IsHostedDeployment);
        this.BindLong("@firstAuditSequenceId", 1L);
        SqlDataReader reader = this.ExecuteReader();
        if (reader.Read())
        {
          GroupComponent.GroupChangesHeaderColumns changesHeaderColumns = new GroupComponent.GroupChangesHeaderColumns();
          everyoneId = changesHeaderColumns.Id.GetGuid((IDataReader) reader);
          lastSequenceId = (long) changesHeaderColumns.SequenceId.GetInt32((IDataReader) reader);
        }
        else
        {
          everyoneId = Guid.Empty;
          lastSequenceId = 0L;
        }
        reader.NextResult();
        ResultCollection changes = new ResultCollection((IDataReader) reader, this.ProcedureName, this.RequestContext);
        changes.AddBinder<MembershipChangeInfo>((ObjectBinder<MembershipChangeInfo>) this.GetGroupChangesColumnBinder());
        changes.AddBinder<Guid>((ObjectBinder<Guid>) new GroupComponent.IdentityIdColumns());
        if (includeIdentities)
          changes.AddBinder<GroupComponent.ReferencedIdentity>((ObjectBinder<GroupComponent.ReferencedIdentity>) new GroupComponent5.ReferencedIdentityColumns());
        return changes;
      }
      finally
      {
        this.TraceLeave(4704509, nameof (GetChanges));
      }
    }

    protected virtual GroupComponent.GroupChangesColumns2 GetGroupChangesColumnBinder() => new GroupComponent.GroupChangesColumns2();

    public override void ReplaceSnapshot(
      Guid parentScopeId,
      IEnumerable<IdentityScope> scopes,
      IEnumerable<GroupDescription> groups,
      IEnumerable<GroupMembership> memberships)
    {
      try
      {
        this.TraceEnter(4704510, nameof (ReplaceSnapshot));
        this.PrepareStoredProcedure("prc_ReplaceGroupSnapshot");
        this.BindNullableGuid("@parentScopeId", parentScopeId);
        this.BindGroupScopeTable2("@scopes", scopes);
        this.BindVersionedGroupTable("@groups", groups);
        this.BindGroupMembershipTable("@memberships", memberships.Select<GroupMembership, Tuple<IdentityDescriptor, Guid, bool>>((System.Func<GroupMembership, Tuple<IdentityDescriptor, Guid, bool>>) (membership => new Tuple<IdentityDescriptor, Guid, bool>(membership.Descriptor, membership.Id, membership.Active))));
        this.BindBoolean("@useGroupScopeVisibilityWrites", true);
        this.BindBoolean("@updateGroupAudit", this.RequestContext.ExecutionEnvironment.IsHostedDeployment);
        this.ExecuteNonQuery();
      }
      finally
      {
        this.TraceLeave(4704519, nameof (ReplaceSnapshot));
      }
    }

    public override long ChangeGroupsIds(IEnumerable<KeyValuePair<Guid, Guid>> groupIdMap)
    {
      try
      {
        this.TraceEnter(4704520, nameof (ChangeGroupsIds));
        this.PrepareStoredProcedure("prc_ChangeGroupIds");
        this.BindKeyValuePairGuidGuidTable("@updates", groupIdMap);
        this.BindGuid("@eventAuthor", this.Author);
        return (long) this.ReadIntSequenceIdResult(this.ExecuteReader());
      }
      finally
      {
        this.TraceLeave(4704529, nameof (ChangeGroupsIds));
      }
    }

    public override IList<GroupMembershipCycleRecord> GetCyclicGroupMemberships(Guid scopeId)
    {
      try
      {
        this.TraceEnter(4704530, nameof (GetCyclicGroupMemberships));
        string groupMemberships = HostingResources.stmt_GetCyclicGroupMemberships();
        this.PrepareSqlBatch(groupMemberships.Length);
        this.AddStatement(groupMemberships);
        this.BindGuid("@scopeId", scopeId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<GroupMembershipCycleRecord>((ObjectBinder<GroupMembershipCycleRecord>) new GroupComponent.GroupMembershipCycleRecordColumns());
          return (IList<GroupMembershipCycleRecord>) resultCollection.GetCurrent<GroupMembershipCycleRecord>().ToList<GroupMembershipCycleRecord>();
        }
      }
      finally
      {
        this.TraceLeave(4704539, nameof (GetCyclicGroupMemberships));
      }
    }
  }
}
