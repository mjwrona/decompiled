// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.OnCall.TeamMembershipResult
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

namespace Microsoft.AzureAd.Icm.Types.OnCall
{
  public class TeamMembershipResult
  {
    public string PublicTeamId { get; set; }

    public TeamMembershipStatus Status { get; set; }

    public string Message { get; set; }
  }
}
