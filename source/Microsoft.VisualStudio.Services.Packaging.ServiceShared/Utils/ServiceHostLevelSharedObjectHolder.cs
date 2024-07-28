// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils.ServiceHostLevelSharedObjectHolder
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Concurrent;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils
{
  public static class ServiceHostLevelSharedObjectHolder
  {
    public static T Get<T>(IVssRequestContext requestContext, Guid key, Func<Guid, T> factory) where T : class => requestContext.GetService<ServiceHostLevelSharedObjectHolder.ServiceHostObjectHolderService>().GetOrAdd<T>(key, factory);

    private class ServiceHostObjectHolderService : IVssFrameworkService
    {
      private readonly ConcurrentDictionary<Guid, object> dictionary = new ConcurrentDictionary<Guid, object>();

      void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
      {
      }

      void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
      {
      }

      public T GetOrAdd<T>(Guid key, Func<Guid, T> factory) where T : class => (T) this.dictionary.GetOrAdd(key, (Func<Guid, object>) factory);
    }
  }
}
