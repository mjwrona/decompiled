// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Navigation.UserSettingsContext
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Authentication;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.WebAccess.Controls;
using Microsoft.TeamFoundation.Server.WebAccess.TfsCommon;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.FeatureManagement;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.FeatureManagement;
using Microsoft.VisualStudio.Services.IdentityImage;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess.Navigation
{
  [DataContract]
  public class UserSettingsContext : MenuBarHeaderItemContext
  {
    private const string c_featureManagementFlag = "WebAccess.FeatureManagement.UI";
    private const string c_featuresDevModeCookieName = "features-dev-mode";
    private const string c_ieUserAgentString = "Trident/7.0";
    private bool m_anonymous;

    public UserSettingsContext(IVssRequestContext requestContext)
      : base(30)
    {
      this.m_anonymous = HeaderActionHelpers.HasClaim(requestContext, "anonymous");
      this.Available = true;
    }

    public override void AddServerContribution(
      HtmlHelper htmlHelper,
      IDictionary<string, IHtmlString> contributions)
    {
      MenuBar menuBar = ControlFactory.Create<MenuBar>();
      menuBar.EnhancementCssClass = (string) null;
      menuBar.CssClass<MenuBar>("bowtie-menus profile-menu l1-menubar");
      this.PopulateMenuItem(htmlHelper.ViewContext.TfsWebContext(), menuBar.AddMenuItem());
      contributions["ms.vss-tfs-web.header-level1-profile-menu"] = (IHtmlString) menuBar.ToHtml(htmlHelper);
    }

    public override void PopulateMenuItem(TfsWebContext webContext, MenuItem menuItem)
    {
      if (this.m_anonymous)
      {
        menuItem.Text(WebAccessServerResources.HeaderSignInText).Title(WebAccessServerResources.HeaderSignInTitle).ActionLink(UserSettingsContext.GetSignInUrl(webContext.TfsRequestContext));
      }
      else
      {
        UserContext userContext = UserSettingsContext.GetUserContext(webContext.TfsRequestContext);
        menuItem.ItemId("profile").CssClass<MenuItem>("profile").HideDrop().ShowIcon(false).AddExtraOptions((object) new
        {
          align = "right-bottom"
        });
        string displayName = userContext.DisplayName;
        string str = userContext.IsAcsAccount ? userContext.MailAddress : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}\\{1}", (object) userContext.Domain, (object) userContext.AccountName);
        if (!string.IsNullOrEmpty(str))
        {
          if (!string.IsNullOrEmpty(displayName))
            str = displayName + " (" + str + ")";
        }
        else
          str = displayName;
        menuItem.AriaLabel(str);
        menuItem.AddMenuItem();
        if (userContext.IdentityImage != null)
          menuItem.Html("<img class='profile-image' src='" + userContext.IdentityImage + "' alt='' />");
        else
          menuItem.Text(displayName);
      }
    }

    public override void AddActions(IVssRequestContext requestContext)
    {
      foreach (HeaderAction allAction in UserSettingsContext.GetAllActions(requestContext))
        this.AddAction(allAction.Id, allAction);
    }

    protected override IDictionary<string, object> GetExtraProperties(
      IVssRequestContext requestContext)
    {
      return (IDictionary<string, object>) new Dictionary<string, object>()
      {
        {
          "IdentityImageUri",
          (object) UserSettingsContext.GetCurrentUserIdentityImageUrl(requestContext)
        }
      };
    }

    public static IEnumerable<HeaderAction> GetAllActions(IVssRequestContext requestContext) => new List<HeaderAction>()
    {
      UserSettingsContext.GetUserAction(requestContext),
      new HeaderAction() { Id = "userDivider", Text = "-" },
      UserSettingsContext.GetProfileAction(requestContext),
      UserSettingsContext.GetSecurityAction(requestContext),
      UserSettingsContext.GetUsageAction(requestContext),
      UserSettingsContext.GetAlertsAction(requestContext),
      UserSettingsContext.GetFeatureManagementAction(requestContext),
      UserSettingsContext.GetThemesAction(requestContext),
      UserSettingsContext.GetHelpAction(requestContext),
      new HeaderAction() { Id = "divider", Text = "-" },
      UserSettingsContext.GetSignInAsAction(requestContext),
      UserSettingsContext.GetSignOutAction(requestContext)
    }.Where<HeaderAction>((Func<HeaderAction, bool>) (a => a != null));

    public static UserContext GetUserContext(IVssRequestContext requestContext)
    {
      UserContext userContext = new UserContext();
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      userContext.IsAcsAccount = userIdentity.Descriptor.IdentityType.Equals("Microsoft.IdentityModel.Claims.ClaimsIdentity", StringComparison.OrdinalIgnoreCase) || userIdentity.Descriptor.IdentityType.Equals("Microsoft.TeamFoundation.BindPendingIdentity", StringComparison.OrdinalIgnoreCase);
      userContext.DisplayName = userIdentity.DisplayName;
      userContext.MailAddress = userIdentity.GetProperty<string>("Mail", string.Empty);
      userContext.AccountName = userIdentity.GetProperty<string>("Account", string.Empty);
      userContext.Domain = userIdentity.GetProperty<string>("Domain", string.Empty);
      userContext.FormattedAccountName = string.IsNullOrEmpty(userContext.Domain) ? userContext.AccountName : string.Format("{0}\\{1}", (object) userContext.Domain, (object) userContext.AccountName);
      userContext.IdentityImage = UserSettingsContext.GetCurrentUserIdentityImageUrl(requestContext);
      return userContext;
    }

    public static HeaderAction GetUserAction(IVssRequestContext requestContext)
    {
      HeaderAction userAction = (HeaderAction) null;
      if (!HeaderActionHelpers.HasClaim(requestContext, "anonymous"))
      {
        UserContext userContext = UserSettingsContext.GetUserContext(requestContext);
        userAction = new HeaderAction()
        {
          ComponentType = "userMenuItem",
          Id = "user",
          Text = userContext.DisplayName,
          ItemType = 2,
          Title = userContext.IsAcsAccount ? userContext.MailAddress : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}\\{1}", (object) userContext.Domain, (object) userContext.AccountName),
          Url = userContext.IdentityImage
        };
      }
      return userAction;
    }

    public static HeaderAction GetProfileAction(IVssRequestContext requestContext)
    {
      HeaderAction profileAction = (HeaderAction) null;
      if (!HeaderActionHelpers.HasClaim(requestContext, "anonymous"))
      {
        requestContext.To(TeamFoundationHostType.Deployment);
        if (requestContext.ExecutionEnvironment.IsHostedDeployment)
        {
          string aexProfileUrl = UserSettingsContext.GetAexProfileUrl(requestContext);
          if (!string.IsNullOrEmpty(aexProfileUrl))
            profileAction = new HeaderAction()
            {
              Id = "profile",
              Text = WACommonResources.ManageProfile,
              Icon = "bowtie-icon bowtie-contact-card",
              Url = aexProfileUrl,
              Title = WACommonResources.ManageProfile
            };
        }
        else
          profileAction = new HeaderAction()
          {
            Id = "profile",
            Text = WACommonResources.ManageProfile,
            Icon = "bowtie-icon bowtie-contact-card",
            CommandId = "manageUserProfile",
            Command = new HeaderCommand()
            {
              MethodName = "show",
              ServiceName = "LegacyProfileDialogService",
              Dependencies = new string[1]
              {
                "ms.vss-tfs-web.header-async-content"
              }
            }
          };
      }
      return profileAction;
    }

    public static HeaderAction GetSecurityAction(IVssRequestContext requestContext)
    {
      HeaderAction securityAction = (HeaderAction) null;
      if (HeaderActionHelpers.HasClaim(requestContext, "member") && !requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        IContributionRoutingService service = requestContext.GetService<IContributionRoutingService>();
        string str = service.RouteUrl(requestContext, "ms.vss-tfs-web.user-settings-tokens-default-route");
        if (string.IsNullOrEmpty(str))
          str = service.RouteUrl(requestContext, "Personal_access_token_default", new RouteValueDictionary()
          {
            {
              "action",
              (object) "Index"
            },
            {
              "controller",
              (object) "PersonalAccessToken"
            },
            {
              "routeArea",
              (object) ""
            },
            {
              "serviceHost",
              (object) requestContext.ServiceHost
            },
            {
              "project",
              (object) string.Empty
            },
            {
              "team",
              (object) string.Empty
            }
          });
        securityAction = new HeaderAction()
        {
          Id = "security",
          Text = WACommonResources.ManageSecurityPreferences,
          Icon = "bowtie-icon bowtie-security-access",
          TargetSelf = true,
          Url = str
        };
      }
      return securityAction;
    }

    public static HeaderAction GetUsageAction(IVssRequestContext requestContext)
    {
      HeaderAction usageAction = (HeaderAction) null;
      if (HeaderActionHelpers.HasClaim(requestContext, "member") && requestContext.ExecutionEnvironment.IsHostedDeployment && !requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        string str = requestContext.GetService<IContributionRoutingService>().RouteUrl(requestContext, "ms.vss-admin-web.user-admin-hub-route", new RouteValueDictionary()
        {
          {
            "adminPivot",
            (object) "usage"
          }
        });
        usageAction = new HeaderAction()
        {
          Id = "usage",
          Text = Resources.ManageUsage(),
          Icon = "bowtie-icon bowtie-chart-stacked-line",
          Url = str
        };
      }
      return usageAction;
    }

    public static HeaderAction GetFeatureManagementAction(IVssRequestContext requestContext)
    {
      HeaderAction managementAction = (HeaderAction) null;
      if (HeaderActionHelpers.HasClaim(requestContext, "member") && requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) && requestContext.IsFeatureEnabled("WebAccess.FeatureManagement.UI"))
      {
        bool flag = UserSettingsContext.GetFeatureManagementDevMode();
        if (!flag)
        {
          IContributedFeatureService service = requestContext.GetService<IContributedFeatureService>();
          string featuresContributionId = TfsFeatureManagementConstants.GetManagedFeaturesContributionId(requestContext);
          IVssRequestContext requestContext1 = requestContext;
          string contributionId = featuresContributionId;
          flag = service.GetFeaturesForTarget(requestContext1, contributionId).Any<ContributedFeature>();
        }
        if (flag)
          managementAction = new HeaderAction()
          {
            Id = "manageFeatures",
            CommandId = "manageFeatures",
            Text = requestContext.ExecutionEnvironment.IsHostedDeployment ? WebAccessServerResources.ManageFeatures : WebAccessServerResources.ManageFeaturesOnPrem,
            Title = WebAccessServerResources.ManageFeaturesTooltip,
            Icon = "bowtie-icon bowtie-giftbox-open",
            Command = new HeaderCommand()
            {
              MethodName = "show",
              ServiceName = "IFeaturePanel",
              Dependencies = new string[1]
              {
                "ms.vss-tfs-web.feature-management-content"
              }
            }
          };
      }
      return managementAction;
    }

    public static HeaderAction GetHelpAction(IVssRequestContext requestContext) => new HeaderAction()
    {
      Id = "help",
      Text = WebAccessServerResources.Help,
      Title = WebAccessServerResources.Help,
      Icon = "bowtie-icon bowtie-status-help",
      MenuId = "ms.vss-tfs-web.help-menu"
    };

    public static HeaderAction GetThemesAction(IVssRequestContext requestContext)
    {
      HeaderAction themesAction = (HeaderAction) null;
      requestContext.GetService<IContributedFeatureService>();
      if (HeaderActionHelpers.HasClaim(requestContext, "member") && requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) && requestContext.UserAgent != null && requestContext.UserAgent.IndexOf("Trident/7.0", StringComparison.OrdinalIgnoreCase) == -1)
        themesAction = new HeaderAction()
        {
          Id = "themes",
          Text = WebAccessServerResources.ManageThemesText,
          Title = WebAccessServerResources.ManageThemesTitle,
          FabricIconName = "Color",
          Command = new HeaderCommand()
          {
            MethodName = "show",
            ServiceName = "IThemePanelService",
            Dependencies = new string[2]
            {
              "ms.vss-tfs-web.theme-management-content",
              "ms.vss-tfs-web.available-themes-data-provider"
            }
          }
        };
      return themesAction;
    }

    public static bool GetFeatureManagementDevMode()
    {
      bool managementDevMode = false;
      HttpCookie cookie = HttpContext.Current.Request.Cookies["features-dev-mode"];
      if (cookie != null)
        managementDevMode = "true".Equals(cookie.Value, StringComparison.OrdinalIgnoreCase);
      return managementDevMode;
    }

    public static HeaderAction GetAlertsAction(IVssRequestContext requestContext)
    {
      HeaderAction alertsAction = (HeaderAction) null;
      if (HeaderActionHelpers.HasClaim(requestContext, "member"))
      {
        if (requestContext.IsFeatureEnabled("Notifications.Web.NotificationsUI"))
        {
          if (requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
          {
            IContributionRoutingService service = requestContext.GetService<IContributionRoutingService>();
            alertsAction = new HeaderAction()
            {
              Id = "alerts",
              Text = WACommonResources.ManageNotifications,
              TargetSelf = true,
              Icon = "bowtie-icon bowtie-notification",
              Url = service.RouteUrl(requestContext, "ms.vss-notifications-web.user-notifications-route")
            };
          }
        }
        else if (requestContext.GetService<IContributionRoutingService>().GetRouteValue<string>(requestContext, "project") != null && requestContext.FeatureContext().AreStandardFeaturesAvailable && WebContextFactory.GetPageContext(requestContext).WebAccessConfiguration.MailSettings.Enabled)
          alertsAction = new HeaderAction()
          {
            Id = "alerts",
            Text = WACommonResources.ManageAlerts,
            CommandId = "manageAlerts"
          };
      }
      return alertsAction;
    }

    public static HeaderAction GetSignInAsAction(IVssRequestContext requestContext)
    {
      HeaderAction signInAsAction = (HeaderAction) null;
      if (HeaderActionHelpers.HasClaim(requestContext, "member") && !requestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        string input = requestContext.RawUrl();
        Regex regex = new Regex("&*__rt=fps(&__ver=\\d+)*");
        if (regex.IsMatch(input))
          input = regex.Replace(input, "");
        IContributionRoutingService service = requestContext.GetService<IContributionRoutingService>();
        signInAsAction = new HeaderAction()
        {
          Id = "signInAs",
          Text = WACommonResources.SignInAsDifferentUser,
          Url = service.RouteUrl(requestContext, "ServiceHostControllerAction", "index", "signout", new RouteValueDictionary()
          {
            {
              "routeArea",
              (object) ""
            },
            {
              "serviceHost",
              (object) requestContext.ServiceHost.OrganizationServiceHost
            },
            {
              "mode",
              (object) "SignInAsDifferentUser"
            },
            {
              "redirectUrl",
              (object) input
            },
            {
              "project",
              (object) string.Empty
            },
            {
              "team",
              (object) string.Empty
            }
          })
        };
      }
      return signInAsAction;
    }

    public static HeaderAction GetSignOutAction(
      IVssRequestContext requestContext,
      IAadAuthenticationSessionTokenConfiguration sessionTokenConfiguration = null)
    {
      HeaderAction signOutAction = (HeaderAction) null;
      if (!HeaderActionHelpers.HasClaim(requestContext, "anonymous"))
      {
        if (requestContext.IsHosted() && (sessionTokenConfiguration ?? (IAadAuthenticationSessionTokenConfiguration) new AadAuthenticationSessionTokenConfiguration()).IssueAadAuthenticationCookieEnabled(requestContext))
          signOutAction = new HeaderAction()
          {
            Id = "signOut",
            Text = WACommonResources.SignOut,
            CommandId = "msaljsSignOut",
            Command = new HeaderCommand()
            {
              MethodName = "signOut",
              ServiceName = "ITfsMsalInteractiveService",
              Dependencies = new string[1]
              {
                "ms.vss-tfs-web.tfs-signout-url-data-provider"
              }
            }
          };
        else
          signOutAction = new HeaderAction()
          {
            Id = "signOut",
            Text = WACommonResources.SignOut,
            Url = UserSettingsContext.GetSignOutUrl(requestContext)
          };
      }
      return signOutAction;
    }

    public static string GetSignInUrl(IVssRequestContext requestContext)
    {
      string str1 = requestContext.GetService<IContributionRoutingService>().GetUrl(requestContext);
      string path = requestContext.VirtualPath();
      if (!string.IsNullOrEmpty(str1) && !string.IsNullOrEmpty(path))
      {
        string pathIfNeeded1 = UriUtility.AppendSlashToPathIfNeeded(str1);
        string pathIfNeeded2 = UriUtility.AppendSlashToPathIfNeeded(path);
        string str2 = pathIfNeeded2;
        if (pathIfNeeded1.StartsWith(str2, StringComparison.OrdinalIgnoreCase))
          str1 = str1.Substring(pathIfNeeded2.Length - 1);
      }
      ILocationService service = requestContext.GetService<ILocationService>();
      AccessMapping accessMapping = service.DetermineAccessMapping(requestContext);
      Uri replyToOverride = new Uri(service.LocationForAccessMapping(requestContext, str1, RelativeToSetting.Context, accessMapping));
      return requestContext.GetService<ITeamFoundationAuthenticationService>().GetSignInRedirectLocation(requestContext, replyToOverride: replyToOverride);
    }

    public static string GetSignOutUrl(IVssRequestContext requestContext)
    {
      if (requestContext.IsHosted())
      {
        if (requestContext.IsDevOpsDomainRequest())
          return requestContext.GetService<ITeamFoundationAuthenticationService>().BuildHostedSignOutUrl(requestContext);
        IContributionRoutingService service = requestContext.GetService<IContributionRoutingService>();
        string str = (string) null;
        IVssRequestContext requestContext1 = requestContext;
        return service.RouteUrl(requestContext1, "ServiceHostControllerAction", "index", "signout", new RouteValueDictionary()
        {
          {
            "routeArea",
            (object) ""
          },
          {
            "serviceHost",
            (object) requestContext.ServiceHost
          },
          {
            "redirectUrl",
            (object) str
          },
          {
            "project",
            (object) string.Empty
          },
          {
            "team",
            (object) string.Empty
          }
        });
      }
      IContributionRoutingService service1 = requestContext.GetService<IContributionRoutingService>();
      string str1 = requestContext.RawUrl();
      IVssRequestContext requestContext2 = requestContext;
      return service1.RouteUrl(requestContext2, "ServiceHostControllerAction", "index", "signout", new RouteValueDictionary()
      {
        {
          "routeArea",
          (object) ""
        },
        {
          "serviceHost",
          (object) requestContext.ServiceHost.OrganizationServiceHost
        },
        {
          "redirectUrl",
          (object) str1
        },
        {
          "project",
          (object) string.Empty
        },
        {
          "team",
          (object) string.Empty
        }
      });
    }

    private static string GetCurrentUserIdentityImageUrl(IVssRequestContext requestContext)
    {
      string identityImageUrl = (string) null;
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment || !requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        IdentityImageService identityImageService = IdentityImageServiceUtil.GetIdentityImageService(requestContext);
        RouteValueDictionary routeValues = new RouteValueDictionary();
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
        IVssRequestContext requestContext1 = requestContext;
        Guid id = userIdentity.Id;
        Guid? cachedIdentityImageId = identityImageService.GetCachedIdentityImageId(requestContext1, id);
        routeValues["routeArea"] = (object) "Api";
        routeValues["size"] = (object) 0;
        routeValues["id"] = (object) userIdentity.Id;
        routeValues["project"] = (object) string.Empty;
        routeValues["team"] = (object) string.Empty;
        routeValues["t"] = cachedIdentityImageId.HasValue ? (object) cachedIdentityImageId.Value : (object) DateTime.Now.Ticks;
        identityImageUrl = requestContext.GetService<IContributionRoutingService>().RouteUrl(requestContext, "ApiServiceHostControllerAction", "identityImage", "common", routeValues);
      }
      return identityImageUrl;
    }

    public static string GetAexProfileUrl(IVssRequestContext requestContext)
    {
      string aexProfileUrl = (string) null;
      try
      {
        IVssRequestContext deploymentTfsRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        string locationUrlWithTimeout = UserSettingsContext.GetAexLocationUrlWithTimeout(deploymentTfsRequestContext, AccessMappingConstants.AffinitizedMultiInstanceAccessMappingMoniker);
        if (string.IsNullOrWhiteSpace(locationUrlWithTimeout))
          locationUrlWithTimeout = UserSettingsContext.GetAexLocationUrlWithTimeout(deploymentTfsRequestContext, AccessMappingConstants.ClientAccessMappingMoniker);
        if (!string.IsNullOrWhiteSpace(locationUrlWithTimeout))
        {
          UriBuilder uriBuilder = new UriBuilder(locationUrlWithTimeout);
          uriBuilder.Path = "go/profile/";
          NameValueCollection queryString = HttpUtility.ParseQueryString(string.Empty);
          queryString["campaign"] = "o~msft~vsts~usercard";
          uriBuilder.Query = queryString.ToString();
          aexProfileUrl = uriBuilder.ToString();
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(504250, nameof (GetAexProfileUrl), nameof (UserSettingsContext), ex);
      }
      return aexProfileUrl;
    }

    public static string GetAexAuthRegisterAppUrl(IVssRequestContext requestContext)
    {
      string authRegisterAppUrl = (string) null;
      try
      {
        IVssRequestContext deploymentTfsRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        string locationUrlWithTimeout = UserSettingsContext.GetAexLocationUrlWithTimeout(deploymentTfsRequestContext, AccessMappingConstants.AffinitizedMultiInstanceAccessMappingMoniker);
        if (string.IsNullOrWhiteSpace(locationUrlWithTimeout))
          locationUrlWithTimeout = UserSettingsContext.GetAexLocationUrlWithTimeout(deploymentTfsRequestContext, AccessMappingConstants.ClientAccessMappingMoniker);
        if (!string.IsNullOrWhiteSpace(locationUrlWithTimeout))
          authRegisterAppUrl = new UriBuilder(locationUrlWithTimeout)
          {
            Path = "app/register/"
          }.ToString();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(504252, nameof (GetAexAuthRegisterAppUrl), nameof (UserSettingsContext), ex);
      }
      return authRegisterAppUrl;
    }

    private static string GetAexLocationUrlWithTimeout(
      IVssRequestContext deploymentTfsRequestContext,
      string mapping)
    {
      ILocationService service = deploymentTfsRequestContext.GetService<ILocationService>();
      using (deploymentTfsRequestContext.CreateAsyncTimeOutScope(UserSettingsContext.GetAexLocationTimeout(deploymentTfsRequestContext)))
        return service.GetLocationServiceUrl(deploymentTfsRequestContext, AexServiceConstants.ServiceInstanceTypeId, mapping);
    }

    private static TimeSpan GetAexLocationTimeout(IVssRequestContext requestContext) => TimeSpan.FromMilliseconds((double) requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/Profile/AexLocation/Timeout", 2000));
  }
}
