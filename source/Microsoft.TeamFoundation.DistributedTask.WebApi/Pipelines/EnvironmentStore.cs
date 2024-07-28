// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.EnvironmentStore
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class EnvironmentStore : IEnvironmentStore
  {
    private IEnvironmentResolver m_resolver;
    private IDictionary<string, EnvironmentInstance> m_environmentsByName;
    private IDictionary<int, EnvironmentInstance> m_environmentsById;

    public EnvironmentStore(IList<EnvironmentInstance> environments, IEnvironmentResolver resolver = null)
    {
      this.m_resolver = resolver;
      this.m_environmentsByName = (IDictionary<string, EnvironmentInstance>) new Dictionary<string, EnvironmentInstance>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.m_environmentsById = (IDictionary<int, EnvironmentInstance>) new Dictionary<int, EnvironmentInstance>();
      this.Add(environments != null ? environments.ToArray<EnvironmentInstance>() : (EnvironmentInstance[]) null);
    }

    public void Add(params EnvironmentInstance[] environments)
    {
      if (environments == null)
        return;
      foreach (EnvironmentInstance environment in environments)
      {
        if (environment != null)
        {
          this.m_environmentsById[environment.Id] = environment;
          string name = environment.Name;
          if (!string.IsNullOrWhiteSpace(name))
            this.m_environmentsByName[name] = environment;
        }
      }
    }

    public EnvironmentInstance ResolveEnvironment(string name)
    {
      EnvironmentInstance environmentInstance;
      if (!this.m_environmentsByName.TryGetValue(name, out environmentInstance) && this.m_resolver != null)
      {
        environmentInstance = this.m_resolver?.Resolve(name);
        this.Add(environmentInstance);
      }
      return environmentInstance;
    }

    public EnvironmentInstance ResolveEnvironment(int id)
    {
      EnvironmentInstance environmentInstance;
      if (!this.m_environmentsById.TryGetValue(id, out environmentInstance) && this.m_resolver != null)
      {
        environmentInstance = this.m_resolver?.Resolve(id);
        this.Add(environmentInstance);
      }
      return environmentInstance;
    }

    public EnvironmentInstance Get(EnvironmentReference reference)
    {
      if (reference == null)
        return (EnvironmentInstance) null;
      ExpressionValue<string> name = reference.Name;
      return ((object) name != null ? (name.IsLiteral ? 1 : 0) : 0) != 0 ? this.ResolveEnvironment(reference.Name.Literal) : this.ResolveEnvironment(reference.Id);
    }

    public IList<EnvironmentReference> GetReferences() => (IList<EnvironmentReference>) this.m_environmentsById.Values.Select<EnvironmentInstance, EnvironmentReference>((Func<EnvironmentInstance, EnvironmentReference>) (x =>
    {
      return new EnvironmentReference()
      {
        Id = x.Id,
        Name = (ExpressionValue<string>) x.Name
      };
    })).ToList<EnvironmentReference>();
  }
}
