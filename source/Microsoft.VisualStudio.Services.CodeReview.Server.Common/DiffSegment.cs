// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.Common.DiffSegment
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F16CDF2D-8103-4EAE-A2A8-4FA5B1C1BE58
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.Server.Common.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.VisualStudio.Services.CodeReview.Server.Common
{
  public class DiffSegment
  {
    private DiffSegment m_next;
    private DiffSegmentType m_type;
    private long m_originalStartOffset;
    private long m_originalByteLength;
    private int m_originalStart;
    private int m_originalLength;
    private long m_modifiedStartOffset;
    private int m_modifiedStart;
    private int m_modifiedLength;
    private long m_modifiedByteLength;
    private long m_latestStartOffset;
    private int m_latestStart;
    private int m_latestLength;
    private long m_latestByteLength;
    private IDiffChange[] m_charDiffs;

    public long OriginalByteLength
    {
      get => this.m_originalByteLength;
      set => this.m_originalByteLength = value;
    }

    public long ModifiedByteLength
    {
      get => this.m_modifiedByteLength;
      set => this.m_modifiedByteLength = value;
    }

    public long OriginalStartOffset
    {
      get => this.m_originalStartOffset;
      set => this.m_originalStartOffset = value;
    }

    public long ModifiedStartOffset
    {
      get => this.m_modifiedStartOffset;
      set => this.m_modifiedStartOffset = value;
    }

    public DiffSegment Next
    {
      get => this.m_next;
      set => this.m_next = value;
    }

    public DiffSegmentType Type
    {
      get => this.m_type;
      set => this.m_type = value;
    }

    public int OriginalStart
    {
      get => this.m_originalStart;
      set => this.m_originalStart = value;
    }

    public int OriginalLength
    {
      get => this.m_originalLength;
      set => this.m_originalLength = value;
    }

    public int ModifiedStart
    {
      get => this.m_modifiedStart;
      set => this.m_modifiedStart = value;
    }

    public int ModifiedLength
    {
      get => this.m_modifiedLength;
      set => this.m_modifiedLength = value;
    }

    public int LatestStart
    {
      get => this.m_latestStart;
      set => this.m_latestStart = value;
    }

    public int LatestLength
    {
      get => this.m_latestLength;
      set => this.m_latestLength = value;
    }

    public long LatestStartOffset
    {
      get => this.m_latestStartOffset;
      set => this.m_latestStartOffset = value;
    }

    public long LatestByteLength
    {
      get => this.m_latestByteLength;
      set => this.m_latestByteLength = value;
    }

    public IDiffChange[] CharDiffs
    {
      get => this.m_charDiffs;
      set => this.m_charDiffs = value;
    }

    public static DiffSegment Convert(
      IDiffChange[] diffList,
      int originalLength,
      int modifiedLength)
    {
      return DiffSegment.Convert(((IEnumerable<IDiffChange>) diffList).Select<IDiffChange, LineChanges>((Func<IDiffChange, LineChanges>) (x => new LineChanges(x, (IDiffChange[]) null))).ToArray<LineChanges>(), originalLength, modifiedLength);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static DiffSegment Convert(
      LineChanges[] diffList,
      int originalLength,
      int modifiedLength)
    {
      DiffSegment diffSegment1 = (DiffSegment) null;
      DiffSegment diffSegment2 = (DiffSegment) null;
      int num1 = 0;
      int num2 = 0;
      for (int index = 0; index <= diffList.Length; ++index)
      {
        LineChanges diff = index < diffList.Length ? diffList[index] : (LineChanges) null;
        IDiffChange lineChange = diff?.LineChange;
        int num3 = (lineChange != null ? lineChange.ModifiedStart : modifiedLength) - num2;
        DiffSegment diffSegment3 = new DiffSegment();
        diffSegment3.Type = DiffSegmentType.Common;
        diffSegment3.OriginalStart = num1;
        diffSegment3.OriginalLength = num3;
        diffSegment3.ModifiedStart = num2;
        diffSegment3.ModifiedLength = num3;
        diffSegment3.CharDiffs = diff?.CharChange;
        if (diffSegment2 == null)
        {
          diffSegment2 = diffSegment3;
          diffSegment1 = diffSegment2;
        }
        else
        {
          diffSegment2.Next = diffSegment3;
          diffSegment2 = diffSegment3;
        }
        if (lineChange != null)
        {
          num1 = lineChange.OriginalStart + lineChange.OriginalLength;
          num2 = lineChange.ModifiedStart + lineChange.ModifiedLength;
        }
      }
      return diffSegment1;
    }
  }
}
