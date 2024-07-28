// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.QueuedBuild
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server
{
  [CallOnDeserialization("AfterDeserialize")]
  [ClassVisibility(ClientVisibility.Internal)]
  [RequiredClientService("BuildServer")]
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Build")]
  public sealed class QueuedBuild : ICacheable
  {
    private List<string> m_buildUris = new List<string>();

    public QueuedBuild()
    {
      this.GetOption = GetOption.LatestOnBuild;
      this.Reason = BuildReason.Manual;
      this.Priority = QueuePriority.Normal;
    }

    [XmlAttribute]
    [DefaultValue(typeof (Guid), "00000000-0000-0000-0000-000000000000")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public Guid BatchId { get; set; }

    [XmlAttribute]
    [ClientType(typeof (Uri))]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Internal, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public string BuildControllerUri { get; set; }

    [XmlAttribute]
    [ClientType(typeof (Uri))]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Internal, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public string BuildDefinitionUri { get; set; }

    [ClientType(typeof (Uri[]))]
    [ClientProperty(ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public List<string> BuildUris => this.m_buildUris;

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public string CustomGetVersion { get; set; }

    [XmlIgnore]
    public BuildDefinition Definition { get; internal set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public string DropLocation { get; set; }

    [XmlAttribute]
    [DefaultValue(GetOption.LatestOnBuild)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public GetOption GetOption { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public int Id { get; set; }

    [XmlAttribute]
    [DefaultValue(QueuePriority.Normal)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public QueuePriority Priority { get; set; }

    [XmlElement]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public string ProcessParameters { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Internal, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public int QueuePosition { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public DateTime QueueTime { get; set; }

    [XmlAttribute]
    [DefaultValue(BuildReason.Manual)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Internal, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public BuildReason Reason { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public string RequestedBy { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public string RequestedByDisplayName { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public string RequestedFor { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public string RequestedForDisplayName { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public string ShelvesetName { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Internal, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public QueueStatus Status { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public string TeamProject { get; set; }

    [XmlIgnore]
    public int? BuildId { get; set; }

    [XmlIgnore]
    public Guid ProjectId { get; set; }

    [XmlIgnore]
    public TeamFoundationIdentity RequestedByIdentity { get; set; }

    [XmlIgnore]
    public TeamFoundationIdentity RequestedForIdentity { get; set; }

    internal bool IsRequestor(TeamFoundationIdentity requestor)
    {
      if (this.RequestedByIdentity != null && requestor.TeamFoundationId == this.RequestedByIdentity.TeamFoundationId)
        return true;
      return this.RequestedForIdentity != null && requestor.TeamFoundationId == this.RequestedForIdentity.TeamFoundationId;
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[QueuedBuild Id={0} BuildDefinitionUri={1} BuildControllerUri={2} BatchId={3} BuildUris={4}]", (object) this.Id, (object) this.BuildDefinitionUri, (object) this.BuildControllerUri, (object) this.BatchId, (object) this.BuildUris.ListItems<string>());

    int ICacheable.GetCachedSize() => 800;
  }
}
