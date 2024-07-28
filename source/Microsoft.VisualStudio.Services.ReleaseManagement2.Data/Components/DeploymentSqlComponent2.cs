// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.DeploymentSqlComponent2
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  public class DeploymentSqlComponent2 : DeploymentSqlComponent
  {
    protected override void BindMaxDeployments(int maxDeployments) => this.BindNullableInt(nameof (maxDeployments), new int?(maxDeployments));

    protected override void BindGroupByEnvironment(bool groupByEnvironment) => this.BindNullableBoolean(nameof (groupByEnvironment), new bool?(groupByEnvironment));
  }
}
