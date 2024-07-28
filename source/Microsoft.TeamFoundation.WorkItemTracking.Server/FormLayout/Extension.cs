// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Extension
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Common.Provision;
using System;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout
{
  [DataContract]
  public class Extension : IXmlSerializable
  {
    [DataMember(Name = "id")]
    public string Id { get; set; }

    public Extension Clone() => new Extension()
    {
      Id = this.Id
    };

    public XmlSchema GetSchema() => (XmlSchema) null;

    public void ReadXml(XmlReader reader) => throw new NotImplementedException();

    public void WriteXml(XmlWriter writer)
    {
      if (string.IsNullOrEmpty(this.Id))
        return;
      writer.WriteAttributeString(ProvisionAttributes.ExtensionId, this.Id);
    }
  }
}
