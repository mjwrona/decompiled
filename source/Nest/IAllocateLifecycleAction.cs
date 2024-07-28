// Decompiled with JetBrains decompiler
// Type: Nest.IAllocateLifecycleAction
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public interface IAllocateLifecycleAction : ILifecycleAction
  {
    [DataMember(Name = "exclude")]
    IDictionary<string, string> Exclude { get; set; }

    [DataMember(Name = "include")]
    IDictionary<string, string> Include { get; set; }

    [DataMember(Name = "number_of_replicas")]
    int? NumberOfReplicas { get; set; }

    [DataMember(Name = "require")]
    IDictionary<string, string> Require { get; set; }
  }
}
