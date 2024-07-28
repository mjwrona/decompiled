// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityBatchInfo
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Identity
{
  [DataContract]
  public class IdentityBatchInfo
  {
    private IdentityBatchInfo()
    {
    }

    public IdentityBatchInfo(
      IList<SubjectDescriptor> subjectDescriptors,
      QueryMembership queryMembership = QueryMembership.None,
      IEnumerable<string> propertyNames = null,
      bool includeRestrictedVisibility = false)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) subjectDescriptors, nameof (subjectDescriptors));
      this.SubjectDescriptors = new List<SubjectDescriptor>((IEnumerable<SubjectDescriptor>) subjectDescriptors);
      this.QueryMembership = queryMembership;
      this.PropertyNames = propertyNames;
      this.IncludeRestrictedVisibility = includeRestrictedVisibility;
    }

    public IdentityBatchInfo(
      IList<IdentityDescriptor> descriptors,
      QueryMembership queryMembership = QueryMembership.None,
      IEnumerable<string> propertyNames = null,
      bool includeRestrictedVisibility = false)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) descriptors, nameof (descriptors));
      this.Descriptors = new List<IdentityDescriptor>((IEnumerable<IdentityDescriptor>) descriptors);
      this.QueryMembership = queryMembership;
      this.PropertyNames = propertyNames;
      this.IncludeRestrictedVisibility = includeRestrictedVisibility;
    }

    public IdentityBatchInfo(
      IList<Guid> identityIds,
      QueryMembership queryMembership = QueryMembership.None,
      IEnumerable<string> propertyNames = null,
      bool includeRestrictedVisibility = false)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) identityIds, nameof (identityIds));
      this.IdentityIds = new List<Guid>((IEnumerable<Guid>) identityIds);
      this.QueryMembership = queryMembership;
      this.PropertyNames = propertyNames;
      this.IncludeRestrictedVisibility = includeRestrictedVisibility;
    }

    public IdentityBatchInfo(
      IList<SocialDescriptor> socialDescriptors,
      QueryMembership queryMembership = QueryMembership.None,
      IEnumerable<string> propertyNames = null,
      bool includeRestrictedVisibility = false)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) socialDescriptors, nameof (socialDescriptors));
      this.SocialDescriptors = new List<SocialDescriptor>((IEnumerable<SocialDescriptor>) socialDescriptors);
      this.QueryMembership = queryMembership;
      this.PropertyNames = propertyNames;
      this.IncludeRestrictedVisibility = includeRestrictedVisibility;
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public List<IdentityDescriptor> Descriptors { get; private set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public List<SubjectDescriptor> SubjectDescriptors { get; private set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public List<Guid> IdentityIds { get; private set; }

    [DataMember]
    public QueryMembership QueryMembership { get; private set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public IEnumerable<string> PropertyNames { get; private set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool IncludeRestrictedVisibility { get; private set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public List<SocialDescriptor> SocialDescriptors { get; private set; }
  }
}
