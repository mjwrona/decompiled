// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.TfsCredentialCacheEntry
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;

namespace Microsoft.VisualStudio.Services.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class TfsCredentialCacheEntry
  {
    private string m_targetName;
    private NetworkCredential m_credentials;
    private string m_comment;
    private Dictionary<string, string> m_attributes = new Dictionary<string, string>();

    public TfsCredentialCacheEntry(Uri uri, string userName, string password)
      : this(uri, userName, password, (string) null, CachedCredentialsType.Other, true)
    {
    }

    public TfsCredentialCacheEntry(
      Uri uri,
      string userName,
      string password,
      string comment,
      CachedCredentialsType type,
      bool nonInteractive)
      : this(uri.AbsoluteUri, new NetworkCredential(userName, password), comment, type, nonInteractive)
    {
    }

    public TfsCredentialCacheEntry(
      string targetName,
      NetworkCredential credentials,
      string comment,
      CachedCredentialsType type,
      bool nonInteractive)
    {
      this.TargetName = targetName;
      this.Credentials = credentials;
      if (!string.IsNullOrEmpty(comment))
        this.Comment = comment;
      this.Type = type;
      this.NonInteractive = nonInteractive;
    }

    public string TargetName
    {
      get => this.m_targetName;
      set => this.m_targetName = value;
    }

    public NetworkCredential Credentials
    {
      get => this.m_credentials;
      set => this.m_credentials = value;
    }

    public string Comment
    {
      get => this.m_comment;
      set => this.m_comment = value;
    }

    public CachedCredentialsType Type
    {
      get => this.ParseAttributeEnum<CachedCredentialsType>(CredentialsCacheConstants.CredentialsTypeKeyword, CachedCredentialsType.Other);
      set => this.Attributes[CredentialsCacheConstants.CredentialsTypeKeyword] = value.ToString();
    }

    public bool NonInteractive
    {
      get
      {
        string attribute = this.Attributes[CredentialsCacheConstants.NonInteractiveKeyword];
        bool result;
        return string.IsNullOrEmpty(attribute) || !bool.TryParse(attribute, out result) || result;
      }
      set => this.Attributes[CredentialsCacheConstants.NonInteractiveKeyword] = value.ToString();
    }

    public Dictionary<string, string> Attributes => this.m_attributes;

    private T ParseAttributeEnum<T>(string keyword, T defaultValue)
    {
      string attribute = this.Attributes[keyword];
      return !string.IsNullOrEmpty(attribute) && System.Enum.IsDefined(typeof (T), (object) this.Attributes[keyword]) ? (T) System.Enum.Parse(typeof (T), attribute) : defaultValue;
    }
  }
}
