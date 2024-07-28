// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.TokenRevocation.ServiceBaseTracePoints
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.VisualStudio.Services.TokenRevocation
{
  public static class ServiceBaseTracePoints
  {
    public const int GetAllRulesEnter = 1049082;
    public const int GetAllRulesException = 1049083;
    public const int GetAllRulesLeave = 1049084;
    public const int MatchAcceptanceRule = 104986;
    public const int MatchRejectionRule = 104987;
    public const int UnexpectedRejectionRule = 104988;
    public const int UnexpectedAcceptanceRule = 104989;
    public const int UnexpectedOrgHostCheck = 104990;
    public const int HostIdsFromToken = 104991;
    public const int TokenRevocationEvent = 104992;
    public const int RulesAtCurrentContext = 1049093;
    public const int SqlNotificationInvalidatingRulesCache = 1049085;
  }
}
