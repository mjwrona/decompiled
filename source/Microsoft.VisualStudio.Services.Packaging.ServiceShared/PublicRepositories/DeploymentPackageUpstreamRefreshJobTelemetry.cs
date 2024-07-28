// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PublicRepositories.DeploymentPackageUpstreamRefreshJobTelemetry
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.PublicRepositories
{
  public class DeploymentPackageUpstreamRefreshJobTelemetry : JobTelemetry
  {
    public int InterestedFeedCount { get; set; }

    public int InterestedCollectionCount { get; set; }

    public int SucceededCollectionCount { get; set; }

    public int FailedCollectionCount { get; set; }

    public int NonexistentCollectionCount { get; set; }

    public int ShutdownCollectionCount { get; set; }
  }
}
