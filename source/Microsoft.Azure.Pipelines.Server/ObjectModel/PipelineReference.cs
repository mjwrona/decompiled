// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Server.ObjectModel.PipelineReference
// Assembly: Microsoft.Azure.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC20940E-746B-4985-9936-F8ACD7ADA1DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.Azure.Pipelines.Server.ObjectModel
{
  public class PipelineReference
  {
    public PipelineReference(
      Guid projectId,
      int pipelineId,
      int revision,
      string name,
      string folder)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForNonPositiveInt(pipelineId, nameof (pipelineId));
      ArgumentUtility.CheckForNonPositiveInt(revision, nameof (revision));
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      this.ProjectId = projectId;
      this.Id = pipelineId;
      this.Revision = revision;
      this.Name = name;
      this.Folder = string.IsNullOrEmpty(folder) ? "/" : folder;
    }

    public Guid ProjectId { get; private set; }

    public int Id { get; private set; }

    public int Revision { get; private set; }

    public string Name { get; private set; }

    public string Folder { get; private set; }
  }
}
