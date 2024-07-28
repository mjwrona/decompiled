// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.TeamFoundationResourceHydrationService
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.WindowsAzure.ResourceGroup.ResourceHydration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class TeamFoundationResourceHydrationService : 
    IResourceHydrationService,
    IVssFrameworkService
  {
    private const string ResourceHydrationKey = "ResourceHydrationStoreConnectionString";
    private const string ResourceHydrationResourceProviderName = "/Service/Commerce/ResourceHydration/ResourceProvider";
    private const string ResourceHydrationResourceProviderRegion = "/Service/Commerce/ResourceHydration/Region";
    private const string ResourceHydrationResourceProviderApiVersion = "/Service/Commerce/ResourceHydration/ApiVersion";
    private static readonly string Area = "Commerce";
    private static readonly string Layer = nameof (TeamFoundationResourceHydrationService);
    private const string kpi_area = "Microsoft.VisualStudio.Commerce";
    private const string create_kpi_name = "ResourceHydrationCreate";
    private const string delete_kpi_name = "ResourceHydrationDelete";
    private bool dispose;

    internal ResourceHydrationClient Client { get; set; }

    public void ServiceStart(IVssRequestContext requestContext)
    {
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
        throw new InvalidOperationException(FrameworkResources.ServiceAvailableInHostedTfsOnly());
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      TeamFoundationStrongBoxService service = vssRequestContext.GetService<TeamFoundationStrongBoxService>();
      try
      {
        string connectionString = this.GetStrongBoxConnectionString(vssRequestContext);
        this.Client = (ResourceHydrationClient) null;
        if (!connectionString.Equals("DefaultEndpointsProtocol=https;AccountName=<Azure Storage Account Name>;AccountKey=<Azure Storage Primary Key>", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(connectionString))
          this.Client = new ResourceHydrationClient(connectionString);
        service.RegisterNotification(requestContext, new StrongBoxItemChangedCallback(this.OnStrongBoxItemChanged), FrameworkServerConstants.ConfigurationSecretsDrawerName, (IEnumerable<string>) new string[1]
        {
          "ResourceHydrationStoreConnectionString"
        });
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5106160, TeamFoundationResourceHydrationService.Area, TeamFoundationResourceHydrationService.Layer, ex);
        throw;
      }
    }

    [ExcludeFromCodeCoverage]
    internal virtual string GetStrongBoxConnectionString(IVssRequestContext requestContext)
    {
      try
      {
        TeamFoundationStrongBoxService service = requestContext.GetService<TeamFoundationStrongBoxService>();
        Guid drawerId = service.UnlockDrawer(requestContext, FrameworkServerConstants.ConfigurationSecretsDrawerName, true);
        return service.GetString(requestContext, drawerId, "ResourceHydrationStoreConnectionString");
      }
      catch (StrongBoxItemNotFoundException ex)
      {
        requestContext.Trace(5109021, TraceLevel.Error, "Commerce", TeamFoundationResourceHydrationService.Layer, "Unable to retrieve the storage from the strongbox using the ResourceHydrationStoreConnectionString key so returning empty string.");
        requestContext.TraceException(5109021, "Commerce", TeamFoundationResourceHydrationService.Layer, (Exception) ex);
        TeamFoundationEventLog.Default.Log(requestContext, "StrongBoxFailedEvent", TeamFoundationEventId.StrongBoxFailedEvent, EventLogEntryType.Error, (object) "ResourceHydrationStoreConnectionString");
        throw;
      }
    }

    public void Cleanup(IVssRequestContext requestContext)
    {
      if (!this.dispose)
        this.UnRegisterNotification(requestContext);
      this.dispose = true;
    }

    private void UnRegisterNotification(IVssRequestContext requestContext) => requestContext.GetService<ITeamFoundationStrongBoxService>().UnregisterNotification(requestContext, new StrongBoxItemChangedCallback(this.OnStrongBoxItemChanged));

    private void OnStrongBoxItemChanged(
      IVssRequestContext requestContext,
      IEnumerable<StrongBoxItemName> itemNames)
    {
      string connectionString = this.GetStrongBoxConnectionString(requestContext.To(TeamFoundationHostType.Deployment).Elevate());
      this.Client = (ResourceHydrationClient) null;
      if (connectionString.Equals("DefaultEndpointsProtocol=https;AccountName=<Azure Storage Account Name>;AccountKey=<Azure Storage Primary Key>", StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty(connectionString))
        return;
      this.Client = new ResourceHydrationClient(connectionString);
    }

    public void ServiceEnd(IVssRequestContext requestContext) => this.Cleanup(requestContext);

    private void PublishIncrementKpiByOne(IVssRequestContext requestContext, string KpiName)
    {
      KpiMetric metric = new KpiMetric()
      {
        Name = KpiName,
        Value = 1.0,
        TimeStamp = this.GetUtcNow()
      };
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
      vssRequestContext.GetService<KpiService>().Publish(vssRequestContext, "Microsoft.VisualStudio.Commerce", metric);
    }

    public void CreateResourceHydrationRequest(
      IVssRequestContext requestContext,
      string subscriptionId,
      string resourceGroup,
      string resourceType,
      string resourceName)
    {
      requestContext.TraceEnter(5106170, TeamFoundationResourceHydrationService.Area, TeamFoundationResourceHydrationService.Layer, nameof (CreateResourceHydrationRequest));
      try
      {
        if (this.Client == null)
        {
          requestContext.Trace(5106171, TraceLevel.Info, TeamFoundationResourceHydrationService.Area, TeamFoundationResourceHydrationService.Layer, "Default connection string detected, ignoring hydration request");
        }
        else
        {
          ResourceHydrationRequest hydrationRequest = this.GetResourceHydrationRequest(requestContext, subscriptionId, resourceGroup, resourceType, resourceName, ProvisioningOperation.Create);
          this.Client.CommitResourceHydrationRequest(requestContext, hydrationRequest);
          this.PublishIncrementKpiByOne(requestContext, "ResourceHydrationCreate");
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5106178, TeamFoundationResourceHydrationService.Area, TeamFoundationResourceHydrationService.Layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5106179, TeamFoundationResourceHydrationService.Area, TeamFoundationResourceHydrationService.Layer, nameof (CreateResourceHydrationRequest));
      }
    }

    public void CreateResourceHydrationRequest(
      IVssRequestContext requestContext,
      string subscriptionId,
      string resourceGroup,
      string resourceType,
      string resourceName,
      string resourceRegion)
    {
      requestContext.TraceEnter(5106170, TeamFoundationResourceHydrationService.Area, TeamFoundationResourceHydrationService.Layer, nameof (CreateResourceHydrationRequest));
      try
      {
        if (this.Client == null)
        {
          requestContext.Trace(5106171, TraceLevel.Info, TeamFoundationResourceHydrationService.Area, TeamFoundationResourceHydrationService.Layer, "Default connection string detected, ignoring hydration request");
        }
        else
        {
          ResourceHydrationRequest hydrationRequest = this.GetResourceHydrationRequest(requestContext, subscriptionId, resourceGroup, resourceType, resourceName, resourceRegion, ProvisioningOperation.Create);
          this.Client.CommitResourceHydrationRequest(requestContext, hydrationRequest);
          this.PublishIncrementKpiByOne(requestContext, "ResourceHydrationCreate");
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5106178, TeamFoundationResourceHydrationService.Area, TeamFoundationResourceHydrationService.Layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5106179, TeamFoundationResourceHydrationService.Area, TeamFoundationResourceHydrationService.Layer, nameof (CreateResourceHydrationRequest));
      }
    }

    public void DeleteResourceHydrationRequest(
      IVssRequestContext requestContext,
      string subscriptionId,
      string resourceGroup,
      string resourceType,
      string resourceName)
    {
      requestContext.TraceEnter(5106180, TeamFoundationResourceHydrationService.Area, TeamFoundationResourceHydrationService.Layer, nameof (DeleteResourceHydrationRequest));
      try
      {
        if (this.Client == null)
        {
          requestContext.Trace(5106181, TraceLevel.Info, TeamFoundationResourceHydrationService.Area, TeamFoundationResourceHydrationService.Layer, "Default connection string detected, ignoring hydration request");
        }
        else
        {
          ResourceHydrationRequest hydrationRequest = this.GetResourceHydrationRequest(requestContext, subscriptionId, resourceGroup, resourceType, resourceName, ProvisioningOperation.Delete);
          this.Client.CommitResourceHydrationRequest(requestContext, hydrationRequest);
          this.PublishIncrementKpiByOne(requestContext, "ResourceHydrationDelete");
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5106188, TeamFoundationResourceHydrationService.Area, TeamFoundationResourceHydrationService.Layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5106189, TeamFoundationResourceHydrationService.Area, TeamFoundationResourceHydrationService.Layer, nameof (DeleteResourceHydrationRequest));
      }
    }

    public void DeleteResourceHydrationRequest(
      IVssRequestContext requestContext,
      string subscriptionId,
      string resourceGroup,
      string resourceType,
      string resourceName,
      string resourceRegion)
    {
      requestContext.TraceEnter(5106180, TeamFoundationResourceHydrationService.Area, TeamFoundationResourceHydrationService.Layer, nameof (DeleteResourceHydrationRequest));
      try
      {
        if (this.Client == null)
        {
          requestContext.Trace(5106181, TraceLevel.Info, TeamFoundationResourceHydrationService.Area, TeamFoundationResourceHydrationService.Layer, "Default connection string detected, ignoring hydration request");
        }
        else
        {
          ResourceHydrationRequest hydrationRequest = this.GetResourceHydrationRequest(requestContext, subscriptionId, resourceGroup, resourceType, resourceName, resourceRegion, ProvisioningOperation.Delete);
          this.Client.CommitResourceHydrationRequest(requestContext, hydrationRequest);
          this.PublishIncrementKpiByOne(requestContext, "ResourceHydrationDelete");
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5106188, TeamFoundationResourceHydrationService.Area, TeamFoundationResourceHydrationService.Layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5106189, TeamFoundationResourceHydrationService.Area, TeamFoundationResourceHydrationService.Layer, nameof (DeleteResourceHydrationRequest));
      }
    }

    public void DeleteResourceHydrationRequest(
      IVssRequestContext requestContext,
      string subscriptionId,
      string resourceGroup,
      string resourceType,
      string resourceName,
      string childResourceType,
      string childResourceName)
    {
      requestContext.TraceEnter(5106180, TeamFoundationResourceHydrationService.Area, TeamFoundationResourceHydrationService.Layer, nameof (DeleteResourceHydrationRequest));
      try
      {
        if (this.Client == null)
        {
          requestContext.Trace(5106181, TraceLevel.Info, TeamFoundationResourceHydrationService.Area, TeamFoundationResourceHydrationService.Layer, "Default connection string detected, ignoring hydration request");
        }
        else
        {
          ResourceHydrationRequest hydrationRequest = this.GetResourceHydrationRequest(requestContext, subscriptionId, resourceGroup, resourceType, resourceName, childResourceType, childResourceName, ProvisioningOperation.Delete);
          this.Client.CommitResourceHydrationRequest(requestContext, hydrationRequest);
          this.PublishIncrementKpiByOne(requestContext, "ResourceHydrationDelete");
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5106188, TeamFoundationResourceHydrationService.Area, TeamFoundationResourceHydrationService.Layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5106189, TeamFoundationResourceHydrationService.Area, TeamFoundationResourceHydrationService.Layer, nameof (DeleteResourceHydrationRequest));
      }
    }

    internal ResourceHydrationRequest GetResourceHydrationRequest(
      IVssRequestContext requestContext,
      string subscriptionId,
      string resourceGroup,
      string resourceType,
      string resourceName,
      ProvisioningOperation operation)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      string resourceRegion = vssRequestContext.GetService<IVssRegistryService>().GetValue<string>(vssRequestContext, (RegistryQuery) "/Service/Commerce/ResourceHydration/Region", string.Empty);
      return this.GetResourceHydrationRequest(requestContext, subscriptionId, resourceGroup, resourceType, resourceName, resourceRegion, operation);
    }

    internal ResourceHydrationRequest GetResourceHydrationRequest(
      IVssRequestContext requestContext,
      string subscriptionId,
      string resourceGroup,
      string resourceType,
      string resourceName,
      string resourceRegion,
      ProvisioningOperation operation)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IVssRegistryService service = vssRequestContext.GetService<IVssRegistryService>();
      string str1 = service.GetValue<string>(vssRequestContext, (RegistryQuery) "/Service/Commerce/ResourceHydration/ResourceProvider", string.Empty);
      string str2 = service.GetValue<string>(vssRequestContext, (RegistryQuery) "/Service/Commerce/ResourceHydration/ApiVersion", string.Empty);
      string str3 = "/subscriptions/" + subscriptionId + "/resourceGroups/" + resourceGroup + "/providers/" + str1 + "/" + resourceType + "/" + resourceName;
      return new ResourceHydrationRequest()
      {
        ResourceOperation = operation,
        ResourceUri = str3,
        ResourceLocation = resourceRegion,
        ApiVersion = str2,
        CorrelationId = requestContext.ActivityId.ToString()
      };
    }

    internal ResourceHydrationRequest GetResourceHydrationRequest(
      IVssRequestContext requestContext,
      string subscriptionId,
      string resourceGroup,
      string resourceType,
      string resourceName,
      string childResourceType,
      string childResourceName,
      ProvisioningOperation operation)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      string resourceRegion = vssRequestContext.GetService<IVssRegistryService>().GetValue<string>(vssRequestContext, (RegistryQuery) "/Service/Commerce/ResourceHydration/Region", string.Empty);
      return this.GetResourceHydrationRequest(requestContext, subscriptionId, resourceGroup, resourceType, resourceName, childResourceType, childResourceName, resourceRegion, operation);
    }

    internal ResourceHydrationRequest GetResourceHydrationRequest(
      IVssRequestContext requestContext,
      string subscriptionId,
      string resourceGroup,
      string resourceType,
      string resourceName,
      string childResourceType,
      string childResourceName,
      string resourceRegion,
      ProvisioningOperation operation)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IVssRegistryService service = vssRequestContext.GetService<IVssRegistryService>();
      string str1 = service.GetValue<string>(vssRequestContext, (RegistryQuery) "/Service/Commerce/ResourceHydration/ResourceProvider", string.Empty);
      string str2 = service.GetValue<string>(vssRequestContext, (RegistryQuery) "/Service/Commerce/ResourceHydration/ApiVersion", string.Empty);
      string str3 = "/subscriptions/" + subscriptionId + "/resourceGroups/" + resourceGroup + "/providers/" + str1 + "/" + resourceType + "/" + resourceName + "/" + childResourceType + "/" + childResourceName;
      return new ResourceHydrationRequest()
      {
        ResourceOperation = operation,
        ResourceUri = str3,
        ResourceLocation = resourceRegion,
        ApiVersion = str2,
        CorrelationId = requestContext.ActivityId.ToString()
      };
    }

    internal virtual DateTime GetUtcNow() => DateTime.UtcNow;
  }
}
