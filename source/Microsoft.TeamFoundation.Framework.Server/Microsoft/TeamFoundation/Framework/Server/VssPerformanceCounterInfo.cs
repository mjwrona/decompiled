// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VssPerformanceCounterInfo
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Diagnostics.PerformanceData;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class VssPerformanceCounterInfo
  {
    public VssPerformanceCounterInfo(int id, string name, CounterType type)
    {
      this.Id = id;
      this.Name = name;
      this.Type = type;
    }

    public VssPerformanceCounterInfo(int id, string name, CounterType type, string uri)
    {
      this.Id = id;
      this.Name = name;
      this.Type = type;
      this.Uri = uri;
    }

    public int Id { get; private set; }

    public string Name { get; private set; }

    public CounterType Type { get; private set; }

    public string Uri { get; private set; }
  }
}
