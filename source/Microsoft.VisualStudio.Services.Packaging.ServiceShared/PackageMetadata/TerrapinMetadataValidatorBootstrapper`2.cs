// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata.TerrapinMetadataValidatorBootstrapper`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Constants;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion.CentralFeedServices;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata
{
  public class TerrapinMetadataValidatorBootstrapper<TPackageName, TPackageVersion>
    where TPackageName : IPackageName
    where TPackageVersion : class, IPackageVersion
  {
    private readonly IVssRequestContext requestContext;
    private readonly IIdentityResolver identityResolver;

    public TerrapinMetadataValidatorBootstrapper(
      IVssRequestContext requestContext,
      IIdentityResolver identityResolver)
    {
      this.requestContext = requestContext;
      this.identityResolver = identityResolver;
    }

    public ITerrapinMetadataValidator<TPackageName, TPackageVersion> Bootstrap() => !this.requestContext.ExecutionEnvironment.IsHostedDeployment ? (ITerrapinMetadataValidator<TPackageName, TPackageVersion>) new NoOpTerrapinMetadataValidator<TPackageName, TPackageVersion>() : this.BootstrapHosted();

    [MethodImpl(MethodImplOptions.NoInlining)]
    private ITerrapinMetadataValidator<TPackageName, TPackageVersion> BootstrapHosted()
    {
      ITracerService tracerFacade = this.requestContext.GetTracerFacade();
      ITerrapinApiStrategy terrapinApiStrategy = FeatureAvailabilityConstants.PackageIngestionUsingTerrapinApiV2.Bootstrap(this.requestContext).Get() ? (ITerrapinApiStrategy) new TerrapinApiV2Strategy(PackagingServerConstants.TerrapinApiV2BaseUriSetting.Bootstrap(this.requestContext)) : (ITerrapinApiStrategy) new TerrapinApiV1Strategy(PackagingServerConstants.TerrapinApiV1BaseUriSetting.Bootstrap(this.requestContext));
      return (ITerrapinMetadataValidator<TPackageName, TPackageVersion>) new TerrapinMetadataValidator<TPackageName, TPackageVersion>((ITerrapinService) new TerrapinService((IFactory<string>) new SpecificResourceAppTokenProvider(tracerFacade, (IInjectableAadTokenService) new AadTokenServiceFacade(this.requestContext), PackagingServerConstants.TerrapinApiAuthResourceSetting.Bootstrap(this.requestContext), PackagingServerConstants.TerrapinApiAuthAadTenantSetting.Bootstrap(this.requestContext)), (IOrgLevelPackagingSetting<bool>) new ShouldCallTerrapinSetting(FeatureAvailabilityConstants.CallTerrapinOnRefresh.Bootstrap(this.requestContext), this.requestContext.GetExecutionEnvironmentFacade(), tracerFacade), tracerFacade, (IHttpClient) new HttpClientFacade(this.requestContext, TerrapinHttpClientFacade.Get()), terrapinApiStrategy), tracerFacade, this.identityResolver, (IRegistryService) this.requestContext.GetRegistryFacade(), (ITimeProvider) new DefaultTimeProvider());
    }
  }
}
