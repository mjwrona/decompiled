// Decompiled with JetBrains decompiler
// Type: Nest.NormalizerDescriptorBase`2
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public abstract class NormalizerDescriptorBase<TNormalizer, TNormalizerInterface> : 
    DescriptorBase<TNormalizer, TNormalizerInterface>,
    INormalizer
    where TNormalizer : NormalizerDescriptorBase<TNormalizer, TNormalizerInterface>, TNormalizerInterface
    where TNormalizerInterface : class, INormalizer
  {
    protected abstract string Type { get; }

    string INormalizer.Type => this.Type;

    string INormalizer.Version { get; set; }

    public TNormalizer Version(string version) => this.Assign<string>(version, (Action<TNormalizerInterface, string>) ((a, v) => a.Version = v));
  }
}
