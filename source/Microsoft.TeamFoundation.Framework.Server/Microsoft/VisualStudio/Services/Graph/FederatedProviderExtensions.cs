// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.FederatedProviderExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Graph
{
  public static class FederatedProviderExtensions
  {
    private static readonly IReadOnlyDictionary<FederatedProvider, string> NamesByProvider = (IReadOnlyDictionary<FederatedProvider, string>) new Dictionary<FederatedProvider, string>()
    {
      {
        FederatedProvider.GitHub,
        "github.com"
      }
    };
    private const string Area = "Graph";
    private const string Layer = "FederatedProviderExtensions";

    public static bool HasUpn(this IReadOnlyVssIdentity identity) => !string.IsNullOrEmpty(identity.GetProperty<string>("Account", string.Empty));

    public static bool HasEmaillessUpn(this IReadOnlyVssIdentity identity)
    {
      string property = identity.GetProperty<string>("Account", string.Empty);
      ArgumentUtility.CheckStringForNullOrEmpty(property, "AccountName");
      return !ArgumentUtility.IsValidEmailAddress(property);
    }

    public static bool IsFederatedWith(
      this IReadOnlyVssIdentity identity,
      FederatedProvider provider,
      IVssRequestContext context,
      [CallerMemberName] string callerName = null)
    {
      return context.IsFederatedWith(provider, identity.GetSubjectDescriptor(context), callerName);
    }

    public static bool IsFederatedWith(
      this IVssRequestContext context,
      FederatedProvider provider,
      SubjectDescriptor descriptor = default (SubjectDescriptor),
      [CallerMemberName] string callerName = null)
    {
      return FederatedProviderExtensions.GetProviderData(context, provider, FederatedProviderExtensions.SelectProvidedOrAuthenticatedDescriptor(context, descriptor), callerName, nameof (IsFederatedWith)) != null;
    }

    public static bool HasFederatedTokenFor(
      this IReadOnlyVssIdentity identity,
      FederatedProvider provider,
      IVssRequestContext context,
      [CallerMemberName] string callerName = null)
    {
      return context.HasFederatedTokenFor(provider, identity.GetSubjectDescriptor(context), nameof (HasFederatedTokenFor));
    }

    public static bool HasFederatedTokenFor(
      this IVssRequestContext context,
      FederatedProvider provider,
      SubjectDescriptor descriptor = default (SubjectDescriptor),
      [CallerMemberName] string callerName = null)
    {
      return !string.IsNullOrEmpty(FederatedProviderExtensions.GetProviderData(context, provider, FederatedProviderExtensions.SelectProvidedOrAuthenticatedDescriptor(context, descriptor), callerName, nameof (HasFederatedTokenFor))?.AccessToken);
    }

    internal static SubjectDescriptor SelectProvidedOrAuthenticatedDescriptor(
      IVssRequestContext context,
      SubjectDescriptor descriptor)
    {
      if (descriptor != new SubjectDescriptor())
        return descriptor;
      ArgumentUtility.CheckForNull<IdentityDescriptor>(context.UserContext, "context.UserContext");
      return context.UserContext.ToSubjectDescriptor(context);
    }

    internal static bool IsFederatableUserType(this SubjectDescriptor subjectDescriptor) => subjectDescriptor.IsMsaUserType();

    private static GraphFederatedProviderData GetProviderData(
      IVssRequestContext context,
      FederatedProvider provider,
      SubjectDescriptor descriptor,
      string callerName,
      [CallerMemberName] string operation = null)
    {
      if (!descriptor.IsFederatableUserType())
      {
        context.TraceDataConditionally(6059182, TraceLevel.Verbose, "Graph", nameof (FederatedProviderExtensions), "Shortcircuiting call for subject which is of a type that cannot be federated", (Func<object>) (() => (object) new
        {
          operation = operation,
          callerName = callerName,
          descriptor = descriptor,
          provider = provider
        }), nameof (GetProviderData));
        return (GraphFederatedProviderData) null;
      }
      string providerName = FederatedProviderExtensions.NamesByProvider.GetValueOrDefault<FederatedProvider, string>(provider);
      if (providerName == null)
        throw new ArgumentException(string.Format("Provider {0} not recognized", (object) provider), nameof (provider));
      GraphFederatedProviderData providerData = context.GetService<IGraphFederatedProviderService>().AcquireProviderData(context, descriptor, providerName);
      if (providerData == null)
        context.TraceDataConditionally(6059183, TraceLevel.Verbose, "Graph", nameof (FederatedProviderExtensions), "Could not acquire provider data", (Func<object>) (() => (object) new
        {
          operation = operation,
          callerName = callerName,
          descriptor = descriptor,
          provider = provider,
          providerName = providerName
        }), nameof (GetProviderData));
      else
        context.TraceDataConditionally(6059184, TraceLevel.Verbose, "Graph", nameof (FederatedProviderExtensions), "Acquired provider data", (Func<object>) (() => (object) new
        {
          operation = operation,
          callerName = callerName,
          descriptor = descriptor,
          provider = provider,
          providerName = providerName,
          providerData = providerData
        }), nameof (GetProviderData));
      return providerData;
    }
  }
}
