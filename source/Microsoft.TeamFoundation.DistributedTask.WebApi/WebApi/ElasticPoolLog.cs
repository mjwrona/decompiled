// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.ElasticPoolLog
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class ElasticPoolLog
  {
    internal ElasticPoolLog()
    {
    }

    public ElasticPoolLog(int poolId, LogLevel level, OperationType operation, string message)
    {
      ArgumentUtility.CheckForNonPositiveInt(poolId, nameof (PoolId));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(message, nameof (Message));
      this.PoolId = poolId;
      this.Level = level;
      this.Operation = operation;
      this.Message = message;
      this.Timestamp = DateTime.UtcNow;
    }

    public ElasticPoolLog(ElasticPoolLog logToBeCloned)
    {
      this.Id = logToBeCloned.Id;
      this.PoolId = logToBeCloned.PoolId;
      this.Level = logToBeCloned.Level;
      this.Operation = logToBeCloned.Operation;
      this.Message = logToBeCloned.Message;
      this.Timestamp = logToBeCloned.Timestamp;
    }

    [DataMember]
    public long Id { get; internal set; }

    [DataMember]
    public int PoolId { get; set; }

    [DataMember]
    public LogLevel Level { get; set; }

    [DataMember]
    public OperationType Operation { get; set; }

    [DataMember]
    public string Message { get; set; }

    [DataMember]
    public DateTime Timestamp { get; internal set; }
  }
}
