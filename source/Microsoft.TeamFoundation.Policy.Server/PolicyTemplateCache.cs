// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Policy.Server.PolicyTemplateCache
// Assembly: Microsoft.TeamFoundation.Policy.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C7B03386-B27B-4823-BB4F-89F7D7E42DDD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Policy.Server.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Policy.Server
{
  internal sealed class PolicyTemplateCache : 
    IReadOnlyDictionary<Guid, ITeamFoundationPolicy>,
    IReadOnlyCollection<KeyValuePair<Guid, ITeamFoundationPolicy>>,
    IEnumerable<KeyValuePair<Guid, ITeamFoundationPolicy>>,
    IEnumerable,
    IDisposable
  {
    private Dictionary<Guid, ITeamFoundationPolicy> m_templates;
    private IReadOnlyList<ITeamFoundationPolicy> m_allPolicies;

    internal PolicyTemplateCache(IReadOnlyList<ITeamFoundationPolicy> allPolicies)
    {
      this.m_allPolicies = allPolicies;
      this.m_templates = allPolicies.ToDictionary<ITeamFoundationPolicy, Guid>((Func<ITeamFoundationPolicy, Guid>) (p => p.Id));
    }

    public ITeamFoundationPolicy this[Guid key] => this.m_templates[key];

    public bool IsDynamicPolicy(Guid key)
    {
      ITeamFoundationPolicy foundationPolicy;
      this.m_templates.TryGetValue(key, out foundationPolicy);
      return foundationPolicy is IDynamicEvaluationPolicy;
    }

    public int Count => this.m_templates.Count;

    public IEnumerable<Guid> Keys => (IEnumerable<Guid>) this.m_templates.Keys;

    public IEnumerable<ITeamFoundationPolicy> Values => (IEnumerable<ITeamFoundationPolicy>) this.m_templates.Values;

    public bool ContainsKey(Guid key) => this.m_templates.ContainsKey(key);

    public IEnumerator<KeyValuePair<Guid, ITeamFoundationPolicy>> GetEnumerator() => (IEnumerator<KeyValuePair<Guid, ITeamFoundationPolicy>>) this.m_templates.GetEnumerator();

    public bool TryGetValue(Guid key, out ITeamFoundationPolicy value) => this.m_templates.TryGetValue(key, out value);

    internal TPolicy CreateInstance<TPolicy>(ITeamFoundationPolicy template) where TPolicy : ITeamFoundationPolicy => (TPolicy) Activator.CreateInstance(template.GetType());

    internal bool IsOfType<T>(Guid id)
    {
      ITeamFoundationPolicy foundationPolicy;
      return this.TryGetValue(id, out foundationPolicy) && foundationPolicy is T;
    }

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.m_templates.GetEnumerator();

    void IDisposable.Dispose()
    {
      if (this.m_allPolicies == null)
        return;
      if (this.m_allPolicies is IDisposable allPolicies)
        allPolicies.Dispose();
      this.m_allPolicies = (IReadOnlyList<ITeamFoundationPolicy>) null;
    }
  }
}
