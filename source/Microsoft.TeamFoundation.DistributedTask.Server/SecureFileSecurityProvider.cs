// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.SecureFileSecurityProvider
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  public class SecureFileSecurityProvider : LibrarySecurityProvider
  {
    private static readonly string SecureFile = nameof (SecureFile);

    public static string GetToken(string itemId) => SecureFileSecurityProvider.SecureFile + (object) DefaultSecurityProvider.NamespaceSeparator + itemId;

    protected override string GetTokenSuffix(string itemId) => SecureFileSecurityProvider.GetToken(itemId);
  }
}
