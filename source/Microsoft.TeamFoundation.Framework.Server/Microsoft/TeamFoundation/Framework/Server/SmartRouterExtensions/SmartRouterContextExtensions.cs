// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SmartRouterExtensions.SmartRouterContextExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll


#nullable enable
namespace Microsoft.TeamFoundation.Framework.Server.SmartRouterExtensions
{
  public static class SmartRouterContextExtensions
  {
    public static SmartRouterContext? TryGetSmartRouterContext(
      this IVssRequestContext requestContext)
    {
      SmartRouterContext smartRouterContext;
      return !requestContext.TryGetItem<SmartRouterContext>("SmartRouterContext", out smartRouterContext) ? (SmartRouterContext) null : smartRouterContext;
    }

    public static SmartRouterContext GetSmartRouterContext(this IVssRequestContext requestContext)
    {
      SmartRouterContext smartRouterContext = requestContext.TryGetSmartRouterContext();
      if (smartRouterContext == null)
        requestContext.Items["SmartRouterContext"] = (object) (smartRouterContext = new SmartRouterContext());
      return smartRouterContext;
    }
  }
}
