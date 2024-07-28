// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebApiConverters.BuildRequestValidationResultExtensions
// Assembly: Microsoft.TeamFoundation.Build2.WebApiConverters, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9963E502-0ADF-445A-89CE-AAA11161F2F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebApiConverters.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.TeamFoundation.Build2.WebApiConverters
{
  public static class BuildRequestValidationResultExtensions
  {
    public static Microsoft.TeamFoundation.Build.WebApi.BuildRequestValidationResult ToWebApiBuildRequestValidationResult(
      this Microsoft.TeamFoundation.Build2.Server.BuildRequestValidationResult srvBuildRequestValidationResult,
      ISecuredObject securedObject)
    {
      ArgumentUtility.CheckForNull<ISecuredObject>(securedObject, nameof (securedObject));
      if (srvBuildRequestValidationResult == null)
        return (Microsoft.TeamFoundation.Build.WebApi.BuildRequestValidationResult) null;
      return new Microsoft.TeamFoundation.Build.WebApi.BuildRequestValidationResult(securedObject)
      {
        Result = (Microsoft.TeamFoundation.Build.WebApi.ValidationResult) srvBuildRequestValidationResult.Result,
        Message = srvBuildRequestValidationResult.Message
      };
    }

    public static Microsoft.TeamFoundation.Build2.Server.BuildRequestValidationResult ToServerBuildRequestValidationResult(
      this Microsoft.TeamFoundation.Build.WebApi.BuildRequestValidationResult webApiBuildRequestValidationResult)
    {
      if (webApiBuildRequestValidationResult == null)
        return (Microsoft.TeamFoundation.Build2.Server.BuildRequestValidationResult) null;
      return new Microsoft.TeamFoundation.Build2.Server.BuildRequestValidationResult()
      {
        Result = (Microsoft.TeamFoundation.Build2.Server.ValidationResult) webApiBuildRequestValidationResult.Result,
        Message = webApiBuildRequestValidationResult.Message
      };
    }
  }
}
