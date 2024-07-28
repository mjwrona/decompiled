// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.WebApi.Types.API.NpmPackageVersionDeletionState
// Assembly: Microsoft.VisualStudio.Services.Npm.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 639B57A1-1338-429F-9659-38C0A0394E05
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Npm.WebApi.Types.API
{
  [DataContract]
  [ClientIncludeModel]
  public class NpmPackageVersionDeletionState
  {
    public NpmPackageVersionDeletionState(string name, string version, DateTime unpublishedDate)
    {
      this.Name = name;
      this.Version = version;
      this.UnpublishedDate = unpublishedDate;
    }

    [DataMember(IsRequired = true)]
    public string Name { get; set; }

    [DataMember(IsRequired = true)]
    public string Version { get; set; }

    [DataMember(IsRequired = true)]
    public DateTime UnpublishedDate { get; set; }
  }
}
