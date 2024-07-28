// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TracePoints
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal static class TracePoints
  {
    internal static class VssfAuthenticationModuleTrace
    {
      internal static int ExternalUserResolutionDecision = 60200;
      internal static int ExternalUserResolutionInfo = 60201;
      internal static int ExternalUserResolutionError = 60202;
      internal static int TenantSwitchRequiredEntry = 60205;
      internal static int TenantSwitchInfo = 60206;
      internal static int TenantSwitchRequiredExit = 60210;
      internal static int ServicePrincipalExpected = 60211;
      internal static int CreatingSilentAADProfile = 60212;
      internal static int NextTracePoint = 60213;

      internal enum IpResolutionTracePoint
      {
        MetricCollectionError = 60117, // 0x0000EAD5
      }

      internal enum ArrForwardedIpValidatorTracePoints
      {
        IsIpTrustedEnter = 60110, // 0x0000EACE
        IsIpTrustedLeave = 60111, // 0x0000EACF
        SpoofingDetected = 60112, // 0x0000EAD0
        TrustedIp = 60113, // 0x0000EAD1
        BothHeadersMissing = 60114, // 0x0000EAD2
        UnexpectedTokenValidationError = 60116, // 0x0000EAD4
      }

      internal enum AzureFrontDoorIpValidatorTracePoints
      {
        ValidateEnter = 60035, // 0x0000EA83
        ValidateEmptyIpWarn = 60036, // 0x0000EA84
        ValidateEmptyIpLeave = 60037, // 0x0000EA85
        ValidateKnownIpLeave = 60038, // 0x0000EA86
        ValidateAfdImpresonationLeave = 60039, // 0x0000EA87
        ValidateException = 60048, // 0x0000EA90
        ValidateExceptionLeave = 60049, // 0x0000EA91
        ValidateAfdImpersonationWarn = 60053, // 0x0000EA95
        ValidateLeaveDisabled = 60054, // 0x0000EA96
      }
    }
  }
}
