// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Common.JobResultCheckpointManager`1
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Common;
using System;

namespace Microsoft.VisualStudio.Services.Content.Server.Common
{
  public class JobResultCheckpointManager<T> where T : class, IJobCheckpoint
  {
    private const string RegistryCheckpoint = "/Checkpoint";
    private readonly string RegistryCheckpointPath;
    private readonly string Version;
    private readonly Func<T, bool> IsCheckpointSet;

    public JobResultCheckpointManager(
      string registryBasePath,
      string version,
      Func<T, bool> isCheckpointSet)
    {
      this.RegistryCheckpointPath = registryBasePath + "/Checkpoint";
      this.Version = version;
      this.IsCheckpointSet = isCheckpointSet;
    }

    public T LoadCheckpoint(
      IVssRequestContext requestContext,
      JobParameters jobParameters,
      TimeSpan checkpointValidityPeriod)
    {
      return this.ReadCheckpoint(this.GetCheckpointValue(requestContext, jobParameters), jobParameters, checkpointValidityPeriod);
    }

    public T LoadCheckpointDeleteStale(
      IVssRequestContext requestContext,
      JobParameters jobParameters,
      TimeSpan checkpointValidityPeriod)
    {
      string checkpointValue = this.GetCheckpointValue(requestContext, jobParameters);
      T obj = this.ReadCheckpoint(checkpointValue, jobParameters, checkpointValidityPeriod);
      if (checkpointValue != null && (object) obj == null)
        requestContext.GetService<IVssRegistryService>().DeleteEntries(requestContext, this.GetCheckpointPath(jobParameters));
      return obj;
    }

    private string GetCheckpointValue(
      IVssRequestContext requestContext,
      JobParameters jobParameters)
    {
      return requestContext.GetService<IVssRegistryService>().GetValue(requestContext, (RegistryQuery) this.GetCheckpointPath(jobParameters), false, (string) null);
    }

    private string GetCheckpointPath(JobParameters jobParameters) => string.Format("{0}/D-{1}/P-{2}-{3}", (object) this.RegistryCheckpointPath, (object) jobParameters.DomainId, (object) jobParameters.TotalPartitions, (object) jobParameters.PartitionId);

    private string GetCheckpointPath(T jobInfo) => string.Format("{0}/D-{1}/P-{2}-{3}", (object) this.RegistryCheckpointPath, (object) jobInfo.DomainId, (object) jobInfo.TotalPartitions, (object) jobInfo.PartitionId);

    private T ReadCheckpoint(
      string checkpointStr,
      JobParameters jobParameters,
      TimeSpan checkpointValidityPeriod)
    {
      if (checkpointStr != null && checkpointValidityPeriod > TimeSpan.Zero)
      {
        T obj = JsonSerializer.Deserialize<T>(checkpointStr);
        if ((object) obj != null && obj.Version == this.Version && this.IsCheckpointSet(obj))
        {
          if (!obj.FirstJobStartTime.HasValue || obj.RunId == null)
            throw new ArgumentNullException(string.Format("D-[{0}/P-[{1}/{2}]]: Some checkpoint fields are null: {3}", (object) jobParameters.DomainId, (object) jobParameters.PartitionId, (object) jobParameters.TotalPartitions, (object) checkpointStr));
          if (obj.FirstJobStartTime.Value > DateTimeOffset.UtcNow.Subtract(checkpointValidityPeriod) && obj.FirstJobStartTime.Value < DateTimeOffset.UtcNow && obj.TotalPartitions == jobParameters.TotalPartitions)
          {
            if (obj.PartitionId != jobParameters.PartitionId || obj.DomainId != jobParameters.DomainId)
              throw new ArgumentException(string.Format("D-[{0}/P-[{1}/{2}]: Incompatible Checkpoint PartitionId: {3} DomainId: {4}", (object) jobParameters.DomainId, (object) jobParameters.PartitionId, (object) jobParameters.TotalPartitions, (object) obj.PartitionId, (object) obj.DomainId));
            obj.IsResumedFromCheckpoint = true;
            return obj;
          }
        }
      }
      return default (T);
    }

    public void SaveCheckpoint(IVssRequestContext requestContext, T jobInfo)
    {
      if (!this.IsCheckpointSet(jobInfo))
        return;
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string str1 = JsonSerializer.Serialize<T>(jobInfo);
      IVssRequestContext requestContext1 = requestContext;
      string checkpointPath = this.GetCheckpointPath(jobInfo);
      string str2 = str1;
      service.SetValue<string>(requestContext1, checkpointPath, str2);
    }
  }
}
