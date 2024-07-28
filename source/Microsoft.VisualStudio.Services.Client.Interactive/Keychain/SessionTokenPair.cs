// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.Keychain.SessionTokenPair
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using Newtonsoft.Json;

namespace Microsoft.VisualStudio.Services.Client.Keychain
{
  internal class SessionTokenPair
  {
    public SessionTokenPair()
      : this((string) null, (string) null)
    {
    }

    [JsonConstructor]
    public SessionTokenPair(string selfDescribing, string compact)
    {
      this.SelfDescribing = selfDescribing;
      this.Compact = compact;
    }

    public string GetToken(SessionTokenType tokenType)
    {
      if (tokenType == SessionTokenType.SelfDescribing)
        return this.SelfDescribing;
      return tokenType == SessionTokenType.Compact ? this.Compact : string.Empty;
    }

    public string SelfDescribing { get; private set; }

    public string Compact { get; private set; }

    public string Serialize() => JsonConvert.SerializeObject((object) this);

    public static SessionTokenPair GetTokenPair(string tokenValue) => SessionTokenPair.TryDeserialize(tokenValue) ?? new SessionTokenPair(tokenValue, (string) null);

    private static SessionTokenPair TryDeserialize(string tokenValue)
    {
      SessionTokenPair sessionTokenPair = (SessionTokenPair) null;
      try
      {
        sessionTokenPair = JsonConvert.DeserializeObject<SessionTokenPair>(tokenValue);
      }
      catch
      {
      }
      return sessionTokenPair;
    }
  }
}
