// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Client.Internal.IncompressiblePackageContentFile
// Assembly: Microsoft.VisualStudio.Services.NuGet.Client.Internal, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E63C245C-898F-41A7-9916-45B2DC75C1BE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Client.Internal.dll

using System;
using System.IO;

namespace Microsoft.VisualStudio.Services.NuGet.Client.Internal
{
  public class IncompressiblePackageContentFile : IPackageContentFile
  {
    private readonly long length;
    private readonly int seed;

    public IncompressiblePackageContentFile(string path, long length, int seed)
    {
      this.Path = path;
      this.length = length;
      this.seed = seed;
    }

    public string Path { get; }

    public long PackagedLengthHint => this.length;

    public void WriteTo(Stream stream)
    {
      byte[] buffer = new byte[1024];
      int num = this.seed;
      int count;
      for (long length = this.length; length > 0L; length -= (long) count)
      {
        count = (int) Math.Min(1024L, length);
        for (int index = 0; index < count; ++index)
        {
          num = num * 2707 + 1;
          buffer[index] = (byte) num;
        }
        stream.Write(buffer, 0, count);
      }
    }
  }
}
