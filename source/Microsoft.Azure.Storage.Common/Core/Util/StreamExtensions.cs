// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Core.Util.StreamExtensions
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using Microsoft.Azure.Storage.Core.Executor;
using Microsoft.Azure.Storage.Shared.Protocol;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Storage.Core.Util
{
  internal static class StreamExtensions
  {
    internal static Stream AsInputStream(this Stream stream) => stream;

    [DebuggerNonUserCode]
    internal static int GetBufferSize(Stream inStream) => inStream.CanSeek && inStream.Length - inStream.Position > 0L ? (int) Math.Min(inStream.Length - inStream.Position, 65536L) : 65536;

    public static long Seek(this Stream stream, long offset) => stream.Seek(offset, SeekOrigin.Begin);

    [DebuggerNonUserCode]
    internal static void WriteToSync<T>(
      this Stream stream,
      Stream toStream,
      long? copyLength,
      long? maxLength,
      ChecksumRequested calculateChecksum,
      bool syncRead,
      ExecutionState<T> executionState,
      StreamDescriptor streamCopyState)
    {
      if (copyLength.HasValue && maxLength.HasValue)
        throw new ArgumentException("Cannot specify both copyLength and maxLength.");
      if (stream.CanSeek && maxLength.HasValue)
      {
        long num = stream.Length - stream.Position;
        long? nullable = maxLength;
        long valueOrDefault = nullable.GetValueOrDefault();
        if (num > valueOrDefault & nullable.HasValue)
          throw new InvalidOperationException("The length of the stream exceeds the permitted length.");
      }
      if (stream.CanSeek && copyLength.HasValue)
      {
        long num = stream.Length - stream.Position;
        long? nullable = copyLength;
        long valueOrDefault = nullable.GetValueOrDefault();
        if (num < valueOrDefault & nullable.HasValue)
          throw new ArgumentOutOfRangeException(nameof (copyLength), "The requested number of bytes exceeds the length of the stream remaining from the specified position.");
      }
      byte[] numArray = new byte[StreamExtensions.GetBufferSize(stream)];
      if (streamCopyState != null && calculateChecksum.HasAny && streamCopyState.ChecksumWrapper == null)
        streamCopyState.ChecksumWrapper = new ChecksumWrapper(calculateChecksum.MD5, calculateChecksum.CRC64);
      RegisteredWaitHandle registeredWaitHandle = (RegisteredWaitHandle) null;
      ManualResetEvent waitObject = (ManualResetEvent) null;
      if (!syncRead && executionState.OperationExpiryTime.HasValue)
      {
        waitObject = new ManualResetEvent(false);
        registeredWaitHandle = ThreadPool.RegisterWaitForSingleObject((WaitHandle) waitObject, new WaitOrTimerCallback(StreamExtensions.MaximumCopyTimeCallback<T>), (object) executionState, executionState.RemainingTimeout, true);
      }
      try
      {
        long? val1 = copyLength;
        int count;
        long? nullable;
        do
        {
          if (executionState.OperationExpiryTime.HasValue && DateTime.Now.CompareTo(executionState.OperationExpiryTime.Value) > 0)
            throw Exceptions.GenerateTimeoutException(executionState.Cmd != null ? executionState.Cmd.CurrentResult : (RequestResult) null, (Exception) null);
          int read = StreamExtensions.MinBytesToRead(val1, numArray.Length);
          if (read != 0)
          {
            count = syncRead ? stream.Read(numArray, 0, read) : stream.EndRead(stream.BeginRead(numArray, 0, read, (AsyncCallback) null, (object) null));
            if (val1.HasValue)
            {
              nullable = val1;
              long num = (long) count;
              val1 = nullable.HasValue ? new long?(nullable.GetValueOrDefault() - num) : new long?();
            }
            if (count > 0)
            {
              toStream.Write(numArray, 0, count);
              if (streamCopyState != null)
              {
                streamCopyState.Length += (long) count;
                if (maxLength.HasValue && streamCopyState.Length > maxLength.Value)
                  throw new InvalidOperationException("The length of the stream exceeds the permitted length.");
                if (streamCopyState.ChecksumWrapper != null)
                  streamCopyState.ChecksumWrapper.UpdateHash(numArray, 0, count);
              }
            }
          }
          else
            break;
        }
        while (count != 0);
        if (val1.HasValue)
        {
          nullable = val1;
          long num = 0;
          if (!(nullable.GetValueOrDefault() == num & nullable.HasValue))
            throw new ArgumentOutOfRangeException(nameof (copyLength), "The requested number of bytes exceeds the length of the stream remaining from the specified position.");
        }
      }
      catch (Exception ex)
      {
        if (executionState.OperationExpiryTime.HasValue && DateTime.Now.CompareTo(executionState.OperationExpiryTime.Value) > 0)
          throw Exceptions.GenerateTimeoutException(executionState.Cmd != null ? executionState.Cmd.CurrentResult : (RequestResult) null, (Exception) null);
        throw;
      }
      finally
      {
        registeredWaitHandle?.Unregister((WaitHandle) null);
        waitObject?.Close();
      }
      if (streamCopyState == null || streamCopyState.ChecksumWrapper == null)
        return;
      if (streamCopyState.ChecksumWrapper.CRC64 != null)
        streamCopyState.Crc64 = streamCopyState.ChecksumWrapper.CRC64.ComputeHash();
      if (streamCopyState.ChecksumWrapper.MD5 != null)
        streamCopyState.Md5 = streamCopyState.ChecksumWrapper.MD5.ComputeHash();
      streamCopyState.ChecksumWrapper = (ChecksumWrapper) null;
    }

    private static void MaximumCopyTimeCallback<T>(object state, bool timedOut)
    {
      ExecutionState<T> executionState = (ExecutionState<T>) state;
      if (!timedOut)
        return;
      if (executionState.Req == null)
        return;
      try
      {
        executionState.ReqTimedOut = true;
        executionState.CancellationTokenSource.Cancel();
      }
      catch (Exception ex)
      {
      }
    }

    private static int MinBytesToRead(long? val1, int val2)
    {
      if (val1.HasValue)
      {
        long? nullable = val1;
        long num = (long) val2;
        if (nullable.GetValueOrDefault() < num & nullable.HasValue)
          return (int) val1.Value;
      }
      return val2;
    }

    [DebuggerNonUserCode]
    internal static Stream WrapWithByteCountingStream(
      this Stream stream,
      RequestResult result,
      bool reverseCapture = false)
    {
      return (Stream) new ByteCountingStream(stream, result, reverseCapture);
    }

    [DebuggerNonUserCode]
    internal static Task WriteToAsync<T>(
      this Stream stream,
      Stream toStream,
      IBufferManager bufferManager,
      long? copyLength,
      long? maxLength,
      ChecksumRequested calculateChecksum,
      ExecutionState<T> executionState,
      StreamDescriptor streamCopyState,
      CancellationToken cancellationToken,
      Action<ExecutionState<T>> completed = null)
    {
      return new AsyncStreamCopier<T>(stream, toStream, executionState, bufferManager, new int?(StreamExtensions.GetBufferSize(stream)), calculateChecksum, streamCopyState).StartCopyStream(completed, copyLength, maxLength, cancellationToken);
    }
  }
}
