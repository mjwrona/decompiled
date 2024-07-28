// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Client.Internal.PackageBuilderSlim
// Assembly: Microsoft.VisualStudio.Services.NuGet.Client.Internal, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E63C245C-898F-41A7-9916-45B2DC75C1BE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Client.Internal.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace Microsoft.VisualStudio.Services.NuGet.Client.Internal
{
  public class PackageBuilderSlim
  {
    public string Id { get; set; }

    public string Version { get; set; }

    public string Description { get; set; }

    public bool GarbageNuspec { get; set; }

    public bool NoNuspec { get; set; }

    public int ExtraNuspecSize { get; set; }

    public string AssemblyFrameworkVersion { get; set; } = "net45";

    public long PackageContentFileSize { get; set; }

    public string LicenseFilePath { get; set; }

    public string IconFilePath { get; set; }

    public List<IPackageContentFile> ExtraFiles { get; } = new List<IPackageContentFile>();

    public Stream GetStream()
    {
      Stream stream = this.PackageContentFileSize <= 50000000L ? (Stream) new MemoryStream() : (Stream) File.Create(Path.GetTempFileName(), 4096, FileOptions.DeleteOnClose);
      this.BuildCore(stream);
      stream.Position = 0L;
      return stream;
    }

    public void SaveTo(Stream stream) => this.BuildCore((Stream) new PackageBuilderSlim.WriteOnlyStreamAdaptor(stream));

    public override string ToString() => string.Format("{0} v{1}", (object) (this.Id ?? "(no name)"), (object) (this.Version ?? "(no version)"));

    public void BuildCore(Stream stream)
    {
      DateTimeOffset dateTimeOffset = new DateTimeOffset(2015, 12, 14, 5, 5, 5, TimeSpan.FromMinutes(1.0));
      List<IPackageContentFile> packageContentFileList = this.BuildFiles();
      using (ZipArchive zipArchive = new ZipArchive(stream, ZipArchiveMode.Create, true))
      {
        foreach (IPackageContentFile packageContentFile in packageContentFileList)
        {
          ZipArchiveEntry entry = zipArchive.CreateEntry(packageContentFile.Path, CompressionLevel.NoCompression);
          entry.LastWriteTime = dateTimeOffset;
          using (Stream stream1 = entry.Open())
            packageContentFile.WriteTo(stream1);
        }
      }
    }

    public List<IPackageContentFile> BuildFiles()
    {
      List<IPackageContentFile> packageContentFileList = new List<IPackageContentFile>();
      if (!this.NoNuspec)
      {
        IPackageContentFile packageContentFile = !this.GarbageNuspec ? (IPackageContentFile) new NuspecPackageContentFile(this.Id, this.Version, this.Description, this.ExtraNuspecSize, this.IconFilePath, this.LicenseFilePath) : (IPackageContentFile) new IncompressiblePackageContentFile("package.nuspec", 500L, packageContentFileList.Count);
        packageContentFileList.Add(packageContentFile);
        packageContentFileList.Add((IPackageContentFile) new OpcRelsPackageContentFile(new Dictionary<string, string>()
        {
          {
            "http://schemas.microsoft.com/packaging/2010/07/manifest",
            "/" + packageContentFile.Path
          }
        }));
        packageContentFileList.Add((IPackageContentFile) new OpcContentTypesPackageContentFile());
        packageContentFileList.AddRange((IEnumerable<IPackageContentFile>) this.ExtraFiles);
      }
      if (this.PackageContentFileSize > 0L)
        packageContentFileList.Add((IPackageContentFile) new IncompressiblePackageContentFile("lib\\" + this.AssemblyFrameworkVersion + "\\file.dll", this.PackageContentFileSize, packageContentFileList.Count));
      return packageContentFileList;
    }

    private class WriteOnlyStreamAdaptor : Stream
    {
      private readonly Stream stream;
      private long position;

      public WriteOnlyStreamAdaptor(Stream stream)
      {
        this.stream = stream;
        this.position = 0L;
      }

      public override bool CanRead => false;

      public override bool CanSeek => false;

      public override bool CanWrite => true;

      public override long Length => throw new NotSupportedException();

      public override long Position
      {
        get => this.position;
        set => throw new NotSupportedException();
      }

      public override void Flush() => this.stream.Flush();

      public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

      public override void SetLength(long value) => throw new NotSupportedException();

      public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();

      public override void Write(byte[] buffer, int offset, int count)
      {
        this.stream.Write(buffer, offset, count);
        this.position += (long) count;
      }
    }
  }
}
