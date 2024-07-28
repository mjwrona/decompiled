// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.IdentityMappingCollection
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace Microsoft.TeamFoundation.Server.Core
{
  [CollectionDataContract(Name = "IdentityMappings", ItemName = "Mapping")]
  public class IdentityMappingCollection : List<IdentityMappingEntry>
  {
    public IdentityMappingCollection()
    {
    }

    public IdentityMappingCollection(IList<IdentityMappingEntry> source)
      : base((IEnumerable<IdentityMappingEntry>) source)
    {
    }

    public static string ToXmlString(IdentityMappingCollection mappings)
    {
      using (StringWriter w = new StringWriter())
      {
        using (XmlTextWriter writer = new XmlTextWriter((TextWriter) w))
        {
          new DataContractSerializer(mappings.GetType()).WriteObject((XmlWriter) writer, (object) mappings);
          return w.ToString();
        }
      }
    }

    public static IdentityMappingCollection FromString(string xmlString)
    {
      DataContractSerializer contractSerializer = new DataContractSerializer(typeof (IdentityMappingCollection));
      XmlReaderSettings settings = new XmlReaderSettings()
      {
        DtdProcessing = DtdProcessing.Prohibit,
        XmlResolver = (XmlResolver) null
      };
      using (StringReader input = new StringReader(xmlString))
      {
        using (XmlReader reader = XmlReader.Create((TextReader) input, settings))
          return contractSerializer.ReadObject(reader) as IdentityMappingCollection;
      }
    }
  }
}
