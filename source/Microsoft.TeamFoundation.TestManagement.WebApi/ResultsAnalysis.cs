// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.ResultsAnalysis
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public class ResultsAnalysis : TestManagementBaseSecuredObject
  {
    public ResultsAnalysis()
    {
    }

    public ResultsAnalysis(ResultsAnalysis ra)
    {
      if (ra == null)
        return;
      if (ra.PreviousContext != null)
      {
        this.PreviousContext = new PipelineReference()
        {
          PipelineId = ra.PreviousContext.PipelineId
        };
        if (ra.PreviousContext.StageReference != null)
          this.PreviousContext.StageReference = new StageReference()
          {
            StageName = ra.PreviousContext.StageReference.StageName
          };
        if (ra.PreviousContext.PhaseReference != null)
          this.PreviousContext.PhaseReference = new PhaseReference()
          {
            PhaseName = ra.PreviousContext.PhaseReference.PhaseName
          };
        if (ra.PreviousContext.JobReference != null)
          this.PreviousContext.JobReference = new JobReference()
          {
            JobName = ra.PreviousContext.JobReference.JobName
          };
      }
      this.TestFailuresAnalysis = ra.TestFailuresAnalysis;
      this.ResultsDifference = ra.ResultsDifference;
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public PipelineReference PreviousContext { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public TestResultFailuresAnalysis TestFailuresAnalysis { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public AggregatedResultsDifference ResultsDifference { get; set; }

    internal override void InitializeSecureObject(ISecuredObject securedObject)
    {
      base.InitializeSecureObject(securedObject);
      this.PreviousContext?.InitializeSecureObject(securedObject);
      this.TestFailuresAnalysis?.InitializeSecureObject(securedObject);
      this.ResultsDifference?.InitializeSecureObject(securedObject);
    }
  }
}
