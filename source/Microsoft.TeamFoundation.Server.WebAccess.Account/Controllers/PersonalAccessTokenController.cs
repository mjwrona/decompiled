// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Account.Controllers.PersonalAccessTokenController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Account, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC21A176-69BE-407E-B3DD-80612369F784
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Account.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Aad;
using Microsoft.TeamFoundation.Server.WebAccess.Html;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.Organization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Account.Controllers
{
  [SupportedRouteArea(NavigationContextLevels.ApplicationAll)]
  [DemandFeature("00000000-0000-0000-0000-000000000000", false)]
  public class PersonalAccessTokenController : AccountAreaController
  {
    public PersonalAccessTokenController() => this.m_executeContributedRequestHandlers = true;

    [HttpGet]
    [TfsTraceFilter(505001, 505010)]
    public ActionResult Index()
    {
      this.ConfigureLeftHubSplitter(AccountServerResources.SecurityDetailsNavigationSplitter, toggleButtonCollapsedTooltip: AccountServerResources.SecurityDetailsNavigationSplitterCollapsed, toggleButtonExpandedTooltip: AccountServerResources.SecurityDetailsNavigationSplitterExpanded);
      return (ActionResult) this.View();
    }

    [HttpGet]
    [TfsTraceFilter(505011, 505020)]
    public ActionResult Edit(string authorizationId = null)
    {
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      PersonalAccessTokenModel model = new PersonalAccessTokenModel();
      model.Description = "";
      model.AccountMode = ApplicableAccountMode.AllAccounts;
      model.ScopeMode = AuthorizedScopeMode.AllScopes;
      model.SelectedAccounts = new System.Collections.Generic.List<string>();
      model.SelectedScopes = new System.Collections.Generic.List<string>();
      model.AuthorizationId = authorizationId;
      model.IsValid = true;
      model.DisplayAllAccountsOption = !this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.Identity.DisablePATDeploymentContextForAllAccounts");
      model.ValidExpirationValues = (IDictionary<string, string>) new Dictionary<string, string>()
      {
        {
          "90",
          string.Format(AccountServerResources.PATExpiresInDaysOption, (object) "90")
        },
        {
          "180",
          string.Format(AccountServerResources.PATExpiresInDaysOption, (object) "180")
        },
        {
          "365",
          AccountServerResources.PATExpiresInOneYearOption
        }
      };
      if (!string.IsNullOrEmpty(authorizationId))
      {
        IDelegatedAuthorizationService service = this.TfsRequestContext.GetService<IDelegatedAuthorizationService>();
        try
        {
          Guid result;
          if (Guid.TryParse(authorizationId, out result))
          {
            SessionToken sessionToken = service.GetSessionToken(this.TfsRequestContext, result);
            if (sessionToken != null)
            {
              model.Description = HttpUtility.HtmlDecode(sessionToken.DisplayName);
              model.IsValid = sessionToken.IsValid;
              if (sessionToken.Scope.Contains("app_token") || sessionToken.Scope.Contains("preview_api_all"))
              {
                model.ScopeMode = AuthorizedScopeMode.AllScopes;
              }
              else
              {
                model.ScopeMode = AuthorizedScopeMode.SelectedScopes;
                model.SelectedScopes = ((IEnumerable<string>) sessionToken.Scope.Split(' ')).ToList<string>();
              }
              if (sessionToken.TargetAccounts != null && sessionToken.TargetAccounts.Any<Guid>())
              {
                model.AccountMode = ApplicableAccountMode.SelectedAccounts;
                model.SelectedAccounts = new System.Collections.Generic.List<string>(sessionToken.TargetAccounts.Select<Guid, string>((Func<Guid, string>) (x => x.ToString())));
              }
              else
                model.AccountMode = ApplicableAccountMode.AllAccounts;
              model.ExpiresUtc = sessionToken.ValidTo.ToString() + " UTC";
              double totalDays = (sessionToken.ValidTo - sessionToken.ValidFrom).TotalDays;
              foreach (KeyValuePair<string, string> validExpirationValue in (IEnumerable<KeyValuePair<string, string>>) model.ValidExpirationValues)
              {
                if (Math.Abs((double) int.Parse(validExpirationValue.Key) - totalDays) < 1.0)
                  model.SelectedExpiration = validExpirationValue.Key;
              }
            }
          }
        }
        catch (Exception ex)
        {
        }
      }
      else
      {
        Guid guid = this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.Identity.DisableUsageOfCollectionIdForPATScoping") ? this.TfsRequestContext.To(TeamFoundationHostType.Application).ServiceHost.InstanceId : this.TfsRequestContext.ServiceHost.InstanceId;
        if (!this.TfsRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) && guid != Guid.Empty)
        {
          model.SelectedAccounts = new System.Collections.Generic.List<string>()
          {
            guid.ToString()
          };
          model.AccountMode = ApplicableAccountMode.SelectedAccounts;
        }
        if (this.TfsRequestContext.IsHosted())
        {
          Collection collection = this.TfsRequestContext.GetService<ICollectionService>().GetCollection(this.TfsRequestContext, (IEnumerable<string>) null);
          model.AllAccounts = (IDictionary<Guid, string>) new Dictionary<Guid, string>()
          {
            {
              guid,
              collection.Name
            }
          };
        }
      }
      if (this.TfsRequestContext.IsHosted())
      {
        try
        {
          IAadTenantDetailProvider extension = this.TfsRequestContext.GetExtension<IAadTenantDetailProvider>();
          model.Tenant = extension?.GetDisplayName(this.TfsRequestContext.Elevate());
        }
        catch
        {
        }
      }
      AuthorizationScopeDefinitions scopeDefinitions = AuthorizationScopeDefinitions.Default;
      IDictionary<string, string> localizedScopeTitleMap = AuthorizationScopeDefinitions.LocalizedTitleMap;
      IInstalledExtensionManager installedExtensionManager = this.TfsRequestContext.GetService<IInstalledExtensionManager>();
      string localizedTitle;
      model.AllScopes = (IList<KeyValuePair<string, string>>) ((IEnumerable<AuthorizationScopeDefinition>) scopeDefinitions.scopes).Where<AuthorizationScopeDefinition>((Func<AuthorizationScopeDefinition, bool>) (x =>
      {
        if (x.availability == AuthorizationScopeAvailability.Public || x.availability == AuthorizationScopeAvailability.None && !string.IsNullOrEmpty(x.availabilityFeatureFlag) && this.TfsRequestContext.IsFeatureEnabled(x.availabilityFeatureFlag))
          return true;
        return !string.IsNullOrEmpty(x.firstPartyExtensionName) && installedExtensionManager.IsExtensionActive(this.TfsRequestContext, "ms", x.firstPartyExtensionName);
      })).Select<AuthorizationScopeDefinition, KeyValuePair<string, string>>((Func<AuthorizationScopeDefinition, KeyValuePair<string, string>>) (x => new KeyValuePair<string, string>(x.scope, localizedScopeTitleMap.TryGetValue(x.title, out localizedTitle) ? localizedTitle : x.title))).OrderBy<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (x => x.Value)).ToList<KeyValuePair<string, string>>();
      this.ConfigureLeftHubSplitter(AccountServerResources.SecurityDetailsNavigationSplitter, toggleButtonCollapsedTooltip: AccountServerResources.SecurityDetailsNavigationSplitterCollapsed, toggleButtonExpandedTooltip: AccountServerResources.SecurityDetailsNavigationSplitterExpanded);
      return (ActionResult) this.View((object) model);
    }

    private System.Collections.Generic.List<string> UnpackList(System.Collections.Generic.List<string> value)
    {
      if (value == null || !value.Any<string>())
        return (System.Collections.Generic.List<string>) null;
      return ((IEnumerable<string>) value.First<string>().Split(',')).ToList<string>();
    }

    [HttpPost]
    [TfsBypassAntiForgeryValidation]
    public ActionResult Index(string authorizationId, string accessToken)
    {
      PersonalAccessTokenIndexModel model = new PersonalAccessTokenIndexModel()
      {
        AuthorizationId = authorizationId,
        AccessToken = accessToken
      };
      this.ConfigureLeftHubSplitter(AccountServerResources.SecurityDetailsNavigationSplitter);
      return (ActionResult) this.View(nameof (Index), (object) model);
    }

    [HttpPost]
    [ValidateInput(false)]
    [TfsTraceFilter(505021, 505030)]
    public ActionResult Edit(PersonalAccessTokenModel personalAccessToken)
    {
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      if (personalAccessToken.ScopeMode == AuthorizedScopeMode.AllScopes)
        personalAccessToken.SelectedScopes = new System.Collections.Generic.List<string>()
        {
          "app_token"
        };
      personalAccessToken.SelectedScopes = this.UnpackList(personalAccessToken.SelectedScopes);
      personalAccessToken.SelectedAccounts = personalAccessToken.SelectedScopes != null && !string.IsNullOrEmpty(personalAccessToken.SelectedScopes.First<string>()) ? this.UnpackList(personalAccessToken.SelectedAccounts) : throw new ArgumentException(AccountResources.TokenNoScopeSelectedMessage);
      DateTime dateTime = DateTime.UtcNow.AddDays(double.Parse(personalAccessToken.SelectedExpiration)).AddMinutes(-1.0);
      if (dateTime <= DateTime.UtcNow)
        throw new ArgumentException(AccountResources.InvalidTokenExperiationDate);
      string scope1 = string.Join(" ", (IEnumerable<string>) personalAccessToken.SelectedScopes);
      IVssRequestContext vssRequestContext = personalAccessToken.AccountMode == ApplicableAccountMode.AllAccounts ? this.TfsRequestContext.To(TeamFoundationHostType.Deployment) : this.TfsRequestContext;
      IDelegatedAuthorizationService service = vssRequestContext.GetService<IDelegatedAuthorizationService>();
      Guid result1;
      if (Guid.TryParse(personalAccessToken.AuthorizationId, out result1))
      {
        service.UpdateSessionToken(vssRequestContext, result1, personalAccessToken.Description, scope1, new DateTime?(dateTime));
        return (ActionResult) this.Json(new object());
      }
      System.Collections.Generic.List<Guid> guidList1 = (System.Collections.Generic.List<Guid>) null;
      if (personalAccessToken.AccountMode == ApplicableAccountMode.SelectedAccounts)
      {
        guidList1 = new System.Collections.Generic.List<Guid>();
        foreach (string selectedAccount in personalAccessToken.SelectedAccounts)
        {
          Guid result2;
          if (Guid.TryParse(selectedAccount, out result2))
            guidList1.Add(result2);
        }
      }
      if (vssRequestContext.IsFeatureEnabled("VisualStudio.Services.Identity.PATPaginationAndFiltering"))
      {
        IDelegatedAuthorizationService authorizationService = service;
        IVssRequestContext requestContext = vssRequestContext;
        string description = personalAccessToken.Description;
        DateTime? nullable = new DateTime?(dateTime);
        string str = scope1;
        IList<Guid> guidList2 = (IList<Guid>) guidList1;
        Guid? clientId = new Guid?();
        Guid? userId = new Guid?();
        string name = description;
        DateTime? validTo = nullable;
        string scope2 = str;
        IList<Guid> targetAccounts = guidList2;
        Guid? authorizationId = new Guid?();
        Guid? accessId = new Guid?();
        return (ActionResult) this.Json((object) new PersonalAccessTokenDetailsModel(authorizationService.IssueSessionToken(requestContext, clientId, userId, name, validTo, scope2, targetAccounts, SessionTokenType.Compact, isRequestedByTfsPatWebUI: true, authorizationId: authorizationId, accessId: accessId).SessionToken));
      }
      IDelegatedAuthorizationService authorizationService1 = service;
      IVssRequestContext requestContext1 = vssRequestContext;
      string description1 = personalAccessToken.Description;
      DateTime? nullable1 = new DateTime?(dateTime);
      string str1 = scope1;
      IList<Guid> guidList3 = (IList<Guid>) guidList1;
      Guid? clientId1 = new Guid?();
      Guid? userId1 = new Guid?();
      string name1 = description1;
      DateTime? validTo1 = nullable1;
      string scope3 = str1;
      IList<Guid> targetAccounts1 = guidList3;
      Guid? authorizationId1 = new Guid?();
      Guid? accessId1 = new Guid?();
      return (ActionResult) this.Json((object) new PersonalAccessTokenDetailsModel(authorizationService1.IssueSessionToken(requestContext1, clientId1, userId1, name1, validTo1, scope3, targetAccounts1, SessionTokenType.Compact, authorizationId: authorizationId1, accessId: accessId1).SessionToken));
    }

    [HttpGet]
    [TfsTraceFilter(505031, 505040)]
    public ActionResult List(TokenPageRequest pageRequest = null)
    {
      UserPreferences userPreferences = this.TfsRequestContext.GetService<IUserPreferencesService>().GetUserPreferences(this.TfsRequestContext);
      IDelegatedAuthorizationService service = this.TfsRequestContext.GetService<IDelegatedAuthorizationService>();
      bool flag = this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.Identity.PATPaginationAndFiltering");
      if (flag && pageRequest != null && !string.IsNullOrWhiteSpace(pageRequest.PageRequestTimeStamp))
      {
        PagedSessionTokens pagedSessionTokens = service.ListSessionTokensByPage(this.TfsRequestContext, pageRequest);
        if (pagedSessionTokens.SessionTokens == null)
          pagedSessionTokens.SessionTokens = (IList<SessionToken>) new System.Collections.Generic.List<SessionToken>();
        return (ActionResult) this.Json((object) new PersonalAccessTokenPageData()
        {
          PersonalAccessTokenDetailsModelList = (IList<PersonalAccessTokenDetailsModel>) pagedSessionTokens.SessionTokens.Select<SessionToken, PersonalAccessTokenDetailsModel>((Func<SessionToken, PersonalAccessTokenDetailsModel>) (x => new PersonalAccessTokenDetailsModel(x, userPreferences.DatePattern))).ToList<PersonalAccessTokenDetailsModel>(),
          NextRowNumber = pagedSessionTokens.NextRowNumber
        }, JsonRequestBehavior.AllowGet);
      }
      this.TfsRequestContext.TraceSerializedConditionally(505081, TraceLevel.Verbose, this.AreaName, this.TraceArea, "PAT pagination and filtering enabled: {0}, TokenPageRequest: [1}", (object) flag, (object) pageRequest);
      IList<SessionToken> source = service.ListSessionTokens(this.TfsRequestContext);
      return (ActionResult) this.Json((object) new PersonalAccessTokenPageData()
      {
        PersonalAccessTokenDetailsModelList = (IList<PersonalAccessTokenDetailsModel>) source.Select<SessionToken, PersonalAccessTokenDetailsModel>((Func<SessionToken, PersonalAccessTokenDetailsModel>) (x => new PersonalAccessTokenDetailsModel(x, userPreferences.DatePattern))).ToList<PersonalAccessTokenDetailsModel>(),
        NextRowNumber = 0
      }, JsonRequestBehavior.AllowGet);
    }

    [HttpDelete]
    [TfsBypassAntiForgeryValidation]
    [TfsTraceFilter(505051, 505060)]
    public ActionResult Revoke(string authorizationId)
    {
      Guid result;
      if (!Guid.TryParse(authorizationId, out result))
        throw new ArgumentException(AccountResources.InvalidGuidMessage);
      IDelegatedAuthorizationService service = this.TfsRequestContext.GetService<IDelegatedAuthorizationService>();
      try
      {
        service.RevokeSessionToken(this.TfsRequestContext, result);
      }
      catch (SessionTokenNotFoundException ex)
      {
        return (ActionResult) this.HttpStatusCodeWithMessage(HttpStatusCode.NotFound, ex.Message);
      }
      return (ActionResult) new HttpStatusCodeResult(200);
    }

    [HttpPatch]
    [TfsBypassAntiForgeryValidation]
    [TfsTraceFilter(505061, 505070)]
    public ActionResult Regenerate(string authorizationId)
    {
      Guid result;
      if (!Guid.TryParse(authorizationId, out result))
        throw new ArgumentException(AccountResources.InvalidGuidMessage);
      IDelegatedAuthorizationService service = this.TfsRequestContext.GetService<IDelegatedAuthorizationService>();
      SessionToken sessionToken = service.GetSessionToken(this.TfsRequestContext, result);
      DateTime dateTime = DateTime.UtcNow.AddDays((sessionToken.ValidTo - sessionToken.ValidFrom).TotalDays);
      IDelegatedAuthorizationService authorizationService = service;
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string str = HttpUtility.HtmlDecode(sessionToken.DisplayName);
      DateTime? nullable = new DateTime?(dateTime);
      string scope1 = sessionToken.Scope;
      IList<Guid> targetAccounts1 = sessionToken.TargetAccounts;
      Guid? clientId = new Guid?();
      Guid? userId = new Guid?();
      string name = str;
      DateTime? validTo = nullable;
      string scope2 = scope1;
      IList<Guid> targetAccounts2 = targetAccounts1;
      Guid? authorizationId1 = new Guid?();
      Guid? accessId = new Guid?();
      SessionTokenResult sessionTokenResult = authorizationService.IssueSessionToken(tfsRequestContext, clientId, userId, name, validTo, scope2, targetAccounts2, SessionTokenType.Compact, authorizationId: authorizationId1, accessId: accessId);
      if (sessionToken.IsValid)
        service.RevokeSessionToken(this.TfsRequestContext, sessionToken.AuthorizationId);
      return (ActionResult) this.Json((object) new PersonalAccessTokenDetailsModel(sessionTokenResult.SessionToken));
    }

    [HttpDelete]
    [TfsBypassAntiForgeryValidation]
    [TfsTraceFilter(505071, 505080)]
    public ActionResult RevokeAll()
    {
      IDelegatedAuthorizationService service = this.TfsRequestContext.GetService<IDelegatedAuthorizationService>();
      if (this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.Identity.PATPaginationAndFiltering"))
      {
        service.RevokeAllSessionTokensOfUser(this.TfsRequestContext);
      }
      else
      {
        foreach (SessionToken listSessionToken in (IEnumerable<SessionToken>) service.ListSessionTokens(this.TfsRequestContext))
        {
          if (listSessionToken.IsValid)
            service.RevokeSessionToken(this.TfsRequestContext, listSessionToken.AuthorizationId);
        }
      }
      return (ActionResult) new HttpStatusCodeResult(200);
    }
  }
}
