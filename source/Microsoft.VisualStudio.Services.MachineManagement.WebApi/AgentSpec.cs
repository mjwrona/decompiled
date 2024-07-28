// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.AgentSpec
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.MachineManagement.WebApi
{
  public class AgentSpec : ICloneable
  {
    public AgentSpec()
    {
    }

    private AgentSpec(AgentSpec cloneFrom)
    {
      this.SpecName = cloneFrom.SpecName;
      this.AgentSpecId = cloneFrom.AgentSpecId;
      this.ImageLabel = cloneFrom.ImageLabel;
      this.IsVisible = cloneFrom.IsVisible;
      this.Size = cloneFrom.Size;
    }

    public int AgentSpecId { get; set; }

    public string ImageLabel { get; set; }

    public bool IsVisible { get; set; }

    public string SpecName { get; set; }

    public string Size { get; set; }

    public override string ToString() => "SpecName: " + this.SpecName + " ImageLabel: " + this.ImageLabel + " Size: " + this.Size;

    public AgentSpec Clone() => new AgentSpec(this);

    object ICloneable.Clone() => (object) this.Clone();

    public override bool Equals(object obj)
    {
      AgentSpec agentSpec = obj as AgentSpec;
      if (!(agentSpec != (AgentSpec) null) || !string.Equals(agentSpec.SpecName, this.SpecName, StringComparison.OrdinalIgnoreCase) || !string.Equals(agentSpec.ImageLabel, this.ImageLabel, StringComparison.OrdinalIgnoreCase))
        return false;
      if (string.Equals(agentSpec.Size, this.Size))
        return true;
      return string.IsNullOrEmpty(agentSpec.Size) && string.IsNullOrEmpty(this.Size);
    }

    public static bool operator ==(AgentSpec lhs, AgentSpec rhs)
    {
      if ((object) lhs != null)
        return lhs.Equals((object) rhs);
      return (object) rhs == null;
    }

    public static bool operator !=(AgentSpec lhs, AgentSpec rhs) => !(lhs == rhs);

    public override int GetHashCode() => (this.SpecName ?? this.ImageLabel ?? "").GetHashCode();
  }
}
