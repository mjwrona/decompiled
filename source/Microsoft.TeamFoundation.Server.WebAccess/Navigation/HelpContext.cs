// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Navigation.HelpContext
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Controls;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess.Navigation
{
  [DataContract]
  public class HelpContext : MenuBarHeaderItemContext
  {
    public HelpContext(IVssRequestContext requestContext)
      : base(70)
    {
    }

    public override void AddServerContribution(
      HtmlHelper htmlHelper,
      IDictionary<string, IHtmlString> contributions)
    {
      MenuBar menuBar = ControlFactory.Create<MenuBar>();
      menuBar.EnhancementCssClass = (string) null;
      menuBar.CssClass<MenuBar>("bowtie-menus right-menu l1-menubar");
      this.PopulateMenuItem(htmlHelper.ViewContext.TfsWebContext(), menuBar.AddMenuItem());
      contributions["ms.vss-tfs-web.header-level1-right-menu"] = (IHtmlString) menuBar.ToHtml(htmlHelper);
    }

    public override void PopulateMenuItem(TfsWebContext webContext, MenuItem menuItem) => menuItem.ItemId("ellipsis").Icon("bowtie-icon bowtie-ellipsis").ShowText(false).AriaLabel(WACommonResources.HelpMenuLabel).CssClass<MenuItem>("ellipsis").HideDrop().AddExtraOptions((object) new
    {
      align = "right-bottom"
    }).AddMenuItem();

    public override void AddActions(IVssRequestContext requestContext)
    {
      foreach (HeaderAction allAction in HelpContext.GetAllActions(requestContext))
        this.AddAction(allAction.Id, allAction);
    }

    public static IEnumerable<HeaderAction> GetAllActions(IVssRequestContext requestContext) => new List<HeaderAction>()
    {
      HelpContext.GetWelcomeAction(requestContext),
      HelpContext.GetMsdnAction(requestContext),
      HelpContext.GetGettingStartedAction(requestContext),
      HelpContext.GetCommunityAction(requestContext),
      HelpContext.GetKeyboardShortcutsAction(requestContext),
      HelpContext.GetSupportAction(requestContext),
      HelpContext.GetPrivacyAction(requestContext),
      HelpContext.GetAboutAction(requestContext)
    }.Where<HeaderAction>((Func<HeaderAction, bool>) (a => a != null));

    public static HeaderAction GetWelcomeAction(IVssRequestContext requestContext)
    {
      HeaderAction welcomeAction = (HeaderAction) null;
      if (requestContext.ExecutionEnvironment.IsHostedDeployment)
        welcomeAction = new HeaderAction()
        {
          Id = "welcome",
          Text = WACommonResources.Header_WelcomePortal,
          Url = "https://go.microsoft.com/fwlink/?LinkId=245131"
        };
      return welcomeAction;
    }

    public static HeaderAction GetMsdnAction(IVssRequestContext requestContext) => new HeaderAction()
    {
      Id = "msdn",
      Text = WACommonResources.NavigationHelpMenuMsdn,
      Url = "https://go.microsoft.com/fwlink/?LinkId=256363"
    };

    public static HeaderAction GetGettingStartedAction(IVssRequestContext requestContext)
    {
      HeaderAction gettingStartedAction = (HeaderAction) null;
      if (requestContext.ExecutionEnvironment.IsHostedDeployment)
        gettingStartedAction = new HeaderAction()
        {
          Id = "gettingStarted",
          Text = WACommonResources.NavigationHelpMenuGettingStarted,
          Url = "https://go.microsoft.com/fwlink/?LinkID=235189"
        };
      return gettingStartedAction;
    }

    public static HeaderAction GetCommunityAction(IVssRequestContext requestContext) => new HeaderAction()
    {
      Id = "community",
      Text = WACommonResources.Header_Community,
      Url = "https://go.microsoft.com/fwlink/?LinkId=253552"
    };

    public static HeaderAction GetSupportAction(IVssRequestContext requestContext) => new HeaderAction()
    {
      Id = "support",
      Text = WACommonResources.Header_Support,
      Url = requestContext.ExecutionEnvironment.IsHostedDeployment ? "https://go.microsoft.com/fwlink/?LinkId=825593" : "https://go.microsoft.com/fwlink/?LinkId=825594"
    };

    public static HeaderAction GetKeyboardShortcutsAction(IVssRequestContext requestContext)
    {
      IContributionRoutingService service = requestContext.GetService<IContributionRoutingService>();
      HeaderAction keyboardShortcutsAction = (HeaderAction) null;
      IVssRequestContext requestContext1 = requestContext;
      if (service.GetRouteValue<string>(requestContext1, "project") != null)
        keyboardShortcutsAction = new HeaderAction()
        {
          Id = "keyboardShortcuts",
          Text = WACommonResources.Header_KeyboardShortcuts,
          CommandId = "keyboard-shortcuts"
        };
      return keyboardShortcutsAction;
    }

    public static HeaderAction GetPrivacyAction(IVssRequestContext requestContext)
    {
      HeaderAction privacyAction = (HeaderAction) null;
      if (requestContext.ExecutionEnvironment.IsHostedDeployment)
        privacyAction = new HeaderAction()
        {
          Id = "privacy",
          Text = WACommonResources.Header_Privacy,
          Url = "https://go.microsoft.com/fwlink/?LinkId=264782"
        };
      return privacyAction;
    }

    public static HeaderAction GetAboutAction(IVssRequestContext requestContext)
    {
      HeaderAction aboutAction = (HeaderAction) null;
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment && requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
      {
        IContributionRoutingService service = requestContext.GetService<IContributionRoutingService>();
        RouteValueDictionary routeValues = new RouteValueDictionary()
        {
          {
            "routeArea",
            (object) ""
          },
          {
            "project",
            (object) string.Empty
          },
          {
            "team",
            (object) string.Empty
          }
        };
        aboutAction = new HeaderAction()
        {
          Id = "about",
          Text = WACommonResources.Header_About
        };
        aboutAction.Url = service.RouteUrl(requestContext, "ServiceHostControllerAction", "About", "home", routeValues);
        aboutAction.TargetSelf = true;
      }
      return aboutAction;
    }
  }
}
