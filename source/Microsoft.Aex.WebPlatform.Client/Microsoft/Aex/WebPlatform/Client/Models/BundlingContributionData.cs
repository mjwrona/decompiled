// Decompiled with JetBrains decompiler
// Type: Microsoft.Aex.WebPlatform.Client.Models.BundlingContributionData
// Assembly: Microsoft.Aex.WebPlatform.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 303A015F-DE9D-4C4D-9B53-21E1AFC3D2F7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Aex.WebPlatform.Client.dll

using Microsoft.TeamFoundation.Server.WebAccess;
using Microsoft.TeamFoundation.Server.WebAccess.Bundling;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Aex.WebPlatform.Client.Models
{
  [GenerateInterface]
  [DataContract]
  public class BundlingContributionData
  {
    [DataMember]
    public string HostUri { get; set; }

    [DataMember]
    public DynamicBundlesCollection Bundles { get; set; }

    [DataMember]
    public IEnumerable<string> RequiredModules { get; set; }

    [DataMember]
    public IDictionary<string, ContributionPath> OverrideContributionPaths { get; set; }
  }
}
