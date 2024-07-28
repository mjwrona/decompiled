// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Common.DiffUtil
// Assembly: Microsoft.TeamFoundation.VersionControl.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 156CCB01-0A1F-468C-A332-06DB9F9B179E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Common.dll

using Microsoft.TeamFoundation.Diff;
using Microsoft.TeamFoundation.VersionControl.Common.Internal;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace Microsoft.TeamFoundation.VersionControl.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class DiffUtil
  {
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
        diffOptions.StreamWriter.WriteLine(Resources.Get("DiffSeparator"));
        diffOutput.Output((IList<DiffLine>) original, (IList<DiffLine>) modified, diffList);
        diffOptions.StreamWriter.WriteLine(Resources.Get("DiffSeparator"));
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
