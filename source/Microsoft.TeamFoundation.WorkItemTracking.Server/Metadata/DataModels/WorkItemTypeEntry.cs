// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.WorkItemTypeEntry
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels
{
  internal class WorkItemTypeEntry
  {
    private IDictionary<int, FieldUsageEntry> m_usageFields;

    public WorkItemTypeEntry() => this.m_usageFields = (IDictionary<int, FieldUsageEntry>) new Dictionary<int, FieldUsageEntry>();

    public static WorkItemTypeEntry Create(WorkItemTypeRecord workItemTypeRecord) => new WorkItemTypeEntry()
    {
      Id = new int?(workItemTypeRecord.Id),
      Description = workItemTypeRecord.Description,
      Name = workItemTypeRecord.Name,
      ReferenceName = workItemTypeRecord.ReferenceName,
      ProjectId = workItemTypeRecord.ProjectId
    };

    public int? Id { get; set; }

    public string Name { get; set; }

    public string ReferenceName { get; set; }

    public string Description { get; set; }

    public int ProjectId { get; set; }

    public string Color { get; set; }

    public string Icon { get; set; }

    public IEnumerable<FieldUsageEntry> UsageFields => (IEnumerable<FieldUsageEntry>) this.m_usageFields.Values;

    public void AddField(int fieldId, FieldSource fieldSource = FieldSource.WorkItemType)
    {
      FieldUsageEntry fieldUsageEntry;
      if (this.m_usageFields.TryGetValue(fieldId, out fieldUsageEntry))
        fieldUsageEntry.FieldSource |= fieldSource;
      else
        this.m_usageFields[fieldId] = new FieldUsageEntry(fieldId, fieldSource);
    }

    public string Form { get; set; }
  }
}
