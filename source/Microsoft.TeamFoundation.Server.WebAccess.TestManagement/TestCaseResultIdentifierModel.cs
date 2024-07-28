// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.TestCaseResultIdentifierModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.TestManagement.Server;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  [DataContract]
  public class TestCaseResultIdentifierModel
  {
    public TestCaseResultIdentifierModel()
    {
    }

    public TestCaseResultIdentifierModel(TestCaseResultIdentifier id)
    {
      this.TestRunId = id.TestRunId;
      this.TestResultId = id.TestResultId;
    }

    [DataMember(Name = "testRunId")]
    public int TestRunId { get; set; }

    [DataMember(Name = "testResultId")]
    public int TestResultId { get; set; }

    internal TestCaseResultIdentifier CreateFromModel() => new TestCaseResultIdentifier(this.TestRunId, this.TestResultId);
  }
}
