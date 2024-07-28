// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.Common.EndpointVariableResolver
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 762B8E87-3651-4560-BE0D-F9006FB93C96
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.Common.dll

using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.Common
{
  public static class EndpointVariableResolver
  {
    public static int MaxTimeOutInSeconds = 4;
    private const string EndpointSubscriptionId = "endpoint.subscriptionId";

    public static string ResolveVariable(string endpointVariable, ServiceEndpoint serviceEndpoint)
    {
      string variableName = EndpointVariableResolver.GetVariableName(endpointVariable);
      string dataValue = EndpointVariableResolver.GetDataValue(serviceEndpoint, endpointVariable);
      if (dataValue != null)
        return dataValue;
      if (serviceEndpoint.Authorization != null && serviceEndpoint.Authorization.Parameters.ContainsKey(variableName))
        return serviceEndpoint.Authorization.Parameters[variableName];
      throw new ServiceEndpointException(Resources.InvalidEndpointVariable((object) endpointVariable));
    }

    public static IEnumerable<string> GetVariables(string stringWithVariables)
    {
      string pattern = "\\$\\((.*?)\\)";
      try
      {
        return Regex.Matches(stringWithVariables, pattern, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds((double) (EndpointVariableResolver.MaxTimeOutInSeconds * 1000))).Cast<Match>().Select<Match, string>((Func<Match, string>) (match => match.Groups[1].Value));
      }
      catch (RegexMatchTimeoutException ex)
      {
        throw new InvalidEndpointResponseException(Resources.RegexMatchTimeExceeded((object) pattern, (object) (EndpointVariableResolver.MaxTimeOutInSeconds * 1000)));
      }
    }

    public static bool ResolveDataVariable(
      string endpointVariable,
      ServiceEndpoint serviceEndpoint,
      Dictionary<string, string> parameters,
      out string resolvedDataVariableValue,
      ServiceEndpointType endpointType = null,
      bool throwIfInvalidVariable = true)
    {
      if (parameters != null && parameters.ContainsKey(endpointVariable))
      {
        resolvedDataVariableValue = parameters[endpointVariable];
        resolvedDataVariableValue = EndpointVariableResolver.IsSubscriptionIdVariable(endpointVariable) ? resolvedDataVariableValue.ToLower() : resolvedDataVariableValue;
        return true;
      }
      resolvedDataVariableValue = EndpointVariableResolver.GetDataValue(serviceEndpoint, endpointVariable, endpointType, throwIfInvalidVariable);
      if (!string.IsNullOrEmpty(resolvedDataVariableValue))
        return true;
      if (throwIfInvalidVariable)
        throw new ServiceEndpointException(Resources.InvalidEndpointVariable((object) endpointVariable));
      return false;
    }

    private static string GetDataValue(
      ServiceEndpoint serviceEndpoint,
      string variableName,
      ServiceEndpointType endpointType = null,
      bool throwIfInvalidVariable = true)
    {
      string dataVariableName = EndpointVariableResolver.GetVariableName(variableName, throwIfInvalidVariable);
      string dataValue = (string) null;
      InputDescriptor inputDescriptor = endpointType != null ? endpointType.InputDescriptors.FirstOrDefault<InputDescriptor>((Func<InputDescriptor, bool>) (id => string.Equals(id.Id, dataVariableName, StringComparison.OrdinalIgnoreCase))) : (InputDescriptor) null;
      if (inputDescriptor != null && inputDescriptor.IsConfidential)
        return (string) null;
      if (serviceEndpoint.Data.ContainsKey(dataVariableName))
        dataValue = serviceEndpoint.Data[dataVariableName];
      if (string.Equals(dataVariableName, "url", StringComparison.OrdinalIgnoreCase))
        dataValue = serviceEndpoint.Url.AbsoluteUri;
      if (!EndpointVariableResolver.IsSubscriptionIdVariable(variableName))
        return dataValue;
      return dataValue?.ToLower();
    }

    private static string GetVariableName(string variable, bool throwIfInvalidVariable = true)
    {
      string[] strArray = variable.Split('.');
      if (strArray.Length == 2 && string.Equals(strArray[0], "endpoint", StringComparison.OrdinalIgnoreCase))
        return strArray[1];
      if (throwIfInvalidVariable)
        throw new ServiceEndpointException(Resources.InvalidEndpointVariable((object) variable));
      return variable;
    }

    private static bool IsSubscriptionIdVariable(string endpointVariable) => string.Equals(endpointVariable, "endpoint.subscriptionId", StringComparison.OrdinalIgnoreCase);
  }
}
