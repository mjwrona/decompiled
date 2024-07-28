// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IVssRegistryService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.RegistryService.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DefaultServiceImplementation(typeof (CachedRegistryService), typeof (VirtualCachedRegistryService))]
  public interface IVssRegistryService : IVssFrameworkService
  {
    IEnumerable<RegistryItem> Read(IVssRequestContext requestContext, in RegistryQuery query);

    IEnumerable<IEnumerable<RegistryItem>> Read(
      IVssRequestContext requestContext,
      IEnumerable<RegistryQuery> queries);

    void Write(IVssRequestContext requestContext, IEnumerable<RegistryItem> items);

    IVssRegistryService GetParent(IVssRequestContext requestContext);

    void RegisterNotification(
      IVssRequestContext requestContext,
      RegistrySettingsChangedCallback callback,
      bool fallThru,
      IEnumerable<RegistryQuery> filters,
      Guid serviceHostId = default (Guid));

    void UnregisterNotification(
      IVssRequestContext requestContext,
      RegistrySettingsChangedCallback callback);
  }
}
