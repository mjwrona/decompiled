// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.MembershipChangedEvent
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;

namespace Microsoft.TeamFoundation.Server
{
  [Obsolete("The MembershipChangedEvent class is obsolete.")]
  public class MembershipChangedEvent
  {
    public string GroupSid;
    public string MemberSid;
    public string ChangeType;

    public MembershipChangedEvent()
    {
      this.GroupSid = string.Empty;
      this.MemberSid = string.Empty;
      this.ChangeType = string.Empty;
    }

    public MembershipChangedEvent(string groupSid, string memberSid, string changeType)
    {
      this.GroupSid = Aux.NormalizeString(groupSid, false);
      this.MemberSid = Aux.NormalizeString(memberSid, false);
      this.ChangeType = Aux.NormalizeString(changeType, false);
    }
  }
}
