// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.BufferPoolStatsPerfCounters
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal static class BufferPoolStatsPerfCounters
  {
    private const string UriBase = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_BufferPool";
    internal const string Allocated = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_BufferPoolCurrentCount";
    internal const string InUse = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_BufferPoolCurrentlyUsed";
    internal const string Unpooled = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_BufferPoolCurrentlyUsedUnpooled";
  }
}
