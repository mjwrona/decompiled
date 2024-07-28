// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.ResourcesMoveRequestExtensions
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal static class ResourcesMoveRequestExtensions
  {
    internal static IEnumerable<string> GetCollectionNames(
      this ResourcesMoveRequestInternal resourcesMoveRequest)
    {
      return resourcesMoveRequest.ResourceIdentifiers.Where<ArmResourceIdentifier>((Func<ArmResourceIdentifier, bool>) (rId => string.Equals(rId.ResourceType, "account", StringComparison.OrdinalIgnoreCase))).Select<ArmResourceIdentifier, string>((Func<ArmResourceIdentifier, string>) (rId => rId.ResourceName));
    }

    internal static void SetIdentityInfo(
      this ResourcesMoveRequestInternal resourcesMoveRequest,
      HttpRequestHeaders headers)
    {
      resourcesMoveRequest.Upn = headers.GetValues("x-ms-client-principal-name").FirstOrDefault<string>();
      resourcesMoveRequest.TenantId = headers.GetValues("x-ms-client-tenant-id").FirstOrDefault<string>();
    }

    internal static void Validate(
      this ResourcesMoveRequestInternal resourcesMoveRequest,
      Guid subscriptionId,
      string resourceGroupName)
    {
      if (!resourcesMoveRequest.ResourceIdentifiers.Any<ArmResourceIdentifier>())
        throw new ArgumentException(HostingResources.AtleastOneResourceNeedToBeSpecified(), "Resources");
      ResourcesMoveRequestExtensions.ValidationCollectionNames(resourcesMoveRequest);
      ResourcesMoveRequestExtensions.EnsureResourcesAreFromTheSameResourceGroup(resourcesMoveRequest, subscriptionId, resourceGroupName);
    }

    internal static Guid GetTargetSubscriptionId(
      this ResourcesMoveRequestInternal resourcesMoveRequest)
    {
      return new Guid(ResourcesMoveRequestExtensions.GetTargetResourceGroupToken((ResourcesMoveRequest) resourcesMoveRequest, "subscriptions"));
    }

    internal static string GetTargetResourceGroupName(
      this ResourcesMoveRequestInternal resourcesMoveRequest)
    {
      return ResourcesMoveRequestExtensions.GetTargetResourceGroupToken((ResourcesMoveRequest) resourcesMoveRequest, "resourceGroups");
    }

    private static void EnsureResourcesAreFromTheSameResourceGroup(
      ResourcesMoveRequestInternal resourcesMoveRequest,
      Guid subscriptionId,
      string resourceGroupName)
    {
      if (resourcesMoveRequest.ResourceIdentifiers.Any<ArmResourceIdentifier>((Func<ArmResourceIdentifier, bool>) (resourceIdentifier => !resourceIdentifier.SubscriptionId.Equals(subscriptionId) || !string.Equals(resourceGroupName, resourceIdentifier.ResourceGroupName, StringComparison.OrdinalIgnoreCase))))
        throw new ArgumentException(HostingResources.SubscriptionId0OrResourceGroup1DoesNotMatch((object) subscriptionId, (object) resourceGroupName), "Resources");
    }

    private static void EnsureSourceAndTargetResourceGroupNamesMatch(
      ResourcesMoveRequestInternal resourcesMoveRequest)
    {
      string resourceGroupToken = ResourcesMoveRequestExtensions.GetTargetResourceGroupToken((ResourcesMoveRequest) resourcesMoveRequest, "resourceGroups");
      string resourceGroupName = resourcesMoveRequest.ResourceIdentifiers.First<ArmResourceIdentifier>().ResourceGroupName;
      if (!string.Equals(resourceGroupName, resourceGroupToken, StringComparison.OrdinalIgnoreCase))
        throw new ArgumentException(HostingResources.ResourceGroupNameCannotBeChangedWhileMovingResourcesSourceRG0TargetRG1((object) resourceGroupName, (object) resourceGroupToken), "TargetResourceGroup");
    }

    private static void ValidationCollectionNames(ResourcesMoveRequestInternal resourcesMoveRequest)
    {
      List<string> list = resourcesMoveRequest.GetCollectionNames().ToList<string>();
      if (list.Count < 1)
        throw new ArgumentException(HostingResources.ResourcesToMoveDoesNotContainAnyAccount(), "Resources");
      if (list.Count > 1)
        throw new ArgumentException(HostingResources.OnlyOneAccountCanBeMovedAtATime(), "Resources");
    }

    private static string GetTargetResourceGroupToken(
      ResourcesMoveRequest resourcesMoveRequest,
      string tokenName)
    {
      string[] source = resourcesMoveRequest.TargetResourceGroup.Trim('/').Split(new char[1]
      {
        '/'
      }, StringSplitOptions.RemoveEmptyEntries);
      if (source.Length != 4 || !string.Equals(source[0], "subscriptions", StringComparison.OrdinalIgnoreCase) || !string.Equals(source[2], "resourceGroups", StringComparison.OrdinalIgnoreCase) || !Guid.TryParse(source[1], out Guid _) || string.IsNullOrWhiteSpace(source[3]))
        throw new ArgumentException(HostingResources.TargetResourceGroupId0IsInvalid((object) resourcesMoveRequest.TargetResourceGroup), "TargetResourceGroup");
      return !string.Equals(tokenName, "subscriptions", StringComparison.OrdinalIgnoreCase) ? ((IEnumerable<string>) source).Last<string>() : source[1];
    }
  }
}
