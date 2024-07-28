// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestRunStatistic
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClassVisibility(ClientVisibility.Internal)]
  public class TestRunStatistic
  {
    [XmlIgnore]
    public int TestRunId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public byte State { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public byte Outcome { get; set; }

    [XmlElement]
    [ClientProperty(ClientVisibility.Private)]
    public TestResolutionState ResolutionState { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public int Count { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public byte ResultMetadata { get; set; }

    internal static List<TestRunStatistic> Query(
      TestManagementRequestContext context,
      string projectName,
      int testRunId)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      if (!context.SecurityManager.HasViewTestResultsPermission(context, projectFromName.String))
        return new List<TestRunStatistic>();
      using (TestManagementDatabase managementDatabase1 = TestManagementDatabase.Create(context))
      {
        TestManagementDatabase managementDatabase2 = managementDatabase1;
        List<int> testRunIds = new List<int>(1);
        testRunIds.Add(testRunId);
        Guid guidId = projectFromName.GuidId;
        int num = context.IsTcmService ? 1 : 0;
        Dictionary<int, List<TestRunStatistic>> dictionary = managementDatabase2.QueryTestRunStatistics(testRunIds, guidId, num != 0);
        if (dictionary != null)
        {
          if (dictionary.ContainsKey(testRunId))
            return dictionary[testRunId];
        }
      }
      return new List<TestRunStatistic>();
    }
  }
}
