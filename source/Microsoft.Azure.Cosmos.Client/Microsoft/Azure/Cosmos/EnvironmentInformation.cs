// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.EnvironmentInformation
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System.Reflection;
using System.Runtime.InteropServices;

namespace Microsoft.Azure.Cosmos
{
  internal sealed class EnvironmentInformation
  {
    private static readonly string clientSDKVersion;
    private static readonly string framework;
    private static readonly string architecture;
    private static readonly string os;

    static EnvironmentInformation()
    {
      System.Version version = Assembly.GetAssembly(typeof (CosmosClient)).GetName().Version;
      EnvironmentInformation.clientSDKVersion = string.Format("{0}.{1}.{2}", (object) version.Major, (object) version.Minor, (object) version.Build);
      EnvironmentInformation.framework = RuntimeInformation.FrameworkDescription;
      EnvironmentInformation.architecture = RuntimeInformation.ProcessArchitecture.ToString();
      EnvironmentInformation.os = RuntimeInformation.OSDescription;
    }

    public string ClientVersion => EnvironmentInformation.clientSDKVersion;

    public string OperatingSystem => EnvironmentInformation.os;

    public string RuntimeFramework => EnvironmentInformation.framework;

    public string ProcessArchitecture => EnvironmentInformation.architecture;
  }
}
