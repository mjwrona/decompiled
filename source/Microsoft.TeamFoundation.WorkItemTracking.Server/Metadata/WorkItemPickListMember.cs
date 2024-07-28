// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemPickListMember
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  public class WorkItemPickListMember
  {
    public Guid Id { get; private set; }

    public string Value { get; private set; }

    public WorkItemPickList List { get; private set; }

    public int ConstId { get; private set; }

    public WorkItemPickListType Type { get; private set; }

    public static WorkItemPickListMember Create(
      WorkItemPickListMemberRecord record,
      WorkItemPickList list)
    {
      return new WorkItemPickListMember()
      {
        Id = record.Id,
        Value = record.Value,
        List = list,
        ConstId = record.ConstId,
        Type = list.Type
      };
    }

    public T GetValueAs<T>() => CommonWITUtils.ConvertValue<T>((object) this.Value);
  }
}
