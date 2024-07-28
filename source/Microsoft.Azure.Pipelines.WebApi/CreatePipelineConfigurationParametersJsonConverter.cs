// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.WebApi.CreatePipelineConfigurationParametersJsonConverter
// Assembly: Microsoft.Azure.Pipelines.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9955A178-37CB-46CB-B455-32EA2A66C5BA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.WebApi.dll

using System;

namespace Microsoft.Azure.Pipelines.WebApi
{
  public class CreatePipelineConfigurationParametersJsonConverter : 
    ConfigurationTypeJsonConverter<CreatePipelineConfigurationParameters>
  {
    protected override CreatePipelineConfigurationParameters Create(Type objectType) => objectType == typeof (CreateYamlPipelineConfigurationParameters) ? (CreatePipelineConfigurationParameters) new CreateYamlPipelineConfigurationParameters() : (CreatePipelineConfigurationParameters) null;

    protected override CreatePipelineConfigurationParameters Create(ConfigurationType type) => type == ConfigurationType.Yaml ? (CreatePipelineConfigurationParameters) new CreateYamlPipelineConfigurationParameters() : (CreatePipelineConfigurationParameters) new UnknownCreatePipelineConfigurationParameters(type);
  }
}
