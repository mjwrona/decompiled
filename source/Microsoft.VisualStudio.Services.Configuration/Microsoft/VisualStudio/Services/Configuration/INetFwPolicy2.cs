// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.INetFwPolicy2
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Services.Configuration
{
  [Guid("98325047-C671-4174-8D81-DEFCD3F03186")]
  [TypeLibType(4160)]
  public interface INetFwPolicy2
  {
    [DispId(1)]
    int CurrentProfileTypes { [DispId(1)] get; }

    [DispId(2)]
    bool FirewallEnabled { [DispId(2)] get; [DispId(2)] set; }

    [DispId(3)]
    object ExcludedInterfaces { [DispId(3)] [return: MarshalAs(UnmanagedType.Struct)] get; [DispId(3)] [param: MarshalAs(UnmanagedType.Struct)] set; }

    [DispId(4)]
    bool BlockAllInboundTraffic { [DispId(4)] get; [DispId(4)] set; }

    [DispId(5)]
    bool NotificationsDisabled { [DispId(5)] get; [DispId(5)] set; }

    [DispId(6)]
    bool UnicastResponsesToMulticastBroadcastDisabled { [DispId(6)] get; [DispId(6)] set; }

    [DispId(7)]
    INetFwRules Rules { [DispId(7)] [return: MarshalAs(UnmanagedType.Interface)] get; }

    [DispId(14)]
    bool IsRuleGroupCurrentlyEnabled { [DispId(14)] get; }

    [DispId(9)]
    void EnableRuleGroup([In] int profileTypesBitmask, [MarshalAs(UnmanagedType.BStr), In] string group, [In] bool enable);

    [DispId(10)]
    bool IsRuleGroupEnabled([In] int profileTypesBitmask, [MarshalAs(UnmanagedType.BStr), In] string group);

    [DispId(11)]
    void RestoreLocalFirewallDefaults();
  }
}
