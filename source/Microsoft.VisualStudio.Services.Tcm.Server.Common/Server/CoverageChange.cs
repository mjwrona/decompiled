// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.CoverageChange
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  [ClassVisibility(ClientVisibility.Internal)]
  public class CoverageChange
  {
    [XmlAttribute]
    public int CoverageChangeId { get; set; }

    [XmlAttribute]
    public int TestRunId { get; set; }

    [XmlAttribute]
    public int SessionId { get; set; }

    [XmlAttribute]
    public int TestResultId { get; set; }

    [XmlAttribute]
    public int ConfigurationId { get; set; }

    [XmlElement]
    public BuildConfiguration BuildConfiguration { get; set; }

    [XmlAttribute]
    public CoverageChangeType ChangeType { get; set; }

    internal static List<CoverageChange> QueryCoverageChanges(TestManagementRequestContext context)
    {
      context.SecurityManager.CheckServiceAccount(context);
      Dictionary<int, string> dictionary = new Dictionary<int, string>();
      Dictionary<int, Guid> projectGuids = new Dictionary<int, Guid>();
      List<CoverageChange> coverageChangeList = new List<CoverageChange>();
      List<CoverageChangeExtension> coverageChangeExtensionList;
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        coverageChangeExtensionList = managementDatabase.QueryCoverageChanges(projectGuids);
      foreach (CoverageChangeExtension coverageChangeExtension in coverageChangeExtensionList)
      {
        int dataSpaceId = coverageChangeExtension.dataSpaceId;
        string str;
        if (dictionary.TryGetValue(dataSpaceId, out str))
        {
          coverageChangeExtension.BuildConfiguration.TeamProjectName = str;
        }
        else
        {
          dictionary[dataSpaceId] = context.ProjectServiceHelper.GetProjectName(projectGuids[dataSpaceId]);
          coverageChangeExtension.BuildConfiguration.TeamProjectName = dictionary[dataSpaceId];
        }
        coverageChangeList.Add((CoverageChange) coverageChangeExtension);
      }
      return coverageChangeList;
    }
  }
}
