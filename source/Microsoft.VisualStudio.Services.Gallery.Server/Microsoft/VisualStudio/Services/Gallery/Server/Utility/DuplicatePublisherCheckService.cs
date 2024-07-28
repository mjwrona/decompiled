// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Utility.DuplicatePublisherCheckService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.Server.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Utility
{
  public class DuplicatePublisherCheckService : IDuplicatePublisherCheckService, IVssFrameworkService
  {
    private const string ServiceLayer = "DuplicatePublisherCheckService";
    private DuplicatePublisherCheckCache _duplicatePublisherCheckCache;

    public DuplicatePublisherCheckService() => this._duplicatePublisherCheckCache = new DuplicatePublisherCheckCache();

    internal DuplicatePublisherCheckService(
      DuplicatePublisherCheckCache duplicatePublisherCheckCache)
    {
      this._duplicatePublisherCheckCache = duplicatePublisherCheckCache;
    }

    public void ServiceStart(IVssRequestContext systemRequestContext) => systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().RegisterNotification(systemRequestContext, "Default", GalleryNotificationEventIds.PublisherUpdateDelete, new SqlNotificationCallback(this.PublisherChangeCallback), false);

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    private void ConfigurationChangeCallback(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
    }

    private void PublisherChangeCallback(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      if (!requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnablePublisherDisplayNameCachePopulation"))
        return;
      if (!string.IsNullOrEmpty(eventData))
      {
        string[] strArray1 = eventData.Split(':');
        if (strArray1.Length > 1 && !string.IsNullOrWhiteSpace(strArray1[0]) && !string.IsNullOrWhiteSpace(strArray1[1]))
        {
          string a = strArray1[0];
          if (string.Equals(a, "created"))
            this._duplicatePublisherCheckCache.AddRemovePublishersName(requestContext, (IReadOnlyList<string>) new List<string>()
            {
              strArray1[1]
            }, false, true);
          if (string.Equals(a, "updated"))
          {
            string[] strArray2 = strArray1[1].Split(';');
            if (strArray2.Length > 1)
            {
              string str1 = strArray2[0];
              string str2 = strArray2[1];
              if (!string.IsNullOrWhiteSpace(str1) && !string.IsNullOrWhiteSpace(str2))
              {
                this._duplicatePublisherCheckCache.AddRemovePublishersName(requestContext, (IReadOnlyList<string>) new List<string>()
                {
                  str1
                }, true, true);
                this._duplicatePublisherCheckCache.AddRemovePublishersName(requestContext, (IReadOnlyList<string>) new List<string>()
                {
                  str2
                }, false, true);
              }
            }
          }
          if (string.Equals(a, "deleted"))
            this._duplicatePublisherCheckCache.AddRemovePublishersName(requestContext, (IReadOnlyList<string>) new List<string>()
            {
              strArray1[1]
            }, true, true);
        }
      }
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("eventType", "DuplicatePublisherCheckCacheUpdate");
      properties.Add(nameof (eventData), eventData);
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Microsoft.VisualStudio.Services.Gallery", "DuplicatePublisherDisplayNameCheck", properties);
    }

    public bool DoesPublisherDisplayNameExists(
      IVssRequestContext requestContext,
      string publisherDisplayName)
    {
      try
      {
        if (!string.IsNullOrWhiteSpace(publisherDisplayName))
          return this._duplicatePublisherCheckCache.IsDuplicatePublisherDisplayName(requestContext, publisherDisplayName.ToUpper());
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12062105, "gallery", nameof (DuplicatePublisherCheckService), ex);
      }
      return false;
    }

    public void PopulatePublisherDisplayNames(IVssRequestContext requestContext)
    {
      IReadOnlyList<string> list = (IReadOnlyList<string>) new List<string>();
      using (PublisherComponent component = requestContext.CreateComponent<PublisherComponent>())
      {
        if (component is PublisherComponent10)
          list = (component as PublisherComponent10).FetchAllPublisherDisplayNames();
      }
      this.SubmitInChunksOfSize(requestContext, list, 100);
    }

    private void SubmitInChunksOfSize(
      IVssRequestContext requestContext,
      IReadOnlyList<string> list,
      int chunkSize)
    {
      for (int count = 0; count < list.Count; count += chunkSize)
      {
        IEnumerable<string> source = list.Skip<string>(count).Take<string>(chunkSize);
        if (this._duplicatePublisherCheckCache != null)
        {
          try
          {
            if (!this._duplicatePublisherCheckCache.AddRemovePublishersName(requestContext, (IReadOnlyList<string>) source.ToList<string>(), false))
              requestContext.TraceAlways(12062105, TraceLevel.Error, "gallery", nameof (DuplicatePublisherCheckService), "Failed to Publish chunk number {0}", (object) (count / chunkSize));
          }
          catch (Exception ex)
          {
            requestContext.TraceAlways(12062105, TraceLevel.Error, "gallery", nameof (DuplicatePublisherCheckService), "Failed to Publish chunk number {0}", (object) (count / chunkSize));
            requestContext.TraceException(12062105, "gallery", nameof (DuplicatePublisherCheckService), ex);
          }
        }
      }
    }
  }
}
