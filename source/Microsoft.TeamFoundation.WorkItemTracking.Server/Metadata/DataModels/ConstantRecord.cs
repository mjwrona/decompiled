// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.ConstantRecord
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels
{
  public class ConstantRecord
  {
    public ConstantRecord() => this.HasUniqueIdentityDisplayName = true;

    public int Id { get; set; }

    public string Domain { get; set; }

    public string Name { get; set; }

    public string DisplayText { get; set; }

    public Guid TeamFoundationId { get; set; }

    public string StringValue { get; set; }

    public bool HasUniqueIdentityDisplayName { get; set; }
  }
}
