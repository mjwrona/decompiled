// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Adapters.WebApiAssembliesResolver
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Interfaces;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Http.Dispatcher;

namespace Microsoft.AspNet.OData.Adapters
{
  internal class WebApiAssembliesResolver : IWebApiAssembliesResolver
  {
    private IAssembliesResolver innerResolver;
    public static IWebApiAssembliesResolver Default = (IWebApiAssembliesResolver) new WebApiAssembliesResolver();

    public WebApiAssembliesResolver() => this.innerResolver = (IAssembliesResolver) new DefaultAssembliesResolver();

    public WebApiAssembliesResolver(IAssembliesResolver resolver) => this.innerResolver = resolver != null ? resolver : throw Error.ArgumentNull(nameof (resolver));

    public IEnumerable<Assembly> Assemblies => (IEnumerable<Assembly>) this.innerResolver.GetAssemblies();
  }
}
