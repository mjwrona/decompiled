// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.ServiceEndpointConverter
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  public static class ServiceEndpointConverter
  {
    public static Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint ToLegacyServiceEndpoint(
      this Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint serviceEndpoint)
    {
      if (serviceEndpoint == null)
        return (Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint) null;
      Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint legacyServiceEndpoint = new Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint();
      legacyServiceEndpoint.AdministratorsGroup = serviceEndpoint.AdministratorsGroup;
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.EndpointAuthorization authorization = serviceEndpoint.Authorization;
      legacyServiceEndpoint.Authorization = authorization != null ? authorization.ToLegacyEndpointAuthorization() : (Microsoft.TeamFoundation.DistributedTask.WebApi.EndpointAuthorization) null;
      legacyServiceEndpoint.CreatedBy = serviceEndpoint.CreatedBy;
      legacyServiceEndpoint.Data = serviceEndpoint.Data;
      legacyServiceEndpoint.Description = serviceEndpoint.Description;
      legacyServiceEndpoint.GroupScopeId = serviceEndpoint.GroupScopeId;
      legacyServiceEndpoint.Id = serviceEndpoint.Id;
      legacyServiceEndpoint.IsReady = serviceEndpoint.IsReady;
      legacyServiceEndpoint.IsDisabled = serviceEndpoint.IsDisabled;
      legacyServiceEndpoint.Name = serviceEndpoint.Name;
      legacyServiceEndpoint.OperationStatus = serviceEndpoint.OperationStatus;
      legacyServiceEndpoint.ReadersGroup = serviceEndpoint.ReadersGroup;
      legacyServiceEndpoint.Type = serviceEndpoint.Type;
      legacyServiceEndpoint.Url = serviceEndpoint.Url;
      legacyServiceEndpoint.Owner = serviceEndpoint.Owner;
      return legacyServiceEndpoint;
    }

    public static Microsoft.TeamFoundation.DistributedTask.WebApi.AzureManagementGroupQueryResult ToLegacyAzureManagementGroupQueryResult(
      this Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AzureManagementGroupQueryResult azureManagementGroupQueryResult)
    {
      Microsoft.TeamFoundation.DistributedTask.WebApi.AzureManagementGroupQueryResult groupQueryResult = new Microsoft.TeamFoundation.DistributedTask.WebApi.AzureManagementGroupQueryResult()
      {
        ErrorMessage = azureManagementGroupQueryResult.ErrorMessage,
        Value = new List<Microsoft.TeamFoundation.DistributedTask.WebApi.AzureManagementGroup>()
      };
      if (azureManagementGroupQueryResult != null && !azureManagementGroupQueryResult.Value.IsNullOrEmpty<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AzureManagementGroup>())
      {
        foreach (Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AzureManagementGroup amg in azureManagementGroupQueryResult.Value)
          groupQueryResult.Value.Add(amg.ToLegacyAzureManagementGroup());
      }
      return groupQueryResult;
    }

    public static Microsoft.TeamFoundation.DistributedTask.WebApi.AzureSubscriptionQueryResult ToLegacyAzureSubscriptionQueryResult(
      this Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AzureSubscriptionQueryResult azureSubscriptionQueryResult)
    {
      if (azureSubscriptionQueryResult == null)
        return (Microsoft.TeamFoundation.DistributedTask.WebApi.AzureSubscriptionQueryResult) null;
      Microsoft.TeamFoundation.DistributedTask.WebApi.AzureSubscriptionQueryResult subscriptionQueryResult = new Microsoft.TeamFoundation.DistributedTask.WebApi.AzureSubscriptionQueryResult()
      {
        ErrorMessage = azureSubscriptionQueryResult.ErrorMessage,
        Value = new List<Microsoft.TeamFoundation.DistributedTask.WebApi.AzureSubscription>()
      };
      if (azureSubscriptionQueryResult != null && !azureSubscriptionQueryResult.Value.IsNullOrEmpty<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AzureSubscription>())
      {
        foreach (Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AzureSubscription azureSubscription in azureSubscriptionQueryResult.Value)
          subscriptionQueryResult.Value.Add(azureSubscription.ToLegacyAzureSubscription());
      }
      return subscriptionQueryResult;
    }

    public static Microsoft.TeamFoundation.DistributedTask.WebApi.AzureManagementGroup ToLegacyAzureManagementGroup(
      this Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AzureManagementGroup amg)
    {
      return new Microsoft.TeamFoundation.DistributedTask.WebApi.AzureManagementGroup()
      {
        DisplayName = amg.DisplayName,
        Id = amg.Id,
        Name = amg.Name,
        TenantId = amg.TenantId
      };
    }

    public static Microsoft.TeamFoundation.DistributedTask.WebApi.AadOauthTokenResult ToLegacyAadOauthTokenResult(
      this Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AadOauthTokenResult oauthTokenResult)
    {
      return new Microsoft.TeamFoundation.DistributedTask.WebApi.AadOauthTokenResult()
      {
        AccessToken = oauthTokenResult.AccessToken,
        RefreshTokenCache = oauthTokenResult.RefreshTokenCache
      };
    }

    public static Microsoft.TeamFoundation.DistributedTask.WebApi.AzureSubscription ToLegacyAzureSubscription(
      this Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AzureSubscription azureSubscription)
    {
      return new Microsoft.TeamFoundation.DistributedTask.WebApi.AzureSubscription()
      {
        DisplayName = azureSubscription.DisplayName,
        SubscriptionId = azureSubscription.SubscriptionId,
        SubscriptionTenantId = azureSubscription.SubscriptionTenantId,
        SubscriptionTenantName = azureSubscription.SubscriptionTenantName
      };
    }

    public static Microsoft.TeamFoundation.DistributedTask.WebApi.EndpointAuthorization ToLegacyEndpointAuthorization(
      this Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.EndpointAuthorization endpointAuthorization)
    {
      Microsoft.TeamFoundation.DistributedTask.WebApi.EndpointAuthorization endpointAuthorization1 = new Microsoft.TeamFoundation.DistributedTask.WebApi.EndpointAuthorization()
      {
        Scheme = endpointAuthorization.Scheme
      };
      foreach (KeyValuePair<string, string> parameter in (IEnumerable<KeyValuePair<string, string>>) endpointAuthorization.Parameters)
        endpointAuthorization1.Parameters.Add(parameter.Key, parameter.Value);
      return endpointAuthorization1;
    }

    public static Microsoft.TeamFoundation.DistributedTask.WebApi.AuthorizationHeader ToLegacyAuthorizationHeader(
      this Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AuthorizationHeader AuthorizationHeader)
    {
      return new Microsoft.TeamFoundation.DistributedTask.WebApi.AuthorizationHeader()
      {
        Name = AuthorizationHeader.Name,
        Value = AuthorizationHeader.Value
      };
    }

    public static Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpointRequestResult ToLegacyServiceEndpointRequestResult(
      this Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointRequestResult serviceEndpointRequestResult)
    {
      return new Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpointRequestResult()
      {
        ErrorMessage = serviceEndpointRequestResult.ErrorMessage,
        Result = serviceEndpointRequestResult.Result,
        StatusCode = serviceEndpointRequestResult.StatusCode
      };
    }

    public static Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpointType ToLegacyServiceEndpointType(
      this Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointType serviceEndpointType)
    {
      Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpointType serviceEndpointType1 = new Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpointType();
      serviceEndpointType1.Description = serviceEndpointType.Description;
      serviceEndpointType1.DisplayName = serviceEndpointType.DisplayName;
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.EndpointUrl endpointUrl = serviceEndpointType.EndpointUrl;
      serviceEndpointType1.EndpointUrl = endpointUrl != null ? endpointUrl.ToLegacyEndpointUrl() : (Microsoft.TeamFoundation.DistributedTask.WebApi.EndpointUrl) null;
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.HelpLink helpLink = serviceEndpointType.HelpLink;
      serviceEndpointType1.HelpLink = helpLink != null ? helpLink.ToLegacyHelpLink() : (Microsoft.TeamFoundation.DistributedTask.WebApi.HelpLink) null;
      serviceEndpointType1.HelpMarkDown = serviceEndpointType.HelpMarkDown;
      serviceEndpointType1.IconUrl = serviceEndpointType.IconUrl;
      serviceEndpointType1.Name = serviceEndpointType.Name;
      serviceEndpointType1.UiContributionId = serviceEndpointType.UiContributionId;
      List<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpointAuthenticationScheme> authenticationSchemes1 = serviceEndpointType1.AuthenticationSchemes;
      List<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointAuthenticationScheme> authenticationSchemes2 = serviceEndpointType.AuthenticationSchemes;
      IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpointAuthenticationScheme> collection1 = authenticationSchemes2 != null ? authenticationSchemes2.Select<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointAuthenticationScheme, Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpointAuthenticationScheme>((Func<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointAuthenticationScheme, Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpointAuthenticationScheme>) (authenticationScheme => authenticationScheme.ToLegacyAuthenticationScheme())) : (IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpointAuthenticationScheme>) null;
      authenticationSchemes1.AddRange(collection1);
      List<Microsoft.TeamFoundation.DistributedTask.WebApi.DataSource> dataSources1 = serviceEndpointType1.DataSources;
      List<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.DataSource> dataSources2 = serviceEndpointType.DataSources;
      IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.DataSource> collection2 = dataSources2 != null ? dataSources2.Select<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.DataSource, Microsoft.TeamFoundation.DistributedTask.WebApi.DataSource>((Func<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.DataSource, Microsoft.TeamFoundation.DistributedTask.WebApi.DataSource>) (dataSource => dataSource.ToLegacyDataSource())) : (IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.DataSource>) null;
      dataSources1.AddRange(collection2);
      List<Microsoft.TeamFoundation.DistributedTask.WebApi.DependencyData> dependencyData1 = serviceEndpointType1.DependencyData;
      List<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.DependencyData> dependencyData2 = serviceEndpointType.DependencyData;
      IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.DependencyData> collection3 = dependencyData2 != null ? dependencyData2.Select<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.DependencyData, Microsoft.TeamFoundation.DistributedTask.WebApi.DependencyData>((Func<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.DependencyData, Microsoft.TeamFoundation.DistributedTask.WebApi.DependencyData>) (dependencyData => dependencyData.ToLegacyDependencyData())) : (IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.DependencyData>) null;
      dependencyData1.AddRange(collection3);
      serviceEndpointType1.InputDescriptors.AddRange((IEnumerable<InputDescriptor>) serviceEndpointType.InputDescriptors);
      serviceEndpointType1.TrustedHosts.AddRange((IEnumerable<string>) serviceEndpointType.TrustedHosts);
      return serviceEndpointType1;
    }

    public static Microsoft.TeamFoundation.DistributedTask.WebApi.EndpointUrl ToLegacyEndpointUrl(
      this Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.EndpointUrl endpointUrl)
    {
      Microsoft.TeamFoundation.DistributedTask.WebApi.EndpointUrl legacyEndpointUrl = new Microsoft.TeamFoundation.DistributedTask.WebApi.EndpointUrl();
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.DependsOn dependsOn = endpointUrl.DependsOn;
      legacyEndpointUrl.DependsOn = dependsOn != null ? dependsOn.ToLegacyDependsOn() : (Microsoft.TeamFoundation.DistributedTask.WebApi.DependsOn) null;
      legacyEndpointUrl.DisplayName = endpointUrl.DisplayName;
      legacyEndpointUrl.HelpText = endpointUrl.HelpText;
      legacyEndpointUrl.IsVisible = endpointUrl.IsVisible;
      legacyEndpointUrl.Value = endpointUrl.Value;
      return legacyEndpointUrl;
    }

    public static Microsoft.TeamFoundation.DistributedTask.WebApi.DependsOn ToLegacyDependsOn(
      this Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.DependsOn dependsOn)
    {
      Microsoft.TeamFoundation.DistributedTask.WebApi.DependsOn legacyDependsOn = new Microsoft.TeamFoundation.DistributedTask.WebApi.DependsOn();
      legacyDependsOn.Input = dependsOn.Input;
      List<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.DependencyBinding> map1 = dependsOn.Map;
      legacyDependsOn.Map = map1 != null ? map1.Select<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.DependencyBinding, Microsoft.TeamFoundation.DistributedTask.WebApi.DependencyBinding>((Func<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.DependencyBinding, Microsoft.TeamFoundation.DistributedTask.WebApi.DependencyBinding>) (map => map.ToLegacyDependencyBinding())).ToList<Microsoft.TeamFoundation.DistributedTask.WebApi.DependencyBinding>() : (List<Microsoft.TeamFoundation.DistributedTask.WebApi.DependencyBinding>) null;
      return legacyDependsOn;
    }

    public static Microsoft.TeamFoundation.DistributedTask.WebApi.DependencyBinding ToLegacyDependencyBinding(
      this Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.DependencyBinding dependencyBinding)
    {
      return new Microsoft.TeamFoundation.DistributedTask.WebApi.DependencyBinding()
      {
        Key = dependencyBinding.Key,
        Value = dependencyBinding.Value
      };
    }

    public static Microsoft.TeamFoundation.DistributedTask.WebApi.HelpLink ToLegacyHelpLink(
      this Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.HelpLink helpLink)
    {
      return new Microsoft.TeamFoundation.DistributedTask.WebApi.HelpLink()
      {
        Text = helpLink.Text,
        Url = helpLink.Url
      };
    }

    public static Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpointAuthenticationScheme ToLegacyAuthenticationScheme(
      this Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointAuthenticationScheme serviceEndpointAuthenticationScheme)
    {
      Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpointAuthenticationScheme authenticationScheme = new Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpointAuthenticationScheme();
      List<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AuthorizationHeader> authorizationHeaders = serviceEndpointAuthenticationScheme.AuthorizationHeaders;
      authenticationScheme.AuthorizationHeaders = authorizationHeaders != null ? authorizationHeaders.Select<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AuthorizationHeader, Microsoft.TeamFoundation.DistributedTask.WebApi.AuthorizationHeader>((Func<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AuthorizationHeader, Microsoft.TeamFoundation.DistributedTask.WebApi.AuthorizationHeader>) (header => header.ToLegacyAuthorizationHeader())).ToList<Microsoft.TeamFoundation.DistributedTask.WebApi.AuthorizationHeader>() : (List<Microsoft.TeamFoundation.DistributedTask.WebApi.AuthorizationHeader>) null;
      authenticationScheme.DisplayName = serviceEndpointAuthenticationScheme.DisplayName;
      authenticationScheme.InputDescriptors = serviceEndpointAuthenticationScheme.InputDescriptors;
      authenticationScheme.Scheme = serviceEndpointAuthenticationScheme.Scheme;
      return authenticationScheme;
    }

    public static Microsoft.TeamFoundation.DistributedTask.WebApi.DataSource ToLegacyDataSource(
      this Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.DataSource dataSource)
    {
      Microsoft.TeamFoundation.DistributedTask.WebApi.DataSource legacyDataSource = new Microsoft.TeamFoundation.DistributedTask.WebApi.DataSource();
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AuthenticationSchemeReference authenticationScheme = dataSource.AuthenticationScheme;
      legacyDataSource.AuthenticationScheme = authenticationScheme != null ? authenticationScheme.ToLegacyAuthenticationSchemeReference() : (Microsoft.TeamFoundation.DistributedTask.WebApi.AuthenticationSchemeReference) null;
      legacyDataSource.EndpointUrl = dataSource.EndpointUrl;
      List<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AuthorizationHeader> headers = dataSource.Headers;
      legacyDataSource.Headers = headers != null ? headers.Select<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AuthorizationHeader, Microsoft.TeamFoundation.DistributedTask.WebApi.AuthorizationHeader>((Func<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AuthorizationHeader, Microsoft.TeamFoundation.DistributedTask.WebApi.AuthorizationHeader>) (header => header.ToLegacyAuthorizationHeader())).ToList<Microsoft.TeamFoundation.DistributedTask.WebApi.AuthorizationHeader>() : (List<Microsoft.TeamFoundation.DistributedTask.WebApi.AuthorizationHeader>) null;
      legacyDataSource.Name = dataSource.Name;
      legacyDataSource.ResourceUrl = dataSource.ResourceUrl;
      legacyDataSource.ResultSelector = dataSource.ResultSelector;
      return legacyDataSource;
    }

    public static Microsoft.TeamFoundation.DistributedTask.WebApi.AuthenticationSchemeReference ToLegacyAuthenticationSchemeReference(
      this Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AuthenticationSchemeReference authenticationSchemeReference)
    {
      return new Microsoft.TeamFoundation.DistributedTask.WebApi.AuthenticationSchemeReference()
      {
        Inputs = authenticationSchemeReference.Inputs,
        Type = authenticationSchemeReference.Type
      };
    }

    public static Microsoft.TeamFoundation.DistributedTask.WebApi.DependencyData ToLegacyDependencyData(
      this Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.DependencyData dependencyData)
    {
      return new Microsoft.TeamFoundation.DistributedTask.WebApi.DependencyData()
      {
        Input = dependencyData.Input,
        Map = dependencyData.Map
      };
    }

    public static Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpointExecutionRecord ToLegacyServiceEndpointExecutionRecord(
      this Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointExecutionRecord executionRecord)
    {
      Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpointExecutionRecord endpointExecutionRecord = new Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpointExecutionRecord();
      endpointExecutionRecord.EndpointId = executionRecord.EndpointId;
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointExecutionData data = executionRecord.Data;
      endpointExecutionRecord.Data = data != null ? data.ToLegacyServiceEndpointExecutionData() : (Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpointExecutionData) null;
      return endpointExecutionRecord;
    }

    public static Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpointExecutionData ToLegacyServiceEndpointExecutionData(
      this Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointExecutionData executionData)
    {
      Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpointExecutionData endpointExecutionData = new Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpointExecutionData();
      ServiceEndpointExecutionOwner definition = executionData.Definition;
      endpointExecutionData.Definition = definition != null ? definition.ToLegacyServiceEndpointExecutionOwner() : (TaskOrchestrationOwner) null;
      endpointExecutionData.FinishTime = executionData.FinishTime;
      endpointExecutionData.Id = executionData.Id;
      ServiceEndpointExecutionOwner owner = executionData.Owner;
      endpointExecutionData.Owner = owner != null ? owner.ToLegacyServiceEndpointExecutionOwner() : (TaskOrchestrationOwner) null;
      endpointExecutionData.PlanType = executionData.PlanType;
      endpointExecutionData.Result = ServiceEndpointConverter.ToLegacyServiceEndpointExecutionResult(executionData.Result);
      endpointExecutionData.StartTime = executionData.StartTime;
      return endpointExecutionData;
    }

    public static TaskOrchestrationOwner ToLegacyServiceEndpointExecutionOwner(
      this ServiceEndpointExecutionOwner serviceEndpointExecutionOwner)
    {
      return new TaskOrchestrationOwner()
      {
        Id = serviceEndpointExecutionOwner.Id,
        Name = serviceEndpointExecutionOwner.Name
      };
    }

    public static TaskResult? ToLegacyServiceEndpointExecutionResult(
      ServiceEndpointExecutionResult? serviceEndpointExecutionResult)
    {
      if (!serviceEndpointExecutionResult.HasValue)
        return new TaskResult?();
      if (serviceEndpointExecutionResult.HasValue)
      {
        switch (serviceEndpointExecutionResult.GetValueOrDefault())
        {
          case ServiceEndpointExecutionResult.Succeeded:
            return new TaskResult?(TaskResult.Succeeded);
          case ServiceEndpointExecutionResult.SucceededWithIssues:
            return new TaskResult?(TaskResult.SucceededWithIssues);
          case ServiceEndpointExecutionResult.Failed:
            return new TaskResult?(TaskResult.Failed);
          case ServiceEndpointExecutionResult.Canceled:
            return new TaskResult?(TaskResult.Canceled);
          case ServiceEndpointExecutionResult.Skipped:
            return new TaskResult?(TaskResult.Skipped);
          case ServiceEndpointExecutionResult.Abandoned:
            return new TaskResult?(TaskResult.Abandoned);
        }
      }
      return new TaskResult?();
    }

    public static Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointExecutionRecordsInput ToServiceEndpointExecutionRecordsInput(
      this Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpointExecutionRecordsInput legacyExecutionRecordsInput)
    {
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointExecutionRecordsInput executionRecordsInput = new Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointExecutionRecordsInput();
      executionRecordsInput.EndpointIds = legacyExecutionRecordsInput.EndpointIds;
      Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpointExecutionData data = legacyExecutionRecordsInput.Data;
      executionRecordsInput.Data = data != null ? data.ToServiceEndpointExecutionData() : (Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointExecutionData) null;
      return executionRecordsInput;
    }

    public static ServiceEndpointExecutionOwner ToServiceEndpointExecutionOwner(
      this TaskOrchestrationOwner taskOrchestrationOwner)
    {
      return new ServiceEndpointExecutionOwner()
      {
        Id = taskOrchestrationOwner.Id,
        Name = taskOrchestrationOwner.Name
      };
    }

    public static ServiceEndpointExecutionResult? ToServiceEndpointExecutionResult(
      TaskResult? taskResult)
    {
      if (!taskResult.HasValue)
        return new ServiceEndpointExecutionResult?();
      if (taskResult.HasValue)
      {
        switch (taskResult.GetValueOrDefault())
        {
          case TaskResult.Succeeded:
            return new ServiceEndpointExecutionResult?(ServiceEndpointExecutionResult.Succeeded);
          case TaskResult.SucceededWithIssues:
            return new ServiceEndpointExecutionResult?(ServiceEndpointExecutionResult.SucceededWithIssues);
          case TaskResult.Failed:
            return new ServiceEndpointExecutionResult?(ServiceEndpointExecutionResult.Failed);
          case TaskResult.Canceled:
            return new ServiceEndpointExecutionResult?(ServiceEndpointExecutionResult.Canceled);
          case TaskResult.Skipped:
            return new ServiceEndpointExecutionResult?(ServiceEndpointExecutionResult.Skipped);
          case TaskResult.Abandoned:
            return new ServiceEndpointExecutionResult?(ServiceEndpointExecutionResult.Abandoned);
        }
      }
      return new ServiceEndpointExecutionResult?();
    }

    public static Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointExecutionData ToServiceEndpointExecutionData(
      this Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpointExecutionData executionData)
    {
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointExecutionData endpointExecutionData = new Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointExecutionData();
      TaskOrchestrationOwner definition = executionData.Definition;
      endpointExecutionData.Definition = definition != null ? definition.ToServiceEndpointExecutionOwner() : (ServiceEndpointExecutionOwner) null;
      endpointExecutionData.FinishTime = executionData.FinishTime;
      endpointExecutionData.Id = executionData.Id;
      TaskOrchestrationOwner owner = executionData.Owner;
      endpointExecutionData.Owner = owner != null ? owner.ToServiceEndpointExecutionOwner() : (ServiceEndpointExecutionOwner) null;
      endpointExecutionData.PlanType = executionData.PlanType;
      endpointExecutionData.Result = ServiceEndpointConverter.ToServiceEndpointExecutionResult(executionData.Result);
      endpointExecutionData.StartTime = executionData.StartTime;
      return endpointExecutionData;
    }

    public static Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.DataSourceBinding ToDataSourceBinding(
      this Microsoft.TeamFoundation.DistributedTask.WebApi.DataSourceBinding dataSourceBinding)
    {
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.DataSourceBinding dataSourceBinding1 = new Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.DataSourceBinding();
      dataSourceBinding1.DataSourceName = dataSourceBinding.DataSourceName;
      dataSourceBinding1.EndpointId = dataSourceBinding.EndpointId;
      dataSourceBinding1.EndpointUrl = dataSourceBinding.EndpointUrl;
      dataSourceBinding1.Headers = dataSourceBinding.Headers;
      dataSourceBinding1.ResultSelector = dataSourceBinding.ResultSelector;
      dataSourceBinding1.ResultTemplate = dataSourceBinding.ResultTemplate;
      dataSourceBinding1.Target = dataSourceBinding.Target;
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.DataSourceBinding dataSourceBinding2 = dataSourceBinding1;
      foreach (KeyValuePair<string, string> parameter in dataSourceBinding.Parameters)
        dataSourceBinding2.Parameters.Add(parameter.Key, parameter.Value);
      return dataSourceBinding2;
    }

    public static Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint ToServiceEndpoint(
      this Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint legacyServiceEndpoint)
    {
      if (legacyServiceEndpoint == null)
        return (Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint) null;
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint serviceEndpoint = new Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint();
      serviceEndpoint.AdministratorsGroup = legacyServiceEndpoint.AdministratorsGroup;
      Microsoft.TeamFoundation.DistributedTask.WebApi.EndpointAuthorization authorization = legacyServiceEndpoint.Authorization;
      serviceEndpoint.Authorization = authorization != null ? authorization.ToEndpointAuthorization() : (Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.EndpointAuthorization) null;
      serviceEndpoint.CreatedBy = legacyServiceEndpoint.CreatedBy;
      serviceEndpoint.Data = legacyServiceEndpoint.Data;
      serviceEndpoint.Description = legacyServiceEndpoint.Description;
      serviceEndpoint.GroupScopeId = legacyServiceEndpoint.GroupScopeId;
      serviceEndpoint.Id = legacyServiceEndpoint.Id;
      serviceEndpoint.IsReady = legacyServiceEndpoint.IsReady;
      serviceEndpoint.IsDisabled = legacyServiceEndpoint.IsDisabled;
      serviceEndpoint.Name = legacyServiceEndpoint.Name;
      serviceEndpoint.OperationStatus = legacyServiceEndpoint.OperationStatus;
      serviceEndpoint.ReadersGroup = legacyServiceEndpoint.ReadersGroup;
      serviceEndpoint.Type = legacyServiceEndpoint.Type;
      serviceEndpoint.Url = legacyServiceEndpoint.Url;
      return serviceEndpoint;
    }

    public static Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.EndpointAuthorization ToEndpointAuthorization(
      this Microsoft.TeamFoundation.DistributedTask.WebApi.EndpointAuthorization legacyEndpointAuthorization)
    {
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.EndpointAuthorization endpointAuthorization = new Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.EndpointAuthorization()
      {
        Scheme = legacyEndpointAuthorization.Scheme
      };
      foreach (KeyValuePair<string, string> parameter in (IEnumerable<KeyValuePair<string, string>>) legacyEndpointAuthorization.Parameters)
        endpointAuthorization.Parameters.Add(parameter.Key, parameter.Value);
      return endpointAuthorization;
    }

    public static Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AuthorizationHeader ToAuthorizationHeader(
      this Microsoft.TeamFoundation.DistributedTask.WebApi.AuthorizationHeader legacyAuthorizationHeader)
    {
      return new Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AuthorizationHeader()
      {
        Name = legacyAuthorizationHeader.Name,
        Value = legacyAuthorizationHeader.Value
      };
    }

    public static Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointRequest ToServiceEndpointRequest(
      this Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpointRequest legacyServiceEndpointRequest)
    {
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointRequest serviceEndpointRequest = new Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointRequest();
      Microsoft.TeamFoundation.DistributedTask.WebApi.DataSourceDetails dataSourceDetails = legacyServiceEndpointRequest.DataSourceDetails;
      serviceEndpointRequest.DataSourceDetails = dataSourceDetails != null ? dataSourceDetails.ToDataSourceDetails() : (Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.DataSourceDetails) null;
      Microsoft.TeamFoundation.DistributedTask.WebApi.ResultTransformationDetails transformationDetails = legacyServiceEndpointRequest.ResultTransformationDetails;
      serviceEndpointRequest.ResultTransformationDetails = transformationDetails != null ? transformationDetails.ToResultTransformationDetails() : (Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ResultTransformationDetails) null;
      Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpointDetails serviceEndpointDetails = legacyServiceEndpointRequest.ServiceEndpointDetails;
      serviceEndpointRequest.ServiceEndpointDetails = serviceEndpointDetails != null ? serviceEndpointDetails.ToServiceEndpointDetails() : (Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointDetails) null;
      return serviceEndpointRequest;
    }

    public static Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AadLoginPromptOption ToAadLoginPromptOption(
      this Microsoft.TeamFoundation.DistributedTask.WebApi.AadLoginPromptOption legacyAadLoginPromptOption)
    {
      switch (legacyAadLoginPromptOption)
      {
        case Microsoft.TeamFoundation.DistributedTask.WebApi.AadLoginPromptOption.NoOption:
          return Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AadLoginPromptOption.NoOption;
        case Microsoft.TeamFoundation.DistributedTask.WebApi.AadLoginPromptOption.Login:
          return Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AadLoginPromptOption.Login;
        case Microsoft.TeamFoundation.DistributedTask.WebApi.AadLoginPromptOption.SelectAccount:
          return Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AadLoginPromptOption.SelectAccount;
        case Microsoft.TeamFoundation.DistributedTask.WebApi.AadLoginPromptOption.FreshLogin:
          return Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AadLoginPromptOption.FreshLogin;
        case Microsoft.TeamFoundation.DistributedTask.WebApi.AadLoginPromptOption.FreshLoginWithMfa:
          return Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AadLoginPromptOption.FreshLoginWithMfa;
        default:
          return Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AadLoginPromptOption.SelectAccount;
      }
    }

    public static Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AadOauthTokenRequest ToAadOAuthTokenRequest(
      this Microsoft.TeamFoundation.DistributedTask.WebApi.AadOauthTokenRequest legacyAadOauthTokenRequest)
    {
      return new Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AadOauthTokenRequest()
      {
        TenantId = legacyAadOauthTokenRequest.TenantId,
        Refresh = legacyAadOauthTokenRequest.Refresh,
        Resource = legacyAadOauthTokenRequest.Resource,
        Token = legacyAadOauthTokenRequest.Token
      };
    }

    public static Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.DataSourceDetails ToDataSourceDetails(
      this Microsoft.TeamFoundation.DistributedTask.WebApi.DataSourceDetails legacyDataSourceDetails)
    {
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.DataSourceDetails dataSourceDetails1 = new Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.DataSourceDetails();
      dataSourceDetails1.DataSourceName = legacyDataSourceDetails.DataSourceName;
      dataSourceDetails1.DataSourceUrl = legacyDataSourceDetails.DataSourceUrl;
      List<Microsoft.TeamFoundation.DistributedTask.WebApi.AuthorizationHeader> headers = legacyDataSourceDetails.Headers;
      dataSourceDetails1.Headers = headers != null ? headers.Select<Microsoft.TeamFoundation.DistributedTask.WebApi.AuthorizationHeader, Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AuthorizationHeader>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.AuthorizationHeader, Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AuthorizationHeader>) (header => header.ToAuthorizationHeader())).ToList<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AuthorizationHeader>() : (List<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AuthorizationHeader>) null;
      dataSourceDetails1.ResourceUrl = legacyDataSourceDetails.ResourceUrl;
      dataSourceDetails1.ResultSelector = legacyDataSourceDetails.ResultSelector;
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.DataSourceDetails dataSourceDetails2 = dataSourceDetails1;
      foreach (KeyValuePair<string, string> parameter in legacyDataSourceDetails.Parameters)
        dataSourceDetails2.Parameters.Add(parameter.Key, parameter.Value);
      return dataSourceDetails2;
    }

    public static Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ResultTransformationDetails ToResultTransformationDetails(
      this Microsoft.TeamFoundation.DistributedTask.WebApi.ResultTransformationDetails legacyResultTransformationDetails)
    {
      return new Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ResultTransformationDetails()
      {
        ResultTemplate = legacyResultTransformationDetails.ResultTemplate
      };
    }

    public static Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointDetails ToServiceEndpointDetails(
      this Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpointDetails legacyServiceEndpointDetails)
    {
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointDetails serviceEndpointDetails = new Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointDetails();
      Microsoft.TeamFoundation.DistributedTask.WebApi.EndpointAuthorization authorization = legacyServiceEndpointDetails.Authorization;
      serviceEndpointDetails.Authorization = authorization != null ? authorization.ToEndpointAuthorization() : (Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.EndpointAuthorization) null;
      serviceEndpointDetails.Data = legacyServiceEndpointDetails.Data;
      serviceEndpointDetails.Type = legacyServiceEndpointDetails.Type;
      serviceEndpointDetails.Url = legacyServiceEndpointDetails.Url;
      return serviceEndpointDetails;
    }
  }
}
