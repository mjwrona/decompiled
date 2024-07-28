// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DevSecOps.Common.DiffUtil
// Assembly: Microsoft.VisualStudio.Services.DevSecOps.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 072F1303-F456-426E-A1CB-C0838641751B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DevSecOps.Common.dll

using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace Microsoft.VisualStudio.Services.DevSecOps.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class DiffUtil
  {
    public static string DiffSeparator = "===================================================================";

    public static void DiffFiles(
      string originalPath,
      Encoding originalEncoding,
      string modifiedPath,
      Encoding modifiedEncoding,
      DiffOptions diffOptions,
      string fileLabel)
    {
      DiffFinder<DiffLine> lcsDiff = DiffFinder<DiffLine>.LcsDiff;
      DiffOutput diffOutput = DiffOutput.Create(diffOptions);
      DiffFile original = DiffFile.Create(originalPath, originalEncoding, diffOptions);
      DiffFile modified = DiffFile.Create(modifiedPath, modifiedEncoding, diffOptions);
      DiffLineComparer elementComparer = new DiffLineComparer(diffOptions);
      using (lcsDiff)
      {
        IDiffChange[] diffList = lcsDiff.Diff((IList<DiffLine>) original, (IList<DiffLine>) modified, (IEqualityComparer<DiffLine>) elementComparer);
        if (diffList.Length == 0)
          return;
        diffOptions.StreamWriter.WriteLine(fileLabel);
        diffOptions.StreamWriter.WriteLine(DiffUtil.DiffSeparator);
        diffOutput.Output((IList<DiffLine>) original, (IList<DiffLine>) modified, diffList);
        diffOptions.StreamWriter.WriteLine(DiffUtil.DiffSeparator);
      }
    }

    public static DiffSummary Diff(
      Stream originalStream,
      Encoding originalEncoding,
      Stream modifiedStream,
      Encoding modifiedEncoding,
      DiffOptions diffOptions,
      bool includeChangesInSummary)
    {
      DiffFinder<DiffLine> lcsDiff = DiffFinder<DiffLine>.LcsDiff;
      DiffFile original = DiffFile.Create(originalStream, originalEncoding, diffOptions);
      DiffFile modified = DiffFile.Create(modifiedStream, modifiedEncoding, diffOptions);
      DiffLineComparer elementComparer = new DiffLineComparer(diffOptions);
      using (lcsDiff)
      {
        IDiffChange[] diffList = lcsDiff.Diff((IList<DiffLine>) original, (IList<DiffLine>) modified, (IEqualityComparer<DiffLine>) elementComparer);
        return new DiffSummary(original, modified, diffList, includeChangesInSummary);
      }
    }

    public static DiffSummary DiffFiles(
      string originalPath,
      Encoding originalEncoding,
      string modifiedPath,
      Encoding modifiedEncoding,
      DiffOptions diffOptions,
      bool includeChangesInSummary)
    {
      DiffFinder<DiffLine> lcsDiff = DiffFinder<DiffLine>.LcsDiff;
      DiffFile original = DiffFile.Create(originalPath, originalEncoding, diffOptions);
      DiffFile modified = DiffFile.Create(modifiedPath, modifiedEncoding, diffOptions);
      DiffLineComparer elementComparer = new DiffLineComparer(diffOptions);
      using (lcsDiff)
      {
        IDiffChange[] diffList = lcsDiff.Diff((IList<DiffLine>) original, (IList<DiffLine>) modified, (IEqualityComparer<DiffLine>) elementComparer);
        return new DiffSummary(original, modified, diffList, includeChangesInSummary);
      }
    }
  }
}
