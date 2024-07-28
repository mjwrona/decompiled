// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingStepDetail
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [XmlInclude(typeof (ServicingStepStateChange))]
  [XmlInclude(typeof (ServicingStepLogEntry))]
  public abstract class ServicingStepDetail
  {
    protected ServicingStepDetail() => this.DetailTime = DateTime.UtcNow;

    [XmlAttribute("qtime")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public DateTime QueueTime { get; set; }

    [XmlAttribute("did")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public long DetailId { get; set; }

    [XmlAttribute("dtime")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public DateTime DetailTime { get; set; }

    [XmlAttribute("sop")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string ServicingOperation { get; set; }

    [XmlAttribute("gid")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string ServicingStepGroupId { get; set; }

    [XmlAttribute("sid")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string ServicingStepId { get; set; }

    public abstract string ToLogEntryLine();
  }
}
