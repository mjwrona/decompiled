// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.Models.DeploymentWhatIfProperties
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Newtonsoft.Json;

namespace Microsoft.Azure.Management.ResourceManager.Models
{
  public class DeploymentWhatIfProperties : DeploymentProperties
  {
    public DeploymentWhatIfProperties()
    {
    }

    public DeploymentWhatIfProperties(
      DeploymentMode mode,
      object template = null,
      TemplateLink templateLink = null,
      object parameters = null,
      ParametersLink parametersLink = null,
      DebugSetting debugSetting = null,
      OnErrorDeployment onErrorDeployment = null,
      DeploymentWhatIfSettings whatIfSettings = null)
      : base(mode, template, templateLink, parameters, parametersLink, debugSetting, onErrorDeployment)
    {
      this.WhatIfSettings = whatIfSettings;
    }

    [JsonProperty(PropertyName = "whatIfSettings")]
    public DeploymentWhatIfSettings WhatIfSettings { get; set; }

    public override void Validate() => base.Validate();
  }
}
