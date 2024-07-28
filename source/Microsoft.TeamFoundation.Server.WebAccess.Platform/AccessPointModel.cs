// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.AccessPointModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  [DataContract]
  public class AccessPointModel
  {
    public AccessPointModel()
    {
    }

    public AccessPointModel(System.Uri uri)
    {
      this.Uri = UriUtility.GetInvariantAbsoluteUri(uri);
      this.Scheme = uri.Scheme;
      this.Authority = uri.Authority;
    }

    [DataMember(EmitDefaultValue = false)]
    public string Uri { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Scheme { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Authority { get; set; }
  }
}
