// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services.InputValuesDictionaryExtension
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Build;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services
{
  public static class InputValuesDictionaryExtension
  {
    public static InputValue GetDefaultVersionLatestWithBuildDefinitionBranchAndTagsValue(
      this IDictionary<string, string> inputValues,
      IVssRequestContext requestContext,
      IList<InputValue> versions)
    {
      if (inputValues == null)
        throw new ArgumentNullException(nameof (inputValues));
      if (versions == null)
        throw new ArgumentNullException(nameof (versions));
      string branchFilter = string.Empty;
      if (inputValues.HasDefinitions())
      {
        Guid result1;
        if (!Guid.TryParse(ArtifactTypeBase.GetSourceInput(inputValues, "project"), out result1))
          result1 = Guid.Empty;
        int result2;
        if (!int.TryParse(inputValues.GetDefinitionIdStrings().First<string>(), out result2))
          result2 = 0;
        branchFilter = BuildArtifact.GetDefaultBranchForBuildDefinition(requestContext, result1, result2);
      }
      return inputValues.GetVersionWithBranchAndTagsValue(versions, branchFilter);
    }
  }
}
