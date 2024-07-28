// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.StreamParser
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System.ComponentModel;
using System.IO;

namespace Microsoft.VisualStudio.Services.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class StreamParser
  {
    private readonly Stream m_stream;
    private readonly int m_chunkSize;

    public StreamParser(Stream fileStream, int chunkSize)
    {
      this.m_stream = fileStream;
      this.m_chunkSize = chunkSize;
    }

    public long Length => this.m_stream.Length;

    public SubStream GetNextStream() => new SubStream(this.m_stream, this.m_chunkSize);
  }
}
