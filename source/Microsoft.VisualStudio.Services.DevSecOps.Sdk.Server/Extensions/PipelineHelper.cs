// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.Extensions.PipelineHelper
// Assembly: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AEA81E2B-AB47-44C0-8043-66C0E1018997
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.dll

using Microsoft.VisualStudio.Services.DevSecOps.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.Extensions
{
  public abstract class PipelineHelper
  {
    public abstract bool IsUserDefined(Violation violation);

    public abstract string ComposeErrorMessage(
      IEnumerable<string> credentialLocations,
      bool usePrescriptiveBlockMessage = false);

    protected internal string JoinCredentialLocations(
      IEnumerable<string> credentialLocations,
      int maxErrorsToInclude = 3)
    {
      StringBuilder stringBuilder = new StringBuilder();
      int num = 1;
      bool flag = false;
      foreach (string str in (IEnumerable<string>) credentialLocations.OrderBy<string, string>((Func<string, string>) (l => l)))
      {
        if (num > maxErrorsToInclude)
        {
          stringBuilder.Append("\r\n...");
          break;
        }
        if (flag)
          stringBuilder.Append("\r\n");
        stringBuilder.Append(str);
        flag = true;
        ++num;
      }
      return stringBuilder.ToString();
    }
  }
}
