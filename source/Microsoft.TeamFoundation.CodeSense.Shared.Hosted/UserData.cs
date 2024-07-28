// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.UserData
// Assembly: Microsoft.TeamFoundation.CodeSense.Shared.Hosted, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2039D619-61FF-4054-B164-BD20C0E404E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Shared.Hosted.dll

using Newtonsoft.Json;

namespace Microsoft.TeamFoundation.CodeSense.Server
{
  public sealed class UserData
  {
    public UserData(string name)
      : this(name, name, string.Empty)
    {
    }

    public UserData(string name, string displayName)
      : this(name, displayName, string.Empty)
    {
    }

    [JsonConstructor]
    public UserData(string name, string displayName, string email)
    {
      this.UniqueName = name;
      this.DisplayName = displayName;
      this.Email = email;
    }

    [JsonProperty]
    public string UniqueName { get; private set; }

    [JsonProperty]
    public string DisplayName { get; private set; }

    [JsonProperty]
    public string Email { get; private set; }
  }
}
