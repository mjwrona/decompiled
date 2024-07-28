// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Client.Internal.OpcRelsPackageContentFile
// Assembly: Microsoft.VisualStudio.Services.NuGet.Client.Internal, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E63C245C-898F-41A7-9916-45B2DC75C1BE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Client.Internal.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Microsoft.VisualStudio.Services.NuGet.Client.Internal
{
  public class OpcRelsPackageContentFile : IPackageContentFile
  {
    private readonly Dictionary<string, string> relationships;

    public OpcRelsPackageContentFile(Dictionary<string, string> relationships) => this.relationships = relationships;

    public string Path => "_rels/.rels";

    public long PackagedLengthHint => 1000;

    public void WriteTo(Stream stream)
    {
      XNamespace relsNs = XNamespace.Get("http://schemas.openxmlformats.org/package/2006/relationships");
      new XDocument(new object[1]
      {
        (object) new XElement(relsNs.GetName("Relationships"), (object) this.relationships.Select<KeyValuePair<string, string>, XElement>((Func<KeyValuePair<string, string>, int, XElement>) ((rel, idx) => new XElement(relsNs.GetName("Relationship"), new object[3]
        {
          (object) new XAttribute((XName) "Id", (object) string.Format("R{0}", (object) idx)),
          (object) new XAttribute((XName) "Type", (object) rel.Key),
          (object) new XAttribute((XName) "Target", (object) rel.Value)
        }))))
      }).Save(stream);
    }
  }
}
