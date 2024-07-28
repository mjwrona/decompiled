// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Navigation.ProvideASuggestionContext
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Navigation
{
  [DataContract]
  public class ProvideASuggestionContext : MenuBarHeaderItemContext
  {
    public ProvideASuggestionContext(IVssRequestContext requestContext)
      : base(60)
    {
      this.Available = requestContext.IsHosted() && requestContext.IsFeatureEnabled("WebAccess.SendASmile.Navbar");
    }

    public override void AddServerContribution(
      HtmlHelper htmlHelper,
      IDictionary<string, IHtmlString> contributions)
    {
      MenuBar menuBar = ControlFactory.Create<MenuBar>();
      menuBar.EnhancementCssClass = (string) null;
      menuBar.CssClass<MenuBar>("bowtie-menus send-a-smile-menu l1-menubar");
      this.PopulateMenuItem(htmlHelper.ViewContext.TfsWebContext(), menuBar.AddMenuItem());
      contributions["ms.vss-tfs-web.header-level1-send-a-smile"] = (IHtmlString) menuBar.ToHtml(htmlHelper);
    }

    public override void PopulateMenuItem(TfsWebContext webContext, MenuItem menuItem)
    {
      if (webContext.IsFeatureAvailable("WebAccess.SendASmile.NewBehavior"))
        menuItem.ItemId("l1-customer-report").Icon("send-a-smile-icon bowtie-icon bowtie-feedback").ShowText(false).Title(WACommonResources.FeedbackMenuHint).HideDrop().AddExtraOptions((object) new
        {
          align = "right-bottom"
        }).AddMenuItem();
      else
        menuItem.ItemId("l1-send-a-smile").Icon("send-a-smile-icon bowtie-icon bowtie-feedback-positive-outline").ShowText(false).Title(WACommonResources.FeedbackMenuHint).HideDrop().AddExtraOptions((object) new
        {
          align = "right-bottom"
        }).AddMenuItem();
    }

    public static IEnumerable<HeaderAction> GetAllActions(IVssRequestContext requestContext) => new List<HeaderAction>()
    {
      ProvideASuggestionContext.GetSendASmileAction(requestContext),
      ProvideASuggestionContext.GetSendAFrownAction(requestContext)
    }.Where<HeaderAction>((Func<HeaderAction, bool>) (a => a != null));

    public static HeaderAction GetSendASmileAction(IVssRequestContext requestContext) => new HeaderAction()
    {
      Id = "sendASmile",
      Text = WebAccessServerResources.SendASmile,
      Icon = "bowtie-icon bowtie-feedback-positive-outline",
      CommandId = "ms.vss-tfs-web.send-smile"
    };

    public static HeaderAction GetSendAFrownAction(IVssRequestContext requestContext) => new HeaderAction()
    {
      Id = "sendAFrown",
      Text = WebAccessServerResources.SendAFrown,
      Icon = "bowtie-icon bowtie-feedback-negative-outline",
      CommandId = "ms.vss-tfs-web.send-frown"
    };
  }
}
