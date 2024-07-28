// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils.ProtocolRegistrarExtensions
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils
{
  public static class ProtocolRegistrarExtensions
  {
    public static IFactory<IFeedRequest, IConverter<string, IPackageName>> BootstrapStringToPackageNameConverterFactory(
      this IProtocolRegistrar protocolRegistrar)
    {
      return ByFuncInputFactory.For<IFeedRequest, IConverter<string, IPackageName>>((Func<IFeedRequest, IConverter<string, IPackageName>>) (feedRequest => protocolRegistrar.GetIdentityResolver(feedRequest.Protocol).NameResolver));
    }

    public static IFactory<IFeedRequest, Task<IUpstreamMetadataManager>> BootstrapUpstreamMetadataManagerFactory(
      this IProtocolRegistrar protocolRegistrar,
      IVssRequestContext requestContext)
    {
      return (IFactory<IFeedRequest, Task<IUpstreamMetadataManager>>) new ProtocolRegistrarExtensions.FactoryBuilder<Task<IUpstreamMetadataManager>>(protocolRegistrar, requestContext, (Func<IRequiredProtocolBootstrappers, Func<IVssRequestContext, IBootstrapper<IFactory<IFeedRequest, Task<IUpstreamMetadataManager>>>>>) (x => new Func<IVssRequestContext, IBootstrapper<IFactory<IFeedRequest, Task<IUpstreamMetadataManager>>>>(x.GetUpstreamMetadataManagerFactoryBootstrapper)));
    }

    private class FactoryBuilder<TReturn> : IFactory<IFeedRequest, TReturn>
    {
      private readonly IProtocolRegistrar protocolRegistrar;
      private readonly IVssRequestContext requestContext;
      private readonly Func<IRequiredProtocolBootstrappers, Func<IVssRequestContext, IBootstrapper<IFactory<IFeedRequest, TReturn>>>> bootstrapperSelectorFunc;

      public FactoryBuilder(
        IProtocolRegistrar protocolRegistrar,
        IVssRequestContext requestContext,
        Func<IRequiredProtocolBootstrappers, Func<IVssRequestContext, IBootstrapper<IFactory<IFeedRequest, TReturn>>>> bootstrapperSelectorFunc)
      {
        this.protocolRegistrar = protocolRegistrar;
        this.requestContext = requestContext;
        this.bootstrapperSelectorFunc = bootstrapperSelectorFunc;
      }

      public TReturn Get(IFeedRequest input) => this.bootstrapperSelectorFunc(this.protocolRegistrar.GetBootstrappers(input.Protocol))(this.requestContext).Bootstrap().Get(input);
    }
  }
}
