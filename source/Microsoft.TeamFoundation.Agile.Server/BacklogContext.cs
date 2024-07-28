// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Server.BacklogContext
// Assembly: Microsoft.TeamFoundation.Agile.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B4912F51-3FCA-4D2B-A7B5-CF15E2F3B46B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Server.dll

using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Agile.Server
{
  [DataContract]
  public class BacklogContext
  {
    protected BacklogContext()
    {
    }

    public BacklogContext(
      Team team,
      BacklogLevelConfiguration currentLevelConfiguration,
      string[] portfolioBacklogNames = null)
    {
      this.Team = team;
      this.Portfolios = portfolioBacklogNames;
      this.CurrentLevelConfiguration = currentLevelConfiguration;
      this.ShowInProgress = true;
    }

    [DataMember(Name = "team")]
    public Team Team { get; private set; }

    public virtual BacklogLevelConfiguration CurrentLevelConfiguration { get; protected set; }

    [DataMember(Name = "portfolios", EmitDefaultValue = true)]
    public string[] Portfolios { get; set; }

    [DataMember(Name = "showInProgress", EmitDefaultValue = true)]
    public bool ShowInProgress { get; set; }

    [DataMember(Name = "includeParents", EmitDefaultValue = true)]
    public bool IncludeParents { get; set; }

    [DataMember(Name = "updateUrlActionAndParameter", EmitDefaultValue = true)]
    public bool UpdateUrlActionAndParameter { get; set; }

    [DataMember(Name = "actionNameFromMru", EmitDefaultValue = true)]
    public string ActionNameFromMru { get; set; }

    [DataMember(Name = "levelName", EmitDefaultValue = true)]
    public string LevelName
    {
      get => this.CurrentLevelConfiguration.Name;
      private set
      {
      }
    }
  }
}
