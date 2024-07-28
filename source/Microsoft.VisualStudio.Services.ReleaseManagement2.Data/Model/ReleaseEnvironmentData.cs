// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironmentData
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model
{
  public class ReleaseEnvironmentData
  {
    public ReleaseEnvironment Environment { get; set; }

    public int ReleaseDefinitionId { get; set; }

    public string ReleaseDefinitionName { get; set; }

    public int ReleaseId { get; set; }

    public string ReleaseName { get; set; }

    [SuppressMessage("Microsoft.Usage", "CA2227", Justification = "Need the setter")]
    public IList<ArtifactSource> LinkedArtifacts { get; set; }

    public ReleaseEnvironmentData() => this.LinkedArtifacts = (IList<ArtifactSource>) new List<ArtifactSource>();
  }
}
