// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Partitioning.PartitioningResourceIds
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Partitioning
{
  [GenerateAllConstants(null)]
  public static class PartitioningResourceIds
  {
    public const string AreaName = "Partitioning";
    public const string AreaId = "{0129E64E-3F98-43F8-9073-212C19D832CB}";
    public const string PartitionContainersResource = "Containers";
    public static readonly Guid PartitionContainers = new Guid("{55FDD96F-CBFE-461A-B0AC-890454FF434A}");
    public const string PartitionsResource = "Partitions";
    public static readonly Guid Partitions = new Guid("{4ECE3A4B-1D02-4313-8843-DD7B02C8F639}");
  }
}
