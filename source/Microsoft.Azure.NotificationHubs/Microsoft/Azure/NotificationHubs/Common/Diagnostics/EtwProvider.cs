// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Common.Diagnostics.EtwProvider
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Security;
using System.Security.Permissions;

namespace Microsoft.Azure.NotificationHubs.Common.Diagnostics
{
  internal sealed class EtwProvider : DiagnosticsEventProvider
  {
    private Action invokeControllerCallback;

    [SecurityCritical]
    [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
    internal EtwProvider(Guid id)
      : base(id)
    {
    }

    internal Action ControllerCallBack
    {
      get => this.invokeControllerCallback;
      set => this.invokeControllerCallback = value;
    }

    protected override void OnControllerCommand()
    {
      if (this.invokeControllerCallback == null)
        return;
      this.invokeControllerCallback();
    }
  }
}
