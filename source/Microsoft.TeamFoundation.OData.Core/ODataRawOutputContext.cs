// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataRawOutputContext
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Metadata;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData
{
  internal class ODataRawOutputContext : ODataOutputContext
  {
    protected Encoding encoding;
    protected IODataOutputInStreamErrorListener outputInStreamErrorListener;
    private Stream messageOutputStream;
    private AsyncBufferedStream asynchronousOutputStream;
    private Stream outputStream;
    private RawValueWriter rawValueWriter;

    internal ODataRawOutputContext(
      ODataFormat format,
      ODataMessageInfo messageInfo,
      ODataMessageWriterSettings messageWriterSettings)
      : base(format, messageInfo, messageWriterSettings)
    {
      try
      {
        this.messageOutputStream = messageInfo.MessageStream;
        this.encoding = messageInfo.Encoding;
        if (this.Synchronous)
        {
          this.outputStream = this.messageOutputStream;
        }
        else
        {
          this.asynchronousOutputStream = new AsyncBufferedStream(this.messageOutputStream);
          this.outputStream = (Stream) this.asynchronousOutputStream;
        }
      }
      catch
      {
        this.messageOutputStream.Dispose();
        throw;
      }
    }

    internal Stream OutputStream => this.outputStream;

    internal TextWriter TextWriter => this.rawValueWriter.TextWriter;

    internal void Flush()
    {
      if (this.rawValueWriter == null)
        return;
      this.rawValueWriter.Flush();
    }

    internal Task FlushAsync() => TaskUtils.GetTaskForSynchronousOperationReturningTask((Func<Task>) (() =>
    {
      if (this.rawValueWriter != null)
        this.rawValueWriter.Flush();
      return this.asynchronousOutputStream.FlushAsync();
    })).FollowOnSuccessWithTask((Func<Task, Task>) (asyncBufferedStreamFlushTask => this.messageOutputStream.FlushAsync()));

    internal override void WriteInStreamError(ODataError error, bool includeDebugInformation)
    {
      if (this.outputInStreamErrorListener != null)
        this.outputInStreamErrorListener.OnInStreamError();
      throw new ODataException(Strings.ODataMessageWriter_CannotWriteInStreamErrorForRawValues);
    }

    internal override Task WriteInStreamErrorAsync(ODataError error, bool includeDebugInformation)
    {
      if (this.outputInStreamErrorListener != null)
        this.outputInStreamErrorListener.OnInStreamError();
      throw new ODataException(Strings.ODataMessageWriter_CannotWriteInStreamErrorForRawValues);
    }

    internal override ODataAsynchronousWriter CreateODataAsynchronousWriter() => this.CreateODataAsynchronousWriterImplementation();

    internal override Task<ODataAsynchronousWriter> CreateODataAsynchronousWriterAsync() => TaskUtils.GetTaskForSynchronousOperation<ODataAsynchronousWriter>((Func<ODataAsynchronousWriter>) (() => this.CreateODataAsynchronousWriterImplementation()));

    internal override void WriteValue(object value)
    {
      this.WriteValueImplementation(value);
      this.Flush();
    }

    internal override Task WriteValueAsync(object value) => TaskUtils.GetTaskForSynchronousOperationReturningTask((Func<Task>) (() =>
    {
      this.WriteValueImplementation(value);
      return this.FlushAsync();
    }));

    internal void InitializeRawValueWriter() => this.rawValueWriter = new RawValueWriter(this.MessageWriterSettings, this.outputStream, this.encoding);

    internal void CloseWriter()
    {
      this.rawValueWriter.Dispose();
      this.rawValueWriter = (RawValueWriter) null;
    }

    internal void VerifyNotDisposed()
    {
      if (this.messageOutputStream == null)
        throw new ObjectDisposedException(this.GetType().FullName);
    }

    internal void FlushBuffers()
    {
      if (this.asynchronousOutputStream == null)
        return;
      this.asynchronousOutputStream.FlushSync();
    }

    internal Task FlushBuffersAsync() => this.asynchronousOutputStream != null ? this.asynchronousOutputStream.FlushAsync() : TaskUtils.CompletedTask;

    [SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "rawValueWriter", Justification = "We intentionally don't dispose rawValueWriter, we instead dispose the underlying stream manually.")]
    protected override void Dispose(bool disposing)
    {
      try
      {
        if (this.messageOutputStream != null)
        {
          if (this.rawValueWriter != null)
            this.rawValueWriter.Flush();
          if (this.asynchronousOutputStream != null)
          {
            this.asynchronousOutputStream.FlushSync();
            this.asynchronousOutputStream.Dispose();
          }
          this.messageOutputStream.Dispose();
        }
      }
      finally
      {
        this.messageOutputStream = (Stream) null;
        this.asynchronousOutputStream = (AsyncBufferedStream) null;
        this.outputStream = (Stream) null;
        this.rawValueWriter = (RawValueWriter) null;
      }
      base.Dispose(disposing);
    }

    private void WriteValueImplementation(object value)
    {
      if (value is byte[] buffer)
      {
        this.OutputStream.Write(buffer, 0, buffer.Length);
      }
      else
      {
        value = this.Model.ConvertToUnderlyingTypeIfUIntValue(value);
        this.InitializeRawValueWriter();
        this.rawValueWriter.Start();
        this.rawValueWriter.WriteRawValue(value);
        this.rawValueWriter.End();
      }
    }

    private ODataAsynchronousWriter CreateODataAsynchronousWriterImplementation()
    {
      this.encoding = this.encoding ?? (Encoding) MediaTypeUtils.EncodingUtf8NoPreamble;
      ODataAsynchronousWriter writerImplementation = new ODataAsynchronousWriter(this);
      this.outputInStreamErrorListener = (IODataOutputInStreamErrorListener) writerImplementation;
      return writerImplementation;
    }
  }
}
