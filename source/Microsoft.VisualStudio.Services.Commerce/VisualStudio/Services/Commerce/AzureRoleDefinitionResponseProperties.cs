// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.AzureRoleDefinitionResponseProperties
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class AzureRoleDefinitionResponseProperties
  {
    public string RoleName { get; set; }

    public string Type { get; set; }

    public string Description { get; set; }

    public List<string> AssignableScopes { get; set; }

    public List<Permission> Permissions { get; set; }

    public string CreatedOn { get; set; }

    public string UpdatedOn { get; set; }

    public object CreatedBy { get; set; }

    public object UpdatedBy { get; set; }
  }
}
