// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.Contracts.AllPlansViewDataProviderContract
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 53130500-4E07-459F-A593-E61E658993AF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.dll

using Microsoft.TeamFoundation.Core.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.Contracts
{
  public class AllPlansViewDataProviderContract : TestPlanViewDataProviderContract
  {
    protected Dictionary<string, Team> teams;

    public AllPlansViewDataProviderContract(IEnumerable<WebApiTeam> teams)
    {
      this.PlansWithoutTeams = new List<int>();
      this.teams = teams.ToDictionary<WebApiTeam, string, Team>((Func<WebApiTeam, string>) (team => team.Id.ToString()), (Func<WebApiTeam, Team>) (team => new Team()
      {
        Id = team.Id.ToString(),
        Name = team.Name
      }));
    }

    public void AddPlanIdsToTeam(WebApiTeam team, List<int> planIds)
    {
      string key;
      if (team.Identity == null)
      {
        key = Guid.Empty.ToString();
        this.teams.Add(key, new Team());
        this.teams[key].Id = key;
        this.teams[key].Name = "placeholder";
      }
      else
        key = team.Id.ToString();
      this.teams[key].TestPlans = planIds.Select<int, TestPlan>((Func<int, TestPlan>) (planId => new TestPlan()
      {
        Id = planId
      })).ToList<TestPlan>();
    }

    public override List<Team> Teams => this.teams.Select<KeyValuePair<string, Team>, Team>((Func<KeyValuePair<string, Team>, Team>) (kvp => kvp.Value)).ToList<Team>();

    public List<int> PlansWithoutTeams { get; set; }
  }
}
