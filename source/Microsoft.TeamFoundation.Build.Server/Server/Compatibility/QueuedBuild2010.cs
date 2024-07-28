// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.QueuedBuild2010
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  [ClassVisibility(ClientVisibility.Internal)]
  [RequiredClientService("BuildServer")]
  [XmlType("QueuedBuild")]
  public sealed class QueuedBuild2010
  {
    public QueuedBuild2010()
    {
      this.GetOption = GetOption2010.LatestOnBuild;
      this.Reason = BuildReason2010.Manual;
      this.Priority = QueuePriority2010.Normal;
    }

    [XmlAttribute]
    [ClientType(typeof (Uri))]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public string BuildControllerUri { get; set; }

    [XmlAttribute]
    [ClientType(typeof (Uri))]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public string BuildDefinitionUri { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public string CustomGetVersion { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public string DropLocation { get; set; }

    [XmlAttribute]
    [DefaultValue(GetOption2010.LatestOnBuild)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public GetOption2010 GetOption { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public int Id { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public QueuePriority2010 Priority { get; set; }

    [XmlElement]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public string ProcessParameters { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public int QueuePosition { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public DateTime QueueTime { get; set; }

    [XmlAttribute]
    [DefaultValue(BuildReason2010.Manual)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public BuildReason2010 Reason { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public string RequestedBy { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public string RequestedFor { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public string ShelvesetName { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public QueueStatus2010 Status { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public string TeamProject { get; set; }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public BuildDetail2010 Build { get; set; }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[QueuedBuild2010 Id={0} BuildDefinitionUri={1} BuildControllerUri={2}]", (object) this.Id, (object) this.BuildDefinitionUri, (object) this.BuildControllerUri);
  }
}
