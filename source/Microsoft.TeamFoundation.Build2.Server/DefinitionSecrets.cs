// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.DefinitionSecrets
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal class DefinitionSecrets
  {
    private readonly Dictionary<string, BuildDefinitionVariable> m_secretVariables = new Dictionary<string, BuildDefinitionVariable>();
    private readonly Dictionary<string, string> m_repositorySecrets = new Dictionary<string, string>();
    private const string c_strongBoxDefinitionDrawerFormat = "/Service/Build2/Definitions/{0}";
    private const string c_strongBoxDefinitionSecretVariableFormat = "{0}/Variables/{1}";
    private const string c_strongBoxDefinitionRepositorySecretFormat = "{0}/Repository/{1}";

    public static string GetDefinitionDrawerName(BuildDefinition definition) => string.Format("/Service/Build2/Definitions/{0}", (object) definition.Id);

    public static string GetDefinitionSecretVariableKey(
      BuildDefinition definition,
      string variableName)
    {
      return string.Format("{0}/Variables/{1}", (object) definition.Revision, (object) variableName).ToLowerInvariant();
    }

    public static string GetDefinitionRepositorySecretKey(
      BuildDefinition definition,
      string repositoryProperty)
    {
      return string.Format("{0}/Repository/{1}", (object) definition.Revision, (object) repositoryProperty).ToLowerInvariant();
    }

    public IDictionary<string, BuildDefinitionVariable> SecretVariables => (IDictionary<string, BuildDefinitionVariable>) this.m_secretVariables;

    public IDictionary<string, string> RepositorySecrets => (IDictionary<string, string>) this.m_repositorySecrets;
  }
}
