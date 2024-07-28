// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MSGraph.MicrosoftGraphServiceConstants
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

namespace Microsoft.VisualStudio.Services.MSGraph
{
  internal static class MicrosoftGraphServiceConstants
  {
    internal static class RegistryKeys
    {
      private const string Prefix = "/Service/MicrosoftGraph";
      internal const string NotificationFilter = "/Service/MicrosoftGraph/...";
      internal const string GraphApiResource = "/Service/MicrosoftGraph/GraphApiResource";
      internal const string GraphApiVersion = "/Service/MicrosoftGraph/GraphApiVersion";
    }

    internal static class Defaults
    {
      internal const string InvitationRedirectUrl = "https://www.microsoft.com";
    }
  }
}
