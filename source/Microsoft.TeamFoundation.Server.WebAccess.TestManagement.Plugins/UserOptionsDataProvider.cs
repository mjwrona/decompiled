// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.UserOptionsDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 53130500-4E07-459F-A593-E61E658993AF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.Contracts;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Settings;
using System;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins
{
  public class UserOptionsDataProvider : IExtensionDataProvider
  {
    private const string FilterSettingKeyFormat = "{0}/{1}/Filter";
    private const string SelectedPivotSettingKey = "TestPlanHub/Navigation/SelectedPivot";
    private const string HubName = "Test Plans";
    private ContextIdentifier project;
    protected Func<IVssRequestContext, ContextIdentifier> GetProject;

    public UserOptionsDataProvider() => this.GetProject = (Func<IVssRequestContext, ContextIdentifier>) (requestContext => WebPageDataProviderUtil.GetPageSource(requestContext).Project);

    public string Name => "TestManagement.Provider.TestPlansHubUserOptions";

    public object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      this.project = this.GetProject(requestContext);
      requestContext.GetService<ISettingsService>();
      return (object) new UserOptions()
      {
        AllPlansFilterState = this.GetSavedFilterState(requestContext, "all"),
        MyPlansFilterState = this.GetSavedFilterState(requestContext, "mine"),
        SelectedPivot = this.GetSelectedPivot(requestContext)
      };
    }

    private string GetSelectedPivot(IVssRequestContext requestContext) => requestContext.GetService<ISettingsService>().GetValue<string>(requestContext, SettingsUserScope.User, "Project", this.project.Id.ToString(), "TestPlanHub/Navigation/SelectedPivot", string.Empty);

    private string GetSavedFilterState(IVssRequestContext requestContext, string pivotName) => requestContext.GetService<ISettingsService>().GetValue<string>(requestContext, SettingsUserScope.User, "Project", this.project.Id.ToString(), UserOptionsDataProvider.GetFilterRegistryKey("Test Plans", pivotName), string.Empty);

    private static string GetFilterRegistryKey(string hubName, string pivotName) => string.Format("{0}/{1}/Filter", (object) hubName, (object) pivotName);
  }
}
