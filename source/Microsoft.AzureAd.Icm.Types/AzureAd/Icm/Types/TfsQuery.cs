// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.TfsQuery
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System.Xml.Serialization;

namespace Microsoft.AzureAd.Icm.Types
{
  public class TfsQuery
  {
    [XmlElement(ElementName = "And", Type = typeof (TfsGroupAnd))]
    [XmlElement(ElementName = "Or", Type = typeof (TfsGroupOr))]
    public TfsGroupOperator Group { get; set; }

    public void ThrowIfInvalid()
    {
      ArgumentCheck.ThrowIfNull((object) this.Group, "group", nameof (ThrowIfInvalid), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Connectors\\TfsQuery.cs");
      this.Group.ThrowIfInvalid();
    }

    public enum TfsOperator
    {
      Invalid,
      Equals,
      NotEquals,
    }
  }
}
