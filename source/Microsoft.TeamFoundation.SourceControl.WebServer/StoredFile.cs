// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.StoredFile
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using System;
using System.IO;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public class StoredFile : IDisposable
  {
    private bool m_disposed;
    private readonly Stream m_stream;
    private readonly byte[] m_hashValue;

    public StoredFile(Stream stream, byte[] hashValue)
    {
      this.m_stream = stream;
      this.m_hashValue = hashValue;
    }

    public Stream Stream => this.m_stream;

    public byte[] HashValue => this.m_hashValue;

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected void Dispose(bool disposing)
    {
      if (!disposing || this.m_disposed)
        return;
      if (this.m_stream != null)
        this.m_stream.Dispose();
      this.m_disposed = true;
    }
  }
}
