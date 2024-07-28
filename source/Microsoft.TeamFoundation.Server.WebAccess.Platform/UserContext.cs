// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.UserContext
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  [DataContract]
  public class UserContext : ContextIdentifier
  {
    private Microsoft.VisualStudio.Services.Identity.Identity m_identity;

    public UserContext(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      this.m_identity = identity;
      this.Id = identity.Id;
      this.Name = identity.DisplayName;
      this.Email = identity.GetProperty<string>("Mail", (string) null);
      this.UniqueName = IdentityHelper.GetUniqueName(identity);
      SubjectDescriptor subjectDescriptor = identity.GetSubjectDescriptor(requestContext);
      this.SubjectType = subjectDescriptor.SubjectType;
      this.SubjectId = subjectDescriptor.Identifier;
    }

    public UserContext()
    {
    }

    [DataMember(EmitDefaultValue = false, Order = 24)]
    public string SubjectType { get; set; }

    [DataMember(EmitDefaultValue = false, Order = 25)]
    public string SubjectId { get; set; }

    [DataMember(EmitDefaultValue = false, Order = 30)]
    public string Email { get; set; }

    [DataMember(EmitDefaultValue = false, Order = 40)]
    public string UniqueName { get; set; }

    [DataMember(EmitDefaultValue = false, Order = 50)]
    public virtual bool LimitedAccess => false;
  }
}
