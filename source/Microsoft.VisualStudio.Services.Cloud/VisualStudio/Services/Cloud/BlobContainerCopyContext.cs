// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.BlobContainerCopyContext
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.Storage.Blob;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud.BlobWrappers;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class BlobContainerCopyContext : IBlobCopyContext
  {
    private BlockingCollection<BlobCopyRequest> m_blockingCollection;
    private CancellationToken m_cancellationToken;
    private ICloudBlobContainerWrapper m_sourceContainer;
    private ICloudBlobContainerWrapper m_targetContainer;
    private bool m_failIfUnexpectedBlobsOnTarget;

    public BlobContainerCopyContext(
      ICloudBlobContainerWrapper sourceContainer,
      ICloudBlobContainerWrapper targetContainer,
      BlockingCollection<BlobCopyRequest> blockingCollection,
      bool failIfUnexpectedBlobsOnTarget,
      CancellationToken cancellationToken)
    {
      this.m_sourceContainer = sourceContainer;
      this.m_targetContainer = targetContainer;
      this.m_blockingCollection = blockingCollection;
      this.m_failIfUnexpectedBlobsOnTarget = failIfUnexpectedBlobsOnTarget;
      this.m_cancellationToken = cancellationToken;
    }

    public int Compare(
      IVssRequestContext requestContext,
      ICloudBlobReadOnlyInfo sourceBlob,
      ICloudBlobReadOnlyInfo targetBlob)
    {
      return string.Compare(sourceBlob.Name, targetBlob.Name, StringComparison.Ordinal);
    }

    public void OnSourceAndTarget(
      IVssRequestContext requestContext,
      ICloudBlobWrapper sourceBlob,
      ICloudBlobWrapper targetBlob)
    {
      if (this.m_failIfUnexpectedBlobsOnTarget && (sourceBlob.ContentMD5 == null || targetBlob.ContentMD5 == null || !string.Equals(sourceBlob.ContentMD5, targetBlob.ContentMD5, StringComparison.Ordinal)))
        throw new DataMigrationBlobMismatchException(HostingResources.DataMigrationTargetBlobNotMatchSourceBlobError((object) sourceBlob.Name, (object) sourceBlob.ContainerName));
    }

    public void OnSourceOnly(IVssRequestContext requestContext, ICloudBlobWrapper sourceBlob) => this.m_blockingCollection.Add(this.CreateCopyRequest(this.m_sourceContainer, sourceBlob), this.m_cancellationToken);

    public void OnTargetOnly(IVssRequestContext requestContext, ICloudBlobWrapper targetBlob)
    {
      if (this.m_failIfUnexpectedBlobsOnTarget)
        throw new DataMigrationBlobMismatchException(HostingResources.DataMigrationTargetExtraBlobError((object) targetBlob.Name, (object) targetBlob.ContainerName));
    }

    public void StartIteration(
      IVssRequestContext requestContext,
      ICloudBlobContainerWrapper sourceContainer,
      ICloudBlobContainerWrapper targetContainer,
      ICloudBlobWrapper sourceBlob,
      ICloudBlobWrapper targetBlob)
    {
    }

    private BlobCopyRequest CreateCopyRequest(
      ICloudBlobContainerWrapper sourceContainer,
      ICloudBlobWrapper sourceBlob)
    {
      ICloudBlobWrapper targetBlob = sourceBlob.BlobType == BlobType.BlockBlob ? this.m_targetContainer.GetBlockBlobReference(sourceBlob.Name) : this.m_targetContainer.GetPageBlobReference(sourceBlob.Name, sourceBlob.GetLength());
      return new BlobCopyRequest(sourceContainer, sourceBlob, targetBlob);
    }
  }
}
