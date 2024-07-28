// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Rntbd.LinuxSystemUtilizationReader
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.Azure.Documents.Rntbd
{
  internal sealed class LinuxSystemUtilizationReader : SystemUtilizationReaderBase
  {
    private readonly LinuxSystemUtilizationReader.ProcStatFileParser procStatFileParser;
    private readonly LinuxSystemUtilizationReader.ProcMemInfoFileParser procMemInfoFileParser;
    private ulong lastIdleJiffies;
    private ulong lastKernelJiffies;
    private ulong lastOtherJiffies;
    private ulong lastUserJiffies;

    public LinuxSystemUtilizationReader()
      : this((string) null, (string) null)
    {
    }

    internal LinuxSystemUtilizationReader(string procStatFilePath, string procMemoryInfoPath)
    {
      this.procStatFileParser = string.IsNullOrWhiteSpace(procStatFilePath) ? new LinuxSystemUtilizationReader.ProcStatFileParser() : new LinuxSystemUtilizationReader.ProcStatFileParser(procStatFilePath);
      this.procMemInfoFileParser = string.IsNullOrWhiteSpace(procMemoryInfoPath) ? new LinuxSystemUtilizationReader.ProcMemInfoFileParser() : new LinuxSystemUtilizationReader.ProcMemInfoFileParser(procMemoryInfoPath);
      this.lastIdleJiffies = 0UL;
      this.lastKernelJiffies = 0UL;
      this.lastOtherJiffies = 0UL;
      this.lastUserJiffies = 0UL;
    }

    protected override float GetSystemWideCpuUsageCore()
    {
      ulong userJiffiesElaped;
      ulong kernelJiffiesElapsed;
      ulong idleJiffiesElapsed;
      ulong otherJiffiesElapsed;
      if (!this.procStatFileParser.TryParseStatFile(out userJiffiesElaped, out kernelJiffiesElapsed, out idleJiffiesElapsed, out otherJiffiesElapsed))
        return float.NaN;
      float wideCpuUsageCore = 0.0f;
      if (this.lastIdleJiffies != 0UL)
      {
        ulong num1 = kernelJiffiesElapsed - this.lastKernelJiffies + (userJiffiesElaped - this.lastUserJiffies) + otherJiffiesElapsed - this.lastOtherJiffies;
        ulong num2 = num1 + idleJiffiesElapsed - this.lastIdleJiffies;
        if (num2 == 0UL)
          return float.NaN;
        wideCpuUsageCore = (float) (100.0 * ((double) num1 / (double) num2));
      }
      this.lastUserJiffies = userJiffiesElaped;
      this.lastKernelJiffies = kernelJiffiesElapsed;
      this.lastIdleJiffies = idleJiffiesElapsed;
      this.lastOtherJiffies = otherJiffiesElapsed;
      return wideCpuUsageCore;
    }

    protected override long? GetSystemWideMemoryAvailabiltyCore()
    {
      long? freeMemory;
      long? availableMemory;
      return !this.procMemInfoFileParser.TryParseMemInfoFile(out freeMemory, out availableMemory) ? new long?() : availableMemory ?? freeMemory;
    }

    private class ProcStatFileParser
    {
      private const string cpuPrefixFirstLine = "cpu";
      private const string DefaultProcStatFilePath = "/proc/stat";
      private readonly string procStatFilePath;
      private readonly LinuxSystemUtilizationReader.ReusableTextReader reusableReader;

      public ProcStatFileParser()
        : this("/proc/stat")
      {
      }

      internal ProcStatFileParser(string procStatFilePath)
      {
        if (string.IsNullOrWhiteSpace(procStatFilePath))
          throw new ArgumentNullException(nameof (procStatFilePath));
        this.reusableReader = new LinuxSystemUtilizationReader.ReusableTextReader(Encoding.UTF8, 256);
        this.procStatFilePath = procStatFilePath;
      }

      public bool TryParseStatFile(
        out ulong userJiffiesElaped,
        out ulong kernelJiffiesElapsed,
        out ulong idleJiffiesElapsed,
        out ulong otherJiffiesElapsed)
      {
        userJiffiesElaped = 0UL;
        kernelJiffiesElapsed = 0UL;
        idleJiffiesElapsed = 0UL;
        otherJiffiesElapsed = 0UL;
        string firstLine;
        if (!this.TryReadProcStatFirstLine(this.reusableReader, out firstLine))
          return false;
        try
        {
          LinuxSystemUtilizationReader.StringParser stringParser = new LinuxSystemUtilizationReader.StringParser(firstLine, ' ', true);
          if (!"cpu".Equals(stringParser.MoveAndExtractNext(), StringComparison.OrdinalIgnoreCase))
          {
            DefaultTrace.TraceCritical("Unexpected procfs/cpu-file format. '$" + firstLine + "'");
            return false;
          }
          ulong nextUint64_1 = stringParser.ParseNextUInt64();
          ulong nextUint64_2 = stringParser.ParseNextUInt64();
          ulong nextUint64_3 = stringParser.ParseNextUInt64();
          ulong nextUint64_4 = stringParser.ParseNextUInt64();
          ulong num = 0;
          while (stringParser.HasNext)
            num += stringParser.ParseNextUInt64();
          userJiffiesElaped = nextUint64_1 + nextUint64_2;
          kernelJiffiesElapsed = nextUint64_3;
          idleJiffiesElapsed = nextUint64_4;
          otherJiffiesElapsed = num;
          return true;
        }
        catch (InvalidDataException ex)
        {
          return false;
        }
      }

      private bool TryReadProcStatFirstLine(
        LinuxSystemUtilizationReader.ReusableTextReader reusableReader,
        out string firstLine)
      {
        try
        {
          using (FileStream source = new FileStream(this.procStatFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, 1, false))
          {
            firstLine = reusableReader.ReadJustFirstLine((Stream) source);
            return true;
          }
        }
        catch (IOException ex)
        {
          DefaultTrace.TraceError(ex.Message);
          firstLine = (string) null;
          return false;
        }
      }
    }

    private class ProcMemInfoFileParser
    {
      private const string DefaultProcMemInfoFilePath = "/proc/meminfo";
      private readonly string procMemInfoFilePath;
      private readonly LinuxSystemUtilizationReader.ReusableTextReader reusableReader;

      public ProcMemInfoFileParser()
        : this("/proc/meminfo")
      {
      }

      internal ProcMemInfoFileParser(string procMemInfoFilePath)
      {
        if (string.IsNullOrWhiteSpace(procMemInfoFilePath))
          throw new ArgumentNullException(nameof (procMemInfoFilePath));
        this.reusableReader = new LinuxSystemUtilizationReader.ReusableTextReader(Encoding.UTF8, 256);
        this.procMemInfoFilePath = procMemInfoFilePath;
      }

      public bool TryParseMemInfoFile(out long? freeMemory, out long? availableMemory)
      {
        freeMemory = new long?();
        availableMemory = new long?();
        List<string> data;
        if (!this.TryReadProcMemInfo(this.reusableReader, out data))
        {
          DefaultTrace.TraceCritical("Not able to read memory information from /proc/meminfo");
          return false;
        }
        try
        {
          foreach (string buffer in data)
          {
            LinuxSystemUtilizationReader.StringParser stringParser = new LinuxSystemUtilizationReader.StringParser(buffer, ' ', true);
            if (stringParser.MoveAndExtractNext().Contains("MemFree"))
              freeMemory = new long?((long) stringParser.ParseNextUInt64());
            else if (buffer.Contains("MemAvailable"))
              availableMemory = new long?((long) stringParser.ParseNextUInt64());
          }
          if (!freeMemory.HasValue && !availableMemory.HasValue)
            throw new InvalidDataException("Free Memory and Available Memory information is not available.");
          return true;
        }
        catch (InvalidDataException ex)
        {
          DefaultTrace.TraceError(ex.Message);
          return false;
        }
      }

      private bool TryReadProcMemInfo(
        LinuxSystemUtilizationReader.ReusableTextReader reusableReader,
        out List<string> data)
      {
        try
        {
          using (FileStream source = new FileStream(this.procMemInfoFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, 1, false))
          {
            data = reusableReader.ReadMultipleLines((Stream) source);
            return true;
          }
        }
        catch (IOException ex)
        {
          DefaultTrace.TraceError(ex.Message);
          data = (List<string>) null;
          return false;
        }
      }
    }

    private struct StringParser
    {
      private readonly string buffer;
      private readonly char separator;
      private readonly bool skipEmpty;
      private int endIndex;
      private int startIndex;

      public StringParser(string buffer, char separator, bool skipEmpty = false)
      {
        this.buffer = buffer != null ? buffer : throw new ArgumentNullException(nameof (buffer));
        this.separator = separator;
        this.skipEmpty = skipEmpty;
        this.startIndex = -1;
        this.endIndex = -1;
      }

      public bool HasNext => this.endIndex < this.buffer.Length;

      public string ExtractCurrent()
      {
        if (this.buffer == null || this.startIndex == -1)
          throw new InvalidOperationException();
        return this.buffer.Substring(this.startIndex, this.endIndex - this.startIndex);
      }

      public string MoveAndExtractNext()
      {
        this.MoveNextOrFail();
        return this.buffer.Substring(this.startIndex, this.endIndex - this.startIndex);
      }

      public bool MoveNext()
      {
        if (this.buffer == null)
          throw new InvalidOperationException();
        while (this.endIndex < this.buffer.Length)
        {
          int num = this.buffer.IndexOf(this.separator, this.endIndex + 1);
          this.startIndex = this.endIndex + 1;
          this.endIndex = num >= 0 ? num : this.buffer.Length;
          if (!this.skipEmpty || this.endIndex >= this.startIndex + 1)
            return true;
        }
        this.startIndex = this.endIndex;
        return false;
      }

      public void MoveNextOrFail()
      {
        if (this.MoveNext())
          return;
        LinuxSystemUtilizationReader.StringParser.ThrowForInvalidData();
      }

      public unsafe ulong ParseNextUInt64()
      {
        this.MoveNextOrFail();
        ulong nextUint64 = 0;
        string str = this.buffer;
        char* chPtr1 = (char*) str;
        if ((IntPtr) chPtr1 != IntPtr.Zero)
          chPtr1 += RuntimeHelpers.OffsetToStringData;
        char* chPtr2 = chPtr1 + this.startIndex;
        for (char* chPtr3 = chPtr1 + this.endIndex; chPtr2 != chPtr3; ++chPtr2)
        {
          int num = (int) *chPtr2 - 48;
          if (num < 0 || num > 9)
            LinuxSystemUtilizationReader.StringParser.ThrowForInvalidData();
          nextUint64 = checked (nextUint64 * 10UL + (ulong) num);
        }
        str = (string) null;
        return nextUint64;
      }

      private static void ThrowForInvalidData() => throw new InvalidDataException();
    }

    private sealed class ReusableTextReader
    {
      private static readonly char[] lineBreakChars = Environment.NewLine.ToCharArray();
      private readonly StringBuilder builder;
      private readonly byte[] bytes;
      private readonly char[] chars;
      private readonly Decoder decoder;
      private readonly Encoding encoding;

      public ReusableTextReader(Encoding encoding = null, int bufferSize = 1024)
      {
        this.encoding = encoding != null ? encoding : Encoding.UTF8;
        this.builder = new StringBuilder();
        this.decoder = encoding.GetDecoder();
        this.bytes = new byte[bufferSize];
        this.chars = new char[encoding.GetMaxCharCount(this.bytes.Length)];
      }

      public string ReadJustFirstLine(Stream source)
      {
        int byteCount;
        while ((byteCount = source.Read(this.bytes, 0, this.bytes.Length)) != 0)
        {
          int chars = this.decoder.GetChars(this.bytes, 0, byteCount, this.chars, 0);
          int charCount = -1;
          for (int index = 0; index < chars; ++index)
          {
            if (((IEnumerable<char>) LinuxSystemUtilizationReader.ReusableTextReader.lineBreakChars).Contains<char>(this.chars[index]))
            {
              charCount = index;
              break;
            }
          }
          if (charCount < 0)
          {
            this.builder.Append(this.chars, 0, chars);
          }
          else
          {
            this.builder.Append(this.chars, 0, charCount);
            break;
          }
        }
        string str = this.builder.ToString();
        this.builder.Clear();
        this.decoder.Reset();
        return str;
      }

      public List<string> ReadMultipleLines(Stream source)
      {
        List<string> stringList = new List<string>();
        using (StreamReader streamReader = new StreamReader(source, this.encoding))
        {
          string str;
          while ((str = streamReader.ReadLine()) != null)
            stringList.Add(str);
        }
        return stringList;
      }
    }
  }
}
