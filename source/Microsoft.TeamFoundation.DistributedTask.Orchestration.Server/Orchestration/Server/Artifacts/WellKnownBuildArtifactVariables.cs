// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.WellKnownBuildArtifactVariables
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts
{
  public static class WellKnownBuildArtifactVariables
  {
    public const string BuildType = "Build.Type";
    public const string BuildRepositoryName = "Build.Repository.Name";
    public const string BuildRepositoryProvider = "Build.Repository.Provider";
    public const string BuildDefinitionId = "Build.DefinitionId";
    public const string ProjectName = "projectName";
    public const string ProjectId = "projectID";
    public const string PipelineId = "pipelineID";
    public const string PipelineName = "pipelineName";
    public const string RunName = "runName";
    public const string RunId = "runID";
    public const string RunURI = "runURI";
    public const string SourceBranch = "sourceBranch";
    public const string SourceCommit = "sourceCommit";
    public const string SourceProvider = "sourceProvider";
    public const string RequestedFor = "requestedFor";
    public const string RequestedForID = "requestedForID";
    private const string baseKey = "resources.pipeline";

    public static string GetVariableKey(string alias, string key) => string.Format("{0}.{1}.{2}", (object) "resources.pipeline", (object) alias, (object) key);
  }
}
