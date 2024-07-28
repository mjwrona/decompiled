// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.DataDriven.DataDrivenConsumerResourceManager
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.VisualStudio.Services.FormInput;
using System.Resources;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.DataDriven
{
  public class DataDrivenConsumerResourceManager
  {
    private const string c_consumerNameKeyFormat = "{0}_ConsumerName";
    private const string c_consumerDescriptionKeyFormat = "{0}_ConsumerDescription";
    private const string c_consumerInputNameKeyFormat = "{0}_{1}_ConsumerInputName";
    private const string c_consumerInputDescriptionKeyFormat = "{0}_{1}_ConsumerInputDescription";
    private const string c_actionNameKeyFormat = "{0}_{1}_ActionName";
    private const string c_actionDescriptionKeyFormat = "{0}_{1}_ActionDescription";
    private const string c_actionInputNameKeyFormat = "{0}_{1}_{2}_ActionInputName";
    private const string c_actionInputDescriptionKeyFormat = "{0}_{1}_{2}_ActionInputDescription";
    private readonly ResourceManager m_resourceManager;
    private static ResourceManager s_defaultResourceManager = DataDrivenConsumerResources.ResourceManager;
    private static Regex s_notAllowedCharsInResourceNameRegex = new Regex("[^a-zA-Z0-9_]", RegexOptions.Compiled);

    public DataDrivenConsumerResourceManager()
      : this((ResourceManager) null)
    {
    }

    public DataDrivenConsumerResourceManager(ResourceManager resourceManager)
    {
      this.m_resourceManager = resourceManager ?? DataDrivenConsumerResourceManager.s_defaultResourceManager;
      this.m_resourceManager.IgnoreCase = true;
    }

    public string ConsumerName(DataDrivenConsumerConfig consumerConfig) => this.GetString(string.Format("{0}_ConsumerName", (object) consumerConfig.Id), consumerConfig.Name);

    public string ConsumerDescription(DataDrivenConsumerConfig consumerConfig) => this.GetString(string.Format("{0}_ConsumerDescription", (object) consumerConfig.Id), consumerConfig.Description);

    public string ConsumerInputName(
      DataDrivenConsumerConfig consumerConfig,
      InputDescriptor inputDescriptor)
    {
      return this.GetString(string.Format("{0}_{1}_ConsumerInputName", (object) consumerConfig.Id, (object) inputDescriptor.Id), inputDescriptor.Name);
    }

    public string ConsumerInputDescription(
      DataDrivenConsumerConfig consumerConfig,
      InputDescriptor inputDescriptor)
    {
      return this.GetString(string.Format("{0}_{1}_ConsumerInputDescription", (object) consumerConfig.Id, (object) inputDescriptor.Id), inputDescriptor.Description);
    }

    public string ActionName(DataDrivenConsumerActionConfig actionConfig) => this.GetString(string.Format("{0}_{1}_ActionName", (object) actionConfig.ConsumerId, (object) actionConfig.Id), actionConfig.Name);

    public string ActionDescription(DataDrivenConsumerActionConfig actionConfig) => this.GetString(string.Format("{0}_{1}_ActionDescription", (object) actionConfig.ConsumerId, (object) actionConfig.Id), actionConfig.Description);

    public string ActionInputName(
      DataDrivenConsumerActionConfig actionConfig,
      InputDescriptor inputDescriptor)
    {
      return this.GetString(string.Format("{0}_{1}_{2}_ActionInputName", (object) actionConfig.ConsumerId, (object) actionConfig.Id, (object) inputDescriptor.Id), inputDescriptor.Name);
    }

    public string ActionInputDescription(
      DataDrivenConsumerActionConfig actionConfig,
      InputDescriptor inputDescriptor)
    {
      return this.GetString(string.Format("{0}_{1}_{2}_ActionInputDescription", (object) actionConfig.ConsumerId, (object) actionConfig.Id, (object) inputDescriptor.Id), inputDescriptor.Description);
    }

    private string GetString(string resourceKey, string defaultValue) => this.m_resourceManager.GetString(DataDrivenConsumerResourceManager.SanitizeIdentifier(resourceKey)) ?? defaultValue;

    private static string SanitizeIdentifier(string identifier) => DataDrivenConsumerResourceManager.s_notAllowedCharsInResourceNameRegex.Replace(identifier, string.Empty);
  }
}
