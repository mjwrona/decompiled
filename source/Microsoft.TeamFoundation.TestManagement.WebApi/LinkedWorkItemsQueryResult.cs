// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.LinkedWorkItemsQueryResult
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  public class LinkedWorkItemsQueryResult
  {
    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public string AutomatedTestName { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public int TestCaseId { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public int PlanId { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public int SuiteId { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public int PointId { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public List<WorkItemReference> WorkItems { get; set; }
  }
}
