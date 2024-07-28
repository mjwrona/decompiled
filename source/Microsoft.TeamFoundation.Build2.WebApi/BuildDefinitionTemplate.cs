// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionTemplate
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [DataContract]
  public class BuildDefinitionTemplate
  {
    [DataMember(EmitDefaultValue = false, Name = "Icons")]
    private Dictionary<string, string> m_icons;

    public BuildDefinitionTemplate() => this.Category = "Custom";

    [DataMember(IsRequired = true)]
    public string Id { get; set; }

    [DataMember(IsRequired = true)]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = true)]
    public bool CanDelete { get; set; }

    [DataMember(EmitDefaultValue = true)]
    public string Category { get; set; }

    [DataMember(EmitDefaultValue = true)]
    public string DefaultHostedQueue { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid IconTaskId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Description { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public BuildDefinition Template { get; set; }

    public IDictionary<string, string> Icons
    {
      get
      {
        if (this.m_icons == null)
          this.m_icons = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.Ordinal);
        return (IDictionary<string, string>) this.m_icons;
      }
      internal set => this.m_icons = new Dictionary<string, string>(value, (IEqualityComparer<string>) StringComparer.Ordinal);
    }
  }
}
