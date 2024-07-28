// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildRequestValidatorProvider
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public static class BuildRequestValidatorProvider
  {
    public static IEnumerable<IBuildRequestValidator> GetDefaultValidators()
    {
      yield return (IBuildRequestValidator) new DemandsRequestValidator();
      yield return (IBuildRequestValidator) new AbusiveUserRequestValidator();
      yield return (IBuildRequestValidator) new VariablesRequestValidator();
    }

    public static IEnumerable<IBuildRequestValidator> GetValidators(
      BuildRequestValidationOptions options)
    {
      yield return (IBuildRequestValidator) new DemandsRequestValidator(options.WarnIfNoMatchingAgent, options.RequireOnlineAgent);
      yield return (IBuildRequestValidator) new AbusiveUserRequestValidator();
      yield return (IBuildRequestValidator) new VariablesRequestValidator(options.InternalRuntimeVariables);
      if (options.ValidateSourceVersionFormat)
        yield return (IBuildRequestValidator) new SourceVersionFormatValidator();
    }
  }
}
