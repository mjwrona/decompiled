// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.Keychain.SessionCounterManager
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

namespace Microsoft.VisualStudio.Services.Client.Keychain
{
  internal sealed class SessionCounterManager
  {
    private static SessionCounterManager instance;
    private static readonly object syncLock = new object();

    internal static void EnsureSessionCounterSet()
    {
      if (SessionCounterManager.instance != null)
        return;
      lock (SessionCounterManager.syncLock)
      {
        if (SessionCounterManager.instance != null)
          return;
        SessionCounterManager.instance = new SessionCounterManager();
      }
    }

    private SessionCounterManager() => ClientNativeMethods.SetQueryNetSessionCount(SessionOp.SESSION_INCREMENT);

    ~SessionCounterManager()
    {
      if (ClientNativeMethods.SetQueryNetSessionCount(SessionOp.SESSION_QUERY) <= 0)
        return;
      ClientNativeMethods.SetQueryNetSessionCount(SessionOp.SESSION_DECREMENT);
    }
  }
}
