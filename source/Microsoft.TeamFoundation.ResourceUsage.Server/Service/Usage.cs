// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ResourceUsage.Server.Service.Usage
// Assembly: Microsoft.TeamFoundation.ResourceUsage.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55568389-A340-4F60-8DD1-887E0E3F1980
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.ResourceUsage.Server.dll

namespace Microsoft.TeamFoundation.ResourceUsage.Server.Service
{
  public struct Usage
  {
    public Usage(int count, int limit)
    {
      this.Count = count;
      this.Limit = limit;
    }

    public int Count { get; }

    public int Limit { get; }
  }
}
