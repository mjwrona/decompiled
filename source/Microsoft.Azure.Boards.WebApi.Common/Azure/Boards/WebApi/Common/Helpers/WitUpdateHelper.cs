// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.WebApi.Common.Helpers.WitUpdateHelper
// Assembly: Microsoft.Azure.Boards.WebApi.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC99C479-6852-4E74-BCA4-2660760F9D83
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.WebApi.Common.dll

using Microsoft.Azure.Boards.WebApi.Common.Converters;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Boards.WebApi.Common.Helpers
{
  public static class WitUpdateHelper
  {
    public static Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem PrepareUpdateWorkItem(
      IVssRequestContext requestContext,
      string projectNameOrGuid,
      string type,
      PatchDocument<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> document,
      bool useWorkItemIdentity = false)
    {
      if (document == null)
        throw new VssPropertyValidationException(nameof (document), ResourceStrings.MissingPatchDocument());
      string projectName = projectNameOrGuid;
      Guid result;
      if (Guid.TryParse(projectNameOrGuid, out result))
        projectName = requestContext.GetService<IProjectService>().GetProject(requestContext, result).Name;
      return requestContext.GetService<ITeamFoundationWorkItemService>().GetWorkItemTemplate(requestContext, projectName, type, useWorkItemIdentity);
    }

    public static Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem PrepareUpdateWorkItem(
      IVssRequestContext requestContext,
      int id,
      PatchDocument<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> document,
      bool useWorkItemIdentity = false)
    {
      if (document == null)
        throw new VssPropertyValidationException(nameof (document), ResourceStrings.MissingPatchDocument());
      ITeamFoundationWorkItemService service = requestContext.GetService<ITeamFoundationWorkItemService>();
      try
      {
        return service.GetWorkItemById(requestContext, id, includeHistory: false, useWorkItemIdentity: useWorkItemIdentity);
      }
      catch (WorkItemUnauthorizedAccessException ex)
      {
        throw new WorkItemUnauthorizedAccessException(id, AccessType.Read);
      }
    }

    public static IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem> PrepareUpdateWorkItems(
      IVssRequestContext requestContext,
      IEnumerable<int> ids,
      bool useWorkItemIdentity = false)
    {
      if (ids == null)
        throw new ArgumentNullException(nameof (ids));
      return WitUpdateHelper.GetServerWorkItems(requestContext, ids, false, useWorkItemIdentity);
    }

    public static IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> GetWorkItems(
      IVssRequestContext requestContext,
      IEnumerable<int> ids,
      bool includeHistory,
      bool returnIdentityRef = false,
      bool returnProjectScopedUrl = true,
      bool includeRelations = true,
      bool includeLinks = true,
      bool expandFields = false)
    {
      if (ids == null)
        throw new ArgumentNullException(nameof (ids));
      return requestContext.GetService<ITeamFoundationWorkItemService>().GetWorkItems(requestContext, ids, 32, includeRelations, includeRelations, includeHistory, useWorkItemIdentity: returnIdentityRef).Select<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem, Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem, Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>) (wi =>
      {
        WorkItemTrackingRequestContext witRequestContext = requestContext.WitContext();
        Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem workItem = wi;
        int num1 = includeRelations ? 1 : 0;
        int num2 = includeLinks ? 1 : 0;
        bool flag1 = returnIdentityRef;
        bool flag2 = returnProjectScopedUrl;
        int num3 = expandFields ? 1 : 0;
        int num4 = flag1 ? 1 : 0;
        int num5 = flag2 ? 1 : 0;
        return WorkItemFactory.Create(witRequestContext, (WorkItemRevision) workItem, num1 != 0, num2 != 0, num3 != 0, false, false, (IEnumerable<string>) null, false, false, (IDictionary<Guid, IdentityReference>) null, num4 != 0, num5 != 0, true);
      }));
    }

    public static IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem> GetServerWorkItems(
      IVssRequestContext requestContext,
      IEnumerable<int> ids,
      bool includeHistory,
      bool returnIdentityRef = false)
    {
      if (ids == null)
        throw new ArgumentNullException(nameof (ids));
      return requestContext.GetService<ITeamFoundationWorkItemService>().GetWorkItems(requestContext, ids, 32, includeHistory: includeHistory, useWorkItemIdentity: returnIdentityRef);
    }

    public static Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem GetWorkItem(
      IVssRequestContext requestContext,
      int id,
      bool isNew,
      bool includeHistory,
      bool returnIdentityRef = false,
      bool returnProjectScopedUrl = true,
      bool includeRelations = true,
      bool includeLinks = true,
      bool expandFields = false)
    {
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem workItemById = requestContext.GetService<ITeamFoundationWorkItemService>().GetWorkItemById(requestContext, id, isNew ? 32 : 16, includeRelations, includeRelations, includeHistory, useWorkItemIdentity: returnIdentityRef);
      WorkItemTrackingRequestContext witRequestContext = requestContext.WitContext();
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem workItem = workItemById;
      int num1 = includeRelations ? 1 : 0;
      int num2 = includeLinks ? 1 : 0;
      bool flag1 = returnIdentityRef;
      bool flag2 = returnProjectScopedUrl;
      int num3 = expandFields ? 1 : 0;
      int num4 = flag1 ? 1 : 0;
      int num5 = flag2 ? 1 : 0;
      return WorkItemFactory.Create(witRequestContext, (WorkItemRevision) workItem, num1 != 0, num2 != 0, num3 != 0, false, false, (IEnumerable<string>) null, false, false, (IDictionary<Guid, IdentityReference>) null, num4 != 0, num5 != 0, true);
    }

    public static Exception TranslateUpdateResultException(
      Exception exception,
      bool shouldReturnAllRuleValidationErrorsFromREST = false)
    {
      if (exception is WorkItemUnauthorizedAccessException)
        return (Exception) new PermissionDeniedException(exception.Message);
      if (exception is WorkItemTrackingAggregateException aggregateException)
      {
        if (shouldReturnAllRuleValidationErrorsFromREST)
        {
          List<RuleValidationException> validationExceptionList = new List<RuleValidationException>();
          List<TeamFoundationServiceException> source = new List<TeamFoundationServiceException>();
          foreach (TeamFoundationServiceException allException in aggregateException.AllExceptions)
          {
            if (allException is RuleValidationException validationException)
              validationExceptionList.Add(validationException);
            else
              source.Add(allException);
          }
          if (source.Any<TeamFoundationServiceException>())
            return (Exception) source.First<TeamFoundationServiceException>();
          if (!validationExceptionList.Any<RuleValidationException>())
            return exception;
          string message1 = validationExceptionList.First<RuleValidationException>().Message;
          if (!message1.EndsWith("."))
            message1 += ".";
          string message2;
          if (validationExceptionList.Count > 2)
          {
            message2 = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.RuleValidationErrors_MultipleAdditional((object) message1, (object) (validationExceptionList.Count - 1));
          }
          else
          {
            if (validationExceptionList.Count != 2)
              return (Exception) validationExceptionList.First<RuleValidationException>();
            message2 = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.RuleValidationErrors_OneAdditional((object) message1);
          }
          return (Exception) new RuleValidationException(message2, (IEnumerable<RuleValidationException>) validationExceptionList);
        }
        TeamFoundationServiceException serviceException;
        if (aggregateException == null)
        {
          serviceException = (TeamFoundationServiceException) null;
        }
        else
        {
          IEnumerable<TeamFoundationServiceException> allExceptions = aggregateException.AllExceptions;
          serviceException = allExceptions != null ? allExceptions.FirstOrDefault<TeamFoundationServiceException>() : (TeamFoundationServiceException) null;
        }
        return (Exception) serviceException ?? exception;
      }
      if (!shouldReturnAllRuleValidationErrorsFromREST || !(exception is RuleValidationException validationException1))
        return exception;
      return (Exception) new RuleValidationException(validationException1.Message, (IEnumerable<RuleValidationException>) new RuleValidationException[1]
      {
        validationException1
      });
    }
  }
}
