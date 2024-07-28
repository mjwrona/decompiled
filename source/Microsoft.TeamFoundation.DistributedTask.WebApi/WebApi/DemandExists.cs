// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.DemandExists
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public sealed class DemandExists : Demand
  {
    public DemandExists(string name)
      : base(name, (string) null)
    {
    }

    public override Demand Clone() => (Demand) new DemandExists(this.Name);

    protected override string GetExpression() => this.Name;

    public override bool IsSatisfied(IDictionary<string, string> capabilities) => capabilities.ContainsKey(this.Name);

    public new void Update(string value) => throw new NotImplementedException();
  }
}
