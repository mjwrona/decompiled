// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Common.Internal.MergeOutput
// Assembly: Microsoft.TeamFoundation.VersionControl.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 156CCB01-0A1F-468C-A332-06DB9F9B179E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Common.dll

using Microsoft.TeamFoundation.Diff;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;

namespace Microsoft.TeamFoundation.VersionControl.Common.Internal
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class MergeOutput
  {
    private Stream m_stream;
    private bool m_lastWrittenLineHadEndOfLine = true;
    private Encoding m_encoding;
    private StreamWriter m_streamWriter;
    private bool m_useStreamWriter;
    private MergeOptions m_options;
    private const string c_targetLabel = "Yours";
    private const string c_latestLabel = "Theirs";
    private const string c_sourceLabel = "Original";

    public MergeOutput(
      MergeOptions mergeOptions,
      Stream stream,
      Encoding encoding,
      bool outputPreamble)
    {
      this.m_useStreamWriter = outputPreamble;
      if (this.m_useStreamWriter)
      {
        this.m_streamWriter = new StreamWriter(stream, encoding);
        this.m_streamWriter.AutoFlush = true;
      }
      else
      {
        this.m_stream = stream;
        this.m_encoding = encoding;
      }
      this.m_options = mergeOptions;
    }

    protected MergeOptions Options => this.m_options;

    private void Write(string value)
    {
      if (this.m_useStreamWriter)
      {
        this.m_streamWriter.Write(value);
      }
      else
      {
        if (value.Length <= 0)
          return;
        byte[] bytes = this.m_encoding.GetBytes(value.ToCharArray());
        this.m_stream.Write(bytes, 0, bytes.Length);
        this.m_stream.Flush();
      }
    }

    private void WriteLine(string line)
    {
      if (this.m_useStreamWriter)
      {
        this.m_streamWriter.WriteLine(line);
      }
      else
      {
        this.Write(line);
        this.Write(Environment.NewLine);
      }
      this.m_lastWrittenLineHadEndOfLine = true;
    }

    private void WriteElementRange(IList<DiffLine> sequence, int startIndex, int endIndex)
    {
      bool flag = false;
      for (int index = startIndex; index <= endIndex; ++index)
      {
        if (!this.m_lastWrittenLineHadEndOfLine)
        {
          this.Write(Environment.NewLine);
          this.m_lastWrittenLineHadEndOfLine = true;
        }
        flag = true;
        this.Write(sequence[index].ToString());
      }
      if (!flag)
        return;
      this.m_lastWrittenLineHadEndOfLine = sequence[endIndex].EndOfLineTerminator != 0;
    }

    public void Output(IList<DiffLine> modified, IList<DiffLine> latest, IMergeChange[] mergeList)
    {
      int startIndex = 0;
      foreach (IMergeChange merge in mergeList)
      {
        IDiffChange modifiedChange = merge.ModifiedChange;
        this.WriteElementRange(modified, startIndex, modifiedChange.OriginalStart - 1);
        try
        {
          this.WriteLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "<<<<<<< {0}", (object) this.Options.TargetLabel));
        }
        catch (EncoderFallbackException ex)
        {
          this.WriteLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "<<<<<<< {0}", (object) "Yours"));
        }
        this.WriteElementRange(modified, modifiedChange.OriginalStart, modifiedChange.OriginalStart + modifiedChange.OriginalLength - 1);
        this.WriteLine("=======");
        this.WriteElementRange(latest, modifiedChange.ModifiedStart, modifiedChange.ModifiedStart + modifiedChange.ModifiedLength - 1);
        try
        {
          this.WriteLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ">>>>>>> {0}", (object) this.Options.LatestLabel));
        }
        catch (EncoderFallbackException ex)
        {
          this.WriteLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ">>>>>>> {0}", (object) "Theirs"));
        }
        startIndex = modifiedChange.OriginalStart + modifiedChange.OriginalLength;
      }
      this.WriteElementRange(modified, startIndex, modified.Count - 1);
    }

    public void Output(
      IList<DiffLine> original,
      IList<DiffLine> modified,
      IList<DiffLine> latest,
      IMergeChange[] mergeList)
    {
      int endIndex = 0;
      int startIndex1 = 0;
      foreach (IMergeChange merge in mergeList)
      {
        switch (merge.ChangeType)
        {
          case MergeChangeType.Modified:
          case MergeChangeType.Both:
            int originalStart1 = merge.ModifiedChange.OriginalStart;
            endIndex = merge.ModifiedChange.OriginalStart + merge.ModifiedChange.OriginalLength - 1;
            this.WriteElementRange(original, startIndex1, originalStart1 - 1);
            this.WriteElementRange(modified, merge.ModifiedChange.ModifiedStart, merge.ModifiedChange.ModifiedStart + merge.ModifiedChange.ModifiedLength - 1);
            break;
          case MergeChangeType.Latest:
            int originalStart2 = merge.LatestChange.OriginalStart;
            endIndex = merge.LatestChange.OriginalStart + merge.LatestChange.OriginalLength - 1;
            this.WriteElementRange(original, startIndex1, originalStart2 - 1);
            this.WriteElementRange(latest, merge.LatestChange.ModifiedStart, merge.LatestChange.ModifiedStart + merge.LatestChange.ModifiedLength - 1);
            break;
          case MergeChangeType.Conflict:
            int startIndex2 = Math.Min(merge.ModifiedChange.OriginalStart, merge.LatestChange.OriginalStart);
            endIndex = Math.Max(merge.ModifiedChange.OriginalStart + merge.ModifiedChange.OriginalLength, merge.LatestChange.OriginalStart + merge.LatestChange.OriginalLength) - 1;
            this.WriteElementRange(original, startIndex1, startIndex2 - 1);
            if (this.m_options.WriteOriginalForConflictingRange)
            {
              this.WriteElementRange(original, startIndex2, endIndex);
              break;
            }
            try
            {
              this.WriteLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "<<<<<<< {0}", (object) this.Options.TargetLabel));
            }
            catch (EncoderFallbackException ex)
            {
              this.WriteLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "<<<<<<< {0}", (object) "Yours"));
            }
            this.WriteElementRange(original, startIndex2, merge.ModifiedChange.OriginalStart - 1);
            this.WriteElementRange(modified, merge.ModifiedChange.ModifiedStart, merge.ModifiedChange.ModifiedStart + merge.ModifiedChange.ModifiedLength - 1);
            this.WriteElementRange(original, merge.ModifiedChange.OriginalStart + merge.ModifiedChange.OriginalLength, endIndex);
            try
            {
              this.WriteLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "||||||| {0}", (object) this.Options.SourceLabel));
            }
            catch (EncoderFallbackException ex)
            {
              this.WriteLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "<<<<<<< {0}", (object) "Original"));
            }
            this.WriteElementRange(original, startIndex2, endIndex);
            this.WriteLine("=======");
            this.WriteElementRange(original, startIndex2, merge.LatestChange.OriginalStart - 1);
            this.WriteElementRange(latest, merge.LatestChange.ModifiedStart, merge.LatestChange.ModifiedStart + merge.LatestChange.ModifiedLength - 1);
            this.WriteElementRange(original, merge.LatestChange.OriginalStart + merge.LatestChange.OriginalLength, endIndex);
            try
            {
              this.WriteLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ">>>>>>> {0}", (object) this.Options.LatestLabel));
              break;
            }
            catch (EncoderFallbackException ex)
            {
              this.WriteLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "<<<<<<< {0}", (object) "Theirs"));
              break;
            }
        }
        startIndex1 = endIndex + 1;
      }
      this.WriteElementRange(original, startIndex1, original.Count - 1);
    }
  }
}
