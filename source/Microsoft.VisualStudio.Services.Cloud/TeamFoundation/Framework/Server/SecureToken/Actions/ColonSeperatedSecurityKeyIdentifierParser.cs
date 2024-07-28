// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SecureToken.Actions.ColonSeperatedSecurityKeyIdentifierParser
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

namespace Microsoft.TeamFoundation.Framework.Server.SecureToken.Actions
{
  internal class ColonSeperatedSecurityKeyIdentifierParser : ISecurityKeyIdentifierParser
  {
    public void Decode(string encodedKey, out string signingKeyNamespace, out int keyId)
    {
      signingKeyNamespace = (string) null;
      keyId = int.MinValue;
      string[] strArray1;
      if (encodedKey == null)
        strArray1 = (string[]) null;
      else
        strArray1 = encodedKey.Split(':');
      string[] strArray2 = strArray1;
      if (strArray2.Length == 2 && !string.IsNullOrWhiteSpace(strArray2[0]) && int.TryParse(strArray2[1], out keyId))
        signingKeyNamespace = strArray2[0];
      this.Validate(signingKeyNamespace, keyId);
    }

    public string Encode(string signingKeyNamespace, int keyId)
    {
      this.Validate(signingKeyNamespace, keyId);
      return signingKeyNamespace + ":" + (object) keyId;
    }

    public void Validate(string signingKeyNamespace, int keyId)
    {
      if (string.IsNullOrWhiteSpace(signingKeyNamespace) || keyId <= 0)
        throw new InvalidTokenException("Invalid Key Information");
    }
  }
}
