// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestResultFailureType
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TestResultFailureType
  {
    protected int m_id;
    protected string m_name;
    protected string m_teamProjectUri;
    protected string m_teamProject;

    public int Id
    {
      get => this.m_id;
      set => this.m_id = value;
    }

    public string Name
    {
      get => this.m_name;
      set
      {
        Validator.CheckAndTrimString(ref value, nameof (Name), 256);
        this.m_name = value;
      }
    }

    internal string TeamProjectUri
    {
      get => this.m_teamProjectUri;
      set => this.m_teamProjectUri = value;
    }

    internal string TeamProject
    {
      get => this.m_teamProject;
      set => this.m_teamProject = value;
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TestResultFailureType Id={0} Name={1}", (object) this.m_id, (object) this.m_name);

    internal static List<TestResultFailureType> QueryTestResultFailureType(
      TestManagementRequestContext context,
      int testFailureTypeId,
      string teamProjectName,
      bool checkPermissions = true)
    {
      context.RequestContext.Trace(1015932, TraceLevel.Info, "TestManagement", "BusinessLayer", "TestResultFailureType.QueryTestResultFailureType. ProjectName: {0}, FailureTypeId: {1}", (object) teamProjectName, (object) testFailureTypeId);
      context.TraceEnter("BusinessLayer", "TestResultFailureType.QueryTestResultFailureType");
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, teamProjectName);
      if (checkPermissions && !context.SecurityManager.HasViewTestResultsPermission(context, projectFromName.String))
        return new List<TestResultFailureType>();
      List<TestResultFailureType> resultFailureTypeList = new List<TestResultFailureType>();
      try
      {
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          resultFailureTypeList = managementDatabase.QueryTestResultFailureTypes(testFailureTypeId, projectFromName.GuidId);
        foreach (TestResultFailureType resultFailureType in resultFailureTypeList)
        {
          resultFailureType.TeamProject = teamProjectName;
          resultFailureType.TeamProjectUri = projectFromName.String;
        }
      }
      catch (Exception ex)
      {
        context.TraceError("BusinessLayer", "TestResultFailureType.QueryTestResultFailureType threw exception. ProjectId: {0}, FailureTypeId: {1}, Exception message: {2}", (object) projectFromName, (object) testFailureTypeId, (object) ex.Message);
        throw;
      }
      finally
      {
        context.TraceLeave("BusinessLayer", "TestResultFailureType.QueryTestResultFailureType");
      }
      return resultFailureTypeList;
    }

    internal static TestResultFailureType CreateTestResultFailureType(
      TestManagementRequestContext context,
      string failureTypeName,
      string teamProjectName)
    {
      context.RequestContext.Trace(1015933, TraceLevel.Info, "TestManagement", "BusinessLayer", "TestResultFailureType.CreateTestResultFailureType. ProjectName: {0}, FailureTypeName: {1}", (object) teamProjectName, (object) failureTypeName);
      context.TraceEnter("BusinessLayer", "TestResultFailureType.CreateTestResultFailureType");
      if (string.IsNullOrEmpty(failureTypeName))
        throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidFieldValue, (object) "Name"));
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, teamProjectName);
      context.SecurityManager.CheckManageTestConfigurationsPermission(context, projectFromName.String);
      TestResultFailureType resultFailureType = new TestResultFailureType();
      try
      {
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          return managementDatabase.CreateTestResultFailureType(failureTypeName, projectFromName.GuidId);
      }
      catch (Exception ex)
      {
        context.TraceError("BusinessLayer", "TestResultFailureType.CreateTestResultFailureType threw exception. ProjectId: {0}, FailureTypeName: {1}, Exception message: {2}", (object) projectFromName, (object) failureTypeName, (object) ex.Message);
        throw;
      }
      finally
      {
        context.TraceLeave("BusinessLayer", "TestResultFailureType.CreateTestResultFailureType");
      }
    }

    internal static bool DeleteTestResultFailureType(
      TestManagementRequestContext context,
      int failureTypeId,
      string projectName)
    {
      context.RequestContext.Trace(1015934, TraceLevel.Info, "TestManagement", "BusinessLayer", "TestResultFailureType.DeleteTestResultFailureType. ProjectName: {0}, FailureTypeId: {1}", (object) projectName, (object) failureTypeId);
      context.TraceEnter("BusinessLayer", "TestResultFailureType.DeleteTestResultFailureType");
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      context.SecurityManager.CheckManageTestConfigurationsPermission(context, projectFromName.String);
      Guid teamFoundationId = context.UserTeamFoundationId;
      try
      {
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          return managementDatabase.DeleteTestResultFailureType(projectFromName.GuidId, failureTypeId, teamFoundationId);
      }
      catch (Exception ex)
      {
        context.TraceError("BusinessLayer", "TestResultFailureType.DeleteTestResultFailureType threw exception. ProjectId: {0}, FailureTypeId: {1}, Exception message: {2}", (object) projectFromName, (object) failureTypeId, (object) ex.Message);
        if (Regex.IsMatch(ex.Message, "Test failure type \\w+ cannot be found"))
          return false;
        throw;
      }
      finally
      {
        context.TraceLeave("BusinessLayer", "TestResultFailureType.DeleteTestResultFailureType");
      }
    }
  }
}
