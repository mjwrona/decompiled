// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.OrganizationSecurity
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.Organization
{
  public static class OrganizationSecurity
  {
    public static readonly Guid NamespaceId = new Guid("9BF6F8D7-772D-4C01-93F6-652A7A44CEF1");
    public static readonly string RootToken = "/";
    public static readonly char PathSeparator = '/';
    public static readonly string CollectionsToken = OrganizationSecurity.RootToken + "Collections";
    public static readonly string CollectionViolatedTermsToken = OrganizationSecurity.RootToken + "CollectionDeletion/ViolatedTerms";
    public static readonly string GracePeriodToken = OrganizationSecurity.RootToken + "CollectionDeletion" + OrganizationSecurity.PathSeparator.ToString() + "GracePeriod";
    public static readonly string PropertiesToken = OrganizationSecurity.RootToken + "Properties";
    public static readonly string TenantLinkingToken = OrganizationSecurity.RootToken + "TenantLinking";

    public static string GenerateCollectionToken(Guid? collectionId) => collectionId.HasValue ? string.Format("{0}{1}{2}", (object) OrganizationSecurity.CollectionsToken, (object) OrganizationSecurity.PathSeparator, (object) collectionId) : OrganizationSecurity.CollectionsToken;

    public static class Permissions
    {
      public const int Read = 1;
      public const int Create = 2;
      public const int Modify = 4;
      public const int Delete = 8;
      public const int SystemRestricted = 16;
      public const int AllExceptSystemRestricted = 15;
    }
  }
}
