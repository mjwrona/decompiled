// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Messaging.ScaleoutMapping
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Infrastructure;
using System;
using System.Collections.Generic;

namespace Microsoft.AspNet.SignalR.Messaging
{
  public class ScaleoutMapping
  {
    public ScaleoutMapping(ulong id, ScaleoutMessage message)
      : this(id, message, ListHelper<LocalEventKeyInfo>.Empty)
    {
    }

    public ScaleoutMapping(
      ulong id,
      ScaleoutMessage message,
      IList<LocalEventKeyInfo> localKeyInfo)
    {
      if (message == null)
        throw new ArgumentNullException(nameof (message));
      if (localKeyInfo == null)
        throw new ArgumentNullException(nameof (localKeyInfo));
      this.Id = id;
      this.LocalKeyInfo = localKeyInfo;
      this.ServerCreationTime = message.ServerCreationTime;
    }

    public ulong Id { get; private set; }

    public IList<LocalEventKeyInfo> LocalKeyInfo { get; private set; }

    public DateTime ServerCreationTime { get; private set; }
  }
}
