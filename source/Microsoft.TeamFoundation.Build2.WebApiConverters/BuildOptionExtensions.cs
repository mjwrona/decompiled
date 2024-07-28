// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebApiConverters.BuildOptionExtensions
// Assembly: Microsoft.TeamFoundation.Build2.WebApiConverters, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9963E502-0ADF-445A-89CE-AAA11161F2F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebApiConverters.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.TeamFoundation.Build2.WebApiConverters
{
  public static class BuildOptionExtensions
  {
    public static Microsoft.TeamFoundation.Build.WebApi.BuildOption ToWebApiBuildOption(
      this Microsoft.TeamFoundation.Build2.Server.BuildOption srvBuildOption,
      ISecuredObject securedObject)
    {
      ArgumentUtility.CheckForNull<ISecuredObject>(securedObject, nameof (securedObject));
      if (srvBuildOption == null)
        return (Microsoft.TeamFoundation.Build.WebApi.BuildOption) null;
      Microsoft.TeamFoundation.Build.WebApi.BuildOption webApiBuildOption = new Microsoft.TeamFoundation.Build.WebApi.BuildOption(securedObject)
      {
        Inputs = srvBuildOption.Inputs,
        Enabled = srvBuildOption.Enabled
      };
      if (srvBuildOption.Definition != null)
        webApiBuildOption.BuildOptionDefinition = srvBuildOption.Definition.ToWebApiBuildOptionDefinitionReference(securedObject);
      return webApiBuildOption;
    }

    public static Microsoft.TeamFoundation.Build2.Server.BuildOption ToServerBuildOption(
      this Microsoft.TeamFoundation.Build.WebApi.BuildOption webApiBuildOption)
    {
      if (webApiBuildOption == null)
        return (Microsoft.TeamFoundation.Build2.Server.BuildOption) null;
      Microsoft.TeamFoundation.Build2.Server.BuildOption serverBuildOption = new Microsoft.TeamFoundation.Build2.Server.BuildOption()
      {
        Inputs = webApiBuildOption.Inputs,
        Enabled = webApiBuildOption.Enabled
      };
      if (webApiBuildOption.BuildOptionDefinition != null)
        serverBuildOption.Definition = webApiBuildOption.BuildOptionDefinition.ToServerBuildOptionDefinition();
      return serverBuildOption;
    }
  }
}
