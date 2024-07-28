// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.Authentication.CredentialProvider.CredentialProviderLoader
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Content.Common.Authentication.CredentialProvider
{
  internal class CredentialProviderLoader : ICredentialProviderLoader
  {
    private const string CredentialProviderPattern = "CredentialProvider*.exe";
    private const string CredentialProvidersEnvar = "ARTIFACT_CREDENTIALPROVIDERS_PATH";

    public IEnumerable<ICredentialProvider> FindCredentialProviders() => (IEnumerable<ICredentialProvider>) CredentialProviderLoader.FindAll(CredentialProviderLoader.ReadPathsFromEnvar("ARTIFACT_CREDENTIALPROVIDERS_PATH")).Select<string, PluginCredentialProvider>((Func<string, PluginCredentialProvider>) (location => new PluginCredentialProvider(location)));

    private static IEnumerable<string> FindAll(IEnumerable<string> directories)
    {
      List<string> all = new List<string>();
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      foreach (string path in directories.Where<string>(CredentialProviderLoader.\u003C\u003EO.\u003C0\u003E__Exists ?? (CredentialProviderLoader.\u003C\u003EO.\u003C0\u003E__Exists = new Func<string, bool>(Directory.Exists))))
      {
        List<string> list = Directory.EnumerateFiles(path, "CredentialProvider*.exe", SearchOption.AllDirectories).ToList<string>();
        list.Sort();
        all.AddRange((IEnumerable<string>) list);
      }
      return (IEnumerable<string>) all;
    }

    private static IEnumerable<string> ReadPathsFromEnvar(string key)
    {
      List<string> stringList = new List<string>();
      string environmentVariable = Environment.GetEnvironmentVariable(key);
      if (!string.IsNullOrEmpty(environmentVariable))
        stringList.AddRange((IEnumerable<string>) environmentVariable.Split(new char[1]
        {
          ';'
        }, StringSplitOptions.RemoveEmptyEntries));
      return (IEnumerable<string>) stringList;
    }
  }
}
