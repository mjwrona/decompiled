// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.AclChangedEvent
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;

namespace Microsoft.TeamFoundation.Server
{
  [Obsolete("The AclChangedEvent class is obsolete.")]
  public class AclChangedEvent
  {
    public string ObjectId;
    public string ActionId;
    public string Sid;
    public string EntryType;
    public string ChangeType;

    public AclChangedEvent()
    {
      this.ObjectId = string.Empty;
      this.ActionId = string.Empty;
      this.Sid = string.Empty;
      this.EntryType = string.Empty;
      this.ChangeType = string.Empty;
    }

    public AclChangedEvent(
      string objectId,
      string actionId,
      string sid,
      string entryType,
      string changeType)
    {
      this.ObjectId = Aux.NormalizeString(objectId, false);
      this.ActionId = Aux.NormalizeString(actionId, false);
      this.Sid = Aux.NormalizeString(sid, false);
      this.EntryType = Aux.NormalizeString(entryType, false);
      this.ChangeType = Aux.NormalizeString(changeType, false);
    }
  }
}
