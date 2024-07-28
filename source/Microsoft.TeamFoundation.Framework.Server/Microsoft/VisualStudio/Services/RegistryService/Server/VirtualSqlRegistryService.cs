// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.RegistryService.Server.VirtualSqlRegistryService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.RegistryService.Server
{
  public sealed class VirtualSqlRegistryService : 
    ISqlRegistryService,
    IVssRegistryService,
    IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public IEnumerable<RegistryItem> Read(IVssRequestContext requestContext, in RegistryQuery query) => (IEnumerable<RegistryItem>) RegistryItem.EmptyArray;

    public IEnumerable<IEnumerable<RegistryItem>> Read(
      IVssRequestContext requestContext,
      IEnumerable<RegistryQuery> queries)
    {
      return (IEnumerable<IEnumerable<RegistryItem>>) queries.Select<RegistryQuery, RegistryItem[]>((Func<RegistryQuery, RegistryItem[]>) (s => RegistryItem.EmptyArray));
    }

    public void Write(IVssRequestContext requestContext, IEnumerable<RegistryItem> items) => throw new VirtualServiceHostException();

    public IVssRegistryService GetParent(IVssRequestContext requestContext) => (IVssRegistryService) requestContext.GetParentService<ISqlRegistryService>();

    public void RegisterNotification(
      IVssRequestContext requestContext,
      RegistrySettingsChangedCallback callback,
      bool fallThru,
      IEnumerable<RegistryQuery> filters,
      Guid serviceHostId = default (Guid))
    {
      throw new NotImplementedException();
    }

    public void UnregisterNotification(
      IVssRequestContext requestContext,
      RegistrySettingsChangedCallback callback)
    {
      throw new NotImplementedException();
    }

    IEnumerable<RegistryItem> IVssRegistryService.Read(
      IVssRequestContext requestContext,
      in RegistryQuery query)
    {
      return this.Read(requestContext, in query);
    }
  }
}
