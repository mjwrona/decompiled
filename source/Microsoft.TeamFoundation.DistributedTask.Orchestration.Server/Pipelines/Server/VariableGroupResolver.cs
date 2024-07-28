// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Server.VariableGroupResolver
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Server
{
  internal sealed class VariableGroupResolver : IVariableGroupResolver
  {
    private readonly Guid m_projectId;
    private readonly IVssRequestContext m_requestContext;
    private readonly bool m_allowImplicitAuthorization;
    private HashSet<VariableGroupReference> m_authorizedReferences = new HashSet<VariableGroupReference>((IEqualityComparer<VariableGroupReference>) new PipelineResources.VariableGroupComparer());

    public VariableGroupResolver(
      IVssRequestContext requestContext,
      Guid projectId,
      bool allowImplicitAuthorization = false)
    {
      this.m_projectId = projectId;
      this.m_requestContext = requestContext;
      this.m_allowImplicitAuthorization = allowImplicitAuthorization;
    }

    public IList<VariableGroup> Resolve(
      ICollection<VariableGroupReference> references,
      ResourceActionFilter actionFilter = ResourceActionFilter.Use)
    {
      List<VariableGroup> list = VariableGroupResolver.Resolve(this.m_requestContext.Elevate(), this.m_projectId, references).ToList<VariableGroup>();
      Dictionary<int, VariableGroup> dictionary;
      if (this.m_allowImplicitAuthorization)
      {
        dictionary = VariableGroupResolver.Resolve(this.m_requestContext, this.m_projectId, references, actionFilter).ToDictionary<VariableGroup, int>((Func<VariableGroup, int>) (g => g.Id));
        foreach (KeyValuePair<int, VariableGroup> keyValuePair in dictionary)
        {
          VariableGroupReference reference = new VariableGroupReference();
          reference.Id = keyValuePair.Value.Id;
          reference.Name = (ExpressionValue<string>) keyValuePair.Value.Name;
          this.Authorize(reference);
        }
      }
      else
        dictionary = new Dictionary<int, VariableGroup>();
      foreach (VariableGroupReference reference in (IEnumerable<VariableGroupReference>) references)
        VariableGroupResolver.ResolveVariableGroupId(reference, list);
      foreach (VariableGroupReference authorizedReference in this.m_authorizedReferences)
        VariableGroupResolver.ResolveVariableGroupId(authorizedReference, list);
      foreach (VariableGroupReference reference1 in (IEnumerable<VariableGroupReference>) references)
      {
        VariableGroupReference reference = reference1;
        if (!dictionary.ContainsKey(reference.Id) && this.m_authorizedReferences.Any<VariableGroupReference>((Func<VariableGroupReference, bool>) (x => x.Id == reference.Id)))
        {
          VariableGroup variableGroup = list.Find((Predicate<VariableGroup>) (f => f.Id == reference.Id));
          if (variableGroup != null)
            dictionary.Add(variableGroup.Id, variableGroup);
        }
      }
      return (IList<VariableGroup>) dictionary.Values.ToList<VariableGroup>();
    }

    public IList<VariableGroupReference> GetAuthorizedReferences() => (IList<VariableGroupReference>) this.m_authorizedReferences.ToList<VariableGroupReference>();

    public void Authorize(VariableGroupReference reference) => this.m_authorizedReferences.Add(reference);

    internal static IList<VariableGroup> Resolve(
      IVssRequestContext requestContext,
      Guid projectId,
      ICollection<VariableGroupReference> references,
      ResourceActionFilter actionFilter = ResourceActionFilter.Use)
    {
      VariableGroupActionFilter groupActionFilter = VariableGroupResolver.ParseToVariableGroupActionFilter(actionFilter);
      List<VariableGroup> variableGroupList = new List<VariableGroup>();
      if (references != null && references.Count > 0)
      {
        IVariableGroupService service = requestContext.GetService<IVariableGroupService>();
        List<int> list1 = references.Where<VariableGroupReference>((Func<VariableGroupReference, bool>) (x => x.Id != 0)).Select<VariableGroupReference, int>((Func<VariableGroupReference, int>) (x => x.Id)).ToList<int>();
        List<string> list2 = references.Where<VariableGroupReference>((Func<VariableGroupReference, bool>) (x => x.Id == 0 && !string.IsNullOrEmpty(x.Name?.Literal))).Select<VariableGroupReference, string>((Func<VariableGroupReference, string>) (x => x.Name.Literal)).ToList<string>();
        if (list2.Count > 0)
        {
          foreach (string groupName in list2)
          {
            VariableGroup variableGroup = service.GetVariableGroups(requestContext, projectId, groupName, groupActionFilter).FirstOrDefault<VariableGroup>();
            if (variableGroup != null)
              list1.Add(variableGroup.Id);
          }
        }
        if (list1.Count > 0)
        {
          IList<VariableGroup> variableGroups = service.GetVariableGroups(requestContext, projectId, (IList<int>) list1, groupActionFilter);
          variableGroupList.AddRange((IEnumerable<VariableGroup>) variableGroups);
        }
      }
      return (IList<VariableGroup>) variableGroupList;
    }

    private static void ResolveVariableGroupId(
      VariableGroupReference reference,
      List<VariableGroup> resolvedGroups)
    {
      if (reference.Id != 0)
        return;
      VariableGroup variableGroup = resolvedGroups.Find((Predicate<VariableGroup>) (f => f.Name.Equals(reference.Name.Literal, StringComparison.OrdinalIgnoreCase)));
      if (variableGroup == null)
        return;
      reference.Id = variableGroup.Id;
    }

    private static VariableGroupActionFilter ParseToVariableGroupActionFilter(
      ResourceActionFilter actionFilter)
    {
      VariableGroupActionFilter groupActionFilter = VariableGroupActionFilter.Use;
      switch (actionFilter)
      {
        case ResourceActionFilter.None:
          groupActionFilter = VariableGroupActionFilter.None;
          break;
        case ResourceActionFilter.Manage:
          groupActionFilter = VariableGroupActionFilter.Manage;
          break;
        case ResourceActionFilter.Use:
          groupActionFilter = VariableGroupActionFilter.Use;
          break;
      }
      return groupActionFilter;
    }
  }
}
