// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CheckinNotificationInfo
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  public class CheckinNotificationInfo
  {
    private CheckinNotificationWorkItemInfo[] m_workItemInfo = Array.Empty<CheckinNotificationWorkItemInfo>();

    public CheckinNotificationWorkItemInfo[] WorkItemInfo
    {
      get => this.m_workItemInfo;
      set => this.m_workItemInfo = value;
    }

    internal void RecordInformation(MethodInformation methodInformation)
    {
      if (this.WorkItemInfo == null || this.WorkItemInfo.Length == 0)
        return;
      foreach (CheckinNotificationWorkItemInfo notificationWorkItemInfo in this.WorkItemInfo)
        methodInformation.AddParameter("workitem(s)", (object) string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}({1})", (object) notificationWorkItemInfo.Id, (object) notificationWorkItemInfo.CheckinAction.ToString()));
    }
  }
}
