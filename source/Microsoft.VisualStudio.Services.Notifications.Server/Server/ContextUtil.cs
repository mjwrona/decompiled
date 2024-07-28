// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.ContextUtil
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public static class ContextUtil
  {
    public static T GetCachedExtension<T>(this IVssRequestContext requestContext, string key)
    {
      object obj = (object) null;
      T cachedExtension = default (T);
      if (!requestContext.Items.TryGetValue(key, out obj))
      {
        using (IDisposableReadOnlyList<T> extensions = requestContext.GetExtensions<T>())
          cachedExtension = extensions != null ? extensions.FirstOrDefault<T>() : default (T);
        requestContext.Items.Add(key, (object) cachedExtension);
      }
      else if (obj != null)
        cachedExtension = (T) obj;
      return cachedExtension;
    }
  }
}
