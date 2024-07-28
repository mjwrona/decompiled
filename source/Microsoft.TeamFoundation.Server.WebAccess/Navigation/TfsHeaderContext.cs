// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Navigation.TfsHeaderContext
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Navigation
{
  [DataContract]
  public class TfsHeaderContext : WebSdkMetadata
  {
    private const string c_contextKey = "__tfsHeaderContext";

    [DataMember(EmitDefaultValue = false)]
    public IDictionary<string, HeaderItemContext> RightMenu { get; private set; }

    public TfsHeaderContext(IVssRequestContext requestContext)
    {
      int num = HeaderPermissionChecks.UserHasAdminSettingsPermissions(requestContext) ? 1 : 0;
      this.RightMenu = (IDictionary<string, HeaderItemContext>) new Dictionary<string, HeaderItemContext>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
      {
        {
          "search",
          (HeaderItemContext) new SearchContext(requestContext)
        }
      };
      this.RightMenu.Add("adminSettings", (HeaderItemContext) new AdminSettingsContext(requestContext));
      UserSettingsContext userSettingsContext = new UserSettingsContext(requestContext);
      ExtensionsContext extensionsContext = new ExtensionsContext(num != 0);
      ProvideASuggestionContext asuggestionContext = new ProvideASuggestionContext(requestContext);
      HelpContext helpContext = new HelpContext(requestContext);
      this.RightMenu.Add("rightMenuBar", (HeaderItemContext) new RightMenuBarContext(new MenuBarHeaderItemContext[4]
      {
        (MenuBarHeaderItemContext) extensionsContext,
        (MenuBarHeaderItemContext) asuggestionContext,
        (MenuBarHeaderItemContext) userSettingsContext,
        (MenuBarHeaderItemContext) helpContext
      }));
    }

    public void AddServerContributions(
      HtmlHelper htmlHelper,
      IDictionary<string, IHtmlString> contributions)
    {
      string[] strArray = new string[6]
      {
        "rightMenuBar",
        "help",
        "userSettings",
        "sendASmile",
        "search",
        "extensions"
      };
      foreach (string key in strArray)
      {
        HeaderItemContext headerItemContext;
        if (this.RightMenu.TryGetValue(key, out headerItemContext) && headerItemContext.Available)
          headerItemContext.AddServerContribution(htmlHelper, contributions);
      }
    }

    public void AddRightMenuActions(IVssRequestContext requestContext)
    {
      foreach (HeaderItemContext headerItemContext in this.RightMenu.Values.Where<HeaderItemContext>((Func<HeaderItemContext, bool>) (ctx => ctx.Available)))
      {
        headerItemContext.AddActions(requestContext);
        headerItemContext.AddExtraProperties(requestContext);
      }
    }

    public static TfsHeaderContext GetHeaderContext(IVssRequestContext requestContext)
    {
      object headerContext1;
      if (requestContext.Items.TryGetValue("__tfsHeaderContext", out headerContext1))
        return headerContext1 as TfsHeaderContext;
      TfsHeaderContext headerContext2 = new TfsHeaderContext(requestContext);
      headerContext2.AddRightMenuActions(requestContext);
      requestContext.Items["__tfsHeaderContext"] = (object) headerContext2;
      return headerContext2;
    }
  }
}
