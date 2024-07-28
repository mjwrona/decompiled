// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ServiceBusWellKnownNamespaces
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public static class ServiceBusWellKnownNamespaces
  {
    public static HashSet<string> NamespacesForTest = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      "vststest",
      "tfstestsb",
      "tfstestsb2",
      "vstsrmodevfabric",
      "codexsbp0",
      "codexsbp1",
      "codexsbp2",
      "codexsbp3",
      "codexsbp4",
      "codexsbp5",
      "codexsbp6",
      "codexsbp7",
      "codexsbp8",
      "codexsbp9"
    };
    public static HashSet<string> NamespacesWithReducedDeleteOnIdleTimeForSubscriptions = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      "testsecretrotationsb"
    };
    public static HashSet<string> NamespacesWithoutHighAvailability = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      "testsecretrotationsb",
      "ppesecretrotation",
      "prodsecretrotationsb"
    };
    public static HashSet<string> LargePremiumTopicNamespace = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      "AzureDevOpsSBCUS1Prod",
      "AzureDevOpsSB1WEUProd",
      "AzureDevOpsSB2WEUProd",
      "AzureDevOpsSBEUS2Prod",
      "AzureDevOpsSBWUS2Prod",
      "AzureDevOpsSBUKSProd",
      "AzureDevOpsSBCCAZRProd",
      "AzureDevOpsSBSEAProd",
      "AzureDevOpsSBEAUZRProd",
      "AzureDevOpsSBSBRZRProd",
      "AzureDevOpsSBCINProd",
      "AzureDevOpsSBWUS3Prod",
      "AzureDevOpsSBCUS1PPE",
      "AzureDevOpsSBEUS2PPE",
      "tfsppech1su1"
    };

    public static bool IsTestNamespace(
      IVssRequestContext requestContext,
      string serviceBusNamespace)
    {
      return ServiceBusWellKnownNamespaces.IsWellKnownNamespace(requestContext, serviceBusNamespace, ServiceBusWellKnownNamespaces.NamespacesForTest);
    }

    public static bool IsNamespaceWithReducedDeleteOnIdleTimeForSubscriptions(
      IVssRequestContext requestContext,
      string serviceBusNamespace)
    {
      return ServiceBusWellKnownNamespaces.IsWellKnownNamespace(requestContext, serviceBusNamespace, ServiceBusWellKnownNamespaces.NamespacesWithReducedDeleteOnIdleTimeForSubscriptions);
    }

    public static bool IsLargeTopicSizePremiumNamespace(
      IVssRequestContext requestContext,
      string serviceBusNamespace)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(serviceBusNamespace, nameof (serviceBusNamespace));
      return ServiceBusWellKnownNamespaces.LargePremiumTopicNamespace.Contains(serviceBusNamespace);
    }

    public static bool IsNotHighAvailabilityNamespace(
      IVssRequestContext requestContext,
      string serviceBusNamespace)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(serviceBusNamespace, nameof (serviceBusNamespace));
      return ServiceBusWellKnownNamespaces.NamespacesWithoutHighAvailability.Contains(serviceBusNamespace);
    }

    private static bool IsWellKnownNamespace(
      IVssRequestContext requestContext,
      string serviceBusNamespace,
      HashSet<string> wellKnownNamespaces)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(serviceBusNamespace, nameof (serviceBusNamespace));
      return !requestContext.ServiceHost.IsProduction && wellKnownNamespaces.Contains(serviceBusNamespace);
    }
  }
}
