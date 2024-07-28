// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.Common.DiffSummary
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F16CDF2D-8103-4EAE-A2A8-4FA5B1C1BE58
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.Server.Common.dll

using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace Microsoft.VisualStudio.Services.CodeReview.Server.Common
{
  public class DiffSummary
  {
    private int m_originalLineCount;
    private int m_modifiedLineCount;
    private int m_totalLinesAdded;
    private int m_totalLinesModified;
    private int m_totalLinesDeleted;
    private IDiffChange[] m_changes;

    public DiffSummary(
      DiffFile original,
      DiffFile modified,
      IDiffChange[] diffList,
      bool includeChanges)
    {
      this.m_originalLineCount = original.Count;
      this.m_modifiedLineCount = modified.Count;
      if (includeChanges)
        this.m_changes = diffList;
      foreach (IDiffChange diff in diffList)
      {
        switch (diff.ChangeType)
        {
          case DiffChangeType.Insert:
            this.m_totalLinesAdded += diff.ModifiedLength;
            break;
          case DiffChangeType.Delete:
            this.m_totalLinesDeleted += diff.OriginalLength;
            break;
          case DiffChangeType.Change:
            if (diff.OriginalLength >= diff.ModifiedLength)
            {
              this.m_totalLinesModified += diff.ModifiedLength;
              this.m_totalLinesDeleted += diff.OriginalLength - diff.ModifiedLength;
              break;
            }
            this.m_totalLinesModified += diff.OriginalLength;
            this.m_totalLinesAdded += diff.ModifiedLength - diff.OriginalLength;
            break;
        }
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public DiffSummary(
      int originalLineCount,
      int modifiedLineCount,
      int totalLinesAdded,
      int totalLinesModified,
      int totalLinesDeleted)
    {
      this.m_originalLineCount = originalLineCount;
      this.m_modifiedLineCount = modifiedLineCount;
      this.m_totalLinesAdded = totalLinesAdded;
      this.m_totalLinesModified = totalLinesModified;
      this.m_totalLinesDeleted = totalLinesDeleted;
    }

    public int OriginalLineCount => this.m_originalLineCount;

    public int ModifiedLineCount => this.m_modifiedLineCount;

    public int TotalLinesAdded => this.m_totalLinesAdded;

    public int TotalLinesModified => this.m_totalLinesModified;

    public int TotalLinesDeleted => this.m_totalLinesDeleted;

    public IDiffChange[] Changes => this.m_changes;

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("DiffSummary instance ");
      stringBuilder.AppendLine(this.GetHashCode().ToString((IFormatProvider) CultureInfo.InvariantCulture));
      stringBuilder.Append("    OriginalLineCount: ");
      stringBuilder.AppendLine(this.OriginalLineCount.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      stringBuilder.Append("    ModifiedLineCount: ");
      stringBuilder.AppendLine(this.ModifiedLineCount.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      stringBuilder.Append("    TotalLinesAdded: ");
      stringBuilder.AppendLine(this.TotalLinesAdded.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      stringBuilder.Append("    TotalLinesDeleted: ");
      stringBuilder.AppendLine(this.TotalLinesDeleted.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      stringBuilder.Append("    TotalLinesModified: ");
      stringBuilder.AppendLine(this.TotalLinesModified.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return stringBuilder.ToString();
    }
  }
}
