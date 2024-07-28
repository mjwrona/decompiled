// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.LegacyWorkItemType
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class LegacyWorkItemType
  {
    private HashSet<int> m_fields;

    internal LegacyWorkItemType() => this.m_fields = new HashSet<int>();

    internal LegacyWorkItemType(int projectId, int id, string name)
      : this()
    {
      this.ProjectId = projectId;
      this.Id = id;
      this.Name = name;
    }

    public int ProjectId { get; internal set; }

    public int Id { get; internal set; }

    public string Name { get; internal set; }

    public IEnumerable<int> FieldIds => (IEnumerable<int>) this.m_fields;

    internal void AddField(int id) => this.m_fields.Add(id);
  }
}
