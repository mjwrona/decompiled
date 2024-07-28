// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.OnPrem.ConcatenatedOnDemandStream
// Assembly: Microsoft.VisualStudio.Services.BlobStore.OnPrem, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EA52CF3A-8E8F-49A1-8A12-783B16F9478A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.OnPrem.dll

using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.VisualStudio.Services.BlobStore.OnPrem
{
  internal class ConcatenatedOnDemandStream : Stream
  {
    private IList<StreamFactory> m_streamFactories;
    private IDictionary<int, Stream> m_createdStreams;
    private Stream m_currentStream;
    private bool m_isCached;
    private long m_readTotal;
    private long m_totalLength;
    private int m_index;
    private bool m_fullReadOnceOnly;
    private bool m_disableReentrant;
    private object m_syncLock = new object();

    public ConcatenatedOnDemandStream(
      IList<StreamFactory> streamFactories,
      bool fullReadOnceOnly,
      long totalLength)
    {
      this.m_streamFactories = streamFactories;
      this.m_totalLength = totalLength;
      this.m_fullReadOnceOnly = fullReadOnceOnly;
      this.m_index = -1;
    }

    public override bool CanRead => true;

    public override bool CanWrite => false;

    public override bool CanSeek => false;

    public override long Length => this.m_totalLength;

    public override long Position
    {
      get => this.m_readTotal;
      set
      {
        if (value != 0L)
          throw new NotSupportedException("Setting position other than 0 is not supported.");
        lock (this.m_syncLock)
        {
          if (this.m_disableReentrant)
            throw new NotSupportedException("The substreams are already disposed. Cannot set position to 0 at this moment.");
          this.m_index = -1;
          this.m_readTotal = 0L;
          if (this.m_currentStream != null)
          {
            if (!this.m_isCached)
            {
              try
              {
                this.m_currentStream.Dispose();
              }
              catch
              {
              }
            }
          }
          this.m_currentStream = (Stream) null;
        }
      }
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      if (offset < 0 || offset >= buffer.Length)
        throw new ArgumentException("offset is out of range.");
      if (count < 0)
        throw new ArgumentException("count is less than zero.");
      if (buffer == null || buffer.Length == 0)
        throw new ArgumentException("buffer has no space.");
      if (count == 0)
        return 0;
      lock (this.m_syncLock)
      {
        int num1 = 0;
        while (true)
        {
          if (this.m_currentStream == null || this.m_currentStream.Position >= this.m_currentStream.Length)
          {
            ++this.m_index;
            if (this.m_index < this.m_streamFactories.Count)
            {
              StreamFactory streamFactory = this.m_streamFactories[this.m_index];
              this.m_currentStream = streamFactory.Creator();
              this.m_isCached = streamFactory.IsCached;
              if (this.m_currentStream != null)
              {
                this.m_currentStream.Position = 0L;
                if (this.m_isCached)
                {
                  if (this.m_createdStreams == null)
                    this.m_createdStreams = (IDictionary<int, Stream>) new Dictionary<int, Stream>();
                  this.m_createdStreams[this.m_index] = this.m_currentStream;
                }
              }
              else
                goto label_15;
            }
            else
              break;
          }
          bool flag = false;
          int num2;
          while ((num2 = this.m_currentStream.Read(buffer, offset, count)) != 0)
          {
            num1 += num2;
            this.m_readTotal += (long) num2;
            if (num2 < count)
            {
              offset += num2;
              count -= num2;
            }
            else if (num2 == count)
            {
              flag = true;
              break;
            }
          }
          if (!flag)
          {
            try
            {
              if (!this.m_isCached)
                this.m_currentStream.Dispose();
            }
            catch
            {
            }
            finally
            {
              this.m_currentStream = (Stream) null;
            }
          }
          else
            goto label_31;
        }
        if (this.m_fullReadOnceOnly)
        {
          this.DisposeCachedStreams();
          goto label_31;
        }
        else
          goto label_31;
label_15:
        throw new ArgumentNullException("A stream factory returns null stream.");
label_31:
        return num1;
      }
    }

    public override void SetLength(long value) => throw new NotSupportedException("Setting length is not supported.");

    public override void Flush() => throw new NotSupportedException("Flushing is not supported.");

    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException("Seeking is not supported.");

    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException("Writing is not supported.");

    protected override void Dispose(bool disposing) => this.DisposeCachedStreams();

    private void DisposeCachedStreams()
    {
      if (this.m_createdStreams == null)
        return;
      foreach (Stream stream in (IEnumerable<Stream>) this.m_createdStreams.Values)
      {
        try
        {
          stream.Dispose();
        }
        catch
        {
        }
      }
      this.m_createdStreams = (IDictionary<int, Stream>) null;
      this.m_disableReentrant = true;
    }
  }
}
