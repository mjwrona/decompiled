// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Upstreams.MavenXmlMetadataToVersionListConverter
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Maven.Server.Models.Xml;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.Maven.Server.Upstreams
{
  public class MavenXmlMetadataToVersionListConverter : 
    IConverter<Stream, IList<string>>,
    IHaveInputType<Stream>,
    IHaveOutputType<IList<string>>
  {
    public IList<string> Convert(Stream stream)
    {
      XmlReaderSettings settings = new XmlReaderSettings()
      {
        IgnoreComments = true,
        DtdProcessing = DtdProcessing.Prohibit,
        XmlResolver = (XmlResolver) null
      };
      using (XmlReader xmlReader = XmlReader.Create((XmlReader) new XmlTextReader(stream)
      {
        Namespaces = false
      }, settings))
        return (IList<string>) ((MavenXmlMetadataBase<MavenArtifactMetadataVersioning>) new XmlSerializer(typeof (MavenArtifactMetadata)).Deserialize(xmlReader)).Versioning.Versions;
    }
  }
}
