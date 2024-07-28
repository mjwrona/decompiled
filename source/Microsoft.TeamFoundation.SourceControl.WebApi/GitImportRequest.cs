// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.GitImportRequest
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [DataContract]
  public class GitImportRequest : VersionControlSecuredObject
  {
    [DataMember]
    public int ImportRequestId { get; set; }

    [DataMember]
    public GitRepository Repository { get; set; }

    [DataMember]
    public GitImportRequestParameters Parameters { get; set; }

    [DataMember]
    public GitAsyncOperationStatus Status { get; set; }

    [DataMember]
    public GitImportStatusDetail DetailedStatus { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "_links", IsRequired = false)]
    public ReferenceLinks Links { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Url { get; set; }

    public override void SetSecuredObject(ISecuredObject securedObject)
    {
      base.SetSecuredObject(securedObject);
      this.Repository?.SetSecuredObject(securedObject);
      this.Parameters?.SetSecuredObject(securedObject);
      this.DetailedStatus?.SetSecuredObject(securedObject);
    }
  }
}
