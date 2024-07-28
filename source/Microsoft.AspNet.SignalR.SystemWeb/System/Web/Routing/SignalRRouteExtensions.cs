// Decompiled with JetBrains decompiler
// Type: System.Web.Routing.SignalRRouteExtensions
// Assembly: Microsoft.AspNet.SignalR.SystemWeb, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 683C8AD1-A25F-40E2-A0AC-9A1047E77A7E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.SystemWeb.dll

using Microsoft.AspNet.SignalR;
using Owin;

namespace System.Web.Routing
{
  public static class SignalRRouteExtensions
  {
    [Obsolete("Use IAppBuilder.MapSignalR<TConnection> in an Owin Startup class. See http://go.microsoft.com/fwlink/?LinkId=320578 for more details.", true)]
    public static RouteBase MapConnection<T>(this RouteCollection routes, string name, string url) where T : PersistentConnection => throw new NotImplementedException();

    [Obsolete("Use IAppBuilder.MapSignalR<TConnection> in an Owin Startup class. See http://go.microsoft.com/fwlink/?LinkId=320578 for more details.", true)]
    public static RouteBase MapConnection<T>(
      this RouteCollection routes,
      string name,
      string url,
      ConnectionConfiguration configuration)
      where T : PersistentConnection
    {
      throw new NotImplementedException();
    }

    [Obsolete("Use IAppBuilder.MapSignalR<TConnection> in an Owin Startup class. See http://go.microsoft.com/fwlink/?LinkId=320578 for more details.", true)]
    public static RouteBase MapConnection<T>(
      this RouteCollection routes,
      string name,
      string url,
      ConnectionConfiguration configuration,
      Action<IAppBuilder> build)
      where T : PersistentConnection
    {
      throw new NotImplementedException();
    }

    [Obsolete("Use IAppBuilder.MapSignalR<TConnection> in an Owin Startup class. See http://go.microsoft.com/fwlink/?LinkId=320578 for more details.", true)]
    public static RouteBase MapConnection(
      this RouteCollection routes,
      string name,
      string url,
      Type type,
      ConnectionConfiguration configuration)
    {
      throw new NotImplementedException();
    }

    [Obsolete("Use IAppBuilder.MapSignalR<TConnection> in an Owin Startup class. See http://go.microsoft.com/fwlink/?LinkId=320578 for more details.", true)]
    public static RouteBase MapConnection(
      this RouteCollection routes,
      string name,
      string url,
      Type type,
      ConnectionConfiguration configuration,
      Action<IAppBuilder> build)
    {
      throw new NotImplementedException();
    }

    [Obsolete("Use IAppBuilder.MapSignalR in an Owin Startup class. See http://go.microsoft.com/fwlink/?LinkId=320578 for more details.", true)]
    public static RouteBase MapHubs(this RouteCollection routes) => throw new NotImplementedException();

    [Obsolete("Use IAppBuilder.MapSignalR in an Owin Startup class. See http://go.microsoft.com/fwlink/?LinkId=320578 for more details.", true)]
    public static RouteBase MapHubs(this RouteCollection routes, HubConfiguration configuration) => throw new NotImplementedException();

    [Obsolete("Use IAppBuilder.MapSignalR in an Owin Startup class. See http://go.microsoft.com/fwlink/?LinkId=320578 for more details.", true)]
    public static RouteBase MapHubs(
      this RouteCollection routes,
      string path,
      HubConfiguration configuration)
    {
      throw new NotImplementedException();
    }

    [Obsolete("Use IAppBuilder.MapSignalR in an Owin Startup class. See http://go.microsoft.com/fwlink/?LinkId=320578 for more details.", true)]
    public static RouteBase MapHubs(
      this RouteCollection routes,
      string path,
      HubConfiguration configuration,
      Action<IAppBuilder> build)
    {
      throw new NotImplementedException();
    }

    [Obsolete("Use IAppBuilder.MapSignalR in an Owin Startup class. See http://go.microsoft.com/fwlink/?LinkId=320578 for more details.", true)]
    internal static RouteBase MapHubs(
      this RouteCollection routes,
      string name,
      string path,
      HubConfiguration configuration,
      Action<IAppBuilder> build)
    {
      throw new NotImplementedException();
    }
  }
}
