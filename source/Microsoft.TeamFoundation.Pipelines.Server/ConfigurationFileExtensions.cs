// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.ConfigurationFileExtensions
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  public static class ConfigurationFileExtensions
  {
    public static Microsoft.TeamFoundation.Pipelines.WebApi.ConfigurationFile ToWebApiConfigurationFile(
      this ConfigurationFile configurationFile)
    {
      ArgumentUtility.CheckForNull<ConfigurationFile>(configurationFile, nameof (configurationFile));
      return new Microsoft.TeamFoundation.Pipelines.WebApi.ConfigurationFile()
      {
        Content = configurationFile.Content,
        Path = configurationFile.Path,
        IsBase64Encoded = configurationFile.IsBase64Encoded
      };
    }
  }
}
