// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.DirectoryEntityViewModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.VisualStudio.Services.Directories;
using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public class DirectoryEntityViewModel : IdentityViewModelBase
  {
    private IDirectoryEntity m_Entity;
    private string m_DisplayName;
    private string m_IdentityType;
    private string m_Scope;
    private string m_SubHeader;
    private bool isUser;
    private bool isGroup;

    public override string IdentityType => this.m_IdentityType;

    public override string SubHeader => this.m_SubHeader;

    public override string FriendlyDisplayName => this.m_DisplayName;

    public DirectoryEntityViewModel(IDirectoryEntity entity)
    {
      this.m_Entity = entity != null ? entity : throw new ArgumentException("Entity cannot be null");
      Guid result;
      this.TeamFoundationId = this.m_Entity.OriginDirectory == null || !(this.m_Entity.OriginDirectory.ToLower().Trim() == "vsd".ToLower().Trim()) || !Guid.TryParse(this.m_Entity.OriginId, out result) ? Guid.Empty : result;
      this.m_Scope = this.m_Entity.ScopeName;
      this.m_DisplayName = this.m_Entity.DisplayName;
      this.m_IdentityType = this.m_Entity.EntityType.Trim().ToLower();
      this.isUser = this.m_Entity.EntityType != null && this.m_Entity.EntityType.ToLower().Trim() == "User".ToLower().Trim() && this.m_Entity is IDirectoryUser;
      this.isGroup = this.m_Entity.EntityType != null && this.m_Entity.EntityType.ToLower().Trim() == "Group".ToLower().Trim() && this.m_Entity is IDirectoryGroup;
      if (this.isUser)
      {
        IDirectoryUser entity1 = this.m_Entity as IDirectoryUser;
        this.m_SubHeader = this.FormatString(!string.IsNullOrWhiteSpace(entity1.SignInAddress) ? entity1.SignInAddress : string.Empty);
        this.DisplayName = this.m_DisplayName;
      }
      else
      {
        if (!this.isGroup)
          return;
        this.m_SubHeader = this.FormatString(this.m_Entity.ScopeName, "[{0}]");
        this.DisplayName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[{0}]\\{1}", (object) this.m_Scope, (object) this.m_DisplayName);
      }
    }

    public override JsObject ToJson()
    {
      if (this.m_Entity == null)
        throw new DirectoryEntityViewModelException("Could not find entity for serialization");
      JsObject json = base.ToJson();
      IDirectoryUser entity1 = this.m_Entity as IDirectoryUser;
      IDirectoryGroup entity2 = this.m_Entity as IDirectoryGroup;
      json["EntityId"] = (object) this.m_Entity.EntityId;
      if (this.isUser)
      {
        json["AccountName"] = (object) this.FormatString(!string.IsNullOrWhiteSpace(entity1.SignInAddress) ? entity1.SignInAddress : string.Empty);
        json["MailAddress"] = (object) this.FormatString(!string.IsNullOrWhiteSpace(entity1.SignInAddress) ? entity1.SignInAddress : (!string.IsNullOrWhiteSpace(entity1.Mail) ? entity1.Mail : string.Empty));
        json["Domain"] = (object) this.FormatString(this.m_Entity.ScopeName);
        json["IsWindowsUser"] = (object) (bool) (string.Equals(this.m_Entity.OriginDirectory, "ad", StringComparison.OrdinalIgnoreCase) ? 1 : (string.Equals(this.m_Entity.OriginDirectory, "wmd", StringComparison.OrdinalIgnoreCase) ? 1 : 0));
      }
      if (this.isGroup)
      {
        json["MailAddress"] = (object) this.FormatString(entity2.Mail);
        json["Scope"] = (object) this.FormatString(this.m_Scope);
        json["Description"] = (object) this.FormatString(entity2.Description);
        json["IsAadGroup"] = (object) string.Equals(this.m_Entity.OriginDirectory, "aad", StringComparison.OrdinalIgnoreCase);
        json["IsWindowsGroup"] = (object) (bool) (string.Equals(this.m_Entity.OriginDirectory, "ad", StringComparison.OrdinalIgnoreCase) ? 1 : (string.Equals(this.m_Entity.OriginDirectory, "wmd", StringComparison.OrdinalIgnoreCase) ? 1 : 0));
      }
      return json;
    }

    private string FormatString(string input, string formatString = null)
    {
      if (formatString == null)
        formatString = "{0}";
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, formatString, (object) input);
    }
  }
}
