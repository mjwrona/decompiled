// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Facade.PMP.IPackageAggregateMutation
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

namespace Microsoft.VisualStudio.Services.Gallery.Server.Facade.PMP
{
  internal interface IPackageAggregateMutation
  {
    string GetUpdatePackageAggregateArchiveStateMutationString(
      string registryName,
      string packageName,
      bool isArchived);

    string GetUpdatePackageAggregateLockStatusMutationString(
      string registryName,
      string packageName,
      bool isLocked);

    string GetDeletePackageAggregateMutationString(string registryName, string packageName);
  }
}
