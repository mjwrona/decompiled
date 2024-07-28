// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Processors.IReleaseNotification
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Processors
{
  public interface IReleaseNotification
  {
    void SendNotification(
      int releaseId,
      string releaseName,
      string releaseDefinitionName,
      string stageTypeName,
      string environmentName,
      int releaseStepId,
      int stepTypeId,
      bool isAutomated);

    void SendFailureNotifications(int releaseId, bool rollback);

    void SendPendingReleaseStepsCancellationNotifications(int releaseId, string releaseName);

    void SendManualInterventionNotification(
      int releaseId,
      int releaseStepId,
      int recipientId,
      int recipientType,
      string details);
  }
}
