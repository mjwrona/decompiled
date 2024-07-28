// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.MergeJobData
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [XmlRoot("MergeJobInput")]
  public class MergeJobData : CoverageJobDataInputBase
  {
    [XmlIgnore]
    private Microsoft.TeamFoundation.TestManagement.Server.JobInvoker _jobInvoker;

    [XmlAttribute("QueueTime")]
    public DateTime QueueTime { get; set; }

    public PipelineContext PipelineContext { get; set; }

    public List<CoverageToolInput> CoverageData { get; set; }

    public string AttachmentStoreTypeName { get; set; }

    [XmlArray("FilesChanged")]
    [XmlArrayItem("FileChanged", typeof (string))]
    public List<string> FilesChanged { get; set; }

    [XmlAttribute("JobInvoker")]
    public int JobInvoker
    {
      get => (int) this._jobInvoker;
      set => this._jobInvoker = Enum.IsDefined(typeof (Microsoft.TeamFoundation.TestManagement.Server.JobInvoker), (object) value) ? (Microsoft.TeamFoundation.TestManagement.Server.JobInvoker) value : Microsoft.TeamFoundation.TestManagement.Server.JobInvoker.Unknown;
    }
  }
}
