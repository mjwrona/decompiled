// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.DemandExists
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  public sealed class DemandExists : Demand
  {
    public DemandExists(string name)
      : this(name, (ISecuredObject) null)
    {
    }

    public DemandExists(string name, ISecuredObject securedObject)
      : base(name, (string) null, securedObject)
    {
    }

    public override Demand Clone() => (Demand) new DemandExists(this.Name);

    protected override string GetExpression() => this.Name;
  }
}
