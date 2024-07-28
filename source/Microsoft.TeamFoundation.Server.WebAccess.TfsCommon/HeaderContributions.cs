// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TfsCommon.HeaderContributions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TfsCommon, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3690C2EA-1623-4663-B65B-BB4B63BFE368
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TfsCommon.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.TfsCommon
{
  public class HeaderContributions
  {
    private const int c_explicitPinnedHubGroupsOrderOffset = 100;

    public static void AddServerContributions(
      IVssRequestContext requestContext,
      HtmlHelper htmlHelper,
      IDictionary<string, IHtmlString> contributions)
    {
      HeaderContributions.AddLeftMenu(requestContext, htmlHelper, contributions);
      HeaderContributions.AddNavigationText(requestContext, htmlHelper, contributions);
      HeaderContributions.AddHubGroups(htmlHelper, contributions);
    }

    private static void AddLeftMenu(
      IVssRequestContext requestContext,
      HtmlHelper htmlHelper,
      IDictionary<string, IHtmlString> contributions)
    {
      ControlBase leftMenu = HeaderContributions.GetLeftMenu(htmlHelper);
      if (leftMenu == null)
        return;
      contributions["ms.vss-tfs-web.header-level1-project-selector"] = (IHtmlString) leftMenu.ToHtml(htmlHelper);
    }

    public static ControlBase GetLeftMenu(HtmlHelper htmlHelper) => (ControlBase) new LeftMenu(htmlHelper);

    private static void AddNavigationText(
      IVssRequestContext requestContext,
      HtmlHelper htmlHelper,
      IDictionary<string, IHtmlString> contributions)
    {
      contributions["ms.vss-tfs-web.header-level1-navigation-text"] = HeaderContributions.GetNavigationText(htmlHelper);
    }

    public static IHtmlString GetNavigationText(HtmlHelper htmlHelper)
    {
      string navigationDisplayText = htmlHelper.ViewContext.WebContext().GetNavigationDisplayText();
      TagBuilder tagBuilder = new TagBuilder("span");
      tagBuilder.Text(navigationDisplayText);
      tagBuilder.Attribute("title", navigationDisplayText);
      tagBuilder.AddClass("l1-navigation-text");
      return (IHtmlString) tagBuilder.ToHtmlString();
    }

    private static void AddHubGroups(
      HtmlHelper htmlHelper,
      IDictionary<string, IHtmlString> contributions)
    {
      ContainerControl containerControl = new ContainerControl("hub-selector", (IHtmlString) HeaderContributions.GetHubSelectorMenu(htmlHelper).ToHtml(htmlHelper));
      contributions["ms.vss-tfs-web.header-level1-hub-selector"] = (IHtmlString) containerControl.ToHtml(htmlHelper);
    }

    public static MenuBar GetHubSelectorMenu(HtmlHelper htmlHelper)
    {
      MenuBar hubSelectorMenu = ControlFactory.Create<MenuBar>();
      hubSelectorMenu.EnhancementCssClass = (string) null;
      hubSelectorMenu.CssClass<MenuBar>("bowtie-menus hubs-menubar l1-menubar");
      PageContext pageContext = WebContextFactory.GetPageContext(htmlHelper.ViewContext.RequestContext);
      Dictionary<string, HubGroup> dictionary = pageContext.HubsContext.HubGroups.ToDictionary<HubGroup, string>((Func<HubGroup, string>) (hg => hg.Id));
      string selectedHubGroupId = pageContext.HubsContext.SelectedHubGroupId;
      if (pageContext.HubsContext.PinningPreferences?.PinnedHubGroupIds != null)
      {
        for (int index = 0; index < pageContext.HubsContext.PinningPreferences.PinnedHubGroupIds.Count; ++index)
        {
          HubGroup hubGroup;
          if (dictionary.TryGetValue(pageContext.HubsContext.PinningPreferences.PinnedHubGroupIds[index], out hubGroup))
            hubGroup.Order += (double) (100 + index);
        }
      }
      bool flag = false;
      if (pageContext.HubsContext.PinningPreferences?.UnpinnedHubGroupIds != null)
      {
        foreach (string unpinnedHubGroupId in pageContext.HubsContext.PinningPreferences.UnpinnedHubGroupIds)
        {
          if (!string.Equals(unpinnedHubGroupId, selectedHubGroupId, StringComparison.Ordinal))
          {
            flag = true;
            break;
          }
        }
      }
      IEnumerable<MenuItem> hubGroupMenuItems1 = HeaderContributions.CreateHubGroupMenuItems(pageContext, selectedHubGroupId, true);
      IEnumerable<MenuItem> hubGroupMenuItems2 = HeaderContributions.CreateHubGroupMenuItems(pageContext, selectedHubGroupId, false);
      hubSelectorMenu.ChildItems.AddRange(hubGroupMenuItems1);
      if (!hubGroupMenuItems1.Any<MenuItem>((Func<MenuItem, bool>) (mi => mi.Selected)) && !hubGroupMenuItems2.Any<MenuItem>((Func<MenuItem, bool>) (mi => mi.Selected)) && !string.IsNullOrEmpty(selectedHubGroupId) && dictionary.ContainsKey(selectedHubGroupId))
      {
        HubGroup hubGroup = dictionary[selectedHubGroupId];
        if (!hubGroup.Hidden)
          hubSelectorMenu.AddMenuItem().Text(hubGroup.Name).CommandId(hubGroup.Id);
      }
      if (flag)
        hubSelectorMenu.AddMenuItem().ShowText(false).CssClass<MenuItem>("more-item").Icon = "menu-item-icon bowtie-icon bowtie-ellipsis";
      if (hubGroupMenuItems2.Any<MenuItem>())
      {
        hubSelectorMenu.AddSeparator();
        hubSelectorMenu.ChildItems.AddRange(hubGroupMenuItems2);
      }
      return hubSelectorMenu;
    }

    private static IEnumerable<MenuItem> CreateHubGroupMenuItems(
      PageContext pageContext,
      string selectedHubGroupId,
      bool collapsible)
    {
      return pageContext.HubsContext.HubGroups.Where<HubGroup>((Func<HubGroup, bool>) (hg =>
      {
        List<string> unpinnedHubGroupIds = pageContext.HubsContext.PinningPreferences?.UnpinnedHubGroupIds;
        if ((unpinnedHubGroupIds == null ? 0 : (unpinnedHubGroupIds.Contains(hg.Id) ? 1 : 0)) != 0 || string.IsNullOrEmpty(hg.Uri) || hg.Hidden)
          return false;
        if (collapsible && !hg.NonCollapsible)
          return true;
        return !collapsible && hg.NonCollapsible;
      })).OrderBy<HubGroup, double>((Func<HubGroup, double>) (hg => hg.Order)).Select<HubGroup, MenuItem>((Func<HubGroup, MenuItem>) (hg =>
      {
        MenuItem control = new MenuItem(hg.Name, hg.Id);
        control.Selected = string.Equals(hg.Id, selectedHubGroupId, StringComparison.OrdinalIgnoreCase);
        control.HideDrop = true;
        control.Href = hg.Uri;
        if (!string.IsNullOrEmpty(hg.Icon) && hg.NonCollapsible)
        {
          control.Icon = hg.Icon;
          control.Title = hg.Name;
          control.ShowText = false;
        }
        if (hg.BuiltIn && !string.IsNullOrEmpty(hg.Id))
          control.CssClass<MenuItem>(hg.Id.Replace(".", "-"));
        return control;
      }));
    }
  }
}
