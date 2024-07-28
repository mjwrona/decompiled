// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.Contracts.MyFavoritePlansDataProviderContract
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 53130500-4E07-459F-A593-E61E658993AF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.dll

using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.Contracts
{
  public class MyFavoritePlansDataProviderContract
  {
    public bool HasTestPlans;

    public List<Favorite> Favorites { get; private set; }

    public void AddFavorites(IEnumerable<Microsoft.VisualStudio.Services.Favorites.WebApi.Favorite> favs, IList<WorkItem> workItems)
    {
      Dictionary<int, string> favsLookup = favs.ToDictionary<Microsoft.VisualStudio.Services.Favorites.WebApi.Favorite, int, string>((Func<Microsoft.VisualStudio.Services.Favorites.WebApi.Favorite, int>) (fav => int.Parse(fav.ArtifactId)), (Func<Microsoft.VisualStudio.Services.Favorites.WebApi.Favorite, string>) (fav => fav.Id.ToString()));
      this.Favorites = workItems.Select<WorkItem, Favorite>((Func<WorkItem, Favorite>) (workItem => new Favorite()
      {
        Id = favsLookup[Convert.ToInt32(workItem.Fields["System.Id"])],
        TestPlan = new TestPlan()
        {
          Id = Convert.ToInt32(workItem.Fields["System.Id"]),
          Name = Convert.ToString(workItem.Fields["System.Title"]),
          Fields = new Fields()
          {
            testPlanId = Convert.ToInt32(workItem.Fields["System.Id"]),
            AssignedTo = this.GetValue<IdentityRef>(workItem.Fields, "System.AssignedTo", new IdentityRef()),
            AreaPath = this.GetValue<string>(workItem.Fields, "System.AreaPath", string.Empty),
            IterationPath = this.GetValue<string>(workItem.Fields, "System.IterationPath", string.Empty),
            State = this.GetValue<string>(workItem.Fields, "System.State", string.Empty)
          }
        }
      })).ToList<Favorite>();
    }

    private T GetValue<T>(IDictionary<string, object> Fields, string key, T defaultValue)
    {
      T obj;
      Fields.TryGetValue<string, T>(key, out obj);
      return obj ?? defaultValue;
    }
  }
}
