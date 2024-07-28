// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CloudConnected.CloudConnectedConstants
// Assembly: Microsoft.VisualStudio.Services.CloudConnected.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6AAFC756-39E6-4247-9102-7DC33B035E4F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CloudConnected.Server.dll

namespace Microsoft.VisualStudio.Services.CloudConnected
{
  public static class CloudConnectedConstants
  {
    public static readonly string ConnectedServerRoot = "/Configuration/ConnectedServer";
    public static readonly string ConnectedServerAccount = CloudConnectedConstants.ConnectedServerRoot + "/AccountName";
    public static readonly string ConnectedServerAccountId = CloudConnectedConstants.ConnectedServerRoot + "/AccountId";
    public static readonly string ConnectedServerSpsUrl = CloudConnectedConstants.ConnectedServerRoot + "/SpsUrl";
    public static readonly string ConnectedServerAuthorization = CloudConnectedConstants.ConnectedServerRoot + "/Authorization";
    public static readonly string ConnectedServerRegistrationId = CloudConnectedConstants.ConnectedServerAuthorization + "/RegistrationId";
    public static readonly string ConnectedServerAuthorizationUrl = CloudConnectedConstants.ConnectedServerAuthorization + "/Url";
    public static readonly string ConnectedServerAuthToken = CloudConnectedConstants.ConnectedServerRoot + "/AuthToken";
  }
}
