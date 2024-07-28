// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebApi.CustomRepositoryHealthResponse
// Assembly: Microsoft.VisualStudio.Services.Search.WebApi.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5D4CB2D3-3C08-46C7-B9C5-51E638F57F9E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.WebApi.Legacy.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.WebApi
{
  [DataContract]
  public class CustomRepositoryHealthResponse
  {
    public CustomRepositoryHealthResponse(
      long continuationToken,
      string lastCommitId,
      bool endOfResult,
      IEnumerable<FailedFile> failedFiles)
    {
      this.ContinuationToken = continuationToken;
      this.LastIndexedCommitId = lastCommitId;
      this.EndOfResult = endOfResult;
      this.FailedFiles = failedFiles;
    }

    [DataMember(Name = "continuationToken")]
    public long ContinuationToken { get; set; }

    [DataMember(Name = "endOfResult")]
    public bool EndOfResult { get; set; }

    [DataMember(Name = "lastIndexedCommitId")]
    public string LastIndexedCommitId { get; set; }

    [DataMember(Name = "failedFiles")]
    public IEnumerable<FailedFile> FailedFiles { get; set; }
  }
}
