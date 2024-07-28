// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Routing.ODataValueProviderFactory
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Extensions;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Http.Controllers;
using System.Web.Http.ValueProviders;
using System.Web.Http.ValueProviders.Providers;

namespace Microsoft.AspNet.OData.Routing
{
  internal class ODataValueProviderFactory : ValueProviderFactory, IUriValueProviderFactory
  {
    public override IValueProvider GetValueProvider(HttpActionContext actionContext)
    {
      if (actionContext == null)
        throw Error.ArgumentNull(nameof (actionContext));
      return (IValueProvider) new ODataValueProviderFactory.ODataValueProvider(actionContext.Request.ODataProperties().RoutingConventionsStore);
    }

    private class ODataValueProvider : NameValuePairsValueProvider
    {
      public ODataValueProvider(IDictionary<string, object> routeData)
        : base(routeData, CultureInfo.InvariantCulture)
      {
      }
    }
  }
}
