// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.FieldUsageEntry
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels
{
  internal class FieldUsageEntry
  {
    public int FieldId { get; set; }

    public FieldSource FieldSource { get; set; }

    public FieldUsageEntry(int fieldId, FieldSource fieldSource)
    {
      this.FieldId = fieldId;
      this.FieldSource = fieldSource;
    }

    public override bool Equals(object obj) => obj is FieldUsageEntry fieldUsageEntry && this.FieldId == fieldUsageEntry.FieldId;

    public override int GetHashCode() => this.FieldId.GetHashCode();
  }
}
