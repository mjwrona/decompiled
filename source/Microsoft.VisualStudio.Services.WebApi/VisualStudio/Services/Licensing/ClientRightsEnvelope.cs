// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.ClientRightsEnvelope
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Licensing
{
  public class ClientRightsEnvelope : IClientRightsEnvelope
  {
    public ClientRightsEnvelope(IList<IClientRight> rights) => this.Rights = rights;

    public Guid ActivityId { get; set; }

    public string Canary { get; set; }

    public DateTimeOffset CreationDate { get; set; }

    public Version EnvelopeVersion { get; set; }

    public DateTimeOffset ExpirationDate { get; set; }

    public TimeSpan RefreshInterval { get; set; }

    public IList<IClientRight> Rights { get; set; }

    public Guid UserId { get; set; }

    public string UserName { get; set; }
  }
}
