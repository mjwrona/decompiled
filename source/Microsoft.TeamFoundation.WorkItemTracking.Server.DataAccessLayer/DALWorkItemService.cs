// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DALWorkItemService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class DALWorkItemService : IVssFrameworkService
  {
    private ConcurrentDictionary<string, MetadataDbStampedCacheEntry<IEnumerable<string>>> m_categoryMembersCache = new ConcurrentDictionary<string, MetadataDbStampedCacheEntry<IEnumerable<string>>>((IEqualityComparer<string>) TFStringComparer.WorkItemCategoryName);

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    internal virtual IEnumerable<WorkItemFieldData> GetAllWorkItemsByWiql(
      IVssRequestContext requestContext,
      string wiql,
      IDictionary wiqlMacros,
      IEnumerable<FieldEntry> fields,
      DateTime? startDay = null,
      QuerySortOrderEntry[] sortFields = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<string>(wiql, nameof (wiql));
      ArgumentUtility.CheckForNull<IEnumerable<FieldEntry>>(fields, "Fields");
      QueryResult queryResult = requestContext.GetService<IWorkItemQueryService>().ExecuteQuery(requestContext, wiql, wiqlMacros);
      return requestContext.GetService<ITeamFoundationWorkItemService>().GetWorkItemFieldValues(requestContext, queryResult.WorkItemIds, fields.Select<FieldEntry, string>((Func<FieldEntry, string>) (f => f.ReferenceName))).Where<WorkItemFieldData>((Func<WorkItemFieldData, bool>) (fd => !startDay.HasValue || fd.ModifiedDate >= startDay.Value));
    }

    private static IEnumerable<FieldEntry> OrganizeFieldEntries(
      IVssRequestContext requestContext,
      IEnumerable<FieldEntry> fields,
      out IOrderedEnumerable<FieldEntry> nonIdFields)
    {
      Dictionary<int, FieldEntry> dictionary = fields.Distinct<FieldEntry>().ToDictionary<FieldEntry, int>((Func<FieldEntry, int>) (x => x.FieldId));
      IFieldTypeDictionary fieldsSnapshot = DALWorkItemService.GetFieldsSnapshot(requestContext);
      if (!dictionary.ContainsKey(-3))
        dictionary.Add(-3, fieldsSnapshot.GetField(-3));
      if (!dictionary.ContainsKey(-2))
        dictionary.Add(-2, fieldsSnapshot.GetField(-2));
      List<FieldEntry> fieldEntryList = new List<FieldEntry>();
      fieldEntryList.Add(dictionary[-3]);
      fieldEntryList.Add(dictionary[-2]);
      nonIdFields = dictionary.Where<KeyValuePair<int, FieldEntry>>((Func<KeyValuePair<int, FieldEntry>, bool>) (x => x.Key != -3 && x.Key != -2)).Select<KeyValuePair<int, FieldEntry>, FieldEntry>((Func<KeyValuePair<int, FieldEntry>, FieldEntry>) (x => x.Value)).OrderBy<FieldEntry, string>((Func<FieldEntry, string>) (x => x.ReferenceName));
      fieldEntryList.AddRange((IEnumerable<FieldEntry>) nonIdFields);
      return (IEnumerable<FieldEntry>) fieldEntryList;
    }

    protected virtual IPermissionCheckHelper GetPermissionHelper(IVssRequestContext requestContext) => (IPermissionCheckHelper) new PermissionCheckHelper(requestContext);

    private static IFieldTypeDictionary GetFieldsSnapshot(IVssRequestContext requestContext) => requestContext.GetService<WorkItemTrackingFieldService>().GetFieldsSnapshot(requestContext);
  }
}
