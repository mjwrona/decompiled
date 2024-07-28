// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.Models.AzurePowerShellScript
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Microsoft.Rest;
using Microsoft.Rest.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Management.ResourceManager.Models
{
  [JsonObject("AzurePowerShell")]
  [JsonTransformation]
  public class AzurePowerShellScript : DeploymentScript
  {
    public AzurePowerShellScript()
    {
    }

    public AzurePowerShellScript(
      ManagedServiceIdentity identity,
      string location,
      TimeSpan retentionInterval,
      string azPowerShellVersion,
      string id = null,
      string name = null,
      string type = null,
      IDictionary<string, string> tags = null,
      string cleanupPreference = null,
      string provisioningState = null,
      ScriptStatus status = null,
      IDictionary<string, object> outputs = null,
      string primaryScriptUri = null,
      IList<string> supportingScriptUris = null,
      string scriptContent = null,
      string arguments = null,
      IList<EnvironmentVariable> environmentVariables = null,
      string forceUpdateTag = null,
      TimeSpan? timeout = null)
      : base(identity, location, id, name, type, tags)
    {
      this.CleanupPreference = cleanupPreference;
      this.ProvisioningState = provisioningState;
      this.Status = status;
      this.Outputs = outputs;
      this.PrimaryScriptUri = primaryScriptUri;
      this.SupportingScriptUris = supportingScriptUris;
      this.ScriptContent = scriptContent;
      this.Arguments = arguments;
      this.EnvironmentVariables = environmentVariables;
      this.ForceUpdateTag = forceUpdateTag;
      this.RetentionInterval = retentionInterval;
      this.Timeout = timeout;
      this.AzPowerShellVersion = azPowerShellVersion;
    }

    [JsonProperty(PropertyName = "properties.cleanupPreference")]
    public string CleanupPreference { get; set; }

    [JsonProperty(PropertyName = "properties.provisioningState")]
    public string ProvisioningState { get; private set; }

    [JsonProperty(PropertyName = "properties.status")]
    public ScriptStatus Status { get; private set; }

    [JsonProperty(PropertyName = "properties.outputs")]
    public IDictionary<string, object> Outputs { get; private set; }

    [JsonProperty(PropertyName = "properties.primaryScriptUri")]
    public string PrimaryScriptUri { get; set; }

    [JsonProperty(PropertyName = "properties.supportingScriptUris")]
    public IList<string> SupportingScriptUris { get; set; }

    [JsonProperty(PropertyName = "properties.scriptContent")]
    public string ScriptContent { get; set; }

    [JsonProperty(PropertyName = "properties.arguments")]
    public string Arguments { get; set; }

    [JsonProperty(PropertyName = "properties.environmentVariables")]
    public IList<EnvironmentVariable> EnvironmentVariables { get; set; }

    [JsonProperty(PropertyName = "properties.forceUpdateTag")]
    public string ForceUpdateTag { get; set; }

    [JsonProperty(PropertyName = "properties.retentionInterval")]
    public TimeSpan RetentionInterval { get; set; }

    [JsonProperty(PropertyName = "properties.timeout")]
    public TimeSpan? Timeout { get; set; }

    [JsonProperty(PropertyName = "properties.azPowerShellVersion")]
    public string AzPowerShellVersion { get; set; }

    public override void Validate()
    {
      base.Validate();
      if (this.AzPowerShellVersion == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "AzPowerShellVersion");
      if (this.ScriptContent != null && this.ScriptContent.Length > 32000)
        throw new ValidationException(ValidationRules.MaxLength, "ScriptContent", (object) 32000);
      if (this.EnvironmentVariables == null)
        return;
      foreach (EnvironmentVariable environmentVariable in (IEnumerable<EnvironmentVariable>) this.EnvironmentVariables)
        environmentVariable?.Validate();
    }
  }
}
