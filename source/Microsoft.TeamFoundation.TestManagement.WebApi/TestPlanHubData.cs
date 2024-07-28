// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.TestPlanHubData
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public sealed class TestPlanHubData
  {
    [DataMember(IsRequired = true, Name = "testPlan")]
    public TestPlan TestPlan { get; set; }

    [DataMember(IsRequired = true, Name = "testSuites")]
    public List<TestSuite> TestSuites { get; set; }

    [DataMember(IsRequired = true, Name = "testPoints")]
    public List<TestPoint> TestPoints { get; set; }

    [DataMember(IsRequired = true, Name = "selectedSuiteId")]
    public int SelectedSuiteId { get; set; }

    [DataMember(IsRequired = true, Name = "totalTestPoints")]
    public int TotalTestPoints { get; set; }
  }
}
