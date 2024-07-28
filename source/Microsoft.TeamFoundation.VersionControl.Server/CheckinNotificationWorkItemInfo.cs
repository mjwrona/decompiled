// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CheckinNotificationWorkItemInfo
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  public class CheckinNotificationWorkItemInfo
  {
    private int m_id;
    private CheckinWorkItemAction m_checkinAction;

    public int Id
    {
      get => this.m_id;
      set => this.m_id = value;
    }

    public CheckinWorkItemAction CheckinAction
    {
      get => this.m_checkinAction;
      set => this.m_checkinAction = value;
    }
  }
}
