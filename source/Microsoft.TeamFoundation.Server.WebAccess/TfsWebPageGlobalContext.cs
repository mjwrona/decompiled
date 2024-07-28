// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TfsWebPageGlobalContext
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  [DataContract]
  public class TfsWebPageGlobalContext : PageContext
  {
    public TfsWebPageGlobalContext(WebContext webContext)
      : base(webContext)
    {
    }

    protected override HubsContext CreateHubsContext(WebContext webContext)
    {
      TfsWebContext tfsWebContext = webContext as TfsWebContext;
      if (tfsWebContext == null)
        return base.CreateHubsContext(webContext);
      HubsContext hubsContext = new HubsContext();
      hubsContext.HubGroups = new List<HubGroup>();
      hubsContext.Hubs = new List<Hub>();
      hubsContext.PopulateFromContributions((WebContext) tfsWebContext, (Func<Contribution, bool>) (hubContribution => TfsWebPageGlobalContext.LicenseCheckFilter(tfsWebContext, hubContribution)));
      this.AddPinningPreferences(tfsWebContext, hubsContext);
      return hubsContext;
    }

    private void AddPinningPreferences(TfsWebContext tfsWebContext, HubsContext hubsContext)
    {
      if (tfsWebContext.CurrentIdentity == null || !hubsContext.HubGroupsUnpinnedByDefault)
        return;
      ISettingsService service = tfsWebContext.TfsRequestContext.GetService<ISettingsService>();
      hubsContext.PinningPreferences = service.GetValue<PinningPreferences>(tfsWebContext.TfsRequestContext, SettingsUserScope.User, "Navigation/PinningPreferences", new PinningPreferences(), false);
      PinningPreferences pinningPreferences = hubsContext.PinningPreferences;
      if (pinningPreferences.PinnedHubGroupIds == null)
        pinningPreferences.PinnedHubGroupIds = new List<string>();
      if (pinningPreferences.UnpinnedHubGroupIds == null)
        pinningPreferences.UnpinnedHubGroupIds = new List<string>();
      foreach (HubGroup hubGroup in hubsContext.HubGroups.Where<HubGroup>((Func<HubGroup, bool>) (hg => !hg.BuiltIn && !hg.Hidden && !string.IsNullOrEmpty(hg.Uri))))
      {
        if (!pinningPreferences.PinnedHubGroupIds.Contains(hubGroup.Id) && !pinningPreferences.UnpinnedHubGroupIds.Contains(hubGroup.Id))
          pinningPreferences.UnpinnedHubGroupIds.Add(hubGroup.Id);
      }
    }

    public static bool LicenseCheckFilter(TfsWebContext tfsWebContext, Contribution hubContribution)
    {
      Guid[] property = hubContribution.GetProperty<Guid[]>("licensedFeatures");
      if (property != null)
      {
        foreach (Guid featureId in property)
        {
          if (!tfsWebContext.FeatureContext.IsFeatureAvailable(featureId))
            return false;
        }
      }
      return true;
    }
  }
}
