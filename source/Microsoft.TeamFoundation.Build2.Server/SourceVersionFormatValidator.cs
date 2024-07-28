// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.SourceVersionFormatValidator
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class SourceVersionFormatValidator : IBuildRequestValidator
  {
    public BuildRequestValidationResult ValidateRequest(
      IVssRequestContext requestContext,
      BuildRequestValidationContext validationContext)
    {
      if (validationContext.Definition.Repository != null && validationContext.SourceProvider != null)
      {
        string errorMessage;
        if (!validationContext.SourceProvider.IsSourceVersionValid(requestContext, validationContext.Build.SourceVersion, out errorMessage))
          return new BuildRequestValidationResult()
          {
            Result = ValidationResult.Error,
            Message = errorMessage
          };
        if (!validationContext.SourceProvider.IsSourceBranchValid(requestContext, validationContext.Build.SourceBranch, out errorMessage))
          return new BuildRequestValidationResult()
          {
            Result = ValidationResult.Error,
            Message = errorMessage
          };
      }
      return new BuildRequestValidationResult()
      {
        Result = ValidationResult.OK
      };
    }
  }
}
