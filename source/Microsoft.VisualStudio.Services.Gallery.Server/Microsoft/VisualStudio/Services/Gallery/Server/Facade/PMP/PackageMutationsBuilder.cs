// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Facade.PMP.PackageMutationsBuilder
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

namespace Microsoft.VisualStudio.Services.Gallery.Server.Facade.PMP
{
  internal class PackageMutationsBuilder : IPackageMutationsBuilder
  {
    public string GetDeletePackageMutationString(
      string registryName,
      string packageName,
      string version)
    {
      return "mutation { hardDeletePackage(input: {    packageIdentity: {      registryName: \"" + registryName + "\",      packageName: \"" + packageName + "\", packageVersion: \"" + version + "\"   }}){      packageIdentity  {        registryName          packageName     packageVersion     } errors {  ... on Error{          message   }}}}";
    }
  }
}
