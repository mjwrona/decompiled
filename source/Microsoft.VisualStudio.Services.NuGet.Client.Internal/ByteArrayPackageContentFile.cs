// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Client.Internal.ByteArrayPackageContentFile
// Assembly: Microsoft.VisualStudio.Services.NuGet.Client.Internal, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E63C245C-898F-41A7-9916-45B2DC75C1BE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Client.Internal.dll

using System.IO;


#nullable enable
namespace Microsoft.VisualStudio.Services.NuGet.Client.Internal
{
  public class ByteArrayPackageContentFile : IPackageContentFile
  {
    public ByteArrayPackageContentFile(string path, byte[] bytes)
    {
      this.Bytes = bytes;
      this.Path = path;
    }

    public byte[] Bytes { get; }

    public string Path { get; }

    public long PackagedLengthHint => (long) this.Bytes.Length;

    public void WriteTo(Stream stream) => stream.Write(this.Bytes, 0, this.Bytes.Length);
  }
}
