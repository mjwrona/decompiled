// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTrackingMetadataComponent41
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using System;
using System.Data;
using System.Globalization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  internal class WorkItemTrackingMetadataComponent41 : WorkItemTrackingMetadataComponent40
  {
    protected override WorkItemTrackingMetadataComponent34.NewGroupMemberBinder GetGroupMemberBinder() => (WorkItemTrackingMetadataComponent34.NewGroupMemberBinder) new WorkItemTrackingMetadataComponent41.GroupMemberBinder();

    internal override bool HasAnyWorkItemsOfTypeForProject(Guid projectId, string workItemTypeName)
    {
      this.PrepareStoredProcedure("prc_ProjectHasAnyWorkItemsOfType");
      this.BindString("@witName", workItemTypeName, 256, false, SqlDbType.NVarChar);
      this.BindGuid("@projectId", projectId);
      return Convert.ToBoolean(this.ExecuteScalar(), (IFormatProvider) CultureInfo.InvariantCulture);
    }

    protected class GroupMemberBinder : WorkItemTrackingMetadataComponent34.NewGroupMemberBinder
    {
      private SqlColumnBinder GroupColumn = new SqlColumnBinder("Group");
      private SqlColumnBinder GroupMemberTeamFoundationIdColumn = new SqlColumnBinder("AADGroupMemberTeamFoundationId");
      private SqlColumnBinder GroupMemberIdentityDisplayNameColumn = new SqlColumnBinder("AADGroupMemberIdentityDisplayName");
      private SqlColumnBinder GroupMemberParentTeamFoundationIdColumn = new SqlColumnBinder("AADGroupMemberParentTeamFoundationId");
      private SqlColumnBinder GroupMemberParentIdentityDisplayNameColumn = new SqlColumnBinder("AADGroupMemberParentIdentityDisplayName");

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
