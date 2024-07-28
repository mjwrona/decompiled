// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ItemPathPair
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal struct ItemPathPair
  {
    public readonly string ProjectNamePath;
    public readonly string ProjectGuidPath;

    public ItemPathPair(string projectNamePath, string projectGuidPath)
    {
      if (projectGuidPath != null)
      {
        ArgumentUtility.CheckForNull<string>(projectNamePath, nameof (ProjectNamePath));
        int length1 = projectNamePath.Length;
        int length2 = "$/".Length;
      }
      this.ProjectNamePath = projectNamePath;
      this.ProjectGuidPath = projectGuidPath;
    }

    public static ItemPathPair FromServerItem(string projectNamePath) => new ItemPathPair(projectNamePath, (string) null);
  }
}
