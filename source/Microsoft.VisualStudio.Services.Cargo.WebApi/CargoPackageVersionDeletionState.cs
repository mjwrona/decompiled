// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.WebApi.Types.API.CargoPackageVersionDeletionState
// Assembly: Microsoft.VisualStudio.Services.Cargo.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 79D1C655-766F-4F71-AAEA-7C02E794C2F8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Cargo.WebApi.Types.API
{
  [DataContract]
  [ClientIncludeModel]
  public class CargoPackageVersionDeletionState
  {
    public CargoPackageVersionDeletionState(string name, string version, DateTime deletedDate)
    {
      this.Name = name;
      this.Version = version;
      this.DeletedDate = deletedDate;
    }

    [DataMember(IsRequired = true)]
    public string Name { get; set; }

    [DataMember(IsRequired = true)]
    public string Version { get; set; }

    [DataMember(IsRequired = true)]
    public DateTime DeletedDate { get; set; }
  }
}
