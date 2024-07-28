// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.TokenStorage.VssTokenKey
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

namespace Microsoft.VisualStudio.Services.Common.TokenStorage
{
  public class VssTokenKey
  {
    private string m_kind;
    private string m_resource;
    private string m_userName;
    private string m_type;
    private static readonly char[] s_invalidKindCharacters = new char[1]
    {
      '\\'
    };

    public VssTokenKey()
    {
    }

    public VssTokenKey(string kind, string resource, string userName, string type)
    {
      this.Kind = kind;
      this.Resource = resource;
      this.UserName = userName;
      this.Type = type;
    }

    public string Kind
    {
      get => this.m_kind;
      set
      {
        ArgumentUtility.CheckStringForNullOrWhiteSpace(value, "kind");
        ArgumentUtility.CheckStringForInvalidCharacters(value, "kind", VssTokenKey.s_invalidKindCharacters);
        this.m_kind = value;
      }
    }

    public string Resource
    {
      get => this.m_resource;
      set
      {
        ArgumentUtility.CheckStringForNullOrWhiteSpace(value, "resource");
        this.m_resource = value;
      }
    }

    public string UserName
    {
      get => this.m_userName;
      set
      {
        ArgumentUtility.CheckStringForNullOrWhiteSpace(value, nameof (value));
        this.m_userName = value;
      }
    }

    public string Type
    {
      get => this.m_type;
      set
      {
        ArgumentUtility.CheckStringForNullOrWhiteSpace(value, nameof (value));
        this.m_type = value;
      }
    }
  }
}
