// Decompiled with JetBrains decompiler
// Type: Nest.GetUserAccessTokenResponse
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  public class GetUserAccessTokenResponse : ResponseBase
  {
    [DataMember(Name = "access_token")]
    public string AccessToken { get; set; }

    [DataMember(Name = "expires_in")]
    public long ExpiresIn { get; set; }

    [DataMember(Name = "scope")]
    public string Scope { get; set; }

    [DataMember(Name = "type")]
    public string Type { get; set; }

    [DataMember(Name = "authentication")]
    public Authentication Authentication { get; set; }
  }
}
