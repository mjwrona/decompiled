// Decompiled with JetBrains decompiler
// Type: WebGrease.Preprocessing.PreprocessingManager
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Linq;
using System.Reflection;
using WebGrease.Configuration;
using WebGrease.Css.Extensions;
using WebGrease.Extensions;

namespace WebGrease.Preprocessing
{
  public class PreprocessingManager
  {
    [ImportMany(typeof (IPreprocessingEngine))]
    private readonly IList<IPreprocessingEngine> registeredPreprocessingEngines = (IList<IPreprocessingEngine>) new List<IPreprocessingEngine>();
    private IWebGreaseContext context;

    internal PreprocessingManager(
      WebGreaseConfiguration webGreaseConfiguration,
      LogManager logManager,
      ITimeMeasure timeMeasure)
    {
      if (webGreaseConfiguration == null)
        throw new ArgumentNullException(nameof (webGreaseConfiguration));
      if (logManager == null)
        throw new ArgumentNullException(nameof (logManager));
      if (timeMeasure == null)
        throw new ArgumentNullException(nameof (timeMeasure));
      this.Initialize(webGreaseConfiguration.PreprocessingPluginPath, logManager, timeMeasure);
    }

    internal PreprocessingManager(PreprocessingManager preprocessingManager) => preprocessingManager.registeredPreprocessingEngines.ForEach<IPreprocessingEngine>((Action<IPreprocessingEngine>) (rp => this.registeredPreprocessingEngines.Add(rp)));

    internal void SetContext(IWebGreaseContext webGreaseContext) => this.context = webGreaseContext;

    internal ContentItem Process(
      ContentItem contentItem,
      PreprocessingConfig preprocessConfig,
      bool minimalOutput = false)
    {
      this.context.Log.Information("Registered preprocessors to use: {0}".InvariantFormat((object) string.Join(";", (IEnumerable<string>) preprocessConfig.PreprocessingEngines)));
      IPreprocessingEngine[] preprocessorsToUse = this.GetProcessors(contentItem, preprocessConfig);
      if (!((IEnumerable<IPreprocessingEngine>) preprocessorsToUse).Any<IPreprocessingEngine>())
        return contentItem;
      this.context.SectionedAction("Preprocessing").MakeCachable(contentItem, (object) new
      {
        relativePath = Path.GetDirectoryName(contentItem.RelativeContentPath),
        preprocessConfig = preprocessConfig,
        pptu = ((IEnumerable<IPreprocessingEngine>) preprocessorsToUse).Select<IPreprocessingEngine, string>((Func<IPreprocessingEngine, string>) (pptu => pptu.Name))
      }).RestoreFromCacheAction((Func<ICacheSection, bool>) (cacheSection =>
      {
        contentItem = cacheSection.GetCachedContentItem("PreprocessingResult");
        return contentItem != null;
      })).Execute((Func<ICacheSection, bool>) (cacheSection =>
      {
        foreach (IPreprocessingEngine preprocessingEngine in preprocessorsToUse)
        {
          this.context.Log.Information("preprocessing with: {0}".InvariantFormat((object) preprocessingEngine.Name));
          contentItem = preprocessingEngine.Process(this.context, contentItem, preprocessConfig, minimalOutput);
          if (contentItem == null)
            return false;
        }
        cacheSection.AddResult(contentItem, "PreprocessingResult");
        return true;
      }));
      return contentItem;
    }

    internal IPreprocessingEngine[] GetProcessors(
      ContentItem contentItem,
      PreprocessingConfig preprocessConfig)
    {
      return preprocessConfig.PreprocessingEngines.SelectMany<string, IPreprocessingEngine>((Func<string, IEnumerable<IPreprocessingEngine>>) (ppe => this.registeredPreprocessingEngines.Where<IPreprocessingEngine>((Func<IPreprocessingEngine, bool>) (rppe => rppe.Name.Equals(ppe, StringComparison.OrdinalIgnoreCase))))).Where<IPreprocessingEngine>((Func<IPreprocessingEngine, bool>) (pptu => pptu.CanProcess(this.context, contentItem, preprocessConfig))).ToArray<IPreprocessingEngine>();
    }

    private void Initialize(string pluginPath, LogManager logManager, ITimeMeasure timeMeasure)
    {
      timeMeasure.Start(false, "Preprocessing", nameof (Initialize));
      logManager.Information(ResourceStrings.PreprocessingInitializeStart.InvariantFormat((object) pluginPath));
      if (string.IsNullOrWhiteSpace(pluginPath))
        pluginPath = new FileInfo(Assembly.GetCallingAssembly().FullName).DirectoryName;
      if (!string.IsNullOrWhiteSpace(pluginPath))
      {
        if (!Directory.Exists(pluginPath))
        {
          logManager.Error((Exception) new DirectoryNotFoundException(pluginPath), ResourceStrings.PreprocessingCouldNotFindThePluginPath.InvariantFormat((object) pluginPath));
          return;
        }
        logManager.Information(ResourceStrings.PreprocessingPluginPath.InvariantFormat((object) pluginPath));
        using (AggregateCatalog catalog = new AggregateCatalog())
        {
          catalog.Catalogs.Add((ComposablePartCatalog) new DirectoryCatalog(pluginPath));
          using (CompositionContainer container = new CompositionContainer((ComposablePartCatalog) catalog, new ExportProvider[0]))
          {
            try
            {
              container.ComposeParts((object) this);
            }
            catch (CompositionException ex)
            {
              logManager.Error((Exception) ex, ResourceStrings.PreprocessingLoadingError);
            }
            foreach (IPreprocessingEngine preprocessingEngine in (IEnumerable<IPreprocessingEngine>) this.registeredPreprocessingEngines)
              logManager.Information(ResourceStrings.PreprocessingEngineFound.InvariantFormat((object) preprocessingEngine.Name));
          }
        }
      }
      logManager.Information(ResourceStrings.PreprocessingInitializeEnd);
      timeMeasure.End(false, "Preprocessing", nameof (Initialize));
    }
  }
}
