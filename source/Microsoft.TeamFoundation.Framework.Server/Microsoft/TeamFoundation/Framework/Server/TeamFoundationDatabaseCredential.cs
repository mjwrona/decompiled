// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationDatabaseCredential
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class TeamFoundationDatabaseCredential
  {
    private int m_id;
    private int m_databaseId;
    private bool m_isPrimaryCredential;
    private TeamFoundationDatabaseCredentialStatus m_credentialStatus;
    private string m_userId;
    private byte[] m_passwordEncrypted;
    private Guid m_signingKeyId;
    private string m_credentialName;
    private string m_description;

    internal void Initialize(
      int id,
      byte[] version,
      int databaseId,
      bool isPrimaryCredential,
      TeamFoundationDatabaseCredentialStatus credentialStatus,
      string userId,
      byte[] passwordEncrypted,
      Guid signingKeyId,
      DateTime? inUseStartTime,
      DateTime? inUseEndTime,
      string name,
      string description)
    {
      this.m_id = id;
      this.Version = version;
      this.m_databaseId = databaseId;
      this.m_isPrimaryCredential = isPrimaryCredential;
      this.m_credentialStatus = credentialStatus;
      this.m_userId = userId;
      this.m_passwordEncrypted = passwordEncrypted;
      this.m_signingKeyId = signingKeyId;
      this.InUseStartTime = inUseStartTime;
      this.InUseEndTime = inUseEndTime;
      this.m_credentialName = name;
      this.m_description = description;
    }

    public int Id => this.m_id;

    public int DatabaseId
    {
      get => this.m_databaseId;
      internal set
      {
        this.IsDatabaseIdDirty = true;
        this.m_databaseId = value;
      }
    }

    internal bool IsDatabaseIdDirty { get; private set; }

    public bool IsPrimaryCredential
    {
      get => this.m_isPrimaryCredential;
      private set
      {
        this.m_isPrimaryCredential = this.CredentialStatus == TeamFoundationDatabaseCredentialStatus.InUse && this.Name.Equals(DatabaseCredentialNames.DefaultCredential);
        this.IsIsPrimaryCredentialDirty = false;
      }
    }

    internal bool IsIsPrimaryCredentialDirty { get; private set; }

    public TeamFoundationDatabaseCredentialStatus CredentialStatus
    {
      get => this.m_credentialStatus;
      internal set
      {
        this.IsCredentialStatusDirty = true;
        this.m_credentialStatus = value;
      }
    }

    internal bool IsCredentialStatusDirty { get; private set; }

    public string UserId
    {
      get => this.m_userId;
      internal set
      {
        this.IsUserIdDirty = true;
        this.m_userId = value;
      }
    }

    internal bool IsUserIdDirty { get; private set; }

    public byte[] PasswordEncrypted
    {
      get => this.m_passwordEncrypted;
      internal set
      {
        this.IsPasswordEncryptedDirty = true;
        this.m_passwordEncrypted = value;
      }
    }

    internal bool IsPasswordEncryptedDirty { get; private set; }

    public Guid SigningKeyId
    {
      get => this.m_signingKeyId;
      internal set
      {
        this.IsSigningKeyIdDirty = true;
        this.m_signingKeyId = value;
      }
    }

    internal bool IsSigningKeyIdDirty { get; private set; }

    internal byte[] Version { get; private set; }

    public DateTime? InUseStartTime { get; private set; }

    public DateTime? InUseEndTime { get; private set; }

    public string Name
    {
      get => this.m_credentialName;
      internal set
      {
        this.IsNameDirty = true;
        this.m_credentialName = value;
      }
    }

    internal bool IsNameDirty { get; private set; }

    public string Description
    {
      get => this.m_description;
      internal set
      {
        this.IsDescriptionDirty = true;
        this.m_description = value;
      }
    }

    internal bool IsDescriptionDirty { get; private set; }

    internal bool IsUpdateRequired() => this.IsDatabaseIdDirty || this.IsIsPrimaryCredentialDirty || this.IsCredentialStatusDirty || this.IsUserIdDirty || this.IsPasswordEncryptedDirty || this.IsSigningKeyIdDirty || this.IsNameDirty || this.IsDescriptionDirty;

    internal void Updated()
    {
      this.IsDatabaseIdDirty = false;
      this.IsIsPrimaryCredentialDirty = false;
      this.IsCredentialStatusDirty = false;
      this.IsUserIdDirty = false;
      this.IsPasswordEncryptedDirty = false;
      this.IsSigningKeyIdDirty = false;
      this.IsNameDirty = false;
      this.IsDescriptionDirty = false;
    }
  }
}
