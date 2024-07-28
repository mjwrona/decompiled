// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.DatabaseLoginInfo
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class DatabaseLoginInfo
  {
    public string CredentialName { get; private set; }

    public string UserId { get; private set; }

    public string Password { get; private set; }

    public byte[] Sid { get; private set; }

    public DatabaseLoginInfo(string userId, string password, string credentialName, byte[] sid = null)
    {
      this.UserId = userId;
      this.Password = password;
      this.CredentialName = credentialName;
      this.Sid = sid;
    }
  }
}
