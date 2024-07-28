// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Messaging.ScaleoutConfiguration
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System;

namespace Microsoft.AspNet.SignalR.Messaging
{
  public class ScaleoutConfiguration
  {
    private int _maxQueueLength;

    public ScaleoutConfiguration()
    {
      this.QueueBehavior = QueuingBehavior.InitialOnly;
      this._maxQueueLength = 1000;
    }

    public virtual QueuingBehavior QueueBehavior { get; set; }

    public virtual int MaxQueueLength
    {
      get => this._maxQueueLength;
      set => this._maxQueueLength = value >= 0 ? value : throw new ArgumentOutOfRangeException(nameof (value));
    }
  }
}
