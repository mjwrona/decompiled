// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.TfsGitRefLogEntryUtilities
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Newtonsoft.Json;

namespace Microsoft.TeamFoundation.Git.Server
{
  public static class TfsGitRefLogEntryUtilities
  {
    public static string SerializeToJson(this TfsGitRefLogEntry tfsGitRefLogEntry) => JsonConvert.SerializeObject((object) tfsGitRefLogEntry);

    public static TfsGitRefLogEntry DeserializeFromJson(string jsonTfsGitRefLogEntry) => JsonConvert.DeserializeObject<TfsGitRefLogEntry>(jsonTfsGitRefLogEntry);
  }
}
