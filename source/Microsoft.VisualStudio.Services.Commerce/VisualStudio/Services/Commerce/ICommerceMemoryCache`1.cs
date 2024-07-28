// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.ICommerceMemoryCache`1
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal interface ICommerceMemoryCache<T> where T : class
  {
    bool TryGetValue(IVssRequestContext requestContext, string key, out T value);

    void Set(IVssRequestContext requestContext, string key, T value);

    bool Remove(IVssRequestContext requestContext, string key);

    bool IsEnabled(IVssRequestContext requestContext, string featureFlag);
  }
}
