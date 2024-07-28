// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Client.Internal.NuspecPackageContentFile
// Assembly: Microsoft.VisualStudio.Services.NuGet.Client.Internal, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E63C245C-898F-41A7-9916-45B2DC75C1BE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Client.Internal.dll

using System.IO;
using System.Linq;
using System.Xml.Linq;


#nullable enable
namespace Microsoft.VisualStudio.Services.NuGet.Client.Internal
{
  public class NuspecPackageContentFile : IPackageContentFile
  {
    public NuspecPackageContentFile(
      string? id,
      string? version,
      string? description,
      int extraSize,
      string? iconFilePath,
      string? licenseFilePath)
    {
      this.Id = id;
      this.Version = version;
      this.Description = description;
      this.ExtraSize = extraSize;
      this.IconFilePath = iconFilePath;
      this.LicenseFilePath = licenseFilePath;
    }

    public string Path => (this.Id ?? "package") + ".nuspec";

    public long PackagedLengthHint => 500;

    public string? Id { get; }

    public string? Version { get; }

    public string? Description { get; }

    public int ExtraSize { get; }

    public string? IconFilePath { get; }

    public string? LicenseFilePath { get; }

    public void WriteTo(Stream stream) => this.GenerateNuspec().Save(stream);

    private XDocument GenerateNuspec()
    {
      XElement content = new XElement((XName) "metadata", (object) new XElement((XName) "authors", (object) "nobody"));
      if (this.Id != null)
        content.Add((object) new XElement((XName) "id", (object) this.Id));
      if (this.Version != null)
        content.Add((object) new XElement((XName) "version", (object) this.Version));
      if (this.Description != null)
        content.Add((object) new XElement((XName) "description", (object) this.Description));
      if (this.IconFilePath != null)
        content.Add((object) new XElement((XName) "icon", (object) this.IconFilePath));
      if (this.LicenseFilePath != null)
        content.Add((object) new XElement((XName) "license", new object[2]
        {
          (object) new XAttribute((XName) "type", (object) "file"),
          (object) this.LicenseFilePath
        }));
      if (this.ExtraSize > 0)
        content.Add((object) new XElement((XName) "extra"), (object) new string(Enumerable.Repeat<char>('*', this.ExtraSize).ToArray<char>()));
      return new XDocument(new object[1]
      {
        (object) new XElement((XName) "package", (object) content)
      });
    }
  }
}
