// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.Client.GraphScope
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Graph.Client
{
  [DataContract]
  public class GraphScope : GraphSubject
  {
    public override string SubjectKind => "scope";

    public SubjectDescriptor AdministratorDescriptor { get; private set; }

    [DataMember(Name = "AdministratorDescriptor", IsRequired = false, EmitDefaultValue = false)]
    private string AdministratorString
    {
      get => this.AdministratorDescriptor.ToString();
      set => this.AdministratorDescriptor = SubjectDescriptor.FromString(value);
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool IsGlobal { get; private set; }

    public SubjectDescriptor ParentDescriptor { get; private set; }

    [DataMember(Name = "ParentDescriptor", IsRequired = false, EmitDefaultValue = false)]
    private string ParentDescriptorString
    {
      get => this.ParentDescriptor.ToString();
      set => this.ParentDescriptor = SubjectDescriptor.FromString(value);
    }

    public SubjectDescriptor SecuringHostDescriptor { get; private set; }

    [DataMember(Name = "SecuringHostDescriptor", IsRequired = false, EmitDefaultValue = false)]
    private string SecuringHostDescriptorString
    {
      get => this.SecuringHostDescriptor.ToString();
      set => this.SecuringHostDescriptor = SubjectDescriptor.FromString(value);
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public GroupScopeType ScopeType { get; private set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal GraphScope(
      string origin,
      string originId,
      SubjectDescriptor descriptor,
      IdentityDescriptor legacyDescriptor,
      string displayName,
      ReferenceLinks links,
      string url,
      SubjectDescriptor administratorDescriptor,
      bool isGlobal,
      SubjectDescriptor parentDescriptor,
      SubjectDescriptor securingHostDescriptor,
      GroupScopeType scopeType = GroupScopeType.Generic)
      : base(origin, originId, descriptor, legacyDescriptor, displayName, links, url)
    {
      this.AdministratorDescriptor = administratorDescriptor;
      this.IsGlobal = isGlobal;
      this.ParentDescriptor = parentDescriptor;
      this.SecuringHostDescriptor = securingHostDescriptor;
      this.ScopeType = scopeType;
    }

    internal GraphScope(
      GraphScope scope,
      string origin = null,
      string originId = null,
      SubjectDescriptor? descriptor = null,
      IdentityDescriptor legacyDescriptor = null,
      string displayName = null,
      ReferenceLinks links = null,
      string url = null,
      SubjectDescriptor? administrator = null,
      bool? isGlobal = null,
      SubjectDescriptor? parentDescriptor = null,
      SubjectDescriptor? securingHostDescriptor = null,
      GroupScopeType? scopeType = GroupScopeType.Generic)
    {
      string origin1 = origin ?? scope?.Origin;
      string originId1 = originId ?? scope?.OriginId;
      SubjectDescriptor? nullable = descriptor;
      SubjectDescriptor descriptor1 = nullable ?? (scope != null ? scope.Descriptor : new SubjectDescriptor());
      IdentityDescriptor legacyDescriptor1 = legacyDescriptor;
      if ((object) legacyDescriptor1 == null)
        legacyDescriptor1 = scope?.LegacyDescriptor ?? (IdentityDescriptor) null;
      string displayName1 = displayName ?? scope?.DisplayName;
      ReferenceLinks links1 = links ?? scope?.Links;
      string url1 = url ?? scope?.Url;
      nullable = administrator;
      SubjectDescriptor administratorDescriptor = nullable ?? (scope != null ? scope.AdministratorDescriptor : new SubjectDescriptor());
      int num = (int) isGlobal ?? (scope != null ? (scope.IsGlobal ? 1 : 0) : 0);
      nullable = parentDescriptor;
      SubjectDescriptor parentDescriptor1 = nullable ?? (scope != null ? scope.ParentDescriptor : new SubjectDescriptor());
      nullable = securingHostDescriptor;
      SubjectDescriptor securingHostDescriptor1 = nullable ?? (scope != null ? scope.SecuringHostDescriptor : new SubjectDescriptor());
      int scopeType1 = (int) scopeType ?? (scope != null ? (int) scope.ScopeType : 0);
      // ISSUE: explicit constructor call
      this.\u002Ector(origin1, originId1, descriptor1, legacyDescriptor1, displayName1, links1, url1, administratorDescriptor, num != 0, parentDescriptor1, securingHostDescriptor1, (GroupScopeType) scopeType1);
    }

    protected GraphScope()
    {
    }
  }
}
