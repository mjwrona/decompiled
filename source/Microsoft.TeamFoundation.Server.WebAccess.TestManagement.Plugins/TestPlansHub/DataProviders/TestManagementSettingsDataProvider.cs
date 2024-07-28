// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.TestPlansHub.DataProviders.TestManagementSettingsDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 53130500-4E07-459F-A593-E61E658993AF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Settings;
using System;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.TestPlansHub.DataProviders
{
  public class TestManagementSettingsDataProvider
  {
    private const string c_selectedPlanIdKey = "SelectedPlanId";

    public class TestPlansLiteHubDataProvider : IExtensionDataProvider
    {
      public string Name => "TestManagement.Provider.TestManagementSettings";

      public object GetData(
        IVssRequestContext requestContext,
        DataProviderContext providerContext,
        Contribution contribution)
      {
        WebPageDataProviderPageSource pageSource = WebPageDataProviderUtil.GetPageSource(requestContext);
        Guid id1 = pageSource.Project.Id;
        ContextIdentifier team = pageSource.Team;
        if (team == null)
        {
          Guid empty = Guid.Empty;
        }
        else
        {
          Guid id2 = team.Id;
        }
        return (object) new
        {
          SelectedPlanId = requestContext.GetService<ISettingsService>().GetValue<int>(requestContext, SettingsUserScope.User, "project", id1.ToString(), "SelectedPlanId", 0, false)
        };
      }
    }
  }
}
