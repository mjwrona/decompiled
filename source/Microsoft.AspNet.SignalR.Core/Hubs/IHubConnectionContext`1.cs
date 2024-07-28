// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Hubs.IHubConnectionContext`1
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System.Collections.Generic;

namespace Microsoft.AspNet.SignalR.Hubs
{
  public interface IHubConnectionContext<T>
  {
    T All { get; }

    T AllExcept(params string[] excludeConnectionIds);

    T Client(string connectionId);

    T Clients(IList<string> connectionIds);

    T Group(string groupName, params string[] excludeConnectionIds);

    T Groups(IList<string> groupNames, params string[] excludeConnectionIds);

    T User(string userId);

    T Users(IList<string> userIds);
  }
}
