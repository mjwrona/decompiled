// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.ErrorHandler.AuthExceptionData
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.ErrorHandler
{
  [DataContract]
  public class AuthExceptionData
  {
    [DataMember(EmitDefaultValue = false)]
    public string RequestAccessEndpoint { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string EmailAddress { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string OrganizationName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string UserSessionToken { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string UrlRequested { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ProjectUri { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool IsPolicyEnabled { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string RequestAccessUrl { get; set; }
  }
}
