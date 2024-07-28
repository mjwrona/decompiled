// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.FileBufferedStream
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class FileBufferedStream : FileBufferedStreamBase
  {
    private readonly string m_filename;
    private readonly FileStream m_readStream;
    private readonly SafeFileHandle m_writeHandle;
    private readonly Stream m_inputStream;
    private readonly bool m_leaveOpen;
    private readonly byte[] m_oneByte = new byte[1];
    private readonly FileBufferedStreamBase.TEST_Args m_TEST_args;
    private ByteArray m_buffer;
    private bool m_closed;
    private bool m_disposed;
    private readonly object m_lock = new object();
    private long m_bytes;
    private bool m_eof;
    private ExceptionDispatchInfo m_exception;
    private long m_position;
    private const int c_bufferSize = 65536;

    public FileBufferedStream(
      IVssRequestContext requestContext,
      Guid repositoryId,
      Stream inputStream,
      bool leaveOpen = false,
      long sizeLimit = 9223372036854775807,
      FileBufferedStreamBase.TEST_Args TEST_args = null)
      : base(sizeLimit)
    {
      this.m_inputStream = inputStream;
      this.m_leaveOpen = leaveOpen;
      this.m_TEST_args = TEST_args;
      this.m_filename = Path.Combine(GitServerUtils.GetCacheDirectory(requestContext, repositoryId), Guid.NewGuid().ToString("N"));
      this.m_writeHandle = Microsoft.TeamFoundation.Common.Internal.NativeMethods.CreateFile(this.m_filename, Microsoft.TeamFoundation.Common.Internal.NativeMethods.FileAccess.GenericWrite, Microsoft.TeamFoundation.Common.Internal.NativeMethods.FileShare.Read, IntPtr.Zero, Microsoft.TeamFoundation.Common.Internal.NativeMethods.CreationDisposition.CreateAlways, Microsoft.TeamFoundation.Common.Internal.NativeMethods.FileAttributes.Temporary | Microsoft.TeamFoundation.Common.Internal.NativeMethods.FileAttributes.DeleteOnClose, IntPtr.Zero);
      if (this.m_writeHandle.IsInvalid)
        throw new Win32Exception();
      this.m_readStream = new FileStream(this.m_filename, FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite | System.IO.FileShare.Delete, 65536);
      this.m_buffer = new ByteArray(65536);
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      ThreadPool.QueueUserWorkItem(FileBufferedStream.\u003C\u003EO.\u003C0\u003E__WriteCallback ?? (FileBufferedStream.\u003C\u003EO.\u003C0\u003E__WriteCallback = new WaitCallback(FileBufferedStream.WriteCallback)), (object) this);
    }

    public override string Name => this.m_filename;

    public override void Close()
    {
      if (!this.m_closed)
      {
        this.m_closed = true;
        lock (this.m_lock)
        {
          while (!this.m_eof)
            Monitor.Wait(this.m_lock);
        }
        if (!this.m_leaveOpen)
          this.m_inputStream.Close();
        if (this.m_buffer != null)
        {
          this.m_buffer.Dispose();
          this.m_buffer = (ByteArray) null;
        }
      }
      base.Close();
    }

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
      if (!disposing || this.m_disposed)
        return;
      this.m_disposed = true;
      this.m_readStream.Dispose();
      this.m_writeHandle.Dispose();
    }

    public override bool BufferingComplete
    {
      get
      {
        lock (this.m_lock)
          return this.m_eof;
      }
    }

    public override Exception Exception => this.m_exception?.SourceException;

    public override bool CanRead => true;

    public override int ReadByte() => this.Read(this.m_oneByte, 0, 1) == 0 ? -1 : (int) this.m_oneByte[0];

    public override int Read(byte[] buffer, int offset, int count)
    {
      ArgumentUtility.CheckForOutOfRange(offset, nameof (offset), 0);
      ArgumentUtility.CheckForOutOfRange(count, nameof (count), 0);
      if (this.m_closed || this.m_disposed)
        throw new ObjectDisposedException(this.GetType().Name);
      lock (this.m_lock)
      {
        while (!this.m_eof && this.m_bytes < this.m_position + (long) count)
          Monitor.Wait(this.m_lock);
        if (this.m_eof && this.m_bytes < this.m_position + (long) count && this.m_exception != null)
          this.m_exception.Throw();
        int num = this.m_eof ? 1 : 0;
      }
      int num1 = this.m_readStream.Read(buffer, offset, count);
      this.m_position += (long) num1;
      return num1;
    }

    public override bool CanSeek => true;

    public override long Position
    {
      get => this.m_position;
      set => this.Seek(value, SeekOrigin.Begin);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      if (this.m_closed || this.m_disposed)
        throw new ObjectDisposedException(this.GetType().Name);
      long var;
      switch (origin)
      {
        case SeekOrigin.Begin:
          var = offset;
          break;
        case SeekOrigin.Current:
          var = this.m_position + offset;
          break;
        case SeekOrigin.End:
          lock (this.m_lock)
          {
            if (this.m_TEST_args != null && this.m_TEST_args.ShouldWaitForEofIfSeekingFromEnd)
            {
              while (!this.m_eof)
                Monitor.Wait(this.m_lock);
            }
            if (!this.m_eof)
              throw new InvalidOperationException();
            var = this.m_bytes + offset;
            break;
          }
        default:
          throw new InvalidOperationException();
      }
      ArgumentUtility.CheckForOutOfRange(var, "newPosition", 0L);
      this.m_readStream.Seek(var - this.m_position, SeekOrigin.Current);
      this.m_position = var;
      return var;
    }

    private static void WriteCallback(object state)
    {
      FileBufferedStream fileBufferedStream = (FileBufferedStream) state;
      bool flag = false;
      try
      {
        while (!flag)
        {
          int numBytesToWrite = fileBufferedStream.m_inputStream.Read(fileBufferedStream.m_buffer.Bytes, 0, fileBufferedStream.m_buffer.SizeRequested);
          if (numBytesToWrite == 0)
          {
            lock (fileBufferedStream.m_lock)
            {
              fileBufferedStream.m_eof = true;
              Monitor.Pulse(fileBufferedStream.m_lock);
              break;
            }
          }
          else
          {
            if (!Microsoft.TeamFoundation.Common.Internal.NativeMethods.WriteFile(fileBufferedStream.m_writeHandle.DangerousGetHandle(), fileBufferedStream.m_buffer.Bytes, numBytesToWrite))
              throw new Win32Exception();
            lock (fileBufferedStream.m_lock)
            {
              flag = fileBufferedStream.m_closed;
              fileBufferedStream.m_bytes += (long) numBytesToWrite;
              if (flag)
                fileBufferedStream.m_eof = true;
              Monitor.Pulse(fileBufferedStream.m_lock);
            }
            if (fileBufferedStream.m_bytes > fileBufferedStream.SizeLimit)
              throw new FileSizeLimitReachedException();
          }
        }
      }
      catch (Exception ex)
      {
        lock (fileBufferedStream.m_lock)
        {
          fileBufferedStream.m_exception = ExceptionDispatchInfo.Capture(ex);
          fileBufferedStream.m_eof = true;
          Monitor.Pulse(fileBufferedStream.m_lock);
        }
      }
    }

    public override bool CanWrite => false;

    public override void Flush() => throw new NotImplementedException();

    public override long Length
    {
      get
      {
        if (this.m_eof)
          return this.m_bytes;
        throw new NotSupportedException();
      }
    }

    public override void SetLength(long value) => throw new NotImplementedException();

    public override void Write(byte[] buffer, int offset, int count) => throw new NotImplementedException();
  }
}
