// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils.TracingUtils
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Utils;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils
{
  public static class TracingUtils
  {
    public const string NoFeed = "NoFeed";

    public static int GetTracePointFor(Guid? feedId, IProtocol protocol, IHasher<uint> hasher)
    {
      string input = feedId?.ToString() ?? "NoFeed";
      IntegerRange integerRange = protocol?.TracePointRange ?? new IntegerRange(20000000, 20999998);
      return (int) ((long) ((ulong) hasher.Hash(input) % (ulong) integerRange.RangeLength) + (long) integerRange.StartInclusive);
    }
  }
}
