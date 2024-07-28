// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.AggregatedResultsDifference
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public class AggregatedResultsDifference : TestManagementBaseSecuredObject
  {
    public AggregatedResultsDifference()
    {
    }

    public AggregatedResultsDifference(ISecuredObject securedObject)
      : base(securedObject)
    {
    }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int IncreaseInTotalTests { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int IncreaseInFailures { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int IncreaseInPassedTests { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int IncreaseInNonImpactedTests { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int IncreaseInOtherTests { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public TimeSpan IncreaseInDuration { get; set; }
  }
}
