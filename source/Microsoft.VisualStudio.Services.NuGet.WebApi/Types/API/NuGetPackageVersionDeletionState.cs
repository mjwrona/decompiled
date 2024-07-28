// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.WebApi.Types.API.NuGetPackageVersionDeletionState
// Assembly: Microsoft.VisualStudio.Services.NuGet.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9D44F181-506D-4445-A06B-7AA7FD5D22D8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.WebApi.dll

using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Types;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.NuGet.WebApi.Types.API
{
  [DataContract]
  [ClientIncludeModel]
  public class NuGetPackageVersionDeletionState : IPackageVersionDeletionState
  {
    public NuGetPackageVersionDeletionState(string name, string version, DateTime deletedDate)
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
