// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityScope_BackCompat
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using System;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.Identity
{
  public class IdentityScope_BackCompat
  {
    public IdentityScope_BackCompat()
    {
    }

    internal IdentityScope_BackCompat(IdentityScope scope)
    {
      this.Id = scope.Id;
      this.LocalScopeId = scope.LocalScopeId;
      this.ParentId = scope.ParentId;
      this.ScopeType = scope.ScopeType;
      this.Name = scope.Name;
      this.Administrators = scope.Administrators.ToBackCompatDescriptor();
      this.IsGlobal = scope.IsGlobal;
      this.SecuringHostId = scope.SecuringHostId;
      this.IsActive = scope.IsActive;
    }

    public Guid Id { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    internal Guid LocalScopeId { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    public Guid ParentId { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    public GroupScopeType ScopeType { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    public string Name { get; set; }

    public IdentityDescriptor_BackCompat Administrators { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    public bool IsGlobal { get; set; }

    public Guid SecuringHostId { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    public bool IsActive { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }
  }
}
