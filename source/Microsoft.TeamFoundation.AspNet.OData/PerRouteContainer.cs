// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.PerRouteContainer
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using System;
using System.Collections.Concurrent;
using System.Web.Http;

namespace Microsoft.AspNet.OData
{
  public class PerRouteContainer : PerRouteContainerBase
  {
    private const string RootContainerMappingsKey = "Microsoft.AspNet.OData.RootContainerMappingsKey";
    private readonly HttpConfiguration configuration;

    public PerRouteContainer(HttpConfiguration configuration) => this.configuration = configuration;

    protected override IServiceProvider GetContainer(string routeName)
    {
      if (string.IsNullOrEmpty(routeName))
        return this.configuration.GetNonODataRootContainer();
      IServiceProvider container;
      if (this.GetRootContainerMappings().TryGetValue(routeName, out container))
        return container;
      throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.NullContainer);
    }

    protected override void SetContainer(string routeName, IServiceProvider rootContainer)
    {
      if (rootContainer == null)
        throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.NullContainer);
      if (string.IsNullOrEmpty(routeName))
        this.configuration.SetNonODataRootContainer(rootContainer);
      else
        this.GetRootContainerMappings()[routeName] = rootContainer;
    }

    private ConcurrentDictionary<string, IServiceProvider> GetRootContainerMappings() => (ConcurrentDictionary<string, IServiceProvider>) this.configuration.Properties.GetOrAdd((object) "Microsoft.AspNet.OData.RootContainerMappingsKey", (Func<object, object>) (key => (object) new ConcurrentDictionary<string, IServiceProvider>()));
  }
}
