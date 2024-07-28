// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Common.JobFramework.RegistryBasedJobParallelismProvider
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;

namespace Microsoft.VisualStudio.Services.Content.Server.Common.JobFramework
{
  public class RegistryBasedJobParallelismProvider : IJobParallelismProvider
  {
    private const string RegistryTotalPartitionsKey = "/TotalPartitions";
    private readonly string RegistryBasePath;

    public RegistryBasedJobParallelismProvider(string registryBasePath) => this.RegistryBasePath = registryBasePath;

    public int GetTotalPartitions(
      IVssRequestContext requestContext,
      IDomainId domainId,
      int defaultPartitionNumber)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string str = string.Format("/Domain/{0}/TotalPartitions", (object) domainId.Serialize());
      int totalPartitions = domainId == (IDomainId) null ? -1 : service.GetValue<int>(requestContext, (RegistryQuery) (this.RegistryBasePath + str), true, -1);
      if (totalPartitions == -1)
        totalPartitions = service.GetValue<int>(requestContext, (RegistryQuery) (this.RegistryBasePath + "/TotalPartitions"), true, defaultPartitionNumber);
      return totalPartitions;
    }
  }
}
