// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TeamFoundationTestExtensionFieldsService
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TeamFoundationTestExtensionFieldsService : 
    TeamFoundationTestManagementService,
    ITeamFoundationTestExtensionFieldsService,
    IVssFrameworkService
  {
    public IList<TestExtensionFieldDetails> AddFields(
      TestManagementRequestContext context,
      Guid projectId,
      IList<TestExtensionFieldDetails> fields)
    {
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        return managementDatabase.AddExtensionFields(context.RequestContext, projectId, fields);
    }

    public IList<TestExtensionFieldDetails> QueryFields(
      TestManagementRequestContext context,
      Guid projectId,
      IList<string> fieldNamesFilter = null,
      CustomTestFieldScope scopeFilter = CustomTestFieldScope.None)
    {
      List<TestExtensionFieldDetails> source = new List<TestExtensionFieldDetails>();
      bool isRunScoped = false;
      bool isResultScoped = false;
      bool isSystemScoped = false;
      if (scopeFilter == CustomTestFieldScope.None)
      {
        isRunScoped = true;
        isResultScoped = true;
        isSystemScoped = true;
      }
      else
      {
        if ((scopeFilter & CustomTestFieldScope.TestRun) == CustomTestFieldScope.TestRun)
          isRunScoped = true;
        if ((scopeFilter & CustomTestFieldScope.TestResult) == CustomTestFieldScope.TestResult)
          isResultScoped = true;
        if ((scopeFilter & CustomTestFieldScope.System) == CustomTestFieldScope.System)
          isSystemScoped = true;
      }
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        source = managementDatabase.QueryExtensionFields(context.RequestContext, projectId, isRunScoped, isResultScoped, isSystemScoped).ToList<TestExtensionFieldDetails>();
      List<TestExtensionFieldDetails> extensionFieldDetailsList = source;
      if (fieldNamesFilter != null)
      {
        extensionFieldDetailsList = new List<TestExtensionFieldDetails>();
        List<string> stringList = new List<string>();
        foreach (string str in (IEnumerable<string>) fieldNamesFilter)
        {
          string fieldName = str;
          TestExtensionFieldDetails extensionFieldDetails = source.Where<TestExtensionFieldDetails>((System.Func<TestExtensionFieldDetails, bool>) (f => string.Equals(fieldName, f.Name, StringComparison.CurrentCultureIgnoreCase))).FirstOrDefault<TestExtensionFieldDetails>();
          if (extensionFieldDetails == null)
            stringList.Add(fieldName);
          else
            extensionFieldDetailsList.Add(extensionFieldDetails);
        }
        if (stringList.Any<string>())
          throw new Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectNotFoundException(string.Format(ServerResources.ExtensionFieldsNotFoundError, (object) string.Join(",", (IEnumerable<string>) stringList)));
      }
      return (IList<TestExtensionFieldDetails>) extensionFieldDetailsList;
    }

    public IList<TestExtensionFieldDetails> DeleteFields(
      TestManagementRequestContext context,
      Guid projectId,
      IList<string> fieldNames = null)
    {
      return (IList<TestExtensionFieldDetails>) null;
    }

    public void CreateDefaultTestExtensionFieldsForExistingProjects(
      TestManagementRequestContext context)
    {
      IEnumerable<ProjectInfo> projects = context.RequestContext.GetService<IProjectService>().GetProjects(context.RequestContext, ProjectState.WellFormed);
      if (projects == null || !projects.Any<ProjectInfo>())
      {
        context.TraceInfo("BusinessLayer", "No projects found having test data.");
      }
      else
      {
        foreach (ProjectInfo projectInfo in projects)
          this.CreateDefaultTestExtensionFieldsForProject(context, projectInfo.Id);
      }
    }

    public void CreateDefaultTestExtensionFieldsForProject(
      TestManagementRequestContext context,
      Guid projectId)
    {
      try
      {
        this.AddFields(context, projectId, (IList<TestExtensionFieldDetails>) new List<TestExtensionFieldDetails>()
        {
          new TestExtensionFieldDetails()
          {
            Name = "StackTrace",
            Type = SqlDbType.NVarChar,
            IsResultScoped = true,
            IsSystemField = true
          },
          new TestExtensionFieldDetails()
          {
            Name = "FailingSince",
            Type = SqlDbType.NVarChar,
            IsResultScoped = true,
            IsSystemField = true
          },
          new TestExtensionFieldDetails()
          {
            Name = "Comment",
            Type = SqlDbType.NVarChar,
            IsResultScoped = true,
            IsSystemField = true
          },
          new TestExtensionFieldDetails()
          {
            Name = "ErrorMessage",
            Type = SqlDbType.NVarChar,
            IsResultScoped = true,
            IsSystemField = true
          },
          new TestExtensionFieldDetails()
          {
            Name = "OutcomeConfidence",
            Type = SqlDbType.Float,
            IsResultScoped = true,
            IsSystemField = true
          },
          new TestExtensionFieldDetails()
          {
            Name = "TestRunSystem",
            Type = SqlDbType.NVarChar,
            IsRunScoped = true,
            IsSystemField = true
          },
          new TestExtensionFieldDetails()
          {
            Name = "AttemptId",
            Type = SqlDbType.Int,
            IsResultScoped = true,
            IsSystemField = true
          },
          new TestExtensionFieldDetails()
          {
            Name = "UnsanitizedTestCaseTitle",
            Type = SqlDbType.NVarChar,
            IsResultScoped = true,
            IsSystemField = true
          },
          new TestExtensionFieldDetails()
          {
            Name = "UnsanitizedAutomatedTestName",
            Type = SqlDbType.NVarChar,
            IsResultScoped = true,
            IsSystemField = true
          },
          new TestExtensionFieldDetails()
          {
            Name = "TestResultGroupType",
            Type = SqlDbType.NVarChar,
            IsResultScoped = true,
            IsSystemField = true
          },
          new TestExtensionFieldDetails()
          {
            Name = "MaxReservedSubResultId",
            Type = SqlDbType.Int,
            IsResultScoped = true,
            IsSystemField = true
          },
          new TestExtensionFieldDetails()
          {
            Name = "IsTestResultFlaky",
            Type = SqlDbType.Bit,
            IsResultScoped = true,
            IsSystemField = true
          },
          new TestExtensionFieldDetails()
          {
            Name = "TestResultFlakyState",
            Type = SqlDbType.TinyInt,
            IsResultScoped = true,
            IsSystemField = true
          },
          new TestExtensionFieldDetails()
          {
            Name = "LogStoreContainerState",
            Type = SqlDbType.Int,
            IsRunScoped = true,
            IsSystemField = true
          },
          new TestExtensionFieldDetails()
          {
            Name = "LogStoreContainerSize",
            Type = SqlDbType.Int,
            IsRunScoped = true,
            IsSystemField = true
          },
          new TestExtensionFieldDetails()
          {
            Name = "NewTestEntry",
            Type = SqlDbType.Int,
            IsResultScoped = true
          }
        });
      }
      catch (Exception ex)
      {
        context.RequestContext.Trace(1015011, TraceLevel.Error, "TestManagement", "BusinessLayer", string.Format("Extension fields not added to project {0} due to exception: {1}", (object) projectId.ToString(), (object) ex.ToString()));
        throw;
      }
    }
  }
}
