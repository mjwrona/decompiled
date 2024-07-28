// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.RunSummary
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public class RunSummary : TestManagementBaseSecuredObject
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int TotalRunsCount { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int NoConfigRunsCount { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public TimeSpan Duration { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public IDictionary<TestRunState, int> RunSummaryByState { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public IDictionary<TestRunOutcome, int> RunSummaryByOutcome { get; set; }
  }
}
