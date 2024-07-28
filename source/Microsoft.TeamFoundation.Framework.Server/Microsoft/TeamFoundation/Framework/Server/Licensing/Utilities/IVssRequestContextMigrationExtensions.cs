// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Licensing.Utilities.IVssRequestContextMigrationExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.GroupLicensingRule;
using Microsoft.VisualStudio.Services.Licensing.Client;
using Microsoft.VisualStudio.Services.Location.Server;
using System;

namespace Microsoft.TeamFoundation.Framework.Server.Licensing.Utilities
{
  public static class IVssRequestContextMigrationExtensions
  {
    private const string Area = "IVssRequestContextMigrationExtensions";
    private const string Layer = "IVssRequestContextMigrationExtensions";
    private static Guid LpsGuid = new Guid("00000043-0000-8888-8000-000000000000");

    public static IAccountLicensingHttpClient GetAccountLicensingHttpClient(
      this IVssRequestContext requestContext)
    {
      return (IAccountLicensingHttpClient) requestContext.GetClient<AccountLicensingHttpClient>();
    }

    public static IAccountLicensingHttpClient GetAccountLicensingHttpClient(
      this IVssRequestContext requestContext,
      Guid hostId)
    {
      Uri hostUri = requestContext.GetService<IUrlHostResolutionService>().GetHostUri(requestContext, hostId, IVssRequestContextMigrationExtensions.LpsGuid);
      return (IAccountLicensingHttpClient) HttpClientHelper.CreateClient<AccountLicensingHttpClient>(requestContext, hostUri);
    }

    internal static IAccountExtensionLicensingHttpClient GetAccountExtensionLicensingHttpClient(
      this IVssRequestContext requestContext)
    {
      return (IAccountExtensionLicensingHttpClient) requestContext.GetClient<AccountExtensionLicensingHttpClient>();
    }

    internal static LicensingRuleHttpClient GetLicensingRuleHttpClient(
      this IVssRequestContext requestContext)
    {
      return requestContext.GetClient<LicensingRuleHttpClient>();
    }
  }
}
