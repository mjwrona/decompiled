// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.TimeZoneList
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi
{
  [DataContract]
  public class TimeZoneList : SecuredObject
  {
    public TimeZoneList() => this.ValidTimeZones = (IList<TimeZone>) new List<TimeZone>();

    [DataMember]
    public IList<TimeZone> ValidTimeZones { get; private set; }

    [DataMember]
    public TimeZone UtcTimeZone { get; set; }

    public override void SetSecuredObject(Guid namespaceId, string token, int requiredPermissions)
    {
      base.SetSecuredObject(namespaceId, token, requiredPermissions);
      IList<TimeZone> validTimeZones = this.ValidTimeZones;
      if (validTimeZones != null)
        validTimeZones.ForEach<TimeZone>((Action<TimeZone>) (i => i.SetSecuredObject(namespaceId, token, requiredPermissions)));
      this.UtcTimeZone?.SetSecuredObject(namespaceId, token, requiredPermissions);
    }
  }
}
