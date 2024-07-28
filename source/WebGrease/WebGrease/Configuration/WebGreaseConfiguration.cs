// Decompiled with JetBrains decompiler
// Type: WebGrease.Configuration.WebGreaseConfiguration
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using WebGrease.Extensions;

namespace WebGrease.Configuration
{
  public class WebGreaseConfiguration
  {
    private static readonly Regex EnvironmentVariablesMatchPattern = new Regex("%(?<name>[a-zA-Z]*?)%", RegexOptions.Compiled);
    private static readonly TimeSpan MinimumCacheTimeout = TimeSpan.FromHours(1.0);
    private readonly Dictionary<string, GlobalConfig> global = new Dictionary<string, GlobalConfig>();

    internal WebGreaseConfiguration()
    {
      this.global = new Dictionary<string, GlobalConfig>();
      this.Global = new GlobalConfig();
      this.ImageExtensions = (IList<string>) new List<string>();
      this.ImageDirectories = (IList<string>) new List<string>();
      this.ImageDirectoriesToHash = (IList<string>) new List<string>();
      this.CssFileSets = (IList<CssFileSet>) new List<CssFileSet>();
      this.JSFileSets = (IList<JSFileSet>) new List<JSFileSet>();
      this.DefaultDpi = (IDictionary<string, HashSet<float>>) new Dictionary<string, HashSet<float>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.DefaultPreprocessing = (IDictionary<string, PreprocessingConfig>) new Dictionary<string, PreprocessingConfig>();
      this.DefaultJSMinification = (IDictionary<string, JsMinificationConfig>) new Dictionary<string, JsMinificationConfig>();
      this.DefaultSpriting = (IDictionary<string, CssSpritingConfig>) new Dictionary<string, CssSpritingConfig>();
      this.DefaultCssMinification = (IDictionary<string, CssMinificationConfig>) new Dictionary<string, CssMinificationConfig>();
      this.DefaultBundling = (IDictionary<string, BundlingConfig>) new Dictionary<string, BundlingConfig>();
      this.DefaultCssResourcePivots = new ResourcePivotGroupCollection();
      this.DefaultJsResourcePivots = new ResourcePivotGroupCollection();
      this.LoadedConfigurationFiles = (IList<string>) new List<string>();
    }

    internal WebGreaseConfiguration(string configType, string preprocessingPluginPath = null)
      : this()
    {
      this.ConfigType = configType;
      this.PreprocessingPluginPath = preprocessingPluginPath;
    }

    internal WebGreaseConfiguration(
      WebGreaseConfiguration configuration,
      FileInfo configurationFile)
      : this(configurationFile, configuration.ConfigType, configuration.SourceDirectory, configuration.DestinationDirectory, configuration.LogsDirectory, configuration.ToolsTempDirectory, configuration.ApplicationRootDirectory, configuration.PreprocessingPluginPath)
    {
      this.CacheEnabled = configuration.CacheEnabled;
      this.CacheRootPath = configuration.CacheRootPath;
      this.CacheTimeout = configuration.CacheTimeout;
      this.CacheUniqueKey = configuration.CacheUniqueKey;
      this.Measure = configuration.Measure;
      this.Overrides = configuration.Overrides;
      this.ReportPath = configuration.ReportPath;
    }

    internal WebGreaseConfiguration(WebGreaseConfiguration configuration)
      : this(configuration.ConfigType, configuration.SourceDirectory, configuration.DestinationDirectory, configuration.LogsDirectory, configuration.ToolsTempDirectory, configuration.ApplicationRootDirectory, configuration.PreprocessingPluginPath)
    {
      this.CacheEnabled = configuration.CacheEnabled;
      this.CacheRootPath = configuration.CacheRootPath;
      this.CacheTimeout = configuration.CacheTimeout;
      this.CacheUniqueKey = configuration.CacheUniqueKey;
      this.Measure = configuration.Measure;
      this.Overrides = configuration.Overrides;
      this.ReportPath = configuration.ReportPath;
    }

    internal WebGreaseConfiguration(
      FileInfo configurationFile,
      string configType,
      string sourceDirectory,
      string destinationDirectory,
      string logsDirectory,
      string toolsTempDirectory = null,
      string appRootDirectory = null,
      string preprocessingPluginPath = null)
      : this(configType, sourceDirectory, destinationDirectory, logsDirectory, toolsTempDirectory, appRootDirectory, preprocessingPluginPath)
    {
      if (configurationFile == null)
        throw new ArgumentNullException(nameof (configType));
      this.Parse(configurationFile.FullName);
    }

    internal WebGreaseConfiguration(
      string configType,
      string sourceDirectory,
      string destinationDirectory,
      string logsDirectory,
      string toolsTempDirectory,
      string appRootDirectory = null,
      string preprocessingPluginPath = null)
      : this(configType, preprocessingPluginPath)
    {
      this.SourceDirectory = sourceDirectory;
      this.DestinationDirectory = destinationDirectory;
      this.LogsDirectory = logsDirectory;
      this.ToolsTempDirectory = toolsTempDirectory;
      this.ApplicationRootDirectory = appRootDirectory ?? Environment.CurrentDirectory;
      this.IntermediateErrorDirectory = Path.Combine(this.ApplicationRootDirectory, "IntermediateErrorFiles");
      if (!string.IsNullOrWhiteSpace(destinationDirectory))
        Directory.CreateDirectory(destinationDirectory);
      if (string.IsNullOrWhiteSpace(logsDirectory))
        return;
      Directory.CreateDirectory(logsDirectory);
    }

    public string SourceDirectory { get; set; }

    internal IEnumerable<string> AllLoadedConfigurationFiles => this.LoadedConfigurationFiles.Concat<string>(this.CssFileSets.SelectMany<CssFileSet, string>((Func<CssFileSet, IEnumerable<string>>) (cfs => (IEnumerable<string>) cfs.LoadedConfigurationFiles)).Concat<string>(this.JSFileSets.SelectMany<JSFileSet, string>((Func<JSFileSet, IEnumerable<string>>) (cfs => (IEnumerable<string>) cfs.LoadedConfigurationFiles)))).Distinct<string>();

    internal GlobalConfig Global { get; private set; }

    internal string ConfigType { get; private set; }

    internal string DestinationDirectory { get; set; }

    internal string TokensDirectory { get; set; }

    internal string OverrideTokensDirectory { get; private set; }

    internal string ApplicationRootDirectory { get; private set; }

    internal string LogsDirectory { get; set; }

    internal string ReportPath { get; set; }

    internal string ToolsTempDirectory { get; private set; }

    internal string PreprocessingPluginPath { get; private set; }

    internal IList<string> ImageDirectories { get; private set; }

    internal IList<string> ImageDirectoriesToHash { get; private set; }

    internal IList<string> ImageExtensions { get; set; }

    internal IList<CssFileSet> CssFileSets { get; private set; }

    internal IList<JSFileSet> JSFileSets { get; private set; }

    internal IList<string> LoadedConfigurationFiles { get; private set; }

    internal bool Measure { get; set; }

    internal string DefaultOutputPathFormat { get; set; }

    internal bool CacheEnabled { get; set; }

    internal string CacheRootPath { get; set; }

    internal string CacheUniqueKey { get; set; }

    internal TimeSpan CacheTimeout { get; set; }

    internal string IntermediateErrorDirectory { get; set; }

    internal IDictionary<string, HashSet<float>> DefaultDpi { get; set; }

    internal TemporaryOverrides Overrides { get; set; }

    internal ResourcePivotGroupCollection DefaultCssResourcePivots { get; set; }

    internal ResourcePivotGroupCollection DefaultJsResourcePivots { get; set; }

    private IDictionary<string, JsMinificationConfig> DefaultJSMinification { get; set; }

    private IDictionary<string, CssMinificationConfig> DefaultCssMinification { get; set; }

    private IDictionary<string, BundlingConfig> DefaultBundling { get; set; }

    private IDictionary<string, CssSpritingConfig> DefaultSpriting { get; set; }

    private IDictionary<string, PreprocessingConfig> DefaultPreprocessing { get; set; }

    internal static void AddSeperatedValues(
      IList<string> list,
      string seperatedValues,
      Func<string, string> action = null)
    {
      if (string.IsNullOrWhiteSpace(seperatedValues))
        return;
      foreach (string str1 in seperatedValues.SafeSplitSemiColonSeperatedValue())
      {
        string str2 = str1.Trim();
        list.Add(action != null ? action(str2) : str2);
      }
    }

    internal static void ForEachConfigSourceElement(
      XElement parentElement,
      string parentFilePath,
      Action<XElement, string> configSourceAction)
    {
      List<string> list = parentElement.Elements((XName) "ConfigSource").Select<XElement, string>((Func<XElement, string>) (e => (string) e)).ToList<string>();
      list.Add((string) parentElement.Attribute((XName) "configSource"));
      foreach (string path2 in list.Where<string>((Func<string, bool>) (cs => !cs.IsNullOrWhitespace())))
      {
        string fullPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(parentFilePath), path2));
        if (!File.Exists(fullPath))
          throw new ConfigurationErrorsException("Configuration file not found: {0}, referenced in : {1}".InvariantFormat((object) fullPath, (object) parentFilePath));
        try
        {
          configSourceAction(XDocument.Load(fullPath).Root, fullPath);
        }
        catch (Exception ex)
        {
          throw new ConfigurationErrorsException("Could not load configuration file: {0}, references in {1}".InvariantFormat((object) path2, (object) parentFilePath), ex);
        }
      }
    }

    internal void Validate()
    {
      this.ApplicationRootDirectory = WebGreaseConfiguration.EnsureAndExpandDirectory(this.ApplicationRootDirectory, false);
      this.DestinationDirectory = WebGreaseConfiguration.EnsureAndExpandDirectory(this.DestinationDirectory, false);
      this.SourceDirectory = WebGreaseConfiguration.EnsureAndExpandDirectory(this.SourceDirectory, false);
      this.PreprocessingPluginPath = WebGreaseConfiguration.EnsureAndExpandDirectory(this.PreprocessingPluginPath, false);
      this.LogsDirectory = WebGreaseConfiguration.EnsureAndExpandDirectory(this.LogsDirectory, true);
      this.CacheRootPath = WebGreaseConfiguration.EnsureAndExpandDirectory(this.CacheRootPath, true);
      this.ToolsTempDirectory = WebGreaseConfiguration.EnsureAndExpandDirectory(this.ToolsTempDirectory, true);
      this.ReportPath = WebGreaseConfiguration.EnsureAndExpandDirectory(this.ReportPath ?? this.LogsDirectory, true);
      if (!(this.CacheTimeout > TimeSpan.Zero) || !(this.CacheTimeout < WebGreaseConfiguration.MinimumCacheTimeout))
        return;
      this.CacheTimeout = WebGreaseConfiguration.MinimumCacheTimeout;
    }

    private static string EnsureAndExpandDirectory(string directory, bool allowCreate)
    {
      if (string.IsNullOrWhiteSpace(directory))
        return (string) null;
      directory = WebGreaseConfiguration.EnvironmentVariablesMatchPattern.Replace(directory, (MatchEvaluator) (match => Environment.GetEnvironmentVariable(match.Groups["name"].Value)));
      DirectoryInfo directoryInfo = new DirectoryInfo(directory);
      if (!directoryInfo.Exists)
      {
        if (!allowCreate)
          throw new DirectoryNotFoundException(directory);
        directoryInfo.Create();
      }
      return directoryInfo.FullName;
    }

    private void Parse(string configurationFile) => this.Parse(XElement.Load(configurationFile), configurationFile);

    private void Parse(XElement element, string configurationFile)
    {
      this.ParseSettings(element.Descendants((XName) "Settings"), configurationFile);
      this.Global = this.global.GetNamedConfig<GlobalConfig>(this.ConfigType);
      foreach (XElement descendant in element.Descendants((XName) "CssFileSet"))
        this.CssFileSets.Add(new CssFileSet(descendant, this.SourceDirectory, this.DefaultCssMinification, this.DefaultSpriting, this.DefaultPreprocessing, this.DefaultBundling, this.DefaultCssResourcePivots, this.Global, this.DefaultOutputPathFormat, this.DefaultDpi, configurationFile));
      foreach (XElement descendant in element.Descendants((XName) "JsFileSet"))
        this.JSFileSets.Add(new JSFileSet(descendant, this.SourceDirectory, this.DefaultJSMinification, this.DefaultPreprocessing, this.DefaultBundling, this.DefaultJsResourcePivots, this.Global, this.DefaultOutputPathFormat, configurationFile));
    }

    private void ParseSettings(IEnumerable<XElement> settingsElements, string configurationFile)
    {
      foreach (XElement settingsElement in settingsElements.Where<XElement>((Func<XElement, bool>) (e => e != null)))
        this.ParseSettings(settingsElement, configurationFile);
    }

    private void ParseSettings(XElement settingsElement, string configurationFile)
    {
      if (settingsElement == null)
        throw new ArgumentNullException(nameof (settingsElement));
      WebGreaseConfiguration.ForEachConfigSourceElement(settingsElement, configurationFile, (Action<XElement, string>) ((element, s) =>
      {
        this.ParseSettings(element, s);
        this.LoadedConfigurationFiles.Add(s);
      }));
      foreach (XElement descendant in settingsElement.Descendants())
      {
        string str = descendant.Name.ToString();
        string seperatedValues = descendant.Value;
        switch (str)
        {
          case "ImageDirectories":
            WebGreaseConfiguration.AddSeperatedValues(this.ImageDirectories, seperatedValues, (Func<string, string>) (value => Path.GetFullPath(Path.Combine(this.SourceDirectory, value))));
            continue;
          case "ImageDirectoriesToHash":
            WebGreaseConfiguration.AddSeperatedValues(this.ImageDirectoriesToHash, seperatedValues, (Func<string, string>) (value => Path.GetFullPath(Path.Combine(this.SourceDirectory, value))));
            continue;
          case "ImageExtensions":
            WebGreaseConfiguration.AddSeperatedValues(this.ImageExtensions, seperatedValues);
            continue;
          case "Dpi":
            IEnumerable<float> collection = seperatedValues.NullSafeAction<string, IEnumerable<string>>(new Func<string, IEnumerable<string>>(StringExtensions.SafeSplitSemiColonSeperatedValue)).Select<string, float?>((Func<string, float?>) (d => d.TryParseFloat())).Where<float?>((Func<float?, bool>) (d => d.HasValue)).Select<float?, float>((Func<float?, float>) (d => d.Value));
            this.DefaultDpi[((string) descendant.Attribute((XName) "output")).AsNullIfWhiteSpace() ?? string.Empty] = new HashSet<float>(collection);
            continue;
          case "TokensDirectory":
            this.TokensDirectory = seperatedValues;
            continue;
          case "OutputPathFormat":
            this.DefaultOutputPathFormat = seperatedValues;
            continue;
          case "OverrideTokensDirectory":
            this.OverrideTokensDirectory = seperatedValues;
            continue;
          case "Locales":
            this.DefaultCssResourcePivots.Set("locales", new ResourcePivotApplyMode?(ResourcePivotApplyMode.ApplyAsStringReplace), seperatedValues.NullSafeAction<string, IEnumerable<string>>((Func<string, IEnumerable<string>>) (sv => sv.SafeSplitSemiColonSeperatedValue())));
            this.DefaultJsResourcePivots.Set("locales", new ResourcePivotApplyMode?(ResourcePivotApplyMode.ApplyAsStringReplace), seperatedValues.NullSafeAction<string, IEnumerable<string>>((Func<string, IEnumerable<string>>) (sv => sv.SafeSplitSemiColonSeperatedValue())));
            continue;
          case "Themes":
            this.DefaultCssResourcePivots.Set("themes", new ResourcePivotApplyMode?(ResourcePivotApplyMode.ApplyAsStringReplace), seperatedValues.NullSafeAction<string, IEnumerable<string>>((Func<string, IEnumerable<string>>) (sv => sv.SafeSplitSemiColonSeperatedValue())));
            continue;
          case "ResourcePivot":
            this.DefaultJsResourcePivots.Set((string) descendant.Attribute((XName) "key"), new ResourcePivotApplyMode?((ResourcePivotApplyMode) ((int) ((string) descendant.Attribute((XName) "applyMode")).TryParseToEnum<ResourcePivotApplyMode>() ?? 0)), ((string) descendant).NullSafeAction<string, IEnumerable<string>>((Func<string, IEnumerable<string>>) (sv => sv.SafeSplitSemiColonSeperatedValue())));
            this.DefaultCssResourcePivots.Set((string) descendant.Attribute((XName) "key"), new ResourcePivotApplyMode?((ResourcePivotApplyMode) ((int) ((string) descendant.Attribute((XName) "applyMode")).TryParseToEnum<ResourcePivotApplyMode>() ?? 0)), ((string) descendant).NullSafeAction<string, IEnumerable<string>>((Func<string, IEnumerable<string>>) (sv => sv.SafeSplitSemiColonSeperatedValue())));
            continue;
          case "Bundling":
            this.DefaultBundling.AddNamedConfig<BundlingConfig>(new BundlingConfig(descendant));
            continue;
          case "Global":
            this.global.AddNamedConfig<GlobalConfig>(new GlobalConfig(descendant));
            continue;
          case "CssMinification":
            this.DefaultCssMinification.AddNamedConfig<CssMinificationConfig>(new CssMinificationConfig(descendant));
            continue;
          case "Spriting":
            this.DefaultSpriting.AddNamedConfig<CssSpritingConfig>(new CssSpritingConfig(descendant));
            continue;
          case "JsMinification":
            this.DefaultJSMinification.AddNamedConfig<JsMinificationConfig>(new JsMinificationConfig(descendant));
            continue;
          case "Preprocessing":
            this.DefaultPreprocessing.AddNamedConfig<PreprocessingConfig>(new PreprocessingConfig(descendant));
            continue;
          default:
            continue;
        }
      }
    }
  }
}
