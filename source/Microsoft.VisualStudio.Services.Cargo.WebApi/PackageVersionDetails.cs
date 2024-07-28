// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.WebApi.Types.API.PackageVersionDetails
// Assembly: Microsoft.VisualStudio.Services.Cargo.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 79D1C655-766F-4F71-AAEA-7C02E794C2F8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.WebApi.dll

using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Types;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Cargo.WebApi.Types.API
{
  [DataContract]
  public class PackageVersionDetails : IPackageVersionDetails
  {
    [DataMember]
    public JsonPatchOperation Views { get; set; }
  }
}
