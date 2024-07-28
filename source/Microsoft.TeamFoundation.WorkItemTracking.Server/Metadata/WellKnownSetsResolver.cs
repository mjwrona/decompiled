// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WellKnownSetsResolver
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  public sealed class WellKnownSetsResolver : ListRuleResolver
  {
    private readonly Dictionary<int, Tuple<IdentityDescriptor, bool>> wellKnowSetsDescriptorDictionary;

    public WellKnownSetsResolver(
      IVssRequestContext requestContext,
      Dictionary<int, Tuple<IdentityDescriptor, bool>> wellKnownSetsDescriptorDictionary)
      : base(requestContext, ListRuleResolverType.HandleWellKnownSets)
    {
      this.wellKnowSetsDescriptorDictionary = wellKnownSetsDescriptorDictionary;
    }

    public override void Resolve(ListRuleResolverState state) => this.requestContext.TraceBlock(911315, 911316, "WorkItemType", "AdditionalWorkItemTypeProperties", nameof (Resolve), (Action) (() =>
    {
      List<ListRule> rule1;
      if (state == null || !state.TryGetRules(this.ResolvingType, out rule1) || rule1.Count == 0)
        return;
      IdentityService service = this.requestContext.GetService<IdentityService>();
      IdentityDescriptor[] array = rule1.SelectMany<ListRule, IdentityDescriptor>((Func<ListRule, IEnumerable<IdentityDescriptor>>) (r => ((IEnumerable<ConstantSetReference>) r.Sets).Select<ConstantSetReference, IdentityDescriptor>((Func<ConstantSetReference, IdentityDescriptor>) (s =>
      {
        Tuple<IdentityDescriptor, bool> tuple;
        if (this.wellKnowSetsDescriptorDictionary.TryGetValue(s.Id, out tuple))
        {
          s.IdentityDescriptor = tuple.Item1;
          s.ExcludeGroups = tuple.Item2;
        }
        return tuple.Item1;
      })).Where<IdentityDescriptor>((Func<IdentityDescriptor, bool>) (i => i != (IdentityDescriptor) null)))).Distinct<IdentityDescriptor>().ToArray<IdentityDescriptor>();
      IList<Microsoft.VisualStudio.Services.Identity.Identity> source = service.ReadIdentities(this.requestContext.Elevate(), (IList<IdentityDescriptor>) array, QueryMembership.None, (IEnumerable<string>) null);
      service.IdentityMapper.MapToWellKnownIdentifiers((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) source);
      Dictionary<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity> dedupedDictionary = source.ToDedupedDictionary<Microsoft.VisualStudio.Services.Identity.Identity, IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, IdentityDescriptor>) (identity => identity.Descriptor), (Func<Microsoft.VisualStudio.Services.Identity.Identity, Microsoft.VisualStudio.Services.Identity.Identity>) (identity => identity), (IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
      foreach (ListRule rule2 in rule1)
      {
        List<ConstantSetReference> constantSetReferenceList1 = new List<ConstantSetReference>();
        foreach (ConstantSetReference set in rule2.Sets)
        {
          Microsoft.VisualStudio.Services.Identity.Identity identity;
          if (dedupedDictionary.TryGetValue(set.IdentityDescriptor, out identity))
          {
            List<ConstantSetReference> constantSetReferenceList2 = constantSetReferenceList1;
            constantSetReferenceList2.Add((ConstantSetReference) new ExtendedConstantSetRef()
            {
              Id = set.Id,
              ExcludeGroups = set.ExcludeGroups,
              IncludeTop = set.IncludeTop,
              Direct = set.Direct,
              TeamFoundationId = identity.Id,
              IdentityDescriptor = identity?.Descriptor,
              DisplayName = identity?.DisplayName,
              IsGroup = (identity != null && identity.IsContainer)
            });
            state.MarkRuleForFutureProcessing(ListRuleResolverType.ConvertEntityId, rule2);
          }
          else
            constantSetReferenceList1.Add(set);
        }
        rule2.Sets = constantSetReferenceList1.ToArray();
      }
    }));
  }
}
