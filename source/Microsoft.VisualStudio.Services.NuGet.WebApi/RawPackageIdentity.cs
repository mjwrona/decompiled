// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.WebApi.RawPackageIdentity
// Assembly: Microsoft.VisualStudio.Services.NuGet.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9D44F181-506D-4445-A06B-7AA7FD5D22D8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.WebApi.dll

namespace Microsoft.VisualStudio.Services.NuGet.WebApi
{
  public class RawPackageIdentity
  {
    public RawPackageIdentity(string id, string version)
    {
      this.Id = id;
      this.Version = version;
    }

    public string Id { get; private set; }

    public string Version { get; private set; }

    public string GetClientNupkgName() => string.Format("{0}.{1}.nupkg", (object) this.Id, (object) this.Version);

    public override string ToString() => string.Format("{0} {1}", (object) this.Id, (object) this.Version);
  }
}
