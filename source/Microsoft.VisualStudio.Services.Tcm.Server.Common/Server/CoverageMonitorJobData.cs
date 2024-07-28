// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.CoverageMonitorJobData
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [XmlRoot("CoverageInput")]
  public class CoverageMonitorJobData : CoverageJobDataInputBase
  {
    [XmlIgnore]
    private Microsoft.TeamFoundation.TestManagement.Server.JobInvoker _jobInvoker;

    public CoverageMonitorJobData()
    {
      this.ModuleCoverageMergeJobIds = new HashSet<Guid>();
      this.Configurations = new HashSet<Configuration>();
    }

    public PipelineContext PipelineContext { get; set; }

    [XmlArray("ModuleCoverageMergeJobIds")]
    [XmlArrayItem("ModuleCoverageMergeJobId", typeof (Guid))]
    public HashSet<Guid> ModuleCoverageMergeJobIds { get; }

    [XmlArray("Configurations")]
    [XmlArrayItem("Configuration", typeof (Configuration))]
    public HashSet<Configuration> Configurations { get; }

    [XmlAttribute("QueueTime")]
    public DateTime QueueTime { get; set; }

    [XmlAttribute("JobInvoker")]
    public int JobInvoker
    {
      get => (int) this._jobInvoker;
      set => this._jobInvoker = Enum.IsDefined(typeof (Microsoft.TeamFoundation.TestManagement.Server.JobInvoker), (object) value) ? (Microsoft.TeamFoundation.TestManagement.Server.JobInvoker) value : Microsoft.TeamFoundation.TestManagement.Server.JobInvoker.Unknown;
    }

    [XmlAttribute("BuildCompletitionTime")]
    public DateTime BuildCompletitionTime { get; set; }
  }
}
