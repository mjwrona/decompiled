// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestExtensibilityHelper
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TestExtensibilityHelper : RestApiHelper, ITestExtensibilityHelper
  {
    public TestExtensibilityHelper(TestManagementRequestContext requestContext)
      : base(requestContext)
    {
    }

    public IList<CustomTestFieldDefinition> AddNewFields(
      GuidAndString projectId,
      IList<CustomTestFieldDefinition> newFields)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) newFields, nameof (newFields), "Test Results");
      IEnumerable<CustomTestFieldDefinition> testFieldDefinitions = newFields.Where<CustomTestFieldDefinition>((System.Func<CustomTestFieldDefinition, bool>) (f => (f.Scope & CustomTestFieldScope.System) == CustomTestFieldScope.System));
      if (testFieldDefinitions != null && testFieldDefinitions.Any<CustomTestFieldDefinition>())
        throw new InvalidPropertyException(string.Format(ServerResources.UsersCannotAddSystemFieldsError, (object) string.Join<CustomTestFieldDefinition>(",", testFieldDefinitions)));
      this.RequestContext.TraceInfo("RestLayer", "TestExtensibilityHelper.AddNewFields projectId = {0}, newFields={1}", (object) projectId.ToString(), (object) string.Join(",", newFields.Select<CustomTestFieldDefinition, string>((System.Func<CustomTestFieldDefinition, string>) (f => f.FieldName))));
      return (IList<CustomTestFieldDefinition>) this.ExecuteAction<List<CustomTestFieldDefinition>>("TestExtensibilityHelper.AddNewFields", (Func<List<CustomTestFieldDefinition>>) (() =>
      {
        IList<TestExtensionFieldDetails> extensionFieldDetailsList = this.RequestContext.GetService<ITeamFoundationTestExtensionFieldsService>().AddFields(this.TestManagementRequestContext, projectId.GuidId, (IList<TestExtensionFieldDetails>) newFields.Select<CustomTestFieldDefinition, TestExtensionFieldDetails>((System.Func<CustomTestFieldDefinition, TestExtensionFieldDetails>) (f => this.ConvertFromWebApiToServerOMFormat(f))).ToList<TestExtensionFieldDetails>());
        this.LogAddFieldsTelemetryPoints(this.RequestContext, extensionFieldDetailsList);
        return extensionFieldDetailsList.Select<TestExtensionFieldDetails, CustomTestFieldDefinition>((System.Func<TestExtensionFieldDetails, CustomTestFieldDefinition>) (f => this.ConvertFromServerOMToWebApiFormat(f))).ToList<CustomTestFieldDefinition>();
      }), 1015056, "TestManagement");
    }

    public IList<CustomTestFieldDefinition> QueryFields(
      GuidAndString projectId,
      CustomTestFieldScope scopeFilter)
    {
      if (scopeFilter == CustomTestFieldScope.None)
        throw new ArgumentException(nameof (scopeFilter)).Expected("Test Results");
      this.RequestContext.TraceInfo("RestLayer", "TestExtensibilityHelper.QueryFields projectId = {0}, scope filter={1}", (object) projectId.ToString(), (object) scopeFilter.ToString());
      return (IList<CustomTestFieldDefinition>) this.ExecuteAction<List<CustomTestFieldDefinition>>("TestExtensibilityHelper.QueryFields", (Func<List<CustomTestFieldDefinition>>) (() =>
      {
        IList<TestExtensionFieldDetails> source = this.RequestContext.GetService<ITeamFoundationTestExtensionFieldsService>().QueryFields(this.TestManagementRequestContext, projectId.GuidId, scopeFilter: scopeFilter);
        this.LogQueryFieldsTelemetryPoints(this.RequestContext, scopeFilter);
        return source.Select<TestExtensionFieldDetails, CustomTestFieldDefinition>((System.Func<TestExtensionFieldDetails, CustomTestFieldDefinition>) (f => this.ConvertFromServerOMToWebApiFormat(f))).ToList<CustomTestFieldDefinition>();
      }), 1015056, "TestManagement");
    }

    internal TestExtensionFieldDetails ConvertFromWebApiToServerOMFormat(
      CustomTestFieldDefinition fieldDetails)
    {
      return new TestExtensionFieldDetails()
      {
        Name = fieldDetails.FieldName,
        Type = (SqlDbType) fieldDetails.FieldType,
        IsRunScoped = (fieldDetails.Scope & CustomTestFieldScope.TestRun) == CustomTestFieldScope.TestRun,
        IsResultScoped = (fieldDetails.Scope & CustomTestFieldScope.TestResult) == CustomTestFieldScope.TestResult,
        IsSystemField = false
      };
    }

    internal CustomTestFieldDefinition ConvertFromServerOMToWebApiFormat(
      TestExtensionFieldDetails fieldDetails)
    {
      CustomTestFieldDefinition webApiFormat = new CustomTestFieldDefinition();
      webApiFormat.FieldId = fieldDetails.Id;
      webApiFormat.FieldName = fieldDetails.Name;
      webApiFormat.FieldType = (CustomTestFieldType) fieldDetails.Type;
      webApiFormat.Scope = CustomTestFieldScope.None;
      if (fieldDetails.IsRunScoped)
        webApiFormat.Scope |= CustomTestFieldScope.TestRun;
      if (fieldDetails.IsResultScoped)
        webApiFormat.Scope |= CustomTestFieldScope.TestResult;
      if (fieldDetails.IsSystemField)
        webApiFormat.Scope |= CustomTestFieldScope.System;
      return webApiFormat;
    }

    private void LogAddFieldsTelemetryPoints(
      IVssRequestContext context,
      IList<TestExtensionFieldDetails> fields)
    {
      CustomerIntelligenceData cid = new CustomerIntelligenceData();
      cid.Add("TestExtensionFieldAdded", (double) fields.Count);
      IEnumerable<TestExtensionFieldDetails> source1 = fields.Where<TestExtensionFieldDetails>((System.Func<TestExtensionFieldDetails, bool>) (f => f.IsRunScoped));
      if (source1 != null && source1.Any<TestExtensionFieldDetails>())
        cid.Add("RunScopedTestExtensionFieldAdded", (double) source1.Count<TestExtensionFieldDetails>());
      IEnumerable<TestExtensionFieldDetails> source2 = fields.Where<TestExtensionFieldDetails>((System.Func<TestExtensionFieldDetails, bool>) (f => f.IsResultScoped));
      if (source2 != null && source2.Any<TestExtensionFieldDetails>())
        cid.Add("ResultScopedTestExtensionFieldAdded", (double) source2.Count<TestExtensionFieldDetails>());
      this.TelemetryLogger.PublishData(context, "TestExtensibility", cid);
    }

    private void LogQueryFieldsTelemetryPoints(
      IVssRequestContext context,
      CustomTestFieldScope scopeFilter)
    {
      CustomerIntelligenceData cid = new CustomerIntelligenceData();
      cid.Add("TestExtensionFieldQueryScope", scopeFilter.ToString());
      this.TelemetryLogger.PublishData(context, "TestExtensibility", cid);
    }
  }
}
