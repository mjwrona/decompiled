// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionSourceProvider
// Assembly: Microsoft.TeamFoundation.Build.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 97B7A530-2EF1-42C1-8A2A-360BCF05C7EF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.WebApi.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [DataContract]
  public sealed class BuildDefinitionSourceProvider
  {
    public BuildDefinitionSourceProvider()
    {
      this.Fields = new Dictionary<string, string>();
      this.SupportedTriggerTypes = DefinitionTriggerType.All;
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DefinitionTriggerType SupportedTriggerTypes { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Dictionary<string, string> Fields { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    internal int Id { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    internal string DefinitionUri { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    internal DateTime LastModified { get; set; }
  }
}
