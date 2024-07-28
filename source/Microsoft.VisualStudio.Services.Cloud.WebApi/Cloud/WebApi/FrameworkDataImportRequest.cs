// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.WebApi.FrameworkDataImportRequest
// Assembly: Microsoft.VisualStudio.Services.Cloud.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52A8E326-8E84-4175-AE92-8ED7AF376B63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.WebApi.dll

using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.Cloud.WebApi
{
  [XmlInclude(typeof (CreateCollectionDataImportRequest))]
  [XmlInclude(typeof (DatabaseDataImportRequest))]
  [XmlInclude(typeof (FileCopyDataImportRequest))]
  [XmlInclude(typeof (HostUpgradeDataImportRequest))]
  [XmlInclude(typeof (OnlinePostHostUpgradeDataImportRequest))]
  [XmlInclude(typeof (StopHostAfterUpgradeDataImportRequest))]
  [XmlInclude(typeof (ObtainDatabaseHoldDataImportRequest))]
  [XmlInclude(typeof (HostMoveDataImportRequest))]
  [XmlInclude(typeof (ActivateDataImportRequest))]
  [XmlInclude(typeof (DataImportDehydrateRequest))]
  [XmlInclude(typeof (RemoveDataImportRequest))]
  [DataContract]
  public abstract class FrameworkDataImportRequest : FrameworkServicingOrchestrationRequest
  {
    [DataMember]
    public int ImportVersion { get; set; }

    public FrameworkDataImportRequest()
    {
    }

    public FrameworkDataImportRequest(FrameworkDataImportRequest other)
      : base((FrameworkServicingOrchestrationRequest) other)
    {
      this.ImportVersion = other.ImportVersion;
    }

    public abstract T Accept<T>(IFrameworkDataImportRequestVisitor<T> visitor);
  }
}
