// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServiceIdentityProvisioner
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities
{
  public static class ServiceIdentityProvisioner
  {
    public static Microsoft.VisualStudio.Services.Identity.Identity GetServiceIdentity(
      IVssRequestContext requestContext,
      ReleaseAuthorizationScope scope,
      Guid projectId)
    {
      return ServiceIdentityProvisioner.GetServiceIdentity(requestContext, scope, projectId, false, (TaskOrchestrationPlan) null);
    }

    public static Microsoft.VisualStudio.Services.Identity.Identity GetServiceIdentity(
      IVssRequestContext requestContext,
      ReleaseAuthorizationScope scope,
      Guid projectId,
      bool throwExceptionIfNotFound)
    {
      return ServiceIdentityProvisioner.GetServiceIdentity(requestContext, scope, projectId, throwExceptionIfNotFound, (TaskOrchestrationPlan) null);
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Required to catch all exception")]
    public static Microsoft.VisualStudio.Services.Identity.Identity GetServiceIdentity(
      IVssRequestContext requestContext,
      ReleaseAuthorizationScope scope,
      Guid projectId,
      bool throwExceptionIfNotFound,
      TaskOrchestrationPlan plan)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (scope == ReleaseAuthorizationScope.Project && projectId == Guid.Empty)
        throw new ArgumentOutOfRangeException(nameof (projectId));
      string message = Resources.ServiceIdentityNotFound;
      if (scope == ReleaseAuthorizationScope.ProjectCollection && projectId != Guid.Empty)
      {
        bool jobScopeSetting = false;
        if (requestContext.GetService<IProjectService>().GetProject(requestContext.Elevate(), projectId).Visibility == ProjectVisibility.Public)
        {
          jobScopeSetting = true;
          requestContext.TraceAlways(1976487, TraceLevel.Info, "ReleaseManagementService", "DistributedTask", "Since this project {0} is public, project scoped service identity is used", (object) projectId);
        }
        else if (requestContext.IsFeatureEnabled("AzureDevOps.ReleaseManagement.HonorJobScopeSettings"))
        {
          bool flag1 = false;
          bool flag2 = requestContext.IsFeatureEnabled("AzureDevOps.ReleaseManagement.CacheJobServiceIdentity");
          if (flag2 && plan != null && plan.ProcessType == OrchestrationProcessType.Container)
            flag1 = requestContext.GetService<IJobScopeCacheService>().TryGetJobScope(requestContext, plan, out jobScopeSetting);
          if (!flag1)
          {
            try
            {
              PipelineGeneralSettings result = requestContext.Elevate().GetClient<BuildHttpClient>().GetBuildGeneralSettingsAsync(projectId).Result;
              bool? scopeForReleases = result.EnforceJobAuthScopeForReleases;
              if (scopeForReleases.HasValue)
              {
                scopeForReleases = result.EnforceJobAuthScopeForReleases;
                if (scopeForReleases.Value)
                {
                  jobScopeSetting = true;
                  requestContext.TraceAlways(1976487, TraceLevel.Info, "ReleaseManagementService", "DistributedTask", "Job authorization scope set at org or project level. Overriding scope to current project.");
                }
                if (flag2)
                {
                  if (plan != null)
                  {
                    if (plan.ProcessType == OrchestrationProcessType.Container)
                    {
                      IJobScopeCacheService service = requestContext.GetService<IJobScopeCacheService>();
                      IVssRequestContext requestContext1 = requestContext;
                      TaskOrchestrationPlan plan1 = plan;
                      scopeForReleases = result.EnforceJobAuthScopeForReleases;
                      int num = scopeForReleases.Value ? 1 : 0;
                      service.Add(requestContext1, plan1, num != 0);
                    }
                  }
                }
              }
            }
            catch (Exception ex)
            {
              jobScopeSetting = true;
              requestContext.Trace(1976495, TraceLevel.Error, "ReleaseManagementService", "DistributedTask", Resources.CannotGetPipelineGeneralSettings, (object) projectId, (object) ex.Message);
            }
          }
        }
        if (jobScopeSetting)
        {
          scope = ReleaseAuthorizationScope.Project;
          message = Resources.ProjectScopedServiceIdentityNotFoundInPublicProject;
        }
      }
      Guid guid = scope == ReleaseAuthorizationScope.Project ? projectId : requestContext.ServiceHost.InstanceId;
      Microsoft.VisualStudio.Services.Identity.Identity frameworkIdentity = IdentityHelper.GetFrameworkIdentity(requestContext.Elevate(), FrameworkIdentityType.ServiceIdentity, "Build", guid.ToString("D"));
      if (frameworkIdentity != null)
      {
        ServiceIdentityProvisioner.TraceInformationMessage(requestContext, 1961041, "Found service identity with ID {0}", (object) frameworkIdentity.Id);
        return frameworkIdentity;
      }
      if (throwExceptionIfNotFound)
        throw new ReleaseManagementServiceException(message);
      return (Microsoft.VisualStudio.Services.Identity.Identity) null;
    }

    private static void TraceInformationMessage(
      IVssRequestContext requestContext,
      int tracePoint,
      string format,
      params object[] args)
    {
      VssRequestContextExtensions.Trace(requestContext, tracePoint, TraceLevel.Info, "ReleaseManagementService", "Service", format, args);
    }
  }
}
