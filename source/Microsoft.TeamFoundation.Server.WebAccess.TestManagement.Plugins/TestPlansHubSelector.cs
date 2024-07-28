// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.TestPlansHubSelector
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 53130500-4E07-459F-A593-E61E658993AF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions;
using Newtonsoft.Json.Linq;
using System;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins
{
  public class TestPlansHubSelector : 
    IPreExecuteContributedRequestHandler,
    IContributedRequestHandler
  {
    protected Func<IVssRequestContext, WebPageDataProviderPageSource> GetPageSource;
    protected Func<string> GetPlanIdFromContext;
    protected Func<IVssRequestContext, IWebUserSettingsHive> GetHive;
    protected Func<IVssRequestContext, string> GetRequestPath;
    private ContextIdentifier Project;
    private ContextIdentifier Team;
    private const string c_planId = "planId";
    private const string featureFlag = "WebAccess.TestManagement.NewTestPlanExperience.Toggle";
    private const string c_testManagementMoniker = "TestManagement";
    private const string c_testHubMoniker = "TestHub";
    private const string c_selectedPlanIdKey = "SelectedPlanId";

    public TestPlansHubSelector()
    {
      this.GetPageSource = (Func<IVssRequestContext, WebPageDataProviderPageSource>) (requestContext => WebPageDataProviderUtil.GetPageSource(requestContext));
      this.GetPlanIdFromContext = (Func<string>) (() => HttpContext.Current.Request["planId"]);
      this.GetHive = (Func<IVssRequestContext, IWebUserSettingsHive>) (requestContext => (IWebUserSettingsHive) new TestManagementWebUserSettingsHive(requestContext));
      this.GetRequestPath = (Func<IVssRequestContext, string>) (requestContext => requestContext.RequestPath());
    }

    public string Name => "TFS.testplanshubselector";

    public void OnPreExecute(
      IVssRequestContext requestContext,
      ActionExecutingContext actionExecutingContext,
      JObject properties)
    {
      if (!this.IsNewTestPlansHubEnabled(requestContext) || this.UrlContainsPlanId())
        return;
      WebPageDataProviderPageSource providerPageSource = this.GetPageSource(requestContext);
      this.Project = providerPageSource.Project;
      this.Team = providerPageSource.Team;
      if (this.GetSavedPlanId(requestContext) > 0)
        return;
      try
      {
        LicenseCheckHelper.ValidateIfUserHasTestManagementAdvancedAccess(requestContext);
      }
      catch (UnauthorizedAccessException ex)
      {
        return;
      }
      string url = this.GetRequestPath(requestContext) + "/mine";
      actionExecutingContext.Result = (ActionResult) new RedirectResult(url);
    }

    private int GetSavedPlanId(IVssRequestContext requestContext)
    {
      int savedPlanId = 0;
      try
      {
        using (IWebUserSettingsHive userSettingsHive = this.GetHive(requestContext))
        {
          userSettingsHive.Cache(this.GetCachePathPattern());
          savedPlanId = userSettingsHive.ReadSetting<int>(this.ComposeKey("SelectedPlanId", true), 0);
        }
      }
      catch (Exception ex)
      {
      }
      return savedPlanId;
    }

    private bool IsNewTestPlansHubEnabled(IVssRequestContext requestContext) => requestContext.GetService<ITeamFoundationFeatureAvailabilityService>().IsFeatureEnabled(requestContext, "WebAccess.TestManagement.NewTestPlanExperience.Toggle");

    private bool UrlContainsPlanId()
    {
      string s = this.GetPlanIdFromContext();
      return !string.IsNullOrEmpty(s) && int.TryParse(s, out int _);
    }

    private string GetCachePathPattern() => "TestManagement" + "/" + (this.Project.Id.ToString() + "/") + "...";

    private string ComposeKey(string property, bool includeTeam = false)
    {
      string str1 = this.Project.Id.ToString() + "/";
      string str2;
      if (includeTeam)
      {
        ContextIdentifier team = this.Team;
        int num;
        if (team == null)
        {
          num = 0;
        }
        else
        {
          Guid id = team.Id;
          num = 1;
        }
        if (num != 0)
        {
          str2 = this.Team.Id.ToString() + "/";
          goto label_7;
        }
      }
      str2 = string.Empty;
label_7:
      string str3 = str2;
      return "TestManagement" + "/" + str1 + str3 + "TestHub" + "/" + property;
    }
  }
}
