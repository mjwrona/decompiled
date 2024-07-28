// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.ChangedIdentities_BackCompat
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Identity
{
  public class ChangedIdentities_BackCompat
  {
    internal ChangedIdentities_BackCompat(ChangedIdentities identities)
    {
      this.Identities = Identity_BackCompat.Convert(identities.Identities);
      this.SequenceContext = identities.SequenceContext == null ? (ChangedIdentitiesContext_BackCompat) null : new ChangedIdentitiesContext_BackCompat(identities.SequenceContext.IdentitySequenceId, identities.SequenceContext.GroupSequenceId);
    }

    public IList<Identity_BackCompat> Identities { get; private set; }

    public ChangedIdentitiesContext_BackCompat SequenceContext { get; private set; }
  }
}
