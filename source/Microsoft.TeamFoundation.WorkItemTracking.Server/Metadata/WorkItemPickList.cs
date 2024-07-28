// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemPickList
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  public class WorkItemPickList : WorkItemPickListMetadata
  {
    public IReadOnlyList<WorkItemPickListMember> Items { get; private set; }

    public static WorkItemPickList Create(WorkItemPickListRecord picklist)
    {
      WorkItemPickList workItemPickList = new WorkItemPickList();
      workItemPickList.Id = picklist.Id;
      workItemPickList.Name = picklist.Name;
      workItemPickList.ConstId = picklist.ConstId;
      workItemPickList.Type = (WorkItemPickListType) picklist.Type;
      workItemPickList.m_isSuggested = picklist.IsSuggested;
      WorkItemPickList list = workItemPickList;
      list.Items = picklist.Items == null ? (IReadOnlyList<WorkItemPickListMember>) new List<WorkItemPickListMember>() : (IReadOnlyList<WorkItemPickListMember>) picklist.Items.Select<WorkItemPickListMemberRecord, WorkItemPickListMember>((Func<WorkItemPickListMemberRecord, int, WorkItemPickListMember>) ((r, i) => WorkItemPickListMember.Create(r, list))).ToList<WorkItemPickListMember>();
      return list;
    }
  }
}
