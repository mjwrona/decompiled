// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Ssh.Server.External.Eldos.PipeSshCommandStreams
// Assembly: Microsoft.TeamFoundation.Ssh.Server.External.Eldos, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76A7154E-5D66-408C-AA1C-E130B17CCD4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Ssh.Server.External.Eldos.dll

using Microsoft.TeamFoundation.Ssh.Server.Core;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Buffers;
using System.IO;
using System.IO.Pipelines;
using System.Threading;

namespace Microsoft.TeamFoundation.Ssh.Server.External.Eldos
{
  public sealed class PipeSshCommandStreams : ISshCommandStreams
  {
    public PipeSshCommandStreams(IPipeSshChannel channel)
    {
      this.StdIn = (Stream) new PipeSshCommandStreams.PipeReaderStream(channel.DataIn);
      this.StdOut = (Stream) new PipeSshCommandStreams.PipeWriterStream(channel.DataOut);
      channel.ExtendedDataIn.Complete();
    }

    public Stream StdIn { get; private set; }

    public Stream StdOut { get; private set; }

    internal static Stream TEST_CreateReadStream(PipeReader src) => (Stream) new PipeSshCommandStreams.PipeReaderStream(src);

    internal static Stream TEST_CreateWriteStream(PipeWriter tgt) => (Stream) new PipeSshCommandStreams.PipeWriterStream(tgt);

    private class PipeReaderStream : PipeSshCommandStreams.PipeBaseStream
    {
      private readonly PipeReader m_reader;

      public PipeReaderStream(PipeReader src) => this.m_reader = src;

      public override bool CanRead => true;

      public override bool CanWrite => false;

      public override int Read(byte[] buffer, int offset, int count)
      {
        ArgumentUtility.CheckForNull<byte[]>(buffer, nameof (buffer));
        ArgumentUtility.CheckForOutOfRange(offset, nameof (offset), 0);
        ArgumentUtility.CheckForOutOfRange(count, nameof (count), 0);
        if (count > buffer.Length - offset)
          throw new ArgumentException("count > buffer.Length - offset");
        ReadResult result = this.m_reader.ReadAsync().AsTask().GetAwaiter().GetResult();
        if (result.IsCanceled)
          throw new System.OperationCanceledException("ReadResult");
        int length = Math.Min(count, checked ((int) result.Buffer.Length));
        ReadOnlySequence<byte> source = result.Buffer.Slice(0, length);
        source.CopyTo<byte>(buffer.AsSpan<byte>(offset));
        this.m_reader.AdvanceTo(source.End);
        return length;
      }

      public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }

    private class PipeWriterStream : PipeSshCommandStreams.PipeBaseStream
    {
      private PipeWriter m_writer;

      public PipeWriterStream(PipeWriter tgt) => this.m_writer = tgt;

      public override bool CanRead => false;

      public override bool CanWrite => true;

      public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();

      public override sealed void Write(byte[] buffer, int offset, int count)
      {
        ArgumentUtility.CheckForNull<byte[]>(buffer, nameof (buffer));
        ArgumentUtility.CheckForOutOfRange(offset, nameof (offset), 0);
        ArgumentUtility.CheckForOutOfRange(count, nameof (count), 0);
        if (count > buffer.Length - offset)
          throw new ArgumentException("count > buffer.Length - offset");
        if (this.m_writer.WriteAsync((ReadOnlyMemory<byte>) buffer.AsMemory<byte>(offset, count), new CancellationToken()).AsTask().GetAwaiter().GetResult().IsCanceled)
          throw new System.OperationCanceledException("FlushResult");
      }
    }

    private abstract class PipeBaseStream : Stream
    {
      public override bool CanSeek => throw new NotSupportedException();

      public override long Length => throw new NotSupportedException();

      public override long Position
      {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
      }

      public override void Flush()
      {
      }

      public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

      public override void SetLength(long value) => throw new NotSupportedException();
    }
  }
}
