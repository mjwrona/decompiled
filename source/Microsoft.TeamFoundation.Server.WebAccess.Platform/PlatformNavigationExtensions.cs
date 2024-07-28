// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.PlatformNavigationExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.CSharp.RuntimeBinder;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Contributions.HtmlContent;
using Microsoft.TeamFoundation.Server.WebAccess.Controls;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.FeatureManagement;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public static class PlatformNavigationExtensions
  {
    public static MvcHtmlString RenderUserMenu(this HtmlHelper htmlHelper)
    {
      WebContext webContext = htmlHelper.ViewContext.WebContext();
      UserContext user = webContext.User;
      bool flag = user == null;
      if (!flag)
      {
        // ISSUE: reference to a compiler-generated field
        if (PlatformNavigationExtensions.\u003C\u003Eo__0.\u003C\u003Ep__1 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PlatformNavigationExtensions.\u003C\u003Eo__0.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, bool>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (bool), typeof (PlatformNavigationExtensions)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, bool> target = PlatformNavigationExtensions.\u003C\u003Eo__0.\u003C\u003Ep__1.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, bool>> p1 = PlatformNavigationExtensions.\u003C\u003Eo__0.\u003C\u003Ep__1;
        // ISSUE: reference to a compiler-generated field
        if (PlatformNavigationExtensions.\u003C\u003Eo__0.\u003C\u003Ep__0 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PlatformNavigationExtensions.\u003C\u003Eo__0.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "SkipUserMenu", typeof (PlatformNavigationExtensions), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj = PlatformNavigationExtensions.\u003C\u003Eo__0.\u003C\u003Ep__0.Target((CallSite) PlatformNavigationExtensions.\u003C\u003Eo__0.\u003C\u003Ep__0, htmlHelper.ViewContext.ViewBag) ?? (object) false;
        flag = target((CallSite) p1, obj);
      }
      if (flag)
        return MvcHtmlString.Empty;
      MenuBar menuBase = ControlFactory.Create<MenuBar>().CssClass<MenuBar>("top-level-menu-v2 user-menu header-item").ShowIcon<MenuBar>(false).PopupAlign<MenuBar>("right-bottom");
      string mailAddress = user.Email;
      if (mailAddress == null)
        mailAddress = string.Empty;
      menuBase.AddMenuItem().Text(user.Name).Title(mailAddress).CommandId("user").IdIsAction(false).TextClass("alignment-marker").AddChildMenuOptions((object) new
      {
        alignToMarkerHorizontal = true
      }).Scope<MenuItem>((Action<MenuItem>) (menuItem =>
      {
        menuItem.AddMenuItem().CssClass<MenuItem>("identity-image").Disabled(true).Html(htmlHelper.IdentityPicture().ToHtmlString());
        menuItem.AddMenuItem().Text(WACommonResources.ManageProfile).TextClass("alignment-marker").CssClass<MenuItem>("my-profile").ActionLink(webContext.Url.LocAwareAction("view", "profile"));
        menuItem.AddMenuItem().Text(WACommonResources.SignOut).TextClass("alignment-marker").CssClass<MenuItem>("sign-out").ActionLink(webContext.Url.LocAwareAction("index", "_signout"));
        menuItem.AddLabel().Text(mailAddress).Title(mailAddress).CssClass<MenuItem>("user-email");
      }));
      return menuBase.ToHtml(htmlHelper);
    }

    public static MvcHtmlString IdentityPicture(this HtmlHelper htmlHelper, ImageSize imageSize = ImageSize.Large)
    {
      WebContext webContext = htmlHelper.ViewContext.WebContext();
      RouteValueDictionary routeValues = new RouteValueDictionary()
      {
        {
          "identityId",
          (object) webContext.CurrentIdentity.Id
        },
        {
          "size",
          (object) (int) imageSize
        }
      };
      return new TagBuilder("img").AddClass("identity-picture").Attribute("src", webContext.Url.Action("avatar", "profile", routeValues)).ToHtmlString();
    }

    public static bool IsContributedFeatureEnabled(
      this WebContext webContext,
      string contributionId)
    {
      return webContext.TfsRequestContext.GetService<IContributedFeatureService>().IsFeatureEnabled(webContext.TfsRequestContext, contributionId);
    }

    public static IHtmlString RenderHeader(this HtmlHelper htmlHelper)
    {
      PageContext pageContext = WebContextFactory.GetPageContext(htmlHelper.ViewContext.RequestContext);
      WebContext webContext = pageContext.WebContext;
      using (WebPerformanceTimerHelpers.StartMeasure(webContext, "NavigationExtensions.RenderHeaderContributions"))
      {
        string collectionContributionId = pageContext.HubsContext.HubGroupsCollectionContributionId;
        IDictionary<string, ContributionNode> dictionary = webContext.TfsRequestContext.GetService<IContributionService>().QueryContributions(webContext.TfsRequestContext, (IEnumerable<string>) new string[1]
        {
          collectionContributionId
        }, ContributionQueryOptions.IncludeAll, (ContributionQueryCallback) ((requestContext, contribution, parentContribution, relationship, queryOptions, evaluatedConditions) => string.Equals(contribution.Type, "ms.vss-web.hub-group", StringComparison.OrdinalIgnoreCase) && !string.Equals(contribution.Id, pageContext.HubsContext.SelectedHubGroupId, StringComparison.OrdinalIgnoreCase) || string.Equals(contribution.Type, "ms.vss-web.hub", StringComparison.OrdinalIgnoreCase) && !string.Equals(contribution.Id, pageContext.HubsContext.SelectedHubId, StringComparison.OrdinalIgnoreCase) ? ContributionQueryOptions.None : ContributionQueryOptions.IncludeAll));
        ContributionNode targetContribution = dictionary.Values.Where<ContributionNode>((Func<ContributionNode, bool>) (c => c.Contribution.IsOfType("ms.vss-web.header"))).FirstOrDefault<ContributionNode>();
        if (targetContribution != null)
          return HtmlContentProviderHelpers.GetRenderedHtml(new ContributionHtmlProviderContext()
          {
            HtmlHelper = htmlHelper,
            SelectedHubGroup = dictionary.Values.FirstOrDefault<ContributionNode>((Func<ContributionNode, bool>) (c => string.Equals(c.Contribution.Type, "ms.vss-web.hub-group", StringComparison.OrdinalIgnoreCase))),
            SelectedHub = dictionary.Values.FirstOrDefault<ContributionNode>((Func<ContributionNode, bool>) (c => string.Equals(c.Contribution.Type, "ms.vss-web.hub", StringComparison.OrdinalIgnoreCase)))
          }, targetContribution);
      }
      return (IHtmlString) MvcHtmlString.Empty;
    }
  }
}
