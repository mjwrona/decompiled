// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.Keychain.SessionTokenDescriptor
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using Microsoft.VisualStudio.Services.Common.TokenStorage;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Client.Keychain
{
  public class SessionTokenDescriptor
  {
    private string scope;
    private string lifeTime;
    private IList<Guid> targetAccounts;
    private SessionTokenType tokenType;
    private const string TokenStorageScopePrefix = "Scope:";
    private const string TokenStorageTargetAccountsPrefix = "TargetAccounts:";

    public string Scope => this.scope;

    public string LifeTime => this.lifeTime;

    public IList<Guid> TargetAccounts => this.targetAccounts;

    public SessionTokenType TokenType => this.tokenType;

    public SessionTokenDescriptor()
    {
      this.scope = string.Empty;
      this.lifeTime = string.Empty;
      this.targetAccounts = (IList<Guid>) null;
      this.tokenType = SessionTokenType.SelfDescribing;
    }

    public SessionTokenDescriptor(
      string scope,
      SessionTokenType tokenType = SessionTokenType.SelfDescribing,
      string lifeTime = null,
      IList<Guid> targetAccounts = null)
    {
      this.scope = scope;
      this.lifeTime = lifeTime;
      this.targetAccounts = targetAccounts;
      this.tokenType = tokenType;
    }

    public static SessionTokenDescriptor FromKey(VssToken token, SessionTokenType tokenType = SessionTokenType.SelfDescribing)
    {
      string empty = string.Empty;
      string orjString1 = string.Empty;
      string userName = token.UserName;
      int num = userName.IndexOf("TargetAccounts:", StringComparison.OrdinalIgnoreCase);
      string orjString2;
      if (num == -1)
      {
        orjString2 = userName;
      }
      else
      {
        orjString2 = userName.Substring(0, num);
        orjString1 = userName.Remove(0, num);
      }
      orjString2.IndexOf("Scope:", StringComparison.OrdinalIgnoreCase);
      string scope = SessionTokenDescriptor.CleanString(orjString2, "Scope:");
      List<Guid> targetAccounts = (List<Guid>) null;
      if (!string.IsNullOrEmpty(orjString1))
      {
        string[] strArray = SessionTokenDescriptor.CleanString(orjString1, "TargetAccounts:").Split(new char[1]
        {
          ';'
        }, StringSplitOptions.RemoveEmptyEntries);
        if (strArray.Length != 0)
        {
          targetAccounts = new List<Guid>();
          foreach (string input in strArray)
            targetAccounts.Add(Guid.Parse(input));
        }
      }
      return new SessionTokenDescriptor(scope, tokenType, targetAccounts: (IList<Guid>) targetAccounts);
    }

    private static string CleanString(string orjString, string prefix) => orjString.IndexOf(prefix, StringComparison.OrdinalIgnoreCase) == 0 ? orjString.Remove(0, prefix.Length) : orjString;

    public string ToKey()
    {
      if (this.scope.StartsWith("Scope:", StringComparison.OrdinalIgnoreCase))
        throw new ArgumentException(string.Format("Given scope value '{0}' has already a prefix '{1}'", (object) this.scope, (object) "Scope:"));
      string str = "Scope:" + this.scope;
      if (this.TargetAccounts != null && this.TargetAccounts.Count > 0)
      {
        str += "TargetAccounts:";
        foreach (Guid targetAccount in (IEnumerable<Guid>) this.TargetAccounts)
          str = str + targetAccount.ToString() + ";";
      }
      return str.ToLowerInvariant();
    }

    public override int GetHashCode()
    {
      int num1 = 0;
      int num2 = this.scope != null ? num1 * 397 ^ this.scope.GetHashCode() : num1;
      int num3 = (this.lifeTime != null ? num2 * 397 ^ this.lifeTime.GetHashCode() : num2) * 397 ^ this.tokenType.GetHashCode();
      return this.targetAccounts != null ? num3 * 397 ^ this.targetAccounts.GetHashCode() : num3;
    }

    public override bool Equals(object obj)
    {
      if (obj == null || this.GetType() != obj.GetType())
        return false;
      SessionTokenDescriptor sessionTokenDescriptor = (SessionTokenDescriptor) obj;
      return string.Equals(this.Scope, sessionTokenDescriptor.Scope, StringComparison.OrdinalIgnoreCase) && string.Equals(this.LifeTime, sessionTokenDescriptor.LifeTime, StringComparison.OrdinalIgnoreCase) && sessionTokenDescriptor.TokenType.Equals((object) this.TokenType) && (sessionTokenDescriptor.TargetAccounts == this.TargetAccounts || sessionTokenDescriptor.TargetAccounts.SequenceEqual<Guid>((IEnumerable<Guid>) this.TargetAccounts));
    }
  }
}
