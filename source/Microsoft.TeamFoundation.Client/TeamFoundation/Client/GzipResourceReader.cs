// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.GzipResourceReader
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.Collections;
using System.IO;
using System.IO.Compression;
using System.Resources;

namespace Microsoft.TeamFoundation.Client
{
  public class GzipResourceReader : IResourceReader, IEnumerable, IDisposable
  {
    private ResourceReader m_resourceReader;

    public GzipResourceReader(Stream stream)
    {
      BinaryReader binaryReader = new BinaryReader(stream);
      binaryReader.ReadInt32();
      binaryReader.ReadInt32();
      int offset1 = binaryReader.ReadInt32();
      stream.Seek((long) offset1, SeekOrigin.Current);
      byte[] buffer = new byte[binaryReader.ReadInt32()];
      MemoryStream memoryStream = new MemoryStream(buffer, 0, buffer.Length, true, true);
      using (DeflateStream deflateStream = new DeflateStream(stream, CompressionMode.Decompress))
      {
        int offset2 = 0;
        while (offset2 < buffer.Length)
          offset2 += deflateStream.Read(buffer, offset2, buffer.Length - offset2);
      }
      this.m_resourceReader = new ResourceReader((Stream) memoryStream);
    }

    void IResourceReader.Close() => this.m_resourceReader.Close();

    IDictionaryEnumerator IResourceReader.GetEnumerator() => this.m_resourceReader.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) this.m_resourceReader).GetEnumerator();

    void IDisposable.Dispose() => this.m_resourceReader.Dispose();
  }
}
