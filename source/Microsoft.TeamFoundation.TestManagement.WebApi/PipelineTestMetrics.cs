// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.PipelineTestMetrics
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public class PipelineTestMetrics : TestManagementBaseSecuredObject
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public PipelineReference CurrentContext { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public ResultSummary ResultSummary { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public ResultsAnalysis ResultsAnalysis { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public RunSummary RunSummary { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public List<PipelineTestMetrics> SummaryAtChild { get; set; }

    internal override void InitializeSecureObject(ISecuredObject securedObject)
    {
      base.InitializeSecureObject(securedObject);
      this.CurrentContext?.InitializeSecureObject(securedObject);
      this.ResultSummary?.InitializeSecureObject(securedObject);
      this.ResultsAnalysis?.InitializeSecureObject(securedObject);
      this.RunSummary?.InitializeSecureObject(securedObject);
      if (this.SummaryAtChild == null)
        return;
      foreach (TestManagementBaseSecuredObject baseSecuredObject in this.SummaryAtChild)
        baseSecuredObject.InitializeSecureObject(securedObject);
    }
  }
}
