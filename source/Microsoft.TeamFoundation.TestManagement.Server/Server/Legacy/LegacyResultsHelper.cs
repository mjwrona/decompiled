// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Legacy.LegacyResultsHelper
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Tcm.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.TestManagement.Server.Legacy
{
  internal class LegacyResultsHelper
  {
    public void DeleteTestRun(
      TfsTestManagementRequestContext context,
      string projectName,
      int[] testRunIds)
    {
      if (!context.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFSLegacy"))
      {
        int[] numArray1;
        if (testRunIds == null)
        {
          numArray1 = (int[]) null;
        }
        else
        {
          IEnumerable<int> source = ((IEnumerable<int>) testRunIds).Where<int>((Func<int, bool>) (testRunId => context.LegacyTcmServiceHelper.IsTestRunInTCM(context.RequestContext, testRunId, false)));
          numArray1 = source != null ? source.ToArray<int>() : (int[]) null;
        }
        int[] source1 = numArray1;
        if (source1 != null && ((IEnumerable<int>) source1).Any<int>())
          context.LegacyTcmServiceHelper.TryDeleteTestRunIds(context.RequestContext, new DeleteTestRunRequest()
          {
            TestRunIds = source1,
            ProjectName = projectName
          });
        TCMServiceDataMigrationRestHelper.BlockRequestsIfDataMigrationInProgressOrFailed(context.RequestContext);
        int[] numArray2;
        if (testRunIds == null)
        {
          numArray2 = (int[]) null;
        }
        else
        {
          IEnumerable<int> source2 = ((IEnumerable<int>) testRunIds).Where<int>((Func<int, bool>) (testRunId => !context.LegacyTcmServiceHelper.IsTestRunInTCM(context.RequestContext, testRunId, false)));
          numArray2 = source2 != null ? source2.ToArray<int>() : (int[]) null;
        }
        int[] numArray3 = numArray2;
        if (numArray3 == null || !((IEnumerable<int>) numArray3).Any<int>())
          return;
        context.RequestContext.GetService<ITestResultsService>().DeleteTestRun((TestManagementRequestContext) context, projectName, numArray3);
      }
      else
      {
        TcmHttpClient client = context.RequestContext.GetClient<TcmHttpClient>();
        if (testRunIds == null)
          return;
        if (!((IEnumerable<int>) testRunIds).Any<int>())
          return;
        try
        {
          TcmHttpClient tcmHttpClient = client;
          DeleteTestRunRequest deleteTestRunRequest = new DeleteTestRunRequest();
          deleteTestRunRequest.TestRunIds = testRunIds;
          deleteTestRunRequest.ProjectName = projectName;
          CancellationToken cancellationToken = new CancellationToken();
          tcmHttpClient.DeleteTestRunIdsAsync(deleteTestRunRequest, cancellationToken: cancellationToken)?.Wait();
        }
        catch (AggregateException ex)
        {
          if (ex.InnerException != null)
            throw ex.InnerException;
          throw;
        }
      }
    }

    public List<int> CreateLogEntriesForRun(
      TfsTestManagementRequestContext context,
      string projectName,
      int testRunId,
      Microsoft.TeamFoundation.TestManagement.Server.TestMessageLogEntry[] logEntries)
    {
      if (!context.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFSLegacy"))
      {
        TCMServiceDataMigrationRestHelper.BlockRequestsIfDataMigrationInProgressOrFailed(context.RequestContext, testRunId);
        List<int> intList = new List<int>();
        ILegacyTCMServiceHelper tcmServiceHelper = context.LegacyTcmServiceHelper;
        IVssRequestContext requestContext = context.RequestContext;
        CreateTestMessageLogEntryRequest logEntryRequest = new CreateTestMessageLogEntryRequest();
        logEntryRequest.ProjectName = projectName;
        logEntryRequest.TestMessageLogEntry = ((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestMessageLogEntry>) logEntries).Select<Microsoft.TeamFoundation.TestManagement.Server.TestMessageLogEntry, Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestMessageLogEntry>((Func<Microsoft.TeamFoundation.TestManagement.Server.TestMessageLogEntry, Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestMessageLogEntry>) (log => TestMessageLogEntryCoverter.Convert(log))).ToArray<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestMessageLogEntry>();
        logEntryRequest.TestRunId = testRunId;
        ref List<int> local = ref intList;
        return tcmServiceHelper.TryCreateLogEntries(requestContext, logEntryRequest, out local) ? intList : TestRun.CreateLogEntriesForRun((TestManagementRequestContext) context, testRunId, logEntries, projectName);
      }
      TcmHttpClient tcmHttpClient = context.RequestContext.GetClient<TcmHttpClient>();
      return TestManagementController.InvokeAction<List<int>>((Func<List<int>>) (() =>
      {
        TcmHttpClient tcmHttpClient1 = tcmHttpClient;
        CreateTestMessageLogEntryRequest request = new CreateTestMessageLogEntryRequest();
        request.ProjectName = projectName;
        request.TestMessageLogEntry = ((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestMessageLogEntry>) logEntries).Select<Microsoft.TeamFoundation.TestManagement.Server.TestMessageLogEntry, Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestMessageLogEntry>((Func<Microsoft.TeamFoundation.TestManagement.Server.TestMessageLogEntry, Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestMessageLogEntry>) (log => TestMessageLogEntryCoverter.Convert(log))).ToArray<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestMessageLogEntry>();
        request.TestRunId = testRunId;
        CancellationToken cancellationToken = new CancellationToken();
        return tcmHttpClient1.CreateTestMessageLogEntriesAsync(request, cancellationToken: cancellationToken)?.Result;
      }));
    }

    public List<Microsoft.TeamFoundation.TestManagement.Server.TestMessageLogEntry> QueryLogEntriesForRun(
      TfsTestManagementRequestContext context,
      string projectName,
      int testRunId,
      int testMessageLogId)
    {
      List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestMessageLogEntry> source = new List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestMessageLogEntry>();
      if (!context.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFSLegacy"))
      {
        ILegacyTCMServiceHelper tcmServiceHelper = context.LegacyTcmServiceHelper;
        IVssRequestContext requestContext = context.RequestContext;
        QueryTestMessageLogEntryRequest logEntryRequest = new QueryTestMessageLogEntryRequest();
        logEntryRequest.ProjectName = projectName;
        logEntryRequest.TestMessageLogId = testMessageLogId;
        logEntryRequest.TestRunId = testRunId;
        ref List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestMessageLogEntry> local = ref source;
        return tcmServiceHelper.TryQueryLogEntries(requestContext, logEntryRequest, out local) ? source.Select<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestMessageLogEntry, Microsoft.TeamFoundation.TestManagement.Server.TestMessageLogEntry>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestMessageLogEntry, Microsoft.TeamFoundation.TestManagement.Server.TestMessageLogEntry>) (logEntry => TestMessageLogEntryCoverter.Convert(logEntry))).ToList<Microsoft.TeamFoundation.TestManagement.Server.TestMessageLogEntry>() : TestRun.QueryLogEntriesForRun((TestManagementRequestContext) context, testRunId, testMessageLogId, projectName);
      }
      TcmHttpClient tcmHttpClient = context.RequestContext.GetClient<TcmHttpClient>();
      return TestManagementController.InvokeAction<List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestMessageLogEntry>>((Func<List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestMessageLogEntry>>) (() =>
      {
        TcmHttpClient tcmHttpClient1 = tcmHttpClient;
        QueryTestMessageLogEntryRequest request = new QueryTestMessageLogEntryRequest();
        request.ProjectName = projectName;
        request.TestMessageLogId = testMessageLogId;
        request.TestRunId = testRunId;
        CancellationToken cancellationToken = new CancellationToken();
        return tcmHttpClient1.QueryTestMessageLogEntryAsync(request, cancellationToken: cancellationToken)?.Result;
      })).Select<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestMessageLogEntry, Microsoft.TeamFoundation.TestManagement.Server.TestMessageLogEntry>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestMessageLogEntry, Microsoft.TeamFoundation.TestManagement.Server.TestMessageLogEntry>) (logEntry => TestMessageLogEntryCoverter.Convert(logEntry))).ToList<Microsoft.TeamFoundation.TestManagement.Server.TestMessageLogEntry>();
    }
  }
}
