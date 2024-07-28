// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.MyPlansViewDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 53130500-4E07-459F-A593-E61E658993AF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.Contracts;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins
{
  public class MyPlansViewDataProvider : MyPlansSkinnyViewDataProvider
  {
    public override string Name => "TestManagement.Provider.MineViewDataProvider";

    public virtual MyFavoritePlansDataProviderContract GetFavoritesData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      return new MyFavoritePlansViewDataProvider().GetData(requestContext, providerContext, contribution) as MyFavoritePlansDataProviderContract;
    }

    public override object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      using (requestContext.TraceBlock(1015687, 1015687, "TestManagement", "WebService", this.Name))
      {
        MyPlansSkinnyViewDataProviderContract data1 = base.GetData(requestContext, providerContext, contribution) as MyPlansSkinnyViewDataProviderContract;
        if (!data1.UserRestricted)
        {
          providerContext.Properties = providerContext.Properties ?? new Dictionary<string, object>();
          providerContext.Properties.Add("Project", (object) this.GetProject(requestContext));
          IWorkItemServiceHelper itemServiceHelper = this.GetWorkItemServiceHelper(new TestManagementRequestContext(requestContext));
          if (itemServiceHelper != null)
            providerContext.Properties.Add("WorkItemServiceHelper", (object) itemServiceHelper);
          MyFavoritePlansDataProviderContract favoritesData = this.GetFavoritesData(requestContext, providerContext, contribution);
          MyPlansViewDataProviderContract data2 = new MyPlansViewDataProviderContract(data1.TeamMap);
          data2.AddFavorites((IEnumerable<Favorite>) favoritesData.Favorites);
          return (object) data2;
        }
        this.TraceInfo(requestContext, "UserRestricted returned from skinnyData");
        return (object) data1;
      }
    }

    private void TraceInfo(
      IVssRequestContext requestContext,
      string message,
      params object[] parameters)
    {
      VssRequestContextExtensions.Trace(requestContext, 1015687, TraceLevel.Info, "TestManagement", "WebService", "MyPlansViewDataProvider " + message, parameters);
    }
  }
}
