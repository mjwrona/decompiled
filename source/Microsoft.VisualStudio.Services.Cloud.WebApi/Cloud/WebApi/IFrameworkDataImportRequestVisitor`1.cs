// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.WebApi.IFrameworkDataImportRequestVisitor`1
// Assembly: Microsoft.VisualStudio.Services.Cloud.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52A8E326-8E84-4175-AE92-8ED7AF376B63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.WebApi.dll

namespace Microsoft.VisualStudio.Services.Cloud.WebApi
{
  public interface IFrameworkDataImportRequestVisitor<T>
  {
    T Visit(CreateCollectionDataImportRequest request);

    T Visit(DatabaseDataImportRequest request);

    T Visit(FileCopyDataImportRequest request);

    T Visit(HostUpgradeDataImportRequest request);

    T Visit(OnlinePostHostUpgradeDataImportRequest request);

    T Visit(StopHostAfterUpgradeDataImportRequest request);

    T Visit(ObtainDatabaseHoldDataImportRequest request);

    T Visit(HostMoveDataImportRequest request);

    T Visit(ActivateDataImportRequest request);

    T Visit(DataImportDehydrateRequest request);

    T Visit(RemoveDataImportRequest request);
  }
}
