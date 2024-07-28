// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.WebApi.Common.Helpers.WorkItemClassificationNodeHelper
// Assembly: Microsoft.Azure.Boards.WebApi.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC99C479-6852-4E74-BCA4-2660760F9D83
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.WebApi.Common.dll

using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Microsoft.Azure.Boards.WebApi.Common.Helpers
{
  public static class WorkItemClassificationNodeHelper
  {
    public static string FixPath(string path) => (path ?? "").Replace('/', '\\');

    public static TreeStructureType ToInternalTreeStructureType(TreeStructureGroup structureGroup)
    {
      if (structureGroup == TreeStructureGroup.Areas)
        return TreeStructureType.Area;
      return structureGroup == TreeStructureGroup.Iterations ? TreeStructureType.Iteration : TreeStructureType.None;
    }
  }
}
