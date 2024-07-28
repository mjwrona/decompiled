// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.Channels.TracingStream
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Client.Channels
{
  internal class TracingStream : Stream
  {
    private Stream m_innerStream;
    private long m_numBytesRead;
    private long m_numBytesWritten;
    private TraceSwitch m_traceSwitch;
    private byte[] m_dataRead = Array.Empty<byte>();
    private byte[] m_dataWritten = Array.Empty<byte>();

    public TracingStream(Stream s, TraceSwitch tracing)
    {
      this.m_innerStream = s;
      this.m_traceSwitch = tracing;
    }

    public override bool CanRead => this.m_innerStream.CanRead;

    public override bool CanSeek => this.m_innerStream.CanSeek;

    public override bool CanWrite => this.m_innerStream.CanWrite;

    public long NumBytesRead => this.m_numBytesRead;

    public long NumBytesWritten => this.m_numBytesWritten;

    public override long Length => this.m_innerStream.Length;

    public override long Position
    {
      get => this.m_innerStream.Position;
      set => this.m_innerStream.Position = value;
    }

    public override void Close()
    {
      if (this.m_innerStream == null)
        return;
      this.m_innerStream.Close();
      this.m_innerStream = (Stream) null;
      if (this.m_numBytesWritten > 0L)
      {
        if (this.m_traceSwitch.TraceInfo)
          TeamFoundationTrace.Info(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Write: {0}", (object) this.m_numBytesWritten));
        if (this.m_dataWritten.Length != 0 && this.m_traceSwitch.TraceVerbose)
          TeamFoundationTrace.Verbose(TracingStream.ToPrettyXml(Encoding.UTF8.GetString(this.m_dataWritten)));
      }
      if (this.m_numBytesRead <= 0L)
        return;
      if (this.m_traceSwitch.TraceInfo)
        TeamFoundationTrace.Info(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Read: {0}", (object) this.m_numBytesRead));
      if (this.m_dataRead.Length == 0 || !this.m_traceSwitch.TraceVerbose)
        return;
      TeamFoundationTrace.Verbose(TracingStream.ToPrettyXml(Encoding.UTF8.GetString(this.m_dataRead)));
    }

    private static string ToPrettyXml(string xml)
    {
      XmlDocument xmlDocument = new XmlDocument();
      try
      {
        XmlReaderSettings settings = new XmlReaderSettings()
        {
          DtdProcessing = DtdProcessing.Prohibit,
          XmlResolver = (XmlResolver) null
        };
        using (StringReader input = new StringReader(xml))
        {
          using (XmlReader reader = XmlReader.Create((TextReader) input, settings))
            xmlDocument.Load(reader);
        }
      }
      catch (XmlException ex)
      {
        return xml;
      }
      using (StringWriter w1 = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture))
      {
        using (XmlTextWriter w2 = new XmlTextWriter((TextWriter) w1))
        {
          w2.Formatting = Formatting.Indented;
          xmlDocument.WriteContentTo((XmlWriter) w2);
        }
        return w1.ToString();
      }
    }

    public override void Flush() => this.m_innerStream.Flush();

    public override IAsyncResult BeginRead(
      byte[] buffer,
      int offset,
      int count,
      AsyncCallback callback,
      object state)
    {
      return (IAsyncResult) new TracingStream.ReadOperation(this, buffer, offset, count, callback, state);
    }

    public override int EndRead(IAsyncResult result) => TracingStream.ReadOperation.End(result);

    public override IAsyncResult BeginWrite(
      byte[] buffer,
      int offset,
      int count,
      AsyncCallback callback,
      object state)
    {
      return (IAsyncResult) new TracingStream.WriteOperation(this, buffer, offset, count, callback, state);
    }

    public override void EndWrite(IAsyncResult result) => TracingStream.WriteOperation.End(result);

    public override long Seek(long offset, SeekOrigin origin) => this.m_innerStream.Seek(offset, origin);

    public override void SetLength(long value) => this.m_innerStream.SetLength(value);

    public override int Read(byte[] buffer, int offset, int count)
    {
      int length = this.m_innerStream.Read(buffer, offset, count);
      if (length > 0)
      {
        this.m_numBytesRead += (long) length;
        if (this.m_traceSwitch.TraceVerbose)
        {
          byte[] destinationArray = new byte[this.m_dataRead.Length + length];
          Array.Copy((Array) this.m_dataRead, (Array) destinationArray, this.m_dataRead.Length);
          Array.Copy((Array) buffer, offset, (Array) destinationArray, this.m_dataRead.Length, length);
          this.m_dataRead = destinationArray;
        }
      }
      return length;
    }

    public override int ReadByte()
    {
      int num = this.m_innerStream.ReadByte();
      if (num != -1)
      {
        ++this.m_numBytesRead;
        if (this.m_traceSwitch.TraceVerbose)
        {
          byte[] destinationArray = new byte[this.m_dataRead.Length + 1];
          Array.Copy((Array) this.m_dataRead, (Array) destinationArray, this.m_dataRead.Length);
          destinationArray[this.m_dataRead.Length] = (byte) num;
          this.m_dataRead = destinationArray;
        }
      }
      return num;
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      if (count > 0)
      {
        this.m_numBytesWritten += (long) count;
        if (this.m_traceSwitch.TraceVerbose)
        {
          byte[] destinationArray = new byte[this.m_dataWritten.Length + count];
          Array.Copy((Array) this.m_dataWritten, (Array) destinationArray, this.m_dataWritten.Length);
          Array.Copy((Array) buffer, offset, (Array) destinationArray, this.m_dataWritten.Length, count);
          this.m_dataWritten = destinationArray;
        }
      }
      this.m_innerStream.Write(buffer, offset, count);
    }

    public override void WriteByte(byte value)
    {
      ++this.m_numBytesWritten;
      if (this.m_traceSwitch.TraceVerbose)
      {
        byte[] destinationArray = new byte[this.m_dataWritten.Length + 1];
        Array.Copy((Array) this.m_dataWritten, (Array) destinationArray, this.m_dataWritten.Length);
        destinationArray[this.m_dataWritten.Length] = value;
        this.m_dataWritten = destinationArray;
      }
      this.m_innerStream.WriteByte(value);
    }

    private sealed class ReadOperation : AsyncOperation
    {
      private byte[] m_buffer;
      private int m_offset;
      private int m_bytesRead;
      private TracingStream m_stream;

      public ReadOperation(
        TracingStream stream,
        byte[] buffer,
        int offset,
        int count,
        AsyncCallback callback,
        object state)
        : base(callback, state)
      {
        this.m_stream = stream;
        this.m_offset = offset;
        this.m_buffer = buffer;
        IAsyncResult result = this.m_stream.m_innerStream.BeginRead(this.m_buffer, offset, count, new AsyncCallback(TracingStream.ReadOperation.EndRead), (object) this);
        if (!result.CompletedSynchronously)
          return;
        this.CompleteRead(result);
      }

      private static void EndRead(IAsyncResult result)
      {
        if (result.CompletedSynchronously)
          return;
        TracingStream.ReadOperation asyncState = (TracingStream.ReadOperation) result.AsyncState;
        bool flag = true;
        Exception exception = (Exception) null;
        try
        {
          flag = asyncState.CompleteRead(result);
        }
        catch (Exception ex)
        {
          exception = ex;
        }
        if (!flag)
          return;
        asyncState.Complete(false, exception);
      }

      private bool CompleteRead(IAsyncResult result)
      {
        this.m_bytesRead = this.m_stream.m_innerStream.EndRead(result);
        if (this.m_bytesRead > 0)
        {
          this.m_stream.m_numBytesRead += (long) this.m_bytesRead;
          if (this.m_stream.m_traceSwitch.TraceVerbose)
          {
            byte[] destinationArray = new byte[this.m_stream.m_dataRead.Length + this.m_bytesRead];
            Array.Copy((Array) this.m_stream.m_dataRead, (Array) destinationArray, this.m_stream.m_dataRead.Length);
            Array.Copy((Array) this.m_buffer, this.m_offset, (Array) destinationArray, this.m_stream.m_dataRead.Length, this.m_bytesRead);
            this.m_stream.m_dataRead = destinationArray;
          }
        }
        return true;
      }

      public static int End(IAsyncResult result) => AsyncOperation.End<TracingStream.ReadOperation>(result).m_bytesRead;
    }

    private sealed class WriteOperation : AsyncOperation
    {
      private int m_count;
      private int m_offset;
      private byte[] m_buffer;
      private TracingStream m_stream;

      public WriteOperation(
        TracingStream stream,
        byte[] buffer,
        int offset,
        int count,
        AsyncCallback callback,
        object state)
        : base(callback, state)
      {
        this.m_count = count;
        this.m_stream = stream;
        this.m_offset = offset;
        this.m_buffer = buffer;
        IAsyncResult result = this.m_stream.m_innerStream.BeginWrite(this.m_buffer, this.m_offset, this.m_count, new AsyncCallback(TracingStream.WriteOperation.EndWrite), (object) this);
        if (!result.CompletedSynchronously)
          return;
        this.CompleteWrite(result);
      }

      private static void EndWrite(IAsyncResult result)
      {
        if (result.CompletedSynchronously)
          return;
        TracingStream.WriteOperation asyncState = (TracingStream.WriteOperation) result.AsyncState;
        bool flag = true;
        Exception exception = (Exception) null;
        try
        {
          flag = asyncState.CompleteWrite(result);
        }
        catch (Exception ex)
        {
          exception = ex;
        }
        if (!flag)
          return;
        asyncState.Complete(false, exception);
      }

      private bool CompleteWrite(IAsyncResult result)
      {
        this.m_stream.m_innerStream.EndWrite(result);
        if (this.m_count > 0)
        {
          this.m_stream.m_numBytesWritten += (long) this.m_count;
          if (this.m_stream.m_traceSwitch.TraceVerbose)
          {
            byte[] destinationArray = new byte[this.m_stream.m_dataWritten.Length + this.m_count];
            Array.Copy((Array) this.m_stream.m_dataWritten, (Array) destinationArray, this.m_stream.m_dataWritten.Length);
            Array.Copy((Array) this.m_buffer, this.m_offset, (Array) destinationArray, this.m_stream.m_dataWritten.Length, this.m_count);
            this.m_stream.m_dataWritten = destinationArray;
          }
        }
        return true;
      }

      public static void End(IAsyncResult result) => AsyncOperation.End<TracingStream.WriteOperation>(result);
    }
  }
}
