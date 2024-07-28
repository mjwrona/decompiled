// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.Predicates.IPredicateValidationHelper
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.WorkItemTracking.Internals;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common.Predicates
{
  public interface IPredicateValidationHelper
  {
    InternalFieldType GetFieldType(string fieldReferenceName);

    int GetFieldId(string fieldReferenceName);

    string GetTreePath(int treeNodeId);

    int GetTreeId(string path, TreeStructureType type);
  }
}
