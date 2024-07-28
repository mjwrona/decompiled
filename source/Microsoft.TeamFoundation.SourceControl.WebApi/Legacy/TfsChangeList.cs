// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.TfsChangeList
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi.Legacy
{
  [DataContract]
  public class TfsChangeList : ChangeList
  {
    public TfsChangeList(bool isShelveset = false) => this.IsShelveset = isShelveset;

    [DataMember(Name = "isShelveset", EmitDefaultValue = false)]
    public bool IsShelveset { get; private set; }

    [DataMember(Name = "shelvesetName", EmitDefaultValue = false)]
    public string ShelvesetName { get; set; }

    [DataMember(Name = "changesetId", EmitDefaultValue = false)]
    public int ChangesetId { get; set; }

    [DataMember(Name = "policyOverride", EmitDefaultValue = false)]
    public TfsPolicyOverrideInfo PolicyOverride { get; set; }

    [DataMember(Name = "ownerRef", EmitDefaultValue = false)]
    public IdentityRef OwnerRef { get; set; }
  }
}
