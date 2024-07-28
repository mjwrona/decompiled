// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.LogStoreGeoRedundancy
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud.AzureBlobGeoRedundancy;
using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class LogStoreGeoRedundancy : ILogStoreGeoRedundancy
  {
    private IAzureBlobGeoRedundancyService _azureBlobGeoRedundancyService;
    private BlobGeoRedundancyEndpoint _blobGeoRedundancyEndPoint;

    public LogStoreGeoRedundancy(
      IVssRequestContext requestContext,
      ILogStoreConnectionEndpoint primaryConnectionEndpoint)
    {
      this._azureBlobGeoRedundancyService = requestContext.To(TeamFoundationHostType.Deployment).GetService<IAzureBlobGeoRedundancyService>();
      this._blobGeoRedundancyEndPoint = this._azureBlobGeoRedundancyService.SetupEndpoint(requestContext, primaryConnectionEndpoint.GetCloudStorageAccount());
    }

    public void RecordCreateBlob(
      IVssRequestContext requestContext,
      string containerName,
      string blobPath)
    {
      this._azureBlobGeoRedundancyService?.CreateBlob(requestContext, this._blobGeoRedundancyEndPoint, containerName, blobPath);
    }

    public void RecordCreateContainer(IVssRequestContext requestContext, string containerName) => this._azureBlobGeoRedundancyService?.CreateContainer(requestContext, this._blobGeoRedundancyEndPoint, containerName);

    public void RecordDeleteContainer(IVssRequestContext requestContext, string containerName) => this._azureBlobGeoRedundancyService?.DeleteContainer(requestContext, this._blobGeoRedundancyEndPoint, containerName);

    public void RecordDeleteBlob(
      IVssRequestContext requestContext,
      string containerName,
      string blobPath)
    {
      this._azureBlobGeoRedundancyService?.DeleteBlob(requestContext, this._blobGeoRedundancyEndPoint, containerName, blobPath);
    }
  }
}
