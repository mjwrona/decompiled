// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ProjectAnalysis.Server.TfsLanguageConfiguratorBase
// Assembly: Microsoft.TeamFoundation.ProjectAnalysis.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 076482BC-74A4-4A35-9427-1E61C33D1FA6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.ProjectAnalysis.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.ProjectAnalysis.Server
{
  internal abstract class TfsLanguageConfiguratorBase : ILanguageConfigurator
  {
    public const string Unknown = "Unknown";
    private readonly IEnumerable<LanguageConfigurationPattern> m_pathExclusionDefaults;
    private bool m_caseSensitivity;
    private const string c_layer = "TfsLanguageConfiguratorBase";

    public TfsLanguageConfiguratorBase(
      IVssRequestContext requestContext,
      Stream content,
      bool caseSensitivity = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      this.TfsRequestContext = requestContext;
      this.m_caseSensitivity = caseSensitivity;
      this.m_pathExclusionDefaults = LanguageConfigurationDefaults.s_excludedPath.Select<string, LanguageConfigurationPattern>((Func<string, LanguageConfigurationPattern>) (p => new LanguageConfigurationPattern(p, this.m_caseSensitivity)));
      this.InitializeOverrides(content);
    }

    protected void InitializeOverrides(Stream content)
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      ISet<string> source1 = (ISet<string>) new HashSet<string>();
      ISet<string> source2 = (ISet<string>) new HashSet<string>();
      if (content != null)
      {
        using (StreamReader streamReader = new StreamReader(content))
        {
          string line;
          while ((line = streamReader.ReadLine()) != null)
          {
            if (this.IsNotComment(line) && !string.IsNullOrWhiteSpace(line))
            {
              string[] config;
              if (this.TryParseLine(line, this.LanguageNameAttributes, out config))
              {
                dictionary.TryAdd<string, string>(config[0], config[1]);
                source2.Add(config[0]);
              }
              else if (this.TryParseLine(line, this.PathExclusionAttributes, out config))
              {
                if ("=true".Equals(config[1], StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty(config[1]))
                  source1.Add(config[0]);
                else
                  source2.Add(config[0]);
              }
            }
          }
        }
      }
      this.NameOverrides = (IEnumerable<KeyValuePair<LanguageConfigurationPattern, string>>) dictionary.ToDictionary<KeyValuePair<string, string>, LanguageConfigurationPattern, string>((Func<KeyValuePair<string, string>, LanguageConfigurationPattern>) (e => new LanguageConfigurationPattern(e.Key, this.m_caseSensitivity)), (Func<KeyValuePair<string, string>, string>) (e => e.Value));
      this.PathExclusionOverrides = source1.Select<string, LanguageConfigurationPattern>((Func<string, LanguageConfigurationPattern>) (i => new LanguageConfigurationPattern(i, this.m_caseSensitivity)));
      this.PathInclusionOverrides = source2.Select<string, LanguageConfigurationPattern>((Func<string, LanguageConfigurationPattern>) (i => new LanguageConfigurationPattern(i, this.m_caseSensitivity)));
    }

    public virtual bool ShouldExclude(string filePath)
    {
      if (this.PathInclusionOverrides != null && this.PathInclusionOverrides.Any<LanguageConfigurationPattern>((Func<LanguageConfigurationPattern, bool>) (s => s.TryMatches(this.TfsRequestContext, filePath, false))))
        return false;
      if (this.PathExclusionOverrides != null && this.PathExclusionOverrides.Any<LanguageConfigurationPattern>((Func<LanguageConfigurationPattern, bool>) (s => s.TryMatches(this.TfsRequestContext, filePath, true))) || this.m_pathExclusionDefaults.Any<LanguageConfigurationPattern>((Func<LanguageConfigurationPattern, bool>) (s => s.TryMatches(this.TfsRequestContext, filePath, true))))
        return true;
      string languageExtension = filePath.GetLanguageExtension();
      return LanguageConfigurationDefaults.s_excludedExtensions.Contains(languageExtension);
    }

    public virtual string GetLanguageName(string filePath)
    {
      if (this.NameOverrides != null)
      {
        foreach (KeyValuePair<LanguageConfigurationPattern, string> nameOverride in this.NameOverrides)
        {
          if (nameOverride.Key.Matches(filePath))
            return nameOverride.Value;
        }
      }
      string languageExtension = filePath.GetLanguageExtension();
      if (string.IsNullOrWhiteSpace(languageExtension))
        return "Unknown";
      string str;
      return !LanguageConfigurationDefaults.s_extToLanguageNames.TryGetValue(languageExtension, out str) ? languageExtension : str;
    }

    protected bool IsNotComment(string line) => !line.Trim().StartsWith("#");

    protected IVssRequestContext TfsRequestContext { get; private set; }

    protected abstract bool TryParseLine(string line, string[] v, out string[] config);

    protected abstract string[] LanguageNameAttributes { get; }

    protected abstract string[] PathExclusionAttributes { get; }

    protected IEnumerable<KeyValuePair<LanguageConfigurationPattern, string>> NameOverrides { get; set; }

    protected IEnumerable<LanguageConfigurationPattern> PathInclusionOverrides { get; set; }

    protected IEnumerable<LanguageConfigurationPattern> PathExclusionOverrides { get; set; }
  }
}
