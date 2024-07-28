// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.TestPointModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.TestManagement.Client;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  [DataContract]
  internal class TestPointModel
  {
    [DataMember(Name = "workItemProperties")]
    public Dictionary<string, object> testCaseFields;
    [DataMember(Name = "sequenceNumber")]
    public int SequenceNumber;

    public TestPointModel()
    {
    }

    public TestPointModel(int testCaseId) => this.TestCaseId = testCaseId;

    [DataMember(Name = "testPointId")]
    public int TestPointId { get; set; }

    [DataMember(Name = "testCaseId")]
    public int TestCaseId { get; set; }

    [DataMember(Name = "tester")]
    public string Tester { get; set; }

    [DataMember(Name = "assignedTo")]
    public Guid AssignedTo { get; set; }

    [DataMember(Name = "configurationId")]
    public int ConfigurationId { get; set; }

    [DataMember(Name = "configurationName")]
    public string ConfigurationName { get; set; }

    [DataMember(Name = "suiteName")]
    public string SuiteName { get; set; }

    [DataMember(Name = "suiteId")]
    public int SuiteId { get; set; }

    [DataMember(Name = "mostRecentRunId")]
    public int MostRecentRunId { get; set; }

    [DataMember(Name = "mostRecentResultId")]
    public int MostRecentResultId { get; set; }

    [DataMember(Name = "mostRecentResultOutcome")]
    public TestOutcome MostRecentOutcome { get; set; }

    [DataMember(Name = "lastResultState")]
    public TestResultState LastResultState { get; set; }

    [DataMember(Name = "state")]
    public TestPointState State { get; set; }

    [DataMember(Name = "automated")]
    public bool Automated { get; set; }

    [DataMember(Name = "outcome")]
    public string Outcome { get; set; }

    [DataMember(Name = "lastRunDuration")]
    public double LastRunDuration { get; set; }

    [DataMember(Name = "lastRunBy")]
    public string LastRunBy { get; set; }

    [DataMember(Name = "build")]
    public string Build { get; set; }
  }
}
