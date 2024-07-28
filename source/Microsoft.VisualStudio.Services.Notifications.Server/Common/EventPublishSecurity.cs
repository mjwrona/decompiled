// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Common.EventPublishSecurity
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.Notifications.Common
{
  public static class EventPublishSecurity
  {
    public static readonly Guid NamespaceId = new Guid("7CD317F2-ADC6-4B6C-8D99-6074FAEAF173");
    public static readonly string RootToken = "/";
    public static readonly string PublishEventsResource = EventPublishSecurity.RootToken + "PublishEvents";

    public static class Permissions
    {
      public const int Write = 2;
    }
  }
}
