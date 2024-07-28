// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Types.ProblemPackage
// Assembly: Microsoft.VisualStudio.Services.Packaging.Shared.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9764DF62-33FE-41B6-9E79-DE201B497BE0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Types
{
  [DataContract]
  [ClientIncludeModel]
  public class ProblemPackage
  {
    [DataMember]
    public string PackageName { get; set; }

    [DataMember]
    public string PackageVersion { get; set; }

    [DataMember]
    public string Protocol { get; set; }

    [DataMember]
    public DateTime Timestamp { get; set; }

    [DataMember]
    public IList<ProblemPackageReason> Reasons { get; set; }

    [DataMember]
    public UpstreamSourceInfo UpstreamSource { get; set; }
  }
}
