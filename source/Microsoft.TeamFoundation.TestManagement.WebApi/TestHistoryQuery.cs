// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.TestHistoryQuery
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
  public class TestHistoryQuery : TestManagementBaseSecuredObject
  {
    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public string AutomatedTestName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int TestCaseId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime? MaxCompleteDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int TrendDays { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public TestResultGroupBy GroupBy { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Branch { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int BuildDefinitionId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int ReleaseEnvDefinitionId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ContinuationToken { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public IList<TestResultHistoryForGroup> ResultsForGroup { get; set; }

    internal override void InitializeSecureObject(ISecuredObject securedObject)
    {
      base.InitializeSecureObject(securedObject);
      if (this.ResultsForGroup == null)
        return;
      foreach (TestManagementBaseSecuredObject baseSecuredObject in (IEnumerable<TestResultHistoryForGroup>) this.ResultsForGroup)
        baseSecuredObject.InitializeSecureObject(securedObject);
    }
  }
}
