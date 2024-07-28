// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.EntityIdResolver
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using Microsoft.VisualStudio.Services.Directories.DiscoveryService;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  public sealed class EntityIdResolver : ListRuleResolver
  {
    public EntityIdResolver(IVssRequestContext requestContext)
      : base(requestContext, ListRuleResolverType.ConvertEntityId)
    {
    }

    public override void Resolve(ListRuleResolverState state) => this.requestContext.TraceBlock(911315, 911316, "WorkItemType", "AdditionalWorkItemTypeProperties", nameof (Resolve), (Action) (() =>
    {
      List<ListRule> rule;
      if (state == null || !state.TryGetRules(this.ResolvingType, out rule) || rule.Count == 0)
        return;
      DirectoryDiscoveryService service = this.requestContext.GetService<DirectoryDiscoveryService>();
      List<string> list = rule.SelectMany<ListRule, string>((Func<ListRule, IEnumerable<string>>) (r => ((IEnumerable<ConstantSetReference>) r.Sets).Where<ConstantSetReference>((Func<ConstantSetReference, bool>) (s => s.TeamFoundationId != Guid.Empty && s.IdentityDescriptor != (IdentityDescriptor) null)).Select<ConstantSetReference, string>((Func<ConstantSetReference, string>) (s => s.TeamFoundationId.ToString())))).Distinct<string>().ToList<string>();
      IVssRequestContext requestContext = this.requestContext;
      DirectoryConvertKeysResponse convertKeysResponse = service.ConvertKeys(requestContext, new DirectoryConvertKeysRequest()
      {
        ConvertFrom = "VisualStudioIdentifier",
        ConvertTo = "DirectoryEntityIdentifier",
        Directories = (IEnumerable<string>) new List<string>()
        {
          "vsd"
        },
        Keys = (IEnumerable<string>) list
      });
      IEnumerable<Exception> exceptions = convertKeysResponse.Results.Where<KeyValuePair<string, DirectoryConvertKeyResult>>((Func<KeyValuePair<string, DirectoryConvertKeyResult>, bool>) (r => r.Value.Exception != null)).Select<KeyValuePair<string, DirectoryConvertKeyResult>, Exception>((Func<KeyValuePair<string, DirectoryConvertKeyResult>, Exception>) (r => r.Value.Exception));
      if (exceptions.Any<Exception>())
        throw new AggregateException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ResolveEntitiesError((object) string.Join(",", (IEnumerable<string>) list)), exceptions);
      Dictionary<string, string> dictionary = convertKeysResponse.Results.ToDictionary<KeyValuePair<string, DirectoryConvertKeyResult>, string, string>((Func<KeyValuePair<string, DirectoryConvertKeyResult>, string>) (pair => pair.Key), (Func<KeyValuePair<string, DirectoryConvertKeyResult>, string>) (pair => pair.Value.Key));
      foreach (ListRule listRule in rule)
      {
        foreach (ConstantSetReference set in listRule.Sets)
        {
          string str;
          if (set is ExtendedConstantSetRef extendedConstantSetRef2 && !(extendedConstantSetRef2.TeamFoundationId == Guid.Empty) && !(extendedConstantSetRef2.IdentityDescriptor == (IdentityDescriptor) null) && dictionary.TryGetValue(set.TeamFoundationId.ToString(), out str))
            extendedConstantSetRef2.EntityId = str;
        }
      }
    }));
  }
}
