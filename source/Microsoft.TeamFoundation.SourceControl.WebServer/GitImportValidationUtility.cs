// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitImportValidationUtility
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  internal static class GitImportValidationUtility
  {
    internal static bool ShouldSkipValidation(
      IVssRequestContext requestContext,
      HttpRequestMessage request)
    {
      string str = requestContext.GetService<IVssRegistryService>().GetValue<string>(requestContext, (RegistryQuery) "/Service/Git/Settings/Import/HostHeadersWhichBypassValidation", "Sonoma (CI)");
      if (request.Headers.UserAgent == null)
        return false;
      return ((IEnumerable<string>) str.Split(';')).Any<string>((Func<string, bool>) (x => request.Headers.UserAgent.ToString().ToLower().Contains(x.ToLower())));
    }

    internal static bool ValidateImportParams(
      IVssRequestContext requestContext,
      ImportRepositoryValidation remoteRepository,
      string traceArea,
      ClientTraceData ctData,
      out string errorMessage)
    {
      errorMessage = (string) null;
      return remoteRepository.GitSource != null ? GitToGitImportValidator.ValidateGitToGitImportParams(requestContext, remoteRepository, traceArea, ctData) : TfvcToGitImportValidator.ValidateTfvcToGitImportParams(requestContext, remoteRepository, traceArea, ctData, out errorMessage);
    }
  }
}
