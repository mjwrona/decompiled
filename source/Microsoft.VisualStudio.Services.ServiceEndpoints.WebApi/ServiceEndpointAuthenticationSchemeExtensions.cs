// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointAuthenticationSchemeExtensions
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52155B17-64DE-4C30-B15E-F2E70DBED717
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.dll

using Microsoft.VisualStudio.Services.FormInput;
using System;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi
{
  public static class ServiceEndpointAuthenticationSchemeExtensions
  {
    public static bool TryGetInputDescriptor(
      this ServiceEndpointAuthenticationScheme authScheme,
      string parameterKey,
      out InputDescriptor inputDescriptor)
    {
      inputDescriptor = (InputDescriptor) null;
      if (authScheme != null)
        inputDescriptor = authScheme.InputDescriptors.FirstOrDefault<InputDescriptor>((Func<InputDescriptor, bool>) (d => string.Equals(d.Id, parameterKey, StringComparison.OrdinalIgnoreCase)));
      return inputDescriptor != null;
    }

    public static bool IsOauth2(
      this ServiceEndpointAuthenticationScheme authScheme)
    {
      return authScheme != null && authScheme.Scheme.Equals("OAuth2", StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsOauth(
      this ServiceEndpointAuthenticationScheme authScheme)
    {
      return authScheme != null && authScheme.Scheme.Equals("OAuth", StringComparison.OrdinalIgnoreCase);
    }
  }
}
