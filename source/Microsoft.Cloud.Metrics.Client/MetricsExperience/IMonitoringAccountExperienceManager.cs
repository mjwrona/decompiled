// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.MetricsExperience.IMonitoringAccountExperienceManager
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Microsoft.Cloud.Metrics.Client.Configuration;
using Microsoft.Online.Metrics.Serialization.MetricsExperience;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Cloud.Metrics.Client.MetricsExperience
{
  public interface IMonitoringAccountExperienceManager
  {
    Task BuildExperienceAsync(
      IMonitoringAccount monitoringAccount,
      string experience,
      CancellationToken cancellationToken,
      bool skipLimitValidation = false,
      bool subscribe = false,
      Guid? traceId = null);

    Task<ExperienceConfigurationState> RemoveExperienceAsync(
      IMonitoringAccount monitoringAccount,
      string experience,
      CancellationToken cancellationToken,
      bool shouldCommit = false,
      Guid? traceId = null);

    Task<ExperienceConfigurationState> GetStateOfExperienceAsync(
      IMonitoringAccount monitoringAccount,
      string experience,
      CancellationToken cancellationToken,
      Guid? traceId = null);

    Task<IEnumerable<string>> GetListOfExperienceNamesAsync(
      IMonitoringAccount monitoringAccount,
      CancellationToken cancellationToken,
      Guid? traceId = null);

    Task<IEnumerable<ExperienceInformationState>> GetListOfExperiencesAsync(
      IMonitoringAccount monitoringAccount,
      CancellationToken cancellationToken,
      Guid? traceId = null);

    Task<bool> CheckExperienceTypeIsValid(
      IMonitoringAccount monitoringAccount,
      string experience,
      CancellationToken cancellationToken,
      Guid? traceId = null);

    Task SubscribeToExperienceAsync(
      IMonitoringAccount monitoringAccount,
      string experience,
      Guid? traceId = null);

    Task UnsubscribeFromExperienceAsync(
      IMonitoringAccount monitoringAccount,
      string experience,
      Guid? traceId = null);

    Task<IEnumerable<string>> GetSubscribedExperiencesAsync(
      IMonitoringAccount monitoringAccount,
      Guid? traceId = null);
  }
}
