// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.CVS.CvsServiceClient
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.Ops.Cvs.Client;
using Microsoft.Ops.Cvs.Client.DataContracts;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Gallery.Server.CVS
{
  internal class CvsServiceClient : ICvsServiceClient, IDisposable
  {
    private CvsClient m_cvsClient;

    public CvsServiceClient(
      Uri serverBaseUri,
      X509Certificate2 cvsAuthCert,
      string subscriptionKey)
    {
      this.m_cvsClient = new CvsClientBuilder(serverBaseUri).ClientCertificate(cvsAuthCert).SubscriptionKey(subscriptionKey).Client();
    }

    public Task<Job> GetJobAsync(string jobId) => this.m_cvsClient.GetJobAsync(jobId);

    public Task<Job> CreateJobAsync(
      ProcessingConfiguration processingConfiguration,
      ContentItem contentItem)
    {
      return this.m_cvsClient.CreateJobAsync(processingConfiguration, contentItem);
    }

    public void Dispose() => this.m_cvsClient.Dispose();
  }
}
