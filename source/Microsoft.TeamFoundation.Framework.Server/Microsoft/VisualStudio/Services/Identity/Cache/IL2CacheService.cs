// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Cache.IL2CacheService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Identity.Cache
{
  [DefaultServiceImplementation(typeof (SecureKeyL2Cache))]
  public interface IL2CacheService : IVssFrameworkService
  {
    bool TrySet<T>(IVssRequestContext context, T value, out string key);

    bool TrySet<T>(IVssRequestContext context, string key, T value);

    bool TryGet<T>(IVssRequestContext context, string key, out T value);

    void Invalidate<T>(IVssRequestContext context, string key);
  }
}
