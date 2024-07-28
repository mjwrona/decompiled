// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TeamFoundationAttachmentService
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TeamFoundationAttachmentService : 
    TeamFoundationTestManagementService,
    ITeamFoundationAttachmentService,
    IVssFrameworkService
  {
    public TeamFoundationAttachmentService()
    {
    }

    public TeamFoundationAttachmentService(TestManagementRequestContext requestContext)
      : base(requestContext)
    {
    }

    public List<TestResultAttachment> GetAttachmentsFromDb(
      TestManagementRequestContext context,
      TeamProjectReference projectRef,
      int testRunId,
      int testResultId,
      int attachmentId,
      int subResultId,
      bool shouldFallBackToOldAttachment = false)
    {
      return this.ExecuteAction<List<TestResultAttachment>>(context.RequestContext, "TeamFoundationLogStoreAttachmentService.GetAttachmentsFromDb", (Func<List<TestResultAttachment>>) (() =>
      {
        using (PerfManager.Measure(context.RequestContext, "BusinessLayer", nameof (GetAttachmentsFromDb)))
        {
          GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectRef.Name);
          List<TestResultAttachment> source = new List<TestResultAttachment>();
          if (context.RequestContext.IsFeatureEnabled("TestManagement.Server.UseIterationIdInLogStoreAttachmentMapper"))
          {
            using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
              source = managementDatabase.QueryAttachmentsV2(context, projectFromName.GuidId, testRunId, testResultId, attachmentId, subResultId, shouldFallBackToOldAttachment);
          }
          else
          {
            using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
              source = managementDatabase.QueryUploadedAttachments(context, projectFromName.GuidId, testRunId, testResultId, attachmentId, subResultId, shouldFallBackToOldAttachment);
          }
          context.RequestContext.Trace(1015921, TraceLevel.Info, "TestManagement", "AttachmentHandler", "No of attachments found for TestRunId: {0}, TestResultId: {1},TestSubResultId: {2} are {3}", (object) testRunId, (object) testResultId, (object) subResultId, (object) source.Count<TestResultAttachment>());
          return source;
        }
      }), 1015921, "TestManagement", "LogStorage");
    }

    public List<TestResultAttachment> GetIterationAttachmentsFromDb(
      TestManagementRequestContext context,
      TeamProjectReference projectRef,
      int testRunId,
      int testResultId,
      int attachmentId,
      int iterationId)
    {
      return this.ExecuteAction<List<TestResultAttachment>>(context.RequestContext, "TeamFoundationLogStoreAttachmentService.GetIterationAttachmentsFromDb", (Func<List<TestResultAttachment>>) (() =>
      {
        using (PerfManager.Measure(context.RequestContext, "BusinessLayer", nameof (GetIterationAttachmentsFromDb)))
        {
          GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectRef.Name);
          List<TestResultAttachment> source = new List<TestResultAttachment>();
          using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
            source = managementDatabase.QueryIterationAttachments(context, projectFromName.GuidId, testRunId, testResultId, attachmentId, iterationId);
          context.RequestContext.Trace(1015921, TraceLevel.Info, "TestManagement", "AttachmentHandler", "GetIterationAttachmentsFromDb: No. of attachments found for TestRunId: {0}, TestResultId: {1}, IterationId: {2} are {3}", (object) testRunId, (object) testResultId, (object) iterationId, (object) source.Count<TestResultAttachment>());
          return source;
        }
      }), 1015921, "TestManagement", "LogStorage");
    }

    public int[] CreateAttachmentMapping(
      TestManagementRequestContext context,
      TeamProjectReference projectRef,
      TestResultAttachment[] attachments)
    {
      return this.ExecuteAction<int[]>(context.RequestContext, "TeamFoundationAttachmentService.CreateAttachmentMapping", (Func<int[]>) (() =>
      {
        using (PerfManager.Measure(context.RequestContext, "BusinessLayer", nameof (CreateAttachmentMapping)))
        {
          if (attachments == null || attachments.Length == 0)
            return (int[]) null;
          GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectRef.Name);
          List<int> source = new List<int>();
          Dictionary<TestCaseResultIdentifier, List<int>> dictionary = new Dictionary<TestCaseResultIdentifier, List<int>>();
          if (context.RequestContext.IsFeatureEnabled("TestManagement.Server.UseIterationIdInLogStoreAttachmentMapper"))
          {
            using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
            {
              dictionary = managementDatabase.CreateAttachmentMappingV2(projectFromName.GuidId, (IEnumerable<TestResultAttachment>) attachments);
              context.RequestContext.Trace(1015920, TraceLevel.Info, "TestManagement", "AttachmentHandler", "Attachments are created in the new flow, containing iterationId");
            }
          }
          else
          {
            using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
            {
              dictionary = managementDatabase.CreateAttachmentMapping(projectFromName.GuidId, (IEnumerable<TestResultAttachment>) attachments);
              context.RequestContext.Trace(1015920, TraceLevel.Info, "TestManagement", "AttachmentHandler", "Attachments are created in the new flow");
            }
          }
          foreach (List<int> collection in dictionary.Values)
            source.AddRange((IEnumerable<int>) collection);
          context.RequestContext.Trace(1015920, TraceLevel.Info, "TestManagement", "AttachmentHandler", "No of Attachments created in the new flow : {0} ", (object) source.Count<int>());
          source.Sort();
          return source.ToArray();
        }
      }), 1015920, "TestManagement", "LogStorage");
    }
  }
}
