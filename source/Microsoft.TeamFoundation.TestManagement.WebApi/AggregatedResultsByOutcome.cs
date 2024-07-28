// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.AggregatedResultsByOutcome
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public class AggregatedResultsByOutcome : TestManagementBaseSecuredObject
  {
    public AggregatedResultsByOutcome()
    {
    }

    public AggregatedResultsByOutcome(ISecuredObject securedObject)
      : base(securedObject)
    {
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public TestOutcome Outcome { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string GroupByField { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public object GroupByValue { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int Count { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public TimeSpan Duration { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int RerunResultCount { get; set; }
  }
}
