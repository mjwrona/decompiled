// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.TestSuiteDisplayModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.Server;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  [DataContract]
  internal class TestSuiteDisplayModel
  {
    public TestSuiteDisplayModel()
    {
    }

    public TestSuiteDisplayModel(ServerTestSuite suite)
    {
      this.Id = suite.Id;
      this.Title = suite.Title;
      this.ChildSuiteIds = new List<int>();
      this.TestCaseIds = new List<int>();
      this.ParentId = suite.ParentId;
      this.RequirementId = suite.RequirementId;
      this.SuiteType = (TestSuiteEntryType) suite.SuiteType;
      this.Revision = suite.Revision;
      this.QueryText = suite.QueryString;
      this.State = (TestSuiteState) suite.State;
      this.Status = suite.Status;
      this.Configurations = TestSuiteDisplayModel.GetSuiteConfigs(suite);
    }

    [DataMember(Name = "id")]
    public int Id { get; set; }

    [DataMember(Name = "pointCount")]
    public int PointCount { get; set; }

    [DataMember(Name = "title")]
    public string Title { get; set; }

    [DataMember(Name = "childSuiteIds")]
    public List<int> ChildSuiteIds { get; set; }

    [DataMember(Name = "testCaseIds")]
    public List<int> TestCaseIds { get; set; }

    [DataMember(Name = "parentSuiteId")]
    public int ParentId { get; set; }

    [DataMember(Name = "type")]
    public TestSuiteEntryType SuiteType { get; set; }

    [DataMember(Name = "requirementId")]
    public int RequirementId { get; set; }

    [DataMember(Name = "revision")]
    public int Revision { get; set; }

    [DataMember(Name = "queryText")]
    public string QueryText { get; set; }

    [DataMember(Name = "state")]
    public TestSuiteState State { get; set; }

    [DataMember(Name = "status")]
    public string Status { get; set; }

    [DataMember(Name = "configurations")]
    public List<TestConfigurationModel> Configurations { get; set; }

    public static List<TestConfigurationModel> GetSuiteConfigs(ServerTestSuite suite)
    {
      List<TestConfigurationModel> suiteConfigs = new List<TestConfigurationModel>();
      for (int index = 0; index < suite.DefaultConfigurations.Count; ++index)
        suiteConfigs.Add(new TestConfigurationModel(suite.DefaultConfigurationNames[index], suite.DefaultConfigurations[index]));
      return suiteConfigs;
    }
  }
}
