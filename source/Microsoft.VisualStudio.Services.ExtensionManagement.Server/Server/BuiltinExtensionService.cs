// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Server.BuiltinExtensionService
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.Types.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Server
{
  public class BuiltinExtensionService : 
    VssBaseService,
    IBuiltinExtensionService,
    IVssFrameworkService
  {
    private int m_loadedDataVersion;
    private int m_currentDataVersion;
    private int m_refreshInProgress;
    private List<PublishedExtension> m_builtInExtensions;
    private const string s_area = "BuiltinExtensionService";
    private const string s_layer = "Service";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      TeamFoundationSqlNotificationService notificationService = systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) ? systemRequestContext.GetService<TeamFoundationSqlNotificationService>() : throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
      notificationService.RegisterNotification(systemRequestContext, "Default", GallerySdkSqlNotificationClasses.ExtensionChanged, new SqlNotificationCallback(this.OnPublishedExtensionChanged), false);
      notificationService.RegisterNotification(systemRequestContext, "Default", ExtensionManagementSqlNotificationClasses.ForceBuiltinRefresh, new SqlNotificationCallback(this.OnForceFlush), false);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      TeamFoundationSqlNotificationService service = systemRequestContext.GetService<TeamFoundationSqlNotificationService>();
      service.UnregisterNotification(systemRequestContext, "Default", GallerySdkSqlNotificationClasses.ExtensionChanged, new SqlNotificationCallback(this.OnPublishedExtensionChanged), false);
      service.UnregisterNotification(systemRequestContext, "Default", ExtensionManagementSqlNotificationClasses.ForceBuiltinRefresh, new SqlNotificationCallback(this.OnForceFlush), false);
    }

    public List<PublishedExtension> QueryBuiltInExtensions(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(10013578, nameof (BuiltinExtensionService), "Service", nameof (QueryBuiltInExtensions));
      try
      {
        if (this.m_builtInExtensions != null)
        {
          if (this.m_currentDataVersion == this.m_loadedDataVersion)
          {
            requestContext.Trace(10013580, TraceLevel.Info, nameof (BuiltinExtensionService), "Service", "Builtin extensions are up to date.  Returning loaded extensions.");
            return this.m_builtInExtensions;
          }
          if (!requestContext.IsFeatureEnabled(ExtensionManagementFeatureFlags.RefreshBuiltInExtensionIfNeededOnAllRequests))
          {
            if (Interlocked.CompareExchange(ref this.m_refreshInProgress, 1, 0) == 0)
            {
              requestContext.Trace(10013581, TraceLevel.Info, nameof (BuiltinExtensionService), "Service", "Trying to fetch fresh values for Builtin extensions.");
              this.LoadExtensions(requestContext);
            }
            else
              requestContext.Trace(10013570, TraceLevel.Info, nameof (BuiltinExtensionService), "Service", "Returning expired value since refresh is currently in progress");
            return this.m_builtInExtensions;
          }
          requestContext.Trace(10013571, TraceLevel.Info, nameof (BuiltinExtensionService), "Service", "All requests attempting to perform local extension refresh.");
        }
        return this.LoadExtensions(requestContext);
      }
      finally
      {
        requestContext.TraceLeave(10013579, nameof (BuiltinExtensionService), "Service", nameof (QueryBuiltInExtensions));
      }
    }

    private List<PublishedExtension> LoadExtensions(IVssRequestContext requestContext)
    {
      List<PublishedExtension> publishedExtensionList = (List<PublishedExtension>) null;
      requestContext.TraceEnter(10013200, nameof (BuiltinExtensionService), "Service", "QueryBuiltInExtensions");
      try
      {
        int currentDataVersion = this.m_currentDataVersion;
        QueryFilter queryFilter = new QueryFilter()
        {
          Criteria = new List<FilterCriteria>()
        };
        queryFilter.Criteria.Add(new FilterCriteria()
        {
          FilterType = 1,
          Value = "$BuiltIn"
        });
        ExtensionQuery query = new ExtensionQuery()
        {
          Filters = new List<QueryFilter>()
        };
        query.Filters.Add(queryFilter);
        query.Flags = ExtensionQueryFlags.IncludeVersions | ExtensionQueryFlags.ExcludeNonValidated | ExtensionQueryFlags.IncludeLatestVersionOnly;
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
        ExtensionQueryResult extensionQueryResult = vssRequestContext.GetService<IGalleryService>().QueryExtensions(vssRequestContext, query);
        if (extensionQueryResult != null && extensionQueryResult.Results.Count == 1)
        {
          publishedExtensionList = extensionQueryResult.Results[0].Extensions;
          for (int index = 0; index < publishedExtensionList.Count; ++index)
          {
            if (!publishedExtensionList[index].Flags.HasFlag((Enum) PublishedExtensionFlags.Validated))
            {
              requestContext.Trace(10013142, TraceLevel.Info, nameof (BuiltinExtensionService), "Service", "Extension has not been validated: {0}.{1}", (object) publishedExtensionList[index].Publisher.PublisherName, (object) publishedExtensionList[index].ExtensionName);
              publishedExtensionList.RemoveAt(index--);
            }
          }
        }
        else
          publishedExtensionList = new List<PublishedExtension>();
        if (currentDataVersion == this.m_currentDataVersion)
        {
          this.m_loadedDataVersion = this.m_currentDataVersion;
          this.m_builtInExtensions = publishedExtensionList;
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10013572, TraceLevel.Error, nameof (BuiltinExtensionService), "Service", ex);
        throw;
      }
      finally
      {
        this.m_refreshInProgress = 0;
        requestContext.TraceLeave(1001357, nameof (BuiltinExtensionService), "Service", "QueryBuiltInExtensions");
      }
      return publishedExtensionList;
    }

    private void OnPublishedExtensionChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      try
      {
        ExtensionChangeNotification changeNotification = TeamFoundationSerializationUtility.Deserialize<ExtensionChangeNotification>(eventData);
        requestContext.Trace(10013572, TraceLevel.Info, nameof (BuiltinExtensionService), "Service", "Builtin extension: Extension Update received for {0}.{1}", (object) changeNotification.PublisherName, (object) changeNotification.ExtensionName);
        if (changeNotification.EventType == ExtensionEventType.ExtensionShared || (changeNotification.Flags & 2) != 2)
          return;
        requestContext.Trace(10013573, TraceLevel.Info, nameof (BuiltinExtensionService), "Service", "Builtin extension has been updated.  Refresh extensions.");
        this.RefreshExtensions(requestContext);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10013285, nameof (BuiltinExtensionService), "Service", ex);
      }
    }

    private void OnForceFlush(IVssRequestContext requestContext, Guid eventClass, string eventData) => this.RefreshExtensions(requestContext);

    private void RefreshExtensions(IVssRequestContext requestContext)
    {
      Interlocked.Increment(ref this.m_currentDataVersion);
      this.QueryBuiltInExtensions(requestContext);
    }
  }
}
