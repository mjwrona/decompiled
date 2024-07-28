// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.TfsExpression
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System.Xml.Serialization;

namespace Microsoft.AzureAd.Icm.Types
{
  public class TfsExpression
  {
    [XmlAttribute]
    public string FieldName { get; set; }

    [XmlAttribute]
    public TfsQuery.TfsOperator Operator { get; set; }

    [XmlAttribute]
    public string Value { get; set; }

    public void ThrowIfInvalid()
    {
      ArgumentCheck.ThrowIfNullEmptyOrWhiteSpace(this.FieldName, "fieldName", nameof (ThrowIfInvalid), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Connectors\\TfsExpression.cs");
      ArgumentCheck.ThrowIfTrue(this.Operator == TfsQuery.TfsOperator.Invalid, "operator", nameof (ThrowIfInvalid), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Connectors\\TfsExpression.cs");
    }
  }
}
