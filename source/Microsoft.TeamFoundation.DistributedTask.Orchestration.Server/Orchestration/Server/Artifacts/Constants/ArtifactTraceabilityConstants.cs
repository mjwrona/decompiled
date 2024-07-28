// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Constants.ArtifactTraceabilityConstants
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Constants
{
  public static class ArtifactTraceabilityConstants
  {
    public static readonly string GenericArtifactName = "*";
    public static readonly string TraceLayer = "ArtifactTraceability";
    public static readonly string SelfArtifactAlias = "self";
    public static readonly int IncorrectId = -1;
    public static readonly string ArtifactTraceabilityDeploymentService = nameof (ArtifactTraceabilityDeploymentService);
  }
}
