// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.JsonWriterStream
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public sealed class JsonWriterStream : Stream
  {
    private readonly MemoryStream streamWriterTarget;
    private readonly StreamWriter streamWriter;
    private readonly JsonWriter jsonTextWriter;
    private readonly Queue<byte> queue = new Queue<byte>();
    private readonly IConcurrentIterator<JsonWrite> writerActionsEnumerator;

    public static JsonStream Create(Formatting formatting, JsonWrites writes) => new JsonStream((Stream) new JsonWriterStream(formatting, writes));

    public static JsonStream Create(Formatting formatting, params JsonWrites[] writes) => new JsonStream((Stream) new JsonWriterStream(formatting, writes));

    private JsonWriterStream(Formatting formatting, JsonWrites writes)
    {
      this.writerActionsEnumerator = writes.Writes;
      this.streamWriterTarget = new MemoryStream();
      this.streamWriter = new StreamWriter((Stream) this.streamWriterTarget, StrictEncodingWithoutBOM.UTF8);
      JsonTextWriter jsonTextWriter = new JsonTextWriter((TextWriter) this.streamWriter);
      jsonTextWriter.Formatting = formatting;
      this.jsonTextWriter = (JsonWriter) jsonTextWriter;
    }

    private JsonWriterStream(Formatting formatting, params JsonWrites[] writes)
      : this(formatting, new JsonWrites(((IEnumerable<JsonWrites>) writes).Select<JsonWrites, IConcurrentIterator<JsonWrite>>((Func<JsonWrites, IConcurrentIterator<JsonWrite>>) (write => write.Writes)).CollectOrdered<JsonWrite>(CancellationToken.None)))
    {
    }

    protected override void Dispose(bool disposing)
    {
      this.writerActionsEnumerator.Dispose();
      this.streamWriter.Dispose();
      base.Dispose(disposing);
    }

    public override int Read(byte[] buffer, int offset, int count) => Task.Run<int>((Func<Task<int>>) (() => this.ReadAsync(buffer, offset, count, CancellationToken.None))).Result;

    public override IAsyncResult BeginRead(
      byte[] buffer,
      int offset,
      int count,
      AsyncCallback callback,
      object state)
    {
      SafeTaskCompletionSource<int> tcs = new SafeTaskCompletionSource<int>(state);
      this.ReadAsync(buffer, offset, count).ContinueWith((Action<Task<int>>) (t =>
      {
        if (t.IsFaulted)
          tcs.TrySetException((IEnumerable<Exception>) t.Exception.InnerExceptions);
        else if (t.IsCanceled)
          tcs.TrySetCanceled();
        else
          tcs.TrySetResult(t.Result);
        AsyncCallback asyncCallback = callback;
        if (asyncCallback == null)
          return;
        asyncCallback((IAsyncResult) tcs.Task);
      }), CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.Default);
      return (IAsyncResult) tcs.Task;
    }

    public override int EndRead(IAsyncResult asyncResult) => ((Task<int>) asyncResult).Result;

    public override async Task<int> ReadAsync(
      byte[] buffer,
      int offset,
      int count,
      CancellationToken cancellationToken)
    {
      int bytesRead = 0;
      while (count > 0)
      {
        while (count > 0 && this.queue.Count > 0)
        {
          buffer[offset] = this.queue.Dequeue();
          --count;
          ++offset;
          ++bytesRead;
        }
        if (count != 0)
        {
          if (await this.writerActionsEnumerator.MoveNextAsync(cancellationToken).ConfigureAwait(false))
          {
            this.writerActionsEnumerator.Current(this.jsonTextWriter);
            this.jsonTextWriter.Flush();
            this.streamWriter.Flush();
            foreach (byte num in ((IEnumerable<byte>) this.streamWriterTarget.GetBuffer()).Take<byte>((int) this.streamWriterTarget.Length))
              this.queue.Enqueue(num);
            this.streamWriterTarget.SetLength(0L);
          }
          else
            break;
        }
        else
          break;
      }
      return bytesRead;
    }

    public override void Flush()
    {
    }

    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

    public override void SetLength(long value) => throw new NotSupportedException();

    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

    public override bool CanRead => true;

    public override bool CanSeek => false;

    public override bool CanWrite => false;

    public override long Length => throw new NotSupportedException();

    public override long Position
    {
      get => throw new NotSupportedException();
      set => throw new NotSupportedException();
    }
  }
}
