// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.CustomArtifactDetails
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts
{
  public class CustomArtifactDetails
  {
    public string ArtifactsUrl { get; set; }

    public string ResultSelector { get; set; }

    public string ResultTemplate { get; set; }

    public Dictionary<string, string> ArtifactVariables { get; set; }

    public List<AuthorizationHeader> AuthorizationHeaders { get; set; }

    public IDictionary<string, string> ArtifactTypeStreamMapping { get; set; }

    public string VersionsUrl { get; set; }

    public string VersionsResultSelector { get; set; }

    public string VersionsResultTemplate { get; set; }
  }
}
