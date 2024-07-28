// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.OwinEnvironmentExtensions
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;

namespace Microsoft.AspNet.SignalR
{
  internal static class OwinEnvironmentExtensions
  {
    internal static CancellationToken GetShutdownToken(this IDictionary<string, object> env)
    {
      object obj;
      return !env.TryGetValue("host.OnAppDisposing", out obj) || !(obj is CancellationToken cancellationToken) ? new CancellationToken() : cancellationToken;
    }

    internal static string GetAppInstanceName(this IDictionary<string, object> environment)
    {
      object obj;
      if (environment.TryGetValue("host.AppName", out obj))
      {
        string appInstanceName = obj as string;
        if (!string.IsNullOrEmpty(appInstanceName))
          return appInstanceName;
      }
      return (string) null;
    }

    internal static TextWriter GetTraceOutput(this IDictionary<string, object> environment)
    {
      object obj;
      return environment.TryGetValue("host.TraceOutput", out obj) ? obj as TextWriter : (TextWriter) null;
    }

    internal static bool SupportsWebSockets(this IDictionary<string, object> environment)
    {
      object obj;
      return environment.TryGetValue("server.Capabilities", out obj) && obj is IDictionary<string, object> dictionary && dictionary.ContainsKey("websocket.Version");
    }

    internal static bool IsDebugEnabled(this IDictionary<string, object> environment)
    {
      object obj;
      if (!environment.TryGetValue("host.AppMode", out obj))
        return false;
      string str = obj as string;
      return !string.IsNullOrWhiteSpace(str) && "development".Equals(str, StringComparison.OrdinalIgnoreCase);
    }

    internal static IEnumerable<Assembly> GetReferenceAssemblies(
      this IDictionary<string, object> environment)
    {
      object obj;
      return environment.TryGetValue("host.ReferencedAssemblies", out obj) ? (IEnumerable<Assembly>) obj : (IEnumerable<Assembly>) null;
    }

    internal static void DisableResponseBuffering(this IDictionary<string, object> environment)
    {
      Action action = environment.Get<Action>("server.DisableResponseBuffering");
      if (action == null)
        return;
      action();
    }

    internal static void DisableRequestCompression(this IDictionary<string, object> environment)
    {
      Action action = environment.Get<Action>("systemweb.DisableResponseCompression");
      if (action == null)
        return;
      action();
    }
  }
}
