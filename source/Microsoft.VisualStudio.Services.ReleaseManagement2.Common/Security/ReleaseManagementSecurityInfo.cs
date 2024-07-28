// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Common.Security.ReleaseManagementSecurityInfo
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C3F75541-7C8A-4AF6-A47E-709CEEE7550D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Common.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security;
using System;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Common.Security
{
  public class ReleaseManagementSecurityInfo
  {
    public ReleaseManagementSecurityPermissions Permission { get; set; }

    public Guid ProjectId { get; set; }

    public string Path { get; set; }

    public int ReleaseDefinitionId { get; set; }

    public int EnvironmentId { get; set; }
  }
}
