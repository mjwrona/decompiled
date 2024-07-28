// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestFailureType
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClassVisibility(ClientVisibility.Internal)]
  public class TestFailureType
  {
    protected int m_id;
    protected string m_name;
    protected string m_teamProjectUri;
    protected string m_teamProject;

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private, UseClientDefinedProperty = true)]
    [DefaultValue(0)]
    public int Id
    {
      get => this.m_id;
      set => this.m_id = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string Name
    {
      get => this.m_name;
      set
      {
        Validator.CheckAndTrimString(ref value, nameof (Name), 256);
        this.m_name = value;
      }
    }

    [XmlIgnore]
    internal string TeamProjectUri
    {
      get => this.m_teamProjectUri;
      set => this.m_teamProjectUri = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string TeamProject
    {
      get => this.m_teamProject;
      set => this.m_teamProject = value;
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TestFailureType Id={0} Name={1}", (object) this.m_id, (object) this.m_name);

    internal int Create(TestManagementRequestContext context, string teamProjectName)
    {
      if (string.IsNullOrEmpty(this.m_name))
        throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidFieldValue, (object) "Name"));
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, teamProjectName);
      context.SecurityManager.CheckManageTestConfigurationsPermission(context, projectFromName.String);
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        return managementDatabase.CreateTestFailureType(this, projectFromName.GuidId);
    }

    internal static List<TestFailureType> ImportFailureTypes(
      TestManagementRequestContext context,
      TestFailureType[] testFailureTypes,
      string teamProjectName)
    {
      TestFailureType.ValidateTestFailureTypes(context, testFailureTypes, teamProjectName);
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, teamProjectName);
      context.SecurityManager.CheckManageTestConfigurationsPermission(context, projectFromName.String);
      Guid teamFoundationId = context.UserTeamFoundationId;
      List<TestFailureType> testFailureTypeList;
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        testFailureTypeList = managementDatabase.ImportFailureTypes(testFailureTypes, projectFromName.GuidId, teamFoundationId);
      foreach (TestFailureType testFailureType in testFailureTypeList)
      {
        testFailureType.TeamProject = teamProjectName;
        testFailureType.TeamProjectUri = projectFromName.String;
      }
      return testFailureTypeList;
    }

    internal void Update(TestManagementRequestContext context, string projectName)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      context.SecurityManager.CheckManageTestConfigurationsPermission(context, projectFromName.String);
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        managementDatabase.UpdateTestFailureType(projectFromName.GuidId, this);
    }

    internal static void Delete(
      TestManagementRequestContext context,
      int failureTypeId,
      string projectName)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      context.SecurityManager.CheckManageTestConfigurationsPermission(context, projectFromName.String);
      Guid teamFoundationId = context.UserTeamFoundationId;
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        managementDatabase.DeleteTestFailureType(projectFromName.GuidId, failureTypeId, teamFoundationId);
    }

    internal static List<TestFailureType> Query(
      TestManagementRequestContext context,
      int testFailureTypeId,
      string teamProjectName,
      bool checkPermissions = true)
    {
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "TestFailureType.Query"))
      {
        GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, teamProjectName);
        if (checkPermissions && !context.SecurityManager.HasViewTestResultsPermission(context, projectFromName.String))
          return new List<TestFailureType>();
        List<TestFailureType> testFailureTypeList;
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          testFailureTypeList = managementDatabase.QueryTestFailureTypes(testFailureTypeId, projectFromName.GuidId);
        foreach (TestFailureType testFailureType in testFailureTypeList)
        {
          testFailureType.TeamProject = teamProjectName;
          testFailureType.TeamProjectUri = projectFromName.String;
        }
        return testFailureTypeList;
      }
    }

    internal static string GetFailureTypeNameFromId(
      TestManagementRequestContext context,
      int failureTypeId,
      string teamProjectName)
    {
      return TestFailureType.Query(context, failureTypeId, teamProjectName).Find((Predicate<TestFailureType>) (x => x.Id == failureTypeId))?.Name;
    }

    internal static byte GetFailureTypeFromId(int failureTypeId) => failureTypeId >= 0 && failureTypeId < 5 ? (byte) failureTypeId : (byte) 0;

    private static void ValidateTestFailureTypes(
      TestManagementRequestContext context,
      TestFailureType[] testFailureTypes,
      string teamProjectName)
    {
      if (testFailureTypes.Length == 0)
        throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.FailureTypesCannotBeEmpty));
      foreach (TestFailureType testFailureType in testFailureTypes)
      {
        if (string.IsNullOrEmpty(testFailureType.Name))
          throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidFieldValue, (object) "Name"));
      }
    }
  }
}
