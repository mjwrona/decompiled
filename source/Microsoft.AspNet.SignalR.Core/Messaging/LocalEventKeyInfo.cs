// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Messaging.LocalEventKeyInfo
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System;

namespace Microsoft.AspNet.SignalR.Messaging
{
  public class LocalEventKeyInfo
  {
    private readonly WeakReference _storeReference;

    public LocalEventKeyInfo(string key, ulong id, Microsoft.AspNet.SignalR.Messaging.MessageStore<Message> store)
    {
      this._storeReference = new WeakReference((object) store);
      this.Key = key;
      this.Id = id;
    }

    public string Key { get; private set; }

    public ulong Id { get; private set; }

    public Microsoft.AspNet.SignalR.Messaging.MessageStore<Message> MessageStore => this._storeReference.Target as Microsoft.AspNet.SignalR.Messaging.MessageStore<Message>;
  }
}
