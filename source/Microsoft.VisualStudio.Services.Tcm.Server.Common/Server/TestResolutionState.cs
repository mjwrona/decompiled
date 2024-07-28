// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestResolutionState
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
  public class TestResolutionState
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

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TestResolutionState Id={0} Name={1}", (object) this.m_id, (object) this.m_name);

    internal int Create(TestManagementRequestContext context, string teamProjectName)
    {
      if (string.IsNullOrEmpty(this.m_name))
        throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidFieldValue, (object) "Name"));
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, teamProjectName);
      context.SecurityManager.CheckManageTestConfigurationsPermission(context, projectFromName.String);
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        return managementDatabase.CreateTestResolutionState(this, projectFromName.GuidId);
    }

    internal static List<TestResolutionState> ImportResolutionStates(
      TestManagementRequestContext context,
      TestResolutionState[] testResolutionStates,
      string teamProjectName)
    {
      TestResolutionState.ValidateTestResolutionStates(context, testResolutionStates, teamProjectName);
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, teamProjectName);
      context.SecurityManager.CheckManageTestConfigurationsPermission(context, projectFromName.String);
      Guid teamFoundationId = context.UserTeamFoundationId;
      List<TestResolutionState> testResolutionStateList;
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        testResolutionStateList = managementDatabase.ImportResolutionStates(testResolutionStates, projectFromName.GuidId, teamFoundationId);
      foreach (TestResolutionState testResolutionState in testResolutionStateList)
      {
        testResolutionState.TeamProject = teamProjectName;
        testResolutionState.TeamProjectUri = projectFromName.String;
      }
      return testResolutionStateList;
    }

    internal void Update(TestManagementRequestContext context, string projectName)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      context.SecurityManager.CheckManageTestConfigurationsPermission(context, projectFromName.String);
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        managementDatabase.UpdateTestResolutionState(projectFromName.GuidId, this);
    }

    internal static void Delete(
      TestManagementRequestContext context,
      int stateId,
      string projectName)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      context.SecurityManager.CheckManageTestConfigurationsPermission(context, projectFromName.String);
      Guid teamFoundationId = context.UserTeamFoundationId;
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        managementDatabase.DeleteTestResolutionState(projectFromName.GuidId, stateId, teamFoundationId);
    }

    internal static List<TestResolutionState> Query(
      TestManagementRequestContext context,
      int testResolutionStateId,
      string teamProjectName,
      bool checkPermissions = true)
    {
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "TestResolutionState.Query"))
      {
        GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, teamProjectName);
        if (checkPermissions && !context.SecurityManager.HasViewTestResultsPermission(context, projectFromName.String))
          return new List<TestResolutionState>();
        List<TestResolutionState> testResolutionStateList;
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          testResolutionStateList = managementDatabase.QueryTestResolutionStates(testResolutionStateId, projectFromName.GuidId);
        foreach (TestResolutionState testResolutionState in testResolutionStateList)
        {
          testResolutionState.TeamProject = teamProjectName;
          testResolutionState.TeamProjectUri = projectFromName.String;
        }
        return testResolutionStateList;
      }
    }

    private static void ValidateTestResolutionStates(
      TestManagementRequestContext context,
      TestResolutionState[] testResolutionStates,
      string teamProjectName)
    {
      if (testResolutionStates.Length == 0)
        throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.ResolutionStatesCannotBeEmpty));
      foreach (TestResolutionState testResolutionState in testResolutionStates)
      {
        if (string.IsNullOrEmpty(testResolutionState.Name))
          throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidFieldValue, (object) "Name"));
      }
    }
  }
}
