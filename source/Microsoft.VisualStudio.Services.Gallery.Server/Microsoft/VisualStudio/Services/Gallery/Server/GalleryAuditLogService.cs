// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.GalleryAuditLogService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.Server.Components;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public class GalleryAuditLogService : IGalleryAuditLogService, IVssFrameworkService
  {
    private const string layer = "GalleryAuditLogService";
    private BatchExecutionHandler<AuditLogEntry> _batchExecutionHandler;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      IVssRegistryService service = systemRequestContext.GetService<IVssRegistryService>();
      int valueFromPath = service.ReadEntries(systemRequestContext, (RegistryQuery) "/Configuration/Service/Gallery/BatchSize/AuditLog").GetValueFromPath<int>("/Configuration/Service/Gallery/BatchSize/AuditLog", 100);
      this.InitBatchHandler(systemRequestContext, valueFromPath);
      service.RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.ConfigurationChangeCallback), false, "/Configuration/Service/Gallery/BatchSize/AuditLog");
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      this._batchExecutionHandler.Flush(systemRequestContext);
      this._batchExecutionHandler = (BatchExecutionHandler<AuditLogEntry>) null;
      systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.ConfigurationChangeCallback));
    }

    public void LogAuditEntry(
      IVssRequestContext requestContext,
      string auditAction,
      string resourceId,
      string resourceType,
      string data = null,
      bool batchRequests = false)
    {
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
        return;
      Guid changedBy = Guid.Empty;
      if (!requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.StopLoggingVsidForAuditLog"))
        changedBy = this.GetVsid(requestContext);
      try
      {
        if (batchRequests && requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableBatchingForAuditLog"))
        {
          this._batchExecutionHandler.Add(requestContext, new AuditLogEntry()
          {
            ChangedByIdentity = changedBy,
            AuditAction = auditAction,
            ActionDate = DateTime.Now,
            ResourceId = resourceId,
            ResourceType = resourceType,
            Data = data
          });
        }
        else
        {
          using (GalleryAuditLogComponent component = requestContext.CreateComponent<GalleryAuditLogComponent>())
            component.LogAuditEntry(changedBy, auditAction, resourceId, resourceType, data);
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12061038, "gallery", nameof (GalleryAuditLogService), ex);
      }
    }

    public virtual void CleanUpAuditLog(IVssRequestContext requestContext, int noOfDays)
    {
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
        return;
      try
      {
        using (GalleryAuditLogComponent component = requestContext.CreateComponent<GalleryAuditLogComponent>())
          component.CleanUpAuditLog(noOfDays);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12061038, "gallery", nameof (GalleryAuditLogService), ex);
        throw;
      }
    }

    private Guid GetVsid(IVssRequestContext requestContext)
    {
      try
      {
        return requestContext.GetUserId();
      }
      catch (Exception ex)
      {
        return Guid.Empty;
      }
    }

    private void ConfigurationChangeCallback(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      int valueFromPath = changedEntries.GetValueFromPath<int>("/Configuration/Service/Gallery/BatchSize/AuditLog", 100);
      this.InitBatchHandler(requestContext, valueFromPath);
    }

    private void InitBatchHandler(IVssRequestContext requestContext, int batchSize)
    {
      if (this._batchExecutionHandler != null)
      {
        this._batchExecutionHandler.Flush(requestContext);
        this._batchExecutionHandler = (BatchExecutionHandler<AuditLogEntry>) null;
      }
      this._batchExecutionHandler = new BatchExecutionHandler<AuditLogEntry>(batchSize, new Func<IVssRequestContext, List<AuditLogEntry>, bool>(this.BatchProcessor));
    }

    internal virtual bool BatchProcessor(
      IVssRequestContext requestContext,
      List<AuditLogEntry> auditLogEntries)
    {
      try
      {
        using (GalleryAuditLogComponent component = requestContext.CreateComponent<GalleryAuditLogComponent>())
        {
          if (component is GalleryAuditLogComponent3 auditLogComponent3)
            auditLogComponent3.LogAuditEntriesBatch(auditLogEntries);
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12061038, "gallery", nameof (GalleryAuditLogService), ex);
        throw;
      }
      return true;
    }
  }
}
