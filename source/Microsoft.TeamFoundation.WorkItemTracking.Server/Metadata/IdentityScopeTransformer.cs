// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.IdentityScopeTransformer
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  public class IdentityScopeTransformer : IdentityRuleTransformer
  {
    private IVssRequestContext requestContext;

    public IdentityScopeTransformer(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IReadOnlyCollection<ProcessTypelet> PopulateIdentityAllowedValueRules(
      IEnumerable<ProcessTypelet> typelets)
    {
      Dictionary<string, IEnumerable<WorkItemFieldRule>> dictionary = typelets.Where<ProcessTypelet>((Func<ProcessTypelet, bool>) (typelet => typelet.FieldRules != null)).ToDictionary<ProcessTypelet, string, IEnumerable<WorkItemFieldRule>>((Func<ProcessTypelet, string>) (typelet => typelet.ReferenceName), (Func<ProcessTypelet, IEnumerable<WorkItemFieldRule>>) (typelet => (IEnumerable<WorkItemFieldRule>) typelet.FieldRules.ToList<WorkItemFieldRule>()));
      ListRuleResolverState state = new ListRuleResolverState(dictionary.SelectMany<KeyValuePair<string, IEnumerable<WorkItemFieldRule>>, WorkItemFieldRule>((Func<KeyValuePair<string, IEnumerable<WorkItemFieldRule>>, IEnumerable<WorkItemFieldRule>>) (v => v.Value)));
      foreach (ListRule rule in state.IdentifyAllFieldListRules(this.requestContext).Where<ListRuleResolverState.FieldListRule>((Func<ListRuleResolverState.FieldListRule, bool>) (f => f.ForIdentityField && f.Rule is AllowedValuesRule)).Select<ListRuleResolverState.FieldListRule, ListRule>((Func<ListRuleResolverState.FieldListRule, ListRule>) (f => f.Rule)))
        state.MarkRuleForFutureProcessing(ListRuleResolverType.HandleWellKnownSets, rule);
      new WellKnownSetsResolver(this.requestContext, new Dictionary<int, Tuple<IdentityDescriptor, bool>>()
      {
        {
          -2,
          new Tuple<IdentityDescriptor, bool>(GroupWellKnownIdentityDescriptors.EveryoneGroup, true)
        },
        {
          -1,
          new Tuple<IdentityDescriptor, bool>(GroupWellKnownIdentityDescriptors.EveryoneGroup, false)
        }
      }).Resolve(state);
      new EntityIdResolver(this.requestContext).Resolve(state);
      return this.ReplaceRules(typelets, dictionary);
    }
  }
}
