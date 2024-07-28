// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.OAuthCallbackStateHelper
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
{
  public class OAuthCallbackStateHelper
  {
    public static string FormatOAuthCallbackState(
      Guid configurationId,
      Guid projectId,
      Uri callbackUrl,
      Uri redirectUri = null,
      bool redirectOnFailure = false,
      string AuthorizationUrl = null,
      string EndpointType = null,
      IDictionary<string, string> Properties = null)
    {
      return new OAuthCallbackStateHelper.OAuthCallbackState(configurationId, projectId, callbackUrl, redirectUri, redirectOnFailure, AuthorizationUrl, EndpointType, Properties).Serialize<OAuthCallbackStateHelper.OAuthCallbackState>();
    }

    public static OAuthCallbackStateHelper.OAuthCallbackState ParseOAuthCallbackState(string state)
    {
      try
      {
        return JsonUtilities.Deserialize<OAuthCallbackStateHelper.OAuthCallbackState>(state);
      }
      catch (Exception ex)
      {
        throw new OAuthEndpointException(ServiceEndpointSdkResources.Error_CallbackStateFormatIncorrect((object) ""));
      }
    }

    [DataContract]
    public class OAuthCallbackState
    {
      [DataMember]
      public Guid ConfigurationId { get; set; }

      [DataMember]
      public Guid ProjectId { get; set; }

      [DataMember(EmitDefaultValue = false)]
      public Uri RedirectUri { get; set; }

      [DataMember(EmitDefaultValue = false)]
      public Uri CallbackUri { get; set; }

      [DataMember(EmitDefaultValue = false)]
      public bool RedirectOnFailure { get; set; }

      [DataMember(EmitDefaultValue = false)]
      public string AuthorizationUrl { get; set; }

      [DataMember(EmitDefaultValue = false)]
      public string EndpointType { get; set; }

      [DataMember(EmitDefaultValue = false)]
      public IDictionary<string, string> Properties { get; set; }

      public OAuthCallbackState(
        Guid configurationId,
        Guid projectId,
        Uri callbackUri,
        Uri redirectUri,
        bool redirectOnFailure,
        string AuthorizationUrl,
        string EndpointType,
        IDictionary<string, string> properties)
      {
        this.ConfigurationId = configurationId;
        this.ProjectId = projectId;
        this.RedirectUri = redirectUri;
        this.RedirectOnFailure = redirectOnFailure;
        this.CallbackUri = callbackUri;
        this.AuthorizationUrl = AuthorizationUrl;
        this.EndpointType = EndpointType;
        this.Properties = properties;
      }
    }
  }
}
