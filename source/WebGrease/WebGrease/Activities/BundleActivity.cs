// Decompiled with JetBrains decompiler
// Type: WebGrease.Activities.BundleActivity
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WebGrease.Configuration;
using WebGrease.Css.Extensions;
using WebGrease.Extensions;

namespace WebGrease.Activities
{
  internal class BundleActivity
  {
    private readonly WebGreaseContext context;

    public BundleActivity(WebGreaseContext webGreaseContext) => this.context = webGreaseContext;

    internal bool Execute(IEnumerable<IFileSet> fileSets) => true & this.BundleFileSets((IEnumerable<IFileSet>) fileSets.OfType<JSFileSet>(), FileTypes.JS) & this.BundleFileSets((IEnumerable<IFileSet>) fileSets.OfType<CssFileSet>(), FileTypes.CSS);

    private bool BundleFileSets(IEnumerable<IFileSet> fileSets, FileTypes fileType)
    {
      if (!fileSets.Any<IFileSet>())
        return true;
      var varBySettings = new
      {
        fileSets = fileSets,
        fileType = fileType,
        Configuration = this.context.Configuration
      };
      return this.context.SectionedAction(new string[2]
      {
        nameof (BundleActivity),
        fileType.ToString()
      }).MakeCachable((object) varBySettings, true).RestoreFromCacheAction(new Func<ICacheSection, bool>(this.RestoreBundleFromCache)).Execute((Func<ICacheSection, bool>) (cacheSection =>
      {
        this.context.Log.Information("Begin {0} bundle pipeline".InvariantFormat((object) fileType));
        bool flag = this.Bundle(fileSets, fileType);
        this.context.Log.Information("End {0} bundle pipeline".InvariantFormat((object) fileType));
        return flag;
      }));
    }

    private bool RestoreBundleFromCache(ICacheSection cacheSection)
    {
      cacheSection.GetCachedContentItems("AssemblerResult", true).ForEach<ContentItem>((Action<ContentItem>) (er => er.WriteToContentPath(this.context.Configuration.DestinationDirectory)));
      return true;
    }

    private bool Bundle(IEnumerable<IFileSet> fileSets, FileTypes fileType)
    {
      bool flag = true;
      WebGreaseContext sessionContext = this.context;
      foreach (IFileSet fileSet1 in fileSets)
      {
        IFileSet fileSet = fileSet1;
        string configType = this.context.Configuration.ConfigType;
        BundlingConfig bundleConfig = fileSet.Bundling.GetNamedConfig<BundlingConfig>(configType);
        if (bundleConfig.ShouldBundleFiles)
        {
          string outputFile = Path.Combine(this.context.Configuration.DestinationDirectory, fileSet.Output);
          PreprocessingConfig preprocessingConfig = fileSet.Preprocessing.GetNamedConfig<PreprocessingConfig>(configType);
          flag = ((flag ? 1 : 0) & (sessionContext.SectionedAction(new string[3]
          {
            nameof (BundleActivity),
            fileType.ToString(),
            "Process"
          }).MakeCachable(fileSet, (object) new
          {
            bundleConfig = bundleConfig,
            preprocessingConfig = preprocessingConfig
          }, true).RestoreFromCacheAction(new Func<ICacheSection, bool>(this.RestoreBundleFromCache)).Execute((Func<ICacheSection, bool>) (fileSetCacheSection =>
          {
            fileSet.LoadedConfigurationFiles.ForEach<string>(new Action<string>(fileSetCacheSection.AddSourceDependency));
            if (Path.GetExtension(outputFile).IsNullOrWhitespace())
            {
              Console.WriteLine(ResourceStrings.InvalidBundlingOutputFile, (object) outputFile);
              return true;
            }
            AssemblerActivity assemblerActivity = new AssemblerActivity((IWebGreaseContext) sessionContext);
            assemblerActivity.OutputFile = outputFile;
            assemblerActivity.Inputs.Clear();
            assemblerActivity.PreprocessingConfig = preprocessingConfig;
            assemblerActivity.Inputs.AddRange((IEnumerable<InputSpec>) fileSet.InputSpecs);
            assemblerActivity.MinimalOutput = bundleConfig.MinimalOutput;
            ContentItem contentItem = assemblerActivity.Execute();
            fileSetCacheSection.AddResult(contentItem, "AssemblerResult", true);
            return true;
          })) ? 1 : 0)) != 0;
        }
      }
      return flag;
    }
  }
}
