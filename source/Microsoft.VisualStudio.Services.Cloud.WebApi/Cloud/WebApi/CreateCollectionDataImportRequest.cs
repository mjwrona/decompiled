// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.WebApi.CreateCollectionDataImportRequest
// Assembly: Microsoft.VisualStudio.Services.Cloud.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52A8E326-8E84-4175-AE92-8ED7AF376B63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Cloud.WebApi
{
  [DataContract]
  public class CreateCollectionDataImportRequest : FrameworkDataImportRequest
  {
    [DataMember]
    public string OrganizationName { get; set; }

    [DataMember]
    public string CollectionName { get; set; }

    [DataMember]
    public string PreferredRegion { get; set; }

    [DataMember]
    public Guid OwnerId { get; set; }

    [IgnoreDataMember]
    public override string JobPluginName => "Microsoft.VisualStudio.Services.Cloud.CreateCollectionDataImportJob";

    [IgnoreDataMember]
    public override bool IsRootRequest => true;

    public override T Accept<T>(IFrameworkDataImportRequestVisitor<T> visitor) => visitor.Visit(this);
  }
}
