// Decompiled with JetBrains decompiler
// Type: Nest.CoordinatedRequestDefaults
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  internal static class CoordinatedRequestDefaults
  {
    public static int BulkAllBackOffRetriesDefault = 0;
    public static TimeSpan BulkAllBackOffTimeDefault = TimeSpan.FromMinutes(1.0);
    public static int BulkAllMaxDegreeOfParallelismDefault = 4;
    public static int BulkAllSizeDefault = 1000;
    public static int ReindexBackPressureFactor = 4;
    public static int ReindexScrollSize = 500;
  }
}
