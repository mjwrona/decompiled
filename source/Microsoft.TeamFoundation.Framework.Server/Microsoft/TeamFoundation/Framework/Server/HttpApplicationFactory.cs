// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HttpApplicationFactory
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class HttpApplicationFactory
  {
    private static Func<IHttpApplication> _contextFunc = (Func<IHttpApplication>) (() => HttpContext.Current != null ? (IHttpApplication) new HttpApplicationWrapper(HttpContext.Current.ApplicationInstance) : (IHttpApplication) null);

    internal static IHttpApplication Current => HttpApplicationFactory._contextFunc();

    internal static DisposableAction SetCurrent(Func<IHttpApplication> current)
    {
      if (current == null)
        HttpApplicationFactory._contextFunc = (Func<IHttpApplication>) (() => HttpContext.Current != null ? (IHttpApplication) new HttpApplicationWrapper(HttpContext.Current.ApplicationInstance) : (IHttpApplication) null);
      Func<IHttpApplication> currentFunc = HttpApplicationFactory._contextFunc;
      HttpApplicationFactory._contextFunc = current;
      return new DisposableAction((Action) (() => HttpApplicationFactory._contextFunc = currentFunc));
    }
  }
}
