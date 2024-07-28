// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.SetRecord
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels
{
  public class SetRecord
  {
    public int ParentId { get; set; }

    public int ItemId { get; set; }

    public string Item { get; set; }

    public bool IsList { get; set; }

    public bool Direct { get; set; }

    public bool IncludeGroups { get; set; }

    public bool IncludeTop { get; set; }

    public int Generation { get; set; }

    public Guid TeamFoundationId { get; set; }

    public long SetHandle
    {
      get
      {
        long parentId = (long) this.ParentId;
        if (this.Direct)
          parentId |= 4294967296L;
        if (this.IncludeTop)
          parentId |= 8589934592L;
        if (this.IncludeGroups)
          parentId |= 17179869184L;
        return parentId;
      }
    }
  }
}
