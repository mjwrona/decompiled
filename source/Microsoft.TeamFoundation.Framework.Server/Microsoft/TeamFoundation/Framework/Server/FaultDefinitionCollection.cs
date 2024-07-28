// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FaultDefinitionCollection
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FaultDefinitionCollection
  {
    private readonly List<FaultDefinition> m_faultDefinitions;

    public FaultDefinitionCollection() => this.m_faultDefinitions = new List<FaultDefinition>();

    public FaultDefinitionCollection(string faultDefinitionsRaw) => this.m_faultDefinitions = JsonConvert.DeserializeObject<List<FaultDefinition>>(faultDefinitionsRaw);

    public void Add(FaultDefinition fault)
    {
      ArgumentUtility.CheckForNull<FaultDefinition>(fault, nameof (fault));
      this.m_faultDefinitions.Add(fault);
    }

    public void AddRange(IEnumerable<FaultDefinition> faultDefinitions)
    {
      ArgumentUtility.CheckForNull<IEnumerable<FaultDefinition>>(faultDefinitions, nameof (faultDefinitions));
      this.m_faultDefinitions.AddRange(faultDefinitions);
    }

    public FaultDefinition[] GetFaults(string faultPoint, string faultType)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(faultPoint, nameof (faultPoint));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(faultPoint, nameof (faultType));
      return this.m_faultDefinitions.Where<FaultDefinition>((Func<FaultDefinition, bool>) (f => !string.IsNullOrWhiteSpace(f.FaultPoint) && !string.IsNullOrWhiteSpace(f.FaultType) && string.Equals(f.FaultPoint, faultPoint, StringComparison.OrdinalIgnoreCase) && string.Equals(f.FaultType, faultType, StringComparison.OrdinalIgnoreCase))).ToArray<FaultDefinition>();
    }

    public string Serialize() => this.m_faultDefinitions.Serialize<List<FaultDefinition>>(true);
  }
}
