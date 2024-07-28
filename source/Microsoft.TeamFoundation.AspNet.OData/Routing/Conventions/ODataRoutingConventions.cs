// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Routing.Conventions.ODataRoutingConventions
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.AspNet.OData.Routing.Conventions
{
  public static class ODataRoutingConventions
  {
    public static IList<IODataRoutingConvention> CreateDefaultWithAttributeRouting(
      string routeName,
      HttpConfiguration configuration)
    {
      if (configuration == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (configuration));
      if (routeName == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (routeName));
      IList<IODataRoutingConvention> attributeRouting = ODataRoutingConventions.CreateDefault();
      attributeRouting.Insert(0, (IODataRoutingConvention) new AttributeRoutingConvention(routeName, configuration));
      return attributeRouting;
    }

    public static IList<IODataRoutingConvention> CreateDefault() => (IList<IODataRoutingConvention>) new List<IODataRoutingConvention>()
    {
      (IODataRoutingConvention) new MetadataRoutingConvention(),
      (IODataRoutingConvention) new EntitySetRoutingConvention(),
      (IODataRoutingConvention) new SingletonRoutingConvention(),
      (IODataRoutingConvention) new EntityRoutingConvention(),
      (IODataRoutingConvention) new NavigationRoutingConvention(),
      (IODataRoutingConvention) new PropertyRoutingConvention(),
      (IODataRoutingConvention) new DynamicPropertyRoutingConvention(),
      (IODataRoutingConvention) new RefRoutingConvention(),
      (IODataRoutingConvention) new ActionRoutingConvention(),
      (IODataRoutingConvention) new FunctionRoutingConvention(),
      (IODataRoutingConvention) new UnmappedRequestRoutingConvention()
    };
  }
}
