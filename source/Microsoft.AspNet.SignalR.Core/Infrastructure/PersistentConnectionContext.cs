// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Infrastructure.PersistentConnectionContext
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

namespace Microsoft.AspNet.SignalR.Infrastructure
{
  internal class PersistentConnectionContext : IPersistentConnectionContext
  {
    public PersistentConnectionContext(IConnection connection, IConnectionGroupManager groupManager)
    {
      this.Connection = connection;
      this.Groups = groupManager;
    }

    public IConnection Connection { get; private set; }

    public IConnectionGroupManager Groups { get; private set; }
  }
}
