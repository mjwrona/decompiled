// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.FileImportBlobProvider
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.Storage.Blob;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class FileImportBlobProvider : IFileImportBlobProvider, IDisposable
  {
    private const int c_blockSize = 5242880;
    private readonly IAzureBlobProvider m_blobProvider;
    private readonly CloudBlobContainer m_blobContainer;
    private readonly IVssRequestContext m_deploymentContext;

    public string ContainerName => this.m_blobContainer.Name;

    public FileImportBlobProvider(IVssRequestContext requestContext)
    {
      Guid instanceId = requestContext.ServiceHost.InstanceId;
      this.m_deploymentContext = requestContext.To(TeamFoundationHostType.Deployment);
      TeamFoundationServiceHostProperties serviceHostProperties = this.m_deploymentContext.GetService<ITeamFoundationHostManagementService>().QueryServiceHostProperties(this.m_deploymentContext, instanceId);
      this.m_blobProvider = this.m_deploymentContext.GetService<IAzureBlobProviderService>().CreateAndInitializeBlobProvider<IAzureBlobProvider>(this.m_deploymentContext, serviceHostProperties.StorageAccountId);
      this.m_blobProvider.DisableBufferManager();
      this.m_blobContainer = this.m_blobProvider.GetCloudBlobContainer(requestContext, instanceId, true);
    }

    public void PutStream(
      string resourceId,
      Stream content,
      IDictionary<string, string> metadata,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      this.m_blobProvider.PutStreamRawUsingBlockBlobClient(this.m_blobContainer, resourceId, content, metadata, new int?(5242880), cancellationToken);
    }

    public void Dispose() => this.m_blobProvider.ServiceEnd(this.m_deploymentContext);
  }
}
