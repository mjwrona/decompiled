// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.SourceProviderAttributes
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [DataContract]
  public class SourceProviderAttributes
  {
    [DataMember]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IList<SupportedTrigger> SupportedTriggers { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IDictionary<string, bool> SupportedCapabilities { get; set; }

    [IgnoreDataMember]
    public SourceProviderAvailability Availability { get; set; }

    [IgnoreDataMember]
    public bool IsExternal { get; set; }

    [IgnoreDataMember]
    public bool SupportsSourceLinks
    {
      get
      {
        bool flag;
        return ((this.SupportedCapabilities == null ? 0 : (this.SupportedCapabilities.TryGetValue("sourceLinks", out flag) ? 1 : 0)) & (flag ? 1 : 0)) != 0;
      }
    }

    [IgnoreDataMember]
    public bool SupportsYamlDefinition
    {
      get
      {
        bool flag;
        return ((this.SupportedCapabilities == null ? 0 : (this.SupportedCapabilities.TryGetValue("yamlDefinition", out flag) ? 1 : 0)) & (flag ? 1 : 0)) != 0;
      }
    }

    [IgnoreDataMember]
    public DefinitionTriggerType SupportedTriggerTypes
    {
      get
      {
        IList<SupportedTrigger> supportedTriggers = this.SupportedTriggers;
        return supportedTriggers == null ? DefinitionTriggerType.None : supportedTriggers.Select<SupportedTrigger, DefinitionTriggerType>((Func<SupportedTrigger, DefinitionTriggerType>) (t => t.Type)).Aggregate<DefinitionTriggerType, DefinitionTriggerType>(DefinitionTriggerType.None, (Func<DefinitionTriggerType, DefinitionTriggerType, DefinitionTriggerType>) ((x, y) => x | y));
      }
    }
  }
}
