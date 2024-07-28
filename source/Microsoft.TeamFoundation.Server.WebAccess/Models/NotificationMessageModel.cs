// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Models.NotificationMessageModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.Azure.Boards.Settings;
using Microsoft.VisualStudio.Services.Common;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Models
{
  [DataContract]
  [Browsable(false)]
  public class NotificationMessageModel
  {
    public NotificationMessageModel(
      string id,
      MessageAreaType type,
      string header,
      string contentHtml = null,
      WebSettingsScope scope = WebSettingsScope.User)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(id, nameof (id));
      ArgumentUtility.CheckStringForNullOrEmpty(header, nameof (header));
      this.Id = id;
      this.Scope = scope;
      this.Type = type;
      this.Header = header;
      if (string.IsNullOrWhiteSpace(contentHtml))
        return;
      this.ContentHtml = contentHtml;
    }

    [DataMember(Name = "id")]
    public string Id { get; private set; }

    [DataMember(Name = "scope")]
    public WebSettingsScope Scope { get; private set; }

    [DataMember(Name = "type")]
    public MessageAreaType Type { get; private set; }

    [DataMember(Name = "header")]
    public string Header { get; private set; }

    [DataMember(Name = "content", EmitDefaultValue = false)]
    public string ContentHtml { get; private set; }
  }
}
