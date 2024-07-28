// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.VssAadSettings
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Client
{
  public static class VssAadSettings
  {
    public const string DefaultAadInstance = "https://login.microsoftonline.com/";
    public const string CommonTenant = "common";
    public const string Resource = "499b84ac-1321-427f-aa17-267ca6975798";
    public const string Client = "872cd9fa-d31f-45e0-9eab-6e460a02d1f1";
    private const string ApplicationTenantId = "f8cdef31-a31e-4b4a-93e4-5f571e91255a";
    public static readonly string[] DefaultScopes = new string[1]
    {
      "499b84ac-1321-427f-aa17-267ca6975798/.default"
    };

    public static Uri NativeClientRedirectUri
    {
      get
      {
        Uri result = (Uri) null;
        try
        {
          string connectedUserValue = VssClientEnvironment.GetSharedConnectedUserValue<string>("AadNativeClientRedirect");
          if (!string.IsNullOrEmpty(connectedUserValue))
            Uri.TryCreate(connectedUserValue, UriKind.RelativeOrAbsolute, out result);
        }
        catch (Exception ex)
        {
        }
        Uri uri = result;
        return (object) uri != null ? uri : new Uri("https://login.microsoftonline.com/common/oauth2/nativeclient");
      }
    }

    public static string ClientId => VssClientEnvironment.GetSharedConnectedUserValue<string>("AadClientIdentifier") ?? "872cd9fa-d31f-45e0-9eab-6e460a02d1f1";

    public static string AadInstance
    {
      get
      {
        string aadInstance = VssClientEnvironment.GetSharedConnectedUserValue<string>(nameof (AadInstance));
        if (string.IsNullOrWhiteSpace(aadInstance))
          aadInstance = "https://login.microsoftonline.com/";
        else if (!aadInstance.EndsWith("/"))
          aadInstance += "/";
        return aadInstance;
      }
    }

    public static string ApplicationTenant => VssClientEnvironment.GetSharedConnectedUserValue<string>("AadApplicationTenant") ?? "f8cdef31-a31e-4b4a-93e4-5f571e91255a";
  }
}
