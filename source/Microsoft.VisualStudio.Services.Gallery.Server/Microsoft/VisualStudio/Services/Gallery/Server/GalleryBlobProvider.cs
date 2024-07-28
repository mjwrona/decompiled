// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.GalleryBlobProvider
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  internal class GalleryBlobProvider
  {
    private IBlobProvider m_blobProvider;
    private string m_drawerName;
    private string m_lookupKey;
    private const string s_layer = "GalleryBlobProvider";
    private const string c_BlobStorageConnectionStringOverride = "BlobStorageConnectionStringOverride";
    private const string c_BlobStorageDrawerNameSetting = "DrawerName";
    private const string c_BlobStorageLookupKeySetting = "LookupKey";
    private const string c_blobStoreProviderType = "Microsoft.VisualStudio.Services.Cloud.AzureBlobGeoRedundancy.AzureBlobGeoRedundancyProvider";

    public GalleryBlobProvider(
      IVssRequestContext requestContext,
      string drawerName,
      string lookupKey)
    {
      this.m_drawerName = drawerName;
      this.m_lookupKey = lookupKey;
      requestContext.GetService<ITeamFoundationStrongBoxService>().RegisterNotification(requestContext, new StrongBoxItemChangedCallback(this.OnStrongBoxItemChanged), this.m_drawerName, (IEnumerable<string>) new string[1]
      {
        this.m_lookupKey
      });
      this.InitializeBlobProvider(requestContext);
    }

    public bool IsValidBlobProvider() => this.m_blobProvider != null;

    public bool PutStream(
      IVssRequestContext requestContext,
      string containerName,
      string resourceId,
      Stream stream,
      IDictionary<string, string> metadata)
    {
      requestContext.TraceEnter(12061118, "gallery", nameof (GalleryBlobProvider), nameof (PutStream));
      bool flag = false;
      if (this.m_blobProvider != null)
      {
        this.m_blobProvider.PutStream(requestContext, containerName, resourceId, stream, metadata);
        flag = true;
      }
      else
        requestContext.Trace(12061118, TraceLevel.Info, "gallery", nameof (GalleryBlobProvider), "No Put Stream: No blob provider");
      requestContext.TraceLeave(12061118, "gallery", nameof (GalleryBlobProvider), nameof (PutStream));
      return flag;
    }

    public bool DeleteBlob(
      IVssRequestContext requestContext,
      string containerName,
      string resourceId)
    {
      requestContext.TraceEnter(12061118, "gallery", nameof (GalleryBlobProvider), nameof (DeleteBlob));
      bool flag = false;
      if (this.m_blobProvider != null)
        flag = this.m_blobProvider.DeleteBlob(requestContext, containerName, resourceId);
      else
        requestContext.Trace(12061118, TraceLevel.Info, "gallery", nameof (GalleryBlobProvider), "No Delete Blob: No blob provider");
      requestContext.TraceLeave(12061118, "gallery", nameof (GalleryBlobProvider), nameof (DeleteBlob));
      return flag;
    }

    public bool BlobExists(
      IVssRequestContext requestContext,
      string containerName,
      string resourceId)
    {
      requestContext.TraceEnter(12061118, "gallery", nameof (GalleryBlobProvider), nameof (BlobExists));
      bool flag = false;
      if (this.m_blobProvider == null)
        requestContext.Trace(12061118, TraceLevel.Info, "gallery", nameof (GalleryBlobProvider), "No blob provider");
      else
        flag = this.m_blobProvider.BlobExists(requestContext, containerName, resourceId);
      requestContext.TraceLeave(12061118, "gallery", nameof (GalleryBlobProvider), nameof (BlobExists));
      return flag;
    }

    public bool SetBlobHeaders(
      IVssRequestContext requestContext,
      string containerId,
      string resourceId,
      string cacheControl,
      string contentType,
      string contentDisposition,
      string contentEncoding,
      string contentLanguage)
    {
      requestContext.TraceEnter(12061118, "gallery", nameof (GalleryBlobProvider), nameof (SetBlobHeaders));
      bool flag = false;
      if (this.m_blobProvider != null)
      {
        this.m_blobProvider.SetBlobHeaders(requestContext, containerId, resourceId, cacheControl, contentType, contentDisposition, contentEncoding, contentLanguage);
        flag = true;
      }
      else
        requestContext.Trace(12061118, TraceLevel.Info, "gallery", nameof (GalleryBlobProvider), "No Set Blob Headers: No blob provider");
      requestContext.TraceLeave(12061118, "gallery", nameof (GalleryBlobProvider), nameof (SetBlobHeaders));
      return flag;
    }

    public void Unload(IVssRequestContext requestContext) => requestContext.GetService<ITeamFoundationStrongBoxService>().UnregisterNotification(requestContext, new StrongBoxItemChangedCallback(this.OnStrongBoxItemChanged));

    private void OnStrongBoxItemChanged(
      IVssRequestContext requestContext,
      IEnumerable<StrongBoxItemName> itemNames)
    {
      if (!itemNames.Any<StrongBoxItemName>((Func<StrongBoxItemName, bool>) (i => !i.LookupKey.EndsWith("-previous", StringComparison.OrdinalIgnoreCase))))
        return;
      this.InitializeBlobProvider(requestContext);
    }

    private void InitializeBlobProvider(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(12061118, "gallery", nameof (GalleryBlobProvider), nameof (InitializeBlobProvider));
      this.m_blobProvider = (IBlobProvider) null;
      ITeamFoundationStrongBoxService service = requestContext.GetService<ITeamFoundationStrongBoxService>();
      StrongBoxItemInfo itemInfo = service.GetItemInfo(requestContext, this.m_drawerName, this.m_lookupKey, false);
      if (itemInfo == null)
      {
        requestContext.Trace(12061118, TraceLevel.Info, "gallery", nameof (InitializeBlobProvider), "No strongbox item info found.");
      }
      else
      {
        string connectionString = service.GetString(requestContext, itemInfo);
        if (string.IsNullOrWhiteSpace(connectionString))
        {
          requestContext.Trace(12061118, TraceLevel.Info, "gallery", nameof (InitializeBlobProvider), "No connection string found.");
        }
        else
        {
          this.m_blobProvider = this.GetBlobProvider(requestContext, connectionString);
          if (this.m_blobProvider == null)
          {
            InvalidOperationException operationException = new InvalidOperationException("No blob provider found.");
            requestContext.TraceException(12061118, "gallery", nameof (InitializeBlobProvider), (Exception) operationException);
            throw operationException;
          }
        }
      }
      requestContext.TraceLeave(12061118, "gallery", nameof (GalleryBlobProvider), nameof (InitializeBlobProvider));
    }

    public IBlobProvider GetBlobProvider(IVssRequestContext requestContext, string connectionString)
    {
      IBlobProvider blobProvider1 = (IBlobProvider) null;
      if (!string.IsNullOrWhiteSpace(connectionString))
      {
        string blobStoreProviderType = requestContext.GetService<IVssRegistryService>().GetValue(requestContext, (RegistryQuery) "/Configuration/Service/Gallery/BlobProvider", "Microsoft.VisualStudio.Services.Cloud.AzureBlobGeoRedundancy.AzureBlobGeoRedundancyProvider");
        blobProvider2 = (IBlobProvider) null;
        Type type = Type.GetType(blobStoreProviderType);
        if (type != (Type) null)
        {
          if (!(Activator.CreateInstance(type) is IBlobProvider blobProvider2))
            requestContext.Trace(14449, TraceLevel.Warning, "gallery", nameof (GalleryBlobProvider), "Type '" + blobStoreProviderType + "' does not implement IBlobProvider");
        }
        else
          requestContext.Trace(14448, TraceLevel.Warning, "gallery", nameof (GalleryBlobProvider), "Could not find type: " + blobStoreProviderType);
        if (blobProvider2 == null)
          blobProvider2 = requestContext.GetExtension<IBlobProvider>((Func<IBlobProvider, bool>) (x => x.GetType().FullName.Equals(blobStoreProviderType, StringComparison.Ordinal)));
        if (blobProvider2 != null)
        {
          blobProvider2.ServiceStart(requestContext, (IDictionary<string, string>) new Dictionary<string, string>()
          {
            ["BlobStorageConnectionStringOverride"] = connectionString,
            ["DrawerName"] = this.m_drawerName,
            ["LookupKey"] = this.m_lookupKey
          });
          blobProvider1 = blobProvider2;
        }
      }
      return blobProvider1;
    }
  }
}
