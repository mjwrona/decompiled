// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Common.Vss
// Assembly: Microsoft.TeamFoundation.VersionControl.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 156CCB01-0A1F-468C-A332-06DB9F9B179E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Common.dll

using Microsoft.TeamFoundation.Diff;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.TeamFoundation.VersionControl.Common
{
  internal class Vss : DiffOutput
  {
    private const int MaxWidth = 512;
    private const int UnlimitedWidth = -1;

    public override void Output(
      IList<DiffLine> original,
      IList<DiffLine> modified,
      IDiffChange[] diffList)
    {
      int contextLines = Math.Max(this.Options.ContextLines, 0);
      int num = this.Options.VssOutputWidth;
      if (this.Options.VssFormatType == VssOutputType.SideBySide && (num == -1 || num > 512))
        num = 512;
      if (num != -1)
      {
        switch (this.Options.VssFormatType)
        {
          case VssOutputType.Vss:
            num -= 12;
            break;
          case VssOutputType.Unix:
            num -= 2;
            break;
          case VssOutputType.SideBySide:
            num = (num - 5) / 2;
            break;
        }
        num = Math.Max(num, 0);
      }
      bool flag = false;
      int index = 0;
      while (index < diffList.Length)
      {
        int hunkStart = index;
        int hunkEnd = this.ComputeHunkEnd(diffList, hunkStart, contextLines);
        int startIndex = Math.Max(diffList[hunkStart].OriginalStart - contextLines, 0);
        Math.Max(diffList[hunkStart].ModifiedStart - contextLines, 0);
        int endIndex1 = Math.Min(diffList[hunkEnd].OriginalStart + diffList[hunkEnd].OriginalLength + contextLines - 1, original.Count - 1);
        Math.Min(diffList[hunkEnd].ModifiedStart + diffList[hunkEnd].ModifiedLength + contextLines - 1, modified.Count - 1);
        if (flag)
          this.Out.WriteLine("********");
        else
          flag = true;
        switch (this.Options.VssFormatType)
        {
          case VssOutputType.Vss:
            this.WriteElementRange(original, "            ", startIndex, diffList[hunkStart].OriginalStart - 1, num);
            break;
          case VssOutputType.Unix:
            this.WriteElementRange(original, "  ", startIndex, diffList[hunkStart].OriginalStart - 1, num);
            break;
          case VssOutputType.SideBySide:
            this.WriteSideBySideRange(original, "     ", startIndex, diffList[hunkStart].OriginalStart - 1, num, Vss.Column.Both);
            break;
        }
        for (; index <= hunkEnd; ++index)
        {
          IDiffChange diff = diffList[index];
          int endIndex2 = diff.OriginalStart + diff.OriginalLength - 1;
          int endIndex3 = diff.ModifiedStart + diff.ModifiedLength - 1;
          switch (diff.ChangeType)
          {
            case DiffChangeType.Insert:
              switch (this.Options.VssFormatType)
              {
                case VssOutputType.Vss:
                  this.WriteVssRange(modified, "{0,3}    Ins: {1}", diff.ModifiedStart, endIndex3, num);
                  break;
                case VssOutputType.Unix:
                  this.WriteUnixChangeHeader(diff);
                  this.WriteElementRange(modified, "> ", diff.ModifiedStart, endIndex3, num);
                  break;
                case VssOutputType.SideBySide:
                  this.WriteSideBySideRange(modified, "  >  ", diff.ModifiedStart, endIndex3, num, Vss.Column.Right);
                  break;
              }
              break;
            case DiffChangeType.Delete:
              switch (this.Options.VssFormatType)
              {
                case VssOutputType.Vss:
                  this.WriteVssRange(original, "{0,3}    Del: {1}", diff.OriginalStart, endIndex2, num);
                  break;
                case VssOutputType.Unix:
                  this.WriteUnixChangeHeader(diff);
                  this.WriteElementRange(original, "< ", diff.OriginalStart, endIndex2, num);
                  break;
                case VssOutputType.SideBySide:
                  this.WriteSideBySideRange(original, "  <  ", diff.OriginalStart, endIndex2, num, Vss.Column.Left);
                  break;
              }
              break;
            case DiffChangeType.Change:
              switch (this.Options.VssFormatType)
              {
                case VssOutputType.Vss:
                  int originalStart1 = diff.OriginalStart;
                  int modifiedStart1;
                  for (modifiedStart1 = diff.ModifiedStart; originalStart1 <= endIndex2 && modifiedStart1 <= endIndex3; ++modifiedStart1)
                  {
                    string str1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0,3} Change: {1}", (object) (originalStart1 + 1), (object) original[originalStart1].Content);
                    if (num != -1 && str1.Length > num)
                      str1 = str1.Substring(0, num);
                    this.Out.WriteLine(str1);
                    string str2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "        To: {0}", (object) modified[modifiedStart1].Content);
                    if (num != -1 && str2.Length > num)
                      str2 = str2.Substring(0, num);
                    this.Out.WriteLine(str2);
                    ++originalStart1;
                  }
                  if (originalStart1 <= endIndex2)
                  {
                    this.WriteVssRange(original, "{0,3}    Del: {1}", originalStart1, endIndex2, num);
                    break;
                  }
                  if (modifiedStart1 <= endIndex3)
                  {
                    this.WriteVssRange(modified, "{0,3}    Ins: {1}", modifiedStart1, endIndex3, num);
                    break;
                  }
                  break;
                case VssOutputType.Unix:
                  this.WriteUnixChangeHeader(diff);
                  this.WriteElementRange(original, "< ", diff.OriginalStart, endIndex2, num);
                  this.Out.WriteLine("---");
                  this.WriteElementRange(modified, "> ", diff.ModifiedStart, endIndex3, num);
                  break;
                case VssOutputType.SideBySide:
                  int originalStart2 = diff.OriginalStart;
                  int modifiedStart2;
                  for (modifiedStart2 = diff.ModifiedStart; originalStart2 <= endIndex2 && modifiedStart2 <= endIndex3; ++modifiedStart2)
                  {
                    string content = original[originalStart2].Content;
                    this.Out.Write(content.Length <= num ? content.PadRight(num) : content.Substring(0, num));
                    this.Out.Write("  |  ");
                    string str = modified[modifiedStart2].Content;
                    if (str.Length > num)
                      str = str.Substring(0, num);
                    this.Out.WriteLine(str);
                    ++originalStart2;
                  }
                  if (originalStart2 <= endIndex2)
                  {
                    this.WriteSideBySideRange(original, "  <  ", originalStart2, endIndex2, num, Vss.Column.Left);
                    break;
                  }
                  if (modifiedStart2 <= endIndex3)
                  {
                    this.WriteSideBySideRange(modified, "  >  ", modifiedStart2, endIndex3, num, Vss.Column.Right);
                    break;
                  }
                  break;
              }
              break;
          }
          if (index < hunkEnd)
          {
            switch (this.Options.VssFormatType)
            {
              case VssOutputType.Vss:
                this.WriteElementRange(original, "            ", endIndex2 + 1, diffList[index + 1].OriginalStart - 1, num);
                continue;
              case VssOutputType.Unix:
                this.WriteElementRange(original, "  ", endIndex2 + 1, diffList[index + 1].OriginalStart - 1, num);
                continue;
              case VssOutputType.SideBySide:
                this.WriteSideBySideRange(original, "     ", endIndex2 + 1, diffList[index + 1].OriginalStart - 1, num, Vss.Column.Both);
                continue;
              default:
                continue;
            }
          }
        }
        switch (this.Options.VssFormatType)
        {
          case VssOutputType.Vss:
            this.WriteElementRange(original, "            ", diffList[hunkEnd].OriginalStart + diffList[hunkEnd].OriginalLength, endIndex1, num);
            continue;
          case VssOutputType.Unix:
            this.WriteElementRange(original, "  ", diffList[hunkEnd].OriginalStart + diffList[hunkEnd].OriginalLength, endIndex1, num);
            continue;
          case VssOutputType.SideBySide:
            this.WriteSideBySideRange(original, "     ", diffList[hunkEnd].OriginalStart + diffList[hunkEnd].OriginalLength, endIndex1, num, Vss.Column.Both);
            continue;
          default:
            continue;
        }
      }
    }

    protected void WriteElementRange(
      IList<DiffLine> sequence,
      string prefix,
      int startIndex,
      int endIndex,
      int maxLength)
    {
      if (startIndex >= sequence.Count || endIndex < 0)
        return;
      startIndex = Math.Max(startIndex, 0);
      endIndex = Math.Min(endIndex, sequence.Count - 1);
      bool flag = maxLength == -1;
      for (int index = startIndex; index <= endIndex; ++index)
      {
        this.Out.Write(prefix);
        if (flag || sequence[index].Content.Length + prefix.Length <= maxLength)
          this.Out.WriteLine(sequence[index].Content);
        else
          this.Out.WriteLine(sequence[index].Content.Substring(0, Math.Max(maxLength - prefix.Length, 0)));
      }
    }

    private void WriteVssRange(
      IList<DiffLine> sequence,
      string template,
      int startIndex,
      int endIndex,
      int maxLength)
    {
      bool flag = maxLength == -1;
      for (int index = startIndex; index <= endIndex; ++index)
      {
        string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, template, (object) (index + 1), (object) sequence[index].Content);
        if (!flag && str.Length > maxLength)
          str = str.Substring(0, maxLength);
        this.Out.WriteLine(str);
      }
    }

    private void WriteSideBySideRange(
      IList<DiffLine> sequence,
      string middleText,
      int startIndex,
      int endIndex,
      int maxLength,
      Vss.Column column)
    {
      for (int index = startIndex; index <= endIndex; ++index)
      {
        string str = sequence[index].Content;
        if (str.Length > maxLength)
          str = str.Substring(0, maxLength);
        else if (column != Vss.Column.Right)
          str = str.PadRight(maxLength);
        switch (column)
        {
          case Vss.Column.Left:
            this.Out.Write(str);
            this.Out.WriteLine(middleText);
            break;
          case Vss.Column.Right:
            this.Out.Write(middleText.PadLeft(maxLength + middleText.Length));
            this.Out.WriteLine(str);
            break;
          case Vss.Column.Both:
            this.Out.Write(str);
            this.Out.Write(middleText);
            this.Out.WriteLine(str);
            break;
        }
      }
    }

    private enum Column
    {
      Left,
      Right,
      Both,
    }
  }
}
