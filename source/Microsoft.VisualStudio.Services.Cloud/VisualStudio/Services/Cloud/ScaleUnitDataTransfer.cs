// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ScaleUnitDataTransfer
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Cloud
{
  [DataContract]
  public class ScaleUnitDataTransfer
  {
    public ScaleUnitDataTransfer(ScaleUnitDataTransfer scaleUnitDataTransfer)
    {
      this.SourceStorageAccount = scaleUnitDataTransfer.SourceStorageAccount;
      this.TargetStorageAccount = scaleUnitDataTransfer.TargetStorageAccount;
      this.PrimaryId = scaleUnitDataTransfer.PrimaryId;
      this.SubsetId = scaleUnitDataTransfer.SubsetId;
      this.ItemId = scaleUnitDataTransfer.ItemId;
      this.TotalEntriesTransferred = scaleUnitDataTransfer.TotalEntriesTransferred;
      this.StartedProcessing = scaleUnitDataTransfer.StartedProcessing;
      this.JobThread = scaleUnitDataTransfer.JobThread;
      this.CompletedProcessing = scaleUnitDataTransfer.CompletedProcessing;
      this.Heartbeat = scaleUnitDataTransfer.Heartbeat;
      this.ItemType = scaleUnitDataTransfer.ItemType;
    }

    public ScaleUnitDataTransfer()
    {
    }

    [DataMember]
    public string SourceStorageAccount { get; set; }

    [DataMember]
    public string TargetStorageAccount { get; set; }

    [DataMember]
    public string PrimaryId { get; set; }

    [DataMember]
    public string SubsetId { get; set; }

    [DataMember]
    public string ItemId { get; set; }

    [DataMember]
    public long? TotalEntriesTransferred { get; set; }

    [DataMember]
    public DateTime? StartedProcessing { get; set; }

    [DataMember]
    public Guid? JobThread { get; set; }

    [DataMember]
    public bool CompletedProcessing { get; set; }

    [DataMember]
    public DateTime Heartbeat { get; set; }

    [DataMember]
    public string ItemType { get; set; }
  }
}
