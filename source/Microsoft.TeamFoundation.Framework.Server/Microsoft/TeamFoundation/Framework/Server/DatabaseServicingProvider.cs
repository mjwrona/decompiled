// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabaseServicingProvider
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DatabaseServicingProvider : 
    IServicingStepGroupExecutionHandler,
    IServicingOperationProvider,
    IServicingResourceProvider
  {
    public static readonly IReadOnlyCollection<string> OperationClassesThatDoNotRecordHistory = (IReadOnlyCollection<string>) new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      "CreateSchema",
      "MoveHost",
      "PreCreateOrganization",
      "CreateHostingOrganization"
    };
    private readonly Guid m_jobId;
    private readonly IVssRequestContext m_requestContext;
    private List<ServicingStepGroupHistoryEntry> m_historyEntries;

    public DatabaseServicingProvider()
    {
    }

    public DatabaseServicingProvider(IVssRequestContext requestContext, Guid jobId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      this.m_requestContext = requestContext;
      this.m_jobId = jobId;
    }

    ServicingOperation IServicingOperationProvider.GetServicingOperation(string servicingOperation) => this.m_requestContext.GetService<TeamFoundationServicingService>().GetServicingOperation(this.m_requestContext, servicingOperation);

    string[] IServicingOperationProvider.GetServicingOperationNames() => this.m_requestContext.GetService<TeamFoundationServicingService>().GetServicingOperationNames(this.m_requestContext).ToArray();

    Stream IServicingResourceProvider.GetServicingResource(string resourceName) => this.m_requestContext.GetService<TeamFoundationServicingService>().GetServicingResource(this.m_requestContext, resourceName);

    ServicingStepGroupExecutionDecision IServicingStepGroupExecutionHandler.StartStepGroup(
      ServicingContext servicingContext,
      ServicingOperation servicingOperation,
      ServicingStepGroup stepGroup)
    {
      ArgumentUtility.CheckForNull<ServicingContext>(servicingContext, nameof (servicingContext));
      ArgumentUtility.CheckForNull<ServicingOperation>(servicingOperation, nameof (servicingOperation));
      ArgumentUtility.CheckForNull<ServicingStepGroup>(stepGroup, nameof (stepGroup));
      string servicingOperationName = servicingOperation.Name;
      string stepGroupName = stepGroup.Name;
      if (stepGroupName.Equals("Install.ProvisionCollectionCreate", StringComparison.Ordinal))
        return ServicingStepGroupExecutionDecision.Execute;
      if (this.m_historyEntries == null)
      {
        using (ServicingHistoryComponent historyComponent = DatabaseServicingProvider.CreateServicingHistoryComponent(servicingContext))
        {
          if (historyComponent != null)
          {
            try
            {
              this.m_historyEntries = historyComponent.QueryStepStepGroupHistory(this.m_jobId);
            }
            catch (Exception ex)
            {
              TeamFoundationTrace.TraceException(ex);
              throw;
            }
          }
        }
      }
      return this.m_historyEntries != null && this.m_historyEntries.FindLast((Predicate<ServicingStepGroupHistoryEntry>) (he =>
      {
        if (!string.Equals(he.StepGroup, stepGroupName, StringComparison.OrdinalIgnoreCase) || !string.Equals(he.ServicingOperation, servicingOperationName, StringComparison.OrdinalIgnoreCase))
          return false;
        return he.ExecutionResult == ServicingStepState.Passed || he.ExecutionResult == ServicingStepState.PassedWithWarnings;
      })) != null ? ServicingStepGroupExecutionDecision.SkipSucceededPreviously : ServicingStepGroupExecutionDecision.Execute;
    }

    void IServicingStepGroupExecutionHandler.FinishStepGroup(
      ServicingContext servicingContext,
      ServicingOperation servicingOperation,
      ServicingStepGroup stepGroup,
      ServicingStepState groupResolution)
    {
      ArgumentUtility.CheckForNull<ServicingContext>(servicingContext, nameof (servicingContext));
      ArgumentUtility.CheckForNull<ServicingOperation>(servicingOperation, nameof (servicingOperation));
      ArgumentUtility.CheckForNull<ServicingStepGroup>(stepGroup, nameof (stepGroup));
      if (groupResolution != ServicingStepState.Passed && groupResolution != ServicingStepState.PassedWithWarnings)
        return;
      string servicingOperationName = servicingOperation.Name;
      string stepGroupName = stepGroup.Name;
      if (this.m_historyEntries != null && this.m_historyEntries.FindLast((Predicate<ServicingStepGroupHistoryEntry>) (he =>
      {
        if (!string.Equals(he.StepGroup, stepGroupName, StringComparison.OrdinalIgnoreCase) || !string.Equals(he.ServicingOperation, servicingOperationName, StringComparison.OrdinalIgnoreCase))
          return false;
        return he.ExecutionResult == ServicingStepState.Passed || he.ExecutionResult == ServicingStepState.PassedWithWarnings;
      })) != null)
        return;
      using (ServicingHistoryComponent historyComponent = DatabaseServicingProvider.CreateServicingHistoryComponent(servicingContext))
      {
        if (historyComponent == null)
          return;
        historyComponent.AddServicingStepGroupHistory(this.m_jobId, servicingOperation.Name, stepGroup.Name, groupResolution);
        if (this.m_historyEntries == null)
          return;
        this.m_historyEntries.Add(new ServicingStepGroupHistoryEntry()
        {
          Id = 0,
          ExecutionResult = groupResolution,
          ExecutionTime = DateTime.UtcNow,
          JobId = this.m_jobId,
          ServicingOperation = servicingOperationName,
          StepGroup = stepGroupName
        });
      }
    }

    private static ServicingHistoryComponent CreateServicingHistoryComponent(
      ServicingContext servicingContext)
    {
      string operationClass = servicingContext.OperationClass;
      int num;
      ISqlConnectionInfo connectionInfo1;
      if (operationClass.Equals("UpgradeDatabase", StringComparison.Ordinal))
      {
        num = int.MaxValue;
        connectionInfo1 = servicingContext.GetConnectionInfo();
      }
      else if (DatabaseServicingProvider.OperationClassesThatDoNotRecordHistory.Contains<string>(operationClass))
      {
        num = 0;
        connectionInfo1 = (ISqlConnectionInfo) null;
      }
      else if (operationClass.Equals("CreateCollection", StringComparison.Ordinal) || operationClass.Equals("PrepareCollection", StringComparison.Ordinal))
      {
        num = 0;
        connectionInfo1 = (ISqlConnectionInfo) null;
        ISqlConnectionInfo connectionInfo2;
        if (servicingContext.TryGetItem<ISqlConnectionInfo>(ServicingItemConstants.ConnectionInfo, out connectionInfo2))
        {
          bool flag = true;
          if (string.Equals(servicingContext.CurrentStepGroupName, ServicingOperationConstants.CreatePartitionDbSchema, StringComparison.Ordinal) || string.Equals(servicingContext.CurrentStepGroupName, "Install.ProvisionCollectionCreate", StringComparison.Ordinal))
          {
            using (SqlScriptResourceComponent componentRaw = connectionInfo2.CreateComponentRaw<SqlScriptResourceComponent>())
              flag = componentRaw.ExecuteStatementScalar("SELECT OBJECT_ID('dbo.prc_QueryPartition')", 300) != DBNull.Value;
          }
          if (flag)
          {
            try
            {
              num = servicingContext.GetTargetPartitionId();
              if (num == 0 && operationClass.Equals("CreateCollection", StringComparison.Ordinal))
                num = 1;
              connectionInfo1 = servicingContext.GetConnectionInfo();
            }
            catch (Exception ex)
            {
            }
          }
        }
      }
      else if (operationClass.Equals("DataImport", StringComparison.Ordinal))
      {
        num = servicingContext.DeploymentRequestContext.ServiceHost.PartitionId;
        connectionInfo1 = servicingContext.DeploymentRequestContext.FrameworkConnectionInfo;
      }
      else if (servicingContext.TargetHostId != Guid.Empty)
      {
        try
        {
          num = servicingContext.GetTargetPartitionId();
          connectionInfo1 = servicingContext.GetConnectionInfo();
        }
        catch (DatabaseConfigurationException ex)
        {
          num = 0;
          connectionInfo1 = (ISqlConnectionInfo) null;
        }
      }
      else
      {
        num = 0;
        connectionInfo1 = (ISqlConnectionInfo) null;
      }
      ServicingHistoryComponent historyComponent = (ServicingHistoryComponent) null;
      if (num > 0 && connectionInfo1 != null)
      {
        historyComponent = TeamFoundationResourceManagementService.CreateComponentRaw<ServicingHistoryComponent>(connectionInfo1, 120, handleNoResourceManagementSchema: true);
        historyComponent.PartitionId = num;
      }
      return historyComponent;
    }

    public ServicingOperationTarget Target => ServicingOperationTarget.Any;
  }
}
