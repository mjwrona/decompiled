// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionExtensions
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  public static class BuildDefinitionExtensions
  {
    public static T GetProcess<T>(this BuildDefinition definition) where T : BuildProcess
    {
      ArgumentUtility.CheckForNull<BuildDefinition>(definition, nameof (definition));
      ArgumentUtility.CheckForNull<BuildProcess>(definition.Process, "Process");
      ArgumentUtility.CheckType<T>((object) definition.Process, "Process", nameof (T));
      return definition.Process as T;
    }
  }
}
