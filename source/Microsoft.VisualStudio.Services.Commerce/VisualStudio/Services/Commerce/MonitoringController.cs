// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.MonitoringController
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Commerce.Common;
using Microsoft.WindowsAzure.Management.Monitoring.ResourceProvider.DataContracts;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web.Http;
using System.Xml;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class MonitoringController : CommerceControllerBase
  {
    private IUsageEventsStore usageEventStore;
    private MonitoringController.IUsageEventStoreInitializer usageStoreInitializer;
    private MonitoringController.IUsageDataAggregatorResolver usageDataAggregatorResolver;
    private static readonly Dictionary<string, MetricCategory> metricCategoryMappings = new Dictionary<string, MetricCategory>()
    {
      {
        "Build",
        MetricCategory.Default
      },
      {
        "LoadTest",
        MetricCategory.Default
      }
    };
    private static readonly List<ResourceName> monitoredResources = new List<ResourceName>()
    {
      ResourceName.Build,
      ResourceName.LoadTest
    };
    private Dictionary<ResourceName, MeteredResource> defaultQuantities;

    internal virtual void Init()
    {
      if (this.TfsRequestContext == null)
        return;
      if (this.usageStoreInitializer == null)
        this.usageStoreInitializer = (MonitoringController.IUsageEventStoreInitializer) new MonitoringController.UsageEventStoreInitializer();
      if (this.usageEventStore == null)
        this.usageEventStore = this.usageStoreInitializer.InitializeUsageEventStore(this.TfsRequestContext);
      if (this.usageDataAggregatorResolver != null)
        return;
      this.usageDataAggregatorResolver = (MonitoringController.IUsageDataAggregatorResolver) new MonitoringController.UsageDataAggregatorResolver();
    }

    public MonitoringController() => this.Init();

    internal MonitoringController(
      MonitoringController.IUsageEventStoreInitializer usageEventStoreInitializer,
      MonitoringController.IUsageDataAggregatorResolver usageDataAggregatorResolver)
    {
      if (this.TfsRequestContext == null)
        return;
      this.usageEventStore = usageEventStoreInitializer.InitializeUsageEventStore(this.TfsRequestContext);
      this.usageStoreInitializer = usageEventStoreInitializer;
      this.usageDataAggregatorResolver = usageDataAggregatorResolver;
    }

    internal virtual string SerializeJson<T>(T obj)
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        new DataContractJsonSerializer(typeof (T)).WriteObject((Stream) memoryStream, (object) obj);
        return Encoding.UTF8.GetString(memoryStream.ToArray());
      }
    }

    internal virtual string GetUnit(ResourceName resourceName)
    {
      if (this.defaultQuantities == null)
        this.defaultQuantities = this.InitDefaultQuantities();
      string unit = "Seats";
      if (this.defaultQuantities.ContainsKey(resourceName))
      {
        MeteredResource defaultQuantity = this.defaultQuantities[resourceName];
        if (defaultQuantity.Unit != null)
          unit = defaultQuantity.Unit;
      }
      return unit;
    }

    internal virtual Dictionary<ResourceName, MeteredResource> InitDefaultQuantities()
    {
      IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<IOfferMeterService>().GetOfferMeters(vssRequestContext).Select<IOfferMeter, MeteredResource>((Func<IOfferMeter, MeteredResource>) (s => ((OfferMeter) s).ToMeteredResource())).Where<MeteredResource>((Func<MeteredResource, bool>) (s => s != null)).ToDictionary<MeteredResource, ResourceName, MeteredResource>((Func<MeteredResource, ResourceName>) (s => s.ResourceName), (Func<MeteredResource, MeteredResource>) (s => s));
    }

    internal override string Layer => nameof (MonitoringController);

    [HttpGet]
    [ActionName("Usages")]
    [TraceFilter(5106001, 5106010)]
    [TraceExceptions(5106008)]
    [TraceRequest(5106002)]
    [TraceResponse(5106007)]
    public List<ResourceUsageMetric> GetUsages(
      string subscriptionId,
      string cloudServiceName,
      string resourceType,
      string resourceName)
    {
      AzureResourceAccount azureResourceAccount = this.TfsRequestContext.GetService<PlatformSubscriptionService>().GetAzureResourceAccount(this.TfsRequestContext, new Guid(subscriptionId), AccountProviderNamespace.VisualStudioOnline, resourceName, true);
      if (azureResourceAccount == null)
        throw new AzureResourceAccountDoesNotExistException(resourceName);
      List<ResourceUsageMetric> usageMetricList = new List<ResourceUsageMetric>();
      IOfferSubscriptionService offerSubscriptionService = this.TfsRequestContext.GetService<IOfferSubscriptionService>();
      CollectionHelper.WithCollectionContext(this.TfsRequestContext, azureResourceAccount.AccountId, (Action<IVssRequestContext>) (accountContext => usageMetricList.AddRange(MonitoringController.monitoredResources.Select<ResourceName, ResourceUsageMetric>((Func<ResourceName, ResourceUsageMetric>) (resource =>
      {
        ISubscriptionResource subscriptionResource = offerSubscriptionService.GetOfferSubscription(accountContext, resource.ToString()).ToSubscriptionResource();
        return new ResourceUsageMetric()
        {
          CurrentValue = (double) subscriptionResource.CommittedQuantity,
          Limit = (double) subscriptionResource.MaximumQuantity,
          Name = resource.ToString(),
          NextResetTime = new DateTime?(subscriptionResource.ResetDate),
          Unit = this.GetUnit(resource)
        };
      })))), new RequestContextType?(RequestContextType.SystemContext), nameof (GetUsages));
      return usageMetricList;
    }

    public virtual ResourceMetricResponses GetMetricsByName(
      string subscriptionId,
      string cloudServiceName,
      string resourceType,
      string resourceName,
      string[] metricNames,
      DateTime startTime,
      DateTime endTime,
      TimeSpan timeGrain)
    {
      this.Init();
      ITeamFoundationHostManagementService service1 = this.TfsRequestContext.Elevate().GetService<ITeamFoundationHostManagementService>();
      AzureResourceAccount azureResourceAccount = this.TfsRequestContext.GetService<PlatformSubscriptionService>().GetAzureResourceAccount(this.TfsRequestContext, new Guid(subscriptionId), AccountProviderNamespace.VisualStudioOnline, resourceName, true);
      if (azureResourceAccount == null)
        throw new AzureResourceAccountDoesNotExistException(resourceName);
      ResourceMetricResponses metricsByName1 = new ResourceMetricResponses();
      List<ResourceName> resourceNameList = new List<ResourceName>();
      foreach (string metricName in metricNames)
      {
        ResourceName result;
        if (System.Enum.TryParse<ResourceName>(metricName, out result))
          resourceNameList.Add(result);
      }
      if (timeGrain.TotalDays == (double) (int) timeGrain.TotalDays || timeGrain.TotalHours == (double) (int) timeGrain.TotalHours)
      {
        IOfferSubscriptionService service2 = this.TfsRequestContext.GetService<IOfferSubscriptionService>();
        using (IVssRequestContext requestContext = service1.BeginRequest(this.TfsRequestContext, azureResourceAccount.AccountId, RequestContextType.SystemContext))
        {
          foreach (ResourceName resourceName1 in resourceNameList)
          {
            ResourceMetricResponse resourceMetricResponse = new ResourceMetricResponse()
            {
              Data = new ResourceMetricSet()
              {
                Name = resourceName1.ToString(),
                DisplayName = resourceName1.ToString(),
                StartTime = startTime,
                EndTime = endTime,
                Unit = this.GetUnit(resourceName1),
                TimeGrain = timeGrain,
                Payload = new List<ResourceMetricSample>()
              }
            };
            IEnumerable<ResourceMetricSample> collection = service2.GetUsage(requestContext, startTime, endTime, timeGrain, resourceName1).Select<IUsageEventAggregate, ResourceMetricSample>((Func<IUsageEventAggregate, ResourceMetricSample>) (x => new ResourceMetricSample()
            {
              Total = new double?((double) x.Value),
              TimeCreated = x.StartTime
            }));
            resourceMetricResponse.Data.Payload.AddRange(collection);
            metricsByName1.Add(resourceMetricResponse);
          }
        }
        return metricsByName1;
      }
      UsageDataAggregator usageDataAggregator = this.usageDataAggregatorResolver.GetUsageDataAggregator(endTime - startTime, timeGrain);
      if (azureResourceAccount.AccountId != Guid.Empty)
      {
        using (IVssRequestContext requestContext = service1.BeginRequest(this.TfsRequestContext, azureResourceAccount.AccountId, RequestContextType.SystemContext))
          return usageDataAggregator.AggregateUsage(requestContext, this.usageEventStore, startTime, endTime, (IEnumerable<ResourceName>) resourceNameList);
      }
      else
      {
        ResourceMetricResponses metricsByName2 = new ResourceMetricResponses();
        metricsByName2.Add(this.GetDefaultResponse(startTime, endTime, timeGrain, ResourceName.Build));
        metricsByName2.Add(this.GetDefaultResponse(startTime, endTime, timeGrain, ResourceName.LoadTest));
        return metricsByName2;
      }
    }

    internal virtual ResourceMetricResponse GetDefaultResponse(
      DateTime start,
      DateTime end,
      TimeSpan timeGrain,
      ResourceName resourceName)
    {
      ResourceMetricResponse defaultResponse = new ResourceMetricResponse()
      {
        Data = new ResourceMetricSet()
        {
          StartTime = start,
          TimeGrain = timeGrain,
          EndTime = end,
          Unit = this.GetUnit(resourceName),
          Name = resourceName.ToString(),
          DisplayName = resourceName.ToString(),
          Payload = new List<ResourceMetricSample>()
        }
      };
      long num = (long) (int) Math.Ceiling((end - start).TotalMinutes / timeGrain.TotalMinutes);
      for (int index = 0; (long) index < num; ++index)
      {
        DateTime dateTime = start.AddMinutes((double) index * timeGrain.TotalMinutes);
        start.AddMinutes((double) (index + 1) * timeGrain.TotalMinutes);
        defaultResponse.Data.Payload.Add(new ResourceMetricSample()
        {
          Total = new double?(0.0),
          TimeCreated = dateTime
        });
      }
      return defaultResponse;
    }

    public virtual ResourceMetricResponses GetMetricsByCategory(
      string subscriptionId,
      string cloudServiceName,
      string resourceType,
      string resourceName,
      MetricCategory category,
      DateTime startTime,
      DateTime endTime,
      TimeSpan timeGrain)
    {
      List<string> list = MonitoringController.metricCategoryMappings.Keys.Where<string>((Func<string, bool>) (metricType => MonitoringController.metricCategoryMappings[metricType] == category)).ToList<string>();
      return this.GetMetricsByName(subscriptionId, cloudServiceName, resourceType, resourceName, list.ToArray(), startTime, endTime, timeGrain);
    }

    public ResourceMetricResponses GetAllMetrics(
      string subscriptionId,
      string cloudServiceName,
      string resourceType,
      string resourceName,
      DateTime startTime,
      DateTime endTime,
      TimeSpan timeGrain)
    {
      return this.GetMetricsByName(subscriptionId, cloudServiceName, resourceType, resourceName, MonitoringController.metricCategoryMappings.Keys.ToArray<string>(), startTime, endTime, timeGrain);
    }

    [ExcludeFromCodeCoverage]
    internal virtual NameValueCollection GetQueryString() => this.Request.RequestUri.ParseQueryString();

    [HttpGet]
    [ActionName("Metrics")]
    [TraceFilter(5106031, 5106040)]
    [TraceExceptions(5106038)]
    [TraceRequest(5106032)]
    [TraceResponse(5106037)]
    public HttpResponseMessage GetMetrics(
      string subscriptionId,
      string cloudServiceName,
      string resourceType,
      string resourceName,
      string startTime,
      string endTime,
      string timeGrain)
    {
      NameValueCollection queryString = this.GetQueryString();
      string str = queryString["names"];
      string actualValue = queryString["category"];
      DateTime dateTime1 = new DateTime();
      DateTime dateTime2 = new DateTime();
      TimeSpan timeSpan1 = new TimeSpan();
      DateTime startTime1;
      DateTime endTime1;
      TimeSpan timeSpan2;
      try
      {
        startTime1 = DateTime.Parse(startTime);
        endTime1 = DateTime.Parse(endTime);
        timeSpan2 = XmlConvert.ToTimeSpan(timeGrain);
      }
      catch (Exception ex)
      {
        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
          ReasonPhrase = "Unparseable parameter startTime, endTime, or TimeGrain"
        });
      }
      if (!string.IsNullOrEmpty(str))
      {
        string[] metricNames = str.Split(',');
        StringContent stringContent = new StringContent(this.SerializeJson<ResourceMetricResponses>(this.GetMetricsByName(subscriptionId, cloudServiceName, resourceType, resourceName, metricNames, startTime1, endTime1, timeSpan2)), Encoding.UTF8, "application/json");
        return new HttpResponseMessage(HttpStatusCode.OK)
        {
          Content = (HttpContent) stringContent
        };
      }
      if (!string.IsNullOrEmpty(actualValue))
      {
        MetricCategory result;
        if (!System.Enum.TryParse<MetricCategory>(actualValue, out result))
          throw new ArgumentOutOfRangeException("category", (object) actualValue, "Failed to parse enum as type MetricCategory");
        StringContent stringContent = new StringContent(this.SerializeJson<ResourceMetricResponses>(this.GetMetricsByCategory(subscriptionId, cloudServiceName, resourceType, resourceName, result, startTime1, endTime1, timeSpan2)), Encoding.UTF8, "application/json");
        return new HttpResponseMessage(HttpStatusCode.OK)
        {
          Content = (HttpContent) stringContent
        };
      }
      StringContent stringContent1 = new StringContent(this.SerializeJson<ResourceMetricResponses>(this.GetAllMetrics(subscriptionId, cloudServiceName, resourceType, resourceName, startTime1, endTime1, timeSpan2)), Encoding.UTF8, "application/json");
      return new HttpResponseMessage(HttpStatusCode.OK)
      {
        Content = (HttpContent) stringContent1
      };
    }

    [HttpGet]
    [ActionName("MetricDefinitions")]
    [TraceFilter(5106041, 5106050)]
    [TraceExceptions(5106044)]
    [TraceRequest(5106012)]
    [TraceResponse(5106017)]
    public List<ResourceMetricDefinition> GetMetricDefinition() => new List<ResourceMetricDefinition>()
    {
      new ResourceMetricDefinition()
      {
        DisplayName = "Build",
        MetricAvailabilities = (IList<ResourceMetricAvailability>) new List<ResourceMetricAvailability>()
        {
          new ResourceMetricAvailability()
          {
            Retention = new TimeSpan(7, 0, 0, 0),
            TimeGrain = new TimeSpan(0, 1, 0, 0)
          }
        },
        Name = "Build",
        PrimaryAggregationType = "Average",
        Unit = this.GetUnit(ResourceName.Build)
      },
      new ResourceMetricDefinition()
      {
        DisplayName = "LoadTest",
        MetricAvailabilities = (IList<ResourceMetricAvailability>) new List<ResourceMetricAvailability>()
        {
          new ResourceMetricAvailability()
          {
            Retention = new TimeSpan(7, 0, 0, 0),
            TimeGrain = new TimeSpan(0, 1, 0, 0)
          }
        },
        Name = "LoadTest",
        PrimaryAggregationType = "Average",
        Unit = this.GetUnit(ResourceName.LoadTest)
      }
    };

    internal interface IUsageEventStoreInitializer
    {
      IUsageEventsStore InitializeUsageEventStore(IVssRequestContext requestContext);
    }

    internal class UsageEventStoreInitializer : MonitoringController.IUsageEventStoreInitializer
    {
      public IUsageEventsStore InitializeUsageEventStore(IVssRequestContext requestContext)
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        return Activator.CreateInstance(Type.GetType(vssRequestContext.GetService<IVssRegistryService>().GetValue<string>(vssRequestContext, (RegistryQuery) "/Service/Commerce/Metering/UsageEventStorageProvider", string.Empty), true), (object) vssRequestContext) as IUsageEventsStore;
      }
    }

    internal interface IUsageDataAggregatorResolver
    {
      UsageDataAggregator GetUsageDataAggregator(TimeSpan totalSpan, TimeSpan timeGrain);
    }

    internal class UsageDataAggregatorResolver : MonitoringController.IUsageDataAggregatorResolver
    {
      public UsageDataAggregator GetUsageDataAggregator(TimeSpan totalSpan, TimeSpan timeGrain) => totalSpan.TotalDays >= 1.0 ? (UsageDataAggregator) new ParallelUsageDataAggregator(timeGrain) : (UsageDataAggregator) new SingleQueryUsageDataAggregator(timeGrain);
    }
  }
}
