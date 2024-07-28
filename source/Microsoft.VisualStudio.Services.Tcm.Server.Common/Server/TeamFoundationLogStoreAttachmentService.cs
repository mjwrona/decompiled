// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TeamFoundationLogStoreAttachmentService
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TeamFoundationLogStoreAttachmentService : 
    TeamFoundationTestManagementService,
    ITeamFoundationLogStoreAttachmentService,
    IVssFrameworkService
  {
    public TeamFoundationLogStoreAttachmentService()
    {
    }

    public TeamFoundationLogStoreAttachmentService(TestManagementRequestContext requestContext)
      : base(requestContext)
    {
    }

    public List<TestResultAttachment> GetLogStoreAttachments(
      TestManagementRequestContext context,
      TeamProjectReference projectRef,
      int testRunId,
      int testResultId,
      int subResultId,
      bool shouldFallBackToOldAttachment = false)
    {
      return this.ExecuteAction<List<TestResultAttachment>>(context.RequestContext, "TeamFoundationLogStoreAttachmentService.GetLogStoreAttachments", (Func<List<TestResultAttachment>>) (() =>
      {
        using (PerfManager.Measure(context.RequestContext, "BusinessLayer", nameof (GetLogStoreAttachments)))
        {
          GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectRef.Name);
          List<TestResultAttachment> storeAttachments = new List<TestResultAttachment>();
          using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
            storeAttachments = managementDatabase.QueryLogStoreAttachments(context, projectFromName.GuidId, testRunId, testResultId, subResultId, shouldFallBackToOldAttachment);
          return storeAttachments;
        }
      }), 1015680, "TestManagement", "LogStorage");
    }

    public long[] CreateLogStoreAttachmentMapping(
      TestManagementRequestContext context,
      TeamProjectReference projectRef,
      TestResultAttachment[] attachments)
    {
      return this.ExecuteAction<long[]>(context.RequestContext, "TeamFoundationLogStoreAttachmentService.CreateLogStoreAttachmentMapping", (Func<long[]>) (() =>
      {
        using (PerfManager.Measure(context.RequestContext, "BusinessLayer", nameof (CreateLogStoreAttachmentMapping)))
        {
          if (attachments == null || attachments.Length == 0)
            return (long[]) null;
          GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectRef.Name);
          List<long> longList = new List<long>();
          Dictionary<TestCaseResultIdentifier, List<long>> dictionary = new Dictionary<TestCaseResultIdentifier, List<long>>();
          using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
            dictionary = managementDatabase.CreateLogStoreAttachmentMapping(projectFromName.GuidId, (IEnumerable<TestResultAttachment>) attachments);
          foreach (List<long> collection in dictionary.Values)
            longList.AddRange((IEnumerable<long>) collection);
          longList.Sort();
          return longList.ToArray();
        }
      }), 1015680, "TestManagement", "LogStorage");
    }
  }
}
