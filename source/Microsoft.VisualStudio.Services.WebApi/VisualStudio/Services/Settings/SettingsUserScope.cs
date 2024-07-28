// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Settings.SettingsUserScope
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Settings
{
  [DataContract]
  public struct SettingsUserScope
  {
    private const string c_globalMeScopeName = "globalme";
    private const string c_meScopeName = "me";
    private const string c_hostScopeName = "host";
    public static SettingsUserScope GlobalUser = new SettingsUserScope(true, true, Guid.Empty);
    public static SettingsUserScope User = new SettingsUserScope(false, true, Guid.Empty);
    public static SettingsUserScope AllUsers = new SettingsUserScope(false, false, Guid.Empty);

    public static SettingsUserScope SpecificUser(Guid userId) => new SettingsUserScope(false, true, userId);

    public static SettingsUserScope Parse(string identifier)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(identifier, nameof (identifier));
      if (string.Equals("me", identifier, StringComparison.OrdinalIgnoreCase))
        return SettingsUserScope.User;
      if (string.Equals("host", identifier, StringComparison.OrdinalIgnoreCase))
        return SettingsUserScope.AllUsers;
      if (string.Equals("globalme", identifier, StringComparison.OrdinalIgnoreCase))
        return SettingsUserScope.GlobalUser;
      Guid result;
      if (Guid.TryParse(identifier, out result))
        return SettingsUserScope.SpecificUser(result);
      throw new ArgumentException("userId");
    }

    private SettingsUserScope(bool isGlobalScoped, bool isUserScoped, Guid userId)
    {
      this.IsUserScoped = isUserScoped;
      this.IsGlobalScoped = isGlobalScoped;
      this.UserId = userId;
      if (isGlobalScoped && !isUserScoped)
        throw new ArgumentException("Only user scope can be global");
    }

    [DataMember(EmitDefaultValue = true)]
    public Guid UserId { get; private set; }

    [DataMember]
    public bool IsUserScoped { get; private set; }

    [DataMember]
    public bool IsGlobalScoped { get; private set; }

    public override string ToString()
    {
      if (!this.IsUserScoped)
        return "host";
      return this.UserId == Guid.Empty ? "me" : this.UserId.ToString();
    }
  }
}
