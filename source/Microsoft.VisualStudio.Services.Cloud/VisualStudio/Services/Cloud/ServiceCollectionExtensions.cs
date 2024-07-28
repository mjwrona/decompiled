// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ServiceCollectionExtensions
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.GeoReplication;
using Microsoft.VisualStudio.Services.Cloud.GeoReplication;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public static class ServiceCollectionExtensions
  {
    public static IVssFrameworkServiceCollection AddCloud(
      this IVssFrameworkServiceCollection services)
    {
      services.AddFramework();
      services.TryAdd<IGeoReplicationService, GeoReplicationService>();
      services.TryAdd<IContentValidationService, HostedContentValidationService>();
      services.TryAdd<IContentViolationService, HostedContentViolationService>();
      services.TryAdd<IHostRegionService, HostRegionService>();
      services.TryAdd<ITeamFoundationBasicAuthService, FrameworkBasicAuthService>();
      return services;
    }
  }
}
