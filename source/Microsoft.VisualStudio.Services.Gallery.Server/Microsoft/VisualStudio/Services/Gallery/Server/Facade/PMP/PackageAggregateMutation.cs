// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Facade.PMP.PackageAggregateMutation
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

namespace Microsoft.VisualStudio.Services.Gallery.Server.Facade.PMP
{
  internal class PackageAggregateMutation : IPackageAggregateMutation
  {
    public string GetUpdatePackageAggregateArchiveStateMutationString(
      string registryName,
      string packageName,
      bool isArchived)
    {
      return "mutation { updatePackageAggregateArchivedState(input: {    packageAggregateIdentity: {      registryName: \"" + registryName + "\",      packageName: \"" + packageName + "\",    }    isArchived :" + isArchived.ToString().ToLower() + ",  }){      packageAggregate {        identity {          registryName          packageName        }        metadata {          isArchived        }    }    errors {      ... on Error {        message      }      ... on PackageAggregateNotFoundError {        message      }      ... on PackageAggregateUpdateFailedError {        message      }          }      }}";
    }

    public string GetUpdatePackageAggregateLockStatusMutationString(
      string registryName,
      string packageName,
      bool isLocked)
    {
      return "mutation { updatePackageAggregateLockedState(input: {    packageAggregateIdentity: {      registryName: \"" + registryName + "\",      packageName: \"" + packageName + "\",    }    isLocked :" + isLocked.ToString().ToLower() + ",  }){      packageAggregate {        identity {          registryName          packageName        }        metadata {          isLocked        }    }    errors {      ... on Error {        message      }      ... on PackageAggregateNotFoundError {        message      }      ... on PackageAggregateUpdateFailedError {        message      }          }      }}";
    }

    public string GetDeletePackageAggregateMutationString(string registryName, string packageName) => "mutation { hardDeletePackageAggregate(input: {    packageAggregateIdentity: {      registryName: \"" + registryName + "\",      packageName: \"" + packageName + "\",    }}){      packageAggregateIdentity  {        registryName          packageName          } errors {  ... on Error{          message   }}}}";
  }
}
