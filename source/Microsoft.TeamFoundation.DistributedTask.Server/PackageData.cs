// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.PackageData
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal class PackageData
  {
    public PackageData(PackageMetadata package, IDictionary<string, string> data)
    {
      ArgumentUtility.CheckForNull<PackageMetadata>(package, nameof (package));
      this.Package = package;
      this.Data = data;
    }

    public PackageMetadata Package { get; private set; }

    public IDictionary<string, string> Data { get; private set; }
  }
}
