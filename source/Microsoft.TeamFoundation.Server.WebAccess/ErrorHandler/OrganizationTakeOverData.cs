// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.ErrorHandler.OrganizationTakeOverData
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.ErrorHandler
{
  [DataContract]
  public class OrganizationTakeOverData
  {
    [DataMember(EmitDefaultValue = false)]
    public string OrganizationTakeOverEndPoint { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string OrganizationSettingsEndPoint { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string UserSessionToken { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid NewOwnerId { get; set; }
  }
}
