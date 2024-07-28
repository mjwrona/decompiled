// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.WebApi.Constants
// Assembly: Microsoft.Azure.Pipelines.Checks.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 381241F9-9196-42AF-BB4C-5187E3EFE32E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.Azure.Pipelines.Checks.WebApi
{
  [GenerateSpecificConstants(null)]
  public static class Constants
  {
    public const string AreaId = "4A933897-0488-45AF-BD82-6FD3AD33F46A";
    public const string AreaName = "PipelinesChecks";
    public const string ChecksConfigurationsLocationIdString = "86C8381E-5AEE-4CDE-8AE4-25C0C7F5EAEA";
    public static readonly Guid ChecksConfigurationsLocationId = new Guid("86C8381E-5AEE-4CDE-8AE4-25C0C7F5EAEA");
    public const string ChecksConfigurationsResource = "configurations";
    public const string ChecksQueryConfigurationsLocationIdString = "5F3D0E64-F943-4584-8811-77EB495E831E";
    public static readonly Guid ChecksQueryConfigurationsLocationId = new Guid("5F3D0E64-F943-4584-8811-77EB495E831E");
    public const string ChecksQueryConfigurationsResource = "queryconfigurations";
    public const string ChecksRunsLocationIdString = "91282C1D-C183-444F-9554-1485BFB3879D";
    public static readonly Guid ChecksRunsLocationId = new Guid("91282C1D-C183-444F-9554-1485BFB3879D");
    public const string ChecksRunsResource = "runs";
    public const string AuthChecksAsyncFlowFeatureFlag = "Pipelines.Checks.AuthChecksAsyncFlow";
    public const string EnableQWITaskCheck = "Pipelines.Checks.EnableQWITaskCheck";
    public const string EnableServiceNowCheck = "Pipelines.Checks.EnableServiceNowCheck";
    public const string EnableAllTaskChecks = "Pipelines.Checks.EnableAllTaskChecks";
    public const string EnableRerunChecks = "Pipelines.Checks.EnableRerunChecks";
    public const string EnableOrderingChecks = "Pipelines.Checks.EnableOrderingChecks";
    public const string AllowChangingApprovalOrder = "Pipelines.Checks.AllowChangingApprovalOrder";
    public const string DoNotAllowDuplicateCheckTypes = "Pipelines.Checks.DoNotAllowDuplicateCheckTypes";
    public const string ArtifactPolicyCheckFeatureFlag = "Pipelines.Checks.ArtifactPolicyCheck";
    public const string LogDetailsAboutConfiguredChecks = "Pipelines.Checks.LogDetailsAboutConfiguredChecks";
    public const string EnableDisabledChecksFeature = "Pipelines.Checks.EnableDisabledChecksFeature";
    [GenerateConstant(null)]
    public const int DEFAULT_RETRY_INTERVAL_IN_MINUTES = 0;
    [GenerateConstant(null)]
    public const int DEFAULT_TIMEOUT_IN_MINUTES = 1440;
    [GenerateConstant(null)]
    public const int MAX_TIMEOUT_VALUE_IN_MINUTES = 43200;
  }
}
