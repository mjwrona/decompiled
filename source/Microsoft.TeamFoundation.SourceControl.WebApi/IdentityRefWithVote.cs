// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.IdentityRefWithVote
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
  public class IdentityRefWithVote : IdentityRef
  {
    public IdentityRefWithVote()
    {
    }

    public IdentityRefWithVote(IdentityRef identity)
    {
      if (identity == null)
        return;
      this.DisplayName = identity.DisplayName;
      this.Id = identity.Id;
      this.ImageUrl = identity.ImageUrl;
      this.IsContainer = identity.IsContainer;
      this.ProfileUrl = identity.ProfileUrl;
      this.UniqueName = identity.UniqueName;
      this.Url = identity.Url;
      this.Links = identity.Links;
      if (!(identity is IdentityRefWithVote))
        return;
      this.Vote = ((IdentityRefWithVote) identity).Vote;
      this.ReviewerUrl = ((IdentityRefWithVote) identity).ReviewerUrl;
    }

    [DataMember(Name = "reviewerUrl")]
    [JsonProperty(PropertyName = "reviewerUrl")]
    public string ReviewerUrl { get; set; }

    [DataMember(Name = "vote")]
    [JsonProperty(PropertyName = "vote")]
    public short Vote { get; set; }

    [DataMember(Name = "votedFor", EmitDefaultValue = false)]
    [JsonProperty(PropertyName = "votedFor", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public IdentityRefWithVote[] VotedFor { get; set; }

    [DataMember(Name = "hasDeclined", EmitDefaultValue = false)]
    [JsonProperty(PropertyName = "hasDeclined", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public bool? HasDeclined { get; set; }

    [DataMember(Name = "isRequired", EmitDefaultValue = false)]
    [JsonProperty(PropertyName = "isRequired", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public bool IsRequired { get; set; }

    [DataMember(Name = "isFlagged", EmitDefaultValue = false)]
    [JsonProperty(PropertyName = "isFlagged", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public bool? IsFlagged { get; set; }

    [DataMember(Name = "isReapprove", EmitDefaultValue = false)]
    [JsonProperty(PropertyName = "isReapprove", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public bool? IsReapprove { get; set; }
  }
}
