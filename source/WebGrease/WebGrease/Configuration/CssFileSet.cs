// Decompiled with JetBrains decompiler
// Type: WebGrease.Configuration.CssFileSet
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using WebGrease.Css.Extensions;
using WebGrease.Extensions;

namespace WebGrease.Configuration
{
  internal sealed class CssFileSet : FileSetBase
  {
    private bool localDpiUsed;
    private IDictionary<string, HashSet<float>> allDpi = (IDictionary<string, HashSet<float>>) new Dictionary<string, HashSet<float>>();

    internal CssFileSet()
    {
      this.Minification = (IDictionary<string, CssMinificationConfig>) new Dictionary<string, CssMinificationConfig>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.ImageSpriting = (IDictionary<string, CssSpritingConfig>) new Dictionary<string, CssSpritingConfig>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.Autonaming = (IDictionary<string, AutoNameConfig>) new Dictionary<string, AutoNameConfig>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.Dpi = new HashSet<float>();
    }

    internal CssFileSet(
      XElement cssFileSetElement,
      string sourceDirectory,
      IDictionary<string, CssMinificationConfig> defaultMinification,
      IDictionary<string, CssSpritingConfig> defaultSpriting,
      IDictionary<string, PreprocessingConfig> defaultPreprocessing,
      IDictionary<string, BundlingConfig> defaultBundling,
      ResourcePivotGroupCollection defaultResourcePivots,
      GlobalConfig globalConfig,
      string defaultOutputPathFormat,
      IDictionary<string, HashSet<float>> defaultDpi,
      string configurationFile)
      : this()
    {
      this.InitializeDefaults(defaultResourcePivots, defaultPreprocessing, defaultBundling, defaultOutputPathFormat);
      this.InitializeDefaults(defaultMinification, defaultSpriting, defaultDpi);
      this.Load(this.Initialize(cssFileSetElement, globalConfig, configurationFile), sourceDirectory);
    }

    public IDictionary<string, AutoNameConfig> Autonaming { get; private set; }

    internal IDictionary<string, CssMinificationConfig> Minification { get; private set; }

    internal HashSet<float> Dpi { get; private set; }

    internal IDictionary<string, CssSpritingConfig> ImageSpriting { get; private set; }

    public override string ToString() => "[CssFileSet:{0}]".InvariantFormat((object) this.Output);

    protected override void Load(IEnumerable<XElement> fileSetElements, string sourceDirectory)
    {
      base.Load(fileSetElements, sourceDirectory);
      foreach (XElement fileSetElement in fileSetElements)
      {
        string str1 = fileSetElement.Name.ToString();
        string str2 = (string) fileSetElement;
        switch (str1)
        {
          case "Dpi":
            if (!this.localDpiUsed)
            {
              this.localDpiUsed = true;
              this.allDpi.Clear();
            }
            IEnumerable<float> collection = str2.NullSafeAction<string, IEnumerable<string>>(new Func<string, IEnumerable<string>>(StringExtensions.SafeSplitSemiColonSeperatedValue)).Select<string, float?>((Func<string, float?>) (d => d.TryParseFloat())).Where<float?>((Func<float?, bool>) (d => d.HasValue)).Select<float?, float>((Func<float?, float>) (d => d.Value));
            this.allDpi[((string) fileSetElement.Attribute((XName) "output")).AsNullIfWhiteSpace() ?? string.Empty] = new HashSet<float>(collection);
            continue;
          case "Minification":
            this.Minification.AddNamedConfig<CssMinificationConfig>(new CssMinificationConfig(fileSetElement));
            continue;
          case "Spriting":
            this.ImageSpriting.AddNamedConfig<CssSpritingConfig>(new CssSpritingConfig(fileSetElement));
            continue;
          case "Autoname":
            this.Autonaming.AddNamedConfig<AutoNameConfig>(new AutoNameConfig(fileSetElement));
            continue;
          default:
            continue;
        }
      }
      HashSet<float> floatSet1;
      if (!this.allDpi.TryGetValue(this.allDpi.Keys.FirstOrDefault<string>((Func<string, bool>) (k => !k.IsNullOrWhitespace() && this.Output.IndexOf(k, StringComparison.OrdinalIgnoreCase) != -1)) ?? string.Empty, out floatSet1))
        this.allDpi.TryGetValue(string.Empty, out floatSet1);
      HashSet<float> floatSet2 = floatSet1;
      if (floatSet2 == null)
        floatSet2 = new HashSet<float>() { 1f };
      this.Dpi = floatSet2;
    }

    private void InitializeDefaults(
      IDictionary<string, CssMinificationConfig> defaultMinification,
      IDictionary<string, CssSpritingConfig> defaultSpriting,
      IDictionary<string, HashSet<float>> defaultDpi)
    {
      if (defaultDpi != null && defaultDpi.Any<KeyValuePair<string, HashSet<float>>>())
        defaultDpi.ForEach<KeyValuePair<string, HashSet<float>>>((Action<KeyValuePair<string, HashSet<float>>>) (dd => this.allDpi[dd.Key] = dd.Value));
      if (defaultMinification != null && defaultMinification.Count > 0)
      {
        foreach (string key in (IEnumerable<string>) defaultMinification.Keys)
          this.Minification[key] = defaultMinification[key];
      }
      if (defaultSpriting == null || defaultSpriting.Count <= 0)
        return;
      foreach (string key in (IEnumerable<string>) defaultSpriting.Keys)
        this.ImageSpriting[key] = defaultSpriting[key];
    }
  }
}
