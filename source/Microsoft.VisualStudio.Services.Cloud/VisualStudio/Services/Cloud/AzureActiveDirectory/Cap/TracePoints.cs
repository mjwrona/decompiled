// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Cap.TracePoints
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

namespace Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Cap
{
  public static class TracePoints
  {
    private const int Start = 9003000;

    public static class FrameworkAadCapService
    {
    }

    public static class AadCapRequestAuthenticationValidator
    {
      internal const int IsValidEnter = 9003300;
      internal const int IsValidLeave = 9003301;
      internal const int IsValidSkippedServiceHost = 9003302;
      internal const int IsValidSkippedFeatureAvailability = 9003303;
      internal const int IsValidSkippedMechanism = 9003304;
      internal const int IsValidSkippedOrgPolicy = 9003305;
      internal const int IsValidSkippedRequestType = 9003306;
      internal const int IsValidInvokeService = 9003307;
      internal const int IsValidServiceResult = 9003308;
      internal const int IsValidCatchCredsNotFound = 9003309;
      internal const int IsValidGeneralException = 9003310;
      internal const int IsValidNoIp = 9003311;
      internal const int IsValidSkippedTenantId = 9003312;
      internal const int CompleteInvalidRequestEnter = 9003350;
      internal const int CompleteInvalidRequestLeave = 9003351;
      internal const int CompleteInvalidRedirect = 9003352;
      internal const int CompleteInvalidForbidden = 9003353;
    }

    public static class AadCapRedisCache
    {
      internal const int TryGetEnter = 9003400;
      internal const int TryGetLeave = 9003401;
      internal const int TryGetException = 9003402;
      internal const int SetEnter = 9003410;
      internal const int SetLeave = 9003411;
      internal const int SetException = 9003412;
    }

    public static class AadCapMemoryCacheService
    {
      internal const int TryGetEnter = 9003500;
      internal const int TryGetLeave = 9003501;
      internal const int SetEnter = 9003510;
      internal const int SetLeave = 9003511;
      internal const int InitialExpirationValue = 520;
      internal const int OnRegistryChangeUpdateExpiration = 521;
      internal const int OnRegistryChangeNoUpdate = 521;
    }

    public static class AadCapController
    {
      internal const int GetAllowedEnter = 9003800;
      internal const int GetAllowedLeave = 9003801;
      internal const int GetAllowedInfo = 9003802;
      internal const int GetAllowedError = 9003803;
      internal const int GetAllowedSuccess = 9003804;
    }
  }
}
