// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.DataDriven.DataDrivenConsumerConfig
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Resources;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.DataDriven
{
  public class DataDrivenConsumerConfig
  {
    private const string c_jsonConfigResourceNameSuffix = ".json";
    private const string c_resourceManagerNameSuffix = "Resources";
    private const string c_translationsResourceNameSuffix = "Resources.resources";

    public string Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string ImageUrl { get; set; }

    public string InformationUrl { get; set; }

    public InputDescriptor[] InputDescriptors { get; set; }

    public ExternalConfigurationDescriptor ExternalConfiguration { get; set; }

    public AuthenticationType AuthenticationType { get; set; }

    public DataDrivenConsumerActionConfig[] Actions { get; set; }

    public static DataDrivenConsumerConfig CreateFromConsumerType(Type consumerType)
    {
      string name = consumerType.FullName + ".json";
      using (StreamReader streamReader = new StreamReader(consumerType.Assembly.GetManifestResourceStream(name)))
        return DataDrivenConsumerConfig.CreateFromJsonString(streamReader.ReadToEnd(), DataDrivenConsumerConfig.GetResourceManagerForConsumer(consumerType));
    }

    public static DataDrivenConsumerConfig CreateFromJsonString(
      string jsonRepresentation,
      ResourceManager resourceManager = null)
    {
      DataDrivenConsumerConfig fromJsonString = JsonConvert.DeserializeObject<DataDrivenConsumerConfig>(jsonRepresentation, CommonConsumerSettings.JsonSerializerSettings);
      DataDrivenConsumerConfig.SetConsumerDataInActions(fromJsonString);
      DataDrivenConsumerConfig.ApplyTranslations(fromJsonString, resourceManager);
      return fromJsonString;
    }

    private static void SetConsumerDataInActions(DataDrivenConsumerConfig config)
    {
      if (config.Actions == null)
        return;
      foreach (DataDrivenConsumerActionConfig action in config.Actions)
      {
        action.ConsumerId = config.Id;
        action.ConsumerInputDescriptors = config.InputDescriptors;
      }
    }

    private static void ApplyTranslations(
      DataDrivenConsumerConfig consumerConfig,
      ResourceManager resourceManager)
    {
      DataDrivenConsumerResourceManager dataDrivenResources = new DataDrivenConsumerResourceManager(resourceManager);
      consumerConfig.Name = dataDrivenResources.ConsumerName(consumerConfig);
      consumerConfig.Description = dataDrivenResources.ConsumerDescription(consumerConfig);
      DataDrivenConsumerConfig.ApplyTranslationsToConsumerInputs(consumerConfig, dataDrivenResources);
      DataDrivenConsumerConfig.ApplyTranslationsToConsumerActions(consumerConfig, dataDrivenResources);
    }

    private static void ApplyTranslationsToConsumerActions(
      DataDrivenConsumerConfig consumerConfig,
      DataDrivenConsumerResourceManager dataDrivenResources)
    {
      if (consumerConfig.Actions == null)
        return;
      foreach (DataDrivenConsumerActionConfig action in consumerConfig.Actions)
        DataDrivenConsumerConfig.ApplyTranslationsToConsumerAction(action, dataDrivenResources);
    }

    private static void ApplyTranslationsToConsumerAction(
      DataDrivenConsumerActionConfig actionConfig,
      DataDrivenConsumerResourceManager dataDrivenResources)
    {
      actionConfig.Name = dataDrivenResources.ActionName(actionConfig);
      actionConfig.Description = dataDrivenResources.ActionDescription(actionConfig);
      DataDrivenConsumerConfig.ApplyTranslationsToActionInputs(actionConfig, dataDrivenResources);
    }

    private static void ApplyTranslationsToActionInputs(
      DataDrivenConsumerActionConfig actionConfig,
      DataDrivenConsumerResourceManager dataDrivenResources)
    {
      if (actionConfig.InputDescriptors == null)
        return;
      foreach (InputDescriptor inputDescriptor in actionConfig.InputDescriptors)
      {
        inputDescriptor.Name = dataDrivenResources.ActionInputName(actionConfig, inputDescriptor);
        inputDescriptor.Description = dataDrivenResources.ActionInputDescription(actionConfig, inputDescriptor);
      }
    }

    private static void ApplyTranslationsToConsumerInputs(
      DataDrivenConsumerConfig consumerConfig,
      DataDrivenConsumerResourceManager dataDrivenResources)
    {
      if (consumerConfig.InputDescriptors == null)
        return;
      foreach (InputDescriptor inputDescriptor in consumerConfig.InputDescriptors)
      {
        inputDescriptor.Name = dataDrivenResources.ConsumerInputName(consumerConfig, inputDescriptor);
        inputDescriptor.Description = dataDrivenResources.ConsumerInputDescription(consumerConfig, inputDescriptor);
      }
    }

    private static ResourceManager GetResourceManagerForConsumer(Type consumerType)
    {
      string resourceName = consumerType.FullName + "Resources.resources";
      ResourceManager managerForConsumer = (ResourceManager) null;
      if (consumerType.Assembly.GetManifestResourceInfo(resourceName) != null)
        managerForConsumer = new ResourceManager(consumerType.FullName + "Resources", consumerType.Assembly);
      return managerForConsumer;
    }
  }
}
