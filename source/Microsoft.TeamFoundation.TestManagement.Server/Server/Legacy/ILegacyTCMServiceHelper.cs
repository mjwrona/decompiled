// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Legacy.ILegacyTCMServiceHelper
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.TeamFoundation.TestManagement.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Tcm.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server.Legacy
{
  public interface ILegacyTCMServiceHelper
  {
    bool TryDeleteTestRunIds(IVssRequestContext context, DeleteTestRunRequest deleteTestRunRequest);

    bool TryCreateLogEntries(
      IVssRequestContext context,
      CreateTestMessageLogEntryRequest logEntryRequest,
      out List<int> logs);

    bool TryQueryLogEntries(
      IVssRequestContext context,
      QueryTestMessageLogEntryRequest logEntryRequest,
      out List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestMessageLogEntry> logEntries);

    bool TryCreateBlockedResults(
      IVssRequestContext context,
      GuidAndString projectId,
      List<LegacyTestCaseResult> testCaseResults,
      out List<LegacyTestCaseResult> newTestCaseResults);

    bool TryGetTestSettingsCompatById(
      IVssRequestContext context,
      Guid projectId,
      int testSettingsId,
      out LegacyTestSettings newTestSettings);

    bool TryCreateTestSettingsCompat(
      IVssRequestContext context,
      Guid projectId,
      LegacyTestSettings legacyTestSettings,
      out Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.UpdatedProperties newUpdateProperties);

    bool TryQueryTestSettings(
      IVssRequestContext context,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultsStoreQuery query,
      bool omitSettings,
      out List<LegacyTestSettings> newTestSettings);

    bool TryQueryTestSettingsCount(
      IVssRequestContext context,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultsStoreQuery query,
      out int? testSettingsCount);

    bool TryUpdateTestSettings(
      IVssRequestContext context,
      Guid projectId,
      LegacyTestSettings legacyTestSettings,
      out Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.UpdatedProperties newUpdateProperties);

    bool IsTestRunInTCM(IVssRequestContext requestContext, int runId, bool queryFlow = true);

    bool TryUploadAttachments(
      IVssRequestContext requestContext,
      int testRunId,
      int sessionId,
      Dictionary<string, string> requestParams,
      List<HttpPostedTcmAttachment> files);

    bool TryDownloadAttachments(
      TfsTestManagementRequestContext requestContext,
      List<int> attachmentIds,
      List<long> lengths,
      out TcmAttachment attachments);

    bool TryCreateAfnStrip(
      IVssRequestContext context,
      Guid projectId,
      AfnStrip afnStrip,
      out AfnStrip createdAfnStrip);

    bool TryGetDefaultAfnStrips(
      IVssRequestContext context,
      Guid projectId,
      IList<int> testCaseIds,
      out List<AfnStrip> afnStrips);

    bool TryFilterPoints(
      IVssRequestContext context,
      Guid projectGuid,
      FilterPointQuery pointQuery,
      out List<PointLastResult> filteredPoints);

    bool TryGetManualTestResultsByUpdatedDate(
      IVssRequestContext requestContext,
      Guid projectGuid,
      DateTime updateDate,
      int runId,
      int resultId,
      out TestResultsWithWatermark testCaseResults);
  }
}
