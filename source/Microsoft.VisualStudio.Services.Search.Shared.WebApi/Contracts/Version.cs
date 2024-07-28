// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Version
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 504F400B-CBC4-4007-9816-31A8DED1C3FC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts
{
  [DataContract]
  public class Version : SearchSecuredV2Object
  {
    public Version(string branchName, string changeId)
    {
      this.BranchName = branchName;
      this.ChangeId = changeId;
    }

    [DataMember(Name = "branchName")]
    public string BranchName { get; set; }

    [DataMember(Name = "changeId")]
    public string ChangeId { get; set; }
  }
}
