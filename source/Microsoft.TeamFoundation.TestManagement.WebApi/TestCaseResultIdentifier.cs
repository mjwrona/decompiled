// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public class TestCaseResultIdentifier : TestManagementBaseSecuredObject
  {
    [DataMember(IsRequired = true)]
    public int TestResultId;
    [DataMember(IsRequired = true)]
    public int TestRunId;

    public TestCaseResultIdentifier()
    {
    }

    public TestCaseResultIdentifier(ISecuredObject securedObject)
      : base(securedObject)
    {
    }

    public TestCaseResultIdentifier(int runId, int resultId)
    {
      this.TestRunId = runId;
      this.TestResultId = resultId;
    }

    public override string ToString() => new JavaScriptSerializer().Serialize((object) this);

    public override bool Equals(object obj) => obj is TestCaseResultIdentifier resultIdentifier && this.TestResultId == resultIdentifier.TestResultId && this.TestRunId == resultIdentifier.TestRunId;

    public override int GetHashCode() => this.TestResultId * 31 + this.TestRunId;
  }
}
