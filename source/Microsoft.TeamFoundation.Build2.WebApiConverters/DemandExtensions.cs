// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebApiConverters.DemandExtensions
// Assembly: Microsoft.TeamFoundation.Build2.WebApiConverters, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9963E502-0ADF-445A-89CE-AAA11161F2F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebApiConverters.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.TeamFoundation.Build2.WebApiConverters
{
  public static class DemandExtensions
  {
    public static Microsoft.TeamFoundation.Build.WebApi.Demand ToWebApiDemand(
      this Microsoft.TeamFoundation.Build2.Server.Demand srvDemand,
      ISecuredObject securedObject)
    {
      ArgumentUtility.CheckForNull<ISecuredObject>(securedObject, nameof (securedObject));
      if (srvDemand == null)
        return (Microsoft.TeamFoundation.Build.WebApi.Demand) null;
      return !(srvDemand is Microsoft.TeamFoundation.Build2.Server.DemandEquals) ? (Microsoft.TeamFoundation.Build.WebApi.Demand) new Microsoft.TeamFoundation.Build.WebApi.DemandExists(srvDemand.Name, securedObject) : (Microsoft.TeamFoundation.Build.WebApi.Demand) new Microsoft.TeamFoundation.Build.WebApi.DemandEquals(srvDemand.Name, srvDemand.Value, securedObject);
    }

    public static Microsoft.TeamFoundation.Build2.Server.Demand ToServerDemand(
      this Microsoft.TeamFoundation.Build.WebApi.Demand webApiDemand)
    {
      if (webApiDemand == null)
        return (Microsoft.TeamFoundation.Build2.Server.Demand) null;
      return webApiDemand is Microsoft.TeamFoundation.Build.WebApi.DemandEquals ? (Microsoft.TeamFoundation.Build2.Server.Demand) new Microsoft.TeamFoundation.Build2.Server.DemandEquals(webApiDemand.Name, webApiDemand.Value) : (Microsoft.TeamFoundation.Build2.Server.Demand) new Microsoft.TeamFoundation.Build2.Server.DemandExists(webApiDemand.Name);
    }
  }
}
