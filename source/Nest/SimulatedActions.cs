// Decompiled with JetBrains decompiler
// Type: Nest.SimulatedActions
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;

namespace Nest
{
  [JsonFormatter(typeof (SimulatedActionsFormatter))]
  public class SimulatedActions
  {
    private SimulatedActions()
    {
    }

    public IEnumerable<string> Actions { get; private set; }

    public static SimulatedActions All => new SimulatedActions()
    {
      UseAll = true
    };

    public bool UseAll { get; private set; }

    public static SimulatedActions Some(params string[] actions) => new SimulatedActions()
    {
      Actions = (IEnumerable<string>) actions
    };

    public static SimulatedActions Some(IEnumerable<string> actions) => new SimulatedActions()
    {
      Actions = actions
    };
  }
}
