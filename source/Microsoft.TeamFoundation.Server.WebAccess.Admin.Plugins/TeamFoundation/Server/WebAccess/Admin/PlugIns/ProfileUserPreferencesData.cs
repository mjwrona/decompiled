// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns.ProfileUserPreferencesData
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.Models;
using Microsoft.VisualStudio.Services.WebPlatform;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns
{
  [DataContract]
  public class ProfileUserPreferencesData
  {
    [DataMember(EmitDefaultValue = false)]
    public IdentityData UserIdentity { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Culture { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string DatePattern { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string TimePattern { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string TimeZone { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string OldProfileUrl { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public WebSessionToken NamedSessionToken { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ProfilePreferencesModel ProfilePreferences { get; set; }
  }
}
