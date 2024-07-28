// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.BaseDeploymentInput
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts
{
  [KnownType(typeof (ServerDeploymentInput))]
  [KnownType(typeof (AgentDeploymentInput))]
  [KnownType(typeof (MachineGroupDeploymentInput))]
  [KnownType(typeof (GatesDeploymentInput))]
  [DataContract]
  public abstract class BaseDeploymentInput : 
    ReleaseManagementSecuredObject,
    IEquatable<BaseDeploymentInput>
  {
    private string _condition;

    public JObject ToJObject() => JObject.FromObject((object) this);

    protected BaseDeploymentInput() => this.Setup(0, 1, (string) null, (IDictionary<string, string>) null);

    protected BaseDeploymentInput(int timeoutInMinutes) => this.Setup(timeoutInMinutes, 1, (string) null, (IDictionary<string, string>) null);

    protected BaseDeploymentInput(int timeoutInMinutes, int jobCancelTimeoutInMinutes) => this.Setup(timeoutInMinutes, jobCancelTimeoutInMinutes, (string) null, (IDictionary<string, string>) null);

    protected BaseDeploymentInput(
      int timeoutInMinutes,
      int jobCancelTimeoutInMinutes,
      string condition,
      IDictionary<string, string> overrideInputs)
    {
      this.Setup(timeoutInMinutes, jobCancelTimeoutInMinutes, condition, overrideInputs);
    }

    private void Setup(
      int timeoutInMinutes,
      int jobCancelTimeoutInMinutes,
      string condition,
      IDictionary<string, string> overrideInputs)
    {
      this.TimeoutInMinutes = timeoutInMinutes;
      this.JobCancelTimeoutInMinutes = jobCancelTimeoutInMinutes;
      this.Condition = condition;
      this.OverrideInputs = overrideInputs != null ? (IDictionary<string, string>) new Dictionary<string, string>(overrideInputs, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) : (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    [DataMember]
    public int TimeoutInMinutes { get; set; }

    [DataMember]
    public int JobCancelTimeoutInMinutes { get; set; }

    [Obsolete]
    public bool ShareOutputVariables { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Condition
    {
      get => !string.IsNullOrWhiteSpace(this._condition) ? this._condition : "succeeded()";
      set => this._condition = value;
    }

    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter needed for deserialization")]
    [DataMember(EmitDefaultValue = false)]
    public IDictionary<string, string> OverrideInputs { get; set; }

    public virtual bool Equals(BaseDeploymentInput other)
    {
      BaseDeploymentInput baseDeploymentInput = other;
      if (baseDeploymentInput == null || this.TimeoutInMinutes != baseDeploymentInput.TimeoutInMinutes || !(this.Condition ?? string.Empty).Equals(baseDeploymentInput.Condition ?? string.Empty))
        return false;
      if (this.OverrideInputs == null && baseDeploymentInput.OverrideInputs == null)
        return true;
      if (this.OverrideInputs == null)
        return baseDeploymentInput.OverrideInputs.Count == 0;
      if (baseDeploymentInput.OverrideInputs == null)
        return this.OverrideInputs.Count == 0;
      if (this.OverrideInputs.Count != baseDeploymentInput.OverrideInputs.Count)
        return false;
      foreach (string key in (IEnumerable<string>) this.OverrideInputs.Keys)
      {
        if (!baseDeploymentInput.OverrideInputs.ContainsKey(key) || !string.Equals(this.OverrideInputs[key], baseDeploymentInput.OverrideInputs[key], StringComparison.OrdinalIgnoreCase))
          return false;
      }
      return true;
    }

    public abstract BaseDeploymentInput Clone();
  }
}
