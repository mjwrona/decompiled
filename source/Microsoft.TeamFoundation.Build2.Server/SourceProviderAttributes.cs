// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.SourceProviderAttributes
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class SourceProviderAttributes
  {
    public string Name { get; set; }

    public IList<SupportedTrigger> SupportedTriggers { get; set; }

    public IDictionary<string, bool> SupportedCapabilities { get; set; }

    public SourceProviderAvailability Availability { get; set; }

    public bool IsExternal { get; set; }

    public bool SupportsSourceLinks
    {
      get
      {
        bool flag;
        return ((this.SupportedCapabilities == null ? 0 : (this.SupportedCapabilities.TryGetValue("sourceLinks", out flag) ? 1 : 0)) & (flag ? 1 : 0)) != 0;
      }
    }

    public bool SupportsYamlDefinition
    {
      get
      {
        bool flag;
        return ((this.SupportedCapabilities == null ? 0 : (this.SupportedCapabilities.TryGetValue("yamlDefinition", out flag) ? 1 : 0)) & (flag ? 1 : 0)) != 0;
      }
    }

    public bool ProtectYamlEdit
    {
      get
      {
        bool flag;
        return ((this.SupportedCapabilities == null ? 0 : (this.SupportedCapabilities.TryGetValue("protectYamlEdit", out flag) ? 1 : 0)) & (flag ? 1 : 0)) != 0;
      }
    }

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
