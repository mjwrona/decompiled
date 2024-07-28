// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTrackingMetadataComponent34
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  internal class WorkItemTrackingMetadataComponent34 : WorkItemTrackingMetadataComponent33
  {
    protected virtual WorkItemTrackingMetadataComponent34.NewGroupMemberBinder GetGroupMemberBinder() => new WorkItemTrackingMetadataComponent34.NewGroupMemberBinder();

    protected override WorkItemTrackingMetadataComponent.RuleRecordBinder GetRuleRecordBinder() => (WorkItemTrackingMetadataComponent.RuleRecordBinder) new WorkItemTrackingMetadataComponent34.RuleRecordBinder2();

    protected override WorkItemTrackingMetadataComponent.ForNotRuleGroupRecordBinder GetForNotRuleGroupRecordBinder() => (WorkItemTrackingMetadataComponent.ForNotRuleGroupRecordBinder) new WorkItemTrackingMetadataComponent34.ForNotRuleGroupRecordBinder2();

    public override IEnumerable<GroupMemberEntry> FindGroupMembersOfType(
      IEnumerable<string> groupNames,
      GroupType groupType,
      int maxMemberForEachGroup = -1,
      int maxIteration = -1)
    {
      if (groupType != GroupType.ActiveDirectory)
        throw new NotSupportedException("This implementation only supports TfsMigrator backwards compatibility");
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.stmt_FindADGroupMembers.sql");
      this.PrepareSqlBatch(resourceAsString.Length, 3600);
      this.AddStatement(resourceAsString);
      this.BindStringTable("@groups", groupNames);
      return this.ExecuteUnknown<IEnumerable<GroupMemberEntry>>((System.Func<IDataReader, IEnumerable<GroupMemberEntry>>) (reader => (IEnumerable<GroupMemberEntry>) this.GetGroupMemberBinder().BindAll(reader).ToList<GroupMemberEntry>()));
    }

    protected class RuleRecordBinder2 : WorkItemTrackingMetadataComponent.RuleRecordBinder
    {
      private SqlColumnBinder PersonVsIdColumn = new SqlColumnBinder("PersonVsId");

      public override RuleRecord Bind(IDataReader reader)
      {
        RuleRecord ruleRecord = base.Bind(reader);
        ruleRecord.PersonVsId = this.PersonVsIdColumn.GetGuid(reader, true);
        return ruleRecord;
      }
    }

    protected class ForNotRuleGroupRecordBinder2 : 
      WorkItemTrackingMetadataComponent.ForNotRuleGroupRecordBinder
    {
      private SqlColumnBinder TeamFoundationIdColumn = new SqlColumnBinder("TeamFoundationId");

      public override ForNotRuleGroupRecord Bind(IDataReader reader) => new ForNotRuleGroupRecord()
      {
        TeamFoundationId = this.TeamFoundationIdColumn.GetGuid(reader, true)
      };
    }

    protected class NewGroupMemberBinder : WorkItemTrackingObjectBinder<GroupMemberEntry>
    {
      private SqlColumnBinder GroupColumn = new SqlColumnBinder("Group");
      private SqlColumnBinder GroupMemberTeamFoundationIdColumn = new SqlColumnBinder("GroupMemberTeamFoundationId");
      private SqlColumnBinder GroupMemberIdentityDisplayNameColumn = new SqlColumnBinder("GroupMemberIdentityDisplayName");
      private SqlColumnBinder GroupMemberParentTeamFoundationIdColumn = new SqlColumnBinder("GroupMemberParentTeamFoundationId");
      private SqlColumnBinder GroupMemberParentIdentityDisplayNameColumn = new SqlColumnBinder("GroupMemberParentIdentityDisplayName");

      public override GroupMemberEntry Bind(IDataReader reader) => new GroupMemberEntry()
      {
        Group = this.GroupColumn.GetString(reader, false),
        MemberTeamFoundationId = this.GroupMemberTeamFoundationIdColumn.GetGuid(reader, false),
        MemberIdentityDisplayName = this.GroupMemberIdentityDisplayNameColumn.GetString(reader, false),
        ParentTeamFoundationId = this.GroupMemberParentTeamFoundationIdColumn.GetGuid(reader, false),
        ParentIdentityDisplayName = this.GroupMemberParentIdentityDisplayNameColumn.GetString(reader, false)
      };
    }
  }
}
