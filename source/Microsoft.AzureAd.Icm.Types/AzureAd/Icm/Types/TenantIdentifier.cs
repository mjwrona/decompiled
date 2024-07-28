// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.TenantIdentifier
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.AzureAd.Icm.Types
{
  [Serializable]
  public class TenantIdentifier
  {
    public TenantIdentifier(Guid publicId)
    {
      ArgumentCheck.ThrowIfEqualTo<Guid>(publicId, Guid.Empty, nameof (publicId), ".ctor", "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Incidents\\TenantIdentifier.cs");
      this.PublicId = new Guid?(publicId);
    }

    public TenantIdentifier(string friendlyId)
    {
      ArgumentCheck.ThrowIfNullEmptyOrWhiteSpace(friendlyId, nameof (friendlyId), ".ctor", "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Incidents\\TenantIdentifier.cs");
      this.FriendlyId = friendlyId.Trim();
    }

    [DataMember]
    public Guid? PublicId { get; private set; }

    [DataMember]
    public string FriendlyId { get; private set; }
  }
}
