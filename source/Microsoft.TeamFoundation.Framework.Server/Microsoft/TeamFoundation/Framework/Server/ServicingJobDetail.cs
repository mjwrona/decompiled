// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingJobDetail
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class ServicingJobDetail
  {
    [XmlAttribute("hid")]
    public Guid HostId { get; set; }

    [XmlAttribute("jid")]
    public Guid JobId { get; set; }

    [XmlAttribute("oc")]
    public string OperationClass { get; set; }

    [XmlAttribute("o")]
    public string OperationString { get; set; }

    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private)]
    public string[] Operations { get; set; }

    [XmlAttribute("js")]
    public int JobStatusValue { get; set; }

    [XmlIgnore]
    public ServicingJobStatus JobStatus
    {
      get => (ServicingJobStatus) this.JobStatusValue;
      set => this.JobStatusValue = (int) value;
    }

    [XmlAttribute("qp")]
    public int QueuePosition { get; set; }

    [XmlAttribute("r")]
    public int ResultValue { get; set; }

    [XmlIgnore]
    public ServicingJobResult Result
    {
      get => (ServicingJobResult) this.ResultValue;
      set => this.ResultValue = (int) value;
    }

    [XmlAttribute("qt")]
    public DateTime QueueTime { get; set; }

    [XmlAttribute("st")]
    public DateTime StartTime { get; set; }

    [XmlAttribute("et")]
    public DateTime EndTime { get; set; }

    [XmlAttribute("tsc")]
    public short TotalStepCount { get; set; }

    [XmlAttribute("csc")]
    public short CompletedStepCount { get; set; }
  }
}
