// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.ContentStitcher.ContentStitcherHostnameNotFoundException
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.DedupStorage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9521FAE3-5DB1-49D0-98DB-6A544E3AB730
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.DedupStorage.dll

using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.ContentStitcher
{
  public class ContentStitcherHostnameNotFoundException : Exception
  {
    public ContentStitcherHostnameNotFoundException(
      string hostedServiceName,
      string serviceName,
      string blobStoreTenantName)
      : base("Cannot locate the hosted service short name [vsblob] in [" + hostedServiceName + "] for [" + serviceName + "] under registry path: [/Configuration/Settings/HostedServiceName] or from the BlobStore tenant [" + blobStoreTenantName + "] fetched from location service.")
    {
    }
  }
}
