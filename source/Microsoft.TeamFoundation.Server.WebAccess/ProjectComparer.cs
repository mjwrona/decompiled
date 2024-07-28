// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.ProjectComparer
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Core.WebApi;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  internal class ProjectComparer : IComparer<ProjectInfo>
  {
    public static ProjectComparer Instance = new ProjectComparer();

    private ProjectComparer()
    {
    }

    public int Compare(ProjectInfo first, ProjectInfo second) => TFStringComparer.TeamProjectNameUI.Compare(first.Name, second.Name);
  }
}
