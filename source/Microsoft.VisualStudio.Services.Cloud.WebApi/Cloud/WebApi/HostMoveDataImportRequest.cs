// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.WebApi.HostMoveDataImportRequest
// Assembly: Microsoft.VisualStudio.Services.Cloud.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52A8E326-8E84-4175-AE92-8ED7AF376B63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Cloud.WebApi
{
  [DataContract]
  public class HostMoveDataImportRequest : FrameworkDataImportRequest
  {
    [DataMember]
    public DataImportHostMove HostToMovePostImport { get; set; }

    [DataMember]
    public Guid NeighborHostId { get; set; }

    [IgnoreDataMember]
    public override string JobPluginName => "Microsoft.VisualStudio.Services.Cloud.CollectionHostMoveDataImportJob";

    [IgnoreDataMember]
    public override bool IsRootRequest => false;

    public override T Accept<T>(IFrameworkDataImportRequestVisitor<T> visitor) => visitor.Visit(this);

    public override string ToString() => string.Format("[{0}, HostToMovePostImport={1}, NeighborHostId={2}]", (object) base.ToString(), (object) this.HostToMovePostImport, (object) this.NeighborHostId);
  }
}
