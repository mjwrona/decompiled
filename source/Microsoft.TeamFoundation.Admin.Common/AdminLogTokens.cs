// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Admin.AdminLogTokens
// Assembly: Microsoft.TeamFoundation.Admin.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B4DC7473-FE52-49C1-BB5D-1E769BB5001D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Admin.Common.dll

using System;

namespace Microsoft.TeamFoundation.Admin
{
  public static class AdminLogTokens
  {
    internal static string GetActivityDisplayName(LogActivity logActivity)
    {
      string activityByToken = AdminLogTokens.GetActivityByToken(AdminLogTokens.GetActivityToken(logActivity));
      if (string.IsNullOrEmpty(activityByToken))
        activityByToken = logActivity.ToString();
      return activityByToken;
    }

    internal static string GetAreaDisplayName(LogArea logArea)
    {
      string areaByToken = AdminLogTokens.GetAreaByToken(AdminLogTokens.GetAreaToken(logArea));
      if (string.IsNullOrEmpty(areaByToken))
        areaByToken = logArea.ToString();
      return areaByToken;
    }

    internal static string GetTypeByToken(string token)
    {
      AdminTrace.Verbose("GetTypeByToken: {0}", (object) token);
      string typeByToken = (string) null;
      if (string.Equals(token, AdminLogTokens.GetTypeToken(LogType.Configuration), StringComparison.OrdinalIgnoreCase))
        typeByToken = AdminCommonResources.LogDisplayConfiguration();
      else if (string.Equals(token, AdminLogTokens.GetTypeToken(LogType.Error), StringComparison.OrdinalIgnoreCase))
        typeByToken = AdminCommonResources.LogDisplayError();
      AdminTrace.Verbose("value: {0}", (object) typeByToken);
      return typeByToken;
    }

    internal static string GetActivityByToken(string token)
    {
      AdminTrace.Verbose("GetActivityByToken: {0}", (object) token);
      string activityByToken = (string) null;
      if (string.Equals(token, AdminLogTokens.GetActivityToken(LogActivity.Account), StringComparison.OrdinalIgnoreCase))
        activityByToken = AdminCommonResources.LogDisplayAccount();
      else if (string.Equals(token, AdminLogTokens.GetActivityToken(LogActivity.Deploy), StringComparison.OrdinalIgnoreCase))
        activityByToken = AdminCommonResources.LogDisplayDeploy();
      else if (string.Equals(token, AdminLogTokens.GetActivityToken(LogActivity.Diagnose), StringComparison.OrdinalIgnoreCase))
        activityByToken = AdminCommonResources.LogDisplayDiagnose();
      else if (string.Equals(token, AdminLogTokens.GetActivityToken(LogActivity.Settings), StringComparison.OrdinalIgnoreCase))
        activityByToken = AdminCommonResources.LogDisplaySettings();
      else if (string.Equals(token, AdminLogTokens.GetActivityToken(LogActivity.Upgrade), StringComparison.OrdinalIgnoreCase))
        activityByToken = AdminCommonResources.LogDisplayUpgrade();
      else if (string.Equals(token, AdminLogTokens.GetActivityToken(LogActivity.Servicing), StringComparison.OrdinalIgnoreCase))
        activityByToken = AdminCommonResources.LogDisplayServicing();
      else if (string.Equals(token, AdminLogTokens.GetActivityToken(LogActivity.Configuration), StringComparison.OrdinalIgnoreCase))
        activityByToken = AdminCommonResources.LogDisplayConfiguration();
      else if (string.Equals(token, AdminLogTokens.GetActivityToken(LogActivity.Collection), StringComparison.OrdinalIgnoreCase))
        activityByToken = AdminCommonResources.LogDisplayTeamProjectCollection();
      AdminTrace.Verbose("value: {0}", (object) activityByToken);
      return activityByToken;
    }

    internal static string GetAreaByToken(string token)
    {
      AdminTrace.Verbose("GetAreaByToken: {0}", (object) token);
      string areaByToken = (string) null;
      if (string.Equals(token, AdminLogTokens.GetAreaToken(LogArea.ApplicationTier), StringComparison.OrdinalIgnoreCase))
        areaByToken = AdminCommonResources.LogDisplayApplicationTier();
      else if (string.Equals(token, AdminLogTokens.GetAreaToken(LogArea.Proxy), StringComparison.OrdinalIgnoreCase))
        areaByToken = AdminCommonResources.LogDisplayProxy();
      else if (string.Equals(token, AdminLogTokens.GetAreaToken(LogArea.Unknown), StringComparison.OrdinalIgnoreCase))
        areaByToken = string.Empty;
      AdminTrace.Verbose("value: {0}", (object) areaByToken);
      return areaByToken;
    }

    internal static string GetTypeToken(LogType logType)
    {
      string typeToken;
      switch (logType)
      {
        case LogType.Configuration:
          typeToken = "CFG";
          break;
        case LogType.TeamProjectCollection:
          typeToken = "TPC";
          break;
        case LogType.Error:
          typeToken = "ERR";
          break;
        default:
          typeToken = string.Empty;
          break;
      }
      return typeToken;
    }

    internal static string GetActivityToken(LogActivity logActivity)
    {
      string activityToken;
      switch (logActivity)
      {
        case LogActivity.Collection:
          activityToken = "TPC";
          break;
        case LogActivity.Deploy:
          activityToken = "DPLY";
          break;
        case LogActivity.Upgrade:
          activityToken = "UPG";
          break;
        case LogActivity.Account:
          activityToken = "ACCT";
          break;
        case LogActivity.Diagnose:
          activityToken = "DIAG";
          break;
        case LogActivity.Servicing:
          activityToken = "SVC";
          break;
        case LogActivity.Settings:
          activityToken = "SET";
          break;
        case LogActivity.FeatureFlags:
          activityToken = "FLAGS";
          break;
        case LogActivity.Configuration:
          activityToken = "CFG";
          break;
        default:
          activityToken = string.Empty;
          break;
      }
      return activityToken;
    }

    internal static string GetAreaToken(LogArea logArea)
    {
      string areaToken;
      switch (logArea)
      {
        case LogArea.ApplicationTier:
          areaToken = "AT";
          break;
        case LogArea.Proxy:
          areaToken = "PRXY";
          break;
        case LogArea.Unknown:
          areaToken = "UNK";
          break;
        default:
          areaToken = string.Empty;
          break;
      }
      return areaToken;
    }
  }
}
