// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Messaging.ISubscriber
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.AspNet.SignalR.Messaging
{
  public interface ISubscriber
  {
    IList<string> EventKeys { get; }

    Action<TextWriter> WriteCursor { get; set; }

    string Identity { get; }

    event Action<ISubscriber, string> EventKeyAdded;

    event Action<ISubscriber, string> EventKeyRemoved;

    Subscription Subscription { get; set; }
  }
}
