// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.ODataRoutingAttribute
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Routing;
using System;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ValueProviders;

namespace Microsoft.AspNet.OData
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
  public sealed class ODataRoutingAttribute : Attribute, IControllerConfiguration
  {
    public void Initialize(
      HttpControllerSettings controllerSettings,
      HttpControllerDescriptor controllerDescriptor)
    {
      if (controllerSettings == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (controllerSettings));
      if (controllerDescriptor == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (controllerDescriptor));
      ServicesContainer services = controllerSettings.Services;
      services.Replace(typeof (IHttpActionSelector), (object) new ODataActionSelector(services.GetActionSelector()));
      services.Insert(typeof (ValueProviderFactory), 0, (object) new ODataValueProviderFactory());
    }
  }
}
