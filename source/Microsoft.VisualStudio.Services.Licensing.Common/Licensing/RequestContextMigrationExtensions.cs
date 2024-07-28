// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.RequestContextMigrationExtensions
// Assembly: Microsoft.VisualStudio.Services.Licensing.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F3070F25-7414-49A0-9C00-005379F04A49
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Licensing
{
  public static class RequestContextMigrationExtensions
  {
    private static readonly Guid SpsInstanceType = new Guid("00000001-0000-8888-8000-000000000000");
    public const string UseCosmosDbFeatureName = "VisualStudio.Services.Licensing.UseCosmosDb";
    public const string AccountMigratedFeatureFlag = "Visualstudio.Services.Licensing.AccountMigrated";
    public const string DisablePublishLicensingServiceBusMessages = "VisualStudio.Services.Licensing.DisablePublishServiceBusMessages";
    public const string LicensingWriteDisabledFeatureFlag = "Visualstudio.Services.Licensing.WriteDisabled";
    public const string LicensingReadForwardingEnabledFeatureFlag = "Visualstudio.Services.Licensing.ReadForwardingEnabled";
    public const string DisableLpsFeatureFlag = "VisualStudio.Services.Licensing.DisableLPS";
    public const string LicensingTracingDisabled = "VisualStudio.Services.Licensing.DisableLicensingTraces";
    public const string LicensingMigration = "LicensingMigration";
    private const string CommandKeyForForwardLicensingWrites = "ForwardLicensingWrites";
    private const string CommandKeyForForwardLicensingReads = "ForwardLicensingReads";
    private const string CommandKeyForForwardLicensingReadWrites = "ForwardLicensingReadWrites";

    public static bool IsSps(this IVssRequestContext requestContext) => requestContext.ServiceInstanceType() == ServiceInstanceTypes.SPS;

    public static bool IsLps(this IVssRequestContext requestContext) => requestContext.ServiceInstanceType() == LicensingCommonConstants.LicensingInstanceType;

    public static bool PublishServiceBusMessages(this IVssRequestContext requestContext) => requestContext.IsSps() && !requestContext.IsFeatureEnabled("VisualStudio.Services.Licensing.DisablePublishServiceBusMessages") || requestContext.IsLps() && !requestContext.IsFeatureEnabled("VisualStudio.Services.Licensing.DisablePublishServiceBusMessages");

    public static bool UseCosmosDb(this IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("VisualStudio.Services.Licensing.UseCosmosDb");

    public static IVssRequestContext ToLicensingComponentRequestContext(
      this IVssRequestContext requestContext)
    {
      return !requestContext.UseCosmosDb() ? requestContext.To(TeamFoundationHostType.Application) : requestContext;
    }

    public static IVssRequestContext ToLicensingPermissionRequestContext(
      this IVssRequestContext requestContext)
    {
      return !requestContext.IsLps() ? requestContext.To(TeamFoundationHostType.Application) : requestContext;
    }

    public static bool LicensingTracingEnabled(this IVssRequestContext requestContext) => !requestContext.IsFeatureEnabled("VisualStudio.Services.Licensing.DisableLicensingTraces");

    public static Guid GetServiceSource(this IVssRequestContext requestContext)
    {
      if (ServicePrincipals.IsServicePrincipal(requestContext, requestContext.GetAuthenticatedDescriptor()))
      {
        Microsoft.VisualStudio.Services.Identity.Identity authenticatedIdentity = requestContext.GetAuthenticatedIdentity();
        if (authenticatedIdentity != null)
          return authenticatedIdentity.Id;
      }
      return Guid.Empty;
    }

    public static void EnsureCollectionIsFaultedIn(this IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      vssRequestContext.GetService<IInstanceManagementService>().GetHostInstanceMapping(vssRequestContext, requestContext.ServiceHost.InstanceId, LicensingCommonConstants.LicensingInstanceType);
    }
  }
}
