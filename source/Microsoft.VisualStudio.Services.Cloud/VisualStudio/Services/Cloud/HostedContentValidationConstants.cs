// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.HostedContentValidationConstants
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal static class HostedContentValidationConstants
  {
    public const string CvsAPIMKeyLightrailSetting = "CvsAPIMKey";
    public const string CvsClientCertLightrailSetting = "CvsAuthCertificate";
    public const string AvertAPIMKeyLightrailSetting = "AvertAPIMKey";
    public static readonly RegistryQuery AllowedSniDomainNamesQuery = new RegistryQuery("/Service/ContentValidation/CvsAllowedSniNames", false);
    public static readonly RegistryQuery CvsApiEndpointQuery = new RegistryQuery("/Service/ContentValidation/CvsEndpointUri", false);
    public static readonly RegistryQuery CvsCallbackEndpointQuery = new RegistryQuery("/Service/ContentValidation/CvsCallbackUri", false);
    public static readonly RegistryQuery CvsContactNotificationEmailQuery = new RegistryQuery("/Service/ContentValidation/CvsContactNotificationEmail", false);
    public static readonly RegistryQuery AvertApiEndpointQuery = new RegistryQuery("/Service/ContentValidation/AvertEndpointUri", false);
    public static readonly RegistryQuery AvertCallbackEndpointQuery = new RegistryQuery("/Service/ContentValidation/AvertCallbackUri", false);
  }
}
