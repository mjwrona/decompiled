// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.TestPlanCloneRequest
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public sealed class TestPlanCloneRequest
  {
    [DataMember(Name = "destinationTestPlan")]
    public TestPlan DestinationTestPlan { get; set; }

    [DataMember(Name = "suiteIds")]
    public List<int> SuiteIds { get; set; }

    [DataMember(Name = "options")]
    public CloneOptions Options { get; set; }
  }
}
