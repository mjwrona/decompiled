// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTrackingMetadataComponent42
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  internal class WorkItemTrackingMetadataComponent42 : WorkItemTrackingMetadataComponent41
  {
    protected override WorkItemTrackingMetadataComponent34.NewGroupMemberBinder GetGroupMemberBinder() => new WorkItemTrackingMetadataComponent34.NewGroupMemberBinder();

    public override IEnumerable<GroupMemberEntry> FindGroupMembersOfType(
      IEnumerable<string> groupNames,
      GroupType groupType,
      int maxMemberForEachGroup = 100,
      int maxIteration = 50)
    {
      this.PrepareStoredProcedure("prc_FindGroupMembers");
      this.BindStringTable("@groups", groupNames);
      this.BindInt("@groupType", (int) groupType);
      this.BindInt("@maxMembers", maxMemberForEachGroup);
      this.BindInt("@maxIteration", maxIteration);
      return this.ExecuteUnknown<IEnumerable<GroupMemberEntry>>((System.Func<IDataReader, IEnumerable<GroupMemberEntry>>) (reader => (IEnumerable<GroupMemberEntry>) this.GetGroupMemberBinder().BindAll(reader).ToList<GroupMemberEntry>()));
    }
  }
}
