// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers.LanguageMetrics
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.ProjectAnalysis.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers
{
  public class LanguageMetrics
  {
    private IList<string> m_projectDominantLanguages;
    private IDictionary<Guid, IList<string>> m_repositoryLanguages;

    public LanguageMetrics(
      IVssRequestContext requestContext,
      ProjectLanguageAnalytics projectLanguageAnalytics)
    {
      if (projectLanguageAnalytics == null)
        return;
      if (projectLanguageAnalytics.LanguageBreakdown != null)
      {
        double configValueOrDefault1 = requestContext.GetConfigValueOrDefault("/Service/OrgSearch/Settings/ProjectLanguagePercentageThreshold", 10.0);
        int configValueOrDefault2 = requestContext.GetConfigValueOrDefault("/Service/OrgSearch/Settings/ProjectDominantLanguagesCount", 3);
        this.m_projectDominantLanguages = LanguageMetrics.GetLanguages(projectLanguageAnalytics.LanguageBreakdown, configValueOrDefault1, configValueOrDefault2);
      }
      if (projectLanguageAnalytics.RepositoryLanguageAnalytics == null)
        return;
      double configValueOrDefault = requestContext.GetConfigValueOrDefault("/Service/OrgSearch/Settings/RepositoryLanguagePercentageThreshold", 0.0);
      this.m_repositoryLanguages = (IDictionary<Guid, IList<string>>) new Dictionary<Guid, IList<string>>();
      foreach (RepositoryLanguageAnalytics languageAnalytic in (IEnumerable<RepositoryLanguageAnalytics>) projectLanguageAnalytics.RepositoryLanguageAnalytics)
        this.m_repositoryLanguages.Add(languageAnalytic.Id, LanguageMetrics.GetLanguages(languageAnalytic.LanguageBreakdown, configValueOrDefault));
    }

    internal LanguageMetrics(IDictionary<Guid, string> repoLanguageMapping)
    {
      this.m_projectDominantLanguages = (IList<string>) new HashSet<string>((IEnumerable<string>) repoLanguageMapping.Values).ToList<string>();
      this.m_repositoryLanguages = (IDictionary<Guid, IList<string>>) new Dictionary<Guid, IList<string>>();
      foreach (KeyValuePair<Guid, string> keyValuePair in (IEnumerable<KeyValuePair<Guid, string>>) repoLanguageMapping)
        this.m_repositoryLanguages.Add(keyValuePair.Key, (IList<string>) new List<string>()
        {
          keyValuePair.Value
        });
    }

    public string[] GetProjectDominantLanguages() => this.m_projectDominantLanguages == null ? Array.Empty<string>() : this.m_projectDominantLanguages.ToArray<string>();

    public string[] GetRepositoryLanguages(Guid repositoryId)
    {
      IList<string> source = (IList<string>) null;
      if (this.m_repositoryLanguages != null)
        this.m_repositoryLanguages.TryGetValue(repositoryId, out source);
      return source != null ? source.ToArray<string>() : Array.Empty<string>();
    }

    private static IList<string> GetLanguages(
      IList<LanguageStatistics> languageBreakdown,
      double languagePercentageBar = 0.0,
      int numberOfLanguages = 2147483647)
    {
      IDictionary<string, double> source = (IDictionary<string, double>) new Dictionary<string, double>();
      if (languageBreakdown != null)
      {
        foreach (LanguageStatistics languageStatistics in (IEnumerable<LanguageStatistics>) languageBreakdown)
        {
          if (languageStatistics.LanguagePercentage.HasValue)
          {
            double? languagePercentage = languageStatistics.LanguagePercentage;
            double num = languagePercentageBar;
            if (languagePercentage.GetValueOrDefault() >= num & languagePercentage.HasValue)
              source.Add(languageStatistics.Name, languageStatistics.LanguagePercentage.Value);
          }
        }
      }
      return (IList<string>) source.OrderByDescending<KeyValuePair<string, double>, double>((Func<KeyValuePair<string, double>, double>) (x => x.Value)).Take<KeyValuePair<string, double>>(numberOfLanguages).Select<KeyValuePair<string, double>, string>((Func<KeyValuePair<string, double>, string>) (x => x.Key)).ToList<string>();
    }
  }
}
