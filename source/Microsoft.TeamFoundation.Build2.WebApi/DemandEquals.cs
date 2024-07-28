// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.DemandEquals
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  public sealed class DemandEquals : Demand
  {
    public DemandEquals(string name, string value)
      : this(name, value, (ISecuredObject) null)
    {
    }

    public DemandEquals(string name, string value, ISecuredObject securedObject)
      : base(name, value, securedObject)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(value, nameof (value));
    }

    public override Demand Clone() => (Demand) new DemandEquals(this.Name, this.Value);

    protected override string GetExpression() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} -equals {1}", (object) this.Name, (object) this.Value);
  }
}
