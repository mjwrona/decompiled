// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Reporting.DataServices.DataServicesLimitPolicy
// Assembly: Microsoft.TeamFoundation.Reporting.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0871DF71-209E-4628-905A-D95195A70FEC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Reporting.DataServices.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.Reporting.DataServices
{
  public class DataServicesLimitPolicy
  {
    private static readonly int CfdDoneLimitCount = 4000;
    public const string CfdDoneLimitCountRegistryPath = "/Service/DataServices/Settings/CFDDoneLimitCount";

    public static string CfdTooManyDoneElementsMessage => ReportingResources.CfdTooManyDoneElements();

    public static int GetCfdDoneLimitCount(IVssRequestContext requestContext) => DataServicesLimitPolicy.GetLimit(requestContext, "/Service/DataServices/Settings/CFDDoneLimitCount", DataServicesLimitPolicy.CfdDoneLimitCount);

    public static int GetLimit(
      IVssRequestContext requestContext,
      string limitRegistryPath,
      int defaultLimit)
    {
      CachedRegistryService service = requestContext.GetService<CachedRegistryService>();
      return service == null ? defaultLimit : service.GetValue<int>(requestContext, (RegistryQuery) limitRegistryPath, defaultLimit);
    }
  }
}
