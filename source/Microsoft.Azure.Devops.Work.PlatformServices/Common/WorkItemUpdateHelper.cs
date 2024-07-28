// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Work.PlatformServices.Common.WorkItemUpdateHelper
// Assembly: Microsoft.Azure.Devops.Work.PlatformServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7C8E511A-CB9A-4327-9803-A1164853E0F0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Devops.Work.PlatformServices.dll

using Microsoft.Azure.Boards.WebApi.Common.Converters;
using Microsoft.Azure.Boards.WebApi.Common.Helpers;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Devops.Work.PlatformServices.Common
{
  public class WorkItemUpdateHelper
  {
    public static Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem UpdateWorkItem(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem serverWorkItem,
      PatchDocument<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> document,
      bool useLegacyLinkHandling,
      bool validateOnly = false,
      bool bypassRules = false,
      bool suppressNotifications = false,
      bool includeInRecentActivity = false,
      bool useWorkItemIdentity = false,
      bool returnProjectScopedUrl = true,
      WorkItemExpand expand = WorkItemExpand.Relations)
    {
      bool isNew = serverWorkItem.Id <= 0;
      bool flag1 = serverWorkItem.WorkItemType == "Test Plan";
      WorkItemPatchDocument itemPatchDocument = new WorkItemPatchDocument(requestContext.WitContext(), useLegacyLinkHandling, (IPatchDocument<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>) document, serverWorkItem, useWorkItemIdentity, returnProjectScopedUrl, false);
      itemPatchDocument.Evaluate();
      WorkItemUpdateRuleExecutionMode ruleExecutionMode = WorkItemUpdateRuleExecutionMode.Full;
      if (bypassRules)
        ruleExecutionMode = WorkItemUpdateRuleExecutionMode.Bypass;
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate workItemUpdate1 = itemPatchDocument.GetWorkItemUpdate(requestContext.WitContext(), useWorkItemIdentity);
      if (workItemUpdate1.ResourceLinkUpdates.Count<WorkItemResourceLinkUpdate>() > 0)
      {
        foreach (WorkItemResourceLinkUpdate resourceLinkUpdate in workItemUpdate1.ResourceLinkUpdates)
        {
          ResourceLinkType? type = resourceLinkUpdate.Type;
          ResourceLinkType resourceLinkType = ResourceLinkType.Attachment;
          if (type.GetValueOrDefault() == resourceLinkType & type.HasValue)
          {
            int? length = resourceLinkUpdate.Length;
            int num = 0;
            if (length.GetValueOrDefault() == num & length.HasValue)
            {
              ITeamFoundationWorkItemAttachmentService service1 = requestContext.GetService<ITeamFoundationWorkItemAttachmentService>();
              ITeamFoundationFileService service2 = requestContext.GetService<ITeamFoundationFileService>();
              IVssRequestContext requestContext1 = requestContext;
              Guid? projectId = new Guid?();
              Guid attachmentId = new Guid(resourceLinkUpdate.Location);
              ISecuredObject securedObject;
              ref ISecuredObject local = ref securedObject;
              int attachmentTfsFileId = service1.GetAttachmentTfsFileId(requestContext1, projectId, attachmentId, out local);
              resourceLinkUpdate.Length = new int?((int) (service2.GetFileStatistics(requestContext, (long) attachmentTfsFileId) ?? throw new AttachmentNotFoundException(attachmentTfsFileId)).FileLength);
            }
          }
        }
      }
      ITeamFoundationWorkItemService service3 = requestContext.GetService<ITeamFoundationWorkItemService>();
      IVssRequestContext requestContext2 = requestContext;
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate workItemUpdate2 = workItemUpdate1;
      int num1 = (int) ruleExecutionMode;
      bool flag2 = suppressNotifications;
      int num2 = validateOnly ? 1 : 0;
      int num3 = includeInRecentActivity ? 1 : 0;
      int num4 = flag2 ? 1 : 0;
      int num5 = bypassRules ? 1 : 0;
      int num6 = useWorkItemIdentity ? 1 : 0;
      WorkItemUpdateResult itemUpdateResult = service3.UpdateWorkItem(requestContext2, workItemUpdate2, (WorkItemUpdateRuleExecutionMode) num1, num2 != 0, num3 != 0, num4 != 0, num5 != 0, num6 != 0);
      if (itemUpdateResult.Exception != null)
        throw WitUpdateHelper.TranslateUpdateResultException((Exception) itemUpdateResult.Exception, true);
      if (isNew & flag1)
      {
        CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
        CustomerIntelligenceService service4 = requestContext.GetService<CustomerIntelligenceService>();
        intelligenceData.Add("Test plan WIT", (double) itemUpdateResult.Id);
        IVssRequestContext requestContext3 = requestContext;
        string workItemTracking = CustomerIntelligenceArea.WorkItemTracking;
        string testPlanWitApi = CustomerIntelligenceFeature.TestPlanWITApi;
        CustomerIntelligenceData properties = intelligenceData;
        service4.Publish(requestContext3, workItemTracking, testPlanWitApi, properties);
      }
      bool includeRelations = expand == WorkItemExpand.All || expand == WorkItemExpand.Relations;
      bool includeLinks = true;
      bool expandFields = expand == WorkItemExpand.All || expand == WorkItemExpand.Fields;
      if (!validateOnly)
        return WitUpdateHelper.GetWorkItem(requestContext, serverWorkItem.Id, isNew, false, useWorkItemIdentity, returnProjectScopedUrl, includeRelations, includeLinks, expandFields);
      foreach (KeyValuePair<string, object> field in (IEnumerable<KeyValuePair<string, object>>) itemUpdateResult.Fields)
        serverWorkItem.SetFieldValue(requestContext, field.Key, field.Value);
      WorkItemTrackingRequestContext witRequestContext = requestContext.WitContext();
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem workItem = serverWorkItem;
      int num7 = includeLinks ? 1 : 0;
      bool flag3 = useWorkItemIdentity;
      bool flag4 = returnProjectScopedUrl;
      int num8 = expandFields ? 1 : 0;
      int num9 = flag3 ? 1 : 0;
      int num10 = flag4 ? 1 : 0;
      return WorkItemFactory.Create(witRequestContext, (WorkItemRevision) workItem, false, num7 != 0, num8 != 0, false, false, (IEnumerable<string>) null, false, false, (IDictionary<Guid, IdentityReference>) null, num9 != 0, num10 != 0, true);
    }
  }
}
