// Decompiled with JetBrains decompiler
// Type: Nest.IDetectionRule
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [ReadAs(typeof (DetectionRule))]
  public interface IDetectionRule
  {
    [DataMember(Name = "actions")]
    IEnumerable<RuleAction> Actions { get; set; }

    [DataMember(Name = "conditions")]
    IEnumerable<IRuleCondition> Conditions { get; set; }

    [DataMember(Name = "scope")]
    IReadOnlyDictionary<Field, FilterRef> Scope { get; set; }
  }
}
