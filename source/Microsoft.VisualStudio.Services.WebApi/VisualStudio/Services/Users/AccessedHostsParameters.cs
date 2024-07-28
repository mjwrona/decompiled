// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Users.AccessedHostsParameters
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Users
{
  [DataContract]
  public class AccessedHostsParameters
  {
    public AccessedHostsParameters()
    {
    }

    public AccessedHostsParameters(
      SubjectDescriptor userDescriptor,
      IList<AccessedHost> accessedHosts)
    {
      this.UserDescriptor = userDescriptor;
      this.AccessedHosts = accessedHosts;
    }

    [DataMember]
    public SubjectDescriptor UserDescriptor { get; set; }

    [DataMember]
    public IList<AccessedHost> AccessedHosts { get; set; }
  }
}
