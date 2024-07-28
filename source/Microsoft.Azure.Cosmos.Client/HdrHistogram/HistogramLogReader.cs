// Decompiled with JetBrains decompiler
// Type: HdrHistogram.HistogramLogReader
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using HdrHistogram.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace HdrHistogram
{
  internal sealed class HistogramLogReader : IDisposable, IHistogramLogV1Reader
  {
    private static readonly Regex StartTimeMatcher = new Regex("#\\[StartTime: (?<seconds>\\d*\\.\\d{1,3}) ", RegexOptions.Compiled);
    private static readonly Regex BaseTimeMatcher = new Regex("#\\[BaseTime: (?<seconds>\\d*\\.\\d{1,3}) ", RegexOptions.Compiled);
    private static readonly Regex UntaggedLogLineMatcher = new Regex("(?<startTime>\\d*\\.\\d*),(?<interval>\\d*\\.\\d*),(?<max>\\d*\\.\\d*),(?<payload>.*)", RegexOptions.Compiled);
    private static readonly Regex TaggedLogLineMatcher = new Regex("((?<tag>Tag=.+),)?(?<startTime>\\d*\\.\\d*),(?<interval>\\d*\\.\\d*),(?<max>\\d*\\.\\d*),(?<payload>.*)", RegexOptions.Compiled);
    private readonly TextReader _log;
    private double _startTimeInSeconds;

    public static IEnumerable<HistogramBase> Read(Stream inputStream)
    {
      using (HistogramLogReader histogramLogReader = new HistogramLogReader(inputStream))
        return histogramLogReader.ReadHistograms();
    }

    public HistogramLogReader(Stream inputStream) => this._log = (TextReader) new StreamReader(inputStream, Encoding.UTF8, true, 1024, true);

    public IEnumerable<HistogramBase> ReadHistograms()
    {
      this._startTimeInSeconds = 0.0;
      double baseTimeInSeconds = 0.0;
      bool hasStartTime = false;
      bool hasBaseTime = false;
      foreach (string readLine in this.ReadLines())
      {
        if (HistogramLogReader.IsComment(readLine))
        {
          if (HistogramLogReader.IsStartTime(readLine))
          {
            this._startTimeInSeconds = HistogramLogReader.ParseStartTime(readLine);
            hasStartTime = true;
          }
          else if (HistogramLogReader.IsBaseTime(readLine))
          {
            baseTimeInSeconds = HistogramLogReader.ParseBaseTime(readLine);
            hasBaseTime = true;
          }
        }
        else if (!HistogramLogReader.IsLegend(readLine))
        {
          Match match = HistogramLogReader.TaggedLogLineMatcher.Match(readLine);
          string tag = HistogramLogReader.ParseTag(match.Groups["tag"].Value);
          double num1 = HistogramLogReader.ParseDouble(match, "startTime");
          double num2 = HistogramLogReader.ParseDouble(match, "interval");
          HistogramLogReader.ParseDouble(match, "max");
          string s = match.Groups["payload"].Value;
          if (!hasStartTime)
          {
            this._startTimeInSeconds = num1;
            hasStartTime = true;
          }
          if (!hasBaseTime)
          {
            baseTimeInSeconds = num1 >= this._startTimeInSeconds - 31536000.0 ? 0.0 : this._startTimeInSeconds;
            hasBaseTime = true;
          }
          double num3 = num1 + baseTimeInSeconds;
          double num4 = num3 + num2;
          HistogramBase histogramBase = HistogramLogReader.DecodeHistogram(ByteBuffer.Allocate(Convert.FromBase64String(s)), 0L);
          histogramBase.Tag = tag;
          histogramBase.StartTimeStamp = (long) (num3 * 1000.0);
          histogramBase.EndTimeStamp = (long) (num4 * 1000.0);
          yield return histogramBase;
        }
      }
    }

    IEnumerable<HistogramBase> IHistogramLogV1Reader.ReadHistograms()
    {
      this._startTimeInSeconds = 0.0;
      double baseTimeInSeconds = 0.0;
      bool hasStartTime = false;
      bool hasBaseTime = false;
      foreach (string readLine in this.ReadLines())
      {
        if (HistogramLogReader.IsComment(readLine))
        {
          if (HistogramLogReader.IsStartTime(readLine))
          {
            this._startTimeInSeconds = HistogramLogReader.ParseStartTime(readLine);
            hasStartTime = true;
          }
          else if (HistogramLogReader.IsBaseTime(readLine))
          {
            baseTimeInSeconds = HistogramLogReader.ParseBaseTime(readLine);
            hasBaseTime = true;
          }
        }
        else if (!HistogramLogReader.IsV1Legend(readLine))
        {
          Match match = HistogramLogReader.UntaggedLogLineMatcher.Match(readLine);
          double num1 = HistogramLogReader.ParseDouble(match, "startTime");
          double num2 = HistogramLogReader.ParseDouble(match, "interval");
          HistogramLogReader.ParseDouble(match, "max");
          string s = match.Groups["payload"].Value;
          if (!hasStartTime)
          {
            this._startTimeInSeconds = num1;
            hasStartTime = true;
          }
          if (!hasBaseTime)
          {
            baseTimeInSeconds = num1 >= this._startTimeInSeconds - 31536000.0 ? 0.0 : this._startTimeInSeconds;
            hasBaseTime = true;
          }
          double num3 = num1 + baseTimeInSeconds;
          double num4 = num3 + num2;
          HistogramBase histogramBase = HistogramLogReader.DecodeHistogram(ByteBuffer.Allocate(Convert.FromBase64String(s)), 0L);
          histogramBase.StartTimeStamp = (long) (num3 * 1000.0);
          histogramBase.EndTimeStamp = (long) (num4 * 1000.0);
          yield return histogramBase;
        }
      }
    }

    public DateTime GetStartTime() => this._startTimeInSeconds.ToDateFromSecondsSinceEpoch();

    private static HistogramBase DecodeHistogram(
      ByteBuffer buffer,
      long minBarForHighestTrackableValue)
    {
      return HistogramEncoding.DecodeFromCompressedByteBuffer(buffer, minBarForHighestTrackableValue);
    }

    private IEnumerable<string> ReadLines()
    {
      while (true)
      {
        string str = this._log.ReadLine();
        if (str != null)
          yield return str;
        else
          break;
      }
    }

    private static bool IsComment(string line) => line.StartsWith("#");

    private static bool IsStartTime(string line) => line.StartsWith("#[StartTime: ");

    private static bool IsBaseTime(string line) => line.StartsWith("#[BaseTime: ");

    private static bool IsLegend(string line)
    {
      string str = "\"StartTimestamp\",\"Interval_Length\",\"Interval_Max\",\"Interval_Compressed_Histogram\"";
      return line.Equals(str);
    }

    private static bool IsV1Legend(string line)
    {
      string str = "\"StartTimestamp\",\"EndTimestamp\",\"Interval_Max\",\"Interval_Compressed_Histogram\"";
      return line.Equals(str);
    }

    private static string ParseTag(string value)
    {
      if (string.IsNullOrWhiteSpace(value))
        return (string) null;
      value = value.Substring(4);
      return string.IsNullOrWhiteSpace(value) ? (string) null : value;
    }

    private static double ParseStartTime(string line) => HistogramLogReader.ParseDouble(HistogramLogReader.StartTimeMatcher.Match(line), "seconds");

    private static double ParseBaseTime(string line) => HistogramLogReader.ParseDouble(HistogramLogReader.BaseTimeMatcher.Match(line), "seconds");

    private static double ParseDouble(Match match, string group) => double.Parse(match.Groups[group].Value);

    public void Dispose()
    {
      using (this._log)
        ;
    }
  }
}
