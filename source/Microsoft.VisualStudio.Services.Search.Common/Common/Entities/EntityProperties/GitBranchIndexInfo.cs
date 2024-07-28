// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties.GitBranchIndexInfo
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties
{
  [DataContract]
  public class GitBranchIndexInfo : IExtensibleDataObject
  {
    private ExtensionDataObject m_extensionDataValue;

    public ExtensionDataObject ExtensionData
    {
      get => this.m_extensionDataValue;
      set => this.m_extensionDataValue = value;
    }

    public GitBranchIndexInfo()
    {
      this.LastIndexedCommitId = RepositoryConstants.DefaultLastIndexCommitId;
      this.LastIndexedCommitUtcTime = RepositoryConstants.DefaultLastIndexChangeUtcTime;
      this.BranchLastProcessedTime = RepositoryConstants.DefaultLastProcessedTime;
      this.GitIndexJobYieldData = new GitIndexJobYieldData();
      this.BulkIndexJobYieldData = new GitBulkIndexJobYieldData();
    }

    [DataMember(Order = 1)]
    public string LastIndexedCommitId { get; set; }

    [DataMember(Order = 2)]
    public DateTime LastIndexedCommitUtcTime { get; set; }

    [DataMember(Order = 3)]
    public DateTime BranchLastProcessedTime { get; set; }

    [DataMember(Order = 4)]
    public GitIndexJobYieldData GitIndexJobYieldData { get; set; }

    [DataMember(Order = 5)]
    public GitBulkIndexJobYieldData BulkIndexJobYieldData { get; set; }

    public bool IsDefaultLastIndexedCommitId() => string.IsNullOrEmpty(this.LastIndexedCommitId) || this.LastIndexedCommitId.Equals(RepositoryConstants.DefaultLastIndexCommitId);
  }
}
