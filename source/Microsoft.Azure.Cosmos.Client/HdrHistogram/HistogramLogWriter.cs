// Decompiled with JetBrains decompiler
// Type: HdrHistogram.HistogramLogWriter
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using HdrHistogram.Utilities;
using System;
using System.IO;
using System.Text;
using System.Threading;

namespace HdrHistogram
{
  internal sealed class HistogramLogWriter : IDisposable
  {
    private const string HistogramLogFormatVersion = "1.3";
    private readonly TextWriter _log;
    private bool _hasHeaderWritten;
    private int _isDisposed;

    public static void Write(
      Stream outputStream,
      DateTime startTime,
      params HistogramBase[] histograms)
    {
      using (HistogramLogWriter histogramLogWriter = new HistogramLogWriter(outputStream))
        histogramLogWriter.Write(startTime, histograms);
    }

    public HistogramLogWriter(Stream outputStream)
    {
      StreamWriter streamWriter = new StreamWriter(outputStream, Encoding.Unicode, 1024, true);
      streamWriter.NewLine = "\n";
      this._log = (TextWriter) streamWriter;
    }

    public void Write(DateTime startTime, params HistogramBase[] histograms)
    {
      this.WriteLogFormatVersion();
      this.WriteStartTime(startTime);
      this.WriteLegend();
      this._hasHeaderWritten = true;
      foreach (HistogramBase histogram in histograms)
        this.WriteHistogram(histogram);
    }

    public void Append(HistogramBase histogram)
    {
      if (!this._hasHeaderWritten)
        this.Write(histogram.StartTimeStamp.ToDateFromMillisecondsSinceEpoch(), histogram);
      else
        this.WriteHistogram(histogram);
    }

    private void WriteLogFormatVersion()
    {
      this._log.WriteLine("#[Histogram log format version 1.3]");
      this._log.Flush();
    }

    private void WriteStartTime(DateTime startTimeWritten)
    {
      this._log.WriteLine(string.Format("#[StartTime: {0:F3} (seconds since epoch), {1:o}]", (object) startTimeWritten.SecondsSinceUnixEpoch(), (object) startTimeWritten));
      this._log.Flush();
    }

    private void WriteLegend()
    {
      this._log.WriteLine("\"StartTimestamp\",\"Interval_Length\",\"Interval_Max\",\"Interval_Compressed_Histogram\"");
      this._log.Flush();
    }

    private void WriteHistogram(HistogramBase histogram)
    {
      ByteBuffer targetBuffer = ByteBuffer.Allocate(histogram.GetNeededByteBufferCapacity());
      int count = histogram.EncodeIntoCompressedByteBuffer(targetBuffer);
      byte[] numArray = new byte[count];
      targetBuffer.BlockGet((Array) numArray, 0, 0, count);
      double num1 = (double) histogram.StartTimeStamp / 1000.0;
      double num2 = (double) histogram.EndTimeStamp / 1000.0 - num1;
      double num3 = 1000000.0;
      double num4 = (double) histogram.GetMaxValue() / num3;
      string base64String = Convert.ToBase64String(numArray);
      string str;
      if (histogram.Tag != null)
        str = string.Format("Tag={0},{1:F3},{2:F3},{3:F3},{4}", (object) histogram.Tag, (object) num1, (object) num2, (object) num4, (object) base64String);
      else
        str = string.Format("{0:F3},{1:F3},{2:F3},{3}", (object) num1, (object) num2, (object) num4, (object) base64String);
      this._log.WriteLine(str);
      this._log.Flush();
    }

    public void Dispose()
    {
      if (Interlocked.CompareExchange(ref this._isDisposed, 1, 0) != 0)
        return;
      using (this._log)
        ;
    }
  }
}
