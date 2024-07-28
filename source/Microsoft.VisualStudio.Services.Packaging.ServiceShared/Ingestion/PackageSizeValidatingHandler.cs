// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion.PackageSizeValidatingHandler
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion
{
  public class PackageSizeValidatingHandler : 
    IAsyncHandler<long>,
    IAsyncHandler<long, NullResult>,
    IHaveInputType<long>,
    IHaveOutputType<NullResult>
  {
    public static RegistryFrotocolLevelPackagingSettingDefinition<long> MaxSizeRegistrySettingDefinition = new RegistryFrotocolLevelPackagingSettingDefinition<long>((Func<IProtocol, RegistryQuery>) (protocol => (RegistryQuery) ("/Configuration/Packaging/" + protocol.CorrectlyCasedName + "/Ingestion/MaxSize")), (Func<IProtocol, FeedIdentity, RegistryQuery>) ((protocol, feed) => (RegistryQuery) string.Format("/Configuration/Packaging/{0}/Ingestion/MaxSize/{1:d}", (object) protocol.CorrectlyCasedName, (object) feed.Id)), 524288000L);
    public const long MaxPushSizeDefault = 524288000;
    private const string MaxPushSizeRegistryPath = "/Configuration/Packaging/{0}/Ingestion/MaxSize";
    private readonly IRegistryService registryService;
    private readonly string protocol;

    public PackageSizeValidatingHandler(IRegistryService registryService, string protocol)
    {
      this.registryService = registryService;
      this.protocol = protocol;
    }

    public Task<NullResult> Handle(long packageSize)
    {
      long num = this.registryService.GetValue<long>((RegistryQuery) string.Format("/Configuration/Packaging/{0}/Ingestion/MaxSize", (object) this.protocol), 524288000L);
      if (packageSize > num)
        throw new PackageLimitExceededException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_FileLimitExceeded((object) num));
      return Task.FromResult<NullResult>((NullResult) null);
    }
  }
}
