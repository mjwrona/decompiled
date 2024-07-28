// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.EnvironmentService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.DistributedTask.Core.Server.Plugins;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.Server.Converters;
using Microsoft.TeamFoundation.DistributedTask.Server.Data.Model;
using Microsoft.TeamFoundation.DistributedTask.Server.DataAccess;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.DistributedTask.WebApi.Events;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebPlatform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal class EnvironmentService : IEnvironmentService, IVssFrameworkService
  {
    private readonly EnvironmentSecurityProvider securityProvider;
    private readonly RegistryQuery s_readDataFromReplica = new RegistryQuery("/Service/DistributedTaskServer/Settings/ReadAXDataFromReplica");

    internal EnvironmentService()
      : this(new EnvironmentSecurityProvider())
    {
    }

    protected EnvironmentService(EnvironmentSecurityProvider security) => this.securityProvider = security;

    public EnvironmentInstance AddEnvironment(
      IVssRequestContext requestContext,
      Guid projectId,
      EnvironmentInstance environment,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> administrators = null,
      string clientSource = "")
    {
      using (new MethodScope(requestContext, nameof (EnvironmentService), nameof (AddEnvironment)))
      {
        ArgumentValidation.CheckEnvironment(environment, nameof (environment));
        this.securityProvider.CheckCreatePermissions(requestContext, projectId);
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
        string collectionEnvironmentName = this.GetCollectionEnvironmentName(requestContext, projectId, environment);
        ArgumentValidation.CheckEnvironmentName(ref collectionEnvironmentName, "collectionEnvironment", false);
        using (EnvironmentComponent component = requestContext.CreateComponent<EnvironmentComponent>())
        {
          try
          {
            environment = component.AddEnvironment(environment.Name, collectionEnvironmentName, projectId, userIdentity.Id, environment.Description);
          }
          catch (EnvironmentExistsException ex)
          {
            string withRandomString = this.GetCollectionEnvironmentNameWithRandomString(collectionEnvironmentName);
            requestContext.TraceInfo(nameof (EnvironmentService), "Trying to create environment again with collection environment name : {0}", (object) withRandomString);
            environment = component.AddEnvironment(environment.Name, withRandomString, projectId, userIdentity.Id, environment.Description);
          }
        }
        requestContext.TraceInfo(10015213, nameof (EnvironmentService), "Adding environment with Id: {0}.", (object) environment.Id);
        EnvironmentTelemetryFactory.GetLogger(requestContext).PublishAddEnvironment(requestContext, environment, clientSource);
        EnvironmentService.PopulateIdentityReference(requestContext, environment);
        if (administrators == null)
          administrators = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new List<Microsoft.VisualStudio.Services.Identity.Identity>();
        administrators.Add(userIdentity);
        EnvironmentService.GrantAdministratorPrivileges(requestContext.Elevate(), projectId, environment.Id, administrators);
        this.FireEnvironmentAddedEvent(requestContext, environment);
        return environment;
      }
    }

    public string GeneratePersonalAccessTokenWithEnvironmentScope(
      IVssRequestContext requestContext,
      Guid projectId,
      int environmentId)
    {
      using (new MethodScope(requestContext, nameof (EnvironmentService), nameof (GeneratePersonalAccessTokenWithEnvironmentScope)))
      {
        this.CheckViewAndAdditionalPermissions(requestContext, projectId, environmentId, EnvironmentActionFilter.Manage);
        string tokenName = string.Format("Environment-Personal Access Token {0}", (object) DateTime.UtcNow);
        return this.GeneratePersonalAccessTokenWithEnvironmentScope(requestContext, tokenName);
      }
    }

    public EnvironmentInstance GetEnvironmentById(
      IVssRequestContext requestContext,
      Guid projectId,
      int environmentId,
      EnvironmentActionFilter actionFilter = EnvironmentActionFilter.None,
      bool includeResourceReferences = false,
      bool includeLinkedResources = false)
    {
      using (new MethodScope(requestContext, nameof (EnvironmentService), nameof (GetEnvironmentById)))
      {
        this.CheckViewAndAdditionalPermissions(requestContext, projectId, environmentId, actionFilter);
        EnvironmentInstance environmentById;
        using (EnvironmentComponent component = requestContext.CreateComponent<EnvironmentComponent>())
        {
          environmentById = component.GetEnvironmentById(environmentId, projectId, includeResourceReferences);
          if (environmentById == null)
            throw new EnvironmentNotFoundException(TaskResources.EnvironmentNotFound((object) environmentId));
        }
        EnvironmentService.PopulateIdentityReference(requestContext, environmentById);
        if (includeResourceReferences & includeLinkedResources)
        {
          using (IDisposableReadOnlyList<IEnvironmentExtension> extensions = requestContext.GetExtensions<IEnvironmentExtension>())
          {
            foreach (IEnvironmentExtension environmentExtension1 in (IEnumerable<IEnvironmentExtension>) extensions)
            {
              IEnvironmentExtension environmentExtension = environmentExtension1;
              List<EnvironmentResourceReference> list = environmentById.Resources.Where<EnvironmentResourceReference>((Func<EnvironmentResourceReference, bool>) (r => r.Type == environmentExtension.ResourceType)).ToList<EnvironmentResourceReference>();
              if (list.Count<EnvironmentResourceReference>() > 0)
                environmentExtension.PopulateLinkedResources(requestContext, projectId, environmentById.Id, (IList<EnvironmentResourceReference>) list);
            }
          }
        }
        return environmentById;
      }
    }

    public EnvironmentInstance GetEnvironmentByName(
      IVssRequestContext requestContext,
      Guid projectId,
      string environmentName,
      EnvironmentActionFilter actionFilter = EnvironmentActionFilter.None,
      bool includeResourceReferences = false,
      bool includeLinkedResources = false)
    {
      using (new MethodScope(requestContext, nameof (EnvironmentService), nameof (GetEnvironmentByName)))
      {
        EnvironmentInstance environmentInstance = EnvironmentService.GetEnvironmentsInternal(requestContext, projectId, environmentName, (string) null).Where<EnvironmentInstance>((Func<EnvironmentInstance, bool>) (e => string.Equals(e.Name, environmentName, StringComparison.CurrentCultureIgnoreCase))).FirstOrDefault<EnvironmentInstance>();
        if (environmentInstance == null)
          throw new EnvironmentNotFoundException(TaskResources.EnvironmentWithGivenNameNotFound((object) environmentName));
        this.CheckViewAndAdditionalPermissions(requestContext, projectId, environmentInstance.Id, actionFilter);
        return includeResourceReferences ? this.GetEnvironmentById(requestContext, projectId, environmentInstance.Id, actionFilter, includeResourceReferences, includeLinkedResources) : environmentInstance;
      }
    }

    public IList<EnvironmentInstance> GetEnvironmentsByModifiedTime(
      IVssRequestContext requestContext,
      Guid projectId,
      DateTime? fromDate,
      int batchSize)
    {
      ArgumentUtility.CheckGreaterThanOrEqualToZero((float) batchSize, nameof (EnvironmentService));
      bool flag = requestContext.GetService<IVssRegistryService>().GetValue<bool>(requestContext, in this.s_readDataFromReplica, true);
      using (EnvironmentComponent component = requestContext.CreateComponent<EnvironmentComponent>(connectionType: new DatabaseConnectionType?(flag ? DatabaseConnectionType.IntentReadOnly : DatabaseConnectionType.Default)))
        return component.GetEnvironmentsByModifiedTime(projectId, fromDate, batchSize);
    }

    public IPagedList<EnvironmentInstance> GetEnvironments(
      IVssRequestContext requestContext,
      Guid projectId,
      string environmentName = null,
      string lastEnvironmentName = null,
      EnvironmentActionFilter actionFilter = EnvironmentActionFilter.None,
      int maxEnvironmentsCount = 50)
    {
      maxEnvironmentsCount = Math.Min(maxEnvironmentsCount, 1000);
      using (new MethodScope(requestContext, nameof (EnvironmentService), nameof (GetEnvironments)))
      {
        string continuationToken;
        List<EnvironmentInstance> list = this.GetFilteredEnvironments(requestContext, projectId, environmentName, lastEnvironmentName, actionFilter, maxEnvironmentsCount + 1, out continuationToken).ToList<EnvironmentInstance>();
        foreach (EnvironmentInstance environment in list)
          EnvironmentService.PopulateIdentityReference(requestContext, environment);
        return (IPagedList<EnvironmentInstance>) new PagedList<EnvironmentInstance>((IEnumerable<EnvironmentInstance>) list, continuationToken);
      }
    }

    public Task<IList<EnvironmentInstance>> GetEnvironmentsByIdsAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<int> environmentIds,
      EnvironmentActionFilter actionFilter = EnvironmentActionFilter.None,
      bool includeResourceReferences = false,
      bool includeLinkedResources = false)
    {
      using (new MethodScope(requestContext, nameof (EnvironmentService), nameof (GetEnvironmentsByIdsAsync)))
      {
        IList<EnvironmentInstance> environmentsByIds;
        using (EnvironmentComponent component = requestContext.CreateComponent<EnvironmentComponent>())
          environmentsByIds = component.GetEnvironmentsByIds(projectId, (IEnumerable<int>) environmentIds, includeResourceReferences);
        IList<EnvironmentInstance> list1 = (IList<EnvironmentInstance>) this.FilterByViewAndActionFilterPermissions(requestContext, projectId, (IEnumerable<EnvironmentInstance>) environmentsByIds, actionFilter).ToList<EnvironmentInstance>();
        foreach (EnvironmentInstance environment in (IEnumerable<EnvironmentInstance>) list1)
          EnvironmentService.PopulateIdentityReference(requestContext, environment);
        if (includeResourceReferences & includeLinkedResources)
        {
          using (IDisposableReadOnlyList<IEnvironmentExtension> extensions = requestContext.GetExtensions<IEnvironmentExtension>())
          {
            foreach (EnvironmentInstance environmentInstance in (IEnumerable<EnvironmentInstance>) list1)
            {
              foreach (IEnvironmentExtension environmentExtension1 in (IEnumerable<IEnvironmentExtension>) extensions)
              {
                IEnvironmentExtension environmentExtension = environmentExtension1;
                List<EnvironmentResourceReference> list2 = environmentInstance.Resources.Where<EnvironmentResourceReference>((Func<EnvironmentResourceReference, bool>) (r => r.Type == environmentExtension.ResourceType)).ToList<EnvironmentResourceReference>();
                if (list2.Count<EnvironmentResourceReference>() > 0)
                  environmentExtension.PopulateLinkedResources(requestContext, projectId, environmentInstance.Id, (IList<EnvironmentResourceReference>) list2);
              }
            }
          }
        }
        return Task.FromResult<IList<EnvironmentInstance>>(list1);
      }
    }

    public EnvironmentInstance UpdateEnvironment(
      IVssRequestContext requestContext,
      Guid projectId,
      int environmentId,
      EnvironmentInstance environment)
    {
      using (new MethodScope(requestContext, nameof (EnvironmentService), nameof (UpdateEnvironment)))
      {
        ArgumentValidation.CheckEnvironment(environment, nameof (environment), false);
        this.securityProvider.CheckManagePermissions(requestContext, projectId, environmentId);
        requestContext.TraceInfo(10015215, nameof (EnvironmentService), "Updating environment environment with Id: {0}.", (object) environment.Id);
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
        using (EnvironmentComponent component = requestContext.CreateComponent<EnvironmentComponent>())
          environment = component.UpdateEnvironment(projectId, environmentId, userIdentity.Id, environment.Name, environment.Description);
        EnvironmentService.PopulateIdentityReference(requestContext, environment);
        this.FireEnvironmentUpdatedEvent(requestContext, environment);
        return environment;
      }
    }

    public void DeleteEnvironment(
      IVssRequestContext requestContext,
      Guid projectId,
      int environmentId)
    {
      using (new MethodScope(requestContext, nameof (EnvironmentService), nameof (DeleteEnvironment)))
      {
        this.securityProvider.CheckManagePermissions(requestContext, projectId, environmentId);
        requestContext.TraceInfo(10015214, nameof (EnvironmentService), "Deleting environment with Id: {0}.", (object) environmentId);
        using (IDisposableReadOnlyList<IEnvironmentExtension> extensions = requestContext.GetExtensions<IEnvironmentExtension>())
        {
          foreach (IEnvironmentExtension environmentExtension in (IEnumerable<IEnvironmentExtension>) extensions)
            environmentExtension.OnDeleteEnvironment(requestContext, projectId, environmentId);
        }
        using (EnvironmentComponent component = requestContext.CreateComponent<EnvironmentComponent>())
          component.DeleteEnvironment(projectId, environmentId);
        try
        {
          IPipelineResourceAuthorizationProxyService authorizationProxyService = requestContext.GetService<IPipelineResourceAuthorizationProxyService>();
          requestContext.RunSynchronously((Func<Task>) (() => authorizationProxyService.DeletePipelinePermissionsForResource(requestContext.Elevate(), projectId, environmentId.ToString(), "environment")));
        }
        catch (Exception ex)
        {
          string format = "Deleting pipeline permissions for environment with ID: {0} failed with the following error: {1}";
          requestContext.TraceError(10015178, "DistributedTask", format, (object) environmentId.ToString(), (object) ex.Message);
        }
        this.FireEnvironmentDeletedEvent(requestContext, environmentId);
      }
    }

    public IPagedList<EnvironmentObject> GetEnvironmentsWithFilters(
      IVssRequestContext requestContext,
      Guid projectId,
      string environmentName = null,
      string continutationToken = null,
      EnvironmentActionFilter actionFilter = EnvironmentActionFilter.None,
      bool includeLastCompletedRequest = false,
      EnvironmentJobStatus environmentJobStatus = EnvironmentJobStatus.None,
      int maxEnvironmentsCount = 50)
    {
      maxEnvironmentsCount = Math.Min(maxEnvironmentsCount, 1000);
      EnvironmentService.EnvironmentFilter environmentFilter = EnvironmentService.ConvertEnvironmentDeploymentFiltersForSQL(environmentJobStatus);
      using (new MethodScope(requestContext, nameof (EnvironmentService), nameof (GetEnvironmentsWithFilters)))
      {
        string continuationToken;
        List<EnvironmentObject> list = this.GetEnvironmentsWithFiltersInternal(requestContext, projectId, environmentName, continutationToken, actionFilter, includeLastCompletedRequest, maxEnvironmentsCount + 1, (IEnumerable<byte>) environmentFilter.environmentJobStatusFilters, environmentFilter.IncludeInProgress, out continuationToken).ToList<EnvironmentObject>();
        foreach (EnvironmentObject environmentObject in list)
          EnvironmentService.PopulateIdentityReference(requestContext, environmentObject.ToEnvironment());
        return (IPagedList<EnvironmentObject>) new PagedList<EnvironmentObject>((IEnumerable<EnvironmentObject>) list, continuationToken);
      }
    }

    public EnvironmentObject GetEnvironmentResourceWithFilters(
      IVssRequestContext requestContext,
      Guid projectId,
      int environmentId,
      string resourceName = null,
      EnvironmentResourceType environmentResourceTypeFilter = EnvironmentResourceType.Generic | EnvironmentResourceType.VirtualMachine | EnvironmentResourceType.Kubernetes,
      EnvironmentActionFilter actionFilter = EnvironmentActionFilter.None,
      bool includeResourceReferences = false,
      bool includeLastCompletedRequest = false,
      EnvironmentJobStatus environmentJobStatus = EnvironmentJobStatus.None)
    {
      using (new MethodScope(requestContext, nameof (EnvironmentService), nameof (GetEnvironmentResourceWithFilters)))
      {
        this.CheckViewAndAdditionalPermissions(requestContext, projectId, environmentId, actionFilter);
        EnvironmentService.EnvironmentFilter environmentFilter = EnvironmentService.ConvertEnvironmentDeploymentFiltersForSQL(environmentJobStatus);
        IList<byte> environmentResourceType = EnvironmentService.ConvertResourceTypeFiltersForSQL(environmentResourceTypeFilter);
        EnvironmentObject resourceWithFilters;
        using (EnvironmentComponent component = requestContext.CreateComponent<EnvironmentComponent>())
        {
          resourceWithFilters = component.GetEnvironmentResourceWithFilters(environmentId, projectId, resourceName, (IEnumerable<byte>) environmentResourceType, includeResourceReferences, includeLastCompletedRequest, (IEnumerable<byte>) environmentFilter.environmentJobStatusFilters, environmentFilter.IncludeInProgress);
          if (resourceWithFilters == null)
            throw new EnvironmentNotFoundException(TaskResources.EnvironmentNotFound((object) environmentId));
        }
        EnvironmentService.PopulateIdentityReference(requestContext, resourceWithFilters);
        return resourceWithFilters;
      }
    }

    public void CreateTeamProject(IVssRequestContext requestContext, Guid projectId)
    {
      if (!requestContext.IsSystemContext)
        throw new NotSupportedException("EnvironmentService.CreateTeamProject can only be called in system context by ProjectCreate event");
      using (new MethodScope(requestContext, nameof (EnvironmentService), nameof (CreateTeamProject)))
        EnvironmentSecurityProvider.InitializePermissions(requestContext, projectId);
    }

    public void DeleteTeamProject(IVssRequestContext requestContext, Guid projectId)
    {
      if (!requestContext.IsSystemContext)
        throw new NotSupportedException("EnvironmentService.DeleteTeamProject can only be called in system context by ProjectDelete event");
      using (new MethodScope(requestContext, nameof (EnvironmentService), nameof (DeleteTeamProject)))
      {
        using (EnvironmentComponent component = requestContext.CreateComponent<EnvironmentComponent>())
          component.DeleteTeamProject(projectId);
      }
    }

    public EnvironmentInstance ResolveEnvironmentByName(
      IVssRequestContext requestContext,
      Guid projectId,
      string environmentName,
      EnvironmentActionFilter actionFilter = EnvironmentActionFilter.None,
      bool includeResources = false,
      bool includeLinkedResources = false,
      bool createIfMissing = true)
    {
      EnvironmentInstance environment = (EnvironmentInstance) null;
      try
      {
        requestContext.TraceInfo(10015204, nameof (EnvironmentService), "Resolve environment by name '{0}'", (object) environmentName);
        environment = this.GetEnvironmentByName(requestContext, projectId, environmentName, actionFilter, includeResources, includeLinkedResources);
      }
      catch (EnvironmentNotFoundException ex)
      {
        if (createIfMissing)
        {
          requestContext.TraceInfo(10015172, nameof (EnvironmentService), "Could not find environment with name: {0}. Attempting to auto-create.", (object) environmentName);
          if (requestContext.IsSystemContext && requestContext.IsFeatureEnabled("Pipelines.Environments.DisableEnvironmentAutocreationWithSystemContext"))
          {
            requestContext.TraceInfo(10015229, nameof (EnvironmentService), "Environment auto creation with system context is disabled, environment - '{0}' will not be created.", (object) environmentName);
            return environment;
          }
          this.TryCreateEnvironment(requestContext, projectId, environmentName, out environment);
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceError(10015172, nameof (EnvironmentService), "Unable to auto create environment with name '{0}'. Error Message: '{1}'", (object) environmentName, (object) ex.Message);
        requestContext.TraceException(10015172, nameof (EnvironmentService), ex);
      }
      return environment;
    }

    public TaskAgentPoolReference GetEnvironmentPool(
      IVssRequestContext requestContext,
      string environmentName)
    {
      throw new NotImplementedException();
    }

    public TaskAgentPoolReference GetEnvironmentPool(
      IVssRequestContext requestContext,
      Guid projectId,
      int environmentId)
    {
      int? nullable = new int?();
      using (new MethodScope(requestContext, nameof (EnvironmentService), nameof (GetEnvironmentPool)))
      {
        this.CheckViewAndAdditionalPermissions(requestContext, projectId, environmentId);
        using (EnvironmentComponent component = requestContext.CreateComponent<EnvironmentComponent>())
          nullable = component.GetEnvironmentPoolId(environmentId);
        if (!nullable.HasValue)
        {
          requestContext.TraceError(10015189, nameof (EnvironmentService), "pool is not configured for environment {0}.", (object) environmentId);
          throw new EnvironmentPoolNotFoundException(TaskResources.EnvironmentPoolNotFound((object) environmentId));
        }
        TaskAgentPool agentPool = requestContext.GetService<IDistributedTaskResourceService>().GetAgentPool(requestContext, nullable.Value);
        if (agentPool == null || agentPool.Id < 1)
          requestContext.TraceInfo(10015189, nameof (EnvironmentService), "Unable to resolve the  pool with id {0}, linked with environment id {1}", (object) nullable.Value, (object) environmentId);
        return agentPool.AsReference();
      }
    }

    public Task<TaskAgentPoolReference> ProvisionEnvironmentPoolAsync(
      IVssRequestContext requestContext,
      string environmentName)
    {
      throw new NotImplementedException();
    }

    public async Task<TaskAgentPoolReference> ProvisionEnvironmentPoolAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int environmentId)
    {
      TaskAgentPoolReference agentPoolReference;
      using (new MethodScope(requestContext, nameof (EnvironmentService), nameof (ProvisionEnvironmentPoolAsync)))
      {
        this.CheckViewAndAdditionalPermissions(requestContext, projectId, environmentId, EnvironmentActionFilter.Manage);
        IDistributedTaskResourceService service = requestContext.GetService<IDistributedTaskResourceService>();
        TaskAgentPool taskAgentPool1 = new TaskAgentPool();
        taskAgentPool1.Name = "environment-" + environmentId.ToString() + "-" + Guid.NewGuid().ToString();
        taskAgentPool1.PoolType = TaskAgentPoolType.Deployment;
        TaskAgentPool taskAgentPool = taskAgentPool1;
        taskAgentPool = service.AddAgentPool(requestContext.Elevate(), taskAgentPool);
        using (EnvironmentComponent component = requestContext.CreateComponent<EnvironmentComponent>())
          component.RegisterEnvironmentPool(environmentId, taskAgentPool.Id);
        await requestContext.GetService<IPipelineResourceAuthorizationProxyService>().InheritAuthorizationPolicyFromEnvironmentAsync(requestContext, projectId, environmentId, taskAgentPool.Id.ToString(), typeof (TaskAgentPool).ToString());
        agentPoolReference = taskAgentPool.AsReference();
      }
      return agentPoolReference;
    }

    public IList<EnvironmentLinkedResourceReference> GetEnvironmentOwnedResources(
      IVssRequestContext requestContext,
      Guid projectId,
      int environmentId)
    {
      List<EnvironmentLinkedResourceReference> source1 = new List<EnvironmentLinkedResourceReference>();
      EnvironmentInstance environmentById = this.GetEnvironmentById(requestContext, projectId, environmentId, EnvironmentActionFilter.None, true, true);
      string serviceEndpointReferenceTypeFullName = typeof (ServiceEndpointReference).FullName;
      if (environmentById != null)
      {
        source1.AddRange(environmentById.Resources.SelectMany<EnvironmentResourceReference, EnvironmentLinkedResourceReference>((Func<EnvironmentResourceReference, IEnumerable<EnvironmentLinkedResourceReference>>) (r => (IEnumerable<EnvironmentLinkedResourceReference>) r.LinkedResources)));
        IEnumerable<EnvironmentLinkedResourceReference> source2 = source1.Where<EnvironmentLinkedResourceReference>((Func<EnvironmentLinkedResourceReference, bool>) (r => r.TypeName == serviceEndpointReferenceTypeFullName));
        if (source2.Any<EnvironmentLinkedResourceReference>())
        {
          source1.RemoveAll((Predicate<EnvironmentLinkedResourceReference>) (r => r.TypeName == serviceEndpointReferenceTypeFullName));
          List<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint> source3 = requestContext.GetService<IServiceEndpointService2>().QueryServiceEndpoints(requestContext, projectId, (string) null, (IEnumerable<string>) null, source2.Select<EnvironmentLinkedResourceReference, Guid>((Func<EnvironmentLinkedResourceReference, Guid>) (e => new Guid(e.Id))), ServiceEndpointOwner.Environment, false);
          if (source3 != null && source3.Any<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>())
            source1.AddRange(source3.Select<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint, EnvironmentLinkedResourceReference>((Func<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint, EnvironmentLinkedResourceReference>) (e => new EnvironmentLinkedResourceReference()
            {
              Id = e.Id.ToString(),
              TypeName = serviceEndpointReferenceTypeFullName
            })));
        }
      }
      return (IList<EnvironmentLinkedResourceReference>) source1;
    }

    private bool TryCreateEnvironment(
      IVssRequestContext requestContext,
      Guid projectId,
      string environmentName,
      out EnvironmentInstance environment)
    {
      try
      {
        EnvironmentInstance environment1 = new EnvironmentInstance()
        {
          Name = environmentName,
          Description = TaskResources.NewEnvrionmentDescription()
        };
        IList<Microsoft.VisualStudio.Services.Identity.Identity> createdEnvironment = this.GetAdministratorsForAutoCreatedEnvironment(requestContext, projectId);
        environment = this.AddEnvironment(requestContext, projectId, environment1, createdEnvironment, "Yaml");
        return true;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10015172, nameof (EnvironmentService), ex);
        requestContext.TraceError(10015172, nameof (EnvironmentService), "Unable to auto create environmet with name '{0}'. Error Message: '{1}'", (object) environmentName, (object) ex.Message);
        environment = (EnvironmentInstance) null;
        return false;
      }
    }

    private string GetCollectionEnvironmentName(
      IVssRequestContext requestContext,
      Guid projectId,
      EnvironmentInstance environment)
    {
      string projectName = requestContext.GetService<IProjectService>().GetProjectName(requestContext, projectId);
      string environmentName = environment.Name + "-" + projectName;
      try
      {
        ArgumentValidation.CheckEnvironmentName(ref environmentName, "collectionEnvironment", false);
      }
      catch (ArgumentException ex)
      {
        return environment.Name + "-" + projectId.ToString();
      }
      return environmentName;
    }

    private static int ConvertActionFilterToPermissions(EnvironmentActionFilter actionFilter)
    {
      int permissions = 0;
      if ((actionFilter & EnvironmentActionFilter.Manage) == EnvironmentActionFilter.Manage)
        permissions |= 2;
      if ((actionFilter & EnvironmentActionFilter.Use) == EnvironmentActionFilter.Use)
        permissions |= 16;
      return permissions;
    }

    private static void PopulateIdentityReference(
      IVssRequestContext requestContext,
      EnvironmentInstance environment)
    {
      if (environment == null)
        return;
      IdentityService service = requestContext.GetService<IdentityService>();
      if (environment.CreatedBy != null)
        environment.CreatedBy = service.GetIdentity(requestContext, environment.CreatedBy).ToIdentityRef(requestContext);
      if (environment.LastModifiedBy == null)
        return;
      environment.LastModifiedBy = service.GetIdentity(requestContext, environment.LastModifiedBy).ToIdentityRef(requestContext);
    }

    private static void PopulateIdentityReference(
      IVssRequestContext requestContext,
      EnvironmentObject environment)
    {
      if (environment == null)
        return;
      IdentityService service = requestContext.GetService<IdentityService>();
      if (environment.CreatedBy != null)
        environment.CreatedBy = service.GetIdentity(requestContext, environment.CreatedBy).ToIdentityRef(requestContext);
      if (environment.LastModifiedBy == null)
        return;
      environment.LastModifiedBy = service.GetIdentity(requestContext, environment.LastModifiedBy).ToIdentityRef(requestContext);
    }

    private string GeneratePersonalAccessTokenWithEnvironmentScope(
      IVssRequestContext requestContext,
      string tokenName)
    {
      Guid guid = !requestContext.ExecutionEnvironment.IsOnPremisesDeployment ? requestContext.ServiceHost.InstanceId : requestContext.To(TeamFoundationHostType.Deployment).ServiceHost.InstanceId;
      IDelegatedAuthorizationService service = requestContext.GetService<IDelegatedAuthorizationService>();
      IVssRequestContext requestContext1 = requestContext;
      string str = tokenName;
      DateTime? nullable = new DateTime?(DateTime.UtcNow.AddHours(3.0));
      IList<Guid> guidList = (IList<Guid>) new Guid[1]
      {
        guid
      };
      Guid? clientId = new Guid?();
      Guid? userId = new Guid?();
      string name = str;
      DateTime? validTo = nullable;
      IList<Guid> targetAccounts = guidList;
      Guid? authorizationId = new Guid?();
      Guid? accessId = new Guid?();
      SessionTokenResult sessionTokenResult = service.IssueSessionToken(requestContext1, clientId, userId, name, validTo, "vso.environment_manage", targetAccounts, SessionTokenType.Compact, authorizationId: authorizationId, accessId: accessId);
      if (sessionTokenResult != null && sessionTokenResult.HasError)
        throw new SessionTokenException(sessionTokenResult.SessionTokenError);
      return sessionTokenResult?.SessionToken.Token;
    }

    private static IList<EnvironmentInstance> GetEnvironmentsInternal(
      IVssRequestContext requestContext,
      Guid projectId,
      string environmentName,
      string lastEnvironmentName,
      int maxEnvironmentsCount = 50)
    {
      using (EnvironmentComponent component = requestContext.CreateComponent<EnvironmentComponent>())
        return component.GetEnvironments(projectId, environmentName, lastEnvironmentName, maxEnvironmentsCount);
    }

    private IEnumerable<EnvironmentInstance> FilterByViewAndActionFilterPermissions(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<EnvironmentInstance> environments,
      EnvironmentActionFilter actionFilter = EnvironmentActionFilter.None)
    {
      int permissions = 1;
      if (actionFilter != EnvironmentActionFilter.None)
        permissions |= EnvironmentService.ConvertActionFilterToPermissions(actionFilter);
      return environments.Where<EnvironmentInstance>((Func<EnvironmentInstance, bool>) (e => this.securityProvider.HasPermissions(requestContext, EnvironmentSecurityProvider.GetEnvironmentToken(new Guid?(projectId), new int?(e.Id)), permissions)));
    }

    private IEnumerable<EnvironmentObject> FilterByViewAndActionFilterPermissions(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<EnvironmentObject> environments,
      EnvironmentActionFilter actionFilter = EnvironmentActionFilter.None)
    {
      int permissions = 1;
      if (actionFilter != EnvironmentActionFilter.None)
        permissions |= EnvironmentService.ConvertActionFilterToPermissions(actionFilter);
      return environments.Where<EnvironmentObject>((Func<EnvironmentObject, bool>) (e => this.securityProvider.HasPermissions(requestContext, EnvironmentSecurityProvider.GetEnvironmentToken(new Guid?(projectId), new int?(e.Id)), permissions)));
    }

    private void CheckViewAndAdditionalPermissions(
      IVssRequestContext requestContext,
      Guid projectId,
      int environmentId,
      EnvironmentActionFilter actionFilter = EnvironmentActionFilter.None)
    {
      int requiredPermissions = 1;
      if (actionFilter != EnvironmentActionFilter.None)
        requiredPermissions |= EnvironmentService.ConvertActionFilterToPermissions(actionFilter);
      this.securityProvider.CheckPermissions(requestContext, requiredPermissions, new Guid?(projectId), new int?(environmentId));
    }

    private IEnumerable<EnvironmentInstance> GetFilteredEnvironments(
      IVssRequestContext requestContext,
      Guid projectId,
      string environmentName,
      string lastEnvironmentName,
      EnvironmentActionFilter actionFilter,
      int maxEnvironmentsCount,
      out string continuationToken)
    {
      continuationToken = (string) null;
      IList<EnvironmentInstance> environmentsInternal = EnvironmentService.GetEnvironmentsInternal(requestContext, projectId, environmentName, lastEnvironmentName, maxEnvironmentsCount);
      IEnumerable<EnvironmentInstance> filteredEnvironments = this.FilterByViewAndActionFilterPermissions(requestContext, projectId, environmentsInternal.Take<EnvironmentInstance>(maxEnvironmentsCount - 1), actionFilter);
      if (environmentsInternal.Count != maxEnvironmentsCount)
        return filteredEnvironments;
      continuationToken = environmentsInternal[maxEnvironmentsCount - 1].Name;
      return filteredEnvironments;
    }

    private IEnumerable<EnvironmentObject> GetEnvironmentsWithFiltersInternal(
      IVssRequestContext requestContext,
      Guid projectId,
      string environmentName,
      string continutationToken,
      EnvironmentActionFilter actionFilter,
      bool includeLastCompletedRequest,
      int maxEnvironmentsCount,
      IEnumerable<byte> environmentLastJobStatusFilters,
      bool includeInProgressFilter,
      out string continuationToken)
    {
      continuationToken = (string) null;
      IList<EnvironmentObject> environmentsWithFilters;
      using (EnvironmentComponent component = requestContext.CreateComponent<EnvironmentComponent>())
        environmentsWithFilters = component.GetEnvironmentsWithFilters(projectId, environmentName, continutationToken, maxEnvironmentsCount, includeLastCompletedRequest, environmentLastJobStatusFilters, includeInProgressFilter);
      IEnumerable<EnvironmentObject> withFiltersInternal = this.FilterByViewAndActionFilterPermissions(requestContext, projectId, environmentsWithFilters.Take<EnvironmentObject>(maxEnvironmentsCount - 1), actionFilter);
      if (environmentsWithFilters.Count != maxEnvironmentsCount)
        return withFiltersInternal;
      continuationToken = environmentsWithFilters[maxEnvironmentsCount - 1].Name;
      return withFiltersInternal;
    }

    private static EnvironmentService.EnvironmentFilter ConvertEnvironmentDeploymentFiltersForSQL(
      EnvironmentJobStatus environmentJobStatus = EnvironmentJobStatus.None)
    {
      EnvironmentService.EnvironmentFilter environmentFilter = new EnvironmentService.EnvironmentFilter();
      IList<byte> byteList = (IList<byte>) new List<byte>();
      if ((environmentJobStatus & EnvironmentJobStatus.Succeeded) == EnvironmentJobStatus.Succeeded)
        byteList.Add((byte) 0);
      if ((environmentJobStatus & EnvironmentJobStatus.SucceededWithIssues) == EnvironmentJobStatus.SucceededWithIssues)
        byteList.Add((byte) 1);
      if ((environmentJobStatus & EnvironmentJobStatus.Abandoned) == EnvironmentJobStatus.Abandoned)
        byteList.Add((byte) 5);
      if ((environmentJobStatus & EnvironmentJobStatus.Canceled) == EnvironmentJobStatus.Canceled)
        byteList.Add((byte) 3);
      if ((environmentJobStatus & EnvironmentJobStatus.Failed) == EnvironmentJobStatus.Failed)
        byteList.Add((byte) 2);
      if ((environmentJobStatus & EnvironmentJobStatus.Skipped) == EnvironmentJobStatus.Skipped)
        byteList.Add((byte) 4);
      environmentFilter.IncludeInProgress = (environmentJobStatus & EnvironmentJobStatus.InProgress) == EnvironmentJobStatus.InProgress;
      environmentFilter.environmentJobStatusFilters = byteList.Count != 0 ? byteList : (IList<byte>) null;
      return environmentFilter;
    }

    private static IList<byte> ConvertResourceTypeFiltersForSQL(
      EnvironmentResourceType environmentResourceType)
    {
      IList<byte> byteList = (IList<byte>) new List<byte>();
      if ((environmentResourceType & EnvironmentResourceType.Generic) == EnvironmentResourceType.Generic)
        byteList.Add((byte) 1);
      if ((environmentResourceType & EnvironmentResourceType.Undefined) == EnvironmentResourceType.Undefined)
        byteList.Add((byte) 0);
      if ((environmentResourceType & EnvironmentResourceType.Kubernetes) == EnvironmentResourceType.Kubernetes)
        byteList.Add((byte) 4);
      if ((environmentResourceType & EnvironmentResourceType.VirtualMachine) == EnvironmentResourceType.VirtualMachine)
        byteList.Add((byte) 2);
      if (byteList.Count == 0)
        byteList = (IList<byte>) null;
      return byteList;
    }

    private IList<Microsoft.VisualStudio.Services.Identity.Identity> GetAdministratorsForAutoCreatedEnvironment(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      if (!requestContext.IsSystemContext)
        return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new List<Microsoft.VisualStudio.Services.Identity.Identity>();
      IList<Microsoft.VisualStudio.Services.Identity.Identity> source = requestContext.GetService<IdentityService>().ListGroups(requestContext, new Guid[1]
      {
        projectId
      }, false, (IEnumerable<string>) null);
      Microsoft.VisualStudio.Services.Identity.Identity identity1 = source.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (group => group.GetProperty<string>("Account", string.Empty).Equals(TaskResources.ProjectContributorsGroupName(), StringComparison.OrdinalIgnoreCase))).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (identity1 == null)
        requestContext.TraceInfo(10015172, nameof (EnvironmentService), "Could not find project contributors identity group");
      Microsoft.VisualStudio.Services.Identity.Identity identity2 = source.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (group => group.GetProperty<string>("Account", string.Empty).Equals(TaskResources.ProjectAdministratorsGroupName(), StringComparison.OrdinalIgnoreCase))).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (identity2 == null)
        requestContext.TraceInfo(10015172, nameof (EnvironmentService), "Could not find project administrators identity group");
      return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new List<Microsoft.VisualStudio.Services.Identity.Identity>()
      {
        identity1,
        identity2
      };
    }

    private string GetCollectionEnvironmentNameWithRandomString(string collectionEnvironmentName)
    {
      string str = new Random().Next().ToString().Substring(0, 4);
      string withRandomString = TaskResources.EnvironmentNameFormat((object) collectionEnvironmentName, (object) str);
      try
      {
        ArgumentValidation.CheckEnvironmentName(ref collectionEnvironmentName, "collectionEnvironment", false);
      }
      catch (ArgumentException ex)
      {
        withRandomString = Guid.NewGuid().ToString();
      }
      return withRandomString;
    }

    private static void GrantAdministratorPrivileges(
      IVssRequestContext requestContext,
      Guid projectId,
      int environmentId,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> administrators)
    {
      List<IAccessControlEntry> aces = new List<IAccessControlEntry>();
      administrators = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) administrators.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (ad => ad != null)).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
      foreach (Microsoft.VisualStudio.Services.Identity.Identity administrator in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) administrators)
        aces.Add((IAccessControlEntry) new AccessControlEntry(administrator.Descriptor, 27, 0));
      string environmentToken1 = EnvironmentSecurityProvider.GetEnvironmentToken(new Guid?(projectId), new int?(environmentId));
      string environmentToken2 = EnvironmentSecurityProvider.GetEnvironmentToken(environmentId: new int?(environmentId));
      List<AccessControlList> accessControlListList = new List<AccessControlList>()
      {
        new AccessControlList(environmentToken1, true, (IEnumerable<IAccessControlEntry>) aces),
        new AccessControlList(environmentToken2, true, (IEnumerable<IAccessControlEntry>) aces)
      };
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, EnvironmentSecurityProvider.EnvironmentNamespaceId);
      requestContext.TraceInfo(10015182, nameof (EnvironmentService), "Attempting to grant administrator privileges to {0} identities", (object) administrators.Count);
      securityNamespace.SetAccessControlLists(requestContext, (IEnumerable<IAccessControlList>) accessControlListList);
    }

    private void FireEnvironmentAddedEvent(
      IVssRequestContext requestContext,
      EnvironmentInstance environment)
    {
      requestContext.GetService<ITeamFoundationEventService>().PublishNotification(requestContext, (object) new EnvironmentAddedEvent()
      {
        EnvironmentId = environment.Id,
        Name = environment.Name
      });
    }

    private void FireEnvironmentUpdatedEvent(
      IVssRequestContext requestContext,
      EnvironmentInstance environment)
    {
      requestContext.GetService<ITeamFoundationEventService>().PublishNotification(requestContext, (object) new EnvironmentUpdatedEvent()
      {
        EnvironmentId = environment.Id,
        Name = environment.Name
      });
    }

    private void FireEnvironmentDeletedEvent(IVssRequestContext requestContext, int environmentId) => requestContext.GetService<ITeamFoundationEventService>().PublishNotification(requestContext, (object) new EnvironmentDeletedEvent()
    {
      EnvironmentId = environmentId
    });

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext)
    {
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext)
    {
    }

    private struct EnvironmentFilter
    {
      public IList<byte> environmentJobStatusFilters { get; set; }

      public bool IncludeInProgress { get; set; }
    }
  }
}
