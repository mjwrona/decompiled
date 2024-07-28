// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildDefinitionVariable
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build2.Server
{
  [DataContract]
  public class BuildDefinitionVariable
  {
    public BuildDefinitionVariable()
    {
    }

    private BuildDefinitionVariable(BuildDefinitionVariable variableToClone)
    {
      this.Value = variableToClone.Value;
      this.AllowOverride = variableToClone.AllowOverride;
      this.IsSecret = variableToClone.IsSecret;
    }

    [DataMember(EmitDefaultValue = true)]
    public string Value { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool AllowOverride { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool IsSecret { get; set; }

    public BuildDefinitionVariable Clone() => new BuildDefinitionVariable(this);
  }
}
