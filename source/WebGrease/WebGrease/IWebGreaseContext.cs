// Decompiled with JetBrains decompiler
// Type: WebGrease.IWebGreaseContext
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using WebGrease.Activities;
using WebGrease.Configuration;
using WebGrease.Preprocessing;

namespace WebGrease
{
  public interface IWebGreaseContext
  {
    ICacheManager Cache { get; }

    WebGreaseConfiguration Configuration { get; }

    LogManager Log { get; }

    ITimeMeasure Measure { get; }

    PreprocessingManager Preprocessing { get; }

    DateTimeOffset SessionStartTime { get; }

    IEnumerable<KeyValuePair<string, IEnumerable<TimeMeasureResult>>> ThreadedMeasureResults { get; }

    void CleanCache(LogManager logManager = null);

    void CleanDestination();

    IDictionary<string, string> GetAvailableFiles(
      string rootDirectory,
      IEnumerable<string> directories,
      IEnumerable<string> extensions,
      FileTypes fileType);

    string GetValueHash(string value);

    string GetFileHash(string filePath);

    string MakeRelativeToApplicationRoot(string absolutePath);

    string GetWorkingSourceDirectory(string relativePath);

    void Touch(string filePath);

    IWebGreaseSection SectionedAction(params string[] idParts);

    IWebGreaseSection SectionedActionGroup(params string[] idParts);

    bool TemporaryIgnore(IFileSet fileSet, ContentItem contentItem);

    bool TemporaryIgnore(IEnumerable<ResourcePivotKey> resourcePivotKey);

    string EnsureErrorFileOnDisk(string sourceFile, ContentItem sourceContentItem);

    void ParallelForEach<T>(
      Func<T, string[]> idParts,
      IEnumerable<T> items,
      Func<IWebGreaseContext, T, ParallelLoopState, bool> parallelAction,
      Func<IWebGreaseContext, T, bool> serialAction = null);

    string GetBitmapHash(Bitmap bitmap, ImageFormat format);
  }
}
