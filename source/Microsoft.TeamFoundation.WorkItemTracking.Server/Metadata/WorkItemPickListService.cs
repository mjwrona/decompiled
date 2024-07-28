// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemPickListService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.AuditLog;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  public class WorkItemPickListService : IWorkItemPickListService, IVssFrameworkService
  {
    private static readonly Regex s_leadingNumbersCheck = new Regex("^\\d+", RegexOptions.Compiled, CommonWorkItemTrackingConstants.RegexMatchTimeout);

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public virtual WorkItemPickList GetList(
      IVssRequestContext requestContext,
      Guid listId,
      bool bypassCache = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(listId, nameof (listId));
      return requestContext.TraceBlock<WorkItemPickList>(909901, 909902, "PickLists", "ProcessWorkItemPickListService", nameof (GetList), (Func<WorkItemPickList>) (() =>
      {
        WorkItemPickList picklist1 = (WorkItemPickList) null;
        if (bypassCache)
        {
          WorkItemPickListRecord picklist2 = (WorkItemPickListRecord) null;
          using (WorkItemPickListComponent component = requestContext.CreateComponent<WorkItemPickListComponent>())
            picklist2 = component.GetList(listId);
          picklist1 = picklist2 != null ? WorkItemPickList.Create(picklist2) : throw new WorkItemPickListNotFoundException(listId);
        }
        else if (!this.TryGetList(requestContext, listId, out picklist1))
          throw new WorkItemPickListNotFoundException(listId);
        return picklist1;
      }));
    }

    public bool TryGetList(
      IVssRequestContext requestContext,
      Guid listId,
      out WorkItemPickList picklist)
    {
      return requestContext.GetService<WorkItemPickListCacheService>().TryGetPicklist(requestContext, listId, out picklist);
    }

    public IReadOnlyCollection<WorkItemPickListMetadata> GetListsMetadata(
      IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return requestContext.TraceBlock<IReadOnlyCollection<WorkItemPickListMetadata>>(909903, 909904, "PickLists", "ProcessWorkItemPickListService", nameof (GetListsMetadata), (Func<IReadOnlyCollection<WorkItemPickListMetadata>>) (() =>
      {
        IReadOnlyCollection<WorkItemPickListMetadata> metadata = (IReadOnlyCollection<WorkItemPickListMetadata>) null;
        if (!requestContext.GetService<WorkItemPickListCacheService>().TryGetPicklistMetadata(requestContext, out metadata))
          throw new WorkItemPickListMetadataNotFoundException();
        return metadata;
      }));
    }

    public WorkItemPickList CreateList(
      IVssRequestContext requestContext,
      string name,
      WorkItemPickListType type,
      IReadOnlyList<string> items,
      bool isSuggested = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(name, nameof (name));
      ArgumentUtility.CheckForNull<IReadOnlyList<string>>(items, nameof (items));
      return requestContext.TraceBlock<WorkItemPickList>(909905, 909906, "PickLists", "ProcessWorkItemPickListService", nameof (CreateList), (Func<WorkItemPickList>) (() =>
      {
        items = this.GetProcessedAndValidatedListItems(requestContext, items, type, name);
        name = this.GetProcessedAndValidatedListName(name);
        WorkItemPickListService.CheckEditPermission(requestContext);
        int listsPerCollection = requestContext.WitContext().TemplateValidatorConfiguration.MaxPickListsPerCollection;
        IReadOnlyCollection<WorkItemPickListMetadata> listsMetadata = this.GetListsMetadata(requestContext);
        if (listsMetadata.Count >= listsPerCollection)
          throw new WorkItemPickListCountLimitExceededException(listsPerCollection);
        if (listsMetadata.Any<WorkItemPickListMetadata>((Func<WorkItemPickListMetadata, bool>) (l => TFStringComparer.WorkItemFieldFriendlyName.Equals(l.Name, name))))
          throw new WorkItemPickListItemNameAlreadyInUseException(name);
        Guid id = requestContext.WitContext().RequestIdentity.Id;
        WorkItemPickListRecord list;
        using (WorkItemPickListComponent component = requestContext.CreateComponent<WorkItemPickListComponent>())
          list = component.CreateList(name, type, items, id, isSuggested);
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add("CreatedPickList", (object) list.Id);
        properties.Add("CreatedPickListName", name);
        properties.Add("PickListMemberCount", (double) items.Count);
        properties.Add("IsSuggested", isSuggested);
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "ProcessWorkItemPickListService", nameof (CreateList), properties);
        return WorkItemPickList.Create(list);
      }));
    }

    public void DeleteList(IVssRequestContext requestContext, Guid listId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(listId, nameof (listId));
      requestContext.TraceBlock(909911, 909912, "PickLists", "ProcessWorkItemPickListService", nameof (DeleteList), (Action) (() =>
      {
        WorkItemPickListService.CheckEditPermission(requestContext);
        if (!this.TryGetList(requestContext, listId, out WorkItemPickList _))
          throw new WorkItemPickListNotFoundException(listId);
        Guid id = requestContext.WitContext().RequestIdentity.Id;
        using (WorkItemPickListComponent component = requestContext.CreateComponent<WorkItemPickListComponent>())
          component.DeleteList(listId, id);
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add("DeletedPickList", (object) listId);
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "ProcessWorkItemPickListService", nameof (DeleteList), properties);
      }));
    }

    public WorkItemPickList UpdateList(
      IVssRequestContext requestContext,
      Guid listId,
      string listName,
      IReadOnlyList<string> items,
      bool isSuggested = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(listId, nameof (listId));
      ArgumentUtility.CheckForNull<IReadOnlyList<string>>(items, nameof (items));
      return requestContext.TraceBlock<WorkItemPickList>(909907, 909908, "PickLists", "ProcessWorkItemPickListService", nameof (UpdateList), (Func<WorkItemPickList>) (() =>
      {
        WorkItemPickList existingList;
        if (!this.TryGetList(requestContext, listId, out existingList))
          throw new WorkItemPickListNotFoundException(listId);
        if (string.IsNullOrWhiteSpace(listName))
          listName = existingList.Name;
        items = this.GetProcessedAndValidatedListItems(requestContext, items, existingList.Type, listName);
        listName = this.GetProcessedAndValidatedListName(listName);
        WorkItemPickListService.CheckEditPermission(requestContext);
        IReadOnlyCollection<WorkItemPickListMetadata> listsMetadata = this.GetListsMetadata(requestContext);
        if (!TFStringComparer.WorkItemFieldFriendlyName.Equals(existingList.Name, listName) && listsMetadata.Any<WorkItemPickListMetadata>((Func<WorkItemPickListMetadata, bool>) (list => TFStringComparer.WorkItemFieldFriendlyName.Equals(list.Name, listName) && list.Id != existingList.Id)))
          throw new WorkItemPickListItemNameAlreadyInUseException(listName);
        Guid id = requestContext.WitContext().RequestIdentity.Id;
        WorkItemPickListRecord picklist = (WorkItemPickListRecord) null;
        using (WorkItemPickListComponent component = requestContext.CreateComponent<WorkItemPickListComponent>())
          picklist = component.UpdateList(listId, listName, items, id, isSuggested);
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add("UpdatedPickList", (object) listId);
        if (!existingList.Items.Select<WorkItemPickListMember, string>((Func<WorkItemPickListMember, string>) (i => i.Value)).SequenceEqual<string>((IEnumerable<string>) items))
          properties.Add("PickListMemberCount", (double) items.Count);
        if (existingList.Name != listName)
          properties.Add("UpdatedPickListName", listName);
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "ProcessWorkItemPickListService", nameof (UpdateList), properties);
        if (WorkItemTrackingFeatureFlags.IsPicklistValueChangeAuditLogEnabled(requestContext))
        {
          List<string> stringList = new List<string>();
          foreach (WorkItemPickListMember itemPickListMember in (IEnumerable<WorkItemPickListMember>) existingList.Items)
            stringList.Add(itemPickListMember.Value);
          string str1 = string.Join(",", stringList.Except<string>((IEnumerable<string>) items));
          string str2 = string.Join(",", items.Except<string>((IEnumerable<string>) stringList));
          if (str1.Length > 0)
          {
            IVssRequestContext requestContext1 = requestContext;
            string processListRemoveValue = ProcessAuditConstants.ProcessListRemoveValue;
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("PicklistValue", (object) str1);
            Guid targetHostId = new Guid();
            Guid projectId = new Guid();
            requestContext1.LogAuditEvent(processListRemoveValue, data, targetHostId, projectId);
          }
          if (str2.Length > 0)
          {
            IVssRequestContext requestContext2 = requestContext;
            string processListAddValue = ProcessAuditConstants.ProcessListAddValue;
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("PicklistValue", (object) str2);
            Guid targetHostId = new Guid();
            Guid projectId = new Guid();
            requestContext2.LogAuditEvent(processListAddValue, data, targetHostId, projectId);
          }
        }
        return WorkItemPickList.Create(picklist);
      }));
    }

    private IReadOnlyList<string> GetProcessedAndValidatedListItems(
      IVssRequestContext requestContext,
      IReadOnlyList<string> items,
      WorkItemPickListType type,
      string listName)
    {
      List<string> items1 = new List<string>();
      for (int index = 0; index < items.Count; ++index)
      {
        string str = items[index]?.Trim();
        ArgumentUtility.CheckStringForNullOrWhiteSpace(str, string.Format("{0}[{1}]", (object) nameof (items), (object) index));
        switch (type)
        {
          case WorkItemPickListType.Integer:
            if (!int.TryParse(str, out int _))
              throw new ArgumentException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.InvalidListItemType((object) items[index], (object) "Integer"), nameof (items));
            break;
          case WorkItemPickListType.Double:
            if (!double.TryParse(str, out double _))
              throw new ArgumentException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.InvalidListItemType((object) items[index], (object) "Double"), nameof (items));
            break;
        }
        items1.Add(str);
      }
      CommonWITUtils.VerifyListItemsAgainstMetadataLimits(requestContext, listName, items);
      CommonWITUtils.VerifyNoDuplicates((IEnumerable<string>) items1);
      return (IReadOnlyList<string>) items1;
    }

    private string GetProcessedAndValidatedListName(string name)
    {
      string str = name.Trim();
      char[] array = ((IEnumerable<char>) WorkItemTypeMetadata.IllegalNameChars).Where<char>((Func<char, bool>) (c => c != '-')).ToArray<char>();
      CommonWITUtils.CheckValidName(str, 128, array);
      return !WorkItemPickListService.s_leadingNumbersCheck.IsMatch(str) ? str : throw new ArgumentException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.LeadingNumbers(), "listName");
    }

    private static void CheckEditPermission(IVssRequestContext requestContext)
    {
      ITeamFoundationProcessService service = requestContext.GetService<ITeamFoundationProcessService>();
      if (!service.HasProcessPermission(requestContext, 1))
      {
        foreach (ProcessDescriptor processDescriptor in (IEnumerable<ProcessDescriptor>) service.GetProcessDescriptors(requestContext))
        {
          if (service.HasProcessPermission(requestContext, 1, processDescriptor))
            return;
        }
        throw new WorkItemPicklistPermissionException();
      }
    }
  }
}
