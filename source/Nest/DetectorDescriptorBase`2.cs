// Decompiled with JetBrains decompiler
// Type: Nest.DetectorDescriptorBase`2
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public abstract class DetectorDescriptorBase<TDetectorDescriptor, TDetectorInterface> : 
    DescriptorBase<TDetectorDescriptor, TDetectorInterface>,
    IDetector
    where TDetectorDescriptor : DetectorDescriptorBase<TDetectorDescriptor, TDetectorInterface>, TDetectorInterface
    where TDetectorInterface : class, IDetector
  {
    private readonly string _function;

    protected DetectorDescriptorBase(string function) => this._function = function;

    IEnumerable<IDetectionRule> IDetector.CustomRules { get; set; }

    string IDetector.DetectorDescription { get; set; }

    int? IDetector.DetectorIndex { get; set; }

    Nest.ExcludeFrequent? IDetector.ExcludeFrequent { get; set; }

    string IDetector.Function => this._function;

    bool? IDetector.UseNull { get; set; }

    public TDetectorDescriptor DetectorDescription(string description) => this.Assign<string>(description, (Action<TDetectorInterface, string>) ((a, v) => a.DetectorDescription = v));

    public TDetectorDescriptor ExcludeFrequent(Nest.ExcludeFrequent? excludeFrequent) => this.Assign<Nest.ExcludeFrequent?>(excludeFrequent, (Action<TDetectorInterface, Nest.ExcludeFrequent?>) ((a, v) => a.ExcludeFrequent = v));

    public TDetectorDescriptor UseNull(bool? useNull = true) => this.Assign<bool?>(useNull, (Action<TDetectorInterface, bool?>) ((a, v) => a.UseNull = v));

    public TDetectorDescriptor DetectorIndex(int? detectorIndex) => this.Assign<int?>(detectorIndex, (Action<TDetectorInterface, int?>) ((a, v) => a.DetectorIndex = v));

    public TDetectorDescriptor CustomRules(
      Func<DetectionRulesDescriptor, IPromise<List<IDetectionRule>>> selector)
    {
      return this.Assign<List<IDetectionRule>>(selector(new DetectionRulesDescriptor()).Value, (Action<TDetectorInterface, List<IDetectionRule>>) ((a, v) => a.CustomRules = (IEnumerable<IDetectionRule>) v));
    }
  }
}
