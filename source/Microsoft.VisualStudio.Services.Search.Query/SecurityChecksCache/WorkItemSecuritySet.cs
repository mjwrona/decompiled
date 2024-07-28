// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.SecurityChecksCache.WorkItemSecuritySet
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Search.Common;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Query.SecurityChecksCache
{
  public class WorkItemSecuritySet
  {
    private readonly bool m_allAreasAreAccessible;
    private readonly HashSet<ClassificationNode> m_accessibleAreas;

    public HashSet<ClassificationNode> AccessibleAreas => this.m_accessibleAreas;

    public bool AllAreasAreAccessible => this.m_allAreasAreAccessible;

    public WorkItemSecuritySet(
      bool allAreasAreAccessible,
      IEnumerable<ClassificationNode> accessibleAreas)
    {
      this.m_allAreasAreAccessible = allAreasAreAccessible;
      this.m_accessibleAreas = new HashSet<ClassificationNode>((IEqualityComparer<ClassificationNode>) new ClassificationNodeIdComparator());
      if (this.m_allAreasAreAccessible)
        return;
      this.m_accessibleAreas.AddRange<ClassificationNode, HashSet<ClassificationNode>>(accessibleAreas);
    }

    public override bool Equals(object obj)
    {
      WorkItemSecuritySet workItemSecuritySet = obj as WorkItemSecuritySet;
      if (this == obj)
        return true;
      return workItemSecuritySet != null && this.AllAreasAreAccessible == workItemSecuritySet.AllAreasAreAccessible && this.AreClassicationNodeSetEqual(this.AccessibleAreas, workItemSecuritySet.AccessibleAreas);
    }

    public override int GetHashCode()
    {
      if (this.AllAreasAreAccessible)
        return 1;
      int hashCode = 0;
      foreach (ClassificationNode accessibleArea in this.AccessibleAreas)
        hashCode ^= accessibleArea.GetHashCode();
      return hashCode;
    }

    private bool AreClassicationNodeSetEqual(
      HashSet<ClassificationNode> nodes1,
      HashSet<ClassificationNode> nodes2)
    {
      if (nodes1.Count != nodes2.Count)
        return false;
      foreach (ClassificationNode classificationNode in nodes1)
      {
        if (!nodes2.Contains(classificationNode))
          return false;
      }
      return true;
    }
  }
}
