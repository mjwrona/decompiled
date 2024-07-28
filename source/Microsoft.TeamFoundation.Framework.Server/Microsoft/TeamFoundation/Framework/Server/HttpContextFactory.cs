// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HttpContextFactory
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class HttpContextFactory
  {
    private static Func<HttpContextBase> _contextFunc = (Func<HttpContextBase>) (() => HttpContext.Current != null ? (HttpContextBase) new HttpContextWrapper(HttpContext.Current) : (HttpContextBase) null);

    public static HttpContextBase Current => HttpContextFactory._contextFunc();

    internal static DisposableAction SetCurrent(Func<HttpContextBase> current)
    {
      if (current == null)
        HttpContextFactory._contextFunc = (Func<HttpContextBase>) (() => HttpContext.Current != null ? (HttpContextBase) new HttpContextWrapper(HttpContext.Current) : (HttpContextBase) null);
      Func<HttpContextBase> currentFunc = HttpContextFactory._contextFunc;
      HttpContextFactory._contextFunc = current;
      return new DisposableAction((Action) (() => HttpContextFactory._contextFunc = currentFunc));
    }
  }
}
