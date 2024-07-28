// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Users.Server.GitHub.GitHubUserData
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Users.Server.GitHub
{
  [DataContract]
  public class GitHubUserData
  {
    [DataMember(Name = "login")]
    public string Login { get; set; }

    [DataMember(Name = "id")]
    public string Id { get; set; }
  }
}
