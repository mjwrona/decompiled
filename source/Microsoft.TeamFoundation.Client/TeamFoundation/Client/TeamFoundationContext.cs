// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.TeamFoundationContext
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.TeamFoundation.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class TeamFoundationContext : ITeamFoundationContext, ITeamFoundationSourceControlContext
  {
    private static TeamFoundationContext s_empty = new TeamFoundationContext((TfsTeamProjectCollection) null, (string) null, (string) null, (string) null, Guid.Empty, (IEnumerable<Guid>) null, TeamFoundationSourceControlType.Default);

    public TeamFoundationContext(
      TfsTeamProjectCollection collection,
      string teamProjectName,
      string teamProjectUri)
      : this(collection, teamProjectName, teamProjectUri, (string) null, Guid.Empty, (IEnumerable<Guid>) null, TeamFoundationSourceControlType.Default)
    {
    }

    public TeamFoundationContext(
      TfsTeamProjectCollection collection,
      string teamProjectName,
      string teamProjectUri,
      string teamName,
      Guid teamId,
      IEnumerable<Guid> repositoryIds,
      TeamFoundationSourceControlType sourceControlType)
    {
      this.TeamProjectCollection = collection;
      this.TeamProjectName = teamProjectName;
      this.TeamProjectUri = string.IsNullOrEmpty(teamProjectUri) ? (Uri) null : new Uri(teamProjectUri);
      this.TeamName = teamName;
      this.TeamId = teamId;
      this.RepositoryIds = repositoryIds != null ? (IReadOnlyList<Guid>) repositoryIds.ToList<Guid>().AsReadOnly() : (IReadOnlyList<Guid>) null;
      this.SourceControlType = sourceControlType;
    }

    public static TeamFoundationContext Empty => TeamFoundationContext.s_empty;

    public TfsTeamProjectCollection TeamProjectCollection { get; private set; }

    public bool HasCollection => this.TeamProjectCollection != null;

    public string TeamProjectName { get; private set; }

    public Uri TeamProjectUri { get; private set; }

    public bool HasTeamProject => !string.IsNullOrEmpty(this.TeamProjectName) && this.TeamProjectUri != (Uri) null;

    public string TeamName { get; private set; }

    public Guid TeamId { get; private set; }

    public bool HasTeam => this.TeamId != Guid.Empty;

    public TeamFoundationSourceControlType SourceControlType { get; private set; }

    public IReadOnlyList<Guid> RepositoryIds { get; private set; }

    public bool HasRepositoryIds => this.RepositoryIds != null && this.RepositoryIds.Count > 0;
  }
}
