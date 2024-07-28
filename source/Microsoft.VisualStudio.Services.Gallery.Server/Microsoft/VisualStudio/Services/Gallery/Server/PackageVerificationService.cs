// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.PackageVerificationService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.Azure.Storage;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public abstract class PackageVerificationService
  {
    protected Uri m_blobStoreBaseURL;

    protected void LoadBlobStorageBaseUrl(
      IVssRequestContext requestContext,
      int tracePoint,
      string layer)
    {
      using (requestContext.TraceBlock(tracePoint, tracePoint, "gallery", layer, nameof (LoadBlobStorageBaseUrl)))
      {
        this.m_blobStoreBaseURL = (Uri) null;
        ITeamFoundationStrongBoxService service = requestContext.GetService<ITeamFoundationStrongBoxService>();
        StrongBoxItemInfo itemInfo = service.GetItemInfo(requestContext, "ConfigurationSecrets", "GalleryPVStorageConnectionString", false);
        if (itemInfo == null)
        {
          requestContext.Trace(tracePoint, TraceLevel.Info, "gallery", nameof (LoadBlobStorageBaseUrl), "No strongbox item info found.");
        }
        else
        {
          string connectionString = service.GetString(requestContext, itemInfo);
          if (string.IsNullOrWhiteSpace(connectionString))
          {
            requestContext.Trace(tracePoint, TraceLevel.Info, "gallery", nameof (LoadBlobStorageBaseUrl), "No connection string found.");
          }
          else
          {
            this.m_blobStoreBaseURL = CloudStorageAccount.Parse(connectionString).BlobStorageUri.PrimaryUri;
            requestContext.Trace(tracePoint, TraceLevel.Info, "gallery", nameof (LoadBlobStorageBaseUrl), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Blob store base URL: {0}", (object) this.m_blobStoreBaseURL));
          }
        }
      }
    }
  }
}
