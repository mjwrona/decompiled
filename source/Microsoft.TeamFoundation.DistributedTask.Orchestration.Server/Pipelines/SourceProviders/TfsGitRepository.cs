// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.SourceProviders.TfsGitRepository
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using System;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.SourceProviders
{
  public class TfsGitRepository
  {
    public TfsGitRepository()
    {
    }

    public TfsGitRepository(Guid id, string name, Guid projectId, string projectName, string url)
    {
      this.Id = id;
      this.Name = name;
      this.ProjectId = projectId;
      this.ProjectName = projectName;
      this.Url = url;
    }

    public TfsGitRepository(
      Guid id,
      string name,
      Guid projectId,
      string projectName,
      string url,
      string defaultBranch)
    {
      this.Id = id;
      this.Name = name;
      this.ProjectId = projectId;
      this.ProjectName = projectName;
      this.Url = url;
      this.DefaultBranch = defaultBranch;
    }

    public Guid Id { get; set; }

    public string Name { get; set; }

    public Guid ProjectId { get; set; }

    public string ProjectName { get; set; }

    public string Url { get; set; }

    public string DefaultBranch { get; set; }
  }
}
