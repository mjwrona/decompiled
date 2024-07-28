// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules.ListRule
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules
{
  public abstract class ListRule : WorkItemRule
  {
    [XmlElement("value", Type = typeof (string))]
    public HashSet<string> Values { get; set; }

    [XmlElement("set-reference")]
    public ConstantSetReference[] Sets { get; set; }

    public override bool Equals(WorkItemRule other, bool deep)
    {
      if (!base.Equals(other, deep))
        return false;
      ListRule listRule = other as ListRule;
      if (this.Values != null && this.Values.Any<string>())
      {
        if (listRule.Values == null || !listRule.Values.Any<string>() || !this.Values.OrderBy<string, string>((Func<string, string>) (v => v)).SequenceEqual<string>((IEnumerable<string>) listRule.Values.OrderBy<string, string>((Func<string, string>) (v => v))))
          return false;
      }
      else if (listRule.Values != null && listRule.Values.Any<string>())
        return false;
      return this.Sets != null && ((IEnumerable<ConstantSetReference>) this.Sets).Any<ConstantSetReference>() ? listRule.Sets != null && ((IEnumerable<ConstantSetReference>) listRule.Sets).Any<ConstantSetReference>() && ((IEnumerable<ConstantSetReference>) this.Sets).OrderBy<ConstantSetReference, string>((Func<ConstantSetReference, string>) (s => s.ToString()), (IComparer<string>) StringComparer.OrdinalIgnoreCase).SequenceEqual<ConstantSetReference>((IEnumerable<ConstantSetReference>) ((IEnumerable<ConstantSetReference>) listRule.Sets).OrderBy<ConstantSetReference, string>((Func<ConstantSetReference, string>) (s => s.ToString()), (IComparer<string>) StringComparer.OrdinalIgnoreCase)) : listRule.Sets == null || !((IEnumerable<ConstantSetReference>) listRule.Sets).Any<ConstantSetReference>();
    }

    internal override IEnumerable<string> ExtractConstants() => this.GetNonEmptyConstants((IEnumerable<string>) this.Values, base.ExtractConstants());
  }
}
