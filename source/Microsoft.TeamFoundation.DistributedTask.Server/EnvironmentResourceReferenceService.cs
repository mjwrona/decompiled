// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.EnvironmentResourceReferenceService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Server.DataAccess;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  public class EnvironmentResourceReferenceService : 
    IEnvironmentResourceReferenceService,
    IVssFrameworkService
  {
    private string c_layer = nameof (EnvironmentResourceReferenceService);
    private readonly EnvironmentSecurityProvider m_securityProvider;

    public EnvironmentResourceReferenceService()
      : this(new EnvironmentSecurityProvider())
    {
    }

    protected EnvironmentResourceReferenceService(EnvironmentSecurityProvider security) => this.m_securityProvider = security;

    public async Task<IPagedList<EnvironmentResourceReference>> GetEnvironmentResourceReferencesAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      string environmentName,
      int? resourceId = null,
      string resourceName = "",
      EnvironmentResourceType? resourceType = null,
      IList<string> tagFilters = null,
      int top = 50,
      string continuationToken = "")
    {
      IPagedList<EnvironmentResourceReference> referencesInternalAsync;
      using (new MethodScope(requestContext, this.c_layer, nameof (GetEnvironmentResourceReferencesAsync)))
      {
        int id = requestContext.GetService<EnvironmentService>().GetEnvironmentByName(requestContext, projectId, environmentName, EnvironmentActionFilter.None, false, false).Id;
        referencesInternalAsync = await this.GetEnvironmentResourceReferencesInternalAsync(requestContext, projectId, id, resourceId, resourceName, resourceType, tagFilters, top, continuationToken);
      }
      return referencesInternalAsync;
    }

    public async Task<IPagedList<EnvironmentResourceReference>> GetEnvironmentResourceReferencesAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int environmentId,
      int? resourceId = null,
      string resourceName = "",
      EnvironmentResourceType? resourceType = null,
      IList<string> tagFilters = null,
      int top = 50,
      string continuationToken = "")
    {
      IPagedList<EnvironmentResourceReference> referencesInternalAsync;
      using (new MethodScope(requestContext, this.c_layer, nameof (GetEnvironmentResourceReferencesAsync)))
      {
        requestContext.GetService<EnvironmentService>().GetEnvironmentById(requestContext, projectId, environmentId, EnvironmentActionFilter.None, false, false);
        referencesInternalAsync = await this.GetEnvironmentResourceReferencesInternalAsync(requestContext, projectId, environmentId, resourceId, resourceName, resourceType, tagFilters, top, continuationToken);
      }
      return referencesInternalAsync;
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    private IList<EnvironmentResourceReference> FilterEnvironmentResourceReferences(
      IList<EnvironmentResourceReference> resources,
      int? resourceId,
      string resourceName,
      EnvironmentResourceType? resourceType,
      IList<string> tagFilters)
    {
      if (resourceType.HasValue)
        resources = (IList<EnvironmentResourceReference>) resources.Where<EnvironmentResourceReference>((Func<EnvironmentResourceReference, bool>) (r =>
        {
          int type = (int) r.Type;
          EnvironmentResourceType? nullable = resourceType;
          int valueOrDefault = (int) nullable.GetValueOrDefault();
          return type == valueOrDefault & nullable.HasValue;
        })).ToList<EnvironmentResourceReference>();
      if (resourceId.HasValue)
        resources = (IList<EnvironmentResourceReference>) resources.Where<EnvironmentResourceReference>((Func<EnvironmentResourceReference, bool>) (r => r.Id == resourceId.Value)).ToList<EnvironmentResourceReference>();
      if (!string.IsNullOrEmpty(resourceName))
        resources = (IList<EnvironmentResourceReference>) resources.Where<EnvironmentResourceReference>((Func<EnvironmentResourceReference, bool>) (r => r.Name.ToLowerInvariant() == resourceName.ToLowerInvariant())).ToList<EnvironmentResourceReference>();
      if (tagFilters.Count > 0)
        resources = (IList<EnvironmentResourceReference>) EnvironmentResourceReferenceService.MatchTagFilters(resources, tagFilters);
      return resources;
    }

    private static List<EnvironmentResourceReference> MatchTagFilters(
      IList<EnvironmentResourceReference> resources,
      IList<string> tagFilters)
    {
      List<EnvironmentResourceReference> resourceReferenceList = new List<EnvironmentResourceReference>();
      foreach (EnvironmentResourceReference resource in (IEnumerable<EnvironmentResourceReference>) resources)
      {
        bool flag = true;
        foreach (string tagFilter in (IEnumerable<string>) tagFilters)
        {
          if (!resource.Tags.Contains<string>(tagFilter, (IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase))
          {
            flag = false;
            break;
          }
        }
        if (flag)
          resourceReferenceList.Add(resource);
      }
      return resourceReferenceList;
    }

    private async Task<IPagedList<EnvironmentResourceReference>> GetEnvironmentResourceReferencesInternalAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int environmentId,
      int? resourceId = null,
      string resourceName = "",
      EnvironmentResourceType? resourceType = null,
      IList<string> tagFilters = null,
      int top = 50,
      string continuationToken = "")
    {
      this.m_securityProvider.CheckViewPermissions(requestContext, projectId, environmentId);
      IList<EnvironmentResourceReference> resourceReferenceList = (IList<EnvironmentResourceReference>) new List<EnvironmentResourceReference>();
      string continuationToken1 = string.Empty;
      tagFilters = tagFilters ?? (IList<string>) new List<string>();
      if (resourceId.HasValue || !string.IsNullOrEmpty(resourceName))
      {
        using (EnvironmentResourceReferenceComponent component = requestContext.CreateComponent<EnvironmentResourceReferenceComponent>())
        {
          EnvironmentResourceReference referenceByIdOrName = component.GetEnvironmentResourceReferenceByIdOrName(projectId, environmentId, resourceId, resourceName);
          if (referenceByIdOrName == null)
            return (IPagedList<EnvironmentResourceReference>) await Task.FromResult<PagedList<EnvironmentResourceReference>>(new PagedList<EnvironmentResourceReference>((IEnumerable<EnvironmentResourceReference>) resourceReferenceList, continuationToken1));
          resourceReferenceList.Add(referenceByIdOrName);
        }
        resourceReferenceList = this.FilterEnvironmentResourceReferences(resourceReferenceList, resourceId, resourceName, resourceType, tagFilters);
      }
      else
      {
        using (EnvironmentResourceReferenceComponent component = requestContext.CreateComponent<EnvironmentResourceReferenceComponent>())
        {
          List<EnvironmentResourceReference> list = component.GetEnvironmentResourceReferencesByTypeAndTags(projectId, environmentId, resourceType, tagFilters, top + 1, continuationToken).ToList<EnvironmentResourceReference>();
          if (list.Count > top)
          {
            list.Sort((Comparison<EnvironmentResourceReference>) ((r1, r2) => string.Compare(r1.Name, r2.Name, true)));
            continuationToken1 = list[top].Name;
            list = list.Take<EnvironmentResourceReference>(top).ToList<EnvironmentResourceReference>();
          }
          resourceReferenceList = (IList<EnvironmentResourceReference>) list;
        }
      }
      return (IPagedList<EnvironmentResourceReference>) await Task.FromResult<PagedList<EnvironmentResourceReference>>(new PagedList<EnvironmentResourceReference>((IEnumerable<EnvironmentResourceReference>) resourceReferenceList, continuationToken1));
    }
  }
}
