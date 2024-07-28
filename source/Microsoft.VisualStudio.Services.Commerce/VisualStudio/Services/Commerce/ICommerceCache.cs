// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.ICommerceCache
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [DefaultServiceImplementation(typeof (CommerceCache))]
  internal interface ICommerceCache : IVssFrameworkService
  {
    bool TrySet<T>(IVssRequestContext context, string key, T value);

    bool TryGet<T>(IVssRequestContext context, string key, out T value);

    void Invalidate<T>(IVssRequestContext context, string key);

    void SetupCache(Guid cacheNamespace, string featureFlagName, TimeSpan? cacheExpiration);
  }
}
