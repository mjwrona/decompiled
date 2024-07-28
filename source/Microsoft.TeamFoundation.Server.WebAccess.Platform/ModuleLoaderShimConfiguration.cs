// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.ModuleLoaderShimConfiguration
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  [DataContract]
  public class ModuleLoaderShimConfiguration : WebSdkMetadata
  {
    [DataMember]
    public List<string> Deps { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Exports { get; set; }

    public ModuleLoaderShimConfiguration(string[] deps, string exports)
    {
      this.Deps = new List<string>();
      if (deps != null)
        this.Deps.AddRange((IEnumerable<string>) deps);
      this.Exports = exports;
    }
  }
}
