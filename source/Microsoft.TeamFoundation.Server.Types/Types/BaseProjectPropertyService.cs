// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Types.BaseProjectPropertyService
// Assembly: Microsoft.TeamFoundation.Server.Types, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC707FA3-32BF-41E4-BD8A-1BB971125382
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Types.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.Types
{
  internal abstract class BaseProjectPropertyService : IVssFrameworkService
  {
    private ILockName m_propertyLock;
    private ConcurrentDictionary<Guid, int> m_propertyVersions;
    private static readonly char[] s_wildcardCharacters = new char[5]
    {
      '*',
      '?',
      '[',
      '%',
      '_'
    };
    private const string c_area = "Project";
    private const string c_layer = "BaseProjectPropertyService";
    private const int c_initialVersion = 0;

    public virtual void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckProjectCollectionRequestContext();
      this.m_propertyLock = systemRequestContext.ServiceHost.CreateUniqueLockName(typeof (BaseProjectPropertyService).FullName);
      this.m_propertyVersions = new ConcurrentDictionary<Guid, int>();
    }

    public virtual void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public IEnumerable<ProjectProperty> GetProperties(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<string> filters)
    {
      bool castedValueOrDefault = requestContext.Items.GetCastedValueOrDefault<string, bool>("BypassPropertyCache");
      return this.GetProperties(requestContext, (IEnumerable<Guid>) new Guid[1]
      {
        projectId
      }, filters, (castedValueOrDefault ? 1 : 0) != 0)[projectId];
    }

    public IEnumerable<ProjectProperties> GetProjectsProperties(
      IVssRequestContext requestContext,
      IEnumerable<Guid> projectIds,
      IEnumerable<string> filters)
    {
      bool castedValueOrDefault = requestContext.Items.GetCastedValueOrDefault<string, bool>("BypassPropertyCache");
      return this.GetProperties(requestContext, projectIds, filters, castedValueOrDefault).Select<KeyValuePair<Guid, IEnumerable<ProjectProperty>>, ProjectProperties>((Func<KeyValuePair<Guid, IEnumerable<ProjectProperty>>, ProjectProperties>) (kvp => new ProjectProperties()
      {
        ProjectId = kvp.Key,
        Properties = kvp.Value
      }));
    }

    public void SetProperties(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<ProjectProperty> properties)
    {
      this.WriteToSource(requestContext.Elevate(), projectId, properties);
      this.RemoveCache(requestContext, projectId, properties.Select<ProjectProperty, string>((Func<ProjectProperty, string>) (p => p.Name)));
    }

    protected IReadOnlyDictionary<Guid, IEnumerable<ProjectProperty>> GetProperties(
      IVssRequestContext requestContext,
      IEnumerable<Guid> projectIds,
      IEnumerable<string> filters,
      bool bypassCache)
    {
      projectIds = (IEnumerable<Guid>) projectIds.Distinct<Guid>().ToList<Guid>();
      if (bypassCache)
      {
        IProjectService service = requestContext.GetService<IProjectService>();
        foreach (Guid projectId in projectIds)
          service.CheckProjectPermission(requestContext, ProjectInfo.GetProjectUri(projectId), TeamProjectPermissions.BypassPropertyCache, false);
      }
      requestContext = requestContext.Elevate();
      filters = (IEnumerable<string>) filters.Distinct<string>((IEqualityComparer<string>) TFStringComparer.TeamProjectPropertyName).ToList<string>();
      Dictionary<Guid, Dictionary<string, ProjectProperty>> dictionary = new Dictionary<Guid, Dictionary<string, ProjectProperty>>();
      Func<Dictionary<string, ProjectProperty>> createValueToAdd = (Func<Dictionary<string, ProjectProperty>>) (() => new Dictionary<string, ProjectProperty>((IEqualityComparer<string>) TFStringComparer.TeamProjectPropertyName));
      HashSet<Guid> guidSet = new HashSet<Guid>();
      HashSet<string> stringSet = new HashSet<string>();
      if (bypassCache)
      {
        guidSet.AddRange<Guid, HashSet<Guid>>(projectIds);
        stringSet.AddRange<string, HashSet<string>>(filters);
      }
      else
      {
        ProjectPropertyCacheService service = requestContext.GetService<ProjectPropertyCacheService>();
        foreach (Guid projectId in projectIds)
        {
          Dictionary<string, ProjectProperty> orAddValue = dictionary.GetOrAddValue<Guid, Dictionary<string, ProjectProperty>>(projectId, createValueToAdd);
          foreach (string filter in filters)
          {
            ProjectProperty projectProperty;
            if (service.TryGetValue(requestContext, new ProjectPropertyName(projectId, filter), out projectProperty))
            {
              if (projectProperty != null)
                orAddValue.Add(filter, projectProperty.Clone());
            }
            else
            {
              guidSet.Add(projectId);
              stringSet.Add(filter);
            }
          }
        }
      }
      if (guidSet.Count > 0)
      {
        List<string> list = stringSet.Where<string>((Func<string, bool>) (f => f.IndexOfAny(BaseProjectPropertyService.s_wildcardCharacters) < 0)).ToList<string>();
        foreach (KeyValuePair<Guid, IEnumerable<ProjectProperty>> keyValuePair in list.Count <= 0 ? (IEnumerable<KeyValuePair<Guid, IEnumerable<ProjectProperty>>>) this.ReadFromSource(requestContext, (IEnumerable<Guid>) guidSet, (IEnumerable<string>) stringSet) : (IEnumerable<KeyValuePair<Guid, IEnumerable<ProjectProperty>>>) this.ReadFromSourceAndSetCache(requestContext, (IEnumerable<Guid>) guidSet, (IEnumerable<string>) stringSet, (IEnumerable<string>) list))
        {
          Dictionary<string, ProjectProperty> orAddValue = dictionary.GetOrAddValue<Guid, Dictionary<string, ProjectProperty>>(keyValuePair.Key, createValueToAdd);
          foreach (ProjectProperty projectProperty in keyValuePair.Value)
            orAddValue.TryAdd<string, ProjectProperty>(projectProperty.Name, projectProperty);
        }
      }
      return (IReadOnlyDictionary<Guid, IEnumerable<ProjectProperty>>) dictionary.ToDictionary<KeyValuePair<Guid, Dictionary<string, ProjectProperty>>, Guid, IEnumerable<ProjectProperty>>((Func<KeyValuePair<Guid, Dictionary<string, ProjectProperty>>, Guid>) (r => r.Key), (Func<KeyValuePair<Guid, Dictionary<string, ProjectProperty>>, IEnumerable<ProjectProperty>>) (r => r.Value.Values.AsEnumerable<ProjectProperty>()));
    }

    protected void RemoveCache(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<string> propertyNames)
    {
      ProjectPropertyCacheService service = requestContext.GetService<ProjectPropertyCacheService>();
      using (requestContext.Lock(this.m_propertyLock))
      {
        int orAdd = this.m_propertyVersions.GetOrAdd(projectId, 0);
        int num;
        this.m_propertyVersions[projectId] = num = orAdd + 1;
        foreach (string propertyName in propertyNames)
        {
          service.Remove(requestContext, new ProjectPropertyName(projectId, propertyName));
          requestContext.Trace(5500272, TraceLevel.Info, "Project", nameof (BaseProjectPropertyService), "Removed the cache for property '{0}' of project '{1}'.", (object) propertyName, (object) projectId);
        }
      }
    }

    private IReadOnlyDictionary<Guid, IEnumerable<ProjectProperty>> ReadFromSourceAndSetCache(
      IVssRequestContext requestContext,
      IEnumerable<Guid> projectIds,
      IEnumerable<string> filters,
      IEnumerable<string> propertiesToCache)
    {
      IReadOnlyDictionary<Guid, int> dictionary1 = (IReadOnlyDictionary<Guid, int>) projectIds.ToDictionary<Guid, Guid, int>((Func<Guid, Guid>) (id => id), (Func<Guid, int>) (id => this.GetCurrentVersion(id)));
      IReadOnlyDictionary<Guid, IEnumerable<ProjectProperty>> readOnlyDictionary = this.ReadFromSource(requestContext, projectIds, filters);
      ProjectPropertyCacheService service = requestContext.GetService<ProjectPropertyCacheService>();
      foreach (KeyValuePair<Guid, IEnumerable<ProjectProperty>> keyValuePair in (IEnumerable<KeyValuePair<Guid, IEnumerable<ProjectProperty>>>) readOnlyDictionary)
      {
        Guid key1 = keyValuePair.Key;
        int num = dictionary1[key1];
        using (requestContext.Lock(this.m_propertyLock))
        {
          int currentVersion = this.GetCurrentVersion(key1);
          if (num == currentVersion)
          {
            Dictionary<string, ProjectProperty> dictionary2 = keyValuePair.Value.ToDictionary<ProjectProperty, string>((Func<ProjectProperty, string>) (p => p.Name), (IEqualityComparer<string>) TFStringComparer.TeamProjectPropertyName);
            foreach (string str in propertiesToCache)
            {
              ProjectPropertyName key2 = new ProjectPropertyName(key1, str);
              ProjectProperty projectProperty;
              if (dictionary2.TryGetValue(str, out projectProperty))
                service.Set(requestContext, key2, projectProperty.Clone());
              else
                service.Set(requestContext, key2, (ProjectProperty) null);
            }
          }
          else
            requestContext.Trace(5500270, TraceLevel.Info, "Project", nameof (BaseProjectPropertyService), "Did not set cache entries for project '{0}' because of version mismatch. Expected: {1}. Actual: {2}.", (object) key1, (object) num, (object) currentVersion);
        }
      }
      return readOnlyDictionary;
    }

    private int GetCurrentVersion(Guid projectId)
    {
      int num;
      return !this.m_propertyVersions.TryGetValue(projectId, out num) ? 0 : num;
    }

    protected abstract IReadOnlyDictionary<Guid, IEnumerable<ProjectProperty>> ReadFromSource(
      IVssRequestContext requestContext,
      IEnumerable<Guid> projectIds,
      IEnumerable<string> filters);

    protected abstract void WriteToSource(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<ProjectProperty> properties);
  }
}
