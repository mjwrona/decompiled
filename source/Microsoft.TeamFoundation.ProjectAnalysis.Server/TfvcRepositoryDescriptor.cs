// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ProjectAnalysis.Server.TfvcRepositoryDescriptor
// Assembly: Microsoft.TeamFoundation.ProjectAnalysis.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 076482BC-74A4-4A35-9427-1E61C33D1FA6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.ProjectAnalysis.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.ProjectAnalysis.WebApi;
using System;

namespace Microsoft.TeamFoundation.ProjectAnalysis.Server
{
  public class TfvcRepositoryDescriptor : IRepositoryDescriptor
  {
    public TfvcRepositoryDescriptor(ProjectInfo project)
    {
      this.Name = project.Name;
      this.ProjectId = project.Id;
      this.TfvcRootFolder = "$/" + project.Name + "/";
      this.ChangesetId = -1;
    }

    public string Name { get; private set; }

    public Guid Id => this.ProjectId;

    public Guid ProjectId { get; private set; }

    public RepositoryType Type => RepositoryType.Tfvc;

    public string TfvcRootFolder { get; private set; }

    public int ChangesetId { get; set; }
  }
}
