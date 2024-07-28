// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.AggregatedDataForResultTrend
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public class AggregatedDataForResultTrend : TestManagementBaseSecuredObject
  {
    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public TestResultsContext TestResultsContext { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public IDictionary<TestOutcome, AggregatedResultsByOutcome> ResultsByOutcome { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public IDictionary<TestRunState, AggregatedRunsByState> RunSummaryByState { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public TimeSpan Duration { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int TotalTests { get; set; }

    internal override void InitializeSecureObject(ISecuredObject securedObject)
    {
      base.InitializeSecureObject(securedObject);
      this.TestResultsContext?.InitializeSecureObject(securedObject);
      if (this.ResultsByOutcome != null)
      {
        foreach (TestManagementBaseSecuredObject baseSecuredObject in (IEnumerable<AggregatedResultsByOutcome>) this.ResultsByOutcome.Values)
          baseSecuredObject.InitializeSecureObject(securedObject);
      }
      if (this.RunSummaryByState == null)
        return;
      foreach (TestManagementBaseSecuredObject baseSecuredObject in (IEnumerable<AggregatedRunsByState>) this.RunSummaryByState.Values)
        baseSecuredObject.InitializeSecureObject(securedObject);
    }
  }
}
