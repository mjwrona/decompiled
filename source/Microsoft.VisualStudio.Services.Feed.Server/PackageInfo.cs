// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.PackageInfo
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class PackageInfo : IPackageInfo
  {
    public PackageInfo()
    {
    }

    public PackageInfo(IPackageInfo info)
    {
      this.Id = info.Id;
      this.NormalizedName = info.NormalizedName;
      this.Name = info.Name;
      this.ProtocolType = info.ProtocolType;
    }

    public Guid Id { get; set; }

    public string NormalizedName { get; set; }

    public string Name { get; set; }

    public string ProtocolType { get; set; }
  }
}
