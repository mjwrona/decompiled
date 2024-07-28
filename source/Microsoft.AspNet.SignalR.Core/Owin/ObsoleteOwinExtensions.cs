// Decompiled with JetBrains decompiler
// Type: Owin.ObsoleteOwinExtensions
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR;
using System;

namespace Owin
{
  public static class ObsoleteOwinExtensions
  {
    [Obsolete("Use IAppBuilder.MapSignalR in an Owin Startup class. See http://go.microsoft.com/fwlink/?LinkId=320578 for more details.")]
    public static IAppBuilder MapHubs(this IAppBuilder builder) => builder.MapSignalR();

    [Obsolete("Use IAppBuilder.MapSignalR in an Owin Startup class. See http://go.microsoft.com/fwlink/?LinkId=320578 for more details.")]
    public static IAppBuilder MapHubs(this IAppBuilder builder, HubConfiguration configuration) => builder.MapSignalR(configuration);

    [Obsolete("Use IAppBuilder.MapSignalR in an Owin Startup class. See http://go.microsoft.com/fwlink/?LinkId=320578 for more details.")]
    public static IAppBuilder MapHubs(
      this IAppBuilder builder,
      string path,
      HubConfiguration configuration)
    {
      if (!ObsoleteOwinExtensions.IsEmptyOrForwardSlash(path))
        return builder.MapSignalR(path, configuration);
      builder.RunSignalR(configuration);
      return builder;
    }

    [Obsolete("Use IAppBuilder.MapSignalR<TConnection> in an Owin Startup class. See http://go.microsoft.com/fwlink/?LinkId=320578 for more details.")]
    public static IAppBuilder MapConnection<T>(this IAppBuilder builder, string path) where T : PersistentConnection
    {
      if (!ObsoleteOwinExtensions.IsEmptyOrForwardSlash(path))
        return builder.MapSignalR<T>(path);
      builder.RunSignalR<T>();
      return builder;
    }

    [Obsolete("Use IAppBuilder.MapSignalR<TConnection> in an Owin Startup class. See http://go.microsoft.com/fwlink/?LinkId=320578 for more details.")]
    public static IAppBuilder MapConnection<T>(
      this IAppBuilder builder,
      string path,
      ConnectionConfiguration configuration)
      where T : PersistentConnection
    {
      if (!ObsoleteOwinExtensions.IsEmptyOrForwardSlash(path))
        return builder.MapSignalR<T>(path, configuration);
      builder.RunSignalR<T>(configuration);
      return builder;
    }

    [Obsolete("Use IAppBuilder.MapSignalR in an Owin Startup class. See http://go.microsoft.com/fwlink/?LinkId=320578 for more details.")]
    public static IAppBuilder MapConnection(
      this IAppBuilder builder,
      string path,
      Type connectionType,
      ConnectionConfiguration configuration)
    {
      if (!ObsoleteOwinExtensions.IsEmptyOrForwardSlash(path))
        return builder.MapSignalR(path, connectionType, configuration);
      builder.RunSignalR(connectionType, configuration);
      return builder;
    }

    private static bool IsEmptyOrForwardSlash(string path) => path == string.Empty || path == "/";
  }
}
