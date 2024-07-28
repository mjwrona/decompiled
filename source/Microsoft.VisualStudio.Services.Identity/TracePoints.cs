// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.TracePoints
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

namespace Microsoft.VisualStudio.Services.Identity
{
  internal static class TracePoints
  {
    private const int IdentityIdTranslatorStart = 80340;
    private const int IdentityIdTranslatorEnd = 80349;
    public const int IdentitiesTranslateFromMasterException = 80341;
    public const int IdentitiesTranslateToMasterException = 80342;
    private const int IdentitiesRestServicesStart = 800500;
    private const int IdentitiesRestServicesEnd = 800699;
    private const int IdentitiesController = 800500;
    private const int IdentityBatchController = 850550;
    internal const int GetIdentityChangesStart = 850000;
    internal const int GetIdentityChangesException = 850008;
    internal const int GetIdentityChangesEnd = 850009;
    internal const int IdentitySelfControllerException = 800051;
    internal const int ReadIdentityWithTenantStart = 800052;
    internal const int ReadIdentityWithTenantEnd = 800053;
    internal const int GetOrCreateHomeTenantIdentityStart = 800054;
    internal const int GetOrCreateHomeTenantIdentityNoTenantsFound = 800055;
    internal const int GetOrCreateHomeTenantIdentityHomeTenantsFound = 800056;
    internal const int GetOrCreateHomeTenantIdentityNoHomeTenantsFound = 800057;
    internal const int GetOrCreateHomeTenantIdentityCreateHomeTenantIdentity = 800058;
    internal const int GetOrCreateHomeTenantIdentityEnd = 800059;
    internal const int GetIdentityTenantsEnter = 2011601;
    internal const int GetIdentityTenantsValueNull = 2011602;
    internal const int GetIdentityTenantsCache = 2011603;
    internal const int GetIdentityTenantsNoCache = 2011604;
    internal const int GetIdentityTenantsExit = 2011604;
    internal const int ReadIdentityBatchStart = 850550;
    internal const int ReadIdentityBatchException = 850558;
    internal const int ReadIdentityBatchEnd = 850559;
    internal const int ReadIdentitiesByIdsStart = 850560;
    internal const int ReadIdentitiesByIdsEnd = 850569;
    internal const int ReadIdentitiesByDescriptorsStart = 850570;
    internal const int ReadIdentitiesByDescriptorsEnd = 850579;
    internal const int SilentProfileCreationFailed = 850610;
    internal const int CreateIdentityForAadGroupFailed = 850621;
    internal const int CopyAadGroupFromAadToIdentityFailed = 850622;
    internal const int AadServiceReturnNonExistingGroup = 850623;
    internal const int CopyAadGroupFromAadToIdentityDisabled = 850624;
    internal const int CopyAadGroupFromAadToIdentitySucceed = 850625;
    internal const int AccountNotHavingTenant = 850626;
    internal const int ExceptionFromGetAzureActiveDirectoryTenantId = 850627;
    internal const int MaterializedAadGroupsEmpty = 850628;
    internal const int SourceAadGroupsEmpty = 850629;
    internal const int FilteredAadGroupsEmpty = 850630;
    internal const int CopyDirectoryAliasForIdentity = 850631;
    internal const int CopyDirectoryAliasForIdentitySucceed = 850632;
    internal const int CopyDirectoryAliasForIdentityFailed = 850633;
    internal const int CopyDirectoryAliasForIdentityDisabled = 850634;
    internal const int CopyDirectoryAliasInvalidAggregateIdentity = 850635;
    internal const int CopyDirectoryAliasTotalIdentityFound = 850636;
    internal const int CopyDirectoryAliasTotalIdentityUpdated = 850637;
    internal const int OrganizationNotFoundExceptionFromGetAzureActiveDirectoryTenantId = 850638;
    internal const int CopyDirectoryAliasAadAccessException = 850639;
    internal const int CopyDirectoryAliasAadException = 850640;
  }
}
