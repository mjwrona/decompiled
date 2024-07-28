// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Egress.EgressConstants
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Egress
{
  public static class EgressConstants
  {
    public const int LowerLimitForDelayAfterTask = 30;
    public const int UpperLimitForDelayAfterTask = 60;
    public const string TargetVersionNumber = "1.0";
    public const string EgressOperationType = "GetBlob";
    public const string EgressLogsContainerName = "$logs";
    public const int EgressComputeTraceId = 5701998;
    public const int WorkerLifeTimeInMins = 690;
    public static short DefaultLookBackWindowInHrs = 2;
    public static short DefaultWindowSize = 1;
    public static TimeSpan DefaultLookBackWindowSize = TimeSpan.FromHours((double) EgressConstants.DefaultLookBackWindowInHrs);
    public static readonly string RegistryEgressComputeParseParallel = "/Configuration/BlobStore/EgressComputeJob/EgressParseParallel";
    public static readonly string RegistryEgressComputeLookbackWindowInHrs = "/Configuration/BlobStore/EgressComputeJob/EgressLookbackWindowInHrs";
    public static readonly Guid EgressComputeJobId = Guid.Parse("cb21d50a-ca06-442b-8a10-cfec0273c764");

    public enum LogBlobType
    {
      Read,
      Write,
      Delete,
    }

    public enum BillableRequestStatus
    {
      Success,
      AnonymousSuccess,
      SasSuccess,
      ClientTimeoutError,
      AnonymousClientTimeoutError,
      SasClientTimeoutError,
      ClientOtherError,
      SasClientOtherError,
      AnonymousClientOtherError,
      AuthorizationError,
      SasAuthorizationError,
      NetworkError,
      AnonymousNetworkError,
      SasNetworkError,
    }

    public enum AzureBlobStorageLogs
    {
      Blob,
      Table,
      Queue,
    }
  }
}
