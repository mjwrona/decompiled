// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.VsProjectFile
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  internal class VsProjectFile
  {
    public VsProjectFile(
      Guid guid,
      string name,
      FilePath path,
      string targetFramework,
      string toolsVersion,
      IReadOnlyList<VsProjectType> projectTypes,
      IReadOnlyList<string> projectReferences,
      IReadOnlyList<string> packageReferences,
      bool wasLoaded)
    {
      this.Id = guid;
      this.Name = name;
      this.Path = path;
      this.TargetFramework = targetFramework;
      this.ToolsVersion = toolsVersion;
      this.ProjectTypes = projectTypes;
      this.ProjectReferences = projectReferences;
      this.PackageReferences = packageReferences;
      this.WasLoaded = wasLoaded;
    }

    public string Name { get; }

    public FilePath Path { get; }

    public Guid Id { get; }

    public bool WasLoaded { get; }

    public string TargetFramework { get; }

    public string ToolsVersion { get; }

    public IReadOnlyList<VsProjectType> ProjectTypes { get; }

    public IReadOnlyList<string> ProjectReferences { get; }

    public IReadOnlyList<string> PackageReferences { get; }
  }
}
