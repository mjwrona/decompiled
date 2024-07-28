// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemResourceLinkUpdateResultRecord
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Common;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels
{
  internal struct WorkItemResourceLinkUpdateResultRecord
  {
    public int Order { get; set; }

    public LinkUpdateType UpdateType { get; set; }

    public int Status { get; set; }

    public int SourceId { get; set; }

    public int ResourceType { get; set; }

    public int ResourceId { get; set; }

    public string Location { get; set; }

    public string Name { get; set; }

    public string Comment { get; set; }
  }
}
