// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.TestResultsQuery
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public class TestResultsQuery : TestManagementBaseSecuredObject
  {
    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public IList<TestCaseResult> Results { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public IList<string> Fields { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ResultsFilter ResultsFilter { get; set; }

    internal override void InitializeSecureObject(ISecuredObject securedObject)
    {
      base.InitializeSecureObject(securedObject);
      this.ResultsFilter?.InitializeSecureObject(securedObject);
      if (this.Results == null)
        return;
      foreach (TestCaseResult result in (IEnumerable<TestCaseResult>) this.Results)
        result.EnsureSecureObject();
    }
  }
}
