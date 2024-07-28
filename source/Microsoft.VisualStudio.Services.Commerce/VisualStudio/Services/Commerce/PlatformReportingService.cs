// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.PlatformReportingService
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.Azure.Cosmos.Table;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class PlatformReportingService : IReportingService, IVssFrameworkService
  {
    private const string Area = "Commerce";
    private const string Layer = "PlatformReportingService";
    private const string CommerceStorageKey = "CommerceEventsStoreConnectionString";
    private static readonly RegistryQuery reportingEventStoreRegistryQuery = (RegistryQuery) "/Service/Commerce/Reporting/EventStore/*";
    private PlatformReportingService.ServiceSettings serviceSettings;
    private AzureReportingEventStore commerceReportingEventStore;

    public PlatformReportingService()
    {
    }

    internal PlatformReportingService(
      AzureReportingEventStore commerceReportingEventStore)
    {
      this.commerceReportingEventStore = commerceReportingEventStore;
    }

    public void ServiceStart(IVssRequestContext requestContext)
    {
      requestContext.CheckDeploymentRequestContext();
      requestContext.GetService<IVssRegistryService>().RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistrySettingsChanged), in PlatformReportingService.reportingEventStoreRegistryQuery);
      Interlocked.CompareExchange<PlatformReportingService.ServiceSettings>(ref this.serviceSettings, new PlatformReportingService.ServiceSettings(requestContext), (PlatformReportingService.ServiceSettings) null);
      if (this.commerceReportingEventStore != null)
        return;
      using (IDisposableReadOnlyList<IReportingView> extensions = requestContext.GetExtensions<IReportingView>())
      {
        AzureReportingEventStoreContext context = new AzureReportingEventStoreContext()
        {
          ConnectionStringDrawer = FrameworkServerConstants.ConfigurationSecretsDrawerName,
          StorageKey = "CommerceEventsStoreConnectionString",
          EventProcessingDelay = this.serviceSettings.EventProcessingDelay
        };
        this.commerceReportingEventStore = new AzureReportingEventStore(requestContext, (IEnumerable<string>) extensions.Select<IReportingView, string>((Func<IReportingView, string>) (e => e.TableName)).ToList<string>(), context);
      }
    }

    public void ServiceEnd(IVssRequestContext requestContext)
    {
      requestContext.GetService<IVssRegistryService>().UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistrySettingsChanged));
      this.commerceReportingEventStore?.Cleanup(requestContext);
    }

    public void SaveReportingEvent(IVssRequestContext requestContext, ReportingEvent reportingEvent)
    {
      requestContext.CheckDeploymentRequestContext();
      try
      {
        requestContext.TraceEnter(5108960, "Commerce", nameof (PlatformReportingService), new object[1]
        {
          (object) reportingEvent
        }, nameof (SaveReportingEvent));
        this.commerceReportingEventStore.SaveUnprocessedEvent(requestContext, reportingEvent);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5108961, "Commerce", nameof (PlatformReportingService), ex);
      }
      finally
      {
        requestContext.TraceLeave(5108969, "Commerce", nameof (PlatformReportingService), nameof (SaveReportingEvent));
      }
    }

    public void PublishReportingEvents(IVssRequestContext requestContext, string viewName)
    {
      requestContext.CheckDeploymentRequestContext();
      try
      {
        requestContext.TraceEnter(5108970, "Commerce", nameof (PlatformReportingService), new object[1]
        {
          (object) viewName
        }, nameof (PublishReportingEvents));
        this.GetReportingView(requestContext, viewName).ProcessEvents(requestContext, this.commerceReportingEventStore);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5108971, "Commerce", nameof (PlatformReportingService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5108979, "Commerce", nameof (PlatformReportingService), nameof (PublishReportingEvents));
      }
    }

    public IEnumerable<T> GetReportingEvents<T>(
      IVssRequestContext requestContext,
      string viewName,
      string resourceName,
      DateTime startDateInclusive,
      DateTime endDateInclusive,
      string filter)
      where T : ITableEntity, new()
    {
      requestContext.CheckDeploymentRequestContext();
      try
      {
        requestContext.TraceEnter(5108980, "Commerce", nameof (PlatformReportingService), new object[4]
        {
          (object) viewName,
          (object) resourceName,
          (object) startDateInclusive,
          (object) endDateInclusive
        }, nameof (GetReportingEvents));
        return this.GetReportingView(requestContext, viewName).GetProcessedEvents<T>(requestContext, this.commerceReportingEventStore, resourceName, startDateInclusive, endDateInclusive, filter);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5108981, "Commerce", nameof (PlatformReportingService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5108989, "Commerce", nameof (PlatformReportingService), nameof (GetReportingEvents));
      }
    }

    internal virtual IReportingView GetReportingView(
      IVssRequestContext requestContext,
      string viewName)
    {
      using (IDisposableReadOnlyList<IReportingView> extensions = requestContext.GetExtensions<IReportingView>())
        return extensions.SingleOrDefault<IReportingView>((Func<IReportingView, bool>) (e => StringComparer.OrdinalIgnoreCase.Equals(e.ViewName, viewName))) ?? throw new ReportingViewNotSupportedException("The view type " + viewName + " is not supported.");
    }

    internal void OnRegistrySettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      Volatile.Write<PlatformReportingService.ServiceSettings>(ref this.serviceSettings, new PlatformReportingService.ServiceSettings(requestContext));
      this.commerceReportingEventStore.EventProcessingDelay = this.serviceSettings.EventProcessingDelay;
    }

    private class ServiceSettings
    {
      private const int DefaultEventProcessingDelay = 5;

      public ServiceSettings(IVssRequestContext requestContext) => this.EventProcessingDelay = requestContext.GetService<IVssRegistryService>().ReadEntries(requestContext, PlatformReportingService.reportingEventStoreRegistryQuery).GetValueFromPath<int>(nameof (EventProcessingDelay), 5);

      public int EventProcessingDelay { get; private set; }
    }
  }
}
