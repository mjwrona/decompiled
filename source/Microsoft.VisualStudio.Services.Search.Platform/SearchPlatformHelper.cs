// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Search.Platform.Common.SearchPlatformHelper
// Assembly: Microsoft.VisualStudio.Services.Search.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5EF41D26-5C57-4D41-AC16-607E07E24DBC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Search.Platform.Common
{
  public static class SearchPlatformHelper
  {
    public static ISearchCiLoggerService GetSearchCiLoggerService(IVssRequestContext requestContext) => requestContext.GetService<ISearchCiLoggerService>();

    public static ISearchClientTraceLoggerService GetSearchClientTraceLoggerService(
      IVssRequestContext requestContext)
    {
      return requestContext.GetService<ISearchClientTraceLoggerService>();
    }

    public static ISearchKpiLoggerService GetSearchKpiLoggerService(
      IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return (vssRequestContext == null ? requestContext : vssRequestContext.Elevate()).GetService<ISearchKpiLoggerService>();
    }

    public static Guid GetHostId(IVssRequestContext requestContext) => requestContext?.ServiceHost != null ? requestContext.ServiceHost.InstanceId : Guid.Empty;

    public static IDisposableReadOnlyList<T> GetExtensions<T>(
      IVssRequestContext requestContext,
      bool throwIfNotFound = true)
    {
      IDisposableReadOnlyList<T> extensions = requestContext.GetExtensions<T>();
      if (((extensions == null ? 1 : (extensions.Count == 0 ? 1 : 0)) & (throwIfNotFound ? 1 : 0)) != 0)
        throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("Could not find any export for type '{0}'", (object) typeof (T))));
      return extensions ?? (IDisposableReadOnlyList<T>) new DisposableCollection<T>((IReadOnlyList<T>) new List<T>());
    }

    public static T GetExtension<T>(IVssRequestContext requestContext, bool throwIfNotFound = true)
    {
      T extension = requestContext.GetExtension<T>();
      return !((object) extension == null & throwIfNotFound) ? extension : throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("Could not find any export for type '{0}'", (object) typeof (T))));
    }
  }
}
