// Decompiled with JetBrains decompiler
// Type: WebGrease.Configuration.FileSetBase
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using WebGrease.Extensions;

namespace WebGrease.Configuration
{
  internal abstract class FileSetBase : IFileSet
  {
    private readonly IList<string> usingLocalResourcePivot = (IList<string>) new List<string>();

    protected FileSetBase()
    {
      this.ResourcePivots = new ResourcePivotGroupCollection();
      this.AutoNaming = (IDictionary<string, AutoNameConfig>) new Dictionary<string, AutoNameConfig>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.InputSpecs = (IList<InputSpec>) new List<InputSpec>();
      this.Bundling = (IDictionary<string, BundlingConfig>) new Dictionary<string, BundlingConfig>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.Preprocessing = (IDictionary<string, PreprocessingConfig>) new Dictionary<string, PreprocessingConfig>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.LoadedConfigurationFiles = (IList<string>) new List<string>();
    }

    public ResourcePivotGroupCollection ResourcePivots { get; private set; }

    public IList<string> LoadedConfigurationFiles { get; private set; }

    public IList<string> Locales => (IList<string>) this.ResourcePivots["locales"].NullSafeAction<ResourcePivotGroup, string[]>((Func<ResourcePivotGroup, string[]>) (l => l.Keys.ToArray<string>())) ?? (IList<string>) new string[0];

    public IList<string> Themes => (IList<string>) this.ResourcePivots["themes"].NullSafeAction<ResourcePivotGroup, string[]>((Func<ResourcePivotGroup, string[]>) (l => l.Keys.ToArray<string>())) ?? (IList<string>) new string[0];

    public IDictionary<string, PreprocessingConfig> Preprocessing { get; private set; }

    public IDictionary<string, BundlingConfig> Bundling { get; private set; }

    public string Output { get; set; }

    public string OutputPathFormat { get; set; }

    public IList<InputSpec> InputSpecs { get; private set; }

    public IDictionary<string, AutoNameConfig> AutoNaming { get; private set; }

    internal GlobalConfig GlobalConfig { get; private set; }

    protected virtual void Load(IEnumerable<XElement> fileSetElements, string sourceDirectory)
    {
      foreach (XElement fileSetElement in fileSetElements)
      {
        string str1 = fileSetElement.Name.ToString();
        string str2 = fileSetElement.Value;
        switch (str1)
        {
          case "OutputPathFormat":
            this.OutputPathFormat = str2;
            continue;
          case "Inputs":
            this.InputSpecs.AddInputSpecs(sourceDirectory, fileSetElement);
            continue;
          case "Preprocessing":
            this.Preprocessing.AddNamedConfig<PreprocessingConfig>(new PreprocessingConfig(fileSetElement));
            continue;
          case "Bundling":
            this.Bundling.AddNamedConfig<BundlingConfig>(new BundlingConfig(fileSetElement));
            continue;
          case "Autoname":
            this.AutoNaming.AddNamedConfig<AutoNameConfig>(new AutoNameConfig(fileSetElement));
            continue;
          case "Locales":
            if (!this.usingLocalResourcePivot.Contains("locales"))
            {
              this.usingLocalResourcePivot.Add("locales");
              this.ResourcePivots.Clear("locales");
            }
            this.ResourcePivots.Set("locales", new ResourcePivotApplyMode?(ResourcePivotApplyMode.ApplyAsStringReplace), str2.NullSafeAction<string, IEnumerable<string>>((Func<string, IEnumerable<string>>) (sv => sv.SafeSplitSemiColonSeperatedValue())));
            continue;
          case "Themes":
            if (!this.usingLocalResourcePivot.Contains("themes"))
            {
              this.usingLocalResourcePivot.Add("themes");
              this.ResourcePivots.Clear("themes");
            }
            this.ResourcePivots.Set("themes", new ResourcePivotApplyMode?(ResourcePivotApplyMode.ApplyAsStringReplace), str2.NullSafeAction<string, IEnumerable<string>>((Func<string, IEnumerable<string>>) (sv => sv.SafeSplitSemiColonSeperatedValue())));
            continue;
          case "ResourcePivot":
            this.ResourcePivots.Set((string) fileSetElement.Attribute((XName) "key"), new ResourcePivotApplyMode?((ResourcePivotApplyMode) ((int) ((string) fileSetElement.Attribute((XName) "applyMode")).TryParseToEnum<ResourcePivotApplyMode>() ?? 0)), ((string) fileSetElement).NullSafeAction<string, IEnumerable<string>>((Func<string, IEnumerable<string>>) (sv => sv.SafeSplitSemiColonSeperatedValue())));
            continue;
          default:
            continue;
        }
      }
    }

    protected IEnumerable<XElement> Initialize(
      XElement fileSetElement,
      GlobalConfig globalConfig,
      string configurationFile)
    {
      this.Output = (string) fileSetElement.Attribute((XName) "output") ?? string.Empty;
      this.GlobalConfig = globalConfig;
      List<XElement> fileSetElements = fileSetElement.Descendants().ToList<XElement>();
      WebGreaseConfiguration.ForEachConfigSourceElement(fileSetElement, configurationFile, (Action<XElement, string>) ((element, s) =>
      {
        this.LoadedConfigurationFiles.Add(s);
        fileSetElements.AddRange(element.Descendants());
      }));
      return (IEnumerable<XElement>) fileSetElements;
    }

    protected void InitializeDefaults(
      ResourcePivotGroupCollection defaultResourcePivots,
      IDictionary<string, PreprocessingConfig> defaultPreprocessing,
      IDictionary<string, BundlingConfig> defaultBundling,
      string defaultOutputPathFormat)
    {
      if (!string.IsNullOrWhiteSpace(defaultOutputPathFormat))
        this.OutputPathFormat = defaultOutputPathFormat;
      if (defaultResourcePivots != null && defaultResourcePivots.Count<ResourcePivotGroup>() > 0)
      {
        foreach (ResourcePivotGroup defaultResourcePivot in defaultResourcePivots)
          this.ResourcePivots.Set(defaultResourcePivot.Key, new ResourcePivotApplyMode?(defaultResourcePivot.ApplyMode), (IEnumerable<string>) defaultResourcePivot.Keys);
      }
      if (defaultPreprocessing != null && defaultPreprocessing.Count > 0)
      {
        foreach (string key in (IEnumerable<string>) defaultPreprocessing.Keys)
          this.Preprocessing[key] = defaultPreprocessing[key];
      }
      if (defaultBundling == null || defaultBundling.Count <= 0)
        return;
      foreach (string key in (IEnumerable<string>) defaultBundling.Keys)
        this.Bundling[key] = defaultBundling[key];
    }
  }
}
