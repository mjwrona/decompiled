// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Notifications.NotificationsAreaRegistration
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Notifications, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 14A6358C-7796-4C8A-90AB-FFD4A6B1DD61
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Notifications.dll

using System;
using System.Resources;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Notifications
{
  public class NotificationsAreaRegistration : AreaRegistration
  {
    public override string AreaName => "NotificationsUI";

    public override void RegisterArea(AreaRegistrationContext context) => ScriptRegistration.RegisterBundledArea("NotificationsUI", (Func<ResourceManager>) (() => NotificationsResources.ResourceManager), "VSS");
  }
}
