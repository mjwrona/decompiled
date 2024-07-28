// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Framework.Server.IChangeRecordLogger
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.ComponentModel.Composition;

namespace Microsoft.VisualStudio.Framework.Server
{
  [InheritedExport]
  public interface IChangeRecordLogger
  {
    void StartDeploymentEventRaw(
      string hostedServiceName,
      Guid serviceTreeGuid,
      string serviceName,
      string fcmAccountName,
      string servicePrincipalClientId,
      string servicePrincipalCertificateThumbprint,
      byte priority,
      ChangeRecordReleaseContext releaseContext);

    void EndDeploymentEventRaw(
      string hostedServiceName,
      Guid serviceTreeGuid,
      string serviceName,
      string fcmAccountName,
      string servicePrincipalClientId,
      string servicePrincipalCertificateThumbprint,
      byte priority,
      ChangeRecordReleaseContext releaseContext,
      bool failed);

    void EndConfigChangeEventRaw(
      string hostedServiceName,
      Guid serviceTreeGuid,
      string serviceName,
      string fcmAccountName,
      string servicePrincipalClientId,
      string servicePrincipalCertificateThumbprint,
      byte priority,
      ChangeRecordReleaseContext releaseContext,
      bool failed,
      DateTimeOffset configChangeExecutionStartTime);

    void LogCompletedChangeEventRaw(
      string hostedServiceName,
      Guid serviceTreeGuid,
      string serviceName,
      string fcmAccountName,
      string servicePrincipalClientId,
      string servicePrincipalCertificateThumbprint,
      string title,
      string description,
      byte priority,
      ChangeRecordReleaseContext releaseContext,
      bool failed);

    StartedChangeRecordInfo StartChangeEventRaw(
      string hostedServiceName,
      Guid serviceTreeGuid,
      string serviceName,
      string fcmAccountName,
      string servicePrincipalClientId,
      string servicePrincipalCertificateThumbprint,
      string title,
      string description,
      byte priority,
      ChangeRecordReleaseContext releaseContext);

    void EndChangeEventRaw(
      StartedChangeRecordInfo info,
      string hostedServiceName,
      Guid serviceTreeGuid,
      string serviceName,
      string fcmAccountName,
      string servicePrincipalClientId,
      string servicePrincipalCertificateThumbprint,
      ChangeRecordReleaseContext releaseContext,
      bool failed);
  }
}
