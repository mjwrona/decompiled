// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.OrganizationProperties
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.Organization
{
  public static class OrganizationProperties
  {
    public const string HostDeletedMessagePublishedCountPropertyName = "SystemProperty.HostDeletedMessagePublishedCount";
    public static Guid HostDeletedMessagePublishedCountPropertyKind = new Guid("F102B1E5-36FC-41A4-8FB8-4BDABD7B71D6");
    public static Guid OrganizationDeletionPropertyKind = new Guid("4D567E16-4C2A-455B-B0CB-BF4A3712F5C4");
    public const string DelayPhysicalDeletion = "SystemProperty.DelayPhysicalDeletion";
    public const string LastLogicalDeletedDate = "SystemProperty.LastLogicalDeletedDate";
    public const string Description = "SystemProperty.Description";
    public const string PrivacyUrl = "SystemProperty.PrivacyUrl";
    public const string Logo = "SystemProperty.Logo";

    private static class SystemProperty
    {
      internal const string Namespace = "SystemProperty";
      internal const string NamespaceSeparator = ".";
      internal const string HostDeletedMessagePublishedCountToken = "HostDeletedMessagePublishedCount";
      internal const string DescriptionToken = "Description";
      internal const string PrivacyUrlToken = "PrivacyUrl";
      internal const string DelayPhysicalDeletionToken = "DelayPhysicalDeletion";
      internal const string LastLogicalDeletedDateToken = "LastLogicalDeletedDate";
      internal const string LogoToken = "Logo";
    }
  }
}
