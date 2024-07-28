// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.FilterAttributeHelper
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class FilterAttributeHelper
  {
    internal virtual string GetHeadersAsString(
      IEnumerable<KeyValuePair<string, IEnumerable<string>>> responseHeaders)
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (KeyValuePair<string, IEnumerable<string>> responseHeader in responseHeaders)
        stringBuilder.AppendFormat("{0}:{1};", (object) responseHeader.Key, (object) responseHeader.Value.Aggregate<string>((Func<string, string, string>) ((currentValues, nextValue) => currentValues + "," + nextValue)));
      return stringBuilder.ToString();
    }

    internal virtual IEnumerable<KeyValuePair<string, IEnumerable<string>>> GetRequestHeaders(
      HttpActionContext actionContext,
      bool excludeAuthHeaders = true)
    {
      IEnumerable<KeyValuePair<string, IEnumerable<string>>> keyValuePairs;
      if (actionContext == null)
      {
        keyValuePairs = (IEnumerable<KeyValuePair<string, IEnumerable<string>>>) null;
      }
      else
      {
        HttpRequestMessage request = actionContext.Request;
        if (request == null)
        {
          keyValuePairs = (IEnumerable<KeyValuePair<string, IEnumerable<string>>>) null;
        }
        else
        {
          HttpRequestHeaders headers = request.Headers;
          keyValuePairs = headers != null ? headers.Cast<KeyValuePair<string, IEnumerable<string>>>() : (IEnumerable<KeyValuePair<string, IEnumerable<string>>>) null;
        }
      }
      if (keyValuePairs == null)
        keyValuePairs = Enumerable.Empty<KeyValuePair<string, IEnumerable<string>>>();
      IEnumerable<KeyValuePair<string, IEnumerable<string>>> source = keyValuePairs;
      if (excludeAuthHeaders)
        source = source.Where<KeyValuePair<string, IEnumerable<string>>>((Func<KeyValuePair<string, IEnumerable<string>>, bool>) (header => !string.Equals(header.Key, "authorization", StringComparison.OrdinalIgnoreCase)));
      return source;
    }

    internal virtual IEnumerable<KeyValuePair<string, IEnumerable<string>>> GetResponseHeaders(
      HttpActionExecutedContext executedContext)
    {
      IEnumerable<KeyValuePair<string, IEnumerable<string>>> keyValuePairs;
      if (executedContext == null)
      {
        keyValuePairs = (IEnumerable<KeyValuePair<string, IEnumerable<string>>>) null;
      }
      else
      {
        HttpResponseMessage response = executedContext.Response;
        if (response == null)
        {
          keyValuePairs = (IEnumerable<KeyValuePair<string, IEnumerable<string>>>) null;
        }
        else
        {
          HttpResponseHeaders headers = response.Headers;
          keyValuePairs = headers != null ? headers.Cast<KeyValuePair<string, IEnumerable<string>>>() : (IEnumerable<KeyValuePair<string, IEnumerable<string>>>) null;
        }
      }
      return keyValuePairs ?? Enumerable.Empty<KeyValuePair<string, IEnumerable<string>>>();
    }

    internal virtual string GetResponseContent(HttpActionExecutedContext executedContext)
    {
      string responseContent = string.Empty;
      if (executedContext.Response != null && executedContext.Response.Content != null)
        responseContent = executedContext.Response.Content.ReadAsStringAsync().Result;
      return responseContent;
    }

    internal virtual string GetRequestContent(HttpActionContext actionContext)
    {
      string requestContent = string.Empty;
      if (actionContext.Request != null && actionContext.Request.Content != null)
        requestContent = actionContext.Request.Content.ReadAsStringAsync().Result;
      if (requestContent == string.Empty && actionContext.ActionArguments != null)
      {
        StringBuilder seed = new StringBuilder();
        actionContext.ActionArguments.Aggregate<KeyValuePair<string, object>, StringBuilder>(seed, (Func<StringBuilder, KeyValuePair<string, object>, StringBuilder>) ((b, a) => b.Append(string.Format("{0}:{{{1}}};", (object) a.Key, a.Value))));
        requestContent = seed.ToString();
      }
      return requestContent;
    }

    internal virtual CommerceControllerBase GetActionContextController(
      HttpActionExecutedContext executedContext)
    {
      CommerceControllerBase contextController = (CommerceControllerBase) null;
      if (executedContext?.ActionContext?.ControllerContext != null)
        contextController = executedContext.ActionContext.ControllerContext.Controller as CommerceControllerBase;
      return contextController;
    }

    internal virtual CommerceControllerBase GetActionContextController(
      HttpActionContext actionContext)
    {
      CommerceControllerBase contextController = (CommerceControllerBase) null;
      if (actionContext?.ControllerContext != null)
        contextController = actionContext.ControllerContext.Controller as CommerceControllerBase;
      return contextController;
    }

    internal virtual string GetActionName(HttpActionExecutedContext executedContext) => this.GetActionName(executedContext.ActionContext);

    internal virtual string GetActionName(HttpActionContext actionContext) => actionContext.ActionDescriptor.ActionName;

    internal virtual string GetControllerName(HttpActionExecutedContext executedContext) => executedContext.ActionContext.ControllerContext.ControllerDescriptor.ControllerName;
  }
}
