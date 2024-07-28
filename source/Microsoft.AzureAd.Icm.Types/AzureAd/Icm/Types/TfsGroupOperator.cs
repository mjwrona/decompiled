// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.TfsGroupOperator
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.AzureAd.Icm.Types
{
  [XmlInclude(typeof (TfsGroupAnd))]
  [XmlInclude(typeof (TfsGroupOr))]
  public abstract class TfsGroupOperator
  {
    [XmlElement(ElementName = "And", Type = typeof (TfsGroupAnd))]
    [XmlElement(ElementName = "Or", Type = typeof (TfsGroupOr))]
    public List<TfsGroupOperator> Groups { get; set; }

    [XmlElement(ElementName = "Expression")]
    public List<TfsExpression> Expressions { get; set; }

    [XmlIgnore]
    public bool ExpressionsSpecified => this.Groups == null || this.Groups.Count == 0;

    public void ThrowIfInvalid()
    {
      bool flag = this.Groups != null && this.Groups.Count > 0;
      int num = this.Expressions == null ? 0 : (this.Expressions.Count > 0 ? 1 : 0);
      ArgumentCheck.ThrowIfTrue((num & (flag ? 1 : 0)) != 0, "groups and expressions", nameof (ThrowIfInvalid), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Connectors\\TfsGroupOperator.cs");
      if (num != 0)
      {
        foreach (TfsExpression expression in this.Expressions)
          expression.ThrowIfInvalid();
      }
      else
      {
        if (!flag)
          return;
        foreach (TfsGroupOperator group in this.Groups)
          group.ThrowIfInvalid();
      }
    }
  }
}
