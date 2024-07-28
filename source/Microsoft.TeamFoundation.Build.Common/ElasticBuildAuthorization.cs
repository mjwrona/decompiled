// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Common.ElasticBuildAuthorization
// Assembly: Microsoft.TeamFoundation.Build.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AD9C54FA-787C-49B8-AA73-C4A6EF8CE391
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build.Common.dll

using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [DataContract(Name = "Authorization", Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Build/Elastic")]
  public sealed class ElasticBuildAuthorization
  {
    [DataMember(IsRequired = true)]
    public byte[] AccessToken { get; set; }

    [DataMember(IsRequired = true)]
    public string PoolName { get; set; }

    [DataMember(IsRequired = true)]
    public string RoleInstance { get; set; }
  }
}
