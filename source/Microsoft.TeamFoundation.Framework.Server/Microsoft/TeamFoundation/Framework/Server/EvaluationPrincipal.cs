// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.EvaluationPrincipal
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class EvaluationPrincipal
  {
    private static readonly IdentityDescriptorComparer s_descriptorComparer = IdentityDescriptorComparer.Instance;
    private static readonly IReadOnlyList<IdentityDescriptor> s_emptyIdentityDescriptorList = (IReadOnlyList<IdentityDescriptor>) new List<IdentityDescriptor>().AsReadOnly();
    private IReadOnlyList<IdentityDescriptor> _roleDescriptors;

    public EvaluationPrincipal(IdentityDescriptor primaryDescriptor, bool isMembershipIdentity = true)
      : this(primaryDescriptor, isMembershipIdentity ? primaryDescriptor : (IdentityDescriptor) null, (IEnumerable<IdentityDescriptor>) null)
    {
    }

    public EvaluationPrincipal(
      IdentityDescriptor primaryDescriptor,
      IEnumerable<IdentityDescriptor> roleDescriptors)
      : this(primaryDescriptor, primaryDescriptor, roleDescriptors)
    {
    }

    public EvaluationPrincipal(
      IdentityDescriptor primaryDescriptor,
      IdentityDescriptor membershipDescriptor,
      IEnumerable<IdentityDescriptor> roleDescriptors)
    {
      ArgumentUtility.CheckForNull<IdentityDescriptor>(primaryDescriptor, nameof (primaryDescriptor));
      this.PrimaryDescriptor = primaryDescriptor;
      this.MembershipDescriptor = membershipDescriptor;
      if (roleDescriptors == null || !roleDescriptors.Any<IdentityDescriptor>())
        return;
      List<IdentityDescriptor> list = roleDescriptors.ToList<IdentityDescriptor>();
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      list.Sort(EvaluationPrincipal.\u003C\u003EO.\u003C0\u003E__Compare ?? (EvaluationPrincipal.\u003C\u003EO.\u003C0\u003E__Compare = new Comparison<IdentityDescriptor>(EvaluationPrincipal.Compare)));
      for (int index = list.Count - 1; index > 0; --index)
      {
        if (EvaluationPrincipal.s_descriptorComparer.Equals(list[index], list[index - 1]))
          list.RemoveAt(index);
      }
      this._roleDescriptors = (IReadOnlyList<IdentityDescriptor>) list.AsReadOnly();
    }

    public static implicit operator EvaluationPrincipal(IdentityDescriptor descriptor)
    {
      ArgumentUtility.CheckForNull<IdentityDescriptor>(descriptor, nameof (descriptor));
      return new EvaluationPrincipal(descriptor);
    }

    public IdentityDescriptor PrimaryDescriptor { get; }

    public IdentityDescriptor MembershipDescriptor { get; }

    public IReadOnlyList<IdentityDescriptor> RoleDescriptors => this._roleDescriptors ?? EvaluationPrincipal.s_emptyIdentityDescriptorList;

    public EvaluationPrincipal ToWellKnownEvaluationPrincipal(IdentityMapper mapper)
    {
      bool flag = (object) mapper.MapToWellKnownIdentifier(this.PrimaryDescriptor) != (object) this.PrimaryDescriptor;
      if (!flag && (IdentityDescriptor) null != this.MembershipDescriptor)
        flag = (object) mapper.MapToWellKnownIdentifier(this.MembershipDescriptor) != (object) this.MembershipDescriptor;
      if (!flag && this.RoleDescriptors.Any<IdentityDescriptor>())
      {
        foreach (IdentityDescriptor roleDescriptor in (IEnumerable<IdentityDescriptor>) this.RoleDescriptors)
        {
          if ((object) roleDescriptor != (object) mapper.MapToWellKnownIdentifier(roleDescriptor))
          {
            flag = true;
            break;
          }
        }
      }
      return !flag ? this : new EvaluationPrincipal(mapper.MapToWellKnownIdentifier(this.PrimaryDescriptor), (IdentityDescriptor) null != this.MembershipDescriptor ? mapper.MapToWellKnownIdentifier(this.MembershipDescriptor) : (IdentityDescriptor) null, this.RoleDescriptors.Select<IdentityDescriptor, IdentityDescriptor>((Func<IdentityDescriptor, IdentityDescriptor>) (s => mapper.MapToWellKnownIdentifier(s))));
    }

    public override bool Equals(object obj)
    {
      EvaluationPrincipal evaluationPrincipal = obj as EvaluationPrincipal;
      if (this == evaluationPrincipal)
        return true;
      if (!EvaluationPrincipal.s_descriptorComparer.Equals(this.PrimaryDescriptor, evaluationPrincipal.PrimaryDescriptor) || !EvaluationPrincipal.s_descriptorComparer.Equals(this.MembershipDescriptor, evaluationPrincipal.MembershipDescriptor) || this.RoleDescriptors.Count != evaluationPrincipal.RoleDescriptors.Count)
        return false;
      for (int index = 0; index < this.RoleDescriptors.Count; ++index)
      {
        if (!EvaluationPrincipal.s_descriptorComparer.Equals(this.RoleDescriptors[index], evaluationPrincipal.RoleDescriptors[index]))
          return false;
      }
      return true;
    }

    public override int GetHashCode()
    {
      int hashCode = EvaluationPrincipal.s_descriptorComparer.GetHashCode(this.PrimaryDescriptor);
      if (this.MembershipDescriptor != (IdentityDescriptor) null)
        hashCode = 3307 * hashCode + EvaluationPrincipal.s_descriptorComparer.GetHashCode(this.MembershipDescriptor);
      if (this.RoleDescriptors.Any<IdentityDescriptor>())
      {
        int num = 0;
        foreach (IdentityDescriptor roleDescriptor in (IEnumerable<IdentityDescriptor>) this.RoleDescriptors)
          num ^= EvaluationPrincipal.s_descriptorComparer.GetHashCode(roleDescriptor);
        hashCode = 307 * hashCode + num;
      }
      return hashCode;
    }

    private static int Compare(IdentityDescriptor x, IdentityDescriptor y)
    {
      int num = !x.IsTeamFoundationType() ? (!y.IsTeamFoundationType() ? 0 : -1) : (!y.IsTeamFoundationType() ? 1 : 0);
      if (num == 0)
        num = EvaluationPrincipal.s_descriptorComparer.Compare(x, y);
      return num;
    }
  }
}
