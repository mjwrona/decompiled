// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DevSecOps.Common.DiffOutput
// Assembly: Microsoft.VisualStudio.Services.DevSecOps.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 072F1303-F456-426E-A1CB-C0838641751B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DevSecOps.Common.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace Microsoft.VisualStudio.Services.DevSecOps.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public abstract class DiffOutput
  {
    private DiffOptions m_options;

    protected DiffOptions Options => this.m_options;

    protected StreamWriter Out => this.m_options.StreamWriter;

    protected void WriteElementRange(
      IList<DiffLine> sequence,
      string prefix,
      int startIndex,
      int endIndex)
    {
      if (startIndex >= sequence.Count || endIndex < 0)
        return;
      startIndex = Math.Max(startIndex, 0);
      endIndex = Math.Min(endIndex, sequence.Count - 1);
      for (int index = startIndex; index <= endIndex; ++index)
      {
        this.Out.Write(prefix);
        this.Out.WriteLine(sequence[index].Content);
      }
    }

    protected void WriteUnixChangeHeader(IDiffChange change)
    {
      this.Out.Write("{0:d}", (object) (change.ChangeType == DiffChangeType.Insert ? change.OriginalStart : change.OriginalStart + 1));
      if (change.OriginalLength > 1)
        this.Out.Write(",{0:d}", (object) (change.OriginalStart + change.OriginalLength));
      switch (change.ChangeType)
      {
        case DiffChangeType.Insert:
          this.Out.Write("a");
          break;
        case DiffChangeType.Delete:
          this.Out.Write("d");
          break;
        case DiffChangeType.Change:
          this.Out.Write("c");
          break;
      }
      if (change.ModifiedLength > 1)
        this.Out.WriteLine("{0:d},{1:d}", (object) (change.ChangeType == DiffChangeType.Delete ? change.ModifiedStart : change.ModifiedStart + 1), (object) (change.ModifiedStart + change.ModifiedLength));
      else
        this.Out.WriteLine("{0:d}", (object) (change.ChangeType == DiffChangeType.Delete ? change.ModifiedStart : change.ModifiedStart + 1));
    }

    protected int ComputeHunkEnd(IDiffChange[] diffList, int hunkStart, int contextLines)
    {
      int hunkEnd;
      for (hunkEnd = hunkStart; hunkEnd + 1 < diffList.Length; ++hunkEnd)
      {
        IDiffChange diff1 = diffList[hunkEnd];
        IDiffChange diff2 = diffList[hunkEnd + 1];
        if (diff1.OriginalStart + diff1.OriginalLength + 2 * contextLines < diff2.OriginalStart)
          break;
      }
      return hunkEnd;
    }

    public abstract void Output(
      IList<DiffLine> original,
      IList<DiffLine> modified,
      IDiffChange[] diffList);

    public static DiffOutput Create(DiffOptions options)
    {
      DiffOutput diffOutput = (DiffOutput) null;
      switch (options.OutputType)
      {
        case DiffOutputType.Context:
          diffOutput = (DiffOutput) new Context();
          break;
        case DiffOutputType.RCS:
          diffOutput = (DiffOutput) new Rcs();
          break;
        case DiffOutputType.Ed:
          diffOutput = (DiffOutput) new Ed();
          break;
        case DiffOutputType.Unified:
          diffOutput = (DiffOutput) new Unified();
          break;
        case DiffOutputType.UnixNormal:
          diffOutput = (DiffOutput) new UnixNormal();
          break;
        case DiffOutputType.VSS:
          diffOutput = (DiffOutput) new Vss();
          break;
        case DiffOutputType.Binary:
          diffOutput = (DiffOutput) new Binary();
          break;
      }
      if (diffOutput != null)
        diffOutput.m_options = options;
      return diffOutput;
    }
  }
}
