// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.IServiceEndpointOperationsExtension
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.ComponentModel.Composition;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server
{
  [InheritedExport]
  public interface IServiceEndpointOperationsExtension
  {
    string SupportedEndpointType { get; }

    void PreCreateOperations(IVssRequestContext requestContext, ServiceEndpoint endpoint);

    void PostCreateOperations(
      IVssRequestContext requestContext,
      ServiceEndpoint endpoint,
      Guid scopeIdentifier,
      ServiceEndpointType serviceEndpointType = null);

    void PreUpdateOperations(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      ServiceEndpoint endpoint,
      ServiceEndpoint existingEndpoint,
      string operation);

    void PostUpdateOperations(
      IVssRequestContext requestContext,
      ServiceEndpoint endpoint,
      ServiceEndpoint existingEndpoint,
      Guid scopeIdentifier,
      string operation,
      ServiceEndpointType serviceEndpointType);

    void PreDeleteOperations(
      IVssRequestContext requestContext,
      ServiceEndpoint endpoint,
      Guid scopeIdentifier);

    void PostDeleteOperations(
      IVssRequestContext systemRequestContext,
      ServiceEndpoint endpoint,
      Guid scopeIdentifier);

    void PostGetOperations(
      IVssRequestContext requestContext,
      ServiceEndpoint endpoint,
      Guid scopeIdentifier,
      ServiceEndpointType serviceEndpointType);

    void RefreshAuthenticationDataIfRequired(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      ServiceEndpoint endpoint,
      ServiceEndpointType endpointType,
      RefreshAuthenticationParameters refreshAuthenticationParameters);

    bool SupportsUpgradeBetween(string currentScheme, string newScheme);

    bool SupportsDowngradeBetween(string currentScheme, string oldScheme);

    bool TryAuthenticateWithServiceEndpoint(
      IVssRequestContext requestContext,
      ServiceEndpoint endpoint);

    void PreUpgradeOperations(
      IVssRequestContext requestContext,
      ServiceEndpoint existingEndpoint,
      ServiceEndpointType endpointType);

    void ProcessUpgradeOperations(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      ServiceEndpoint existingEndpoint,
      ServiceEndpointType endpointType);

    void PostUpgradeOperations(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      ServiceEndpoint upgradedEndpoint,
      ServiceEndpointType endpointType);
  }
}
