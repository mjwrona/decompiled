// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.CreateScopeInfo
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Identity
{
  [DataContract]
  public class CreateScopeInfo
  {
    public CreateScopeInfo()
    {
    }

    internal CreateScopeInfo(
      Guid parentScopeId,
      GroupScopeType scopeType,
      string scopeName,
      string adminGroupName,
      string adminGroupDescription,
      Guid creatorId)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(scopeName, nameof (scopeName));
      ArgumentUtility.CheckStringForNullOrEmpty(adminGroupName, nameof (adminGroupName));
      ArgumentUtility.CheckStringForNullOrEmpty(adminGroupDescription, "admingGroupDescription");
      this.ParentScopeId = parentScopeId;
      this.ScopeType = scopeType;
      this.ScopeName = scopeName;
      this.AdminGroupName = adminGroupName;
      this.AdminGroupDescription = adminGroupDescription;
      this.CreatorId = creatorId;
    }

    [DataMember]
    public Guid ParentScopeId { get; private set; }

    [DataMember]
    public GroupScopeType ScopeType { get; private set; }

    [DataMember]
    public string ScopeName { get; private set; }

    [DataMember]
    public string AdminGroupName { get; private set; }

    [DataMember]
    public string AdminGroupDescription { get; private set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid CreatorId { get; private set; }
  }
}
