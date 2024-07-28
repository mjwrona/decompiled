// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.TestFailureDetails
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public class TestFailureDetails : TestManagementBaseSecuredObject
  {
    public TestFailureDetails()
    {
    }

    public TestFailureDetails(ISecuredObject securedObject)
      : base(securedObject)
    {
    }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int Count { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public IList<TestCaseResultIdentifier> TestResults { get; set; }

    internal override void InitializeSecureObject(ISecuredObject securedObject)
    {
      base.InitializeSecureObject(securedObject);
      if (this.TestResults == null)
        return;
      foreach (TestManagementBaseSecuredObject testResult in (IEnumerable<TestCaseResultIdentifier>) this.TestResults)
        testResult.InitializeSecureObject(securedObject);
    }
  }
}
