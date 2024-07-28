// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.Models.DeploymentPropertiesExtended
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Management.ResourceManager.Models
{
  public class DeploymentPropertiesExtended
  {
    public DeploymentPropertiesExtended()
    {
    }

    public DeploymentPropertiesExtended(
      string provisioningState = null,
      string correlationId = null,
      DateTime? timestamp = null,
      string duration = null,
      object outputs = null,
      IList<Provider> providers = null,
      IList<Dependency> dependencies = null,
      object template = null,
      TemplateLink templateLink = null,
      object parameters = null,
      ParametersLink parametersLink = null,
      DeploymentMode? mode = null,
      DebugSetting debugSetting = null,
      OnErrorDeploymentExtended onErrorDeployment = null)
    {
      this.ProvisioningState = provisioningState;
      this.CorrelationId = correlationId;
      this.Timestamp = timestamp;
      this.Duration = duration;
      this.Outputs = outputs;
      this.Providers = providers;
      this.Dependencies = dependencies;
      this.Template = template;
      this.TemplateLink = templateLink;
      this.Parameters = parameters;
      this.ParametersLink = parametersLink;
      this.Mode = mode;
      this.DebugSetting = debugSetting;
      this.OnErrorDeployment = onErrorDeployment;
    }

    [JsonProperty(PropertyName = "provisioningState")]
    public string ProvisioningState { get; private set; }

    [JsonProperty(PropertyName = "correlationId")]
    public string CorrelationId { get; private set; }

    [JsonProperty(PropertyName = "timestamp")]
    public DateTime? Timestamp { get; private set; }

    [JsonProperty(PropertyName = "duration")]
    public string Duration { get; private set; }

    [JsonProperty(PropertyName = "outputs")]
    public object Outputs { get; set; }

    [JsonProperty(PropertyName = "providers")]
    public IList<Provider> Providers { get; set; }

    [JsonProperty(PropertyName = "dependencies")]
    public IList<Dependency> Dependencies { get; set; }

    [JsonProperty(PropertyName = "template")]
    public object Template { get; set; }

    [JsonProperty(PropertyName = "templateLink")]
    public TemplateLink TemplateLink { get; set; }

    [JsonProperty(PropertyName = "parameters")]
    public object Parameters { get; set; }

    [JsonProperty(PropertyName = "parametersLink")]
    public ParametersLink ParametersLink { get; set; }

    [JsonProperty(PropertyName = "mode")]
    public DeploymentMode? Mode { get; set; }

    [JsonProperty(PropertyName = "debugSetting")]
    public DebugSetting DebugSetting { get; set; }

    [JsonProperty(PropertyName = "onErrorDeployment")]
    public OnErrorDeploymentExtended OnErrorDeployment { get; set; }

    public virtual void Validate()
    {
      if (this.TemplateLink != null)
        this.TemplateLink.Validate();
      if (this.ParametersLink == null)
        return;
      this.ParametersLink.Validate();
    }
  }
}
