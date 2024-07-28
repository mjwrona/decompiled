// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Navigation.RightMenuBarContext
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Controls;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Navigation
{
  [DataContract]
  public class RightMenuBarContext : HeaderItemContext
  {
    private MenuBarHeaderItemContext[] m_items;

    public RightMenuBarContext(params MenuBarHeaderItemContext[] items)
      : base(70)
    {
      this.m_items = items;
    }

    public override void AddServerContribution(
      HtmlHelper htmlHelper,
      IDictionary<string, IHtmlString> contributions)
    {
      contributions["ms.vss-tfs-web.header-level1-right-menu-bar"] = this.GetServerHtml(htmlHelper);
    }

    private IHtmlString GetServerHtml(HtmlHelper htmlHelper)
    {
      TfsWebContext webContext = htmlHelper.ViewContext.TfsWebContext();
      MenuBar menuBar = ControlFactory.Create<MenuBar>();
      menuBar.EnhancementCssClass = (string) null;
      menuBar.CssClass<MenuBar>("bowtie-menus right-menu-bar l1-menubar");
      foreach (MenuBarHeaderItemContext headerItemContext in this.m_items)
      {
        if (headerItemContext.Available)
        {
          MenuItem menuItem = menuBar.AddMenuItem();
          headerItemContext.PopulateMenuItem(webContext, menuItem);
          menuItem.ClickOpensSubMenu = new bool?(false);
        }
      }
      return (IHtmlString) menuBar.ToHtml(htmlHelper);
    }

    public override void AddActions(IVssRequestContext requestContext)
    {
      foreach (MenuBarHeaderItemContext headerItemContext in this.m_items)
      {
        if (headerItemContext.Available)
        {
          headerItemContext.AddActions(requestContext);
          if (headerItemContext.Actions != null)
          {
            foreach (KeyValuePair<string, HeaderAction> action in (IEnumerable<KeyValuePair<string, HeaderAction>>) headerItemContext.Actions)
              this.AddAction(action.Key, action.Value);
          }
        }
      }
    }

    protected override IDictionary<string, object> GetExtraProperties(
      IVssRequestContext requestContext)
    {
      IDictionary<string, object> extraProperties = (IDictionary<string, object>) new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (MenuBarHeaderItemContext headerItemContext in this.m_items)
      {
        if (headerItemContext.Available)
        {
          headerItemContext.AddExtraProperties(requestContext);
          if (headerItemContext.Properties != null)
          {
            foreach (KeyValuePair<string, object> property in (IEnumerable<KeyValuePair<string, object>>) headerItemContext.Properties)
              extraProperties[property.Key] = property.Value;
          }
        }
      }
      return extraProperties;
    }
  }
}
