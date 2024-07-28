// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.Contracts.MyPlansViewDataProviderContract
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 53130500-4E07-459F-A593-E61E658993AF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.dll

using Microsoft.TeamFoundation.Core.WebApi;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.Contracts
{
  public class MyPlansViewDataProviderContract : MyPlansSkinnyViewDataProviderContract
  {
    public MyPlansViewDataProviderContract(IEnumerable<WebApiTeam> teams)
      : base(teams)
    {
      this.Favorites = new List<Favorite>();
    }

    public MyPlansViewDataProviderContract(Dictionary<string, Team> teamMap)
      : base(teamMap)
    {
      this.Favorites = new List<Favorite>();
    }

    public void AddFavorites(IEnumerable<Favorite> favs) => this.Favorites = favs.ToList<Favorite>();

    public List<Favorite> Favorites { get; private set; }
  }
}
