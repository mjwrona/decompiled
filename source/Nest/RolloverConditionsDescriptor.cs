// Decompiled with JetBrains decompiler
// Type: Nest.RolloverConditionsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class RolloverConditionsDescriptor : 
    DescriptorBase<RolloverConditionsDescriptor, IRolloverConditions>,
    IRolloverConditions
  {
    Time IRolloverConditions.MaxAge { get; set; }

    long? IRolloverConditions.MaxDocs { get; set; }

    string IRolloverConditions.MaxSize { get; set; }

    string IRolloverConditions.MaxPrimaryShardSize { get; set; }

    public RolloverConditionsDescriptor MaxAge(Time maxAge) => this.Assign<Time>(maxAge, (Action<IRolloverConditions, Time>) ((a, v) => a.MaxAge = v));

    public RolloverConditionsDescriptor MaxDocs(long? maxDocs) => this.Assign<long?>(maxDocs, (Action<IRolloverConditions, long?>) ((a, v) => a.MaxDocs = v));

    public RolloverConditionsDescriptor MaxSize(string maxSize) => this.Assign<string>(maxSize, (Action<IRolloverConditions, string>) ((a, v) => a.MaxSize = v));

    public RolloverConditionsDescriptor MaxPrimaryShardSize(string maxPrimaryShardSize) => this.Assign<string>(maxPrimaryShardSize, (Action<IRolloverConditions, string>) ((a, v) => a.MaxPrimaryShardSize = v));
  }
}
