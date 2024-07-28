// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Authorization.WebApi.AuthorizationCheckConstants
// Assembly: Microsoft.Azure.Pipelines.Authorization.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4807FD31-F2A4-4329-AA76-35B262BDA671
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Authorization.WebApi.dll

using System;

namespace Microsoft.Azure.Pipelines.Authorization.WebApi
{
  public class AuthorizationCheckConstants
  {
    public const string AuthorizationCheckTypeName = "Authorization";
    public const string AuthorizationCheckTimelineRecordType = "Checkpoint.Authorization";
    public const string AuthorizationCheckTypeIdString = "E1A1FC6C-C278-4492-AE1C-C42F8697612C";
    public static readonly Guid AuthorizationCheckTypeId = new Guid("E1A1FC6C-C278-4492-AE1C-C42F8697612C");
    public const string AuthChecksAsyncFlowFeatureFlag = "Pipelines.Checks.AuthChecksAsyncFlow";
    public const string AuthorizationCompletedEventType = "MS.Azure.Pipelines.AuthorizationCompleted";
    public const string SkipStalePipelinesRemovalFeatureFlag = "Pipelines.Checks.SkipStalePipelinesRemoval";
  }
}
