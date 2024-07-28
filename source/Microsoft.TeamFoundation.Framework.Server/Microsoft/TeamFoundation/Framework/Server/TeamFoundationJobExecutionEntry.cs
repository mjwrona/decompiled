// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationJobExecutionEntry
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [XmlInclude(typeof (TeamFoundationJobHistoryEntry))]
  [XmlInclude(typeof (TeamFoundationJobQueueEntry))]
  public abstract class TeamFoundationJobExecutionEntry
  {
    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public Guid JobSource { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public Guid JobId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public DateTime QueueTime { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public DateTime ExecutionStartTime { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public Guid RequesterActivityId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public Guid RequesterVsid { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public Guid AgentId { get; set; }

    [XmlIgnore]
    public TeamFoundationJobQueuedReasons QueuedReasons
    {
      get => (TeamFoundationJobQueuedReasons) this.QueuedReasonsValue;
      set => this.QueuedReasonsValue = (int) value;
    }

    [XmlAttribute("qr")]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, PropertyName = "QueuedReasons")]
    public int QueuedReasonsValue { get; set; }

    [XmlIgnore]
    internal TeamFoundationJobQueueFlags QueueFlags
    {
      get => (TeamFoundationJobQueueFlags) this.QueueFlagsValue;
      set => this.QueueFlagsValue = (int) value;
    }

    internal int QueueFlagsValue { get; set; }
  }
}
