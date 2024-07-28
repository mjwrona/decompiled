// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.QueuedBuild2008
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  [ClassVisibility(ClientVisibility.Internal)]
  [RequiredClientService("BuildServer")]
  [XmlType("QueuedBuild")]
  public sealed class QueuedBuild2008
  {
    public QueuedBuild2008() => this.Priority = QueuePriority2010.Normal;

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public)]
    public int Id { get; set; }

    [XmlAttribute]
    [ClientType(typeof (Uri))]
    [ClientProperty(ClientVisibility.Public)]
    public string BuildAgentUri { get; set; }

    [XmlAttribute]
    [ClientType(typeof (Uri))]
    [ClientProperty(ClientVisibility.Public)]
    public string BuildDefinitionUri { get; set; }

    [ClientProperty(ClientVisibility.Public)]
    public string CommandLineArguments { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public)]
    public string DropLocation { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public)]
    public DateTime QueueTime { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public)]
    public int QueuePosition { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public)]
    public QueuePriority2010 Priority { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public)]
    public QueueStatus2010 Status { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public)]
    public GetOption2010 GetOption { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public)]
    public string CustomGetVersion { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public)]
    public string RequestedFor { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public)]
    public string RequestedBy { get; set; }

    [ClientProperty(ClientVisibility.Public)]
    public BuildDetail2010 Build { get; set; }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[QueuedBuild2008 Id={0} BuildDefinitionUri={1} BuildControllerUri={2}]", (object) this.Id, (object) this.BuildDefinitionUri, (object) this.BuildAgentUri);
  }
}
