// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.MultipleAgentExecutionOptions
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build2.Server
{
  [DataContract]
  public class MultipleAgentExecutionOptions : AgentTargetExecutionOptions
  {
    public MultipleAgentExecutionOptions()
      : base(2)
    {
      this.MaxConcurrency = 1;
    }

    [DataMember(EmitDefaultValue = true)]
    [DefaultValue(1)]
    public int MaxConcurrency { get; set; }

    [DataMember(EmitDefaultValue = true)]
    public bool ContinueOnError { get; set; }
  }
}
