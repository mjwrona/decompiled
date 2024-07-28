// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.RuleSetRecord
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels
{
  public class RuleSetRecord
  {
    public int RuleSetId { get; set; }

    public int ParentId { get; set; }

    public int ConstId { get; set; }

    public int ProjectId { get; set; }

    public bool Deleted { get; set; }
  }
}
