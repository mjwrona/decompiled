// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.WebApiConverters.PipelineConfigurationConverters
// Assembly: Microsoft.Azure.Pipelines.WebApiConverters, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 42DDCCD8-4E0C-44F8-A5D2-AEF894388AED
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.WebApiConverters.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.Azure.Pipelines.WebApiConverters
{
  public static class PipelineConfigurationConverters
  {
    private const string TraceLayer = "PipelineConfigurationConverters";

    public static Microsoft.Azure.Pipelines.WebApi.PipelineConfiguration ToWebApiConfiguration(
      this Microsoft.Azure.Pipelines.Server.ObjectModel.PipelineConfiguration configuration,
      IVssRequestContext requestContext,
      ISecuredObject securedObject)
    {
      ArgumentUtility.CheckForNull<ISecuredObject>(securedObject, nameof (securedObject));
      switch (configuration)
      {
        case null:
          return (Microsoft.Azure.Pipelines.WebApi.PipelineConfiguration) null;
        case Microsoft.Azure.Pipelines.Server.ObjectModel.YamlConfiguration yamlConfiguration:
          Microsoft.Azure.Pipelines.WebApi.YamlConfiguration apiConfiguration = new Microsoft.Azure.Pipelines.WebApi.YamlConfiguration(securedObject)
          {
            Path = yamlConfiguration.Path,
            Repository = yamlConfiguration.Repository.ToWebApiRepository(securedObject)
          };
          foreach (KeyValuePair<string, Microsoft.Azure.Pipelines.Server.ObjectModel.Variable> keyValuePair in yamlConfiguration.Variables.Where<KeyValuePair<string, Microsoft.Azure.Pipelines.Server.ObjectModel.Variable>>((Func<KeyValuePair<string, Microsoft.Azure.Pipelines.Server.ObjectModel.Variable>, bool>) (v => v.Value != null)))
          {
            if (keyValuePair.Value.IsSecret && keyValuePair.Value.Value != null)
              requestContext.TraceConditionally(2000001, TraceLevel.Error, "Pipelines", nameof (PipelineConfigurationConverters), (Func<string>) (() => "A secret variable leaked out to the REST layer! THIS IS BAD! " + new StackTrace().ToString()));
            apiConfiguration.Variables.Add(keyValuePair.Key, new Microsoft.Azure.Pipelines.WebApi.Variable(securedObject)
            {
              IsSecret = keyValuePair.Value.IsSecret,
              Value = keyValuePair.Value.IsSecret ? (string) null : keyValuePair.Value.Value
            });
          }
          return (Microsoft.Azure.Pipelines.WebApi.PipelineConfiguration) apiConfiguration;
        case Microsoft.Azure.Pipelines.Server.ObjectModel.DesignerJsonConfiguration jsonConfiguration:
          return (Microsoft.Azure.Pipelines.WebApi.PipelineConfiguration) new Microsoft.Azure.Pipelines.WebApi.DesignerJsonConfiguration(securedObject)
          {
            DesignerJson = jsonConfiguration.DesignerJson
          };
        default:
          return (Microsoft.Azure.Pipelines.WebApi.PipelineConfiguration) null;
      }
    }
  }
}
