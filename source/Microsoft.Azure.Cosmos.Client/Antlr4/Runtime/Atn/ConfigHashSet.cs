// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Atn.ConfigHashSet
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Sharpen;
using System.Collections.Generic;

namespace Antlr4.Runtime.Atn
{
  internal class ConfigHashSet : Dictionary<ATNConfig, ATNConfig>
  {
    public ConfigHashSet(IEqualityComparer<ATNConfig> comparer)
      : base(comparer)
    {
    }

    public ConfigHashSet()
      : base((IEqualityComparer<ATNConfig>) new ConfigEqualityComparator())
    {
    }

    public ATNConfig GetOrAdd(ATNConfig config)
    {
      ATNConfig orAdd;
      if (this.TryGetValue(config, out orAdd))
        return orAdd;
      this.Put<ATNConfig, ATNConfig>(config, config);
      return config;
    }
  }
}
