// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.TestResultCreationRequestModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.TestManagement.Server;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  [DataContract]
  public class TestResultCreationRequestModel
  {
    public TestResultCreationRequestModel()
    {
    }

    public TestResultCreationRequestModel(TestCaseResult result)
    {
      this.TestCaseId = result.TestCaseId;
      this.ConfigurationId = result.ConfigurationId;
      this.ConfigurationName = result.ConfigurationName;
      this.TestPointId = result.TestPointId;
      this.Owner = result.Owner;
    }

    [DataMember(Name = "testCaseId")]
    public int TestCaseId { get; set; }

    [DataMember(Name = "configurationId")]
    public int ConfigurationId { get; set; }

    [DataMember(Name = "configurationName")]
    public string ConfigurationName { get; set; }

    [DataMember(Name = "testPointId")]
    public int TestPointId { get; set; }

    [DataMember(Name = "owner")]
    public Guid Owner { get; set; }
  }
}
