// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.GroupComponent15
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
  internal class GroupComponent15 : GroupComponent14
  {
    public override long ChangeGroupsIds(IEnumerable<KeyValuePair<Guid, Guid>> groupIdMap)
    {
      try
      {
        this.TraceEnter(4704520, nameof (ChangeGroupsIds));
        this.PrepareStoredProcedure("prc_ChangeGroupIds");
        this.BindKeyValuePairGuidGuidTable("@updates", groupIdMap);
        this.BindGuid("@eventAuthor", this.Author);
        return this.ReadLongSequenceIdResult(this.ExecuteReader());
      }
      finally
      {
        this.TraceLeave(4704529, nameof (ChangeGroupsIds));
      }
    }

    public override long CreateGroups(
      Guid scopeId,
      bool errorOnDuplicate,
      GroupDescription[] groups,
      bool addActiveScopeMembership = true)
    {
      try
      {
        this.TraceEnter(4703200, nameof (CreateGroups));
        this.PrepareStoredProcedure("prc_CreateGroups");
        this.BindGuid("@scopeId", scopeId);
        this.BindBoolean("@errorOnDuplicate", errorOnDuplicate);
        this.BindVersionedGroupTable("@groups", (IEnumerable<GroupDescription>) groups);
        this.BindGuid("@eventAuthor", this.Author);
        this.BindBoolean("@useGroupScopeVisibilityWrites", true);
        this.BindBoolean("@addActiveScopeMembership", addActiveScopeMembership);
        return this.ReadLongSequenceIdResult(this.ExecuteReader());
      }
      finally
      {
        this.TraceLeave(4703209, nameof (CreateGroups));
      }
    }

    public override long CreateScope(
      Guid parentScopeId,
      Guid scopeId,
      Guid localScopeId,
      string scopeName,
      Guid securingHostId,
      GroupScopeType scopeType,
      string rootGroupSid,
      string rootGroupName,
      string rootGroupDescription,
      string adminGroupSid,
      string adminGroupName,
      string adminGroupDescription)
    {
      try
      {
        this.TraceEnter(4703000, nameof (CreateScope));
        this.PrepareStoredProcedure("prc_CreateGroupScope");
        this.BindNullableGuid("@parentScopeId", parentScopeId);
        this.BindGuid("@scopeId", scopeId);
        this.BindGuid("@localScopeId", localScopeId);
        this.BindString("@scopeName", DBPath.UserToDatabasePath(scopeName), 256, false, SqlDbType.NVarChar);
        this.BindGuid("@securingHostId", securingHostId);
        this.BindByte("@scopeType", (byte) scopeType);
        this.BindString("@rootGroupSid", rootGroupSid, 256, true, SqlDbType.VarChar);
        this.BindString("@rootGroupName", rootGroupName, 256, true, SqlDbType.NVarChar);
        this.BindString("@rootGroupDescription", rootGroupDescription, 1024, true, SqlDbType.NVarChar);
        this.BindString("@adminGroupSid", adminGroupSid, 256, true, SqlDbType.VarChar);
        this.BindString("@adminGroupName", adminGroupName, 256, true, SqlDbType.NVarChar);
        this.BindString("@adminGroupDescription", adminGroupDescription, 1024, true, SqlDbType.NVarChar);
        this.BindGuid("@eventAuthor", this.Author);
        this.BindBoolean("@useGroupScopeVisibilityWrites", true);
        return this.ReadLongSequenceIdResult(this.ExecuteReader());
      }
      finally
      {
        this.TraceLeave(4703009, nameof (CreateScope));
      }
    }

    public override long DeleteGroupMemberships(
      IEnumerable<Guid> ids,
      bool removeParents,
      bool removeChildren)
    {
      try
      {
        this.TraceEnter(4703080, nameof (DeleteGroupMemberships));
        this.PrepareStoredProcedure("prc_DeleteGroupMemberships");
        this.BindGuidTable("@deletedIdentities", ids);
        this.BindBoolean("@removeParents", removeParents);
        this.BindBoolean("@removeChildren", removeChildren);
        this.BindGuid("@eventAuthor", this.Author);
        this.BindBoolean("@useGroupScopeVisibilityWrites", true);
        return this.ReadLongSequenceIdResult(this.ExecuteReader());
      }
      finally
      {
        this.TraceLeave(4703089, nameof (DeleteGroupMemberships));
      }
    }

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
        this.BindLong("@sequenceId", sequenceId);
        this.BindString("@everyoneSid", everyoneSid, 256, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        this.BindBoolean("@includeIdentities", includeIdentities);
        SqlDataReader reader = this.ExecuteReader();
        if (reader.Read())
        {
          GroupComponent.GroupChangesHeaderColumns changesHeaderColumns = new GroupComponent.GroupChangesHeaderColumns();
          everyoneId = changesHeaderColumns.Id.GetGuid((IDataReader) reader);
          lastSequenceId = changesHeaderColumns.SequenceId.GetInt64((IDataReader) reader);
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
          changes.AddBinder<GroupComponent.ReferencedIdentity>((ObjectBinder<GroupComponent.ReferencedIdentity>) new GroupComponent15.ReferencedIdentityColumns2());
        return changes;
      }
      finally
      {
        this.TraceLeave(4704509, nameof (GetChanges));
      }
    }

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
        this.ExecuteNonQuery();
      }
      finally
      {
        this.TraceLeave(4704519, nameof (ReplaceSnapshot));
      }
    }

    public override void SaveSnapshot(
      Guid parentScopeId,
      IEnumerable<IdentityScope> scopes,
      IEnumerable<GroupDescription> groups,
      IEnumerable<GroupMembership> memberships)
    {
      try
      {
        this.TraceEnter(4703140, nameof (SaveSnapshot));
        this.PrepareStoredProcedure("prc_SaveGroupSnapshot");
        this.BindNullableGuid("@parentScopeId", parentScopeId);
        this.BindGroupScopeTable2("@scopes", scopes);
        this.BindVersionedGroupTable("@groups", groups);
        this.BindGroupMembershipTable("@memberships", memberships.Select<GroupMembership, Tuple<IdentityDescriptor, Guid, bool>>((System.Func<GroupMembership, Tuple<IdentityDescriptor, Guid, bool>>) (membership => new Tuple<IdentityDescriptor, Guid, bool>(membership.Descriptor, membership.Id, membership.Active))));
        this.BindBoolean("@useGroupScopeVisibilityWrites", true);
        this.ExecuteNonQuery();
      }
      finally
      {
        this.TraceLeave(4703149, nameof (SaveSnapshot));
      }
    }

    public override long TransferGroupMembership(IEnumerable<KeyValuePair<Guid, Guid>> updates)
    {
      try
      {
        this.TraceEnter(4703400, nameof (TransferGroupMembership));
        this.PrepareStoredProcedure("prc_TransferGroupMembership");
        this.BindKeyValuePairGuidGuidTable("@updates", updates);
        this.BindGuid("@eventAuthor", this.Author);
        this.BindBoolean("@useGroupScopeVisibilityWrites", true);
        return this.ReadLongSequenceIdResult(this.ExecuteReader());
      }
      finally
      {
        this.TraceLeave(4703409, nameof (TransferGroupMembership));
      }
    }

    public override long UpdateGroupMembership(
      Guid scopeId,
      bool idempotent,
      bool incremental,
      bool insertInactiveUpdates,
      IEnumerable<Tuple<IdentityDescriptor, Guid, bool>> updates)
    {
      try
      {
        this.TraceEnter(4703060, nameof (UpdateGroupMembership));
        this.PrepareStoredProcedure("prc_UpdateGroupMembership");
        this.BindGuid("@scopeId", scopeId);
        this.BindBoolean("@idempotent", idempotent);
        this.BindBoolean("@incremental", incremental);
        this.BindBoolean("@insertInactiveUpdates", insertInactiveUpdates);
        this.BindGroupMembershipTable("@updates", updates);
        this.BindGuid("@eventAuthor", this.Author);
        this.BindBoolean("@useGroupScopeVisibilityWrites", true);
        return this.ReadLongSequenceIdResult(this.ExecuteReader());
      }
      finally
      {
        this.TraceLeave(4703069, nameof (UpdateGroupMembership));
      }
    }

    public override long UpdateScopes(
      bool hardDelete,
      IEnumerable<Guid> scopeDeletes,
      IEnumerable<KeyValuePair<Guid, string>> scopeUpdates)
    {
      try
      {
        this.TraceEnter(4703020, nameof (UpdateScopes));
        this.PrepareStoredProcedure("prc_UpdateGroupScopes");
        this.BindBoolean("@hardDelete", hardDelete);
        this.BindGuidTable("@scopeDeletes", scopeDeletes);
        if (scopeUpdates != null)
        {
          List<KeyValuePair<Guid, string>> keyValuePairList = new List<KeyValuePair<Guid, string>>();
          foreach (KeyValuePair<Guid, string> scopeUpdate in scopeUpdates)
            keyValuePairList.Add(new KeyValuePair<Guid, string>(scopeUpdate.Key, DBPath.UserToDatabasePath(scopeUpdate.Value)));
          scopeUpdates = (IEnumerable<KeyValuePair<Guid, string>>) keyValuePairList;
        }
        this.BindKeyValuePairGuidStringTable("@scopeUpdates", scopeUpdates);
        this.BindGuid("@eventAuthor", this.Author);
        this.BindBoolean("@useGroupScopeVisibilityWrites", true);
        return this.ReadLongSequenceIdResult(this.ExecuteReader());
      }
      finally
      {
        this.TraceLeave(4703029, nameof (UpdateScopes));
      }
    }

    public override long UpdateGroups(
      Guid scopeId,
      IEnumerable<string> groupDeletes,
      IEnumerable<GroupComponent.GroupUpdate> groupUpdates)
    {
      try
      {
        this.TraceEnter(4703050, nameof (UpdateGroups));
        this.PrepareStoredProcedure("prc_UpdateGroups");
        this.BindGuid("@scopeId", scopeId);
        this.BindStringTable("@groupDeletes", groupDeletes, maxLength: 256, nvarchar: false);
        this.BindGroupUpdateTable("@groupUpdates", groupUpdates);
        this.BindGuid("@eventAuthor", this.Author);
        this.BindBoolean("@useGroupScopeVisibilityWrites", true);
        return this.ReadLongSequenceIdResult(this.ExecuteReader());
      }
      finally
      {
        this.TraceLeave(4703059, nameof (UpdateGroups));
      }
    }

    public override long UpdateIdentityMembership(
      bool incremental,
      IEnumerable<Tuple<IdentityDescriptor, Guid, bool>> updates)
    {
      try
      {
        this.TraceEnter(4703070, nameof (UpdateIdentityMembership));
        this.PrepareStoredProcedure("prc_UpdateIdentityMembership");
        this.BindBoolean("@incremental", incremental);
        this.BindGroupMembershipTable("@updates", updates);
        this.BindGuid("@eventAuthor", this.Author);
        this.BindBoolean("@useGroupScopeVisibilityWrites", true);
        return this.ReadLongSequenceIdResult(this.ExecuteReader());
      }
      finally
      {
        this.TraceLeave(4703079, nameof (UpdateIdentityMembership));
      }
    }

    protected long ReadLongSequenceIdResult(SqlDataReader reader) => reader.Read() ? new SqlColumnBinder("SequenceId").GetInt64((IDataReader) reader) : throw new UnexpectedDatabaseResultException(this.ProcedureName);

    public override IList<Guid> GetAncestorScopeIds(Guid scopeId, bool includeInactiveScopes = false)
    {
      try
      {
        this.TraceEnter(4704540, nameof (GetAncestorScopeIds));
        this.PrepareStoredProcedure("prc_GetScopeAncestorIds");
        this.BindGuid("@scopeId", scopeId);
        this.BindBoolean("@includeInactiveScopes", includeInactiveScopes);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<Guid>((ObjectBinder<Guid>) new GroupComponent.ScopeIdColumns());
          return (IList<Guid>) resultCollection.GetCurrent<Guid>().ToList<Guid>();
        }
      }
      finally
      {
        this.TraceLeave(4704549, nameof (GetAncestorScopeIds));
      }
    }

    protected override GroupComponent.GroupChangesColumns2 GetGroupChangesColumnBinder() => (GroupComponent.GroupChangesColumns2) new GroupComponent.GroupChangesColumns3();

    protected class ReferencedIdentityColumns2 : ObjectBinder<GroupComponent.ReferencedIdentity>
    {
      private SqlColumnBinder Id = new SqlColumnBinder(nameof (Id));
      private SqlColumnBinder IsLocal = new SqlColumnBinder(nameof (IsLocal));

      protected override GroupComponent.ReferencedIdentity Bind() => new GroupComponent.ReferencedIdentity()
      {
        IdentityId = this.Id.GetGuid((IDataReader) this.Reader),
        Location = this.IsLocal.GetBoolean((IDataReader) this.Reader) ? GroupComponent.ReferencedIdentityLocation.Local : GroupComponent.ReferencedIdentityLocation.Remote
      };
    }
  }
}
