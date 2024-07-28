// Decompiled with JetBrains decompiler
// Type: WebGrease.Activities.MinifyCssActivity
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WebGrease.Common;
using WebGrease.Configuration;
using WebGrease.Css;
using WebGrease.Css.Ast;
using WebGrease.Css.Extensions;
using WebGrease.Css.ImageAssemblyAnalysis;
using WebGrease.Css.ImageAssemblyAnalysis.LogModel;
using WebGrease.Css.Visitor;
using WebGrease.Extensions;
using WebGrease.ImageAssemble;

namespace WebGrease.Activities
{
  internal sealed class MinifyCssActivity
  {
    private static readonly Regex UrlHashRegexPattern = new Regex("url\\((?<quote>[\"']?)(?:hash\\((?<url>[^)]*))\\)(?<extra>.*?)\\k<quote>\\)", RegexOptions.Compiled);
    private readonly IWebGreaseContext context;
    private IDictionary<string, string> availableSourceImages;

    internal MinifyCssActivity(IWebGreaseContext context)
    {
      this.context = context;
      this.HackSelectors = new HashSet<string>();
      this.BannedSelectors = new HashSet<string>();
      this.ShouldExcludeProperties = true;
      this.ShouldValidateForLowerCase = false;
      this.ShouldOptimize = true;
      this.ShouldAssembleBackgroundImages = true;
      this.ImageAssembleReferencesToIgnore = new HashSet<string>();
      this.OutputUnitFactor = 1.0;
      this.ShouldPreventOrderBasedConflict = false;
      this.ShouldMergeBasedOnCommonDeclarations = false;
    }

    internal string ImageBasePrefixToRemoveFromOutputPathInLog { get; set; }

    internal string ImageBasePrefixToAddToOutputPath { get; set; }

    internal string OutputUnit { private get; set; }

    internal string MissingImageUrl { private get; set; }

    internal double OutputUnitFactor { private get; set; }

    internal ImageType? ForcedSpritingImageType { get; set; }

    internal bool IgnoreImagesWithNonDefaultBackgroundSize { private get; set; }

    internal IList<string> ImageDirectories { private get; set; }

    internal IList<string> ImageExtensions { private get; set; }

    internal string SourceFile { private get; set; }

    internal string DestinationFile { get; set; }

    internal bool ShouldExcludeProperties { get; set; }

    internal bool ShouldValidateForLowerCase { get; set; }

    internal bool ShouldOptimize { private get; set; }

    internal bool ShouldMergeMediaQueries { private get; set; }

    internal bool ShouldAssembleBackgroundImages { private get; set; }

    internal bool ShouldMinify { get; set; }

    internal HashSet<string> HackSelectors { get; set; }

    internal HashSet<string> BannedSelectors { get; set; }

    internal HashSet<string> NonMergeSelectors { get; set; }

    internal string ImageAssembleScanDestinationFile { get; set; }

    internal string ImageSpritingLogPath { get; set; }

    internal string ImagesOutputDirectory { private get; set; }

    internal HashSet<string> ImageAssembleReferencesToIgnore { get; set; }

    internal int? ImageAssemblyPadding { private get; set; }

    internal bool ErrorOnInvalidSprite { get; set; }

    internal HashSet<float> Dpi { get; set; }

    internal IDictionary<string, IDictionary<string, string>> DpiResources { get; set; }

    internal Dictionary<string, IDictionary<string, IDictionary<string, string>>> MergedResources { get; set; }

    internal bool ShouldPreventOrderBasedConflict { get; set; }

    internal bool ShouldMergeBasedOnCommonDeclarations { get; set; }

    internal MinifyCssResult Process(ContentItem contentItem, FileHasherActivity imageHasher = null)
    {
      if (imageHasher != null)
        this.availableSourceImages = this.context.GetAvailableFiles(this.context.Configuration.SourceDirectory, (IEnumerable<string>) this.ImageDirectories, (IEnumerable<string>) this.ImageExtensions, FileTypes.Image);
      string content = contentItem.Content;
      BlockingCollection<ContentItem> minifiedContentItems = new BlockingCollection<ContentItem>();
      BlockingCollection<ContentItem> hashedImageContentItems = new BlockingCollection<ContentItem>();
      BlockingCollection<ContentItem> spritedImageContentItems = new BlockingCollection<ContentItem>();
      Dictionary<ResourcePivotKey[], IDictionary<string, IDictionary<string, string>>> groupedResources = ResourcePivotActivity.GetUsedGroupedResources(content, this.MergedResources);
      HashSet<float> floatSet = this.Dpi;
      if (floatSet == null || !floatSet.Any<float>())
        floatSet = new HashSet<float>((IEnumerable<float>) new float[1]
        {
          1f
        });
      MinifyCssPivot[] array = ((IEnumerable<MinifyCssPivot>) MinifyCssActivity.GetMinifyCssPivots(contentItem, (IEnumerable<float>) floatSet, groupedResources, this.DpiResources).ToArray<MinifyCssPivot>()).Where<MinifyCssPivot>((Func<MinifyCssPivot, bool>) (p => !this.context.TemporaryIgnore((IEnumerable<ResourcePivotKey>) p.NewContentResourcePivotKeys))).ToArray<MinifyCssPivot>();
      StyleSheetNode parsedStylesheetNode = CssParser.Parse(this.context, content, false);
      this.context.ParallelForEach<MinifyCssPivot>((Func<MinifyCssPivot, string[]>) (item => new string[1]
      {
        nameof (MinifyCssActivity)
      }), (IEnumerable<MinifyCssPivot>) array, (Func<IWebGreaseContext, MinifyCssPivot, ParallelLoopState, bool>) ((threadContext, pivot, parallelLoopState) =>
      {
        ContentItem minifiedContentItem = (ContentItem) null;
        ContentItem varByContentItem = ContentItem.FromContent(contentItem.Content, pivot.NewContentResourcePivotKeys);
        bool flag = threadContext.SectionedAction(nameof (MinifyCssActivity), nameof (Process)).MakeCachable(varByContentItem, this.GetVarBySettings(imageHasher, (IEnumerable<ResourcePivotKey>) pivot.NewContentResourcePivotKeys, pivot.MergedResource)).RestoreFromCacheAction((Func<ICacheSection, bool>) (cacheSection =>
        {
          minifiedContentItem = cacheSection.GetCachedContentItem("MinifyCssResult", contentItem.RelativeContentPath, contentItem.AbsoluteDiskPath, (IEnumerable<ResourcePivotKey>) pivot.NewContentResourcePivotKeys);
          hashedImageContentItems.AddRange<ContentItem>(cacheSection.GetCachedContentItems("HashedImage"));
          spritedImageContentItems.AddRange<ContentItem>(cacheSection.GetCachedContentItems("HashedSpriteImage"));
          if (minifiedContentItem == null)
          {
            this.context.Log.Error("Css minify cache result is null");
            return false;
          }
          if (spritedImageContentItems.Any<ContentItem>((Func<ContentItem, bool>) (hi => hi == null)))
          {
            this.context.Log.Error("Sprited image cache result is null");
            return false;
          }
          if (!hashedImageContentItems.Any<ContentItem>((Func<ContentItem, bool>) (hi => hi == null)))
            return true;
          this.context.Log.Error("Hashed image cache result is null");
          return false;
        })).Execute((Func<ICacheSection, bool>) (cacheSection =>
        {
          try
          {
            StyleSheetNode stylesheetNode = MinifyCssActivity.ApplyResources((AstNode) parsedStylesheetNode, pivot.MergedResource, threadContext) as StyleSheetNode;
            stylesheetNode = this.ApplyValidation((AstNode) stylesheetNode, threadContext) as StyleSheetNode;
            stylesheetNode = this.ApplyOptimization((AstNode) stylesheetNode, threadContext) as StyleSheetNode;
            stylesheetNode = this.ApplySpriting((AstNode) stylesheetNode, pivot.Dpi, spritedImageContentItems, threadContext) as StyleSheetNode;
            string str = threadContext.SectionedAction(nameof (MinifyCssActivity), "PrintCss").Execute<string>((Func<string>) (() => !this.ShouldMinify ? stylesheetNode.PrettyPrint() : stylesheetNode.MinifyPrint()));
            if (imageHasher != null)
            {
              Tuple<string, IEnumerable<ContentItem>> tuple = MinifyCssActivity.HashImages(str, imageHasher, cacheSection, threadContext, this.availableSourceImages, this.MissingImageUrl);
              str = tuple.Item1;
              hashedImageContentItems.AddRange<ContentItem>(tuple.Item2);
            }
            minifiedContentItem = ContentItem.FromContent(str, this.DestinationFile, (string) null, pivot.NewContentResourcePivotKeys);
            cacheSection.AddResult(minifiedContentItem, "MinifyCssResult");
          }
          catch (Exception ex)
          {
            this.context.Log.Error(ex, ex.ToString());
            return false;
          }
          return true;
        }));
        Safe.Lock((object) minifiedContentItems, (Action) (() => minifiedContentItems.Add(minifiedContentItem)));
        if (!flag)
          this.context.Log.Error("An errror occurred while minifying '{0}' with resources '{1}'".InvariantFormat((object) contentItem.RelativeContentPath, (object) pivot));
        return flag;
      }));
      return new MinifyCssResult((IEnumerable<ContentItem>) minifiedContentItems, (IEnumerable<ContentItem>) spritedImageContentItems.DistinctBy<ContentItem, string>((Func<ContentItem, string>) (hi => hi.RelativeContentPath)).ToArray<ContentItem>(), (IEnumerable<ContentItem>) hashedImageContentItems.DistinctBy<ContentItem, string>((Func<ContentItem, string>) (hi => hi.RelativeContentPath)).ToArray<ContentItem>());
    }

    internal void Execute(ContentItem contentItem = null, FileHasherActivity imageHasher = null)
    {
      if (contentItem == null)
      {
        if (string.IsNullOrWhiteSpace(this.SourceFile))
          throw new ArgumentException("MinifyCssActivity - The source file cannot be null or whitespace.");
        if (!File.Exists(this.SourceFile))
          throw new FileNotFoundException("MinifyCssActivity - The source file cannot be found.", this.SourceFile);
      }
      if (string.IsNullOrWhiteSpace(this.DestinationFile))
        throw new ArgumentException("MinifyCssActivity - The destination file cannot be null or whitespace.");
      if (contentItem == null)
        contentItem = ContentItem.FromFile(this.SourceFile, Path.IsPathRooted(this.SourceFile) ? this.SourceFile.MakeRelativeToDirectory(this.context.Configuration.SourceDirectory) : this.SourceFile, (string) null);
      MinifyCssResult minifyCssResult = this.Process(contentItem, imageHasher);
      minifyCssResult.Css.FirstOrDefault<ContentItem>()?.WriteTo(this.DestinationFile);
      if (minifyCssResult.SpritedImages != null && minifyCssResult.SpritedImages.Any<ContentItem>())
      {
        foreach (ContentItem spritedImage in minifyCssResult.SpritedImages)
          spritedImage.WriteToContentPath(this.context.Configuration.DestinationDirectory);
      }
      if (minifyCssResult.HashedImages == null || !minifyCssResult.HashedImages.Any<ContentItem>())
        return;
      foreach (ContentItem hashedImage in minifyCssResult.HashedImages)
        hashedImage.WriteToRelativeHashedPath(this.context.Configuration.DestinationDirectory);
    }

    private static Tuple<string, IEnumerable<ContentItem>> HashImages(
      string cssContent,
      FileHasherActivity imageHasher,
      ICacheSection cacheSection,
      IWebGreaseContext threadContext,
      IDictionary<string, string> sourceImages,
      string missingImage)
    {
      return threadContext.SectionedAction(nameof (MinifyCssActivity), "ImageHash").Execute<Tuple<string, IEnumerable<ContentItem>>>((Func<Tuple<string, IEnumerable<ContentItem>>>) (() =>
      {
        HashSet<string> contentImagesToHash = new HashSet<string>();
        List<ContentItem> hashedContentItems = new List<ContentItem>();
        Dictionary<string, string> hashedImages = new Dictionary<string, string>();
        cssContent = MinifyCssActivity.UrlHashRegexPattern.Replace(cssContent, (MatchEvaluator) (match =>
        {
          string str6 = match.Groups["url"].Value;
          string str7 = match.Groups["extra"].Value;
          if (ResourcesResolver.LocalizationResourceKeyRegex.IsMatch(str6))
            return match.Value;
          string str8 = str6.NormalizeUrl();
          string str9 = sourceImages.TryGetValue<string, string>(str8);
          if (str9 == null && !string.IsNullOrWhiteSpace(missingImage))
            str9 = sourceImages.TryGetValue<string, string>(missingImage);
          if (str9 == null)
            throw new BuildWorkflowException("Could not find a matching source image for url: {0}".InvariantFormat((object) str6));
          string str10;
          if (contentImagesToHash.Add(str8))
          {
            ContentItem contentItem = imageHasher.Hash(ContentItem.FromFile(str9, str8, (string) null));
            cacheSection.AddSourceDependency(str9);
            hashedContentItems.Add(contentItem);
            str10 = Path.Combine(imageHasher.BasePrefixToAddToOutputPath ?? Path.AltDirectorySeparatorChar.ToString((IFormatProvider) CultureInfo.InvariantCulture), contentItem.RelativeHashedContentPath.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
            hashedImages.Add(str8, str10);
          }
          else
            str10 = hashedImages[str8];
          return "url(" + str10 + str7 + ")";
        }));
        return Tuple.Create<string, IEnumerable<ContentItem>>(cssContent, (IEnumerable<ContentItem>) hashedContentItems);
      }));
    }

    private static IEnumerable<MinifyCssPivot> GetMinifyCssPivots(
      ContentItem contentItem,
      IEnumerable<float> dpiValues,
      Dictionary<ResourcePivotKey[], IDictionary<string, IDictionary<string, string>>> mergedResources,
      IDictionary<string, IDictionary<string, string>> allDpiResources)
    {
      IEnumerable<ResourcePivotKey> contentResourcePivotKeys = contentItem.ResourcePivotKeys ?? (IEnumerable<ResourcePivotKey>) new ResourcePivotKey[0];
      IEnumerable<\u003C\u003Ef__AnonymousType9<float, string, ResourcePivotKey, IDictionary<string, string>>> dpiPivots = dpiValues.Select(dpi =>
      {
        string resolutionName = EverythingActivity.DpiToResolutionName(dpi);
        IDictionary<string, string> dictionary = (IDictionary<string, string>) null;
        allDpiResources?.TryGetValue(resolutionName, out dictionary);
        ResourcePivotKey resourcePivotKey = new ResourcePivotKey(nameof (dpi), resolutionName);
        return new
        {
          dpi = dpi,
          dpiResolutionName = resolutionName,
          dpiResourcePivotKey = resourcePivotKey,
          dpiResources = dictionary
        };
      });
      return mergedResources.SelectMany<KeyValuePair<ResourcePivotKey[], IDictionary<string, IDictionary<string, string>>>, MinifyCssPivot>((Func<KeyValuePair<ResourcePivotKey[], IDictionary<string, IDictionary<string, string>>>, IEnumerable<MinifyCssPivot>>) (mergedResourceValues =>
      {
        List<IDictionary<string, string>> mergedResource = mergedResourceValues.Value.Values.ToList<IDictionary<string, string>>();
        return dpiPivots.Select(dpiPivot =>
        {
          List<IDictionary<string, string>> list = mergedResource.ToList<IDictionary<string, string>>();
          if (dpiPivot.dpiResources != null)
            list.Add(dpiPivot.dpiResources);
          return new MinifyCssPivot((IEnumerable<IDictionary<string, string>>) list, contentResourcePivotKeys.Concat<ResourcePivotKey>((IEnumerable<ResourcePivotKey>) mergedResourceValues.Key).Concat<ResourcePivotKey>((IEnumerable<ResourcePivotKey>) new ResourcePivotKey[1]
          {
            dpiPivot.dpiResourcePivotKey
          }).ToArray<ResourcePivotKey>(), dpiPivot.dpi);
        });
      }));
    }

    private static AstNode ApplyResources(
      AstNode stylesheetNode,
      IEnumerable<IDictionary<string, string>> resources,
      IWebGreaseContext threadContext)
    {
      if (resources.Any<IDictionary<string, string>>())
        threadContext.SectionedAction(nameof (MinifyCssActivity), "ResourcesResolution").Execute((Action) (() => stylesheetNode = stylesheetNode.Accept((NodeVisitor) new ResourceResolutionVisitor(resources))));
      return stylesheetNode;
    }

    private static ImageLog RestoreSpritedImagesFromCache(
      string mapXmlFile,
      ICacheSection cacheSection,
      BlockingCollection<ContentItem> results,
      string destinationDirectory,
      string imageAssembleScanDestinationFile)
    {
      ContentItem contentItem = cacheSection.GetCachedContentItems("SpriteLogFile").FirstOrDefault<ContentItem>();
      if (contentItem == null)
        return (ImageLog) null;
      if (!string.IsNullOrWhiteSpace(imageAssembleScanDestinationFile))
        cacheSection.GetCachedContentItems("SpriteLogFileXml").FirstOrDefault<ContentItem>()?.WriteTo(mapXmlFile);
      ImageLog imageLog = contentItem.Content.FromJson<ImageLog>(true);
      IEnumerable<ContentItem> cachedContentItems = cacheSection.GetCachedContentItems("HashedSpriteImage");
      cachedContentItems.ForEach<ContentItem>((Action<ContentItem>) (sici => sici.WriteToContentPath(destinationDirectory)));
      results.AddRange<ContentItem>(cachedContentItems);
      return imageLog;
    }

    private static string GetRelativeSpriteCacheKey(
      IEnumerable<InputImage> imageReferencesToAssemble,
      IWebGreaseContext threadContext)
    {
      return string.Join(">", imageReferencesToAssemble.Select<InputImage, string>((Func<InputImage, string>) (ir => "{0}|{1}|{2}".InvariantFormat((object) threadContext.MakeRelativeToApplicationRoot(ir.AbsoluteImagePath), (object) ir.Position, (object) string.Join(":", ir.DuplicateImagePaths.Select<string, string>(new Func<string, string>(threadContext.MakeRelativeToApplicationRoot)))))));
    }

    private object GetVarBySettings(
      FileHasherActivity imageHasher,
      IEnumerable<ResourcePivotKey> resourcePivotKeys,
      IEnumerable<IDictionary<string, string>> dpiResources)
    {
      return (object) new
      {
        resourcePivotKeys = resourcePivotKeys,
        dpiResources = dpiResources,
        ShouldExcludeProperties = this.ShouldExcludeProperties,
        ShouldValidateForLowerCase = this.ShouldValidateForLowerCase,
        ShouldMergeMediaQueries = this.ShouldMergeMediaQueries,
        ShouldOptimize = this.ShouldOptimize,
        ShouldAssembleBackgroundImages = this.ShouldAssembleBackgroundImages,
        ShouldMinify = this.ShouldMinify,
        ShouldPreventOrderBasedConflict = this.ShouldPreventOrderBasedConflict,
        ShouldMergeBasedOnCommonDeclarations = this.ShouldMergeBasedOnCommonDeclarations,
        IgnoreImagesWithNonDefaultBackgroundSize = this.IgnoreImagesWithNonDefaultBackgroundSize,
        HackSelectors = this.HackSelectors,
        BannedSelectors = this.BannedSelectors,
        NonMergeSelectors = this.NonMergeSelectors,
        ImageAssembleReferencesToIgnore = this.ImageAssembleReferencesToIgnore,
        OutputUnit = this.OutputUnit,
        OutputUnitFactor = this.OutputUnitFactor,
        ImageAssemblyPadding = this.ImageAssemblyPadding,
        HashImages = (imageHasher == null),
        ForcedSpritingImageType = this.ForcedSpritingImageType,
        ErrorOnInvalidSprite = this.ErrorOnInvalidSprite,
        ImageAssembleScanDestinationFile = this.ImageAssembleScanDestinationFile,
        ImageSpritingLogPath = this.ImageSpritingLogPath
      };
    }

    private AstNode ApplySpriting(
      AstNode stylesheetNode,
      float dpi,
      BlockingCollection<ContentItem> spritedImageContentItems,
      IWebGreaseContext threadContext)
    {
      if (this.ShouldAssembleBackgroundImages)
        stylesheetNode = this.SpriteBackgroundImages(stylesheetNode, dpi, threadContext, spritedImageContentItems);
      return stylesheetNode;
    }

    private AstNode ApplyOptimization(AstNode stylesheetNode, IWebGreaseContext threadContext)
    {
      if (this.ShouldOptimize)
        threadContext.SectionedAction(nameof (MinifyCssActivity), "Optimize").Execute((Action) (() =>
        {
          stylesheetNode = stylesheetNode.Accept((NodeVisitor) new OptimizationVisitor()
          {
            ShouldMergeMediaQueries = this.ShouldMergeMediaQueries,
            ShouldPreventOrderBasedConflict = this.ShouldPreventOrderBasedConflict,
            ShouldMergeBasedOnCommonDeclarations = this.ShouldMergeBasedOnCommonDeclarations,
            NonMergeRuleSetSelectors = (IEnumerable<string>) this.NonMergeSelectors
          });
          stylesheetNode = stylesheetNode.Accept((NodeVisitor) new ColorOptimizationVisitor());
          stylesheetNode = stylesheetNode.Accept((NodeVisitor) new FloatOptimizationVisitor());
        }));
      return stylesheetNode;
    }

    private AstNode ApplyValidation(AstNode stylesheetNode, IWebGreaseContext threadContext)
    {
      threadContext.SectionedAction(nameof (MinifyCssActivity), "Validate").Execute((Action) (() =>
      {
        if (this.ShouldExcludeProperties)
          stylesheetNode = stylesheetNode.Accept((NodeVisitor) new ExcludePropertyVisitor());
        if (this.ShouldValidateForLowerCase)
          stylesheetNode = stylesheetNode.Accept((NodeVisitor) new ValidateLowercaseVisitor());
        if (this.HackSelectors != null && this.HackSelectors.Any<string>())
          stylesheetNode = stylesheetNode.Accept((NodeVisitor) new SelectorValidationOptimizationVisitor(this.HackSelectors, false, true));
        if (this.BannedSelectors == null || !this.BannedSelectors.Any<string>())
          return;
        stylesheetNode = stylesheetNode.Accept((NodeVisitor) new SelectorValidationOptimizationVisitor(this.BannedSelectors, false, false));
      }));
      return stylesheetNode;
    }

    private AstNode SpriteBackgroundImages(
      AstNode stylesheetNode,
      float dpi,
      IWebGreaseContext threadContext,
      BlockingCollection<ContentItem> spritedImageContentItems)
    {
      return threadContext.SectionedAction(nameof (MinifyCssActivity), "Spriting").Execute<AstNode>((Func<AstNode>) (() =>
      {
        ImageAssemblyScanVisitor assemblyScanVisitor = this.ExecuteImageAssemblyScan(stylesheetNode, threadContext);
        List<ImageLog> imageLogList = new List<ImageLog>();
        int num = 0;
        foreach (ImageAssemblyScanOutput assemblyScanOutput in (IEnumerable<ImageAssemblyScanOutput>) assemblyScanVisitor.ImageAssemblyScanOutputs)
        {
          ImageLog imageLog = this.SpriteImageFromLog(assemblyScanOutput, this.ImageAssembleScanDestinationFile + (num == 0 ? string.Empty : "." + (object) num) + ".xml", assemblyScanVisitor.ImageAssemblyAnalysisLog, threadContext, spritedImageContentItems);
          if (imageLog != null)
          {
            imageLogList.Add(imageLog);
            ++num;
          }
        }
        stylesheetNode = this.ExecuteImageAssemblyUpdate(stylesheetNode, (IEnumerable<ImageLog>) imageLogList, dpi);
        if (!string.IsNullOrWhiteSpace(this.ImageSpritingLogPath))
          assemblyScanVisitor.ImageAssemblyAnalysisLog.Save(this.ImageSpritingLogPath);
        ImageAssemblyAnalysisLog assemblyAnalysisLog = assemblyScanVisitor.ImageAssemblyAnalysisLog;
        if (this.ErrorOnInvalidSprite && assemblyAnalysisLog.FailedSprites.Any<WebGrease.Css.ImageAssemblyAnalysis.LogModel.ImageAssemblyAnalysis>())
        {
          foreach (WebGrease.Css.ImageAssemblyAnalysis.LogModel.ImageAssemblyAnalysis failedSprite in assemblyAnalysisLog.FailedSprites)
          {
            string failureMessage = ImageAssemblyAnalysisLog.GetFailureMessage(failedSprite);
            if (!string.IsNullOrWhiteSpace(failedSprite.Image))
              threadContext.Log.Error("Failed to sprite image {0}\r\nReason:{1}\r\nCss:{2}".InvariantFormat((object) failedSprite.Image, (object) failureMessage, (object) failedSprite.AstNode.PrettyPrint()));
            else
              threadContext.Log.Error("Failed to sprite:{0}\r\nReason:{1}".InvariantFormat((object) failedSprite.Image, (object) failureMessage));
          }
        }
        return stylesheetNode;
      }));
    }

    private ImageLog SpriteImageFromLog(
      ImageAssemblyScanOutput scanOutput,
      string mapXmlFile,
      ImageAssemblyAnalysisLog imageAssemblyAnalysisLog,
      IWebGreaseContext threadContext,
      BlockingCollection<ContentItem> spritedImageContentItems)
    {
      if (scanOutput == null || !scanOutput.ImageReferencesToAssemble.Any<InputImage>())
        return (ImageLog) null;
      ImageLog imageLog = (ImageLog) null;
      IList<InputImage> imageReferencesToAssemble = scanOutput.ImageReferencesToAssemble;
      if (imageReferencesToAssemble == null || imageReferencesToAssemble.Count == 0)
        return (ImageLog) null;
      var varBySettings = new
      {
        imageMap = MinifyCssActivity.GetRelativeSpriteCacheKey((IEnumerable<InputImage>) imageReferencesToAssemble, threadContext),
        ImageAssemblyPadding = this.ImageAssemblyPadding
      };
      return !threadContext.SectionedAction(nameof (MinifyCssActivity), "Spriting", "Assembly").MakeCachable((object) varBySettings, infiniteWaitForLock: true).RestoreFromCacheAction((Func<ICacheSection, bool>) (cacheSection =>
      {
        imageLog = MinifyCssActivity.RestoreSpritedImagesFromCache(mapXmlFile, cacheSection, spritedImageContentItems, threadContext.Configuration.DestinationDirectory, this.ImageAssembleScanDestinationFile);
        return imageLog != null;
      })).Execute((Func<ICacheSection, bool>) (cacheSection =>
      {
        imageLog = this.CreateSpritedImages(mapXmlFile, imageAssemblyAnalysisLog, (IEnumerable<InputImage>) imageReferencesToAssemble, cacheSection, spritedImageContentItems, threadContext);
        return imageLog != null;
      })) ? (ImageLog) null : imageLog;
    }

    private ImageLog CreateSpritedImages(
      string mapXmlFile,
      ImageAssemblyAnalysisLog imageAssemblyAnalysisLog,
      IEnumerable<InputImage> imageReferencesToAssemble,
      ICacheSection cacheSection,
      BlockingCollection<ContentItem> results,
      IWebGreaseContext threadContext)
    {
      if (!Directory.Exists(this.ImagesOutputDirectory))
        Directory.CreateDirectory(this.ImagesOutputDirectory);
      ImageMap imageMap = ImageAssembleGenerator.AssembleImages(imageReferencesToAssemble.ToSafeReadOnlyCollection<InputImage>(), SpritePackingType.Vertical, this.ImagesOutputDirectory, string.Empty, true, threadContext, this.ImageAssemblyPadding, imageAssemblyAnalysisLog, this.ForcedSpritingImageType);
      if (imageMap == null || imageMap.Document == null)
        return (ImageLog) null;
      string destinationDirectory = threadContext.Configuration.DestinationDirectory;
      if (!string.IsNullOrWhiteSpace(this.ImageAssembleScanDestinationFile))
      {
        string content = imageMap.Document.ToString();
        FileHelper.WriteFile(mapXmlFile, content);
        cacheSection.AddResult(ContentItem.FromFile(mapXmlFile, (string) null, (string) null), "SpriteLogFileXml");
      }
      ImageLog spritedImages = new ImageLog(imageMap.Document);
      cacheSection.AddResult(ContentItem.FromContent(spritedImages.ToJson(true)), "SpriteLogFile");
      foreach (string str in spritedImages.InputImages.Select<AssembledImage, string>((Func<AssembledImage, string>) (il => il.OutputFilePath)).Distinct<string>())
      {
        ContentItem contentItem = ContentItem.FromFile(str, str.MakeRelativeToDirectory(destinationDirectory), (string) null);
        results.Add(contentItem);
        cacheSection.AddResult(contentItem, "HashedSpriteImage");
      }
      return spritedImages;
    }

    private ImageAssemblyScanVisitor ExecuteImageAssemblyScan(
      AstNode stylesheetNode,
      IWebGreaseContext threadContext)
    {
      ImageAssemblyScanVisitor assemblyScanVisitor = new ImageAssemblyScanVisitor(this.SourceFile, (IEnumerable<string>) this.ImageAssembleReferencesToIgnore, this.IgnoreImagesWithNonDefaultBackgroundSize, this.OutputUnit, this.OutputUnitFactor, this.availableSourceImages, this.MissingImageUrl, true)
      {
        Context = threadContext
      };
      stylesheetNode.Accept((NodeVisitor) assemblyScanVisitor);
      return assemblyScanVisitor;
    }

    private AstNode ExecuteImageAssemblyUpdate(
      AstNode stylesheetNode,
      IEnumerable<ImageLog> imageLogs,
      float dpi)
    {
      ImageAssemblyUpdateVisitor assemblyUpdateVisitor = new ImageAssemblyUpdateVisitor(this.SourceFile, imageLogs, dpi, this.OutputUnit, this.OutputUnitFactor, this.ImageBasePrefixToRemoveFromOutputPathInLog, this.ImageBasePrefixToAddToOutputPath, this.availableSourceImages, this.MissingImageUrl);
      stylesheetNode = stylesheetNode.Accept((NodeVisitor) assemblyUpdateVisitor);
      return stylesheetNode;
    }
  }
}
