// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.MigratingOperationSettings
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public class MigratingOperationSettings
  {
    public bool PerStorageAccountAndPrefix_Randomness { get; set; } = true;

    public int PerStorageAccountAndPrefix_MaxParallelism { get; set; } = 32;

    public int CopyInternalQueue_MaxParallelism { get; set; } = 8;

    public int SyncDelete_OuterParallelism { get; set; } = 32;

    public int SyncDelete_InnerParallelism { get; set; } = 8;

    public int ParallelismForPrefixes { get; set; } = 1;

    public int InternalActionBlockCapacity { get; set; } = 5000;

    public int ExponentOfTwoForPartitionKeyCheckpoint { get; set; } = 17;

    public int BatchSizeForTableCopyAsync { get; set; } = 90;
  }
}
