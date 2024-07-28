// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.Common.EndpointStringResolver
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 762B8E87-3651-4560-BE0D-F9006FB93C96
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.Common.dll

using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.Common
{
  public class EndpointStringResolver
  {
    private readonly JToken replacementContext;

    public EndpointStringResolver(JToken replacementContext) => this.replacementContext = replacementContext;

    public JToken ResolveVariablesInDollarFormat(
      ServiceEndpoint serviceEndpoint,
      JToken token,
      Dictionary<string, string> parameters,
      ServiceEndpointType endpointType = null)
    {
      Dictionary<JTokenType, Func<JToken, JToken>> mapFuncs = new Dictionary<JTokenType, Func<JToken, JToken>>()
      {
        {
          JTokenType.String,
          (Func<JToken, JToken>) (t => (JToken) this.ResolveVariablesInDollarFormat(serviceEndpoint, t.ToString(), parameters, endpointType, false))
        }
      };
      return token.Map(mapFuncs);
    }

    public string ResolveVariablesInDollarFormat(
      ServiceEndpoint serviceEndpoint,
      string dataSourceUrl,
      Dictionary<string, string> parameters,
      ServiceEndpointType endpointType = null,
      bool throwIfInvalidVariable = true)
    {
      string str = dataSourceUrl;
      foreach (string variable in EndpointVariableResolver.GetVariables(dataSourceUrl))
      {
        string oldValue = "$(" + variable + ")";
        string resolvedDataVariableValue = (string) null;
        if (EndpointVariableResolver.ResolveDataVariable(variable, serviceEndpoint, parameters, out resolvedDataVariableValue, endpointType, throwIfInvalidVariable))
          str = str.Replace(oldValue, resolvedDataVariableValue);
      }
      return str;
    }

    public string ResolveVariablesInDollarFormatUsingDataAndSecret(
      ServiceEndpoint serviceEndpoint,
      string stringWithVariable)
    {
      foreach (string variable in EndpointVariableResolver.GetVariables(stringWithVariable))
      {
        string oldValue = "$(" + variable + ")";
        string newValue = EndpointVariableResolver.ResolveVariable(variable, serviceEndpoint);
        stringWithVariable = stringWithVariable.Replace(oldValue, newValue);
      }
      return stringWithVariable;
    }

    public string ResolveVariablesInMustacheFormat(string template) => new MustacheTemplateEngine().EvaluateTemplate(template, this.replacementContext);
  }
}
