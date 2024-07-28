// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.FrameworkSecurity
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Framework.Common
{
  public static class FrameworkSecurity
  {
    public static readonly Guid FrameworkNamespaceId = new Guid("1f4179b3-6bac-4d01-b421-71ea09171400");
    public static readonly Guid EventSubscriptionNamespaceId = new Guid("58B176E7-3411-457a-89D0-C6D0CCB3C52B");
    public static readonly Guid EventSubscriberNamespaceId = new Guid("2BF24A2B-70BA-43D3-AD97-3D9E1F75622F");
    public static readonly Guid JobNamespaceId = new Guid("2a887f97-db68-4b7c-9ae3-5cebd7add999");
    public static readonly Guid RegistryNamespaceId = new Guid("4ae0db5d-8437-4ee8-a18b-1f6fb38bd34c");
    public static readonly Guid CollectionManagementNamespaceId = new Guid("f66fc5d6-60e1-443e-9d16-851364ce3b99");
    public static readonly Guid CatalogNamespaceId = new Guid("6BACCF73-1500-476f-8B2B-94F4489A59AA");
    public static readonly Guid IdentitiesNamespaceId = new Guid("5A27515B-CCD7-42c9-84F1-54C998F03866");
    public static readonly Guid Identities2NamespaceId = new Guid("C2EFB788-4DD2-4301-B2EE-EC8ED6955B4E");
    public static readonly Guid LocationNamespaceId = LocationSecurityConstants.NamespaceId;
    public static readonly Guid StrongBoxNamespaceId = new Guid("4A9E8381-289A-4DFD-8460-69028EAA93B3");
    public static readonly Guid DiagnosticNamespaceId = new Guid("A1178DF8-8630-4786-B2A0-3A580DDF63EA");
    public static readonly Guid TaggingNamespaceId = new Guid("BB50F182-8E5E-40B8-BC21-E8752A1E7AE2");
    public static readonly Guid TracingNamespaceId = new Guid("0F623D1C-A21B-4A66-B4AE-07DD445502FB");
    public static readonly Guid ProcessTemplatesNamespaceId = new Guid("3E65F728-F8BC-4ecd-8764-7E378B19BFA7");
    public static readonly Guid ProcessNamespaceId = new Guid("2DAB47F9-BD70-49ED-9BD5-8EB051E59C02");
    public static readonly Guid MessageQueueNamespaceId = new Guid("F3E9DDE6-32CD-48BB-B62D-1D73BCAF42F1");
    public static readonly string MessageQueueNamespaceRootToken = "Tfsmq";
    public static readonly char MessageQueuePathSeparator = '/';
    public static readonly string FrameworkNamespaceToken = "FrameworkGlobalSecurity";
    public static readonly string JobNamespaceToken = "AllJobs";
    public static readonly string CollectionManagementNamespaceToken = "AllCollections";
    public static readonly char CollectionManagementPathSeparator = '/';
    public static readonly char RegistryPathSeparator = '/';
    public static readonly string RegistryNamespaceRootToken = FrameworkSecurity.RegistryPathSeparator.ToString();
    public static readonly char IdentitySecurityPathSeparator = '\\';
    public static readonly string IdentitySecurityRootToken = "$";
    public static readonly char LocationPathSeparator = LocationSecurityConstants.PathSeparator;
    public static readonly string LocationNamespaceRootToken = LocationSecurityConstants.NamespaceRootToken;
    public static readonly string ServiceDefinitionsToken = LocationSecurityConstants.ServiceDefinitionsToken;
    public static readonly string AccessMappingsToken = LocationSecurityConstants.AccessMappingsToken;
    public static readonly char StrongBoxSecurityPathSeparator = '/';
    public static readonly string StrongBoxSecurityNamespaceRootToken = "StrongBox";
    public static readonly string ProcessTemplateNamespaceToken = "NAMESPACE";
    public static readonly char DiagnosticPathSeparator = '/';
    public static readonly string DiagnosticNamespaceToken = "Diagnostic";
    public static readonly Guid TeamProjectNamespaceId = TeamProjectSecurityConstants.NamespaceId;
    public static readonly Guid TeamProjectCollectionNamespaceId = new Guid("3E65F728-F8BC-4ecd-8764-7E378B19BFA7");
    public static readonly string TeamProjectCollectionNamespaceToken = "NAMESPACE";
    public static readonly string TracingNamespaceToken = "Global";
    public static readonly Guid ProxyNamespaceId = new Guid("CB4D56D2-E84B-457E-8845-81320A133FBB");
    public static readonly string ProxyNamespaceToken = "Proxy";
  }
}
