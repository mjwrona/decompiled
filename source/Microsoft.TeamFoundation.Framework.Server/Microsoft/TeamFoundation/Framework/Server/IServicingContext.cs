// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IServicingContext
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public interface IServicingContext : ICancelable, IDisposable
  {
    void AddTokenIfNotDefined(string tokenName, string tokenValue);

    void AddItemIfNotDefined(string itemName, object objectValue);

    TComponent CreateComponent<TComponent>(int partitionId) where TComponent : TeamFoundationSqlResourceComponent, new();

    TComponent CreateComponent<TComponent>(int partitiionId, int commandTimeout) where TComponent : TeamFoundationSqlResourceComponent, new();

    TComponent CreateComponent<TComponent>(
      int partitionId,
      int commandTimeout,
      int deadlockPause,
      int maxDeadlockRetries)
      where TComponent : TeamFoundationSqlResourceComponent, new();

    string CurrentServicingOperation { get; }

    IVssRequestContext DeploymentRequestContext { get; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    IServicingLeaseManager ServicingLeaseManager { get; }

    IVssRequestContext GetTargetRequestContext();

    IVssRequestContext GetTargetRequestContext(bool createIfNotExist);

    void DisposeTargetRequestContext();

    ISqlConnectionInfo GetConnectionInfo();

    IDictionary<string, object> Items { get; }

    void Log(ServicingStepLogEntryKind entryKind, string message);

    void LogInfo(string message);

    void LogInfo(string format, params object[] args);

    void Error(string message);

    void Error(string format, params object[] args);

    void Warn(string message);

    void Warn(string format, params object[] args);

    Guid TargetHostId { get; set; }

    ITFLogger TFLogger { get; }

    IDictionary<string, string> Tokens { get; }

    string OperationClass { get; }

    ServicingStepState GroupResolution { get; }

    string CurrentStepGroupName { get; }

    int CurrentStepNumber { get; }

    List<ServicingStep> CurrentStepGroupSteps { get; }

    T ParseToken<T>(string tokenName, T defaultValue);

    bool TryGetToken(string tokenName, out string value);

    string GetRequiredToken(string tokenName);

    T GetRequiredToken<T>(string tokenName);

    T GetItem<T>(string itemName);

    bool TryGetItem<T>(string itemName, out T item);

    int GetTargetPartitionId();

    void SetTargetPartitionId(int partitionId);

    IServicingResourceProvider ResourceProvider { get; }

    void SkipChildren();

    ISqlConnectionInfo GetDefaultConnectionInfo();

    void DisposeDeploymentRequestContext();

    ServicingOperationTarget ServicingOperationTarget { get; set; }

    bool IsOnPrem { get; }

    string ReplaceTokens(string templateText);

    string ReplaceResources(string stepData);

    string StepName { get; }

    void VerifyDeploymentJobSource();
  }
}
