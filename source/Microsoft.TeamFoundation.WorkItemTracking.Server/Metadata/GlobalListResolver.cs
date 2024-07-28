// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.GlobalListResolver
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  public sealed class GlobalListResolver : ListRuleResolver
  {
    private readonly HashSet<int> skippedSetsForExpansion;

    public GlobalListResolver(IVssRequestContext requestContext, HashSet<int> skippedSetsToExpand)
      : base(requestContext, ListRuleResolverType.ExpandGlobalList)
    {
      this.skippedSetsForExpansion = skippedSetsToExpand;
    }

    public override void Resolve(ListRuleResolverState state) => this.requestContext.TraceBlock(911315, 911316, "WorkItemType", "AdditionalWorkItemTypeProperties", nameof (Resolve), (Action) (() =>
    {
      List<ListRule> rule1;
      if (state == null || !state.TryGetRules(this.ResolvingType, out rule1) || rule1.Count == 0)
        return;
      List<ConstantSetReference> source1 = new List<ConstantSetReference>();
      foreach (ListRule listRule in rule1)
      {
        foreach (ConstantSetReference set in listRule.Sets)
        {
          if (!this.skippedSetsForExpansion.Contains(set.Id) && set.TeamFoundationId == Guid.Empty && !set.Direct)
            source1.Add(new ConstantSetReference()
            {
              Id = set.Id,
              Direct = true,
              IncludeTop = true,
              ExcludeGroups = false
            });
        }
      }
      IDictionary<ConstantSetReference, SetRecord[]> directConstantSets = this.requestContext.GetService<ITeamFoundationWorkItemTrackingMetadataService>().GetDirectConstantSets(this.requestContext, source1.Distinct<ConstantSetReference>());
      IEnumerable<Guid> source2 = directConstantSets.SelectMany<KeyValuePair<ConstantSetReference, SetRecord[]>, SetRecord>((Func<KeyValuePair<ConstantSetReference, SetRecord[]>, IEnumerable<SetRecord>>) (x => (IEnumerable<SetRecord>) x.Value)).Select<SetRecord, Guid>((Func<SetRecord, Guid>) (x => x.TeamFoundationId)).Where<Guid>((Func<Guid, bool>) (x => x != Guid.Empty)).Distinct<Guid>();
      IdentityService service = this.requestContext.GetService<IdentityService>();
      Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> dictionary = new Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity>();
      if (source2.Any<Guid>())
        dictionary = service.ReadIdentities(this.requestContext, (IList<Guid>) source2.ToArray<Guid>(), QueryMembership.None, (IEnumerable<string>) null).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (i => i != null)).ToDictionary<Microsoft.VisualStudio.Services.Identity.Identity, Guid, Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid>) (i => i.Id), (Func<Microsoft.VisualStudio.Services.Identity.Identity, Microsoft.VisualStudio.Services.Identity.Identity>) (i => i));
      foreach (ListRule rule2 in rule1)
      {
        List<ConstantSetReference> constantSetReferenceList1 = new List<ConstantSetReference>();
        foreach (ConstantSetReference set in rule2.Sets)
        {
          if (this.skippedSetsForExpansion.Contains(set.Id) || set.TeamFoundationId != Guid.Empty || set.Direct)
          {
            constantSetReferenceList1.Add(set);
          }
          else
          {
            ConstantSetReference key = new ConstantSetReference()
            {
              Id = set.Id,
              Direct = true,
              IncludeTop = true,
              ExcludeGroups = false
            };
            SetRecord[] setRecordArray;
            if (directConstantSets.TryGetValue(key, out setRecordArray))
            {
              foreach (SetRecord setRecord in setRecordArray)
              {
                if (setRecord.ItemId == set.Id)
                {
                  List<ConstantSetReference> constantSetReferenceList2 = constantSetReferenceList1;
                  constantSetReferenceList2.Add((ConstantSetReference) new ExtendedConstantSetRef()
                  {
                    Id = setRecord.ItemId,
                    ExcludeGroups = set.ExcludeGroups,
                    IncludeTop = !set.ExcludeGroups,
                    Direct = false,
                    TeamFoundationId = setRecord.TeamFoundationId,
                    IdentityDescriptor = (dictionary.ContainsKey(setRecord.TeamFoundationId) ? dictionary[setRecord.TeamFoundationId].Descriptor : (IdentityDescriptor) null),
                    DisplayName = (setRecord.Item.Length > 0 ? setRecord.Item.Substring(1, setRecord.Item.Length - 1) : string.Empty),
                    IsGroup = true,
                    DistinctDisplayName = setRecord.Item
                  });
                }
                else if (setRecord.TeamFoundationId != Guid.Empty)
                {
                  Microsoft.VisualStudio.Services.Identity.Identity identity = dictionary.ContainsKey(setRecord.TeamFoundationId) ? dictionary[setRecord.TeamFoundationId] : (Microsoft.VisualStudio.Services.Identity.Identity) null;
                  List<ConstantSetReference> constantSetReferenceList3 = constantSetReferenceList1;
                  constantSetReferenceList3.Add((ConstantSetReference) new ExtendedConstantSetRef()
                  {
                    Id = setRecord.ItemId,
                    ExcludeGroups = set.ExcludeGroups,
                    IncludeTop = !set.ExcludeGroups,
                    Direct = ((identity != null ? (identity.IsContainer ? 1 : 0) : 0) == 0),
                    TeamFoundationId = setRecord.TeamFoundationId,
                    IdentityDescriptor = identity?.Descriptor,
                    DisplayName = (string) null,
                    IsGroup = (identity != null && identity.IsContainer),
                    DistinctDisplayName = setRecord.Item
                  });
                  state.MarkRuleForFutureProcessing(ListRuleResolverType.ConvertEntityId, rule2);
                }
                else
                  rule2.Values.Add(setRecord.Item);
              }
            }
          }
        }
        rule2.Sets = constantSetReferenceList1.ToArray();
      }
    }));
  }
}
