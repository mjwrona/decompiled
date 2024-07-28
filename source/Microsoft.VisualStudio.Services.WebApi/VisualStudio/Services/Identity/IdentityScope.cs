// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityScope
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Identity
{
  [DataContract]
  public class IdentityScope
  {
    private SubjectDescriptor subjectDescriptor;

    internal IdentityScope()
    {
    }

    internal IdentityScope(IdentityScope other)
      : this(other.Id, other.Name)
    {
      this.Administrators = other.Administrators == (IdentityDescriptor) null ? (IdentityDescriptor) null : new IdentityDescriptor(other.Administrators);
      this.IsActive = other.IsActive;
      this.IsGlobal = other.IsGlobal;
      this.LocalScopeId = other.LocalScopeId;
      this.ParentId = other.ParentId;
      this.ScopeType = other.ScopeType;
      this.SecuringHostId = other.SecuringHostId;
    }

    internal IdentityScope(Guid id, string name)
    {
      ArgumentUtility.CheckForEmptyGuid(id, nameof (id));
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      this.Id = id;
      this.Name = name;
    }

    [DataMember(IsRequired = true)]
    public Guid Id { get; internal set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    internal Guid LocalScopeId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid ParentId { get; internal set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public GroupScopeType ScopeType { get; internal set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public IdentityDescriptor Administrators { get; internal set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool IsGlobal { get; internal set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid SecuringHostId { get; internal set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool IsActive { get; internal set; }

    public IdentityScope Clone() => new IdentityScope(this);

    public override string ToString() => string.Format("[Id={0}, Name={1}, LocalScopeId={2}, ParentId={3}, ScopeType={4}, SecuringHostId={5}, Administrators={6}, IsActive={7}, IsGlobal={8}]", (object) this.Id, (object) this.Name, (object) this.LocalScopeId, (object) this.ParentId, (object) this.ScopeType, (object) this.SecuringHostId, (object) this.Administrators, (object) this.IsActive, (object) this.IsGlobal);

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public SubjectDescriptor SubjectDescriptor
    {
      get
      {
        if (this.subjectDescriptor == new SubjectDescriptor())
          this.subjectDescriptor = new SubjectDescriptor("scp", this.Id.ToString());
        return this.subjectDescriptor;
      }
    }
  }
}
