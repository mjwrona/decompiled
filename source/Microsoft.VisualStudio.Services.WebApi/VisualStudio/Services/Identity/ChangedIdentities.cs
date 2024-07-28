// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.ChangedIdentities
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Identity
{
  [DataContract]
  public class ChangedIdentities
  {
    [JsonConstructor]
    private ChangedIdentities()
    {
    }

    public ChangedIdentities(IList<Microsoft.VisualStudio.Services.Identity.Identity> identities, ChangedIdentitiesContext sequenceContext)
      : this(identities, sequenceContext, false)
    {
    }

    public ChangedIdentities(
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identities,
      ChangedIdentitiesContext sequenceContext,
      bool moreData)
    {
      this.Identities = identities;
      this.SequenceContext = sequenceContext;
      this.MoreData = moreData;
    }

    [DataMember]
    public IList<Microsoft.VisualStudio.Services.Identity.Identity> Identities { get; private set; }

    [DataMember]
    public ChangedIdentitiesContext SequenceContext { get; private set; }

    [DataMember]
    public bool MoreData { get; private set; }
  }
}
