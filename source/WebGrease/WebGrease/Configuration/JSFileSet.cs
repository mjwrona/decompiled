// Decompiled with JetBrains decompiler
// Type: WebGrease.Configuration.JSFileSet
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using WebGrease.Extensions;

namespace WebGrease.Configuration
{
  internal sealed class JSFileSet : FileSetBase
  {
    internal JSFileSet()
    {
      this.Minification = (IDictionary<string, JsMinificationConfig>) new Dictionary<string, JsMinificationConfig>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.Validation = (IDictionary<string, JSValidationConfig>) new Dictionary<string, JSValidationConfig>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    public override string ToString() => "[JsFileSet:{0}]".InvariantFormat((object) this.Output);

    internal JSFileSet(
      XElement jsFileSetElement,
      string sourceDirectory,
      IDictionary<string, JsMinificationConfig> defaultMinification,
      IDictionary<string, PreprocessingConfig> defaultPreProcessing,
      IDictionary<string, BundlingConfig> defaultBundling,
      ResourcePivotGroupCollection defaultResourcePivots,
      GlobalConfig globalConfig,
      string defaultOutputPathFormat,
      string configurationFile)
      : this()
    {
      this.InitializeDefaults(defaultResourcePivots, defaultPreProcessing, defaultBundling, defaultOutputPathFormat);
      this.InitializeDefaults(defaultMinification);
      this.Load(this.Initialize(jsFileSetElement, globalConfig, configurationFile), sourceDirectory);
    }

    internal IDictionary<string, JSValidationConfig> Validation { get; private set; }

    internal IDictionary<string, JsMinificationConfig> Minification { get; private set; }

    protected override void Load(IEnumerable<XElement> fileSetElements, string sourceDirectory)
    {
      base.Load(fileSetElements, sourceDirectory);
      foreach (XElement fileSetElement in fileSetElements)
      {
        switch (fileSetElement.Name.ToString())
        {
          case "Minification":
            this.Minification.AddNamedConfig<JsMinificationConfig>(new JsMinificationConfig(fileSetElement));
            continue;
          case "Validation":
            this.Validation.AddNamedConfig<JSValidationConfig>(new JSValidationConfig(fileSetElement));
            continue;
          default:
            continue;
        }
      }
    }

    private void InitializeDefaults(
      IDictionary<string, JsMinificationConfig> defaultMinification)
    {
      if (defaultMinification == null || defaultMinification.Count <= 0)
        return;
      foreach (string key in (IEnumerable<string>) defaultMinification.Keys)
        this.Minification[key] = defaultMinification[key];
    }
  }
}
