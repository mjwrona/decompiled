// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Bundling.BundleAreaMetadata
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Bundling
{
  [DataContract]
  public class BundleAreaMetadata
  {
    [DataMember(Name = "modules", EmitDefaultValue = false)]
    public IDictionary<string, BundleModuleMetadata> Modules;
    [DataMember(Name = "pathMap", EmitDefaultValue = false)]
    public IDictionary<short, string> PathMap;

    public BundleAreaMetadata()
    {
      this.Modules = (IDictionary<string, BundleModuleMetadata>) new Dictionary<string, BundleModuleMetadata>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.PathMap = (IDictionary<short, string>) new Dictionary<short, string>();
    }
  }
}
