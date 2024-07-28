// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitRoutes
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.Git.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class GitRoutes
  {
    public const string GitRouteTypeKey = "GitRouteType";
    public const string RepositoryRouteKeyName = "GitRepositoryName";
    public const string GitAreaKeyName = "GitArea";
    public const string RouteSegmentProject = "project";
    public const string RouteSegmentTeam = "team";
    public const string CollectionRoutePrefix = "VersionControl_Git_Repo_";
    public const string ProjectRoutePrefix = "VersionControl_Project_Git_Repo_";
    public const string TeamRoutePrefix = "VersionControl_Project_Team_Git_Repo_";
  }
}
