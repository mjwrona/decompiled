// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.Extensions.WorkItemPipelineHelper
// Assembly: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AEA81E2B-AB47-44C0-8043-66C0E1018997
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.dll

using Microsoft.VisualStudio.Services.DevSecOps.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.Extensions
{
  public class WorkItemPipelineHelper : PipelineHelper
  {
    public override bool IsUserDefined(Violation violation) => true;

    public override string ComposeErrorMessage(
      IEnumerable<string> credentialLocations,
      bool usePrescriptiveBlockMessage = false)
    {
      string str = string.Empty;
      if (credentialLocations != null && credentialLocations.Count<string>() > 0)
        str = string.Join(", ", (IEnumerable<string>) credentialLocations.OrderBy<string, string>((Func<string, string>) (l => l)));
      return DevSecOpsSdkResources.BlockWorkItemSaveWhenSecretsAreFound((object) str);
    }
  }
}
