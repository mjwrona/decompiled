// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.Models.DeploymentProperties
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Newtonsoft.Json;

namespace Microsoft.Azure.Management.ResourceManager.Models
{
  public class DeploymentProperties
  {
    public DeploymentProperties()
    {
    }

    public DeploymentProperties(
      DeploymentMode mode,
      object template = null,
      TemplateLink templateLink = null,
      object parameters = null,
      ParametersLink parametersLink = null,
      DebugSetting debugSetting = null,
      OnErrorDeployment onErrorDeployment = null)
    {
      this.Template = template;
      this.TemplateLink = templateLink;
      this.Parameters = parameters;
      this.ParametersLink = parametersLink;
      this.Mode = mode;
      this.DebugSetting = debugSetting;
      this.OnErrorDeployment = onErrorDeployment;
    }

    [JsonProperty(PropertyName = "template")]
    public object Template { get; set; }

    [JsonProperty(PropertyName = "templateLink")]
    public TemplateLink TemplateLink { get; set; }

    [JsonProperty(PropertyName = "parameters")]
    public object Parameters { get; set; }

    [JsonProperty(PropertyName = "parametersLink")]
    public ParametersLink ParametersLink { get; set; }

    [JsonProperty(PropertyName = "mode")]
    public DeploymentMode Mode { get; set; }

    [JsonProperty(PropertyName = "debugSetting")]
    public DebugSetting DebugSetting { get; set; }

    [JsonProperty(PropertyName = "onErrorDeployment")]
    public OnErrorDeployment OnErrorDeployment { get; set; }

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
