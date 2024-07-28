// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Common.AzureStorageConstants
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.Content.Server.Common
{
  public static class AzureStorageConstants
  {
    public const int CloudBlobPageBoundedCapacity = 2;
    public const int MaxEntitiesPerTableResultSegment = 1000;
    public const int DefaultNumEntitiesPerTableResultSegment = 10;
    public const int MaxEntitiesPerBlobResultSegment = 5000;
    public static readonly TimeSpan MaxExpectedClockSkew = TimeSpan.FromHours(1.0);
  }
}
