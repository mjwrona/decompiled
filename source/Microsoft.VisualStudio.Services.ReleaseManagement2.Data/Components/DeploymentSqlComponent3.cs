// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.DeploymentSqlComponent3
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using System;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  public class DeploymentSqlComponent3 : DeploymentSqlComponent2
  {
    protected override void BindModifiedTime(DateTime? minModifiedTime, DateTime? maxModifiedTime)
    {
      this.BindNullableDateTime(nameof (minModifiedTime), minModifiedTime);
      this.BindNullableDateTime(nameof (maxModifiedTime), maxModifiedTime);
    }
  }
}
