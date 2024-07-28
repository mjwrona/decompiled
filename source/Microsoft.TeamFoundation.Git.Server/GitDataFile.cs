// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitDataFile
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Threading;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal sealed class GitDataFile : IDisposable
  {
    private readonly ConcurrentObjectPool<Stream> m_pool;
    private readonly Lazy<MemoryMappedViewAccessor> m_memMapView;
    private bool m_isDisposed;

    public GitDataFile(MemoryMappedFile file, long length, string path)
    {
      this.File = file;
      this.Length = length;
      this.Path = path;
      this.m_pool = new ConcurrentObjectPool<Stream>((Func<Stream>) (() => (Stream) new FileStream(this.Path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete)), TimeSpan.FromSeconds(2.0));
      this.m_memMapView = new Lazy<MemoryMappedViewAccessor>((Func<MemoryMappedViewAccessor>) (() => this.File.CreateViewAccessor(0L, this.Length, MemoryMappedFileAccess.Read)), LazyThreadSafetyMode.ExecutionAndPublication);
    }

    public void Dispose()
    {
      if (this.m_isDisposed)
        return;
      if (this.m_memMapView.IsValueCreated)
        this.m_memMapView.Value.Dispose();
      this.m_pool.Dispose();
      this.File.Dispose();
      this.m_isDisposed = true;
    }

    private void ThrowIfDisposed()
    {
      if (this.m_isDisposed)
        throw new ObjectDisposedException(this.GetType().FullName);
    }

    public SafeBufferStream CreateMemoryMappedStream(long offset = 0, long length = -1)
    {
      this.ThrowIfDisposed();
      this.SanitizeLength(offset, ref length);
      return new SafeBufferStream((SafeBuffer) this.m_memMapView.Value.SafeMemoryMappedViewHandle, this.m_memMapView.Value.PointerOffset + offset, length, true);
    }

    public GitRestrictedStream CreateStream(long offset = 0, long length = -1)
    {
      this.ThrowIfDisposed();
      this.SanitizeLength(offset, ref length);
      if (length <= 4096L)
      {
        byte[] numArray = new byte[length];
        Stream stream1 = this.m_pool.Take();
        try
        {
          stream1.Position = offset;
          GitStreamUtil.ReadGreedy(stream1, numArray, 0, (int) length);
          Stream stream2 = (Stream) new MemoryStream(numArray, false);
          try
          {
            return new GitRestrictedStream(stream2, 0L, length, false);
          }
          catch
          {
            stream2.Dispose();
            throw;
          }
        }
        finally
        {
          this.m_pool.Return(stream1);
        }
      }
      else
      {
        Stream fullStream = this.m_pool.Take();
        try
        {
          GitRestrictedStream stream = new GitRestrictedStream(fullStream, offset, length, true);
          stream.PushActionOnDispose((Action) (() => this.m_pool.Return(fullStream)));
          return stream;
        }
        catch
        {
          this.m_pool.Return(fullStream);
          throw;
        }
      }
    }

    private void SanitizeLength(long offset, ref long length) => length = length < 0L ? this.Length - offset : length;

    public MemoryMappedFile File { get; }

    public long Length { get; }

    public string Path { get; }
  }
}
