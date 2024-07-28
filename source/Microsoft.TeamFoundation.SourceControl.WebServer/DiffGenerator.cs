// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.DiffGenerator
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Diff;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi.Legacy;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.TeamFoundation.VersionControl.Common.Internal;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public static class DiffGenerator
  {
    public static List<FileCharDiffBlock> ComputeDifference(
      ITraceRequest tracer,
      Stream originalStream,
      Stream modifiedStream,
      int originalFileEncoding,
      int modifiedFileEncoding,
      bool ignoreTrimmedWhitespace,
      bool lineNumbersOnly,
      bool computeCharDiffs,
      int lineDiffTimeoutMs,
      int wordDiffTimeoutMs)
    {
      if (originalStream == null)
        throw new ArgumentNullException(nameof (originalStream));
      if (modifiedStream == null)
        throw new ArgumentNullException(nameof (modifiedStream));
      DiffOptions diffOpts = new DiffOptions();
      diffOpts.SourceLabel = "Original";
      diffOpts.TargetLabel = "Modified";
      diffOpts.UseThirdPartyTool = false;
      diffOpts.OutputType = DiffOutputType.Context;
      if (ignoreTrimmedWhitespace)
        diffOpts.Flags |= DiffOptionFlags.IgnoreEndOfLineDifference | DiffOptionFlags.IgnoreLeadingAndTrailingWhiteSpace | DiffOptionFlags.IgnoreEndOfFileEndOfLineDifference;
      MemoryStream destStream1 = (MemoryStream) null;
      MemoryStream destStream2 = (MemoryStream) null;
      StreamReader reader1 = (StreamReader) null;
      StreamReader reader2 = (StreamReader) null;
      List<FileCharDiffBlock> source = new List<FileCharDiffBlock>();
      try
      {
        Encoding encoding1 = Encoding.GetEncoding(originalFileEncoding);
        Encoding encoding2 = Encoding.GetEncoding(modifiedFileEncoding);
        if (encoding1.CodePage != encoding2.CodePage)
        {
          destStream1 = new MemoryStream();
          DiffGenerator.ConvertEncoding(originalStream, (Stream) destStream1, encoding1, Encoding.Unicode, false);
          encoding1 = Encoding.Unicode;
          originalStream = (Stream) destStream1;
          originalStream.Seek(0L, SeekOrigin.Begin);
          destStream2 = new MemoryStream();
          DiffGenerator.ConvertEncoding(modifiedStream, (Stream) destStream2, encoding2, Encoding.Unicode, false);
          encoding2 = Encoding.Unicode;
          modifiedStream = (Stream) destStream2;
          modifiedStream.Seek(0L, SeekOrigin.Begin);
        }
        diffOpts.SourceEncoding = encoding1;
        diffOpts.TargetEncoding = encoding2;
        int num1 = 1;
        int num2 = 1;
        bool originalEndsInNewline;
        bool modifiedEndsInNewline;
        DiffSegment diffSegment1 = DiffGenerator.DiffStreams(tracer, originalStream, encoding1, modifiedStream, encoding2, diffOpts, out originalEndsInNewline, out modifiedEndsInNewline, computeCharDiffs, lineDiffTimeoutMs, wordDiffTimeoutMs);
        if (!lineNumbersOnly)
        {
          originalStream.Seek(0L, SeekOrigin.Begin);
          modifiedStream.Seek(0L, SeekOrigin.Begin);
          reader1 = new StreamReader(originalStream, encoding1);
          reader2 = new StreamReader(modifiedStream, encoding2);
        }
        DiffSegment diffSegment2 = diffSegment1;
        DiffSegment diffSegment3 = (DiffSegment) null;
        for (; diffSegment2 != null; diffSegment2 = diffSegment2.Next)
        {
          if (diffSegment3 != null)
          {
            FileDiffBlock fileDiffBlock = new FileDiffBlock()
            {
              OriginalLineNumberStart = num1,
              ModifiedLineNumberStart = num2
            };
            fileDiffBlock.OriginalLinesCount = diffSegment2.OriginalStart - diffSegment3.OriginalStart - diffSegment3.OriginalLength;
            fileDiffBlock.ModifiedLinesCount = diffSegment2.ModifiedStart - diffSegment3.ModifiedStart - diffSegment3.ModifiedLength;
            fileDiffBlock.ChangeType = fileDiffBlock.OriginalLinesCount != 0 ? (fileDiffBlock.ModifiedLinesCount != 0 ? FileDiffBlockChangeType.Edit : FileDiffBlockChangeType.Delete) : FileDiffBlockChangeType.Add;
            if (!lineNumbersOnly)
            {
              fileDiffBlock.OriginalLines = new List<string>();
              for (int index = 0; index < fileDiffBlock.OriginalLinesCount; ++index)
                fileDiffBlock.OriginalLines.Add(DiffGenerator.ReadLine(reader1));
              fileDiffBlock.ModifiedLines = new List<string>();
              for (int index = 0; index < fileDiffBlock.ModifiedLinesCount; ++index)
                fileDiffBlock.ModifiedLines.Add(DiffGenerator.ReadLine(reader2));
            }
            if (computeCharDiffs)
            {
              FileCharDiffBlock fileCharDiffBlock = new FileCharDiffBlock();
              fileCharDiffBlock.LineChange = fileDiffBlock;
              if (computeCharDiffs && fileDiffBlock.ChangeType == FileDiffBlockChangeType.Edit && diffSegment3.CharDiffs != null)
              {
                List<FileDiffBlock> fileDiffBlockList = new List<FileDiffBlock>();
                for (int index = 0; index < diffSegment3.CharDiffs.Length; ++index)
                  fileDiffBlockList.Add(new FileDiffBlock()
                  {
                    ModifiedLineNumberStart = diffSegment3.CharDiffs[index].ModifiedStart,
                    ModifiedLinesCount = diffSegment3.CharDiffs[index].ModifiedEnd - diffSegment3.CharDiffs[index].ModifiedStart,
                    OriginalLineNumberStart = diffSegment3.CharDiffs[index].OriginalStart,
                    OriginalLinesCount = diffSegment3.CharDiffs[index].OriginalEnd - diffSegment3.CharDiffs[index].OriginalStart
                  });
                fileCharDiffBlock.CharChange = fileDiffBlockList;
              }
              source.Add(fileCharDiffBlock);
            }
            else
              source.Add(new FileCharDiffBlock()
              {
                LineChange = fileDiffBlock
              });
            num1 += fileDiffBlock.OriginalLinesCount;
            num2 += fileDiffBlock.ModifiedLinesCount;
          }
          if (diffSegment2.ModifiedLength > 0)
          {
            FileDiffBlock fileDiffBlock = new FileDiffBlock()
            {
              ChangeType = FileDiffBlockChangeType.None,
              OriginalLineNumberStart = num1,
              ModifiedLineNumberStart = num2,
              OriginalLinesCount = diffSegment2.ModifiedLength,
              ModifiedLinesCount = diffSegment2.ModifiedLength
            };
            if (!lineNumbersOnly)
            {
              fileDiffBlock.OriginalLines = new List<string>();
              for (int index = 0; index < diffSegment2.OriginalLength; ++index)
                fileDiffBlock.OriginalLines.Add(DiffGenerator.ReadLine(reader1));
              fileDiffBlock.ModifiedLines = new List<string>();
              for (int index = 0; index < diffSegment2.ModifiedLength; ++index)
                fileDiffBlock.ModifiedLines.Add(DiffGenerator.ReadLine(reader2));
            }
            source.Add(new FileCharDiffBlock()
            {
              LineChange = fileDiffBlock
            });
            num1 += fileDiffBlock.OriginalLinesCount;
            num2 += fileDiffBlock.ModifiedLinesCount;
          }
          diffSegment3 = diffSegment2;
        }
        if (originalEndsInNewline & modifiedEndsInNewline)
        {
          FileDiffBlock lineChange = source.LastOrDefault<FileCharDiffBlock>()?.LineChange;
          if (lineChange != null && lineChange.ChangeType == FileDiffBlockChangeType.None)
          {
            if (!lineNumbersOnly)
            {
              lineChange.OriginalLines.Add(string.Empty);
              lineChange.ModifiedLines.Add(string.Empty);
            }
            ++lineChange.OriginalLinesCount;
            ++lineChange.ModifiedLinesCount;
          }
          else
          {
            FileDiffBlock fileDiffBlock = new FileDiffBlock()
            {
              ChangeType = FileDiffBlockChangeType.None,
              OriginalLineNumberStart = num1,
              ModifiedLineNumberStart = num2,
              OriginalLinesCount = 1,
              ModifiedLinesCount = 1,
              OriginalLines = new List<string>(),
              ModifiedLines = new List<string>()
            };
            if (!lineNumbersOnly)
            {
              fileDiffBlock.OriginalLines.Add(string.Empty);
              fileDiffBlock.ModifiedLines.Add(string.Empty);
            }
            source.Add(new FileCharDiffBlock()
            {
              LineChange = fileDiffBlock
            });
          }
        }
        else if (originalEndsInNewline)
        {
          FileCharDiffBlock fileCharDiffBlock = source.LastOrDefault<FileCharDiffBlock>((Func<FileCharDiffBlock, bool>) (block => block.LineChange.OriginalLinesCount > 0));
          if (fileCharDiffBlock != null)
          {
            if (!lineNumbersOnly)
              fileCharDiffBlock.LineChange.OriginalLines.Add(string.Empty);
            ++fileCharDiffBlock.LineChange.OriginalLinesCount;
          }
        }
        else if (modifiedEndsInNewline)
        {
          FileCharDiffBlock fileCharDiffBlock = source.LastOrDefault<FileCharDiffBlock>((Func<FileCharDiffBlock, bool>) (block => block.LineChange.ModifiedLinesCount > 0));
          if (fileCharDiffBlock != null)
          {
            if (!lineNumbersOnly)
              fileCharDiffBlock.LineChange.ModifiedLines.Add(string.Empty);
            ++fileCharDiffBlock.LineChange.ModifiedLinesCount;
          }
        }
        return source;
      }
      finally
      {
        destStream1?.Close();
        destStream2?.Close();
        reader1?.Close();
        reader2?.Close();
      }
    }

    private static string ReadLine(StreamReader reader) => reader.ReadLine() ?? string.Empty;

    private static DiffSegment DiffStreams(
      ITraceRequest tracer,
      Stream sourceStream,
      Encoding sourceEncoding,
      Stream targetStream,
      Encoding targetEncoding,
      DiffOptions diffOpts,
      out bool originalEndsInNewline,
      out bool modifiedEndsInNewline,
      bool includeCharDiffs,
      int lineDiffTimeoutMs,
      int wordDiffTimeoutMs)
    {
      if (sourceEncoding.CodePage != targetEncoding.CodePage)
        throw new ArgumentException(DiffResources.Get("ArgumentError_TokenizerCodePagesMustMatch")).Expected("git");
      using (StreamWrapper streamWrapper1 = new StreamWrapper(sourceStream, false))
      {
        using (StreamWrapper streamWrapper2 = new StreamWrapper(targetStream, false))
        {
          DiffFinder<DiffLine> lcsDiff = DiffFinder<DiffLine>.LcsDiff;
          DiffFile diffFile1 = DiffFile.Create((Stream) streamWrapper1, sourceEncoding, diffOpts);
          DiffFile diffFile2 = DiffFile.Create((Stream) streamWrapper2, targetEncoding, diffOpts);
          DiffLine diffLine1 = diffFile1.LastOrDefault<DiffLine>();
          originalEndsInNewline = diffLine1 != null && diffLine1.EndOfLineTerminator != 0;
          DiffLine diffLine2 = diffFile2.LastOrDefault<DiffLine>();
          modifiedEndsInNewline = diffLine2 != null && diffLine2.EndOfLineTerminator != 0;
          DiffLineComparer elementComparer = new DiffLineComparer(diffOpts);
          using (lcsDiff)
          {
            Stopwatch sw = Stopwatch.StartNew();
            IDiffChange[] diffChangeArray = lcsDiff.Diff((IList<DiffLine>) diffFile1, (IList<DiffLine>) diffFile2, (IEqualityComparer<DiffLine>) elementComparer, (ContinueDifferencePredicate<DiffLine>) ((originalIndex, originalSequence, longestMatchSoFar) => sw.ElapsedMilliseconds < (long) lineDiffTimeoutMs));
            if (sw.ElapsedMilliseconds > 1000L)
              tracer.TraceAlways(1013845, TraceLevel.Info, nameof (DiffGenerator), nameof (DiffStreams), "Line diff of {0}->{1} (result: {2} diffs) took {3}ms (timeout: {4}ms)", (object) diffFile1.Count, (object) diffFile2.Count, (object) diffChangeArray.Length, (object) sw.ElapsedMilliseconds, (object) lineDiffTimeoutMs);
            DiffSegment diffSegment;
            if (includeCharDiffs)
            {
              sw.Restart();
              LineChanges[] array = lcsDiff.WordDiff(diffChangeArray, (IList<DiffLine>) diffFile1, (IList<DiffLine>) diffFile2, (ContinueDifferencePredicate<WordPosition>) ((originalIndex, originalSequence, longestMatchSoFar) => sw.ElapsedMilliseconds < (long) wordDiffTimeoutMs)).ToArray<LineChanges>();
              if (sw.ElapsedMilliseconds > 1000L)
                tracer.TraceAlways(1013846, TraceLevel.Info, nameof (DiffGenerator), nameof (DiffStreams), "Word diff of {0}->{1} (result: {2} line changes on {3} diffs) took {4}ms (timeout: {5}ms)", (object) diffFile1.Count, (object) diffFile2.Count, (object) array.Length, (object) diffChangeArray.Length, (object) sw.ElapsedMilliseconds, (object) wordDiffTimeoutMs);
              diffSegment = DiffSegment.Convert(array, diffFile1.Count, diffFile2.Count);
            }
            else
              diffSegment = DiffSegment.Convert(((IEnumerable<IDiffChange>) diffChangeArray).Select<IDiffChange, LineChanges>((Func<IDiffChange, LineChanges>) (diff => new LineChanges(diff, (IDiffChange[]) null))).ToArray<LineChanges>(), diffFile1.Count, diffFile2.Count);
            return diffSegment;
          }
        }
      }
    }

    private static void ConvertEncoding(
      Stream sourceStream,
      Stream destStream,
      Encoding sourceEncoding,
      Encoding destEncoding,
      bool includeBOM)
    {
      ArgumentUtility.CheckForNull<Stream>(sourceStream, nameof (sourceStream));
      ArgumentUtility.CheckForNull<Stream>(destStream, nameof (destStream));
      if (sourceEncoding == destEncoding)
      {
        StreamUtil.Copy(sourceStream, destStream, false);
      }
      else
      {
        byte[] preamble1 = sourceEncoding.GetPreamble();
        byte[] preamble2 = destEncoding.GetPreamble();
        byte[] numArray = new byte[preamble1.Length];
        int num = sourceStream.Read(numArray, 0, numArray.Length);
        if (numArray.Length != 0 && num == numArray.Length && ArrayUtil.Equals(preamble1, numArray))
        {
          destStream.Write(preamble2, 0, preamble2.Length);
        }
        else
        {
          sourceStream.Seek(0L, SeekOrigin.Begin);
          if (includeBOM && (destEncoding.CodePage == Encoding.Unicode.CodePage || destEncoding.CodePage == Encoding.UTF7.CodePage || destEncoding.CodePage == Encoding.UTF8.CodePage || destEncoding.CodePage == Encoding.UTF32.CodePage || destEncoding.CodePage == Encoding.BigEndianUnicode.CodePage) && preamble2.Length != 0)
            destStream.Write(preamble2, 0, preamble2.Length);
        }
        StreamReader streamReader = new StreamReader(sourceStream, sourceEncoding, false);
        char[] chArray = new char[131072];
        int count;
        while ((count = streamReader.ReadBlock(chArray, 0, chArray.Length)) > 0)
        {
          byte[] bytes = destEncoding.GetBytes(chArray, 0, count);
          destStream.Write(bytes, 0, bytes.Length);
        }
      }
    }
  }
}
