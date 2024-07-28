// Decompiled with JetBrains decompiler
// Type: Nest.DetectionRulesDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class DetectionRulesDescriptor : 
    DescriptorPromiseBase<DetectionRulesDescriptor, List<IDetectionRule>>
  {
    public DetectionRulesDescriptor()
      : base(new List<IDetectionRule>())
    {
    }

    private DetectionRulesDescriptor Add(IDetectionRule m)
    {
      this.PromisedValue.Add(m);
      return this;
    }

    public DetectionRulesDescriptor Rule(
      Func<DetectionRuleDescriptor, IDetectionRule> selector)
    {
      return this.Add(selector(new DetectionRuleDescriptor()));
    }
  }
}
