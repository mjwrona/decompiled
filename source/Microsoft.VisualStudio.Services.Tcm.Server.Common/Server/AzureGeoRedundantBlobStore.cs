// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.AzureGeoRedundantBlobStore
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class AzureGeoRedundantBlobStore : AzureBlobStore
  {
    private ILogStoreConnectionEndpoint _logStorePrimaryConnectionEndpoint;
    private ILogStoreGeoRedundancy logStoreGeoRedundancy;
    private string _containerName;

    public AzureGeoRedundantBlobStore(
      IVssRequestContext requestContext,
      ILogStoreConnectionEndpoint logStoreConnectionEndpoint,
      string containerName,
      ILogStoreContainerAccessPolicy containerAccessPolicy,
      ILogStoreRequestOption logStoreRequestOption)
      : base(requestContext, logStoreConnectionEndpoint, containerName, containerAccessPolicy, logStoreRequestOption)
    {
      this._logStorePrimaryConnectionEndpoint = logStoreConnectionEndpoint;
      this._containerName = containerName;
      this.InitializeGeoRedundantService(requestContext);
    }

    public override async Task<bool> CreateContainerIfNotExistsAsync(
      IVssRequestContext requestContext,
      ILogStoreOperationContext logStoreOperationContext,
      CancellationToken cancellationToken)
    {
      try
      {
        this.logStoreGeoRedundancy?.RecordCreateContainer(requestContext, this._containerName);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1015795, "TestManagement", "LogStorage", ex);
      }
      return await base.CreateContainerIfNotExistsAsync(requestContext, logStoreOperationContext, cancellationToken).ConfigureAwait(false);
    }

    public override bool DeleteContainer(
      IVssRequestContext requestContext,
      IContainerAccessCondition containerAccessCondition,
      ILogStoreOperationContext logStoreOperationContext)
    {
      try
      {
        this.logStoreGeoRedundancy?.RecordDeleteContainer(requestContext, this._containerName);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1015795, "TestManagement", "LogStorage", ex);
      }
      return base.DeleteContainer(requestContext, containerAccessCondition, logStoreOperationContext);
    }

    public override async Task<bool> DeleteBlobAsync(
      IVssRequestContext requestContext,
      ILogStoreOperationContext logStoreOperationContext,
      string filePath,
      CancellationToken cancellationToken)
    {
      this.RecordDeleteBlob(requestContext, filePath);
      return await base.DeleteBlobAsync(requestContext, logStoreOperationContext, filePath, cancellationToken).ConfigureAwait(false);
    }

    public override bool CreateBlob(
      IVssRequestContext requestContext,
      string filePath,
      Stream stream,
      IDictionary<string, string> metaData = null,
      bool overwrite = false)
    {
      this.RecordCreateBlob(requestContext, filePath);
      return base.CreateBlob(requestContext, filePath, stream, metaData, overwrite);
    }

    public override async Task<bool> CreateBlobAsync(
      IVssRequestContext requestContext,
      ILogStoreOperationContext logStoreOperationContext,
      string filePath,
      Stream stream,
      CancellationToken cancellationToken,
      IDictionary<string, string> metaData = null,
      bool overwrite = false)
    {
      this.RecordCreateBlob(requestContext, filePath);
      return await base.CreateBlobAsync(requestContext, logStoreOperationContext, filePath, stream, cancellationToken, metaData, overwrite).ConfigureAwait(false);
    }

    private void RecordCreateBlob(IVssRequestContext requestContext, string filePath)
    {
      try
      {
        this.logStoreGeoRedundancy?.RecordCreateBlob(requestContext, this._containerName, filePath);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1015795, "TestManagement", "LogStorage", ex);
      }
    }

    private void RecordDeleteBlob(IVssRequestContext requestContext, string filePath)
    {
      try
      {
        this.logStoreGeoRedundancy?.RecordDeleteBlob(requestContext, this._containerName, filePath);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1015795, "TestManagement", "LogStorage", ex);
      }
    }

    private void InitializeGeoRedundantService(IVssRequestContext requestContext)
    {
      try
      {
        if (!requestContext.IsFeatureEnabled("TestManagement.Server.TestLogStoreGeoReplicationEnabled"))
          return;
        this.logStoreGeoRedundancy = (ILogStoreGeoRedundancy) new LogStoreGeoRedundancy(requestContext, this._logStorePrimaryConnectionEndpoint);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1015795, "TestManagement", "LogStorage", ex);
      }
    }
  }
}
