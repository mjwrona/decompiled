// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.PublishCoveragePRStatusJobData
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [XmlRoot("PublishCoverageCheckJobInput")]
  public class PublishCoveragePRStatusJobData
  {
    public PipelineContext PipelineContext { get; set; }

    [XmlAttribute("CoverageStatusCheckState")]
    public CoverageStatusCheckState CoverageStatusCheckState { get; set; }

    [XmlAttribute("ProjectId")]
    public string ProjectId { get; set; }

    [XmlAttribute("BuildId")]
    public int BuildId { get; set; }

    [XmlAttribute("BuildUri")]
    public string BuildUri { get; set; }
  }
}
