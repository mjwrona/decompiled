// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataMessage
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.OData
{
  [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "This class does not own the BufferingReadStream instance.")]
  internal abstract class ODataMessage
  {
    private readonly bool writing;
    private readonly bool enableMessageStreamDisposal;
    private readonly long maxMessageSize;
    private bool? useBufferingReadStream;
    private BufferingReadStream bufferingReadStream;

    protected ODataMessage(bool writing, bool enableMessageStreamDisposal, long maxMessageSize)
    {
      this.writing = writing;
      this.enableMessageStreamDisposal = enableMessageStreamDisposal;
      this.maxMessageSize = maxMessageSize;
    }

    public abstract IEnumerable<KeyValuePair<string, string>> Headers { get; }

    protected internal BufferingReadStream BufferingReadStream => this.bufferingReadStream;

    protected internal bool? UseBufferingReadStream
    {
      get => this.useBufferingReadStream;
      set => this.useBufferingReadStream = value;
    }

    public abstract string GetHeader(string headerName);

    public abstract void SetHeader(string headerName, string headerValue);

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Intentionally a method.")]
    public abstract Stream GetStream();

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Intentionally a method.")]
    public abstract Task<Stream> GetStreamAsync();

    internal abstract TInterface QueryInterface<TInterface>() where TInterface : class;

    protected internal Stream GetStream(Func<Stream> messageStreamFunc, bool isRequest)
    {
      if (!this.writing)
      {
        BufferingReadStream bufferingReadStream = this.TryGetBufferingReadStream();
        if (bufferingReadStream != null)
          return (Stream) bufferingReadStream;
      }
      Stream stream = messageStreamFunc();
      ODataMessage.ValidateMessageStream(stream, isRequest);
      bool flag1 = !this.writing && this.maxMessageSize > 0L;
      if (!this.enableMessageStreamDisposal & flag1)
        stream = MessageStreamWrapper.CreateNonDisposingStreamWithMaxSize(stream, this.maxMessageSize);
      else if (!this.enableMessageStreamDisposal)
        stream = MessageStreamWrapper.CreateNonDisposingStream(stream);
      else if (flag1)
        stream = MessageStreamWrapper.CreateStreamWithMaxSize(stream, this.maxMessageSize);
      if (!this.writing)
      {
        bool? bufferingReadStream = this.useBufferingReadStream;
        bool flag2 = true;
        if (bufferingReadStream.GetValueOrDefault() == flag2 & bufferingReadStream.HasValue)
        {
          this.bufferingReadStream = new BufferingReadStream(stream);
          stream = (Stream) this.bufferingReadStream;
        }
      }
      return stream;
    }

    protected internal Task<Stream> GetStreamAsync(
      Func<Task<Stream>> streamFuncAsync,
      bool isRequest)
    {
      if (!this.writing)
      {
        Stream bufferingReadStream = (Stream) this.TryGetBufferingReadStream();
        if (bufferingReadStream != null)
          return TaskUtils.GetCompletedTask<Stream>(bufferingReadStream);
      }
      Task<Stream> task = streamFuncAsync();
      ODataMessage.ValidateMessageStreamTask(task, isRequest);
      Task<Stream> antecedentTask = task.FollowOnSuccessWith<Stream, Stream>((Func<Task<Stream>, Stream>) (streamTask =>
      {
        Stream streamAsync = streamTask.Result;
        ODataMessage.ValidateMessageStream(streamAsync, isRequest);
        bool flag = !this.writing && this.maxMessageSize > 0L;
        if (!this.enableMessageStreamDisposal & flag)
          streamAsync = MessageStreamWrapper.CreateNonDisposingStreamWithMaxSize(streamAsync, this.maxMessageSize);
        else if (!this.enableMessageStreamDisposal)
          streamAsync = MessageStreamWrapper.CreateNonDisposingStream(streamAsync);
        else if (flag)
          streamAsync = MessageStreamWrapper.CreateStreamWithMaxSize(streamAsync, this.maxMessageSize);
        return streamAsync;
      }));
      if (!this.writing)
      {
        antecedentTask = antecedentTask.FollowOnSuccessWithTask<Stream, BufferedReadStream>((Func<Task<Stream>, Task<BufferedReadStream>>) (streamTask => BufferedReadStream.BufferStreamAsync(streamTask.Result))).FollowOnSuccessWith<BufferedReadStream, Stream>((Func<Task<BufferedReadStream>, Stream>) (streamTask => (Stream) streamTask.Result));
        bool? bufferingReadStream = this.useBufferingReadStream;
        bool flag = true;
        if (bufferingReadStream.GetValueOrDefault() == flag & bufferingReadStream.HasValue)
          antecedentTask = antecedentTask.FollowOnSuccessWith<Stream, Stream>((Func<Task<Stream>, Stream>) (streamTask =>
          {
            this.bufferingReadStream = new BufferingReadStream(streamTask.Result);
            return (Stream) this.bufferingReadStream;
          }));
      }
      return antecedentTask;
    }

    protected void VerifyCanSetHeader()
    {
      if (!this.writing)
        throw new ODataException(Strings.ODataMessage_MustNotModifyMessage);
    }

    private static void ValidateMessageStream(Stream stream, bool isRequest)
    {
      if (stream == null)
        throw new ODataException(isRequest ? Strings.ODataRequestMessage_MessageStreamIsNull : Strings.ODataResponseMessage_MessageStreamIsNull);
    }

    private static void ValidateMessageStreamTask(Task<Stream> streamTask, bool isRequest)
    {
      if (streamTask == null)
        throw new ODataException(isRequest ? Strings.ODataRequestMessage_StreamTaskIsNull : Strings.ODataResponseMessage_StreamTaskIsNull);
    }

    private BufferingReadStream TryGetBufferingReadStream()
    {
      if (this.bufferingReadStream == null)
        return (BufferingReadStream) null;
      BufferingReadStream bufferingReadStream = this.bufferingReadStream;
      if (this.bufferingReadStream.IsBuffering)
        this.bufferingReadStream.ResetStream();
      else
        this.bufferingReadStream = (BufferingReadStream) null;
      return bufferingReadStream;
    }
  }
}
