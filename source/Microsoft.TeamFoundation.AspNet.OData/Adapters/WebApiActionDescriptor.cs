// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Adapters.WebApiActionDescriptor
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http.Controllers;

namespace Microsoft.AspNet.OData.Adapters
{
  internal class WebApiActionDescriptor : IWebApiActionDescriptor
  {
    private IList<ODataRequestMethod> supportedHttpMethods;
    private HttpActionDescriptor innerDescriptor;

    public WebApiActionDescriptor(HttpActionDescriptor actionDescriptor)
    {
      this.innerDescriptor = actionDescriptor != null ? actionDescriptor : throw Error.ArgumentNull(nameof (actionDescriptor));
      if (actionDescriptor.SupportedHttpMethods == null)
        return;
      this.supportedHttpMethods = (IList<ODataRequestMethod>) new List<ODataRequestMethod>();
      foreach (HttpMethod supportedHttpMethod in actionDescriptor.SupportedHttpMethods)
      {
        bool ignoreCase = true;
        ODataRequestMethod result = ODataRequestMethod.Unknown;
        if (Enum.TryParse<ODataRequestMethod>(supportedHttpMethod.Method, ignoreCase, out result))
          this.supportedHttpMethods.Add(result);
      }
    }

    public string ControllerName => this.innerDescriptor.ControllerDescriptor == null ? (string) null : this.innerDescriptor.ControllerDescriptor.ControllerName;

    public string ActionName => this.innerDescriptor.ActionName;

    public IEnumerable<T> GetCustomAttributes<T>(bool inherit) where T : Attribute => (IEnumerable<T>) this.innerDescriptor.GetCustomAttributes<T>(inherit);

    public bool IsHttpMethodSupported(ODataRequestMethod method) => this.supportedHttpMethods == null || this.supportedHttpMethods.Contains(method);
  }
}
