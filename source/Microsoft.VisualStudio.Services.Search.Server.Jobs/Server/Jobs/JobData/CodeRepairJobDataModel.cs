// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.JobData.CodeRepairJobDataModel
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.JobData
{
  [DataContract]
  public class CodeRepairJobDataModel
  {
    public CodeRepairJobDataModel()
    {
    }

    public CodeRepairJobDataModel(
      string indexingUnitType,
      string repositoryId,
      string repositoryType)
    {
      this.IndexingUnitType = indexingUnitType;
      this.RepositoryId = repositoryId;
      this.RepositoryType = repositoryType;
    }

    [DataMember(Name = "IndexingUnitType")]
    public string IndexingUnitType { get; set; }

    [DataMember(Name = "RepositoryId")]
    public string RepositoryId { get; set; }

    [DataMember(Name = "RepositoryType")]
    public string RepositoryType { get; set; }
  }
}
