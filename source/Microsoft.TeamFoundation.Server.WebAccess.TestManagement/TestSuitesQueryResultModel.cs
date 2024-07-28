// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.TestSuitesQueryResultModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  [DataContract]
  internal class TestSuitesQueryResultModel
  {
    [DataMember(Name = "testSuites")]
    public List<TestSuiteModel> TestSuites { get; set; }

    [DataMember(Name = "selectedTester")]
    public TesterModel SelectedTester { get; set; }

    [DataMember(Name = "selectedOutcome")]
    public string SelectedOutCome { get; set; }

    [DataMember(Name = "selectedConfiguration")]
    public TestConfigurationModel SelectedConfiguration { get; set; }

    [DataMember(Name = "testers")]
    public List<TesterModel> Testers { get; set; }

    [DataMember(Name = "configurations")]
    public List<TestConfigurationModel> Configurations { get; set; }
  }
}
