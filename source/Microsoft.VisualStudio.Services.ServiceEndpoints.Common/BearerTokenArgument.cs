// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.Common.BearerTokenArgument
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 762B8E87-3651-4560-BE0D-F9006FB93C96
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.Common.dll

using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.Common
{
  public class BearerTokenArgument
  {
    public string Method;
    public string ContentType;
    public bool IgnoreError;
    public static int MaxBodySizeSupported = 102400;

    public string UserName { set; private get; }

    public string Password { set; private get; }

    public JObject Body { set; private get; }

    public string ResultSelector { set; private get; }

    public string AuthServerUrl { set; private get; }

    public BearerTokenArgument()
    {
      this.UserName = "endpoint.username";
      this.Password = "endpoint.password";
      this.ResultSelector = "jsonpath:$.*";
      this.Method = "get";
      this.Body = new JObject();
      this.ContentType = "application/json";
      this.AuthServerUrl = "endpoint.url";
      this.IgnoreError = false;
    }

    public string GetUserName(ServiceEndpoint serviceEndpoint) => this.ExpandEndpointVariable(this.UserName, serviceEndpoint);

    public string GetPassword(ServiceEndpoint serviceEndpoint) => this.ExpandEndpointVariable(this.Password, serviceEndpoint);

    public string GetAuthServerUrl(ServiceEndpoint serviceEndpoint) => this.ExpandEndpointVariables(this.AuthServerUrl, serviceEndpoint, new Func<string, ServiceEndpoint, string>(this.ExpandEndpointDataVariable));

    public string GetResultSelector() => this.ResultSelector.StartsWith("jsonpath:") ? this.ResultSelector.Replace("jsonpath:", "") : throw new ArgumentException(Resources.InvalidBrokerTokenArgument((object) "ResultSelector"));

    public string GetBody(ServiceEndpoint serviceEndpoint) => this.ExpandEndpointVariables(this.Body.ToString(), serviceEndpoint, new Func<string, ServiceEndpoint, string>(this.ExpandJsonEscapedEndpointVariable));

    private string ExpandEndpointVariables(
      string str,
      ServiceEndpoint serviceEndpoint,
      Func<string, ServiceEndpoint, string> expandVariableHelperFunc)
    {
      return new Regex("endpoint.[a-z,A-Z,0-9,_,-]*").Replace(str, (MatchEvaluator) (m => expandVariableHelperFunc(m.Value, serviceEndpoint)));
    }

    private string ExpandEndpointVariable(string str, ServiceEndpoint serviceEndpoint) => str.StartsWith("endpoint.", StringComparison.OrdinalIgnoreCase) ? EndpointVariableResolver.ResolveVariable(str, serviceEndpoint) : str;

    private string ExpandJsonEscapedEndpointVariable(string str, ServiceEndpoint serviceEndpoint)
    {
      if (!str.StartsWith("endpoint.", StringComparison.OrdinalIgnoreCase))
        return str;
      string str1 = JsonConvert.ToString(EndpointVariableResolver.ResolveVariable(str, serviceEndpoint));
      return str1.Substring(1, str1.Length - 2);
    }

    private string ExpandEndpointDataVariable(string str, ServiceEndpoint serviceEndpoint)
    {
      string resolvedDataVariableValue;
      return str.StartsWith("endpoint.", StringComparison.OrdinalIgnoreCase) && EndpointVariableResolver.ResolveDataVariable(str, serviceEndpoint, (Dictionary<string, string>) null, out resolvedDataVariableValue, throwIfInvalidVariable: false) ? resolvedDataVariableValue : str;
    }
  }
}
