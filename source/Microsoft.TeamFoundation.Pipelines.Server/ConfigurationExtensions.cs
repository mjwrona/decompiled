// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.ConfigurationExtensions
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  public static class ConfigurationExtensions
  {
    public static Microsoft.TeamFoundation.Pipelines.WebApi.Configuration ToWebApiConfiguration(
      this Configuration configuration)
    {
      ArgumentUtility.CheckForNull<Configuration>(configuration, nameof (configuration));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return new Microsoft.TeamFoundation.Pipelines.WebApi.Configuration()
      {
        Files = (IList<Microsoft.TeamFoundation.Pipelines.WebApi.ConfigurationFile>) configuration.Files.Select<ConfigurationFile, Microsoft.TeamFoundation.Pipelines.WebApi.ConfigurationFile>(ConfigurationExtensions.\u003C\u003EO.\u003C0\u003E__ToWebApiConfigurationFile ?? (ConfigurationExtensions.\u003C\u003EO.\u003C0\u003E__ToWebApiConfigurationFile = new Func<ConfigurationFile, Microsoft.TeamFoundation.Pipelines.WebApi.ConfigurationFile>(ConfigurationFileExtensions.ToWebApiConfigurationFile))).ToList<Microsoft.TeamFoundation.Pipelines.WebApi.ConfigurationFile>()
      };
    }
  }
}
