// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.BuildInformationNode
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server
{
  [CallOnDeserialization("AfterDeserialize")]
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Build")]
  public sealed class BuildInformationNode : ICacheable
  {
    private List<InformationField> m_fields = new List<InformationField>();

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public int NodeId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public int ParentId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public)]
    public string Type { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public DateTime LastModifiedDate { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string LastModifiedBy { get; set; }

    [ClientProperty(ClientVisibility.Internal, ClientVisibility.Private, PropertyName = "InternalFields")]
    public List<InformationField> Fields => this.m_fields;

    int ICacheable.GetCachedSize() => 80 + this.m_fields.Count * 128;

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[BuildInformationNode NodeId={0} ParentId={1} Type={2} Fields={3}]", (object) this.NodeId, (object) this.ParentId, (object) this.Type, (object) this.Fields.ListItems<InformationField>());

    internal int GroupId { get; set; }

    internal string BuildUri { get; set; }
  }
}
