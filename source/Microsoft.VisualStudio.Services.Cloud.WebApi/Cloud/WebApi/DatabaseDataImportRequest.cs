// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.WebApi.DatabaseDataImportRequest
// Assembly: Microsoft.VisualStudio.Services.Cloud.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52A8E326-8E84-4175-AE92-8ED7AF376B63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Cloud.WebApi
{
  [DataContract]
  public class DatabaseDataImportRequest : FrameworkDataImportRequest
  {
    public DatabaseDataImportRequest()
    {
    }

    public DatabaseDataImportRequest(DatabaseDataImportRequest other)
      : base((FrameworkDataImportRequest) other)
    {
      this.ConnectionString = other.ConnectionString;
      this.SkipFileContentImport = other.SkipFileContentImport;
      this.SkipPostImportValidation = other.SkipPostImportValidation;
      this.HostToMovePostImport = other.HostToMovePostImport;
      this.NeighborHostId = other.NeighborHostId;
      this.RunType = other.RunType;
    }

    [DataMember]
    public string ConnectionString { get; set; }

    [DataMember]
    public bool SkipFileContentImport { get; set; }

    [DataMember]
    public bool SkipPostImportValidation { get; set; }

    [DataMember]
    public DataImportHostMove HostToMovePostImport { get; set; }

    [DataMember]
    public Guid NeighborHostId { get; set; }

    [DataMember]
    public DataImportRunType RunType { get; set; }

    [IgnoreDataMember]
    public override string JobPluginName => "Microsoft.VisualStudio.Services.Cloud.DatabaseDataImportJob";

    [IgnoreDataMember]
    public override bool IsRootRequest => true;

    public override T Accept<T>(IFrameworkDataImportRequestVisitor<T> visitor) => visitor.Visit(this);

    public override string ToString() => string.Format("[{0}, SkipFileContentImport={1}, SkipPostImportValidation={2}, ConnectionString={3}, HostToMovePostImport={4}, NeighborHostId={5}]", (object) base.ToString(), (object) this.SkipFileContentImport, (object) this.SkipPostImportValidation, (object) SecretUtility.ScrubSecrets(this.ConnectionString, false), (object) this.HostToMovePostImport, (object) this.NeighborHostId);
  }
}
