// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.WebApi.Types.API.PyPiPackageVersionDeletionState
// Assembly: Microsoft.VisualStudio.Services.PyPi.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17E1C323-94FE-4FF1-903A-ED51BA3159D2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.PyPi.WebApi.Types.API
{
  [DataContract]
  [ClientIncludeModel]
  public class PyPiPackageVersionDeletionState
  {
    public PyPiPackageVersionDeletionState(string name, string version, DateTime deletedDate)
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
