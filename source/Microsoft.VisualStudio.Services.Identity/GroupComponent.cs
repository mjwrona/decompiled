// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.GroupComponent
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class GroupComponent : ReadGroupMembershipsComponentBase
  {
    private const string s_area = "GroupComponent";
    private const int c_readGroupsCountTracingThreshold = 200;
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[25]
    {
      (IComponentCreator) new ComponentCreator<GroupComponent>(1),
      (IComponentCreator) new ComponentCreator<GroupComponent2>(2),
      (IComponentCreator) new ComponentCreator<GroupComponent3>(3),
      (IComponentCreator) new ComponentCreator<GroupComponent4>(4),
      (IComponentCreator) new ComponentCreator<GroupComponent5>(5),
      (IComponentCreator) new ComponentCreator<GroupComponent6>(6),
      (IComponentCreator) new ComponentCreator<GroupComponent7>(7),
      (IComponentCreator) new ComponentCreator<GroupComponent8>(8),
      (IComponentCreator) new ComponentCreator<GroupComponent9>(9),
      (IComponentCreator) new ComponentCreator<GroupComponent10>(10),
      (IComponentCreator) new ComponentCreator<GroupComponent11>(11),
      (IComponentCreator) new ComponentCreator<GroupComponent12>(12),
      (IComponentCreator) new ComponentCreator<GroupComponent13>(13),
      (IComponentCreator) new ComponentCreator<GroupComponent14>(14),
      (IComponentCreator) new ComponentCreator<GroupComponent15>(15),
      (IComponentCreator) new ComponentCreator<GroupComponent16>(16),
      (IComponentCreator) new ComponentCreator<GroupComponent17>(17),
      (IComponentCreator) new ComponentCreator<GroupComponent18>(18),
      (IComponentCreator) new ComponentCreator<GroupComponent19>(19),
      (IComponentCreator) new ComponentCreator<GroupComponent20>(20),
      (IComponentCreator) new ComponentCreator<GroupComponent21>(21),
      (IComponentCreator) new ComponentCreator<GroupComponent22>(22),
      (IComponentCreator) new ComponentCreator<GroupComponent23>(23),
      (IComponentCreator) new ComponentCreator<GroupComponent23>(24),
      (IComponentCreator) new ComponentCreator<GroupComponent23>(25)
    }, "Group");
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();

    static GroupComponent()
    {
      GroupComponent.s_sqlExceptionFactories.Add(400032, new SqlExceptionFactory(typeof (AddProjectGroupProjectMismatchException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new AddProjectGroupProjectMismatchException(sqEr.ExtractString("group_name"), sqEr.ExtractString("member_name")))));
      GroupComponent.s_sqlExceptionFactories.Add(400001, new SqlExceptionFactory(typeof (GroupCreationException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new GroupCreationException(sqEr.ExtractString("display_name"), DBPath.DatabaseToUserPath(sqEr.ExtractString("project_name"))))));
      GroupComponent.s_sqlExceptionFactories.Add(400043, new SqlExceptionFactory(typeof (GroupNameNotRecognizedException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new GroupNameNotRecognizedException(DBPath.DatabaseToUserPath(sqEr.ExtractString("group_name"), true)))));
      GroupComponent.s_sqlExceptionFactories.Add(400025, new SqlExceptionFactory(typeof (GroupScopeDoesNotExistException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) =>
      {
        return (Exception) new GroupScopeDoesNotExistException(sqEr.ExtractString("project_uri"))
        {
          Data = {
            [(object) "{421AC3F1-A306-4C9B-B3F6-5812F9121FC8}"] = (object) true
          }
        };
      })));
      GroupComponent.s_sqlExceptionFactories.Add(400007, new SqlExceptionFactory(typeof (FindGroupSidDoesNotExistException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new FindGroupSidDoesNotExistException(sqEr.ExtractString("sid")))));
      GroupComponent.s_sqlExceptionFactories.Add(400016, new SqlExceptionFactory(typeof (GroupScopeCreationException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new GroupScopeCreationException(sqEr.ExtractString("scope_id")))));
      GroupComponent.s_sqlExceptionFactories.Add(400004, new SqlExceptionFactory(typeof (AddMemberCyclicMembershipException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new AddMemberCyclicMembershipException(sqEr.ExtractString("group_name"), sqEr.ExtractString("member_name")))));
      GroupComponent.s_sqlExceptionFactories.Add(400022, new SqlExceptionFactory(typeof (RemoveGroupMemberNotMemberException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new RemoveGroupMemberNotMemberException(sqEr.ExtractString("sid")))));
      GroupComponent.s_sqlExceptionFactories.Add(400005, new SqlExceptionFactory(typeof (AddMemberIdentityAlreadyMemberException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new AddMemberIdentityAlreadyMemberException(sqEr.ExtractString("group_name"), sqEr.ExtractString("member_name")))));
      GroupComponent.s_sqlExceptionFactories.Add(400026, new SqlExceptionFactory(typeof (GroupRenameException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new GroupRenameException(sqEr.ExtractString("display_name")))));
      GroupComponent.s_sqlExceptionFactories.Add(400012, new SqlExceptionFactory(typeof (RemoveSpecialGroupException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new RemoveSpecialGroupException(sqEr.ExtractString("sid"), (SpecialGroupType) sqEr.ExtractInt("special_type")))));
      GroupComponent.s_sqlExceptionFactories.Add(400031, new SqlExceptionFactory(typeof (AddProjectGroupToGlobalGroupException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new AddProjectGroupToGlobalGroupException(sqEr.ExtractString("group_name"), sqEr.ExtractString("member_name")))));
      GroupComponent.s_sqlExceptionFactories.Add(400105, new SqlExceptionFactory(typeof (ReadGroupMembershipsComponentBase.MinGroupSequenceIdException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new ReadGroupMembershipsComponentBase.MinGroupSequenceIdException(sqEr.ExtractString("current_groupSequenceId"), sqEr.ExtractString("min_groupSequenceId")))));
      GroupComponent.s_sqlExceptionFactories.Add(400202, new SqlExceptionFactory(typeof (RestoreGroupScopeValidationException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new RestoreGroupScopeValidationException(sqEr.ExtractString("validation_error")))));
      GroupComponent.s_sqlExceptionFactories.Add(400046, new SqlExceptionFactory(typeof (ReadGroupMembershipsComponentBase.UpdateGroupScopeVisibilityException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new ReadGroupMembershipsComponentBase.UpdateGroupScopeVisibilityException(sqEr.ExtractString("scopeId"), sqEr.ExtractString("updatedId")))));
      GroupComponent.s_sqlExceptionFactories.Add(400203, new SqlExceptionFactory(typeof (ReadGroupMembershipsComponentBase.RestoreGroupScopeExecutionException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new ReadGroupMembershipsComponentBase.RestoreGroupScopeExecutionException(sqEr.ExtractString("scopeId"), sqEr.ExtractString("failure"), sqEr.ExtractString("status")))));
    }

    public GroupComponent() => this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) GroupComponent.s_sqlExceptionFactories;

    protected virtual SqlParameter BindVersionedGroupTable(
      string parameterName,
      IEnumerable<GroupDescription> descriptions)
    {
      return this.BindGroupTable(parameterName, descriptions);
    }

    protected virtual GroupComponent.GroupIdentitiesColumns GetGroupIdentitiesColumns() => new GroupComponent.GroupIdentitiesColumns();

    protected virtual GroupComponent.GroupIdentityColumns GetGroupIdentityColumns() => new GroupComponent.GroupIdentityColumns();

    protected override string TraceArea => nameof (GroupComponent);

    public virtual long CreateScope(
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
        return (long) this.ReadIntSequenceIdResult(this.ExecuteReader());
      }
      finally
      {
        this.TraceLeave(4703009, nameof (CreateScope));
      }
    }

    public ResultCollection QueryScopes(Guid scopeId, string scopeName)
    {
      try
      {
        this.TraceEnter(4703010, nameof (QueryScopes));
        this.PrepareStoredProcedure("prc_QueryGroupScopes");
        this.BindGuid("@scopeId", scopeId);
        if (scopeName != null)
          scopeName = DBPath.UserToDatabasePath(scopeName);
        this.BindString("@scopeName", scopeName, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<IdentityScope>((ObjectBinder<IdentityScope>) new GroupComponent.IdentityScopeColumns());
        return resultCollection;
      }
      finally
      {
        this.TraceLeave(4703019, nameof (QueryScopes));
      }
    }

    public virtual long UpdateScopes(
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
        return (long) this.ReadIntSequenceIdResult(this.ExecuteReader());
      }
      finally
      {
        this.TraceLeave(4703029, nameof (UpdateScopes));
      }
    }

    public virtual long CreateGroups(
      Guid scopeId,
      bool errorOnDuplicate,
      GroupDescription[] groups,
      bool addActiveScopeMembership = true)
    {
      try
      {
        this.TraceEnter(4703030, nameof (CreateGroups));
        this.PrepareForAuditingAction("Group.CreateGroups");
        this.PrepareStoredProcedure("prc_CreateGroups");
        this.BindGuid("@scopeId", scopeId);
        this.BindBoolean("@errorOnDuplicate", errorOnDuplicate);
        this.BindVersionedGroupTable("@groups", (IEnumerable<GroupDescription>) groups);
        this.BindGuid("@eventAuthor", this.Author);
        return (long) this.ReadIntSequenceIdResult(this.ExecuteReader());
      }
      finally
      {
        this.TraceLeave(4703039, nameof (CreateGroups));
      }
    }

    public virtual ResultCollection QueryGroups(
      IEnumerable<Guid> scopeIds,
      bool recurse,
      bool deleted)
    {
      try
      {
        this.TraceEnter(4703040, nameof (QueryGroups));
        if (deleted)
          scopeIds = (IEnumerable<Guid>) null;
        this.PrepareStoredProcedure("prc_QueryGroups");
        this.BindGuidTable("@scopeIds", scopeIds);
        this.BindBoolean("@recurse", recurse);
        ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<Microsoft.VisualStudio.Services.Identity.Identity>((ObjectBinder<Microsoft.VisualStudio.Services.Identity.Identity>) this.GetGroupIdentityColumns());
        return resultCollection;
      }
      finally
      {
        this.TraceLeave(4703049, nameof (QueryGroups));
      }
    }

    public virtual long UpdateGroups(
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
        return (long) this.ReadIntSequenceIdResult(this.ExecuteReader());
      }
      finally
      {
        this.TraceLeave(4703059, nameof (UpdateGroups));
      }
    }

    public virtual long UpdateGroupMembership(
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
        return (long) this.ReadIntSequenceIdResult(this.ExecuteReader());
      }
      finally
      {
        this.TraceLeave(4703069, nameof (UpdateGroupMembership));
      }
    }

    public virtual long UpdateIdentityMembership(
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
        return (long) this.ReadIntSequenceIdResult(this.ExecuteReader());
      }
      finally
      {
        this.TraceLeave(4703079, nameof (UpdateIdentityMembership));
      }
    }

    public virtual long DeleteGroupMemberships(
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
        return (long) this.ReadIntSequenceIdResult(this.ExecuteReader());
      }
      finally
      {
        this.TraceLeave(4703089, nameof (DeleteGroupMemberships));
      }
    }

    public virtual ResultCollection ReadGroups(
      Guid scopeId,
      IEnumerable<string> groupSids,
      IEnumerable<Guid> groupIds,
      bool useXtpProc)
    {
      try
      {
        this.TraceEnter(4703090, nameof (ReadGroups));
        int groupIdsCount = groupIds != null ? groupIds.Count<Guid>() : 0;
        int groupSidsCount = groupSids != null ? groupSids.Count<string>() : 0;
        if (groupSidsCount + groupIdsCount > 200)
        {
          this.TraceConditionally(4703092, TraceLevel.Info, (Func<string>) (() => string.Format("ScopeId: {0} GroupSids count: {1} GroupIds count: {2}", (object) scopeId, (object) groupSidsCount, (object) groupIdsCount)));
          this.TraceConditionally(4703093, TraceLevel.Info, (Func<string>) (() => "StackTrace: " + Environment.StackTrace));
        }
        if (useXtpProc)
        {
          this.PrepareStoredProcedure("prc_ReadGroups_xtp");
          this.BindGuid("@scopeId", scopeId);
          this.BindMemoOrderedStringTable("@groupSids", groupSids, false, true);
          this.BindMemoOrderedGuidTable("@groupIds", groupIds, true);
        }
        else
        {
          this.PrepareStoredProcedure("prc_ReadGroups");
          this.BindGuid("@scopeId", scopeId);
          this.BindOrderedStringTable("@groupSids", groupSids, false, true);
          this.BindOrderedGuidTable("@groupIds", groupIds, true);
        }
        ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<GroupComponent.GroupIdentityData>((ObjectBinder<GroupComponent.GroupIdentityData>) this.GetGroupIdentitiesColumns());
        return resultCollection;
      }
      finally
      {
        this.TraceLeave(4703099, nameof (ReadGroups));
      }
    }

    public virtual ResultCollection ReadGroupBySid(Guid scopeId, string groupSid) => this.ReadGroups(scopeId, (IEnumerable<string>) new string[1]
    {
      groupSid
    }, Enumerable.Empty<Guid>(), false);

    public virtual ResultCollection ReadGroupById(Guid scopeId, Guid groupId) => this.ReadGroups(scopeId, Enumerable.Empty<string>(), (IEnumerable<Guid>) new Guid[1]
    {
      groupId
    }, false);

    public ResultCollection ReadGroup(
      Guid scopeId,
      string scopeName,
      string groupName,
      bool recurse)
    {
      try
      {
        this.TraceEnter(4703100, nameof (ReadGroup));
        this.PrepareStoredProcedure("prc_ReadGroup");
        this.BindGuid("@scopeId", scopeId);
        this.BindString("@scopeName", scopeName != null ? DBPath.UserToDatabasePath(scopeName) : (string) null, 256, true, SqlDbType.NVarChar);
        this.BindString("@groupName", groupName, 256, false, SqlDbType.NVarChar);
        this.BindBoolean("@recurse", recurse);
        ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<Microsoft.VisualStudio.Services.Identity.Identity>((ObjectBinder<Microsoft.VisualStudio.Services.Identity.Identity>) this.GetGroupIdentityColumns());
        return resultCollection;
      }
      finally
      {
        this.TraceLeave(4703109, nameof (ReadGroup));
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
        if (this.RequestContext.ExecutionEnvironment.IsHostedDeployment)
          this.PrepareStoredProcedure("prc_ReadGroupMembership2");
        else
          this.PrepareStoredProcedure("prc_ReadGroupMembership");
        this.BindGuidTable("@identityIds", identityIds);
        this.BindInt("@childrenQuery", (int) childrenQuery);
        this.BindInt("@parentsQuery", (int) parentsQuery);
        this.BindBoolean("@includeRestricted", includeRestricted);
        this.BindNullableInt("@minInactivatedTime", minInactivatedTime.GetValueOrDefault(), 0);
        this.BindGuid("@scopeId", scopeId);
        this.BindBoolean("@returnVisibleIdentities", returnVisibleIdentities);
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

    public virtual ResultCollection GetChanges(
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
        this.TraceEnter(4703120, nameof (GetChanges));
        this.PrepareStoredProcedure("prc_GetGroupChanges");
        this.BindInt("@sequenceId", GroupComponent.ReduceMaxLongToMaxInt(sequenceId));
        this.BindString("@everyoneSid", everyoneSid, 256, BindStringBehavior.Unchanged, SqlDbType.VarChar);
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
        changes.AddBinder<MembershipChangeInfo>((ObjectBinder<MembershipChangeInfo>) new GroupComponent.GroupChangesColumns());
        changes.AddBinder<Guid>((ObjectBinder<Guid>) new GroupComponent.IdentityIdColumns());
        return changes;
      }
      finally
      {
        this.TraceLeave(4703129, nameof (GetChanges));
      }
    }

    public virtual long GetLatestGroupSequenceId(string everyoneSid, Guid scopeId)
    {
      long lastSequenceId;
      using (this.GetChanges(long.MaxValue, everyoneSid, scopeId, out Guid _, out lastSequenceId))
        return lastSequenceId;
    }

    public virtual long GetLatestGroupSequenceId() => throw new NotImplementedException();

    public virtual ResultCollection ReadSnapshot(
      Guid scopeId,
      bool readAllIdentities,
      bool includeInactiveMemberships = false)
    {
      try
      {
        this.TraceEnter(4703130, nameof (ReadSnapshot));
        this.PrepareStoredProcedure("prc_ReadGroupSnapshot");
        this.BindGuid("@scopeId", scopeId);
        this.BindBoolean("@readAllIdentities", readAllIdentities);
        ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<IdentityScope>((ObjectBinder<IdentityScope>) new GroupComponent.IdentityScopeColumns2());
        resultCollection.AddBinder<Microsoft.VisualStudio.Services.Identity.Identity>((ObjectBinder<Microsoft.VisualStudio.Services.Identity.Identity>) this.GetGroupIdentityColumns());
        resultCollection.AddBinder<GroupMembership>((ObjectBinder<GroupMembership>) new ReadGroupMembershipsComponentBase.ChildrenColumns());
        resultCollection.AddBinder<Guid>((ObjectBinder<Guid>) new GroupComponent.IdentityIdColumns());
        return resultCollection;
      }
      finally
      {
        this.TraceLeave(4703139, nameof (ReadSnapshot));
      }
    }

    public virtual void SaveSnapshot(
      Guid parentScopeId,
      IEnumerable<IdentityScope> scopes,
      IEnumerable<GroupDescription> groups,
      IEnumerable<GroupMembership> memberships)
    {
      try
      {
        this.TraceEnter(4703140, nameof (SaveSnapshot));
        this.PrepareStoredProcedure("prc_SaveGroupSnapshot", SqlCommandTimeout.Max(this.CommandTimeout, 3600));
        this.BindNullableGuid("@parentScopeId", parentScopeId);
        this.BindGroupScopeTable2("@scopes", scopes);
        this.BindVersionedGroupTable("@groups", groups);
        this.BindGroupMembershipTable("@memberships", memberships.Select<GroupMembership, Tuple<IdentityDescriptor, Guid, bool>>((System.Func<GroupMembership, Tuple<IdentityDescriptor, Guid, bool>>) (membership => new Tuple<IdentityDescriptor, Guid, bool>(membership.Descriptor, membership.Id, membership.Active))));
        this.ExecuteNonQuery();
      }
      finally
      {
        this.TraceLeave(4703149, nameof (SaveSnapshot));
      }
    }

    public virtual void ReplaceSnapshot(
      Guid parentScopeId,
      IEnumerable<IdentityScope> scopes,
      IEnumerable<GroupDescription> groups,
      IEnumerable<GroupMembership> memberships)
    {
      throw new NotImplementedException();
    }

    public virtual long ChangeGroupsIds(IEnumerable<KeyValuePair<Guid, Guid>> groupIdMap) => throw new NotImplementedException();

    public virtual IList<GroupMembershipCycleRecord> GetCyclicGroupMemberships(Guid scopeId) => (IList<GroupMembershipCycleRecord>) new List<GroupMembershipCycleRecord>();

    internal virtual Guid[] ReadMasterScope(Guid securingHostId, Guid[] localIds)
    {
      try
      {
        this.TraceEnter(4703150, nameof (ReadMasterScope));
        Guid[] guidArray = new Guid[localIds.Length];
        this.PrepareStoredProcedure("prc_ReadMasterScope");
        this.BindGuid("@securingHostId", securingHostId);
        this.BindOrderedGuidTable("@localIds", (IEnumerable<Guid>) localIds, true);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<KeyValuePair<int, Guid>>((ObjectBinder<KeyValuePair<int, Guid>>) new GroupComponent.ScopeMapBinder());
          ObjectBinder<KeyValuePair<int, Guid>> current1 = resultCollection.GetCurrent<KeyValuePair<int, Guid>>();
          while (current1.MoveNext())
          {
            KeyValuePair<int, Guid> current2 = current1.Current;
            guidArray[current2.Key] = current2.Value;
          }
        }
        return guidArray;
      }
      finally
      {
        this.TraceLeave(4703159, nameof (ReadMasterScope));
      }
    }

    public ResultCollection GetGroupsToSync(DateTime? lastSync)
    {
      try
      {
        this.TraceEnter(4703160, nameof (GetGroupsToSync));
        this.PrepareStoredProcedure("prc_GetGroupsToSync");
        this.BindNullableDateTime("@lastSync", lastSync);
        ResultCollection groupsToSync = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        if (lastSync.HasValue)
          groupsToSync.AddBinder<int>((ObjectBinder<int>) new GroupComponent.TotalIdentitiesColumns());
        groupsToSync.AddBinder<IdentityDescriptor>((ObjectBinder<IdentityDescriptor>) new GroupComponent.IdentityDescriptorColumns());
        return groupsToSync;
      }
      finally
      {
        this.TraceLeave(4703169, nameof (GetGroupsToSync));
      }
    }

    public virtual long TransferGroupMembership(IEnumerable<KeyValuePair<Guid, Guid>> updates) => throw new NotImplementedException();

    public virtual int InitGroupScopeVisibility() => throw new NotImplementedException();

    public virtual IReadOnlyList<GroupAuditRecord> GetGroupAuditRecords(
      long startSequenceIdInclusive,
      long endSequenceIdInclusive)
    {
      return (IReadOnlyList<GroupAuditRecord>) Array.Empty<GroupAuditRecord>();
    }

    public virtual void RetrieveFirstAuditSequenceIds(out long firstIdentityAuditSequenceId) => firstIdentityAuditSequenceId = 0L;

    protected static int ReduceMaxLongToMaxInt(long sequenceId)
    {
      if (long.MaxValue == sequenceId)
        sequenceId = (long) int.MaxValue;
      return checked ((int) sequenceId);
    }

    protected int ReadIntSequenceIdResult(SqlDataReader reader) => reader.Read() ? new SqlColumnBinder("SequenceId").GetInt32((IDataReader) reader) : throw new UnexpectedDatabaseResultException(this.ProcedureName);

    public virtual IList<Guid> GetAncestorScopeIds(Guid scopeId, bool includeInactiveScopes = false) => throw new NotImplementedException();

    public virtual IList<Guid> GetIdentityIdsVisibleInScope(Guid scopeId) => throw new NotImplementedException();

    internal virtual IList<Guid> GetIdentityIdsInScope(Guid scopeId) => throw new NotImplementedException();

    public virtual IList<Guid> GetIdentityIdsVisibleInScopeByPage(
      Guid scopeId,
      Guid? pageIndex,
      int pageSize,
      bool includeGroups,
      bool includeNonGroups)
    {
      throw new NotImplementedException();
    }

    public virtual long RestoreGroupScope(Guid scopeId, RestoreProjectOptions restoreOptions) => throw new NotImplementedException();

    public virtual ResultCollection ReadAadGroups(bool readInactive = true) => throw new NotImplementedException();

    protected class IdentityIdColumns : ObjectBinder<Guid>
    {
      private SqlColumnBinder Id = new SqlColumnBinder(nameof (Id));

      protected override Guid Bind() => this.Id.GetGuid((IDataReader) this.Reader);
    }

    protected class ScopeIdColumns : ObjectBinder<Guid>
    {
      private SqlColumnBinder _scopeId = new SqlColumnBinder("ScopeId");

      protected override Guid Bind() => this._scopeId.GetGuid((IDataReader) this.Reader, true);
    }

    private class ScopeMapBinder : ObjectBinder<KeyValuePair<int, Guid>>
    {
      private SqlColumnBinder m_orderId = new SqlColumnBinder("OrderId");
      private SqlColumnBinder m_scopeId = new SqlColumnBinder("ScopeId");

      protected override KeyValuePair<int, Guid> Bind() => new KeyValuePair<int, Guid>(this.m_orderId.GetInt32((IDataReader) this.Reader), this.m_scopeId.GetGuid((IDataReader) this.Reader));
    }

    protected class GroupChangesHeaderColumns
    {
      public SqlColumnBinder SequenceId = new SqlColumnBinder("LastSequenceId");
      public SqlColumnBinder Id = new SqlColumnBinder(nameof (Id));
    }

    protected class GroupChangesColumns : ObjectBinder<MembershipChangeInfo>
    {
      protected SqlColumnBinder ContainerId = new SqlColumnBinder(nameof (ContainerId));
      protected SqlColumnBinder MemberId = new SqlColumnBinder(nameof (MemberId));
      protected SqlColumnBinder Active = new SqlColumnBinder(nameof (Active));
      protected SqlColumnBinder IsMemberGroup = new SqlColumnBinder(nameof (IsMemberGroup));
      protected SqlColumnBinder MemberSid = new SqlColumnBinder(nameof (MemberSid));
      protected SqlColumnBinder MemberType = new SqlColumnBinder(nameof (MemberType));

      protected override MembershipChangeInfo Bind()
      {
        MembershipChangeInfo membershipChangeInfo = new MembershipChangeInfo();
        membershipChangeInfo.ContainerId = this.ContainerId.GetGuid((IDataReader) this.Reader);
        membershipChangeInfo.MemberId = this.MemberId.GetGuid((IDataReader) this.Reader);
        membershipChangeInfo.Active = this.Active.GetBoolean((IDataReader) this.Reader);
        membershipChangeInfo.IsMemberGroup = this.IsMemberGroup.GetBoolean((IDataReader) this.Reader);
        string identifier = this.MemberSid.GetString((IDataReader) this.Reader, true);
        if (identifier != null)
        {
          string identityType = this.MemberType.GetString((IDataReader) this.Reader, false);
          membershipChangeInfo.MemberDescriptor = new IdentityDescriptor(identityType, identifier);
        }
        return membershipChangeInfo;
      }
    }

    protected class GroupChangesColumns2 : GroupComponent.GroupChangesColumns
    {
      protected SqlColumnBinder ContainerSid = new SqlColumnBinder(nameof (ContainerSid));
      protected SqlColumnBinder ContainerType = new SqlColumnBinder(nameof (ContainerType));

      protected override MembershipChangeInfo Bind()
      {
        MembershipChangeInfo membershipChangeInfo = base.Bind();
        string identifier = this.ContainerSid.GetString((IDataReader) this.Reader, true);
        if (identifier != null)
        {
          string identityType = this.ContainerType.GetString((IDataReader) this.Reader, false);
          membershipChangeInfo.ContainerDescriptor = new IdentityDescriptor(identityType, identifier);
        }
        return membershipChangeInfo;
      }
    }

    protected class GroupChangesColumns3 : GroupComponent.GroupChangesColumns2
    {
      protected SqlColumnBinder ContainerScopeId = new SqlColumnBinder(nameof (ContainerScopeId));

      protected override MembershipChangeInfo Bind()
      {
        MembershipChangeInfo membershipChangeInfo = base.Bind();
        membershipChangeInfo.ContainerScopeId = this.ContainerScopeId.GetGuid((IDataReader) this.Reader, true);
        return membershipChangeInfo;
      }
    }

    protected class GroupMembershipCycleRecordColumns : ObjectBinder<GroupMembershipCycleRecord>
    {
      private SqlColumnBinder MemberId = new SqlColumnBinder(nameof (MemberId));
      private SqlColumnBinder MemberDisplayName = new SqlColumnBinder(nameof (MemberDisplayName));
      private SqlColumnBinder ContainerId = new SqlColumnBinder(nameof (ContainerId));
      private SqlColumnBinder ContainerDisplayName = new SqlColumnBinder(nameof (ContainerDisplayName));
      private SqlColumnBinder Level = new SqlColumnBinder(nameof (Level));

      protected override GroupMembershipCycleRecord Bind() => new GroupMembershipCycleRecord()
      {
        MemberId = this.MemberId.GetGuid((IDataReader) this.Reader),
        MemberDisplayName = this.MemberDisplayName.GetString((IDataReader) this.Reader, true),
        ContainerId = this.ContainerId.GetGuid((IDataReader) this.Reader),
        ContainerDisplayName = this.ContainerDisplayName.GetString((IDataReader) this.Reader, true),
        Level = this.Level.GetInt32((IDataReader) this.Reader)
      };
    }

    protected class ChildrenColumns2 : ReadGroupMembershipsComponentBase.ChildrenColumns
    {
      private SqlColumnBinder Active = new SqlColumnBinder(nameof (Active));

      protected override GroupMembership Bind()
      {
        GroupMembership groupMembership = base.Bind();
        groupMembership.Active = this.Active.GetBoolean((IDataReader) this.Reader, true);
        return groupMembership;
      }
    }

    protected class GroupIdentityColumns : ObjectBinder<Microsoft.VisualStudio.Services.Identity.Identity>
    {
      private SqlColumnBinder ScopeId = new SqlColumnBinder(nameof (ScopeId));
      private SqlColumnBinder LocalScopeId = new SqlColumnBinder(nameof (LocalScopeId));
      private SqlColumnBinder ScopeName = new SqlColumnBinder(nameof (ScopeName));
      private SqlColumnBinder Sid = new SqlColumnBinder(nameof (Sid));
      private SqlColumnBinder Id = new SqlColumnBinder(nameof (Id));
      private SqlColumnBinder SpecialType = new SqlColumnBinder(nameof (SpecialType));
      private SqlColumnBinder DisplayName = new SqlColumnBinder(nameof (DisplayName));
      private SqlColumnBinder Description = new SqlColumnBinder(nameof (Description));
      private SqlColumnBinder RestrictedView = new SqlColumnBinder(nameof (RestrictedView));
      private SqlColumnBinder ScopeLocal = new SqlColumnBinder(nameof (ScopeLocal));
      private SqlColumnBinder Active = new SqlColumnBinder(nameof (Active));
      private SqlColumnBinder SecuringHostId = new SqlColumnBinder(nameof (SecuringHostId));
      private SqlColumnBinder ScopeType = new SqlColumnBinder(nameof (ScopeType));

      protected override Microsoft.VisualStudio.Services.Identity.Identity Bind() => this.Bind(this.Reader);

      protected internal virtual Microsoft.VisualStudio.Services.Identity.Identity Bind(
        SqlDataReader reader)
      {
        IdentityDescriptor identityDescriptor = new IdentityDescriptor("Microsoft.TeamFoundation.Identity", this.Sid.GetString((IDataReader) reader, false));
        Guid guid1 = this.ScopeId.GetGuid((IDataReader) reader);
        Guid guid2 = this.LocalScopeId.GetGuid((IDataReader) reader);
        string userPath = DBPath.DatabaseToUserPath(this.ScopeName.GetString((IDataReader) reader, false));
        string str1 = ((SpecialGroupType) this.SpecialType.GetInt32((IDataReader) reader)).ToString();
        string str2 = this.DisplayName.GetString((IDataReader) reader, false);
        string str3;
        if (!str2.StartsWith("[", StringComparison.OrdinalIgnoreCase))
          str3 = "[" + userPath + "]" + "\\" + str2;
        else
          str3 = str2;
        string str4 = str3;
        string str5 = this.Description.GetString((IDataReader) reader, false);
        Guid guid3 = this.Id.GetGuid((IDataReader) reader, false);
        int num = this.RestrictedView.GetBoolean((IDataReader) reader, false) ? 1 : 0;
        bool boolean1 = this.ScopeLocal.GetBoolean((IDataReader) reader, false);
        bool boolean2 = this.Active.GetBoolean((IDataReader) reader, false);
        Guid guid4 = this.SecuringHostId.GetGuid((IDataReader) reader, false);
        GroupScopeType scopeType = (GroupScopeType) this.ScopeType.GetByte((IDataReader) reader);
        string str6 = scopeType.ToString();
        string scopeUri = IdentityHelper.GetScopeUri(guid2, scopeType);
        Microsoft.VisualStudio.Services.Identity.Identity identity1 = new Microsoft.VisualStudio.Services.Identity.Identity();
        identity1.Id = guid3;
        identity1.Descriptor = identityDescriptor;
        identity1.ProviderDisplayName = str4;
        identity1.CustomDisplayName = (string) null;
        identity1.IsActive = boolean2;
        identity1.UniqueUserId = 0;
        identity1.IsContainer = true;
        identity1.Members = (ICollection<IdentityDescriptor>) new List<IdentityDescriptor>();
        identity1.MemberOf = (ICollection<IdentityDescriptor>) new List<IdentityDescriptor>();
        identity1.MemberIds = (ICollection<Guid>) new List<Guid>();
        identity1.MemberOfIds = (ICollection<Guid>) new List<Guid>();
        identity1.MasterId = guid3;
        Microsoft.VisualStudio.Services.Identity.Identity identity2 = identity1;
        identity2.SetProperty("SchemaClassName", (object) "Group");
        identity2.SetProperty("Description", (object) str5);
        identity2.SetProperty("Domain", (object) scopeUri);
        identity2.SetProperty("Account", (object) str2);
        identity2.SetProperty("SecurityGroup", (object) "SecurityGroup");
        identity2.SetProperty("SpecialType", (object) str1);
        identity2.SetProperty("ScopeId", (object) guid1);
        identity2.SetProperty("ScopeType", (object) str6);
        identity2.SetProperty("LocalScopeId", (object) guid2);
        identity2.SetProperty("SecuringHostId", (object) guid4);
        if (!string.IsNullOrEmpty(userPath))
          identity2.SetProperty("ScopeName", (object) userPath);
        if (num != 0)
          identity2.SetProperty("RestrictedVisible", (object) "RestrictedVisible");
        if (!boolean1)
          identity2.SetProperty("CrossProject", (object) "CrossProject");
        if (scopeType != GroupScopeType.TeamProject)
          identity2.SetProperty("GlobalScope", (object) "GlobalScope");
        if (!boolean2)
          identity2.SetProperty("IsGroupDeleted", (object) true);
        return identity2;
      }
    }

    protected class GroupIdentityColumns2 : GroupComponent.GroupIdentityColumns
    {
      private SqlColumnBinder VirtualPlugin = new SqlColumnBinder(nameof (VirtualPlugin));

      protected internal override Microsoft.VisualStudio.Services.Identity.Identity Bind(
        SqlDataReader reader)
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity = base.Bind(reader);
        identity.SetProperty("VirtualPlugin", (object) (this.VirtualPlugin.GetString((IDataReader) reader, true) ?? string.Empty));
        return identity;
      }
    }

    protected class GroupIdentitiesColumns : ObjectBinder<GroupComponent.GroupIdentityData>
    {
      private SqlColumnBinder OrderId = new SqlColumnBinder(nameof (OrderId));
      private readonly GroupComponent.GroupIdentityColumns Columns;

      public GroupIdentitiesColumns() => this.Columns = this.GetColumns();

      protected override GroupComponent.GroupIdentityData Bind() => new GroupComponent.GroupIdentityData()
      {
        Identity = this.Columns.Bind(this.Reader),
        OrderId = this.OrderId.GetInt32((IDataReader) this.Reader)
      };

      protected virtual GroupComponent.GroupIdentityColumns GetColumns() => new GroupComponent.GroupIdentityColumns();
    }

    protected class GroupIdentitiesColumns2 : GroupComponent.GroupIdentitiesColumns
    {
      protected override GroupComponent.GroupIdentityColumns GetColumns() => (GroupComponent.GroupIdentityColumns) new GroupComponent.GroupIdentityColumns2();
    }

    protected class IdentityScopeColumns : ObjectBinder<IdentityScope>
    {
      private SqlColumnBinder Id = new SqlColumnBinder("ScopeId");
      private SqlColumnBinder ParentScopeId = new SqlColumnBinder(nameof (ParentScopeId));
      private SqlColumnBinder Name = new SqlColumnBinder(nameof (Name));
      private SqlColumnBinder AdminSid = new SqlColumnBinder(nameof (AdminSid));
      private SqlColumnBinder SecuringHostId = new SqlColumnBinder(nameof (SecuringHostId));
      private SqlColumnBinder ScopeType = new SqlColumnBinder(nameof (ScopeType));
      private SqlColumnBinder LocalScopeId = new SqlColumnBinder(nameof (LocalScopeId));

      protected override IdentityScope Bind()
      {
        GroupScopeType groupScopeType = (GroupScopeType) this.ScopeType.GetByte((IDataReader) this.Reader);
        return new IdentityScope()
        {
          Id = this.Id.GetGuid((IDataReader) this.Reader),
          LocalScopeId = this.LocalScopeId.GetGuid((IDataReader) this.Reader),
          ParentId = this.ParentScopeId.GetGuid((IDataReader) this.Reader, true),
          ScopeType = groupScopeType,
          Name = DBPath.DatabaseToUserPath(this.Name.GetString((IDataReader) this.Reader, false)),
          Administrators = IdentityHelper.CreateTeamFoundationDescriptor(this.AdminSid.GetString((IDataReader) this.Reader, false)),
          IsGlobal = groupScopeType == GroupScopeType.ServiceHost,
          SecuringHostId = this.SecuringHostId.GetGuid((IDataReader) this.Reader)
        };
      }
    }

    protected class IdentityScopeColumns2 : GroupComponent.IdentityScopeColumns
    {
      private SqlColumnBinder Active = new SqlColumnBinder(nameof (Active));

      protected override IdentityScope Bind()
      {
        IdentityScope identityScope = base.Bind();
        identityScope.IsActive = this.Active.GetBoolean((IDataReader) this.Reader);
        return identityScope;
      }
    }

    protected class IdentityDescriptorColumns : ObjectBinder<IdentityDescriptor>
    {
      private SqlColumnBinder Sid = new SqlColumnBinder(nameof (Sid));
      private SqlColumnBinder Type = new SqlColumnBinder(nameof (Type));

      protected override IdentityDescriptor Bind() => new IdentityDescriptor(this.Type.GetString((IDataReader) this.Reader, false), this.Sid.GetString((IDataReader) this.Reader, false));
    }

    protected class TotalIdentitiesColumns : ObjectBinder<int>
    {
      private SqlColumnBinder TotalIdentities = new SqlColumnBinder(nameof (TotalIdentities));

      protected override int Bind() => this.TotalIdentities.GetInt32((IDataReader) this.Reader);
    }

    internal class GroupUpdate
    {
      public string GroupSid { get; set; }

      public string Name { get; set; }

      public string Description { get; set; }
    }

    internal class GroupIdentityData
    {
      public Microsoft.VisualStudio.Services.Identity.Identity Identity { get; set; }

      public int OrderId { get; set; }
    }

    internal struct ReferencedIdentity
    {
      public Guid IdentityId;
      public GroupComponent.ReferencedIdentityLocation Location;
    }

    internal enum ReferencedIdentityLocation : byte
    {
      Unknown,
      Local,
      Remote,
    }
  }
}
