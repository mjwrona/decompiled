// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Client.Internal.OpcContentTypesPackageContentFile
// Assembly: Microsoft.VisualStudio.Services.NuGet.Client.Internal, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E63C245C-898F-41A7-9916-45B2DC75C1BE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Client.Internal.dll

using System.IO;
using System.Xml.Linq;

namespace Microsoft.VisualStudio.Services.NuGet.Client.Internal
{
  public class OpcContentTypesPackageContentFile : IPackageContentFile
  {
    public string Path => "[Content_Types].xml";

    public long PackagedLengthHint => 500;

    public void WriteTo(Stream stream)
    {
      XNamespace xnamespace = XNamespace.Get("http://schemas.openxmlformats.org/package/2006/content-types");
      new XDocument(new object[1]
      {
        (object) new XElement(xnamespace.GetName("Types"), new object[2]
        {
          (object) new XElement(xnamespace.GetName("Default"), new object[2]
          {
            (object) new XAttribute((XName) "Extension", (object) "rels"),
            (object) new XAttribute((XName) "ContentType", (object) "application/vnd.openxmlformats-package.relationships+xml")
          }),
          (object) new XElement(xnamespace.GetName("Default"), new object[2]
          {
            (object) new XAttribute((XName) "Extension", (object) "nuspec"),
            (object) new XAttribute((XName) "ContentType", (object) "application/octet")
          })
        })
      }).Save(stream);
    }
  }
}
