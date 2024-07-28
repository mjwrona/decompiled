// Decompiled with JetBrains decompiler
// Type: WebGrease.WebGreaseContext
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebGrease.Activities;
using WebGrease.Configuration;
using WebGrease.Extensions;
using WebGrease.Preprocessing;

namespace WebGrease
{
  public class WebGreaseContext : IWebGreaseContext
  {
    private const string IdPartsDelimiter = ".";
    private static readonly ConcurrentDictionary<string, Tuple<DateTime, long, string>> CachedFileHashes = new ConcurrentDictionary<string, Tuple<DateTime, long, string>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private static readonly ThreadLocal<MD5CryptoServiceProvider> Hasher = new ThreadLocal<MD5CryptoServiceProvider>((Func<MD5CryptoServiceProvider>) (() => new MD5CryptoServiceProvider()));
    private static readonly ThreadLocal<Encoding> DefaultEncoding = new ThreadLocal<Encoding>((Func<Encoding>) (() => (Encoding) new UTF8Encoding(false, true)));
    private readonly ConcurrentDictionary<string, string> sessionCachedFileHashes = new ConcurrentDictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private readonly IDictionary<string, IDictionary<string, string>> availableFileCollections = (IDictionary<string, IDictionary<string, string>>) new Dictionary<string, IDictionary<string, string>>();
    private readonly List<KeyValuePair<string, IEnumerable<TimeMeasureResult>>> threadedMeasureResults = new List<KeyValuePair<string, IEnumerable<TimeMeasureResult>>>();

    public WebGreaseContext(IWebGreaseContext parentContext, FileInfo configFile)
    {
      WebGreaseConfiguration configuration = new WebGreaseConfiguration(parentContext.Configuration, configFile);
      configuration.Validate();
      if (configuration.Global.TreatWarningsAsErrors.HasValue && parentContext.Log != null)
      {
        LogManager log = parentContext.Log;
        bool? warningsAsErrors = configuration.Global.TreatWarningsAsErrors;
        int num = !warningsAsErrors.GetValueOrDefault() ? 0 : (warningsAsErrors.HasValue ? 1 : 0);
        log.TreatWarningsAsErrors = num != 0;
      }
      if (parentContext is WebGreaseContext webGreaseContext)
        this.threadedMeasureResults = webGreaseContext.threadedMeasureResults;
      this.Initialize(configuration, parentContext.Log, parentContext.Cache, parentContext.Preprocessing, parentContext.SessionStartTime, parentContext.Measure);
    }

    public WebGreaseContext(
      WebGreaseConfiguration configuration,
      LogManager logManager,
      ICacheSection parentCacheSection = null,
      PreprocessingManager preprocessingManager = null)
    {
      DateTimeOffset now = DateTimeOffset.Now;
      configuration.Validate();
      ITimeMeasure timeMeasure = configuration.Measure ? (ITimeMeasure) new TimeMeasure() : (ITimeMeasure) new NullTimeMeasure();
      ICacheManager cacheManager = configuration.CacheEnabled ? (ICacheManager) new CacheManager(configuration, logManager, parentCacheSection) : (ICacheManager) new NullCacheManager();
      this.Initialize(configuration, logManager, cacheManager, preprocessingManager != null ? new PreprocessingManager(preprocessingManager) : new PreprocessingManager(configuration, logManager, timeMeasure), now, timeMeasure);
    }

    public WebGreaseContext(
      WebGreaseConfiguration configuration,
      Action<string, MessageImportance> logInformation = null,
      Action<string> logWarning = null,
      LogExtendedError logExtendedWarning = null,
      Action<string> logErrorMessage = null,
      LogError logError = null,
      LogExtendedError logExtendedError = null)
      : this(configuration, new LogManager(logInformation, logWarning, logExtendedWarning, logErrorMessage, logError, logExtendedError, configuration.Global.TreatWarningsAsErrors))
    {
    }

    public ICacheManager Cache { get; private set; }

    public WebGreaseConfiguration Configuration { get; private set; }

    public LogManager Log { get; private set; }

    public ITimeMeasure Measure { get; private set; }

    public PreprocessingManager Preprocessing { get; private set; }

    public DateTimeOffset SessionStartTime { get; private set; }

    public IEnumerable<KeyValuePair<string, IEnumerable<TimeMeasureResult>>> ThreadedMeasureResults => (IEnumerable<KeyValuePair<string, IEnumerable<TimeMeasureResult>>>) this.threadedMeasureResults;

    public IWebGreaseSection SectionedAction(params string[] idParts) => WebGreaseSection.Create((IWebGreaseContext) this, idParts, false);

    public IWebGreaseSection SectionedActionGroup(params string[] idParts) => WebGreaseSection.Create((IWebGreaseContext) this, idParts, true);

    public bool TemporaryIgnore(IFileSet fileSet, ContentItem contentItem)
    {
      if (this.Configuration == null || this.Configuration.Overrides == null)
        return false;
      return this.Configuration.Overrides.ShouldIgnore(fileSet) || this.Configuration.Overrides.ShouldIgnore(contentItem);
    }

    public bool TemporaryIgnore(IEnumerable<ResourcePivotKey> resourcePivotKey) => this.Configuration != null && this.Configuration.Overrides != null && this.Configuration.Overrides.ShouldIgnore(resourcePivotKey);

    public void CleanCache(LogManager logManager = null)
    {
      string rootPath = this.Cache.RootPath;
      (logManager ?? this.Log).Information("Cleaning Cache: {0}".InvariantFormat((object) rootPath), MessageImportance.High);
      this.CleanDirectory(rootPath, new string[1]
      {
        "webgrease.caching.lock"
      });
    }

    public void CleanDestination()
    {
      string destinationDirectory = this.Configuration.DestinationDirectory;
      this.Log.Information("Cleaning Destination: {0}".InvariantFormat((object) destinationDirectory), MessageImportance.High);
      this.CleanDirectory(destinationDirectory);
      string logsDirectory = this.Configuration.LogsDirectory;
      this.Log.Information("Cleaning Destination: {0}".InvariantFormat((object) logsDirectory), MessageImportance.High);
      this.CleanDirectory(logsDirectory);
    }

    public IDictionary<string, string> GetAvailableFiles(
      string rootDirectory,
      IEnumerable<string> directories,
      IEnumerable<string> extensions,
      FileTypes fileType)
    {
      string json = new
      {
        rootDirectory = rootDirectory,
        directories = directories,
        extensions = extensions,
        fileType = fileType
      }.ToJson();
      IDictionary<string, string> availableFiles;
      if (!this.availableFileCollections.TryGetValue(json, out availableFiles))
      {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        if (directories == null)
          return (IDictionary<string, string>) dictionary;
        foreach (string directory in directories)
        {
          foreach (string extension in extensions)
            dictionary.AddRange<string, string>((IEnumerable<KeyValuePair<string, string>>) ((IEnumerable<string>) Directory.GetFiles(directory, extension, SearchOption.AllDirectories)).Select<string, string>((Func<string, string>) (f => f.ToLowerInvariant())).ToDictionary<string, string, string>((Func<string, string>) (f => f.MakeRelativeToDirectory(rootDirectory)), (Func<string, string>) (f => f)));
        }
        this.availableFileCollections.Add(json, availableFiles = (IDictionary<string, string>) dictionary);
      }
      return availableFiles;
    }

    public string GetValueHash(string value) => this.SectionedAction(new string[1]
    {
      "ContentHash"
    }).Execute<string>((Func<string>) (() => WebGreaseContext.ComputeContentHash(value ?? string.Empty)));

    public string GetBitmapHash(Bitmap bitmap, ImageFormat format)
    {
      lock (bitmap)
        return this.SectionedAction(new string[1]
        {
          "BitmapHash"
        }).Execute<string>((Func<string>) (() => WebGreaseContext.ComputeBitmapHash(bitmap, format)));
    }

    public string GetContentItemHash(ContentItem contentItem) => this.SectionedAction(new string[1]
    {
      "ContentHash"
    }).Execute<string>((Func<string>) (() => contentItem.GetContentHash((IWebGreaseContext) this)));

    public string GetFileHash(string filePath)
    {
      string fileHash1 = (string) null;
      FileInfo fileInfo = new FileInfo(filePath);
      string key = fileInfo.Exists ? fileInfo.FullName : throw new FileNotFoundException("Could not find the file to create a hash for", filePath);
      if (this.sessionCachedFileHashes.TryGetValue(key, out fileHash1))
        return fileHash1;
      Tuple<DateTime, long, string> tuple;
      WebGreaseContext.CachedFileHashes.TryGetValue(key, out tuple);
      if (tuple != null && tuple.Item1 == fileInfo.LastWriteTimeUtc && tuple.Item2 == fileInfo.Length)
        return tuple.Item3;
      string fileHash2 = WebGreaseContext.ComputeFileHash(fileInfo.FullName);
      WebGreaseContext.CachedFileHashes[key] = new Tuple<DateTime, long, string>(fileInfo.LastWriteTimeUtc, fileInfo.Length, fileHash2);
      this.sessionCachedFileHashes[key] = fileHash2;
      return fileHash2;
    }

    public string MakeRelativeToApplicationRoot(string absolutePath) => absolutePath.MakeRelativeTo(this.Configuration.ApplicationRootDirectory);

    public string GetWorkingSourceDirectory(string relativePath)
    {
      string str = this.Configuration.SourceDirectory ?? string.Empty;
      FileInfo fileInfo = new FileInfo(Path.Combine(str, relativePath));
      return !str.IsNullOrWhitespace() && !fileInfo.FullName.StartsWith(str, StringComparison.OrdinalIgnoreCase) ? str : fileInfo.DirectoryName;
    }

    public void Touch(string filePath)
    {
      DateTime utcDateTime = this.SessionStartTime.UtcDateTime;
      try
      {
        File.SetLastWriteTimeUtc(filePath, utcDateTime);
      }
      catch (Exception ex)
      {
      }
    }

    public string EnsureErrorFileOnDisk(string sourceFile, ContentItem sourceContentItem)
    {
      if (sourceContentItem == null)
        return sourceFile;
      if (sourceFile.IsNullOrWhitespace() || !File.Exists(sourceFile))
      {
        sourceFile = sourceContentItem.RelativeContentPath;
        if (sourceFile.IsNullOrWhitespace())
          sourceFile = Guid.NewGuid().ToString().Replace("-", string.Empty);
        if (sourceContentItem.ResourcePivotKeys != null)
        {
          ResourcePivotKey resourcePivotKey = sourceContentItem.ResourcePivotKeys.FirstOrDefault<ResourcePivotKey>();
          if (resourcePivotKey != null)
          {
            string extension = Path.GetExtension(sourceFile);
            sourceFile = Path.ChangeExtension(sourceFile, "." + resourcePivotKey.ToString("{0}.{1}") + extension);
          }
        }
      }
      sourceFile = sourceFile.NormalizeUrl();
      if (!Path.IsPathRooted(sourceFile))
        sourceFile = Path.Combine(this.Configuration.IntermediateErrorDirectory, sourceFile);
      sourceContentItem.WriteTo(sourceFile);
      return sourceFile;
    }

    public void ParallelForEach<T>(
      Func<T, string[]> idParts,
      IEnumerable<T> items,
      Func<IWebGreaseContext, T, ParallelLoopState, bool> parallelAction,
      Func<IWebGreaseContext, T, bool> serialAction = null)
    {
      this.SectionedAction(idParts(default (T))).Execute((Action) (() =>
      {
        object serialLock = new object();
        List<Tuple<IWebGreaseContext, DelayedLogManager, T>> parallelForEachItems = new List<Tuple<IWebGreaseContext, DelayedLogManager, T>>();
        int done = 0;
        foreach (T obj in items)
        {
          DelayedLogManager delayedLogManager = new DelayedLogManager(this.Log, obj.ToString());
          WebGreaseContext webGreaseContext = new WebGreaseContext(new WebGreaseConfiguration(this.Configuration), delayedLogManager.LogManager, this.Cache.CurrentCacheSection, this.Preprocessing);
          bool flag = true;
          if (serialAction != null)
            flag = serialAction((IWebGreaseContext) webGreaseContext, obj);
          if (flag)
            parallelForEachItems.Add(new Tuple<IWebGreaseContext, DelayedLogManager, T>((IWebGreaseContext) webGreaseContext, delayedLogManager, obj));
        }
        Parallel.ForEach<Tuple<IWebGreaseContext, DelayedLogManager, T>>((IEnumerable<Tuple<IWebGreaseContext, DelayedLogManager, T>>) parallelForEachItems, (Action<Tuple<IWebGreaseContext, DelayedLogManager, T>, ParallelLoopState>) ((item, state) =>
        {
          string sectionId = WebGreaseContext.ToStringId((IEnumerable<string>) idParts(item.Item3));
          int num = parallelAction(item.Item1, item.Item3, state) ? 1 : 0;
          TimeMeasureResult[] measureResult = item.Item1.Measure.GetResults();
          Safe.Lock(serialLock, int.MaxValue, (Action) (() =>
          {
            this.threadedMeasureResults.AddRange(item.Item1.ThreadedMeasureResults);
            this.threadedMeasureResults.Add(new KeyValuePair<string, IEnumerable<TimeMeasureResult>>(sectionId, (IEnumerable<TimeMeasureResult>) measureResult));
            item.Item2.Flush();
            ++done;
            if (done != parallelForEachItems.Count - 1)
              return;
            parallelForEachItems.ForEach((Action<Tuple<IWebGreaseContext, DelayedLogManager, T>>) (i => i.Item2.Flush()));
          }));
        }));
      }));
    }

    internal static string ToStringId(IEnumerable<string> idParts) => string.Join(".", idParts);

    internal static IEnumerable<string> ToIdParts(string id) => (IEnumerable<string>) id.Split("."[0]);

    internal static string ComputeContentHash(string content, Encoding encoding = null)
    {
      using (MemoryStream inputStream = new MemoryStream())
      {
        StreamWriter streamWriter = new StreamWriter((Stream) inputStream, encoding ?? WebGreaseContext.DefaultEncoding.Value);
        streamWriter.Write(content);
        streamWriter.Flush();
        inputStream.Seek(0L, SeekOrigin.Begin);
        return WebGreaseContext.BytesToHash(WebGreaseContext.Hasher.Value.ComputeHash((Stream) inputStream));
      }
    }

    internal static string ComputeFileHash(string filePath)
    {
      using (FileStream inputStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
        return WebGreaseContext.BytesToHash(WebGreaseContext.Hasher.Value.ComputeHash((Stream) inputStream));
    }

    private static bool DelTree(string directory, string[] filesToIgnore)
    {
      bool flag = false;
      foreach (string file1 in Directory.GetFiles(directory))
      {
        string file = file1;
        if (filesToIgnore == null || !((IEnumerable<string>) filesToIgnore).Any<string>((Func<string, bool>) (fti => file.EndsWith(fti, StringComparison.OrdinalIgnoreCase))))
          File.Delete(file);
        else
          flag = true;
      }
      foreach (string directory1 in Directory.GetDirectories(directory))
        flag |= WebGreaseContext.DelTree(directory1, filesToIgnore);
      if (!flag)
        Directory.Delete(directory);
      return flag;
    }

    private static string ComputeBitmapHash(Bitmap bitmap, ImageFormat format)
    {
      using (MemoryStream inputStream = new MemoryStream())
      {
        bitmap.Save((Stream) inputStream, format);
        inputStream.Seek(0L, SeekOrigin.Begin);
        return WebGreaseContext.BytesToHash(WebGreaseContext.Hasher.Value.ComputeHash((Stream) inputStream));
      }
    }

    private static string BytesToHash(byte[] hash) => BitConverter.ToString(hash).Replace("-", string.Empty).ToLower(CultureInfo.InvariantCulture);

    private void CleanDirectory(string directory, string[] filesToIgnore = null)
    {
      try
      {
        if (!string.IsNullOrWhiteSpace(directory) && Directory.Exists(directory))
          WebGreaseContext.DelTree(directory, filesToIgnore);
        if (Directory.Exists(directory))
          return;
        Directory.CreateDirectory(directory);
      }
      catch (Exception ex)
      {
        this.Log.Warning("Error while cleaning {0}: {1}".InvariantFormat((object) directory, (object) ex.Message));
      }
    }

    private void Initialize(
      WebGreaseConfiguration configuration,
      LogManager logManager,
      ICacheManager cacheManager,
      PreprocessingManager preprocessingManager,
      DateTimeOffset runStartTime,
      ITimeMeasure timeMeasure)
    {
      if (configuration == null)
        throw new ArgumentNullException(nameof (configuration));
      if (configuration.Global.TreatWarningsAsErrors.HasValue)
      {
        LogManager logManager1 = logManager;
        bool? warningsAsErrors = configuration.Global.TreatWarningsAsErrors;
        int num = !warningsAsErrors.GetValueOrDefault() ? 0 : (warningsAsErrors.HasValue ? 1 : 0);
        logManager1.TreatWarningsAsErrors = num != 0;
      }
      this.Configuration = configuration;
      this.Configuration.Validate();
      this.Measure = timeMeasure;
      this.Log = logManager;
      this.Cache = cacheManager;
      this.Preprocessing = preprocessingManager;
      this.SessionStartTime = runStartTime;
      this.Cache.SetContext((IWebGreaseContext) this);
      this.Preprocessing.SetContext((IWebGreaseContext) this);
    }
  }
}
