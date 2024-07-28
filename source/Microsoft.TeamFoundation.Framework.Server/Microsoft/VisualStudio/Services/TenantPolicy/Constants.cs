// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.TenantPolicy.Constants
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.TenantPolicy
{
  public static class Constants
  {
    public const string JsonPatchMediaType = "application/json-patch+json";
    public const string TenantRoleTemplateIdsRegistryPath = "/Service/Aad/TenantRoleTemplateIds";
    public const string BypassCheckPermissionToken = "TenantPolicy.CheckPermission.Bypass";
    public const string DefaultTenantAdminRoleTemplateIds = "e3973bdf-4987-49ae-837a-ba8e231c7286";
    public static readonly Guid GenevaServiceGuid = new Guid("0000005B-0000-8888-8000-000000000000");
    public static readonly Guid TokenServiceGuid = new Guid("00000052-0000-8888-8000-000000000000");
    public const int DefaultPATMaxLifespanDays = 30;
  }
}
