// Decompiled with JetBrains decompiler
// Type: WebGrease.Activities.EverythingActivity
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using Antlr.Runtime;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using WebGrease.Configuration;
using WebGrease.Css;
using WebGrease.Css.Extensions;
using WebGrease.Extensions;

namespace WebGrease.Activities
{
  internal sealed class EverythingActivity
  {
    private const string ImagesDestinationDirectoryName = "i";
    private const string JsDestinationDirectoryName = "js";
    private const string CssDestinationDirectoryName = "css";
    private const string ToolsTempDirectoryName = "ToolsTemp";
    private const string StaticAssemblerDirectoryName = "StaticAssemblerOutput";
    private const string PreprocessingTempDirectory = "PreCompiler";
    private readonly string toolsTempDirectory;
    private readonly string staticAssemblerDirectory;
    private readonly string logDirectory;
    private readonly string preprocessingTempDirectory;
    private readonly string imagesLogFile;
    private readonly IWebGreaseContext context;

    internal EverythingActivity(IWebGreaseContext context)
    {
      this.context = context;
      this.logDirectory = context.Configuration.LogsDirectory;
      this.toolsTempDirectory = context.Configuration.ToolsTempDirectory.IsNullOrWhitespace() ? Path.Combine(context.Configuration.LogsDirectory, "ToolsTemp") : context.Configuration.ToolsTempDirectory;
      this.imagesLogFile = Path.Combine(this.logDirectory, "images_log.xml");
      this.preprocessingTempDirectory = Path.Combine(this.toolsTempDirectory, "PreCompiler");
      this.staticAssemblerDirectory = Path.Combine(this.toolsTempDirectory, "StaticAssemblerOutput");
    }

    internal static string DpiToResolutionName(float dpi) => "Resolution{0:0.##}X".InvariantFormat((object) dpi.ToString((IFormatProvider) CultureInfo.InvariantCulture).Replace(".", string.Empty));

    internal bool Execute()
    {
      this.ExecuteHashImages();
      return this.Execute(this.context.Configuration.CssFileSets.OfType<IFileSet>().Concat<IFileSet>((IEnumerable<IFileSet>) this.context.Configuration.JSFileSets));
    }

    internal bool Execute(IEnumerable<IFileSet> fileSets, FileTypes fileType = FileTypes.All)
    {
      bool flag = true;
      JSFileSet[] array1 = fileSets.OfType<JSFileSet>().ToArray<JSFileSet>();
      if (((IEnumerable<JSFileSet>) array1).Any<JSFileSet>())
        flag &= this.ExecuteJS((IEnumerable<JSFileSet>) array1, this.context.Configuration.ConfigType, this.context.Configuration.SourceDirectory, this.context.Configuration.DestinationDirectory);
      CssFileSet[] array2 = fileSets.OfType<CssFileSet>().ToArray<CssFileSet>();
      if (((IEnumerable<CssFileSet>) array2).Any<CssFileSet>())
        flag &= this.ExecuteCss((IEnumerable<CssFileSet>) array2, this.context.Configuration.SourceDirectory, this.context.Configuration.DestinationDirectory, this.context.Configuration.ConfigType, this.context.Configuration.ImageDirectories, this.context.Configuration.ImageExtensions);
      if (fileType.HasFlag((Enum) FileTypes.Image))
        this.ExecuteHashImages();
      return flag;
    }

    internal void ExecuteHashImages()
    {
      if (!this.context.Configuration.ImageDirectoriesToHash.Any<string>())
        return;
      FileHasherActivity imageFileHasher = this.GetImageFileHasher(this.context.Configuration.DestinationDirectory, this.context.Configuration.ImageExtensions);
      EverythingActivity.HashImages(this.context, imageFileHasher, (IEnumerable<string>) this.context.Configuration.ImageDirectoriesToHash, (IEnumerable<string>) this.context.Configuration.ImageExtensions);
      imageFileHasher.Save();
    }

    private static FileHasherActivity GetFileHasher(
      IWebGreaseContext context,
      string hashOutputPath,
      string logFileName,
      FileTypes fileType,
      string outputRelativeToPath,
      string basePrefixToAddToOutputPath = null,
      IEnumerable<string> fileTypeFilters = null)
    {
      string str = fileTypeFilters != null ? string.Join(new string(Strings.FileFilterSeparator), fileTypeFilters) : (string) null;
      return new FileHasherActivity(context)
      {
        DestinationDirectory = hashOutputPath,
        BasePrefixToRemoveFromOutputPathInLog = outputRelativeToPath,
        CreateExtraDirectoryLevelFromHashes = true,
        ShouldPreserveSourceDirectoryStructure = false,
        LogFileName = logFileName,
        FileType = fileType,
        FileTypeFilter = str,
        BasePrefixToAddToOutputPath = basePrefixToAddToOutputPath
      };
    }

    private static IDictionary<string, IDictionary<string, string>> GetMergedResources(
      IWebGreaseContext context,
      FileTypes fileType,
      string resourceGroupKey,
      IEnumerable<string> resourceKeys)
    {
      ResourcesResolutionActivity resolutionActivity = new ResourcesResolutionActivity(context)
      {
        SourceDirectory = context.Configuration.SourceDirectory,
        ApplicationDirectoryName = context.Configuration.TokensDirectory,
        SiteDirectoryName = context.Configuration.OverrideTokensDirectory,
        ResourceGroupKey = resourceGroupKey,
        FileType = fileType
      };
      resolutionActivity.ResourceKeys.AddRange(resourceKeys);
      return resolutionActivity.GetMergedResources();
    }

    private static void EnsureCssLogFile(
      FileHasherActivity cssHasher,
      FileHasherActivity imageHasher,
      ICacheSection cacheSection)
    {
      EverythingActivity.EnsureLogFile(cssHasher, cacheSection.GetCachedContentItems("HashedMinifiedCssResult"));
      EverythingActivity.EnsureLogFile(imageHasher, cacheSection.GetCachedContentItems("HashedImage"));
    }

    private static string GetContentPivotDestinationFilePath(
      string relativeContentPath,
      string destinationDirectoryName,
      string destinationExtension,
      string destinationPathFormat,
      IEnumerable<ResourcePivotKey> resourcePivotKeys = null)
    {
      if (string.IsNullOrWhiteSpace(destinationPathFormat))
      {
        ResourcePivotKey resourcePivotKey1 = resourcePivotKeys != null ? resourcePivotKeys.FirstOrDefault<ResourcePivotKey>((Func<ResourcePivotKey, bool>) (rpk => rpk.GroupKey.Equals("themes"))) : (ResourcePivotKey) null;
        string str = resourcePivotKey1 == null || resourcePivotKey1.Key.IsNullOrWhitespace() ? string.Empty : resourcePivotKey1.Key + "_";
        ResourcePivotKey resourcePivotKey2 = resourcePivotKeys != null ? resourcePivotKeys.FirstOrDefault<ResourcePivotKey>((Func<ResourcePivotKey, bool>) (rpk => rpk.GroupKey.Equals("locales"))) : (ResourcePivotKey) null;
        return Path.Combine(resourcePivotKey2 == null || resourcePivotKey2.Key.IsNullOrWhitespace() ? string.Empty : resourcePivotKey2.Key, destinationDirectoryName, str + Path.ChangeExtension(relativeContentPath, destinationExtension));
      }
      destinationPathFormat = destinationPathFormat.ToLowerInvariant();
      if (resourcePivotKeys != null)
      {
        foreach (ResourcePivotKey resourcePivotKey in resourcePivotKeys)
          destinationPathFormat = destinationPathFormat.Replace("{" + resourcePivotKey.GroupKey.ToLowerInvariant() + "}", resourcePivotKey.Key);
      }
      destinationPathFormat = destinationPathFormat.Replace("{output}", relativeContentPath);
      if (destinationPathFormat.IndexOf("{", StringComparison.OrdinalIgnoreCase) != -1)
        throw new BuildWorkflowException("Could not generate the correct output file, one key was not replaced: {0}".InvariantFormat((object) destinationPathFormat));
      return Path.Combine(destinationDirectoryName, Path.ChangeExtension(destinationPathFormat, destinationExtension));
    }

    private static void EnsureJsLogFile(FileHasherActivity jsHasher, ICacheSection cacheSection) => EverythingActivity.EnsureLogFile(jsHasher, cacheSection.GetCachedContentItems("HashedMinifiedJsResult"));

    private static void EnsureLogFile(
      FileHasherActivity hasher,
      IEnumerable<ContentItem> contentItems)
    {
      hasher.AppendToWorkLog(contentItems);
    }

    private static void HashImages(
      IWebGreaseContext context,
      FileHasherActivity imageHasher,
      IEnumerable<string> imageDirectoriesToHash,
      IEnumerable<string> imageExtensions)
    {
      context.SectionedAction("ImageHash").MakeCachable((object) new
      {
        imageDirectoriesToHash = imageDirectoriesToHash,
        imageExtensions = imageExtensions
      }).RestoreFromCacheAction((Func<ICacheSection, bool>) (cacheSection =>
      {
        IEnumerable<ContentItem> cachedContentItems = cacheSection.GetCachedContentItems("HashedImage");
        cachedContentItems.ForEach<ContentItem>((Action<ContentItem>) (ci => ci.WriteToRelativeHashedPath(context.Configuration.DestinationDirectory)));
        EverythingActivity.EnsureLogFile(imageHasher, cachedContentItems);
        return true;
      })).WhenSkipped((Action<ICacheSection>) (cacheSection => EverythingActivity.EnsureLogFile(imageHasher, cacheSection.GetCachedContentItems("HashedImage")))).Execute((Func<ICacheSection, bool>) (cacheSection =>
      {
        imageDirectoriesToHash.Select<string, InputSpec>((Func<string, InputSpec>) (imageDirectoryToHash => new InputSpec()
        {
          Path = imageDirectoryToHash,
          IsOptional = true,
          SearchPattern = "*.*",
          SearchOption = SearchOption.AllDirectories
        })).ForEach<InputSpec>(new Action<InputSpec>(cacheSection.AddSourceDependency));
        foreach (KeyValuePair<string, string> availableFile in (IEnumerable<KeyValuePair<string, string>>) context.GetAvailableFiles(context.Configuration.SourceDirectory, imageDirectoriesToHash, imageExtensions, FileTypes.Image))
        {
          ContentItem contentItem = imageHasher.Hash(ContentItem.FromFile(availableFile.Value, availableFile.Key, (string) null));
          cacheSection.AddResult(contentItem, "HashedImage", true);
        }
        return true;
      }));
    }

    private static IEnumerable<IEnumerable<ResourcePivotKey>> GetGroupedResourceKeys(
      ResourcePivotKey[] flatResourceKeyList)
    {
      IEnumerable<string> strings = ((IEnumerable<ResourcePivotKey>) flatResourceKeyList).Select<ResourcePivotKey, string>((Func<ResourcePivotKey, string>) (k => k.GroupKey)).Distinct<string>();
      List<IEnumerable<ResourcePivotKey>> source = new List<IEnumerable<ResourcePivotKey>>();
      foreach (string str in strings)
      {
        string groupKey = str;
        List<IEnumerable<ResourcePivotKey>> resourcePivotKeysList = new List<IEnumerable<ResourcePivotKey>>();
        foreach (ResourcePivotKey resourcePivotKey in ((IEnumerable<ResourcePivotKey>) flatResourceKeyList).Where<ResourcePivotKey>((Func<ResourcePivotKey, bool>) (k => k.GroupKey.Equals(groupKey))))
        {
          if (!source.Any<IEnumerable<ResourcePivotKey>>())
          {
            resourcePivotKeysList.Add((IEnumerable<ResourcePivotKey>) new List<ResourcePivotKey>((IEnumerable<ResourcePivotKey>) new ResourcePivotKey[1]
            {
              resourcePivotKey
            }));
          }
          else
          {
            foreach (IEnumerable<ResourcePivotKey> first in source)
              resourcePivotKeysList.Add(first.Concat<ResourcePivotKey>((IEnumerable<ResourcePivotKey>) new ResourcePivotKey[1]
              {
                resourcePivotKey
              }));
          }
        }
        source = resourcePivotKeysList;
      }
      return (IEnumerable<IEnumerable<ResourcePivotKey>>) source;
    }

    private IEnumerable<string> GetDestinationFilePaths(
      ContentItem inputFile,
      string destinationDirectoryName,
      string destinationExtension,
      string destinationPathFormat)
    {
      if (inputFile.ResourcePivotKeys == null || !inputFile.ResourcePivotKeys.Any<ResourcePivotKey>())
        return (IEnumerable<string>) new string[1]
        {
          EverythingActivity.GetContentPivotDestinationFilePath(inputFile.RelativeContentPath, destinationDirectoryName, destinationExtension, destinationPathFormat)
        };
      List<string> destinationFilePaths = new List<string>();
      foreach (IEnumerable<ResourcePivotKey> groupedResourceKey in EverythingActivity.GetGroupedResourceKeys(inputFile.ResourcePivotKeys.ToArray<ResourcePivotKey>()))
      {
        if (!this.context.TemporaryIgnore(groupedResourceKey))
          destinationFilePaths.Add(EverythingActivity.GetContentPivotDestinationFilePath(inputFile.RelativeContentPath, destinationDirectoryName, destinationExtension, destinationPathFormat, groupedResourceKey));
      }
      return (IEnumerable<string>) destinationFilePaths;
    }

    private bool ExecuteCss(
      IEnumerable<CssFileSet> cssFileSets,
      string sourceDirectory,
      string destinationDirectory,
      string configType,
      IList<string> imageDirectories,
      IList<string> imageExtensions)
    {
      string logFileName = Path.Combine(this.context.Configuration.LogsDirectory, "css_log.xml");
      string hashOutputPath = Path.Combine(destinationDirectory, "css");
      string imageHashedOutputPath = Path.Combine(destinationDirectory, "i");
      if (!cssFileSets.Any<CssFileSet>())
        return true;
      FileHasherActivity imageHasher = this.GetImageFileHasher(destinationDirectory, imageExtensions);
      FileHasherActivity cssHasher = EverythingActivity.GetFileHasher(this.context, hashOutputPath, logFileName, FileTypes.CSS, this.context.Configuration.ApplicationRootDirectory);
      bool flag1 = this.context.SectionedAction(nameof (EverythingActivity), "Css").MakeCachable((object) new
      {
        cssFileSets = cssFileSets,
        sourceDirectory = sourceDirectory,
        destinationDirectory = destinationDirectory,
        configType = configType,
        imageExtensions = imageExtensions,
        imageDirectories = imageDirectories
      }, true).WhenSkipped((Action<ICacheSection>) (cacheSection => EverythingActivity.EnsureCssLogFile(cssHasher, imageHasher, cacheSection))).Execute((Func<ICacheSection, bool>) (cacheSection =>
      {
        bool flag2 = true;
        foreach (CssFileSet cssFileSet in cssFileSets)
          flag2 &= this.ExecuteCssFileSet(configType, imageDirectories, imageExtensions, cssFileSet, cssHasher, imageHasher, imageHashedOutputPath);
        return flag2;
      }));
      if (flag1)
      {
        imageHasher.Save();
        cssHasher.Save();
      }
      return flag1;
    }

    private FileHasherActivity GetImageFileHasher(
      string destinationDirectory,
      IList<string> imageExtensions)
    {
      return EverythingActivity.GetFileHasher(this.context, Path.Combine(destinationDirectory, "i"), this.imagesLogFile, FileTypes.Image, destinationDirectory, "../../", (IEnumerable<string>) imageExtensions);
    }

    private bool ExecuteCssFileSet(
      string configType,
      IList<string> imageDirectories,
      IList<string> imageExtensions,
      CssFileSet cssFileSet,
      FileHasherActivity cssHasher,
      FileHasherActivity imageHasher,
      string imagesDestinationDirectory)
    {
      CssSpritingConfig cssSpritingConfig = cssFileSet.ImageSpriting.GetNamedConfig<CssSpritingConfig>(configType);
      CssMinificationConfig cssMinificationConfig = cssFileSet.Minification.GetNamedConfig<CssMinificationConfig>(configType);
      var varBySettings = new
      {
        configType = configType,
        ImageSpriting = cssSpritingConfig,
        Global = cssFileSet.GlobalConfig,
        Bundling = cssFileSet.Bundling.GetNamedConfig<BundlingConfig>(configType),
        Minification = cssMinificationConfig,
        Preprocessing = cssFileSet.Preprocessing.GetNamedConfig<PreprocessingConfig>(configType),
        Locales = cssFileSet.Locales,
        Themes = cssFileSet.Themes
      };
      return this.context.SectionedAction("CssFileSet").MakeCachable((IFileSet) cssFileSet, (object) varBySettings, true).WhenSkipped((Action<ICacheSection>) (cacheSection => EverythingActivity.EnsureCssLogFile(cssHasher, imageHasher, cacheSection))).RestoreFromCacheAction((Func<ICacheSection, bool>) (cacheSection =>
      {
        cssFileSet.LoadedConfigurationFiles.ForEach<string>(new Action<string>(cacheSection.AddSourceDependency));
        IEnumerable<ContentItem> cachedContentItems1 = cacheSection.GetCachedContentItems("HashedMinifiedCssResult");
        cachedContentItems1.ForEach<ContentItem>((Action<ContentItem>) (ci => ci.WriteToRelativeHashedPath(this.context.Configuration.DestinationDirectory)));
        EverythingActivity.EnsureLogFile(cssHasher, cachedContentItems1);
        IEnumerable<ContentItem> cachedContentItems2 = cacheSection.GetCachedContentItems("HashedImage");
        cachedContentItems2.ForEach<ContentItem>((Action<ContentItem>) (ci => ci.WriteToRelativeHashedPath(this.context.Configuration.DestinationDirectory)));
        EverythingActivity.EnsureLogFile(imageHasher, cachedContentItems2);
        cacheSection.GetCachedContentItems("HashedSpriteImage").ForEach<ContentItem>((Action<ContentItem>) (ci => ci.WriteToContentPath(this.context.Configuration.DestinationDirectory)));
        return cachedContentItems1.Any<ContentItem>();
      })).Execute((Func<ICacheSection, bool>) (cacheSection =>
      {
        Dictionary<string, IDictionary<string, IDictionary<string, string>>> dictionary1 = cssFileSet.ResourcePivots.Where<ResourcePivotGroup>((Func<ResourcePivotGroup, bool>) (rp => rp.ApplyMode == ResourcePivotApplyMode.ApplyAsStringReplace)).ToDictionary<ResourcePivotGroup, string, IDictionary<string, IDictionary<string, string>>>((Func<ResourcePivotGroup, string>) (rpg => rpg.Key), (Func<ResourcePivotGroup, IDictionary<string, IDictionary<string, string>>>) (rpg => EverythingActivity.GetMergedResources(this.context, FileTypes.CSS, rpg.Key, (IEnumerable<string>) rpg.Keys)));
        Dictionary<string, IDictionary<string, IDictionary<string, string>>> dictionary2 = cssFileSet.ResourcePivots.Where<ResourcePivotGroup>((Func<ResourcePivotGroup, bool>) (rp => rp.ApplyMode != ResourcePivotApplyMode.ApplyAsStringReplace)).ToDictionary<ResourcePivotGroup, string, IDictionary<string, IDictionary<string, string>>>((Func<ResourcePivotGroup, string>) (rpg => rpg.Key), (Func<ResourcePivotGroup, IDictionary<string, IDictionary<string, string>>>) (rpg => EverythingActivity.GetMergedResources(this.context, FileTypes.CSS, rpg.Key, (IEnumerable<string>) rpg.Keys)));
        IDictionary<string, IDictionary<string, string>> mergedResources = EverythingActivity.GetMergedResources(this.context, FileTypes.CSS, "dpi", cssFileSet.Dpi.Select<float, string>(new Func<float, string>(EverythingActivity.DpiToResolutionName)));
        MinifyCssActivity cssMinifier = this.CreateCssMinifier(imageHasher, imageExtensions, imageDirectories, imagesDestinationDirectory, cssMinificationConfig, cssSpritingConfig, cssFileSet.Dpi, dictionary2, mergedResources);
        IEnumerable<ContentItem> inputItems = this.Bundle((IFileSet) cssFileSet, Path.Combine(this.staticAssemblerDirectory, cssFileSet.Output), FileTypes.CSS, configType, cssMinifier.ShouldMinify);
        if (inputItems == null)
          return false;
        this.context.Log.Information(ResourceStrings.ResolvingTokensAndPerformingLocalization);
        IEnumerable<ContentItem> contentItems = this.ApplyResources(inputItems, dictionary1);
        if (!contentItems.All<ContentItem>((Func<ContentItem, bool>) (l => l != null)))
        {
          this.context.Log.Error((Exception) null, ResourceStrings.ThereWereErrorsWhileApplyingCssresources);
          return false;
        }
        IEnumerable<MinifyCssResult> source = this.MinifyCss(contentItems, cssMinifier, imageHasher, cssSpritingConfig.WriteLogFile, dictionary2);
        if (source.Any<MinifyCssResult>((Func<MinifyCssResult, bool>) (i => i == null)))
        {
          this.context.Log.Error((Exception) null, ResourceStrings.ThereWereErrorsWhileMinifyingTheCssFiles);
          return false;
        }
        this.HashContentItems(cssHasher, source.SelectMany<MinifyCssResult, ContentItem>((Func<MinifyCssResult, IEnumerable<ContentItem>>) (i => i.Css)).Where<ContentItem>((Func<ContentItem, bool>) (n => n != null)), "css", "css", cssFileSet.OutputPathFormat).ForEach<ContentItem>((Action<ContentItem>) (hi => cacheSection.AddResult(hi, "HashedMinifiedCssResult")));
        source.SelectMany<MinifyCssResult, ContentItem>((Func<MinifyCssResult, IEnumerable<ContentItem>>) (mci => mci.HashedImages)).Where<ContentItem>((Func<ContentItem, bool>) (n => n != null)).ForEach<ContentItem>((Action<ContentItem>) (hi => cacheSection.AddResult(hi, "HashedImage")));
        return true;
      }));
    }

    private bool ExecuteJS(
      IEnumerable<JSFileSet> jsFileSets,
      string configType,
      string sourceDirectory,
      string destinationDirectory)
    {
      string logFileName = Path.Combine(this.context.Configuration.LogsDirectory, "js_log.xml");
      string hashOutputPath = Path.Combine(destinationDirectory, "js");
      if (!jsFileSets.Any<JSFileSet>())
        return true;
      FileHasherActivity jsHasher = EverythingActivity.GetFileHasher(this.context, hashOutputPath, logFileName, FileTypes.JS, this.context.Configuration.ApplicationRootDirectory);
      var varBySettings = new
      {
        jsFileSets = jsFileSets,
        configType = configType,
        sourceDirectory = sourceDirectory,
        destinationDirectory = destinationDirectory
      };
      bool flag1 = this.context.SectionedAction(nameof (EverythingActivity), "Js").MakeCachable((object) varBySettings, true).WhenSkipped((Action<ICacheSection>) (cacheSection => EverythingActivity.EnsureJsLogFile(jsHasher, cacheSection))).Execute((Func<ICacheSection, bool>) (cacheSection =>
      {
        bool flag2 = true;
        foreach (JSFileSet jsFileSet in jsFileSets)
          flag2 &= this.ExecuteJSFileSet(jsFileSet, jsHasher, configType);
        return flag2;
      }));
      if (flag1)
        jsHasher.Save();
      return flag1;
    }

    private bool ExecuteJSFileSet(
      JSFileSet jsFileSet,
      FileHasherActivity jsHasher,
      string configType)
    {
      return this.context.SectionedAction("JsFileSet").MakeCachable((IFileSet) jsFileSet, (object) new
      {
        configType = configType
      }, true).RestoreFromCacheAction((Func<ICacheSection, bool>) (cacheSection =>
      {
        IEnumerable<ContentItem> cachedContentItems = cacheSection.GetCachedContentItems("HashedMinifiedJsResult");
        cachedContentItems.ForEach<ContentItem>((Action<ContentItem>) (ci => ci.WriteToRelativeHashedPath(this.context.Configuration.DestinationDirectory)));
        EverythingActivity.EnsureLogFile(jsHasher, cachedContentItems);
        return cachedContentItems.Any<ContentItem>();
      })).WhenSkipped((Action<ICacheSection>) (cacheSection => EverythingActivity.EnsureJsLogFile(jsHasher, cacheSection))).Execute((Func<ICacheSection, bool>) (cacheSection =>
      {
        jsFileSet.LoadedConfigurationFiles.ForEach<string>(new Action<string>(cacheSection.AddSourceDependency));
        JsMinificationConfig namedConfig = jsFileSet.Minification.GetNamedConfig<JsMinificationConfig>(configType);
        IEnumerable<ContentItem> inputItems = this.Bundle((IFileSet) jsFileSet, Path.Combine(this.staticAssemblerDirectory, jsFileSet.Output), FileTypes.JS, configType, namedConfig.ShouldMinify);
        if (inputItems == null)
          return false;
        Dictionary<string, IDictionary<string, IDictionary<string, string>>> dictionary = jsFileSet.ResourcePivots.ToDictionary<ResourcePivotGroup, string, IDictionary<string, IDictionary<string, string>>>((Func<ResourcePivotGroup, string>) (rpg => rpg.Key), (Func<ResourcePivotGroup, IDictionary<string, IDictionary<string, string>>>) (rpg => EverythingActivity.GetMergedResources(this.context, FileTypes.CSS, rpg.Key, (IEnumerable<string>) rpg.Keys)));
        this.context.Log.Information(ResourceStrings.ResolvingTokensAndPerformingLocalization);
        IEnumerable<ContentItem> inputFiles = this.ApplyResources(inputItems, dictionary);
        if (inputFiles == null)
        {
          this.context.Log.Error((Exception) null, "There were errors encountered while resolving tokens.");
          return false;
        }
        this.context.Log.Information("Minimizing javascript files");
        IEnumerable<ContentItem> contentItems = this.MinifyJs(inputFiles, namedConfig, jsFileSet.Validation.GetNamedConfig<JSValidationConfig>(configType));
        if (contentItems.Any<ContentItem>((Func<ContentItem, bool>) (ci => ci == null)))
        {
          this.context.Log.Error((Exception) null, "There were errors encountered while minimizing javascript files.");
          return false;
        }
        this.HashContentItems(jsHasher, contentItems, "js", "js", jsFileSet.OutputPathFormat).ForEach<ContentItem>((Action<ContentItem>) (ci => cacheSection.AddResult(ci, "HashedMinifiedJsResult", true)));
        return true;
      }));
    }

    private IEnumerable<ContentItem> HashContentItems(
      FileHasherActivity hasher,
      IEnumerable<ContentItem> contentItems,
      string destinationDirectoryName,
      string destinationExtension,
      string destinationPathFormat)
    {
      List<ContentItem> contentItemList = new List<ContentItem>();
      foreach (ContentItem contentItem in contentItems.Where<ContentItem>((Func<ContentItem, bool>) (ci => ci != null)))
      {
        IEnumerable<string> destinationFilePaths = this.GetDestinationFilePaths(contentItem, destinationDirectoryName, destinationExtension, destinationPathFormat);
        contentItemList.AddRange(hasher.Hash(contentItem, destinationFilePaths));
      }
      return (IEnumerable<ContentItem>) contentItemList;
    }

    private IEnumerable<ContentItem> Bundle(
      IFileSet fileSet,
      string outputFile,
      FileTypes fileType,
      string configType,
      bool minimalOutput)
    {
      BundlingConfig namedConfig1 = fileSet.Bundling.GetNamedConfig<BundlingConfig>(configType);
      PreprocessingConfig namedConfig2 = fileSet.Preprocessing.GetNamedConfig<PreprocessingConfig>(this.context.Configuration.ConfigType);
      if (namedConfig1.ShouldBundleFiles)
      {
        this.context.Log.Information(ResourceStrings.BundlingFiles);
        ContentItem contentItem = this.BundleFiles((IEnumerable<InputSpec>) fileSet.InputSpecs, outputFile, namedConfig2, fileType, minimalOutput || namedConfig1.MinimalOutput);
        if (contentItem == null)
        {
          this.context.Log.Error((Exception) null, ResourceStrings.ThereWereErrorsWhileBundlingFiles);
          return (IEnumerable<ContentItem>) null;
        }
        return (IEnumerable<ContentItem>) new ContentItem[1]
        {
          contentItem
        };
      }
      if (namedConfig2 != null && namedConfig2.Enabled)
        return this.PreprocessFiles(this.preprocessingTempDirectory, (IEnumerable<InputSpec>) fileSet.InputSpecs, namedConfig2);
      fileSet.InputSpecs.ForEach<InputSpec>(new Action<InputSpec>(this.context.Cache.CurrentCacheSection.AddSourceDependency));
      return fileSet.InputSpecs.GetFiles(this.context.Configuration.SourceDirectory).Select<string, ContentItem>((Func<string, ContentItem>) (f => ContentItem.FromFile(f, f, this.context.Configuration.SourceDirectory)));
    }

    private MinifyCssActivity CreateCssMinifier(
      FileHasherActivity imageHasher,
      IList<string> imageExtensions,
      IList<string> imageDirectories,
      string imagesDestinationDirectory,
      CssMinificationConfig minificationConfig,
      CssSpritingConfig spritingConfig,
      HashSet<float> dpi,
      Dictionary<string, IDictionary<string, IDictionary<string, string>>> mergedResoures,
      IDictionary<string, IDictionary<string, string>> dpiResources)
    {
      return new MinifyCssActivity(this.context)
      {
        ShouldAssembleBackgroundImages = spritingConfig.ShouldAutoSprite,
        ShouldMinify = minificationConfig.ShouldMinify,
        ShouldMergeMediaQueries = minificationConfig.ShouldMergeMediaQueries,
        ShouldOptimize = minificationConfig.ShouldMinify || minificationConfig.ShouldOptimize,
        ShouldValidateForLowerCase = minificationConfig.ShouldValidateLowerCase,
        ShouldExcludeProperties = minificationConfig.ShouldExcludeProperties,
        ShouldMergeBasedOnCommonDeclarations = minificationConfig.ShouldMergeBasedOnCommonDeclarations,
        ShouldPreventOrderBasedConflict = minificationConfig.ShouldPreventOrderBasedConflict,
        ImageExtensions = imageExtensions,
        ImageDirectories = imageDirectories,
        BannedSelectors = new HashSet<string>((IEnumerable<string>) minificationConfig.RemoveSelectors.ToArray<string>()),
        HackSelectors = new HashSet<string>((IEnumerable<string>) minificationConfig.ForbiddenSelectors.ToArray<string>()),
        NonMergeSelectors = new HashSet<string>((IEnumerable<string>) minificationConfig.NonMergeSelectors.ToArray<string>()),
        ImageAssembleReferencesToIgnore = new HashSet<string>((IEnumerable<string>) spritingConfig.ImagesToIgnore.ToArray<string>()),
        ImageAssemblyPadding = new int?(spritingConfig.ImagePadding),
        ErrorOnInvalidSprite = spritingConfig.ErrorOnInvalidSprite,
        OutputUnit = spritingConfig.OutputUnit,
        OutputUnitFactor = spritingConfig.OutputUnitFactor,
        ImagesOutputDirectory = imagesDestinationDirectory,
        IgnoreImagesWithNonDefaultBackgroundSize = spritingConfig.IgnoreImagesWithNonDefaultBackgroundSize,
        ImageBasePrefixToRemoveFromOutputPathInLog = imageHasher?.BasePrefixToRemoveFromOutputPathInLog,
        ImageBasePrefixToAddToOutputPath = imageHasher?.BasePrefixToAddToOutputPath,
        ForcedSpritingImageType = spritingConfig.ForceImageType,
        Dpi = dpi,
        MergedResources = mergedResoures,
        DpiResources = dpiResources
      };
    }

    private IEnumerable<ContentItem> PreprocessFiles(
      string targetFolder,
      IEnumerable<InputSpec> inputFiles,
      PreprocessingConfig preprocessingConfig)
    {
      PreprocessorActivity preprocessorActivity = new PreprocessorActivity(this.context)
      {
        OutputFolder = targetFolder,
        PreprocessingConfig = preprocessingConfig
      };
      preprocessorActivity.Inputs.AddRange(inputFiles);
      return preprocessorActivity.Execute();
    }

    private IEnumerable<MinifyCssResult> MinifyCss(
      IEnumerable<ContentItem> inputCssItems,
      MinifyCssActivity minifier,
      FileHasherActivity imageHasher,
      bool writeSpriteLogFile,
      Dictionary<string, IDictionary<string, IDictionary<string, string>>> mergedResources)
    {
      List<MinifyCssResult> minifyCssResultList = new List<MinifyCssResult>();
      foreach (ContentItem inputCssItem in inputCssItems)
      {
        string relativeContentPath1 = inputCssItem.RelativeContentPath;
        string relativeContentPath2 = inputCssItem.RelativeContentPath;
        string str = string.Join(".", inputCssItem.ResourcePivotKeys.Select<ResourcePivotKey, string>((Func<ResourcePivotKey, string>) (p => p.ToString())));
        this.context.Log.Information("Css Minify start: {0} : {1}".InvariantFormat((object) relativeContentPath2, (object) str));
        minifier.SourceFile = relativeContentPath1;
        minifier.MergedResources = mergedResources;
        minifier.DestinationFile = relativeContentPath2;
        if (writeSpriteLogFile)
        {
          ResourcePivotKey resourcePivotKey = inputCssItem.ResourcePivotKeys.FirstOrDefault<ResourcePivotKey>();
          minifier.ImageSpritingLogPath = Path.Combine(this.context.Configuration.ReportPath, relativeContentPath2 + (resourcePivotKey != null ? "." + resourcePivotKey.ToString("{0}.{1}") : string.Empty) + ".spritingLog.xml");
        }
        try
        {
          MinifyCssResult minifyCssResult = minifier.Process(inputCssItem, imageHasher);
          minifyCssResultList.Add(minifyCssResult);
        }
        catch (Exception ex)
        {
          minifyCssResultList.Add((MinifyCssResult) null);
          this.HandleCssAggregateException(ex, relativeContentPath1, inputCssItem);
        }
      }
      return (IEnumerable<MinifyCssResult>) minifyCssResultList;
    }

    private void HandleCssAggregateException(
      Exception ex,
      string sourceFile,
      ContentItem inputFile)
    {
      if (ex is AggregateException aggregateException2 || ex.InnerException != null && ex.InnerException is AggregateException aggregateException2)
      {
        List<RecognitionException> exceptions = new List<RecognitionException>();
        List<AggregateException> aggregateExceptionList = new List<AggregateException>();
        List<Exception> exceptionList = new List<Exception>();
        foreach (Exception innerException in aggregateException2.InnerExceptions)
        {
          switch (innerException)
          {
            case RecognitionException recognitionException:
              exceptions.Add(recognitionException);
              continue;
            case AggregateException aggregateException:
              aggregateExceptionList.Add(aggregateException);
              continue;
            default:
              exceptionList.Add(innerException);
              continue;
          }
        }
        foreach (BuildWorkflowException buildError in exceptions.CreateBuildErrors(sourceFile))
          this.HandleError(inputFile, (Exception) buildError, sourceFile);
        foreach (Exception ex1 in aggregateExceptionList)
          this.HandleCssAggregateException(ex1, sourceFile, inputFile);
        foreach (Exception ex2 in exceptionList)
          this.HandleError(inputFile, ex2, sourceFile);
      }
      else
        this.HandleError(inputFile, ex, sourceFile);
    }

    private IEnumerable<ContentItem> MinifyJs(
      IEnumerable<ContentItem> inputFiles,
      JsMinificationConfig jsConfig,
      JSValidationConfig jsValidateConfig)
    {
      List<ContentItem> contentItemList = new List<ContentItem>();
      MinifyJSActivity minifyJsActivity = new MinifyJSActivity(this.context)
      {
        ShouldMinify = jsConfig.ShouldMinify,
        ShouldAnalyze = jsValidateConfig.ShouldAnalyze,
        AnalyzeArgs = jsValidateConfig.AnalyzeArguments
      };
      if (!string.IsNullOrWhiteSpace(jsConfig.GlobalsToIgnore))
        minifyJsActivity.MinifyArgs = "/global:" + jsConfig.GlobalsToIgnore + (object) ' ' + jsConfig.MinificationArugments;
      else
        minifyJsActivity.MinifyArgs = jsConfig.MinificationArugments;
      foreach (ContentItem inputFile in inputFiles)
      {
        string relativeContentPath = inputFile.RelativeContentPath;
        if (!this.context.TemporaryIgnore(inputFile.ResourcePivotKeys))
        {
          this.context.Log.Information("Js Minify start: {0}{1}".InvariantFormat((object) relativeContentPath, (object) string.Join(string.Empty, inputFile.ResourcePivotKeys.Select<ResourcePivotKey, string>((Func<ResourcePivotKey, string>) (p => p.ToString())))));
          try
          {
            contentItemList.Add(minifyJsActivity.Minify(inputFile));
          }
          catch (Exception ex)
          {
            contentItemList.Add((ContentItem) null);
            this.HandleError(inputFile, ex, relativeContentPath);
          }
        }
      }
      return (IEnumerable<ContentItem>) contentItemList;
    }

    private IEnumerable<ContentItem> ApplyResources(
      IEnumerable<ContentItem> inputItems,
      Dictionary<string, IDictionary<string, IDictionary<string, string>>> mergedResource)
    {
      if (!mergedResource.Any<KeyValuePair<string, IDictionary<string, IDictionary<string, string>>>>())
        return inputItems;
      List<ContentItem> contentItemList = new List<ContentItem>();
      foreach (ContentItem inputItem in inputItems)
      {
        try
        {
          contentItemList.AddRange(ResourcePivotActivity.ApplyResourceKeys(inputItem, mergedResource));
        }
        catch (Exception ex)
        {
          this.HandleError(inputItem, ex, inputItem.RelativeContentPath);
          contentItemList.Add((ContentItem) null);
        }
      }
      return (IEnumerable<ContentItem>) contentItemList;
    }

    private ContentItem BundleFiles(
      IEnumerable<InputSpec> inputSpecs,
      string outputFile,
      PreprocessingConfig preprocessing,
      FileTypes fileType,
      bool minimalOutput)
    {
      AssemblerActivity assemblerActivity = new AssemblerActivity(this.context)
      {
        PreprocessingConfig = preprocessing,
        AddSemicolons = fileType == FileTypes.JS,
        MinimalOutput = minimalOutput
      };
      foreach (InputSpec inputSpec in inputSpecs)
        assemblerActivity.Inputs.Add(inputSpec);
      assemblerActivity.OutputFile = outputFile;
      try
      {
        return assemblerActivity.Execute(ContentItemType.Value);
      }
      catch (Exception ex)
      {
        this.HandleError((ContentItem) null, ex);
      }
      return (ContentItem) null;
    }

    private void HandleError(ContentItem contentItem, Exception ex, string file = null)
    {
      if (ex.InnerException is BuildWorkflowException)
        ex = ex.InnerException;
      BuildWorkflowException workflowException = ex as BuildWorkflowException;
      if (contentItem != null)
      {
        file = this.context.EnsureErrorFileOnDisk(workflowException != null ? workflowException.File : file, contentItem);
        if (workflowException != null)
          workflowException.File = file;
      }
      if (!string.IsNullOrWhiteSpace(file) && (workflowException == null || workflowException.File.IsNullOrWhitespace()))
        this.context.Log.Error((Exception) null, string.Format((IFormatProvider) CultureInfo.InvariantCulture, ResourceStrings.ErrorsInFileFormat, new object[1]
        {
          (object) file
        }), file);
      this.context.Log.Error(ex, ex.ToString());
      if (ex is AggregateException aggregateException)
      {
        foreach (Exception innerException in aggregateException.InnerExceptions)
          this.HandleError(contentItem, innerException);
      }
      else
      {
        if (ex.InnerException == null)
          return;
        this.HandleError(contentItem, ex.InnerException);
      }
    }
  }
}
