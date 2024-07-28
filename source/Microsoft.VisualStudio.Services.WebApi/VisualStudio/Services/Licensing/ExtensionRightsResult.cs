// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.ExtensionRightsResult
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Licensing
{
  public class ExtensionRightsResult
  {
    public Guid HostId { get; set; }

    public HashSet<string> EntitledExtensions { get; set; }

    public ExtensionRightsResultCode ResultCode { get; set; }

    public ExtensionRightsReasonCode ReasonCode { get; set; }

    public string Reason { get; set; }

    public override string ToString() => string.Format("HostId: {0}; ResultCode: {1}; EntitledExtensions:{2}", (object) this.HostId, (object) this.ResultCode, this.EntitledExtensions == null ? (object) string.Empty : (object) string.Join("|", (IEnumerable<string>) this.EntitledExtensions));
  }
}
