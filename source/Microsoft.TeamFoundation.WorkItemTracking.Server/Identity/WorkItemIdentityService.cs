// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Identity.WorkItemIdentityService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.UpdateState;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Utility;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Identity
{
  internal class WorkItemIdentityService : IWorkItemIdentityService, IVssFrameworkService
  {
    private WorkItemAadIdentityCreator m_witAadIdentityCreator;
    private int m_readBatchSizeLimit;

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext) => this.m_readBatchSizeLimit = systemRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) ? systemRequestContext.To(TeamFoundationHostType.Deployment).GetService<CachedRegistryService>().GetValue<int>(systemRequestContext, (RegistryQuery) "/Service/Integration/Settings/ReadBatchSizeLimit", 100000) : throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);

    public WorkItemIdentityService()
      : this(new WorkItemAadIdentityCreator())
    {
    }

    public WorkItemIdentityService(WorkItemAadIdentityCreator identityCreator) => this.m_witAadIdentityCreator = identityCreator;

    public ResolvedIdentityNamesInfo ResolveIdentityFields(
      WorkItemTrackingRequestContext witRequestContext,
      IEnumerable<WorkItemUpdateState> workItems,
      bool bypassRules)
    {
      ArgumentUtility.CheckForNull<WorkItemTrackingRequestContext>(witRequestContext, nameof (witRequestContext));
      ArgumentUtility.CheckForNull<IEnumerable<WorkItemUpdateState>>(workItems, nameof (workItems));
      IVssRequestContext requestContext = witRequestContext.RequestContext;
      requestContext.TraceEnter(907001, "Identity", nameof (WorkItemIdentityService), nameof (ResolveIdentityFields));
      try
      {
        List<KeyValuePair<int, object>> keyValuePairList = new List<KeyValuePair<int, object>>();
        IFieldTypeDictionary fieldTypeDictionary = requestContext.GetService<WorkItemTrackingFieldService>().GetFieldsSnapshot(requestContext);
        foreach (WorkItemUpdateState workItemUpdateState in workItems.Where<WorkItemUpdateState>((Func<WorkItemUpdateState, bool>) (us => us.Success)).ToArray<WorkItemUpdateState>())
        {
          IEnumerable<KeyValuePair<int, object>> collection = workItemUpdateState.FieldData.GetAllFieldValuesByFieldEntry(requestContext.WitContext(), true).Where<KeyValuePair<FieldEntry, object>>((Func<KeyValuePair<FieldEntry, object>, bool>) (field => fieldTypeDictionary.GetFieldByNameOrId(field.Key.FieldId.ToString()).IsIdentity)).Select<KeyValuePair<FieldEntry, object>, KeyValuePair<int, object>>((Func<KeyValuePair<FieldEntry, object>, KeyValuePair<int, object>>) (field => new KeyValuePair<int, object>(fieldTypeDictionary.GetField(field.Key.FieldId).FieldId, field.Value)));
          KeyValuePair<int, object>[] array = workItemUpdateState.FieldUpdates.Where<KeyValuePair<int, object>>((Func<KeyValuePair<int, object>, bool>) (field => fieldTypeDictionary.GetField(field.Key).IsIdentity)).ToArray<KeyValuePair<int, object>>();
          witRequestContext.RequestContext.Trace(907016, TraceLevel.Verbose, "Identity", nameof (WorkItemIdentityService), "Found identity fields \"" + string.Join<int>(",", ((IEnumerable<KeyValuePair<int, object>>) array).Select<KeyValuePair<int, object>, int>((Func<KeyValuePair<int, object>, int>) (f => f.Key))) + "\" from field updates.");
          keyValuePairList.AddRange((IEnumerable<KeyValuePair<int, object>>) array);
          keyValuePairList.AddRange(collection);
        }
        if (!keyValuePairList.Any<KeyValuePair<int, object>>())
          return new ResolvedIdentityNamesInfo();
        ResolvedIdentityNamesInfo resolvedNamesInfo = this.GetResolvedNamesInfo(witRequestContext, keyValuePairList);
        foreach (WorkItemUpdateState workItem in workItems)
        {
          IEnumerable<WorkItemFieldInvalidException> exceptions = (IEnumerable<WorkItemFieldInvalidException>) new List<WorkItemFieldInvalidException>();
          IEnumerable<KeyValuePair<int, object>> updates = this.ReplaceIdentityFieldsUpdate(witRequestContext, workItem.Id, workItem.FieldUpdates, fieldTypeDictionary, resolvedNamesInfo, bypassRules, out exceptions);
          bool markdownCommentUpdate = workItem.HasMarkdownCommentUpdate;
          workItem.ClearFieldUpdates();
          workItem.AddFieldUpdates(updates, markdownCommentUpdate);
          workItem.UpdateResult.AddExceptions((IEnumerable<TeamFoundationServiceException>) exceptions);
        }
        resolvedNamesInfo.IdentityMap = new Lazy<IDictionary<string, Microsoft.VisualStudio.Services.Identity.Identity>>((Func<IDictionary<string, Microsoft.VisualStudio.Services.Identity.Identity>>) (() => this.GetIdentityMap(requestContext, resolvedNamesInfo)));
        return resolvedNamesInfo;
      }
      finally
      {
        requestContext.TraceLeave(907002, "Identity", nameof (WorkItemIdentityService), nameof (ResolveIdentityFields));
      }
    }

    public ResolvedIdentityNamesInfo ResolveIdentityFields(
      WorkItemTrackingRequestContext witRequestContext,
      IDictionary<int, object> updates,
      IDictionary<int, object> latestValues,
      out IEnumerable<WorkItemFieldInvalidException> exceptions)
    {
      ArgumentUtility.CheckForNull<WorkItemTrackingRequestContext>(witRequestContext, nameof (witRequestContext));
      ArgumentUtility.CheckForNull<IDictionary<int, object>>(updates, nameof (updates));
      ArgumentUtility.CheckForNull<IDictionary<int, object>>(latestValues, nameof (latestValues));
      IVssRequestContext requestContext = witRequestContext.RequestContext;
      exceptions = (IEnumerable<WorkItemFieldInvalidException>) new List<WorkItemFieldInvalidException>();
      requestContext.TraceEnter(907001, "Identity", nameof (WorkItemIdentityService), nameof (ResolveIdentityFields));
      try
      {
        List<KeyValuePair<int, object>> identityFieldsList = new List<KeyValuePair<int, object>>();
        IFieldTypeDictionary fieldTypeDictionary = requestContext.GetService<WorkItemTrackingFieldService>().GetFieldsSnapshot(requestContext);
        IEnumerable<KeyValuePair<int, object>> collection1 = updates.Where<KeyValuePair<int, object>>((Func<KeyValuePair<int, object>, bool>) (field => fieldTypeDictionary.GetField(field.Key).IsIdentity));
        IEnumerable<KeyValuePair<int, object>> collection2 = latestValues.Where<KeyValuePair<int, object>>((Func<KeyValuePair<int, object>, bool>) (field => fieldTypeDictionary.GetField(field.Key).IsIdentity));
        identityFieldsList.AddRange(collection1);
        identityFieldsList.AddRange(collection2);
        ResolvedIdentityNamesInfo resolvedNamesInfo = this.GetResolvedNamesInfo(witRequestContext, identityFieldsList);
        IEnumerable<KeyValuePair<int, object>> keyValuePairs = this.ReplaceIdentityFieldsUpdate(witRequestContext, 0, (IEnumerable<KeyValuePair<int, object>>) updates, fieldTypeDictionary, resolvedNamesInfo, false, out exceptions);
        updates.Clear();
        foreach (KeyValuePair<int, object> keyValuePair in keyValuePairs)
          updates.Add(keyValuePair);
        resolvedNamesInfo.IdentityMap = new Lazy<IDictionary<string, Microsoft.VisualStudio.Services.Identity.Identity>>((Func<IDictionary<string, Microsoft.VisualStudio.Services.Identity.Identity>>) (() => this.GetIdentityMap(requestContext, resolvedNamesInfo)));
        return resolvedNamesInfo;
      }
      finally
      {
        requestContext.TraceLeave(907002, "Identity", nameof (WorkItemIdentityService), nameof (ResolveIdentityFields));
      }
    }

    public ResolvedIdentityNamesInfo ResolveIdentityNames(
      WorkItemTrackingRequestContext witRequestContext,
      IEnumerable<string> names,
      bool includeInactiveIdentitiesForQuery,
      bool syncMissingIdentities = true)
    {
      return this.ResolveIdentityNames(witRequestContext, names, Enumerable.Empty<WorkItemIdentity>(), includeInactiveIdentitiesForQuery, syncMissingIdentities);
    }

    public ResolvedIdentityNamesInfo ResolveIdentityNames(
      WorkItemTrackingRequestContext witRequestContext,
      IEnumerable<string> names,
      IEnumerable<WorkItemIdentity> identities,
      bool includeInactiveIdentitiesForQuery,
      bool syncMissingIdentities = true)
    {
      ArgumentUtility.CheckForNull<WorkItemTrackingRequestContext>(witRequestContext, nameof (witRequestContext));
      ArgumentUtility.CheckForNull<IEnumerable<string>>(names, nameof (names));
      ArgumentUtility.CheckForNull<IEnumerable<WorkItemIdentity>>(identities, nameof (identities));
      IVssRequestContext requestContext = witRequestContext.RequestContext;
      requestContext.TraceEnter(907003, "Identity", nameof (WorkItemIdentityService), nameof (ResolveIdentityNames));
      ResolvedIdentityNamesInfo namesInfo = new ResolvedIdentityNamesInfo();
      try
      {
        if (!names.Any<string>() && !identities.Any<WorkItemIdentity>())
          return namesInfo;
        if (witRequestContext.IsAadBackedAccount)
          names = this.ProcessAadIdentityNames(requestContext, names, namesInfo);
        IList<Microsoft.VisualStudio.Services.Identity.Identity> identities1;
        IDictionary<Guid, List<WorkItemIdentity>> workItemIdentityMap1 = WorkItemIdentityHelper.EnsureVsidToWorkItemIdentityMap(requestContext, identities, out identities1);
        List<ConstantsSearchRecord> source1 = this.SearchConstants(requestContext, names, identities1, includeInactiveIdentitiesForQuery, syncMissingIdentities);
        IDictionary<Guid, WorkItemIdentity> workItemIdentityMap2 = WorkItemIdentityHelper.EnsureVsidToWorkItemIdentityMap(requestContext, (IEnumerable<Guid>) source1.Select<ConstantsSearchRecord, Guid>((Func<ConstantsSearchRecord, Guid>) (r => r.TeamFoundationId)).ToList<Guid>());
        namesInfo.AllRecords = (IEnumerable<ConstantsSearchRecord>) source1;
        IDictionary<string, Guid> values = this.ProcessNonLicensedIdentities(requestContext, names, namesInfo);
        namesInfo.ResolvedNonLicensedIdentities.AddRange<KeyValuePair<string, Guid>, IDictionary<string, Guid>>((IEnumerable<KeyValuePair<string, Guid>>) values);
        if (!source1.Any<ConstantsSearchRecord>())
        {
          namesInfo.NotFoundNames.UnionWith(names);
          namesInfo.NotFoundIdentities.UnionWith(identities);
          return namesInfo;
        }
        Dictionary<string, List<ConstantsSearchRecord>> dictionary = new Dictionary<string, List<ConstantsSearchRecord>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        HashSet<WorkItemIdentity> workItemIdentitySet = new HashSet<WorkItemIdentity>();
        foreach (ConstantsSearchRecord constantsSearchRecord in source1)
        {
          ConstantsSearchRecord record = constantsSearchRecord;
          string key = string.IsNullOrEmpty(record.SearchValue) ? record.DisplayPart : record.SearchValue;
          if (!dictionary.ContainsKey(key))
            dictionary.Add(key, new List<ConstantsSearchRecord>());
          dictionary[key].Add(record);
          List<WorkItemIdentity> source2 = (List<WorkItemIdentity>) null;
          WorkItemIdentity workItemIdentity1 = (WorkItemIdentity) null;
          if (workItemIdentityMap1.TryGetValue(record.TeamFoundationId, out source2))
          {
            source2.ForEach((Action<WorkItemIdentity>) (workItemIdentity => workItemIdentity.DistinctDisplayName = record.DisplayPart));
            workItemIdentity1 = source2.First<WorkItemIdentity>();
          }
          else if (workItemIdentityMap2.TryGetValue(record.TeamFoundationId, out workItemIdentity1))
            workItemIdentity1.DistinctDisplayName = record.DisplayPart;
          else
            workItemIdentity1 = new WorkItemIdentity()
            {
              DistinctDisplayName = record.DisplayPart,
              IdentityRef = WorkItemIdentityHelper.GetIdentityRef(requestContext, (object) record.DisplayPart)
            };
          workItemIdentitySet.Add(workItemIdentity1);
        }
        WorkItemIdentityHelper.AddIdentitiesToDistinctDisplayNameMap(requestContext, (IEnumerable<WorkItemIdentity>) workItemIdentitySet);
        foreach (string key1 in dictionary.Keys)
        {
          string key = key1;
          if (dictionary[key].Count == 1)
            namesInfo.NamesLookup.Add(key, dictionary[key][0]);
          else if (dictionary[key].Count > 1)
          {
            IEnumerable<ConstantsSearchRecord> source3 = dictionary[key].Where<ConstantsSearchRecord>((Func<ConstantsSearchRecord, bool>) (record => record.DisplayPart.Equals(key) && record.TeamFoundationId != Guid.Empty && IdentityHelper.IsDistinctDisplayName(record.DisplayPart)));
            List<ConstantsSearchRecord> list1 = dictionary[key].Where<ConstantsSearchRecord>((Func<ConstantsSearchRecord, bool>) (record => record.TeamFoundationId == Guid.Empty)).ToList<ConstantsSearchRecord>();
            List<ConstantsSearchRecord> list2 = dictionary[key].Where<ConstantsSearchRecord>((Func<ConstantsSearchRecord, bool>) (record => record.TeamFoundationId != Guid.Empty)).ToList<ConstantsSearchRecord>();
            if (source3.Any<ConstantsSearchRecord>())
              namesInfo.NamesLookup.Add(key, source3.FirstOrDefault<ConstantsSearchRecord>());
            else if (list1.Any<ConstantsSearchRecord>() && !includeInactiveIdentitiesForQuery)
            {
              if (list2.Count == 1 && (list2[0].HasUniqueIdentityDisplayName || key.Equals(list2[0].DisambiguationComponent, StringComparison.OrdinalIgnoreCase)))
                namesInfo.NamesLookup.Add(key, list2[0]);
              else
                namesInfo.NamesLookup.Add(key, list1.FirstOrDefault<ConstantsSearchRecord>());
            }
            else
              namesInfo.AmbiguousNamesLookup.Add(key, dictionary[key].ToArray());
          }
        }
        namesInfo.NotFoundIdentities.UnionWith(identities);
        namesInfo.NotFoundIdentities.ExceptWith((IEnumerable<WorkItemIdentity>) workItemIdentitySet);
        namesInfo.NotFoundNames.UnionWith(names);
        namesInfo.NotFoundNames.ExceptWith((IEnumerable<string>) namesInfo.NamesLookup.Keys);
        namesInfo.NotFoundNames.ExceptWith((IEnumerable<string>) namesInfo.AmbiguousNamesLookup.Keys);
        namesInfo.IdentityMap = new Lazy<IDictionary<string, Microsoft.VisualStudio.Services.Identity.Identity>>((Func<IDictionary<string, Microsoft.VisualStudio.Services.Identity.Identity>>) (() => this.GetIdentityMap(requestContext, namesInfo)));
      }
      finally
      {
        requestContext.TraceLeave(907004, "Identity", nameof (WorkItemIdentityService), nameof (ResolveIdentityNames));
      }
      return namesInfo;
    }

    private IDictionary<string, Guid> ProcessNonLicensedIdentities(
      IVssRequestContext requestContext,
      IEnumerable<string> names,
      ResolvedIdentityNamesInfo namesInfo)
    {
      Dictionary<string, Guid> dictionary = new Dictionary<string, Guid>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (string str in names.Where<string>((Func<string, bool>) (n => !namesInfo.AllRecords.Any<ConstantsSearchRecord>((Func<ConstantsSearchRecord, bool>) (r => string.Equals(r.SearchValue, n, StringComparison.OrdinalIgnoreCase))))))
      {
        IdentityDisplayName disambiguatedSearchTerm = IdentityDisplayName.GetDisambiguatedSearchTerm(str);
        if (disambiguatedSearchTerm.Vsid != Guid.Empty)
          dictionary[str] = disambiguatedSearchTerm.Vsid;
      }
      return (IDictionary<string, Guid>) dictionary;
    }

    private List<ConstantsSearchRecord> SearchConstants(
      IVssRequestContext requestContext,
      IEnumerable<string> names,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identities,
      bool includeInactiveIdentitiesForQuery,
      bool syncMissingIdentities)
    {
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> source = identities.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (id => id != null));
      IEnumerable<Guid> vsids = source.Select<Microsoft.VisualStudio.Services.Identity.Identity, Guid>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid>) (id => id.Id)).Distinct<Guid>();
      ITeamFoundationWorkItemTrackingMetadataService service = requestContext.GetService<ITeamFoundationWorkItemTrackingMetadataService>();
      List<ConstantsSearchRecord> list = service.SearchConstantsRecords(requestContext, (IEnumerable<string>) names.ToArray<string>(), vsids, includeInactiveIdentitiesForQuery).ToList<ConstantsSearchRecord>();
      if (!syncMissingIdentities)
        return list;
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      string[] array1 = names.Where<string>(WorkItemIdentityService.\u003C\u003EO.\u003C0\u003E__IsDistinctNameIdentity ?? (WorkItemIdentityService.\u003C\u003EO.\u003C0\u003E__IsDistinctNameIdentity = new Func<string, bool>(IdentityUtilities.IsDistinctNameIdentity))).ToList<string>().Except<string>(list.Select<ConstantsSearchRecord, string>((Func<ConstantsSearchRecord, string>) (x => x.SearchValue)), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).ToArray<string>();
      HashSet<Guid> tfIdFoundinConstants = list.Select<ConstantsSearchRecord, Guid>((Func<ConstantsSearchRecord, Guid>) (x => x.TeamFoundationId)).ToHashSet<Guid>();
      Microsoft.VisualStudio.Services.Identity.Identity[] array2 = source.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (id => !tfIdFoundinConstants.Contains(id.Id))).ToArray<Microsoft.VisualStudio.Services.Identity.Identity>();
      return (array1.Length != 0 || array2.Length != 0) && service.TrySyncIdentitiesToConstants(requestContext, (IList<string>) array1, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) array2) ? service.SearchConstantsRecords(requestContext, (IEnumerable<string>) names.ToArray<string>(), vsids, includeInactiveIdentitiesForQuery).ToList<ConstantsSearchRecord>() : list;
    }

    private IEnumerable<string> ProcessAadIdentityNames(
      IVssRequestContext requestContext,
      IEnumerable<string> names,
      ResolvedIdentityNamesInfo namesInfo)
    {
      requestContext.TraceEnter(907011, "Identity", nameof (WorkItemIdentityService), nameof (ProcessAadIdentityNames));
      try
      {
        List<(string, WorkItemAadIdentifier)> list1 = names.Select<string, (string, WorkItemAadIdentifier)>((Func<string, (string, WorkItemAadIdentifier)>) (x => (x, WorkItemAadIdentifier.Parse(x)))).ToList<(string, WorkItemAadIdentifier)>();
        List<string> list2 = list1.Where<(string, WorkItemAadIdentifier)>((Func<(string, WorkItemAadIdentifier), bool>) (x => x.aadIdentifier == null)).Select<(string, WorkItemAadIdentifier), string>((Func<(string, WorkItemAadIdentifier), string>) (x => x.name)).ToList<string>();
        List<(string, WorkItemAadIdentifier)> list3 = list1.Where<(string, WorkItemAadIdentifier)>((Func<(string, WorkItemAadIdentifier), bool>) (x => x.aadIdentifier != null)).ToList<(string, WorkItemAadIdentifier)>();
        Dictionary<string, string> source = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        IdentityScopeInfo identityScopeInfo = this.ResolveAadEntities(requestContext, list3);
        foreach ((string, WorkItemAadIdentifier) tuple in list3)
        {
          Microsoft.VisualStudio.Services.Identity.Identity identity;
          if (identityScopeInfo.InScopeIdentitiesMap.TryGetValue(tuple.Item2.ObjectId, out identity) && identity != null)
          {
            source[tuple.Item1] = identity.GetLegacyDistinctDisplayName();
            namesInfo.ResolvedAadIdentities.Add(identity.Id, identity);
            namesInfo.AadIdentityLookup.Add(tuple.Item1, identity);
          }
          else if (identityScopeInfo.NotInScopeIds.Contains(tuple.Item2.ObjectId))
            namesInfo.AadNotInScopeNames[tuple.Item1] = tuple.Item2.DisplayName;
          else
            namesInfo.NotFoundNames.Add(tuple.Item1);
        }
        namesInfo.AadNamesLookup = (IDictionary<string, string>) source;
        names = (IEnumerable<string>) list2.Concat<string>(source.Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (x => x.Value))).ToList<string>();
      }
      finally
      {
        requestContext.TraceLeave(907012, "Identity", nameof (WorkItemIdentityService), nameof (ProcessAadIdentityNames));
      }
      return names;
    }

    private IdentityScopeInfo ResolveAadEntities(
      IVssRequestContext requestContext,
      List<(string name, WorkItemAadIdentifier aadIdentifier)> aadCandidateEntities)
    {
      HashSet<Guid> guidSet = new HashSet<Guid>();
      IdentityScopeInfo identityScopeInfo = new IdentityScopeInfo();
      if (aadCandidateEntities.Any<(string, WorkItemAadIdentifier)>())
      {
        List<Guid> list1 = aadCandidateEntities.Where<(string, WorkItemAadIdentifier)>((Func<(string, WorkItemAadIdentifier), bool>) (x => !x.aadIdentifier.IsGroup)).Select<(string, WorkItemAadIdentifier), Guid>((Func<(string, WorkItemAadIdentifier), Guid>) (x => x.aadIdentifier.ObjectId)).ToList<Guid>();
        List<Guid> list2 = aadCandidateEntities.Where<(string, WorkItemAadIdentifier)>((Func<(string, WorkItemAadIdentifier), bool>) (x => x.aadIdentifier.IsGroup)).Select<(string, WorkItemAadIdentifier), Guid>((Func<(string, WorkItemAadIdentifier), Guid>) (x => x.aadIdentifier.ObjectId)).ToList<Guid>();
        identityScopeInfo = IdentityScopeInfo.Merge(this.m_witAadIdentityCreator.EnsureAadEntities(requestContext, (IList<Guid>) list1, false), this.m_witAadIdentityCreator.EnsureAadEntities(requestContext, (IList<Guid>) list2, true));
      }
      return identityScopeInfo;
    }

    public IEnumerable<IdentityRef> SearchIdentities(
      IVssRequestContext requestContext,
      string searchTerm,
      SearchIdentityType identityType = SearchIdentityType.All)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(searchTerm, nameof (searchTerm));
      return requestContext.GetService<ITeamFoundationWorkItemTrackingMetadataService>().SearchConstantIdentityRecords(requestContext, searchTerm, identityType).Select<IdentityConstantRecord, IdentityRef>((Func<IdentityConstantRecord, IdentityRef>) (record => new IdentityRef()
      {
        Id = record.TeamFoundationId.ToString(),
        DisplayName = record.DisplayName ?? string.Empty,
        UniqueName = !record.IsContainer || record.IdentityCategory == IdentityType.WindowsGroup ? record.DisambiguationComponent : record.DisambiguationComponent + "\\" + record.Account,
        IsContainer = record.IsContainer
      }));
    }

    private IDictionary<string, Microsoft.VisualStudio.Services.Identity.Identity> GetIdentityMap(
      IVssRequestContext requestContext,
      ResolvedIdentityNamesInfo resolvedNamesInfo)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      Dictionary<string, Microsoft.VisualStudio.Services.Identity.Identity> identityMap = new Dictionary<string, Microsoft.VisualStudio.Services.Identity.Identity>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      HashSet<Guid> source = new HashSet<Guid>();
      source.UnionWith(resolvedNamesInfo.AllRecords.Where<ConstantsSearchRecord>((Func<ConstantsSearchRecord, bool>) (record => record.TeamFoundationId != Guid.Empty)).Select<ConstantsSearchRecord, Guid>((Func<ConstantsSearchRecord, Guid>) (record => record.TeamFoundationId)));
      if (!source.Any<Guid>())
        return (IDictionary<string, Microsoft.VisualStudio.Services.Identity.Identity>) identityMap;
      Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> dictionary = new Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity>();
      List<Guid> list = source.Where<Guid>((Func<Guid, bool>) (tfId => !resolvedNamesInfo.ResolvedAadIdentities.ContainsKey(tfId))).ToList<Guid>();
      if (list.Any<Guid>())
        dictionary = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<Guid>) list.ToArray(), QueryMembership.None, (IEnumerable<string>) null).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (identity => identity != null)).Distinct<Microsoft.VisualStudio.Services.Identity.Identity>((IEqualityComparer<Microsoft.VisualStudio.Services.Identity.Identity>) IdentityComparer.Instance).ToDictionary<Microsoft.VisualStudio.Services.Identity.Identity, Guid>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid>) (identity => identity.Id));
      foreach (ConstantsSearchRecord allRecord in resolvedNamesInfo.AllRecords)
      {
        if (!identityMap.ContainsKey(allRecord.DisplayPart))
        {
          if (resolvedNamesInfo.ResolvedAadIdentities.ContainsKey(allRecord.TeamFoundationId))
            identityMap.Add(allRecord.DisplayPart, resolvedNamesInfo.ResolvedAadIdentities[allRecord.TeamFoundationId]);
          else if (dictionary.ContainsKey(allRecord.TeamFoundationId))
            identityMap.Add(allRecord.DisplayPart, dictionary[allRecord.TeamFoundationId]);
        }
      }
      return (IDictionary<string, Microsoft.VisualStudio.Services.Identity.Identity>) identityMap;
    }

    private IEnumerable<KeyValuePair<int, object>> ReplaceIdentityFieldsUpdate(
      WorkItemTrackingRequestContext witRequestContext,
      int workItemId,
      IEnumerable<KeyValuePair<int, object>> workItemUpdate,
      IFieldTypeDictionary fieldTypeDictionary,
      ResolvedIdentityNamesInfo resolvedNamesInfo,
      bool bypassRules,
      out IEnumerable<WorkItemFieldInvalidException> exceptions)
    {
      IVssRequestContext requestContext = witRequestContext.RequestContext;
      List<KeyValuePair<int, object>> keyValuePairList = new List<KeyValuePair<int, object>>();
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      List<WorkItemFieldInvalidException> invalidExceptionList = new List<WorkItemFieldInvalidException>();
      bool aadBackedAccount = witRequestContext.IsAadBackedAccount;
      foreach (KeyValuePair<int, object> keyValuePair in workItemUpdate)
      {
        if (fieldTypeDictionary.GetField(keyValuePair.Key).IsIdentity)
        {
          object obj = keyValuePair.Value;
          switch (obj)
          {
            case WorkItemIdentity _ when ((IEnumerable<object>) resolvedNamesInfo.NotFoundIdentities).Contains<object>(obj) && !bypassRules:
              invalidExceptionList.Add(WorkItemIdentityService.CreateInvalidIdentityException(workItemId, keyValuePair.Key, obj.ToString(), fieldTypeDictionary));
              keyValuePairList.Add(keyValuePair);
              continue;
            case WorkItemIdentity _:
              WorkItemIdentity identity = obj as WorkItemIdentity;
              if (identity.DistinctDisplayName == null)
              {
                identity.DistinctDisplayName = identity.IdentityRef?.DisplayName;
                WorkItemIdentityHelper.AddIdentityToDistinctDisplayNameMap(requestContext, identity);
              }
              obj = (object) identity.DistinctDisplayName;
              break;
          }
          string str = (string) null;
          if (aadBackedAccount && obj is string && resolvedNamesInfo.AadNamesLookup.TryGetValue((string) obj, out str))
            obj = (object) str;
          switch (obj)
          {
            case string _ when resolvedNamesInfo.AmbiguousNamesLookup.ContainsKey((string) obj):
              if (requestContext.GetIdentityDisplayType() == IdentityDisplayType.DisplayName | bypassRules)
              {
                keyValuePairList.Add(keyValuePair);
                continue;
              }
              if (string.Compare((string) obj, userIdentity.DisplayName, StringComparison.OrdinalIgnoreCase) == 0)
              {
                string distinctDisplayName = userIdentity.GetLegacyDistinctDisplayName();
                keyValuePairList.Add(new KeyValuePair<int, object>(keyValuePair.Key, (object) distinctDisplayName));
                witRequestContext.RequestContext.Trace(907015, TraceLevel.Verbose, "Identity", nameof (WorkItemIdentityService), string.Format("Replace field value from AmbiguousNamesLookup for fieldId: {0}, IsDistinctValue: {1}", (object) keyValuePair.Key, (object) distinctDisplayName.Contains("<")));
                continue;
              }
              invalidExceptionList.Add(WorkItemIdentityService.CreateAmbiguousIdentityException(workItemId, keyValuePair.Key, (string) obj, ((IEnumerable<ConstantsSearchRecord>) resolvedNamesInfo.AmbiguousNamesLookup[(string) obj]).Select<ConstantsSearchRecord, string>((Func<ConstantsSearchRecord, string>) (record => record.DisplayPart)), fieldTypeDictionary));
              keyValuePairList.Add(keyValuePair);
              continue;
            case string _ when ((IEnumerable<object>) resolvedNamesInfo.NotFoundNames).Contains<object>(obj) && !bypassRules:
              invalidExceptionList.Add(WorkItemIdentityService.CreateInvalidIdentityException(workItemId, keyValuePair.Key, (string) obj, fieldTypeDictionary));
              keyValuePairList.Add(keyValuePair);
              continue;
            case string _ when resolvedNamesInfo.NamesLookup.ContainsKey((string) obj):
              string displayPart = resolvedNamesInfo.NamesLookup[(string) obj].DisplayPart;
              keyValuePairList.Add(new KeyValuePair<int, object>(keyValuePair.Key, (object) displayPart));
              witRequestContext.RequestContext.Trace(907015, TraceLevel.Verbose, "Identity", nameof (WorkItemIdentityService), string.Format("Replace field value from NamesLookup for fieldId: {0}, IsDistinctValue: {1}", (object) keyValuePair.Key, (object) displayPart.Contains("<")));
              continue;
            case string _ when resolvedNamesInfo.AadNotInScopeNames.ContainsKey((string) obj):
              invalidExceptionList.Add(WorkItemIdentityService.CreateAadNotInScopeIdentityException(workItemId, keyValuePair.Key, resolvedNamesInfo.AadNotInScopeNames[(string) obj], fieldTypeDictionary));
              keyValuePairList.Add(keyValuePair);
              continue;
            default:
              keyValuePairList.Add(keyValuePair);
              continue;
          }
        }
        else
          keyValuePairList.Add(keyValuePair);
      }
      exceptions = (IEnumerable<WorkItemFieldInvalidException>) invalidExceptionList;
      return (IEnumerable<KeyValuePair<int, object>>) keyValuePairList;
    }

    private static WorkItemFieldInvalidException CreateAmbiguousIdentityException(
      int workItemId,
      int fieldId,
      string fieldValue,
      IEnumerable<string> ambiguousNames,
      IFieldTypeDictionary fieldTypeDictionary)
    {
      string message = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.FieldAmbiguousIdentity((object) fieldValue, (object) WorkItemIdentityService.GetFieldName(fieldId, fieldTypeDictionary), (object) ambiguousNames.Aggregate<string>((Func<string, string, string>) ((i, j) => i + ";" + j)));
      string fieldReferenceName = WorkItemIdentityService.GetFieldReferenceName(fieldId, fieldTypeDictionary);
      return new WorkItemFieldInvalidException(workItemId, fieldReferenceName, message);
    }

    private static WorkItemFieldInvalidException CreateInvalidIdentityException(
      int workItemId,
      int fieldId,
      string fieldValue,
      IFieldTypeDictionary fieldTypeDictionary)
    {
      string message = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.FieldUnknownIdentity((object) fieldValue, (object) WorkItemIdentityService.GetFieldName(fieldId, fieldTypeDictionary));
      string fieldReferenceName = WorkItemIdentityService.GetFieldReferenceName(fieldId, fieldTypeDictionary);
      return new WorkItemFieldInvalidException(workItemId, fieldReferenceName, message);
    }

    private static WorkItemFieldInvalidException CreateAadNotInScopeIdentityException(
      int workItemId,
      int fieldId,
      string fieldValue,
      IFieldTypeDictionary fieldTypeDictionary)
    {
      string message = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.NotInScopeAadIdentity((object) fieldValue, (object) WorkItemIdentityService.GetFieldName(fieldId, fieldTypeDictionary));
      string fieldReferenceName = WorkItemIdentityService.GetFieldReferenceName(fieldId, fieldTypeDictionary);
      return new WorkItemFieldInvalidException(workItemId, fieldReferenceName, message);
    }

    private static string GetFieldName(int fieldId, IFieldTypeDictionary fieldTypeDictionary) => fieldTypeDictionary.GetField(fieldId).Name;

    private static string GetFieldReferenceName(
      int fieldId,
      IFieldTypeDictionary fieldTypeDictionary)
    {
      return fieldTypeDictionary.GetField(fieldId).ReferenceName;
    }

    private ResolvedIdentityNamesInfo GetResolvedNamesInfo(
      WorkItemTrackingRequestContext witRequestContext,
      List<KeyValuePair<int, object>> identityFieldsList)
    {
      if (identityFieldsList == null || !identityFieldsList.Any<KeyValuePair<int, object>>())
        return new ResolvedIdentityNamesInfo();
      HashSet<string> names = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      List<WorkItemIdentity> identities = new List<WorkItemIdentity>();
      foreach (KeyValuePair<int, object> identityFields in identityFieldsList)
      {
        string str = identityFields.Value as string;
        WorkItemIdentity workItemIdentity = identityFields.Value as WorkItemIdentity;
        if (!string.IsNullOrEmpty(str))
          names.Add(str);
        else if (workItemIdentity != null)
        {
          bool flag = false;
          if (workItemIdentity.IdentityRef != null)
          {
            if (workItemIdentity.IdentityRef.Descriptor.ToString() != null)
            {
              identities.Add(workItemIdentity);
              flag = true;
            }
            else if (!string.IsNullOrEmpty(workItemIdentity.IdentityRef.DisplayName))
              names.Add(workItemIdentity.IdentityRef.DisplayName);
          }
          if (!flag && !string.IsNullOrEmpty(workItemIdentity.DistinctDisplayName))
            names.Add(workItemIdentity.DistinctDisplayName);
        }
      }
      return this.ResolveIdentityNames(witRequestContext, (IEnumerable<string>) names, (IEnumerable<WorkItemIdentity>) identities, false, true);
    }
  }
}
