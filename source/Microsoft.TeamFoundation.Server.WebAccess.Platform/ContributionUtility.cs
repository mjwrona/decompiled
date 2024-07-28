// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.ContributionUtility
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public static class ContributionUtility
  {
    private const string c_moduleContentContributionType = "ms.vss-web.module-content";

    public static ISet<string> GetRequiredModules(IEnumerable<Contribution> contributions)
    {
      HashSet<string> requiredModules = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (Contribution contribution in contributions.Where<Contribution>((Func<Contribution, bool>) (c => !new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
      {
        "ms.vss-web.page-event-subscription"
      }.Contains(c.Type))))
      {
        JObject container = (JObject) null;
        if (string.Equals(contribution.Type, "ms.vss-web.module-content", StringComparison.OrdinalIgnoreCase))
          container = contribution.Properties;
        else if (contribution.Properties != null)
          container = contribution.GetProperty<JToken>("content") as JObject;
        if (container != null)
        {
          string[] other = (string[]) null;
          bool flag;
          if (container.TryGetValue<string[]>("require", out other) && (!container.TryGetValue<bool>("delayLoad", out flag) || !flag))
            requiredModules.UnionWith((IEnumerable<string>) other);
        }
      }
      return (ISet<string>) requiredModules;
    }
  }
}
