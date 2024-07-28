// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Common.SubscriptionInputDescriptorExtensions
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E36C8A02-D97F-45E0-9F96-E7385D8CA092
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FormInput;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Common
{
  public static class SubscriptionInputDescriptorExtensions
  {
    public static void ValidateInternal(
      this InputDescriptor inputDescriptor,
      IVssRequestContext requestContext,
      string inputValue,
      string scope = null)
    {
      inputDescriptor.Validate(inputValue, scope);
      if (inputDescriptor.Validation == null || inputDescriptor.Validation.DataType != InputDataType.Uri)
        return;
      requestContext.GetService<IUrlAddressIpValidatorService>().VerifyUrlIsAllowedIPAddress(requestContext, inputValue, false);
    }
  }
}
