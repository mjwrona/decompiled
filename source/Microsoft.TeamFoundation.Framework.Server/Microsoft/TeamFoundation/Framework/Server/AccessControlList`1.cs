// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AccessControlList`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public abstract class AccessControlList<T> : IAccessControlList, ICloneable where T : IAccessControlEntry, new()
  {
    public string Token;
    public bool InheritPermissions;
    protected List<T> m_aces;
    private const int c_insertionSortCap = 20;

    public AccessControlList(string token, bool inherit)
    {
      this.Token = token;
      this.InheritPermissions = inherit;
      this.m_aces = new List<T>();
    }

    public AccessControlList(string token, bool inherit, IEnumerable<T> aces)
      : this(token, inherit)
    {
      if (aces == null)
        return;
      this.m_aces.AddRange(aces);
      this.SortAndRemoveDuplicates();
    }

    public AccessControlList(AccessControlList<T> acl)
    {
      this.Token = acl.Token;
      this.InheritPermissions = acl.InheritPermissions;
      this.m_aces = new List<T>((IEnumerable<T>) acl.m_aces);
    }

    internal void SortAndRemoveDuplicates()
    {
      if (this.m_aces.Count <= 20)
      {
        for (int index1 = 1; index1 < this.m_aces.Count; ++index1)
        {
          T ace = this.m_aces[index1];
          int index2 = index1 - 1;
          int num;
          do
          {
            num = AccessControlList<T>.Compare(this.m_aces[index2].Descriptor, ace.Descriptor);
            if (num > 0)
            {
              this.m_aces[index2 + 1] = this.m_aces[index2];
              --index2;
            }
            else
              goto label_6;
          }
          while (index2 >= 0);
          this.m_aces[index2 + 1] = ace;
          continue;
label_6:
          if (num < 0)
          {
            this.m_aces[index2 + 1] = ace;
          }
          else
          {
            this.m_aces.RemoveAt(index2 + 1);
            --index1;
            this.m_aces[index2] = ace;
          }
        }
      }
      else
      {
        Dictionary<IdentityDescriptor, T> dictionary = new Dictionary<IdentityDescriptor, T>((IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
        foreach (T ace in this.m_aces)
          dictionary[ace.Descriptor] = ace;
        this.m_aces.Clear();
        this.m_aces.AddRange((IEnumerable<T>) dictionary.Values);
        this.m_aces.Sort((Comparison<T>) ((a, b) => AccessControlList<T>.Compare(a.Descriptor, b.Descriptor)));
      }
    }

    private int Seek(IdentityDescriptor key)
    {
      int num1 = 0;
      int num2 = this.m_aces.Count - 1;
      while (num1 <= num2)
      {
        int index = (num2 - num1) / 2 + num1;
        int num3 = AccessControlList<T>.Compare(this.m_aces[index].Descriptor, key);
        if (num3 < 0)
        {
          num1 = index + 1;
        }
        else
        {
          if (num3 <= 0)
            return index;
          num2 = index - 1;
        }
      }
      return ~num1;
    }

    private static int Compare(IdentityDescriptor x, IdentityDescriptor y)
    {
      int num = !x.IsTeamFoundationType() ? (!y.IsTeamFoundationType() ? 0 : -1) : (!y.IsTeamFoundationType() ? 1 : 0);
      if (num == 0)
        num = IdentityDescriptorComparer.Instance.Compare(x, y);
      return num;
    }

    public IEnumerable<T> AccessControlEntries => (IEnumerable<T>) this.m_aces;

    public int Count => this.m_aces.Count;

    public bool RemovePermissions(
      IdentityDescriptor descriptor,
      int permissionsToRemove,
      out int updatedAllow,
      out int updatedDeny)
    {
      ArgumentUtility.CheckForNull<IdentityDescriptor>(descriptor, nameof (descriptor));
      int index = this.Seek(descriptor);
      if (index < 0)
      {
        updatedAllow = 0;
        updatedDeny = 0;
        return false;
      }
      T ace = this.m_aces[index];
      SecurityUtility.MergePermissions(ace.Allow, ace.Deny, 0, 0, permissionsToRemove, out updatedAllow, out updatedDeny);
      ace.Allow = updatedAllow;
      ace.Deny = updatedDeny;
      if (typeof (T).IsValueType)
        this.m_aces[index] = ace;
      return true;
    }

    public bool RemoveAccessControlEntry(IdentityDescriptor descriptor)
    {
      ArgumentUtility.CheckForNull<IdentityDescriptor>(descriptor, nameof (descriptor));
      int index = this.Seek(descriptor);
      if (index < 0)
        return false;
      this.m_aces.RemoveAt(index);
      return true;
    }

    public T QueryAccessControlEntry(IdentityDescriptor descriptor)
    {
      ArgumentUtility.CheckForNull<IdentityDescriptor>(descriptor, nameof (descriptor));
      int index = this.Seek(descriptor);
      if (index >= 0)
        return this.m_aces[index];
      return new T() { Descriptor = descriptor };
    }

    public T SetAccessControlEntry(T newAce, bool merge)
    {
      int index = this.Seek(newAce.Descriptor);
      T obj1;
      if (index >= 0)
      {
        obj1 = this.m_aces[index];
        if (merge)
        {
          int updatedAllow;
          int updatedDeny;
          SecurityUtility.MergePermissions(obj1.Allow, obj1.Deny, newAce.Allow, newAce.Deny, 0, out updatedAllow, out updatedDeny);
          obj1.Allow = updatedAllow;
          obj1.Deny = updatedDeny;
        }
        else
        {
          ref T local1 = ref obj1;
          T obj2;
          if ((object) default (T) == null)
          {
            obj2 = local1;
            local1 = ref obj2;
          }
          int num = newAce.Allow & ~newAce.Deny;
          local1.Allow = num;
          ref T local2 = ref obj1;
          obj2 = default (T);
          if ((object) obj2 == null)
          {
            obj2 = local2;
            local2 = ref obj2;
          }
          int deny = newAce.Deny;
          local2.Deny = deny;
        }
        if (typeof (T).IsValueType)
          this.m_aces[index] = obj1;
      }
      else
      {
        obj1 = newAce;
        ref T local3 = ref obj1;
        ref T local4 = ref local3;
        if ((object) default (T) == null)
        {
          T obj3 = local4;
          local4 = ref obj3;
        }
        int num = local3.Allow & ~obj1.Deny;
        local4.Allow = num;
        this.m_aces.Insert(~index, obj1);
      }
      return obj1;
    }

    public IEnumerable<T> SetAccessControlEntries(IEnumerable<T> aces, bool merge)
    {
      ArgumentUtility.CheckForNull<IEnumerable<T>>(aces, nameof (aces));
      List<T> objList = new List<T>();
      foreach (T ace in aces)
        objList.Add(this.SetAccessControlEntry(ace, merge));
      return (IEnumerable<T>) objList;
    }

    public void RemoveZeroEntries()
    {
      for (int index = this.m_aces.Count - 1; index >= 0; --index)
      {
        if (this.m_aces[index].IsEmpty)
          this.m_aces.RemoveAt(index);
      }
    }

    string IAccessControlList.Token
    {
      get => this.Token;
      set => this.Token = value;
    }

    bool IAccessControlList.InheritPermissions
    {
      get => this.InheritPermissions;
      set => this.InheritPermissions = value;
    }

    IEnumerable<IAccessControlEntry> IAccessControlList.AccessControlEntries => this.m_aces.Cast<IAccessControlEntry>();

    int IAccessControlList.Count => this.Count;

    bool IAccessControlList.RemovePermissions(
      IdentityDescriptor descriptor,
      int permissionsToRemove,
      out int updatedAllow,
      out int updatedDeny)
    {
      return this.RemovePermissions(descriptor, permissionsToRemove, out updatedAllow, out updatedDeny);
    }

    bool IAccessControlList.RemoveAccessControlEntry(IdentityDescriptor descriptor) => this.RemoveAccessControlEntry(descriptor);

    IAccessControlEntry IAccessControlList.QueryAccessControlEntry(IdentityDescriptor descriptor) => (IAccessControlEntry) this.QueryAccessControlEntry(descriptor);

    public IAccessControlEntry SetAccessControlEntry(IAccessControlEntry newAce, bool merge) => (IAccessControlEntry) this.SetAccessControlEntry(this.Convert(newAce), merge);

    protected abstract T Convert(IAccessControlEntry ace);

    protected IEnumerable<T> Convert(IEnumerable<IAccessControlEntry> aces)
    {
      foreach (IAccessControlEntry ace in aces)
        yield return this.Convert(ace);
    }

    public abstract object Clone();
  }
}
