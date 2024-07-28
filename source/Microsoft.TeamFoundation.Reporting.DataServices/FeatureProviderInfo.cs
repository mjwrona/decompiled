// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Reporting.DataServices.FeatureProviderInfo
// Assembly: Microsoft.TeamFoundation.Reporting.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0871DF71-209E-4628-905A-D95195A70FEC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Reporting.DataServices.dll

namespace Microsoft.TeamFoundation.Reporting.DataServices
{
  internal class FeatureProviderInfo
  {
    public FeatureProviderInfo(string scope, string assemblyLongName, string typeName)
    {
      this.Scope = scope;
      this.AssemblyLongName = assemblyLongName;
      this.TypeName = typeName;
    }

    public string Scope { get; set; }

    public string AssemblyLongName { get; set; }

    public string TypeName { get; set; }
  }
}
