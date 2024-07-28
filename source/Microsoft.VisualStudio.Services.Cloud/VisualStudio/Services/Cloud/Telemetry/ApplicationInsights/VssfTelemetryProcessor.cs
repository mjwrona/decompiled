// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.Telemetry.ApplicationInsights.VssfTelemetryProcessor
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using System;
using System.Web;

namespace Microsoft.VisualStudio.Services.Cloud.Telemetry.ApplicationInsights
{
  public class VssfTelemetryProcessor : ITelemetryProcessor
  {
    private string m_tenant;
    private string m_roleName;
    private const string c_fullTargetName = "Full Target Name";

    private ITelemetryProcessor Next { get; set; }

    public VssfTelemetryProcessor(ITelemetryProcessor next) => this.Next = next;

    public void Process(ITelemetry telemetry)
    {
      try
      {
        if (AzureRoleUtil.IsAvailable)
        {
          ISupportProperties isupportProperties = (ISupportProperties) telemetry;
          telemetry.Context.Cloud.RoleName = this.RoleName;
          if (!isupportProperties.Properties.ContainsKey("DeploymentId"))
            isupportProperties.Properties.Add("DeploymentId", AzureRoleUtil.Environment.DeploymentId);
          HttpContext current = HttpContext.Current;
          if (current != null)
          {
            if (!isupportProperties.Properties.ContainsKey("ActivityId"))
              isupportProperties.Properties.Add("ActivityId", current.Response.Headers["ActivityId"]);
            if (!isupportProperties.Properties.ContainsKey("SessionId"))
              isupportProperties.Properties.Add("SessionId", current.Response.Headers["X-TFS-Session"]);
            if (!isupportProperties.Properties.ContainsKey("E2EID"))
              isupportProperties.Properties.Add("E2EID", current.Response.Headers["X-VSS-E2EID"]);
            if (!isupportProperties.Properties.ContainsKey("Command") && current.Items.Contains((object) "VssfTelemetryProcessor.Command"))
              isupportProperties.Properties.Add("Command", current.Items[(object) "VssfTelemetryProcessor.Command"].ToString());
          }
        }
        switch (telemetry)
        {
          case RequestTelemetry _:
            RequestTelemetry requestTelemetry = telemetry as RequestTelemetry;
            bool? success1 = ((OperationTelemetry) requestTelemetry).Success;
            bool flag1 = false;
            if (success1.GetValueOrDefault() == flag1 & success1.HasValue)
            {
              if (requestTelemetry.ResponseCode.StartsWith("4", StringComparison.Ordinal))
                ((OperationTelemetry) requestTelemetry).Success = new bool?(true);
              if (requestTelemetry.ResponseCode.StartsWith("5", StringComparison.Ordinal))
              {
                if (requestTelemetry.Url.AbsolutePath.EndsWith("asmx"))
                {
                  ((OperationTelemetry) requestTelemetry).Success = new bool?(true);
                  break;
                }
                break;
              }
              break;
            }
            break;
          case DependencyTelemetry _:
            DependencyTelemetry dependencyTelemetry = telemetry as DependencyTelemetry;
            if (dependencyTelemetry.Type.IndexOf("sql", StringComparison.OrdinalIgnoreCase) > -1)
            {
              bool? success2 = ((OperationTelemetry) dependencyTelemetry).Success;
              bool flag2 = false;
              if (success2.GetValueOrDefault() == flag2 & success2.HasValue && string.Equals(dependencyTelemetry.ResultCode, "50000", StringComparison.Ordinal))
                ((OperationTelemetry) dependencyTelemetry).Success = new bool?(true);
            }
            if (dependencyTelemetry.Type.IndexOf("http", StringComparison.OrdinalIgnoreCase) > -1)
            {
              bool? success3 = ((OperationTelemetry) dependencyTelemetry).Success;
              bool flag3 = false;
              if (success3.GetValueOrDefault() == flag3 & success3.HasValue && dependencyTelemetry.ResultCode.StartsWith("4", StringComparison.Ordinal) && !string.Equals(dependencyTelemetry.ResultCode, "429", StringComparison.Ordinal))
                ((OperationTelemetry) dependencyTelemetry).Success = new bool?(true);
            }
            if (dependencyTelemetry.Type.IndexOf("azure queue", StringComparison.OrdinalIgnoreCase) > -1)
              this.CheckAndApplyAdjustedStorageTarget(dependencyTelemetry);
            if (dependencyTelemetry.Type.IndexOf("azure blob", StringComparison.OrdinalIgnoreCase) > -1)
            {
              this.CheckAndApplyAdjustedStorageTarget(dependencyTelemetry);
              bool? success4 = ((OperationTelemetry) dependencyTelemetry).Success;
              bool flag4 = false;
              if (success4.GetValueOrDefault() == flag4 & success4.HasValue && string.Equals(dependencyTelemetry.ResultCode, "404", StringComparison.Ordinal))
                ((OperationTelemetry) dependencyTelemetry).Success = new bool?(true);
              bool? success5 = ((OperationTelemetry) dependencyTelemetry).Success;
              bool flag5 = false;
              if (success5.GetValueOrDefault() == flag5 & success5.HasValue && dependencyTelemetry.Target.EndsWith("-secondary.blob.core.windows.net", StringComparison.OrdinalIgnoreCase) && AzureRoleUtil.Environment.CurrentRoleName.Equals("JobAgent", StringComparison.OrdinalIgnoreCase))
                ((OperationTelemetry) dependencyTelemetry).Success = new bool?(true);
            }
            if (dependencyTelemetry.Type.IndexOf("azure table", StringComparison.OrdinalIgnoreCase) > -1)
            {
              this.CheckAndApplyAdjustedStorageTarget(dependencyTelemetry);
              break;
            }
            break;
          case ExceptionTelemetry _:
            ExceptionTelemetry exceptionTelemetry = telemetry as ExceptionTelemetry;
            string typeName = exceptionTelemetry.ExceptionDetailsInfoList[0].TypeName;
            string message = exceptionTelemetry.Exception.Message;
            if (typeName.IndexOf("System.ArgumentException", StringComparison.OrdinalIgnoreCase) > -1 && message.IndexOf("Ambiguous values for version", StringComparison.OrdinalIgnoreCase) > -1 || typeName.IndexOf("System.Web.HttpException", StringComparison.OrdinalIgnoreCase) > -1 && message.IndexOf("Page not found.", StringComparison.OrdinalIgnoreCase) > -1 || typeName.IndexOf("Microsoft.Azure.Devops.Teams.Service.TeamNotFoundException", StringComparison.OrdinalIgnoreCase) > -1 || typeName.IndexOf("Microsoft.TeamFoundation.Agile.Server.Exceptions.CurrentIterationDoesNotExistException", StringComparison.OrdinalIgnoreCase) > -1 || typeName.IndexOf("Microsoft.TeamFoundation.Build.WebApi.AmbiguousDefinitionNameException", StringComparison.OrdinalIgnoreCase) > -1 || typeName.IndexOf("Microsoft.TeamFoundation.Build.WebApi.ArtifactNotFoundException", StringComparison.OrdinalIgnoreCase) > -1 || typeName.IndexOf("Microsoft.TeamFoundation.Build.WebApi.BuildNotFoundException", StringComparison.OrdinalIgnoreCase) > -1 || typeName.IndexOf("Microsoft.TeamFoundation.Build.WebApi.DefinitionNotFoundException", StringComparison.OrdinalIgnoreCase) > -1 || typeName.IndexOf("Microsoft.TeamFoundation.Core.WebApi.ProjectDoesNotExistException", StringComparison.OrdinalIgnoreCase) > -1 || typeName.IndexOf("Microsoft.TeamFoundation.Core.WebApi.ProjectDoesNotExistWithNameException", StringComparison.OrdinalIgnoreCase) > -1 || typeName.IndexOf("Microsoft.TeamFoundation.DistributedTask.WebApi.TaskAgentAccessTokenExpiredException", StringComparison.OrdinalIgnoreCase) > -1 || typeName.IndexOf("Microsoft.TeamFoundation.DistributedTask.WebApi.TaskAgentNotFoundException", StringComparison.OrdinalIgnoreCase) > -1 || typeName.IndexOf("Microsoft.TeamFoundation.DistributedTask.WebApi.TaskAgentPoolNotFoundException", StringComparison.OrdinalIgnoreCase) > -1 || typeName.IndexOf("Microsoft.TeamFoundation.DistributedTask.WebApi.TaskAgentSessionConflictException", StringComparison.OrdinalIgnoreCase) > -1 || typeName.IndexOf("Microsoft.TeamFoundation.DistributedTask.WebApi.TaskAgentSessionDeletedException", StringComparison.OrdinalIgnoreCase) > -1 || typeName.IndexOf("Microsoft.TeamFoundation.DistributedTask.WebApi.TaskAgentVersionNotSupportedException", StringComparison.OrdinalIgnoreCase) > -1 || typeName.IndexOf("Microsoft.TeamFoundation.Framework.Server.UnauthorizedRequestException", StringComparison.OrdinalIgnoreCase) > -1 || typeName.IndexOf("Microsoft.TeamFoundation.Git.Server.GitItemNotFoundException", StringComparison.OrdinalIgnoreCase) > -1 || typeName.IndexOf("Microsoft.TeamFoundation.Git.Server.GitNeedsPermissionException", StringComparison.OrdinalIgnoreCase) > -1 || typeName.IndexOf("Microsoft.TeamFoundation.Git.Server.GitRepositoryNotFoundException", StringComparison.OrdinalIgnoreCase) > -1 || typeName.IndexOf("Microsoft.TeamFoundation.VersionControl.Server.ItemNotFoundException", StringComparison.OrdinalIgnoreCase) > -1 || typeName.IndexOf("Microsoft.TeamFoundation.Wiki.Server.WikiPageNotFoundException", StringComparison.OrdinalIgnoreCase) > -1 || typeName.IndexOf("Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.SyntaxException", StringComparison.OrdinalIgnoreCase) > -1 || typeName.IndexOf("Microsoft.TeamFoundation.WorkItemTracking.Server.LegacyQueryItemException", StringComparison.OrdinalIgnoreCase) > -1 || typeName.IndexOf("Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItemNotFoundException", StringComparison.OrdinalIgnoreCase) > -1 || typeName.IndexOf("Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemFieldInvalidException", StringComparison.OrdinalIgnoreCase) > -1 || typeName.IndexOf("Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemFieldInvalidTreeNameException", StringComparison.OrdinalIgnoreCase) > -1 || typeName.IndexOf("Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemPageSizeExceededException", StringComparison.OrdinalIgnoreCase) > -1 || typeName.IndexOf("Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTrackingQueryException", StringComparison.OrdinalIgnoreCase) > -1 || typeName.IndexOf("Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemUnauthorizedAccessException", StringComparison.OrdinalIgnoreCase) > -1 || typeName.IndexOf("Microsoft.VisualStudio.Services.Common.VssPropertyValidationException", StringComparison.OrdinalIgnoreCase) > -1 || typeName.IndexOf("Microsoft.VisualStudio.Services.DelegatedAuthorization.SessionTokenCreateException", StringComparison.OrdinalIgnoreCase) > -1 && message.IndexOf("AccessDenied", StringComparison.OrdinalIgnoreCase) > -1 || typeName.IndexOf("Microsoft.VisualStudio.Services.Security.AccessCheckException", StringComparison.OrdinalIgnoreCase) > -1 && message.IndexOf("AccessDenied", StringComparison.OrdinalIgnoreCase) > -1 || typeName.IndexOf("Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.DuplicateServiceConnectionException", StringComparison.OrdinalIgnoreCase) > -1 || typeName.IndexOf("Microsoft.VisualStudio.Services.Aad.AadCredentialsNotFoundException", StringComparison.OrdinalIgnoreCase) > -1 || typeName.IndexOf("Microsoft.VisualStudio.Services.Users.UserAttributeDoesNotExistException", StringComparison.OrdinalIgnoreCase) > -1 || typeName.IndexOf("Microsoft.VisualStudio.Services.Identity.IdentityNotFoundException", StringComparison.OrdinalIgnoreCase) > -1 || typeName.IndexOf("Microsoft.ServiceProfiler.Exceptions.CollectorServiceException", StringComparison.OrdinalIgnoreCase) > -1 || typeName.IndexOf("Microsoft.VisualStudio.Services.DelegatedAuthorization.RegistrationNotFoundException", StringComparison.OrdinalIgnoreCase) > -1 || typeName.IndexOf("Microsoft.VisualStudio.Services.DelegatedAuthorization.InvalidPublicKeyException", StringComparison.OrdinalIgnoreCase) > -1 || typeName.IndexOf("Microsoft.VisualStudio.Services.DelegatedAuthorization.InvalidPersonalAccessTokenException", StringComparison.OrdinalIgnoreCase) > -1 || typeName.IndexOf("Microsoft.VisualStudio.Services.MachineManagement.WebApi.MachineInstanceNotFoundException", StringComparison.OrdinalIgnoreCase) > -1 || typeName.IndexOf("Microsoft.VisualStudio.Services.NuGet.WebApi.Exceptions.PackageNotFoundException", StringComparison.OrdinalIgnoreCase) > -1 || typeName.IndexOf("Microsoft.TeamFoundation.Framework.Server.RequestBlockedException", StringComparison.OrdinalIgnoreCase) > -1 || typeName.IndexOf("Microsoft.VisualStudio.Services.Npm.WebApi.Exceptions.PackageNotFoundException", StringComparison.OrdinalIgnoreCase) > -1 || typeName.IndexOf("Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions.PackageNotFoundException", StringComparison.OrdinalIgnoreCase) > -1 || typeName.IndexOf("Microsoft.VisualStudio.Services.Feed.WebApi.FeedNeedsPermissionsException", StringComparison.OrdinalIgnoreCase) > -1 || typeName.IndexOf("Microsoft.VisualStudio.Services.Feed.WebApi.FeedIdNotFoundException", StringComparison.OrdinalIgnoreCase) > -1)
              return;
            if (typeName.IndexOf("Microsoft.TeamFoundation.Framework.Server.UnauthorizedRequestException", StringComparison.OrdinalIgnoreCase) > -1)
              return;
            break;
        }
      }
      catch (Exception ex)
      {
      }
      this.Next.Process(telemetry);
    }

    private void CheckAndApplyAdjustedStorageTarget(DependencyTelemetry dependencyTelemetry)
    {
      if (!((OperationTelemetry) dependencyTelemetry).Properties.ContainsKey("Full Target Name"))
        ((OperationTelemetry) dependencyTelemetry).Properties.Add("Full Target Name", dependencyTelemetry.Target);
      string str1;
      if (AppInsightsCache.storageDependencyCache.TryGetValue(dependencyTelemetry.Target, out str1))
      {
        dependencyTelemetry.Target = str1;
      }
      else
      {
        if (string.IsNullOrEmpty(this.Tenant) || dependencyTelemetry.Target.IndexOf(this.Tenant, StringComparison.OrdinalIgnoreCase) == 0 || dependencyTelemetry.Target.IndexOf(this.Tenant, StringComparison.OrdinalIgnoreCase) <= -1)
          return;
        string str2 = dependencyTelemetry.Target.Substring(dependencyTelemetry.Target.IndexOf(this.Tenant, StringComparison.OrdinalIgnoreCase));
        AppInsightsCache.storageDependencyCache.TryAdd(dependencyTelemetry.Target, str2);
        dependencyTelemetry.Target = str2;
      }
    }

    private string Tenant
    {
      get
      {
        if (string.IsNullOrEmpty(this.m_tenant))
          AzureRoleUtil.Configuration.Settings.TryGetValue("HostedServiceName", out this.m_tenant);
        return this.m_tenant;
      }
    }

    public string RoleName
    {
      get
      {
        if (string.IsNullOrEmpty(this.m_roleName))
          this.m_roleName = AzureRoleUtil.Configuration.GetStringSetting("InstanceTypeName", (string) null).ToLower() + "-" + this.Tenant + "-" + AzureRoleUtil.Environment.CurrentRoleName;
        return this.m_roleName;
      }
    }
  }
}
