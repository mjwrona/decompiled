// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SqlThreatProtectionConfiguration
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class SqlThreatProtectionConfiguration
  {
    public SqlSecurityAlertPolicyState PolicyState { get; set; }

    public string StorageAccountConnectionString { get; set; }

    public int LogsRetentionDays { get; set; }

    public bool EmailAccountAdmins { get; set; }

    public string EmailAddresses { get; set; }

    public SqlThreatProtectionDisabledAlerts DisabledAlerts { get; set; }
  }
}
