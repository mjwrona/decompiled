// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.ThemingService
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.FeatureManagement;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Settings;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server
{
  internal class ThemingService : IThemingService, IVssFrameworkService
  {
    private static readonly RegistryQuery s_themeQuery = new RegistryQuery("/Configuration/WebAccess/Theme");
    private const string c_requestedThemeKey = "theming.requestedTheme";
    private const string c_defaultTheme = "ms.vss-web.vsts-theme";
    private const string c_ieUserAgentString = "Trident/7.0";
    private static readonly HashSet<string> s_themeContributionTypes = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      "ms.vss-web.theme"
    };
    private static readonly IEnumerable<string> s_themesRootTargets = (IEnumerable<string>) new string[1]
    {
      "ms.vss-web.vsts-theme-root"
    };

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public string GetRequestedThemeId(IVssRequestContext requestContext)
    {
      IContributionManagementService service1 = requestContext.GetService<IContributionManagementService>();
      string requestedThemeId = (string) null;
      string str;
      if (requestContext.GetSessionValue("THEME", out str))
        requestedThemeId = str;
      if (requestContext.UserAgent == null || requestContext.UserAgent.IndexOf("Trident/7.0", StringComparison.OrdinalIgnoreCase) != -1)
        requestedThemeId = "ms.vss-web.vsts-theme";
      if (string.IsNullOrEmpty(requestedThemeId))
      {
        ISettingsService service2 = requestContext.GetService<ISettingsService>();
        requestedThemeId = service2.GetValue<string>(requestContext, SettingsUserScope.GlobalUser, "WebPlatform/Theme", (string) null, false);
        if (string.IsNullOrEmpty(requestedThemeId))
          requestedThemeId = service2.GetValue<string>(requestContext, SettingsUserScope.User, "WebPlatform/Theme", (string) null, false);
      }
      if (string.IsNullOrEmpty(requestedThemeId))
      {
        ContributedSite site = service1.GetSite(requestContext);
        if (site != null)
          requestedThemeId = site.DefaultTheme;
      }
      if (string.IsNullOrEmpty(requestedThemeId))
        requestedThemeId = requestContext.GetService<IVssRegistryService>().GetValue<string>(requestContext, in ThemingService.s_themeQuery, "ms.vss-web.vsts-theme");
      return requestedThemeId;
    }

    public ClientTheme GetRequestedTheme(IVssRequestContext requestContext)
    {
      object requestedTheme1;
      if (requestContext.Items.TryGetValue("theming.requestedTheme", out requestedTheme1))
        return requestedTheme1 as ClientTheme;
      string requestedThemeId = this.GetRequestedThemeId(requestContext);
      ClientTheme requestedTheme2 = this.GetTheme(requestContext, requestedThemeId) ?? this.GetTheme(requestContext, "ms.vss-web.vsts-theme");
      requestContext.Items["theming.requestedTheme"] = (object) requestedTheme2;
      return requestedTheme2;
    }

    public ClientTheme GetTheme(IVssRequestContext requestContext, string themeContributionId)
    {
      ClientTheme theme = (ClientTheme) null;
      requestContext.GetService<IContributedFeatureService>();
      Contribution contribution = requestContext.GetService<IContributionService>().QueryContribution(requestContext, themeContributionId);
      if (contribution != null)
      {
        IEnumerable<ContributionNode> contributionsByType = requestContext.GetService<IContributionManagementService>().GetContributionsByType(requestContext, "ms.vss-web.theme-mapping");
        theme = this.GetTheme(requestContext, contribution.Id, contributionsByType != null ? contributionsByType.Select<ContributionNode, Contribution>((Func<ContributionNode, Contribution>) (c => c.Contribution)) : (IEnumerable<Contribution>) null);
      }
      return theme;
    }

    private ContributedTheme GetThemeFromContributionId(
      IVssRequestContext requestContext,
      string contributionId)
    {
      ContributedTheme fromContributionId = (ContributedTheme) null;
      Contribution contribution = requestContext.GetService<IContributionService>().QueryContribution(requestContext, contributionId);
      if (contribution != null)
      {
        fromContributionId = contribution.GetAssociatedObject<ContributedTheme>("theme");
        if (fromContributionId == null)
        {
          fromContributionId = new ContributedTheme(contribution);
          contribution.SetAssociatedObject<ContributedTheme>("theme", fromContributionId);
        }
      }
      return fromContributionId;
    }

    public IEnumerable<ClientTheme> GetThemes(IVssRequestContext requestContext)
    {
      List<ClientTheme> source = new List<ClientTheme>();
      IContributionService service = requestContext.GetService<IContributionService>();
      IEnumerable<Contribution> contributions = service.QueryContributions(requestContext, ThemingService.s_themesRootTargets, ThemingService.s_themeContributionTypes, ContributionQueryOptions.IncludeSubTree);
      IEnumerable<Contribution> themeMappingContributions = service.QueryContributionsForType(requestContext, "ms.vss-web.theme-mapping");
      if (contributions != null)
      {
        foreach (Contribution contribution in contributions)
        {
          ClientTheme theme = this.GetTheme(requestContext, contribution.Id, themeMappingContributions);
          if (theme != null)
            source.Add(theme);
        }
      }
      return (IEnumerable<ClientTheme>) source.OrderBy<ClientTheme, string>((Func<ClientTheme, string>) (t => t.Name));
    }

    private ClientTheme GetTheme(
      IVssRequestContext requestContext,
      string themeId,
      IEnumerable<Contribution> themeMappingContributions)
    {
      ClientTheme theme = (ClientTheme) null;
      ContributedTheme fromContributionId1 = this.GetThemeFromContributionId(requestContext, themeId);
      if (fromContributionId1 != null)
      {
        theme = new ClientTheme()
        {
          Id = themeId,
          Name = fromContributionId1.Name,
          Extends = fromContributionId1.Extends,
          IsDark = fromContributionId1.Dark,
          IsPreview = fromContributionId1.Preview,
          Data = new WebSdkMetadataDictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
        };
        Stack<ContributedTheme> contributedThemeStack = new Stack<ContributedTheme>();
        for (; fromContributionId1 != null; fromContributionId1 = this.GetThemeFromContributionId(requestContext, fromContributionId1.Extends))
        {
          contributedThemeStack.Push(fromContributionId1);
          if (string.IsNullOrEmpty(fromContributionId1.Extends))
            break;
        }
        List<ContributedTheme> source = new List<ContributedTheme>();
        while (contributedThemeStack.Count > 0)
        {
          ContributedTheme contributedTheme1 = contributedThemeStack.Pop();
          source.Clear();
          if (contributedTheme1.Data != null)
            this.MergeDictionaries<string, string>((IDictionary<string, string>) theme.Data, (IDictionary<string, string>) contributedTheme1.Data);
          if (themeMappingContributions != null)
          {
            foreach (Contribution mappingContribution in themeMappingContributions)
            {
              ContributedTheme fromContributionId2 = this.GetThemeFromContributionId(requestContext, mappingContribution.Id);
              if (string.Equals(contributedTheme1.Contribution.Id, fromContributionId2.Extends, StringComparison.OrdinalIgnoreCase))
                source.Add(fromContributionId2);
            }
          }
          foreach (ContributedTheme contributedTheme2 in (IEnumerable<ContributedTheme>) source.OrderBy<ContributedTheme, string>((Func<ContributedTheme, string>) (mapping => mapping.Key)))
          {
            if (contributedTheme2.Data != null)
              this.MergeDictionaries<string, string>((IDictionary<string, string>) theme.Data, (IDictionary<string, string>) contributedTheme2.Data);
          }
        }
        IContributionService service = requestContext.GetService<IContributionService>();
        Dictionary<string, string>.KeyCollection keys = theme.Data.Keys;
        for (int index = 0; index < keys.Count; ++index)
        {
          string key = keys.ElementAt<string>(index);
          string str = theme.Data[key];
          int num1 = str.IndexOf("url(");
          if (num1 >= 0)
          {
            int startIndex = num1 + 4;
            int num2 = str.IndexOf(")", startIndex);
            if (num2 >= 0)
            {
              string oldValue = str.Substring(startIndex, num2 - startIndex);
              string[] strArray = oldValue.Split(new char[1]
              {
                ':'
              }, 2);
              if (strArray.Length != 0)
              {
                string contributionId;
                string assetType;
                if (strArray.Length == 2)
                {
                  contributionId = strArray[0];
                  assetType = strArray[1];
                }
                else
                {
                  contributionId = themeId;
                  assetType = strArray[0];
                }
                string newValue = service.QueryAssetLocation(requestContext, contributionId, assetType);
                theme.Data[key] = str.Replace(oldValue, newValue);
              }
            }
          }
        }
      }
      return theme;
    }

    private void MergeDictionaries<TKey, TValue>(
      IDictionary<TKey, TValue> target,
      IDictionary<TKey, TValue> toMerge)
    {
      foreach (KeyValuePair<TKey, TValue> keyValuePair in (IEnumerable<KeyValuePair<TKey, TValue>>) toMerge)
        target[keyValuePair.Key] = keyValuePair.Value;
    }
  }
}
