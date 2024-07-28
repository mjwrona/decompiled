// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.HostMigrationSecurityConstants
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public static class HostMigrationSecurityConstants
  {
    public const string AllMigrationsToken = "AllMigrations";
    public static readonly Guid HostMigrationNamespaceId = Guid.Parse("D5E491D7-165B-4678-B6D9-FB864C991CF4");
    public const string SasRequestToken = "SasRequestPermissions";
    public static readonly Guid SasRequestNamespaceId = Guid.Parse("D4EB3132-1F8D-4D77-B79D-284EC40E4A47");
  }
}
