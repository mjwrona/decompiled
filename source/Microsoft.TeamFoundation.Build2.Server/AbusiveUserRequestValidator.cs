// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.AbusiveUserRequestValidator
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class AbusiveUserRequestValidator : IBuildRequestValidator
  {
    private static readonly string s_layer = typeof (AbusiveUserRequestValidator).Name;

    public BuildRequestValidationResult ValidateRequest(
      IVssRequestContext requestContext,
      BuildRequestValidationContext validationContext)
    {
      bool flag = false;
      try
      {
        string username;
        validationContext.Build.TriggerInfo.TryGetValue("pr.sender.name", out username);
        if (!string.IsNullOrEmpty(username))
        {
          flag = validationContext.SourceProvider.IsAbusiveUser(requestContext, validationContext.Build.ProjectId, validationContext.Definition.Repository, username);
          if (flag)
            requestContext.TraceError(12030168, AbusiveUserRequestValidator.s_layer, "Blocking BuildNumber {0} requested by sender {1}", (object) validationContext.Build.BuildNumber, (object) username);
        }
        else
        {
          flag = validationContext.SourceProvider.IsRepositoryOwnedByAbusiveUser(requestContext, validationContext.Build.ProjectId, validationContext.Definition.Repository);
          if (flag)
            requestContext.TraceError(12030168, AbusiveUserRequestValidator.s_layer, "Blocking build requested for repository {0} (projectId: {1}, build definition id: {2})", (object) validationContext.Definition.Repository.Url, (object) validationContext.Build.ProjectId, (object) validationContext.Build.Definition.Id);
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12030169, AbusiveUserRequestValidator.s_layer, ex);
      }
      if (flag)
        return new BuildRequestValidationResult()
        {
          Result = ValidationResult.Error,
          Message = BuildServerResources.AbusiveUserRequestError()
        };
      return new BuildRequestValidationResult()
      {
        Result = ValidationResult.OK,
        Message = string.Empty
      };
    }
  }
}
