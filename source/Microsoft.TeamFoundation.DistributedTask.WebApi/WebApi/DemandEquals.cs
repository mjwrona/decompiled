// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.DemandEquals
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public sealed class DemandEquals : Demand
  {
    public DemandEquals(string name, string value)
      : base(name, value)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(value, nameof (value));
    }

    public override Demand Clone() => (Demand) new DemandEquals(this.Name, this.Value);

    protected override string GetExpression() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} -equals {1}", (object) this.Name, (object) this.Value);

    public override bool IsSatisfied(IDictionary<string, string> capabilities)
    {
      string str;
      return capabilities.TryGetValue(this.Name, out str) && this.Value.Equals(str, StringComparison.OrdinalIgnoreCase);
    }
  }
}
