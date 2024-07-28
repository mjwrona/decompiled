// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.ListExpansionResolver
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Identity;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  public sealed class ListExpansionResolver : ListRuleResolver
  {
    private readonly HashSet<int> skippedSetsForExpansion;
    private readonly WitReadReplicaContext? readReplicaContext;
    private readonly bool isScopedIdentityEnabled;

    public ListExpansionResolver(
      IVssRequestContext context,
      HashSet<int> skippedSetsForExpansion,
      WitReadReplicaContext? readReplicaContext,
      bool isScopedIdentityEnabled)
      : base(context, ListRuleResolverType.ExpandList)
    {
      this.skippedSetsForExpansion = skippedSetsForExpansion;
      this.readReplicaContext = readReplicaContext;
      this.isScopedIdentityEnabled = isScopedIdentityEnabled;
    }

    private bool NeedTrackingForValidUserBug(ListRuleResolverState.FieldListRule listRule) => WorkItemTrackingFeatureFlags.IsDebugValidUserGroupEnabled(this.requestContext) && listRule.ForIdentityField && listRule.Rule is AllowedValuesRule && (listRule.Rule as AllowedValuesRule).ConvertedFromValidUser && ((IEnumerable<ConstantSetReference>) listRule.Rule.Sets).Any<ConstantSetReference>((Func<ConstantSetReference, bool>) (s => s.Id != -2));

    public override void Resolve(ListRuleResolverState state) => this.requestContext.TraceBlock(911315, 911316, "WorkItemType", "AdditionalWorkItemTypeProperties", nameof (Resolve), (Action) (() =>
    {
      if (state == null || !state.FieldRules.Any<WorkItemFieldRule>())
        return;
      List<ListRuleResolverState.FieldListRule> list = state.IdentifyAllFieldListRules(this.requestContext).ToList<ListRuleResolverState.FieldListRule>();
      HashSet<ConstantSetReference> constantSetReferenceSet = new HashSet<ConstantSetReference>();
      HashSet<int> intSet = new HashSet<int>();
      foreach (ListRuleResolverState.FieldListRule fieldListRule in list)
      {
        if (fieldListRule.Rule is MatchRule)
        {
          constantSetReferenceSet.UnionWith((IEnumerable<ConstantSetReference>) fieldListRule.Rule.Sets);
        }
        else
        {
          AllowedValuesRule rule = fieldListRule.Rule as AllowedValuesRule;
          foreach (ConstantSetReference set in fieldListRule.Rule.Sets)
          {
            if (!this.skippedSetsForExpansion.Contains(set.Id))
            {
              if (this.isScopedIdentityEnabled && rule != null && rule.ConvertedFromValidUser && fieldListRule.ForIdentityField)
                intSet.Add(set.Id);
              else
                constantSetReferenceSet.Add(new ConstantSetReference()
                {
                  Id = set.Id,
                  Direct = true,
                  IncludeTop = false,
                  ExcludeGroups = false
                });
            }
          }
        }
      }
      ITeamFoundationWorkItemTrackingMetadataService service1 = this.requestContext.GetService<ITeamFoundationWorkItemTrackingMetadataService>();
      bool aadBackedAccount = this.requestContext.WitContext().IsAadBackedAccount;
      IEnumerable<Guid> guids = (IEnumerable<Guid>) new List<Guid>();
      IDictionary<ConstantSetReference, SetRecord[]> source1 = (IDictionary<ConstantSetReference, SetRecord[]>) new Dictionary<ConstantSetReference, SetRecord[]>();
      Dictionary<int, Guid> dictionary1 = new Dictionary<int, Guid>();
      if (constantSetReferenceSet.Any<ConstantSetReference>())
      {
        source1 = service1.GetDirectConstantSets(this.requestContext, (IEnumerable<ConstantSetReference>) constantSetReferenceSet);
        guids = source1.SelectMany<KeyValuePair<ConstantSetReference, SetRecord[]>, SetRecord>((Func<KeyValuePair<ConstantSetReference, SetRecord[]>, IEnumerable<SetRecord>>) (x => (IEnumerable<SetRecord>) x.Value)).Select<SetRecord, Guid>((Func<SetRecord, Guid>) (x => x.TeamFoundationId)).Where<Guid>((Func<Guid, bool>) (x => x != Guid.Empty)).Distinct<Guid>();
        if (aadBackedAccount)
        {
          dictionary1 = service1.GetConstantRecords(this.requestContext, constantSetReferenceSet.Select<ConstantSetReference, int>((Func<ConstantSetReference, int>) (s => s.Id)), readReplicaContext: this.readReplicaContext).Where<ConstantRecord>((Func<ConstantRecord, bool>) (s => s.TeamFoundationId != Guid.Empty)).ToDictionary<ConstantRecord, int, Guid>((Func<ConstantRecord, int>) (record => record.Id), (Func<ConstantRecord, Guid>) (record => record.TeamFoundationId));
          guids = guids.Concat<Guid>((IEnumerable<Guid>) dictionary1.Values).Distinct<Guid>();
        }
      }
      Dictionary<int, Guid> source2 = new Dictionary<int, Guid>();
      if (this.isScopedIdentityEnabled && intSet.Any<int>())
      {
        source2 = service1.GetConstantRecords(this.requestContext, (IEnumerable<int>) intSet, readReplicaContext: this.readReplicaContext).ToDictionary<ConstantRecord, int, Guid>((Func<ConstantRecord, int>) (c => c.Id), (Func<ConstantRecord, Guid>) (c => c.TeamFoundationId));
        guids = guids.Concat<Guid>(source2.Select<KeyValuePair<int, Guid>, Guid>((Func<KeyValuePair<int, Guid>, Guid>) (x => x.Value)));
      }
      IDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> dictionary2 = (IDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity>) new Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity>();
      if (guids.Any<Guid>())
        dictionary2 = (IDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity>) this.requestContext.GetService<IdentityService>().ReadIdentities(this.requestContext, (IList<Guid>) guids.Distinct<Guid>().ToArray<Guid>(), QueryMembership.None, (IEnumerable<string>) null).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (identity => identity != null)).ToDictionary<Microsoft.VisualStudio.Services.Identity.Identity, Guid, Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid>) (identity => identity.Id), (Func<Microsoft.VisualStudio.Services.Identity.Identity, Microsoft.VisualStudio.Services.Identity.Identity>) (identity => identity));
      foreach (ListRuleResolverState.FieldListRule listRule in list)
      {
        bool flag1 = this.NeedTrackingForValidUserBug(listRule);
        CustomerIntelligenceService service2 = this.requestContext.GetService<CustomerIntelligenceService>();
        CustomerIntelligenceData properties = (CustomerIntelligenceData) null;
        ConstantSetReference[] sets = listRule.Rule.Sets;
        if (flag1)
        {
          properties = new CustomerIntelligenceData();
          properties.Add("FieldId", (double) listRule.FieldId);
          properties.Add("IncomingSets", string.Join<ConstantSetReference>("#", (IEnumerable<ConstantSetReference>) listRule.Rule.Sets));
        }
        List<ConstantSetReference> values = new List<ConstantSetReference>();
        HashSet<string> stringSet = listRule.Rule.Values == null ? new HashSet<string>((IEqualityComparer<string>) TFStringComparer.AllowedValue) : new HashSet<string>((IEnumerable<string>) listRule.Rule.Values, (IEqualityComparer<string>) TFStringComparer.AllowedValue);
        foreach (ConstantSetReference constantSetReference in sets)
        {
          ConstantSetReference setRef = constantSetReference;
          SetRecord[] source3;
          if (listRule.Rule is MatchRule)
          {
            if (source1.TryGetValue(setRef, out source3))
              stringSet.UnionWith(((IEnumerable<SetRecord>) source3).Select<SetRecord, string>((Func<SetRecord, string>) (item => item.Item)));
          }
          else
          {
            ConstantSetReference key1 = new ConstantSetReference()
            {
              Id = setRef.Id,
              Direct = true,
              IncludeTop = false,
              ExcludeGroups = false
            };
            if (source1.TryGetValue(key1, out source3))
            {
              if (flag1)
                properties.Add(string.Format("ItemsFromConstantsSets {0}", (object) setRef.Id), string.Join("#", ((IEnumerable<SetRecord>) source3).Select<SetRecord, string>((Func<SetRecord, string>) (i => string.Format("ItemId: {0};ParentId: {1};Item: {2};IncludeTop: {3};IncludeGroups: {4};TfId: {5};SetHandle: {6};IsList: {7};Direct: {8}", (object) i.ItemId, (object) i.ParentId, (object) i.Item, (object) i.IncludeTop, (object) i.IncludeGroups, (object) i.TeamFoundationId, (object) i.SetHandle, (object) i.IsList, (object) i.Direct)))));
              Guid key2;
              if (aadBackedAccount && dictionary1.TryGetValue(setRef.Id, out key2))
              {
                Microsoft.VisualStudio.Services.Identity.Identity identity;
                bool flag2 = dictionary2.TryGetValue(key2, out identity);
                values.Add(new ConstantSetReference()
                {
                  Id = setRef.Id,
                  ExcludeGroups = setRef.ExcludeGroups,
                  IncludeTop = !setRef.ExcludeGroups,
                  Direct = false,
                  TeamFoundationId = key2,
                  IdentityDescriptor = flag2 ? identity.Descriptor : (IdentityDescriptor) null
                });
                if (flag1)
                {
                  properties.Add(string.Format("TfIdFromConstantSets {0}", (object) setRef.Id), (object) key2);
                  properties.Add(string.Format("TfIdFromIMS {0}", (object) setRef.Id), (object) identity?.Id);
                  properties.Add(string.Format("IdentityDescriptor {0}", (object) setRef.Id), (object) identity?.Descriptor);
                }
              }
              else
              {
                foreach (SetRecord setRecord1 in source3)
                {
                  SetRecord setRecord = setRecord1;
                  Microsoft.VisualStudio.Services.Identity.Identity identity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
                  bool flag3 = false;
                  bool isRealIdentity = false;
                  Func<bool, bool, string, ExtendedConstantSetRef> func = (Func<bool, bool, string, ExtendedConstantSetRef>) ((isDirect, isGroup, overridingDisplayName) =>
                  {
                    return new ExtendedConstantSetRef()
                    {
                      Id = setRecord.ItemId,
                      ExcludeGroups = setRef.ExcludeGroups,
                      IncludeTop = !setRef.ExcludeGroups,
                      Direct = isDirect,
                      TeamFoundationId = setRecord.TeamFoundationId,
                      IdentityDescriptor = isRealIdentity ? identity.Descriptor : (IdentityDescriptor) null,
                      DisplayName = string.IsNullOrWhiteSpace(overridingDisplayName) ? (isRealIdentity ? identity.DisplayName : (string) null) : overridingDisplayName,
                      IsGroup = isGroup,
                      DistinctDisplayName = setRecord.Item
                    };
                  });
                  if (listRule.ForIdentityField && setRecord.TeamFoundationId != Guid.Empty && dictionary2.TryGetValue(setRecord.TeamFoundationId, out identity))
                  {
                    isRealIdentity = true;
                    flag3 = identity.IsContainer;
                  }
                  if (setRecord.ItemId == setRef.Id)
                  {
                    if (setRef.IncludeTop)
                    {
                      string str = setRecord.Item;
                      stringSet.Add(str);
                    }
                  }
                  else if (setRecord.IsList | flag3)
                  {
                    if (setRef.Direct)
                    {
                      if (!setRef.ExcludeGroups)
                      {
                        if (this.isScopedIdentityEnabled && listRule.Rule is AllowedValuesRule && listRule.ForIdentityField)
                        {
                          values.Add((ConstantSetReference) func(true, true, (string) null));
                          state.MarkRuleForFutureProcessing(ListRuleResolverType.ConvertEntityId, listRule.Rule);
                        }
                        else
                          stringSet.Add(setRecord.Item);
                      }
                    }
                    else
                    {
                      values.Add((ConstantSetReference) func(false, true, (string) null));
                      if (listRule.Rule is AllowedValuesRule && listRule.ForIdentityField)
                      {
                        if (this.isScopedIdentityEnabled & flag3)
                          state.MarkRuleForFutureProcessing(ListRuleResolverType.ConvertEntityId, listRule.Rule);
                        else if (!flag3)
                          state.MarkRuleForFutureProcessing(ListRuleResolverType.ExpandGlobalList, listRule.Rule);
                      }
                    }
                  }
                  else if (identity != null)
                  {
                    values.Add((ConstantSetReference) func(true, false, (string) null));
                    state.MarkRuleForFutureProcessing(ListRuleResolverType.ConvertEntityId, listRule.Rule);
                  }
                  else
                  {
                    string str = setRecord.Item;
                    stringSet.Add(str);
                    if (this.isScopedIdentityEnabled && listRule.Rule is AllowedValuesRule && listRule.ForIdentityField)
                      values.Add((ConstantSetReference) func(true, false, str));
                  }
                }
              }
            }
            else
            {
              AllowedValuesRule rule = listRule.Rule as AllowedValuesRule;
              if (!this.skippedSetsForExpansion.Contains(setRef.Id) && this.isScopedIdentityEnabled && rule != null && rule.ConvertedFromValidUser && listRule.ForIdentityField)
              {
                Guid key3;
                Microsoft.VisualStudio.Services.Identity.Identity identity;
                if (source2.TryGetValue(setRef.Id, out key3) && dictionary2.TryGetValue(key3, out identity))
                {
                  List<ConstantSetReference> constantSetReferenceList = values;
                  constantSetReferenceList.Add((ConstantSetReference) new ExtendedConstantSetRef()
                  {
                    Id = setRef.Id,
                    ExcludeGroups = setRef.ExcludeGroups,
                    IncludeTop = setRef.IncludeTop,
                    Direct = setRef.Direct,
                    DistinctDisplayName = (identity != null ? identity.GetLegacyDistinctDisplayName() : (string) null),
                    TeamFoundationId = source2[setRef.Id],
                    IdentityDescriptor = identity?.Descriptor,
                    DisplayName = identity?.DisplayName,
                    IsGroup = (identity != null && identity.IsContainer)
                  });
                  state.MarkRuleForFutureProcessing(ListRuleResolverType.ConvertEntityId, listRule.Rule);
                }
              }
              else
              {
                values.Add(setRef);
                state.MarkRuleForFutureProcessing(ListRuleResolverType.HandleWellKnownSets, listRule.Rule);
              }
            }
          }
        }
        if (flag1)
        {
          properties.Add("FinalSets", string.Join<ConstantSetReference>("#", (IEnumerable<ConstantSetReference>) values));
          service2.Publish(this.requestContext, nameof (ListExpansionResolver), nameof (Resolve), properties);
        }
        listRule.Rule.Values = stringSet;
        listRule.Rule.Sets = values.ToArray();
      }
    }));
  }
}
