// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.TestResultCreationResponseModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.TestManagement.Server;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  [DataContract]
  public class TestResultCreationResponseModel
  {
    public TestResultCreationResponseModel()
    {
    }

    public TestResultCreationResponseModel(TestCaseResult result)
    {
      this.Id = new TestCaseResultIdentifierModel(result.Id);
      this.TestCaseTitle = result.TestCaseTitle;
      this.TestPointId = result.TestPointId;
      this.Priority = (int) result.Priority;
      this.DataRowCount = result.DataRowCount;
      this.TestCaseRevision = result.TestCaseRevision;
    }

    [DataMember(Name = "id")]
    public TestCaseResultIdentifierModel Id { get; set; }

    [DataMember(Name = "testCaseTitle")]
    public string TestCaseTitle { get; set; }

    [DataMember(Name = "testPointId")]
    public int TestPointId { get; set; }

    [DataMember(Name = "dataRowCount")]
    public int DataRowCount { get; set; }

    [DataMember(Name = "testCaseRevision")]
    public int TestCaseRevision { get; set; }

    [DataMember(Name = "priority")]
    public int Priority { get; set; }
  }
}
