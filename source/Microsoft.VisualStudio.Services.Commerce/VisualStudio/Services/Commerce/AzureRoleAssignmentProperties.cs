// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.AzureRoleAssignmentProperties
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class AzureRoleAssignmentProperties
  {
    public string RoleDefinitionId { get; set; }

    public string PrincipalId { get; set; }

    public string Scope { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }

    public string CreatedBy { get; set; }

    public string UpdatedBy { get; set; }
  }
}
