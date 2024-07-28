// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.ProjectOverview.MembersData
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.ProjectOverview, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 97A9928B-E499-4978-909F-1EBC8C5535AE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.ProjectOverview.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.ProjectOverview
{
  [DataContract]
  public class MembersData : AbstractProjectSecuredObject
  {
    [DataMember]
    public bool IsCurrentUserAdmin { get; set; }

    [DataMember]
    public int Count { get; set; }

    [DataMember]
    public IList<MemberInfo> Members { get; set; }

    [DataMember]
    public bool HasMore { get; set; }

    public override void SetSecuredObject(ISecuredObject securedObject)
    {
      ArgumentUtility.CheckForNull<ISecuredObject>(securedObject, nameof (securedObject));
      base.SetSecuredObject(securedObject);
      AbstractProjectSecuredObject.SetSecuredObject(securedObject, (IEnumerable<AbstractProjectSecuredObject>) this.Members);
    }
  }
}
