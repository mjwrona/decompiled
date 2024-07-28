// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointTypeExtensions
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52155B17-64DE-4C30-B15E-F2E70DBED717
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.dll

using Microsoft.VisualStudio.Services.FormInput;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi
{
  public static class ServiceEndpointTypeExtensions
  {
    public static bool HasAuthenticationSchemes(this ServiceEndpointType endpointType) => endpointType?.AuthenticationSchemes != null && endpointType.AuthenticationSchemes.Count > 0;

    public static bool TryGetAuthInputDescriptors(
      this ServiceEndpointType endpointType,
      string authScheme,
      out List<InputDescriptor> inputDescriptors)
    {
      inputDescriptors = (List<InputDescriptor>) null;
      if (endpointType.HasAuthenticationSchemes())
      {
        ServiceEndpointAuthenticationScheme authenticationScheme = endpointType.AuthenticationSchemes.FirstOrDefault<ServiceEndpointAuthenticationScheme>((Func<ServiceEndpointAuthenticationScheme, bool>) (auth => string.Compare(auth.Scheme, authScheme, StringComparison.OrdinalIgnoreCase) == 0));
        inputDescriptors = authenticationScheme?.InputDescriptors;
      }
      return inputDescriptors != null;
    }

    public static bool TryGetInputDescriptor(
      this ServiceEndpointType endpointType,
      string parameterKey,
      out InputDescriptor inputDescriptor)
    {
      inputDescriptor = (InputDescriptor) null;
      if (endpointType != null)
        inputDescriptor = endpointType.InputDescriptors.FirstOrDefault<InputDescriptor>((Func<InputDescriptor, bool>) (d => string.Equals(d.Id, parameterKey, StringComparison.OrdinalIgnoreCase)));
      return inputDescriptor != null;
    }
  }
}
