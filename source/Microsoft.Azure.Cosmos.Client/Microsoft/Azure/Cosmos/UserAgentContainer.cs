// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.UserAgentContainer
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Text.RegularExpressions;

namespace Microsoft.Azure.Cosmos
{
  internal class UserAgentContainer : Microsoft.Azure.Documents.UserAgentContainer
  {
    private const int MaxOperatingSystemString = 30;
    private const int MaxClientId = 10;
    private readonly string cosmosBaseUserAgent;
    private readonly string clientId;

    public UserAgentContainer(
      int clientId,
      string features = null,
      string regionConfiguration = "NS",
      string suffix = null)
    {
      this.clientId = Math.Min(clientId, 10).ToString();
      this.cosmosBaseUserAgent = this.CreateBaseUserAgentString(features, regionConfiguration);
      this.Suffix = suffix ?? string.Empty;
    }

    internal override string BaseUserAgent => this.cosmosBaseUserAgent ?? string.Empty;

    protected virtual void GetEnvironmentInformation(
      out string clientVersion,
      out string processArchitecture,
      out string operatingSystem,
      out string runtimeFramework)
    {
      EnvironmentInformation environmentInformation = new EnvironmentInformation();
      clientVersion = environmentInformation.ClientVersion;
      processArchitecture = environmentInformation.ProcessArchitecture;
      operatingSystem = environmentInformation.OperatingSystem;
      runtimeFramework = environmentInformation.RuntimeFramework;
    }

    private string CreateBaseUserAgentString(string features = null, string regionConfiguration = null)
    {
      string clientVersion;
      string processArchitecture;
      string operatingSystem;
      string runtimeFramework;
      this.GetEnvironmentInformation(out clientVersion, out processArchitecture, out operatingSystem, out runtimeFramework);
      if (operatingSystem.Length > 30)
        operatingSystem = operatingSystem.Substring(0, 30);
      string empty = string.Empty;
      string baseUserAgentString = "cosmos-netstandard-sdk/" + clientVersion + empty + Regex.Replace("|" + this.clientId + "|" + processArchitecture + "|" + operatingSystem + "|" + runtimeFramework + "|", "[^0-9a-zA-Z\\.\\|\\-]+", " ");
      if (!string.IsNullOrEmpty(regionConfiguration))
        baseUserAgentString = baseUserAgentString + regionConfiguration + "|";
      if (!string.IsNullOrEmpty(features))
        baseUserAgentString = baseUserAgentString + "F " + features + "|";
      return baseUserAgentString;
    }
  }
}
