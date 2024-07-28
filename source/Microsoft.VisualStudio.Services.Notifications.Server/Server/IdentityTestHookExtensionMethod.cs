// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.IdentityTestHookExtensionMethod
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal static class IdentityTestHookExtensionMethod
  {
    private static IIdentityTestHook s_testHook;

    private static IIdentityTestHook GetTestHook(IVssRequestContext requestContext)
    {
      if (IdentityTestHookExtensionMethod.s_testHook == null)
      {
        IReadOnlyList<IIdentityTestHook> notificationExtentions = requestContext.GetStaticNotificationExtentions<IIdentityTestHook>();
        if (notificationExtentions != null)
          IdentityTestHookExtensionMethod.s_testHook = notificationExtentions.FirstOrDefault<IIdentityTestHook>();
        if (IdentityTestHookExtensionMethod.s_testHook == null)
          IdentityTestHookExtensionMethod.s_testHook = (IIdentityTestHook) IdentityTestHookExtensionMethod.ProductionTestHook.Singleton;
      }
      return IdentityTestHookExtensionMethod.s_testHook;
    }

    internal static bool IsTestIdentity(this Microsoft.VisualStudio.Services.Identity.Identity identity, IVssRequestContext requestContext) => IdentityTestHookExtensionMethod.GetTestHook(requestContext).IsTestIdentity(identity);

    private class ProductionTestHook : IIdentityTestHook
    {
      internal static IdentityTestHookExtensionMethod.ProductionTestHook Singleton = new IdentityTestHookExtensionMethod.ProductionTestHook(-8675309);

      private ProductionTestHook(int dontCareButItsNotOptional)
      {
      }

      bool IIdentityTestHook.IsTestIdentity(Microsoft.VisualStudio.Services.Identity.Identity identity) => false;
    }
  }
}
