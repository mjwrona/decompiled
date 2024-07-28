// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.OdbJobKey
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class OdbJobKey : IGitJobKey, IXmlSerializable
  {
    private OdbJobKey()
    {
    }

    public OdbJobKey(OdbId odbId) => this.OdbId = odbId;

    public override string ToString() => "ODB: " + this.OdbId.ToString();

    public XmlSchema GetSchema() => (XmlSchema) null;

    public void ReadXml(XmlReader reader)
    {
      this.OdbId = OdbId.Parse(reader.GetAttribute("OdbId"));
      this.OdbId.CheckValid();
    }

    public void WriteXml(XmlWriter writer) => writer.WriteAttributeString("OdbId", this.OdbId.ToString());

    public OdbId OdbId { get; private set; }
  }
}
