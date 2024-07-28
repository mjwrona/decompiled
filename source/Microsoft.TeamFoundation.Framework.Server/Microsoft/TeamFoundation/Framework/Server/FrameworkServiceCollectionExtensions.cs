// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FrameworkServiceCollectionExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Framework;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Licensing;
using Microsoft.VisualStudio.Services.Redis;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class FrameworkServiceCollectionExtensions
  {
    public static IVssFrameworkServiceCollection AddFramework(
      this IVssFrameworkServiceCollection services)
    {
      services.TryAdd<IdentityService, CompositeIdentityService>();
      services.TryAdd<IUserIdentityService, FrameworkIdentityServiceWrapper>();
      services.TryAdd<ILicensingService, FrameworkLicensingService>();
      services.TryAdd<IDisplayVersionService, DisplayVersionService>();
      return services;
    }

    public static IVssFrameworkServiceCollection AddFrameworkOnPrem(
      this IVssFrameworkServiceCollection services)
    {
      services.AddFramework();
      services.TryAdd<IMessageBusManagementService, SqlMessageBusManagementService>();
      services.TryAdd<IMessageBusPublisherService, SqlMessageBusPublisherService>();
      services.TryAdd<IMessageBusSubscriberService, SqlMessageBusSubscriberService>();
      services.TryAdd<IRedisCacheService, StubRedisCacheService>();
      services.TryAdd<IContentValidationService, NoOpContentValidationService>();
      services.TryAdd<IContentValidationService, NoOpContentValidationService>();
      services.TryAdd<IContentViolationService, NoOpContentViolationService>();
      services.TryAdd<IFaultInjectionService, NoOpFaultInjectionService>();
      services.TryAdd<ISmartRouterRequestHandlerService, StubSmartRouterRequestHandlerService>();
      services.TryAdd<IKeyVaultWrappedKeyService, StubKeyVaultWrappedKeyService>();
      return services;
    }
  }
}
