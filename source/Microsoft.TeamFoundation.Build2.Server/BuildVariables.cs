// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildVariables
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

namespace Microsoft.TeamFoundation.Build2.Server
{
  public static class BuildVariables
  {
    public const string RepoId = "build.repository.id";
    public const string RepoName = "build.repository.name";
    public const string BuildTriggeredByDefinitionId = "build.triggeredBy.definitionId";
    public const string BuildTriggeredByBuildId = "build.triggeredBy.buildId";
    public const string BuildTriggeredByDefinitionName = "build.triggeredBy.definitionName";
    public const string BuildTriggeredByBuildNumber = "build.triggeredBy.buildNumber";
    public const string BuildTriggeredByProjectId = "build.triggeredBy.projectId";
    public const string IsTriggeringRepository = "system.istriggeringrepository";
  }
}
