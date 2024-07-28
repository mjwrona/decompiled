// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.EnvironmentResourceService`1
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Server.DataAccess;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal abstract class EnvironmentResourceService<T> : 
    IEnvironmentResourceService<T>,
    IVssFrameworkService
    where T : EnvironmentResource
  {
    protected readonly Func<IVssRequestContext, Microsoft.VisualStudio.Services.Identity.Identity> m_getCurrentUserIdentity;
    protected readonly EnvironmentSecurityProvider m_securityProvider;
    private Func<IVssRequestContext, Microsoft.VisualStudio.Services.Identity.Identity, IdentityRef> m_toIdentityRef;

    public EnvironmentResourceService()
      : this((Func<IVssRequestContext, Microsoft.VisualStudio.Services.Identity.Identity>) (rc => rc.GetUserIdentity()), (Func<IVssRequestContext, Microsoft.VisualStudio.Services.Identity.Identity, IdentityRef>) ((rc, id) => id.ToIdentityRef(rc)))
    {
    }

    protected EnvironmentResourceService(
      Func<IVssRequestContext, Microsoft.VisualStudio.Services.Identity.Identity> getCurrentUserIdentity,
      Func<IVssRequestContext, Microsoft.VisualStudio.Services.Identity.Identity, IdentityRef> toIdentityRef)
    {
      this.m_securityProvider = new EnvironmentSecurityProvider();
      this.m_getCurrentUserIdentity = getCurrentUserIdentity;
      this.m_toIdentityRef = toIdentityRef;
    }

    public virtual async Task<T> AddEnvironmentResourceAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      T resource)
    {
      T obj;
      using (new MethodScope(requestContext, this.c_layer, nameof (AddEnvironmentResourceAsync)))
      {
        this.ValidateCreateInputParameters(requestContext, projectId, resource);
        this.m_securityProvider.CheckManagePermissions(requestContext, projectId, resource.EnvironmentReference.Id);
        resource.CreatedBy.Id = this.m_getCurrentUserIdentity(requestContext).Id.ToString();
        T newlyAddedResource = this.AddResourceEntry(requestContext, projectId, resource);
        this.PopulateIdentityReference(requestContext, newlyAddedResource);
        await this.AuthorizePipelineResourceAsync(requestContext, projectId, newlyAddedResource);
        obj = newlyAddedResource;
      }
      return obj;
    }

    public virtual T GetEnvironmentResource(
      IVssRequestContext requestContext,
      Guid projectId,
      int environmentId,
      int resourceId)
    {
      using (new MethodScope(requestContext, this.c_layer, nameof (GetEnvironmentResource)))
      {
        this.m_securityProvider.CheckViewPermissions(requestContext, projectId, environmentId);
        T resourceInternal = this.GetResourceInternal(requestContext, projectId, environmentId, resourceId);
        this.PopulateIdentityReference(requestContext, resourceInternal);
        return resourceInternal;
      }
    }

    public virtual IList<T> GetEnvironmentResourcesById(
      IVssRequestContext requestContext,
      Guid projectId,
      int environmentId,
      IEnumerable<int> resourceIds)
    {
      using (new MethodScope(requestContext, this.c_layer, nameof (GetEnvironmentResourcesById)))
      {
        this.m_securityProvider.CheckViewPermissions(requestContext, projectId, environmentId);
        IList<T> resourcesByIdInternal = this.GetResourcesByIdInternal(requestContext, projectId, environmentId, resourceIds);
        foreach (T resource in (IEnumerable<T>) resourcesByIdInternal)
          this.PopulateIdentityReference(requestContext, resource);
        return resourcesByIdInternal;
      }
    }

    public virtual T UpdateEnvironmentResource(
      IVssRequestContext requestContext,
      Guid projectId,
      T resource)
    {
      using (new MethodScope(requestContext, this.c_layer, nameof (UpdateEnvironmentResource)))
      {
        ArgumentValidation.CheckResource((EnvironmentResource) resource);
        this.ValidateUpdateParameters(requestContext, projectId, resource);
        this.m_securityProvider.CheckManagePermissions(requestContext, projectId, resource.EnvironmentReference.Id);
        Microsoft.VisualStudio.Services.Identity.Identity identity = this.m_getCurrentUserIdentity(requestContext);
        resource.LastModifiedBy.Id = identity.Id.ToString();
        T resource1;
        using (IEnvironmentResourceComponent<T> resourceComponent = this.GetResourceComponent(requestContext))
          resource1 = resourceComponent.UpdateEnvironmentResource(projectId, resource);
        this.PopulateIdentityReference(requestContext, resource1);
        return resource1;
      }
    }

    public virtual void DeleteEnvironmentResource(
      IVssRequestContext requestContext,
      Guid projectId,
      int environmentId,
      int resourceId)
    {
      using (new MethodScope(requestContext, this.c_layer, nameof (DeleteEnvironmentResource)))
      {
        this.m_securityProvider.CheckManagePermissions(requestContext, projectId, environmentId);
        Guid id = this.m_getCurrentUserIdentity(requestContext).Id;
        this.DeleteResourceEntry(requestContext, projectId, environmentId, resourceId, id);
      }
    }

    public virtual void DeleteEnvironmentResources(
      IVssRequestContext requestContext,
      Guid projectId,
      int environmentId)
    {
      using (new MethodScope(requestContext, this.c_layer, nameof (DeleteEnvironmentResources)))
      {
        this.m_securityProvider.CheckManagePermissions(requestContext, projectId, environmentId);
        Guid id = this.m_getCurrentUserIdentity(requestContext).Id;
        this.DeleteResourceEntries(requestContext, projectId, environmentId, id);
      }
    }

    protected virtual void ValidateCreateInputParameters(
      IVssRequestContext requestContext,
      Guid projectId,
      T resource)
    {
      ArgumentValidation.CheckResource((EnvironmentResource) resource);
    }

    protected virtual async Task AuthorizePipelineResourceAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      T resource)
    {
      await Task.CompletedTask;
    }

    protected virtual void ValidateUpdateParameters(
      IVssRequestContext requestContext,
      Guid projectId,
      T resource)
    {
      ArgumentValidation.CheckResource((EnvironmentResource) resource);
    }

    protected T GetResourceInternal(
      IVssRequestContext requestContext,
      Guid projectId,
      int environmentId,
      int resourceId)
    {
      T environmentResource;
      using (IEnvironmentResourceComponent<T> resourceComponent = this.GetResourceComponent(requestContext))
      {
        environmentResource = resourceComponent.GetEnvironmentResource(projectId, environmentId, resourceId);
        if ((object) environmentResource == null)
          throw new EnvironmentResourceNotFoundException(TaskResources.EnvironmentResourceNotFound((object) resourceId, (object) environmentId));
      }
      return environmentResource;
    }

    protected IList<T> GetResourcesByIdInternal(
      IVssRequestContext requestContext,
      Guid projectId,
      int environmentId,
      IEnumerable<int> resourceIds = null)
    {
      using (IEnvironmentResourceComponent<T> resourceComponent = this.GetResourceComponent(requestContext))
        return resourceComponent.GetEnvironmentResourcesById(projectId, environmentId, resourceIds);
    }

    protected virtual T AddResourceEntry(
      IVssRequestContext requestContext,
      Guid projectId,
      T resource)
    {
      using (IEnvironmentResourceComponent<T> resourceComponent = this.GetResourceComponent(requestContext))
        return resourceComponent.AddEnvironmentResource(projectId, resource);
    }

    protected virtual void DeleteResourceEntry(
      IVssRequestContext requestContext,
      Guid projectId,
      int environmentId,
      int resourceId,
      Guid deletedBy)
    {
      using (IEnvironmentResourceComponent<T> resourceComponent = this.GetResourceComponent(requestContext))
        resourceComponent.DeleteEnvironmentResource(projectId, environmentId, resourceId, deletedBy);
    }

    protected virtual void DeleteResourceEntries(
      IVssRequestContext requestContext,
      Guid projectId,
      int environmentId,
      Guid deletedBy)
    {
      using (IEnvironmentResourceComponent<T> resourceComponent = this.GetResourceComponent(requestContext))
        resourceComponent.DeleteEnvironmentResources(projectId, environmentId, deletedBy);
    }

    protected abstract IEnvironmentResourceComponent<T> GetResourceComponent(
      IVssRequestContext requestContext);

    private void PopulateIdentityReference(IVssRequestContext requestContext, T resource)
    {
      Guid createdById = new Guid(resource.CreatedBy.Id);
      Guid modifiedById = new Guid(resource.LastModifiedBy.Id);
      List<Microsoft.VisualStudio.Services.Identity.Identity> list = requestContext.GetService<IdentityService>().GetIdentities(requestContext, (IEnumerable<Guid>) new Guid[2]
      {
        createdById,
        modifiedById
      }).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
      Microsoft.VisualStudio.Services.Identity.Identity identity1 = list.FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (ri => ri.Id == createdById));
      if (identity1 != null)
        resource.CreatedBy = this.m_toIdentityRef(requestContext, identity1);
      Microsoft.VisualStudio.Services.Identity.Identity identity2 = list.FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (ri => ri.Id == modifiedById));
      if (identity2 == null)
        return;
      resource.LastModifiedBy = this.m_toIdentityRef(requestContext, identity2);
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext)
    {
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext)
    {
    }

    protected abstract string c_layer { get; }
  }
}
