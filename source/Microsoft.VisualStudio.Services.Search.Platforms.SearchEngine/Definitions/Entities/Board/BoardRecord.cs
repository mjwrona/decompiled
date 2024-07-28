// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Board.BoardRecord
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts;
using Microsoft.VisualStudio.Services.Search.WebApi.Contracts;
using System;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Board
{
  public class BoardRecord
  {
    public Project Project { get; private set; }

    public Collection Collection { get; private set; }

    public Team Team { get; private set; }

    public string BoardType { get; private set; }

    public BoardRecord(
      string collectionName,
      string projectName,
      string projectId,
      string teamName,
      string teamId,
      string boardType)
    {
      this.Collection = new Collection()
      {
        Name = collectionName
      };
      this.Project = new Project()
      {
        Name = projectName,
        Id = new Guid(projectId)
      };
      this.Team = new Team()
      {
        Name = teamName,
        Id = new Guid(teamId)
      };
      this.BoardType = boardType;
    }
  }
}
