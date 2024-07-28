// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Bundling.BundleStream
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Bundling
{
  internal class BundleStream : Stream
  {
    private IEnumerable<IBundleStreamProvider> m_streamProviders;
    private int m_streamIndex;
    private long m_offset;
    private readonly long m_length;

    public BundleStream(IEnumerable<IBundleStreamProvider> streamProviders)
    {
      ArgumentUtility.CheckForNull<IEnumerable<IBundleStreamProvider>>(streamProviders, nameof (streamProviders));
      this.m_streamProviders = streamProviders;
      this.m_length = this.m_streamProviders.Sum<IBundleStreamProvider>((Func<IBundleStreamProvider, long>) (sp => sp.Length));
    }

    public override bool CanRead => true;

    public override bool CanSeek => true;

    public override bool CanWrite => false;

    public override long Length => this.m_length;

    public override long Position
    {
      get => this.m_offset;
      set => this.m_offset = value;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      int num1 = 0;
      while (this.m_streamIndex < this.m_streamProviders.Count<IBundleStreamProvider>() && count > 0)
      {
        using (Stream stream = this.m_streamProviders.ElementAt<IBundleStreamProvider>(this.m_streamIndex).GetStream())
        {
          stream.Seek(this.m_offset, SeekOrigin.Begin);
          int num2 = stream.Read(buffer, offset, count);
          count -= num2;
          offset += num2;
          num1 += num2;
          if (stream.Position < stream.Length)
          {
            this.m_offset = stream.Position;
          }
          else
          {
            ++this.m_streamIndex;
            this.m_offset = 0L;
          }
        }
      }
      return num1;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      this.m_offset = 0L;
      this.m_streamIndex = 0;
      return 0;
    }

    public override void Flush() => throw new NotImplementedException();

    public override void SetLength(long value) => throw new NotImplementedException();

    public override void Write(byte[] buffer, int offset, int count) => throw new NotImplementedException();
  }
}
