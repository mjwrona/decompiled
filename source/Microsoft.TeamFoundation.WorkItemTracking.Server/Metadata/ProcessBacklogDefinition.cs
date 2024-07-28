// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.ProcessBacklogDefinition
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  public class ProcessBacklogDefinition
  {
    public string SingularName { get; internal set; }

    public string PluralName { get; set; }

    public string CategoryReferenceName { get; set; }

    public int Rank { get; set; }

    public IReadOnlyCollection<string> WorkItemTypesInCategory { get; set; }

    public IReadOnlyCollection<string> AddPanelFields { get; set; }

    public IReadOnlyCollection<BacklogColumn> Columns { get; set; }

    public int WorkItemCountLimit { get; set; }

    public string DefaultWorkItemTypeName { get; set; }
  }
}
