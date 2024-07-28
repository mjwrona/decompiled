// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Process.CategoryWorkItemTypes
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Process
{
  public class CategoryWorkItemTypes
  {
    public CategoryWorkItemTypes(string name) => this.Name = name;

    public string Name { get; private set; }

    public IReadOnlyCollection<string> WorkItemTypeNames { get; set; }

    public string DefaultWorkItemTypeName { get; set; }

    public string CategoryReferenceName { get; set; }
  }
}
