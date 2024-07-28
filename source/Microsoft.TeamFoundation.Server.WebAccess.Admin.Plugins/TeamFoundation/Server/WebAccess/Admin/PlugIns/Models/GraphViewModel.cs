// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.Models.GraphViewModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using Microsoft.CSharp.RuntimeBinder;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Directories;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.Models
{
  [DataContract]
  public class GraphViewModel
  {
    [DataMember(EmitDefaultValue = false)]
    public Guid IdentityId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Description { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string PrincipalName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string OriginId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string SubjectKind { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string MetaType { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string DisplayName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Domain { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string MailAddress { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ApplicationId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Scope { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Descriptor { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool IsDefaultTeam { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool IsAadGroup { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool IsWellKnownGroup { get; set; }

    [DataMember(EmitDefaultValue = true)]
    public string EntityId { get; set; }

    [DataMember(EmitDefaultValue = true)]
    public string EntityType { get; set; }

    [DataMember(EmitDefaultValue = true)]
    public string Sid { get; set; }

    public GraphViewModel(TeamFoundationIdentity identity, Guid collectionHostId)
    {
      bool property = identity.TryGetProperty(TeamConstants.TeamPropertyName, out object _);
      if (property || identity.IsContainer)
      {
        object obj1 = !property ? (object) new GroupIdentityViewModel(identity) : (object) new TeamIdentityViewModel(identity);
        // ISSUE: reference to a compiler-generated field
        if (GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__1 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, string>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (GraphViewModel)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, string> target1 = GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__1.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, string>> p1 = GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__1;
        // ISSUE: reference to a compiler-generated field
        if (GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__0 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, "FriendlyDisplayName", typeof (GraphViewModel), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj2 = GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__0.Target((CallSite) GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__0, obj1);
        this.DisplayName = target1((CallSite) p1, obj2);
        // ISSUE: reference to a compiler-generated field
        if (GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__3 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__3 = CallSite<Func<CallSite, object, string>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (GraphViewModel)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, string> target2 = GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__3.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, string>> p3 = GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__3;
        // ISSUE: reference to a compiler-generated field
        if (GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__2 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, nameof (DisplayName), typeof (GraphViewModel), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj3 = GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__2.Target((CallSite) GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__2, obj1);
        this.PrincipalName = target2((CallSite) p3, obj3);
        // ISSUE: reference to a compiler-generated field
        if (GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__6 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__6 = CallSite<Func<CallSite, object, string>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (GraphViewModel)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, string> target3 = GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__6.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, string>> p6 = GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__6;
        // ISSUE: reference to a compiler-generated field
        if (GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__5 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__5 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "ToString", (IEnumerable<System.Type>) null, typeof (GraphViewModel), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, object> target4 = GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__5.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, object>> p5 = GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__5;
        // ISSUE: reference to a compiler-generated field
        if (GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__4 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__4 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, "TeamFoundationId", typeof (GraphViewModel), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj4 = GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__4.Target((CallSite) GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__4, obj1);
        object obj5 = target4((CallSite) p5, obj4);
        this.OriginId = target3((CallSite) p6, obj5);
        // ISSUE: reference to a compiler-generated field
        if (GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__8 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__8 = CallSite<Func<CallSite, object, string>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (GraphViewModel)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, string> target5 = GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__8.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, string>> p8 = GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__8;
        // ISSUE: reference to a compiler-generated field
        if (GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__7 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__7 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, "IdentityType", typeof (GraphViewModel), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj6 = GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__7.Target((CallSite) GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__7, obj1);
        this.SubjectKind = target5((CallSite) p8, obj6);
        // ISSUE: reference to a compiler-generated field
        if (GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__10 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__10 = CallSite<Func<CallSite, object, string>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (GraphViewModel)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, string> target6 = GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__10.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, string>> p10 = GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__10;
        // ISSUE: reference to a compiler-generated field
        if (GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__9 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__9 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, nameof (Description), typeof (GraphViewModel), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj7 = GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__9.Target((CallSite) GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__9, obj1);
        this.Description = target6((CallSite) p10, obj7);
        this.Domain = collectionHostId.ToString();
        // ISSUE: reference to a compiler-generated field
        if (GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__12 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__12 = CallSite<Func<CallSite, object, string>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (GraphViewModel)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, string> target7 = GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__12.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, string>> p12 = GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__12;
        // ISSUE: reference to a compiler-generated field
        if (GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__11 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__11 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, nameof (Scope), typeof (GraphViewModel), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj8 = GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__11.Target((CallSite) GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__11, obj1);
        this.Scope = target7((CallSite) p12, obj8);
        // ISSUE: reference to a compiler-generated field
        if (GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__14 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__14 = CallSite<Func<CallSite, object, Guid>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, typeof (Guid), typeof (GraphViewModel)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, Guid> target8 = GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__14.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, Guid>> p14 = GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__14;
        // ISSUE: reference to a compiler-generated field
        if (GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__13 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__13 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, "TeamFoundationId", typeof (GraphViewModel), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj9 = GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__13.Target((CallSite) GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__13, obj1);
        this.IdentityId = target8((CallSite) p14, obj9);
        this.Descriptor = (string) identity.SubjectDescriptor;
        // ISSUE: reference to a compiler-generated field
        if (GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__16 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__16 = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, typeof (bool), typeof (GraphViewModel)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, bool> target9 = GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__16.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, bool>> p16 = GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__16;
        // ISSUE: reference to a compiler-generated field
        if (GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__15 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__15 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, nameof (IsAadGroup), typeof (GraphViewModel), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj10 = GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__15.Target((CallSite) GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__15, obj1);
        this.IsAadGroup = target9((CallSite) p16, obj10);
        // ISSUE: reference to a compiler-generated field
        if (GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__19 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__19 = CallSite<Func<CallSite, object, string>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (GraphViewModel)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, string> target10 = GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__19.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, string>> p19 = GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__19;
        // ISSUE: reference to a compiler-generated field
        if (GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__18 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__18 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, "Identifier", typeof (GraphViewModel), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, object> target11 = GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__18.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, object>> p18 = GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__18;
        // ISSUE: reference to a compiler-generated field
        if (GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__17 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__17 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, nameof (Descriptor), typeof (GraphViewModel), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj11 = GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__17.Target((CallSite) GraphViewModel.\u003C\u003Eo__72.\u003C\u003Ep__17, obj1);
        object obj12 = target11((CallSite) p18, obj11);
        this.Sid = target10((CallSite) p19, obj12);
      }
      else if (identity.Descriptor.IsAadServicePrincipalType())
      {
        ServicePrincipalIdentityViewModel identityViewModel = new ServicePrincipalIdentityViewModel(identity);
        this.DisplayName = identityViewModel.FriendlyDisplayName;
        this.ApplicationId = identityViewModel.ApplicationId;
        this.SubjectKind = identityViewModel.IdentityType;
        this.IdentityId = identityViewModel.TeamFoundationId;
        this.OriginId = identityViewModel.TeamFoundationId.ToString();
        this.Descriptor = (string) identity.SubjectDescriptor;
        this.MetaType = identityViewModel.FriendlyMetaType;
      }
      else
      {
        UserIdentityViewModel identityViewModel = new UserIdentityViewModel(identity);
        this.DisplayName = identityViewModel.FriendlyDisplayName;
        this.MailAddress = identityViewModel.MailAddress;
        this.SubjectKind = identityViewModel.IdentityType;
        this.IdentityId = identityViewModel.TeamFoundationId;
        this.OriginId = identityViewModel.TeamFoundationId.ToString();
        this.Descriptor = (string) identity.SubjectDescriptor;
        this.MetaType = identityViewModel.FriendlyMetaType;
      }
    }

    public GraphViewModel(IDirectoryEntity identity, Guid collectionHostId)
    {
      string str1 = identity.GetType().GetProperty(nameof (Description)) != (PropertyInfo) null ? (identity[nameof (Description)] != null ? identity[nameof (Description)].ToString() : string.Empty) : string.Empty;
      string str2 = identity.GetType().GetProperty("Mail") != (PropertyInfo) null ? (identity["Mail"] != null ? identity["Mail"].ToString() : string.Empty) : string.Empty;
      this.DisplayName = identity.DisplayName;
      this.PrincipalName = identity.ScopeName + "\\" + identity.DisplayName;
      this.OriginId = identity.OriginId;
      this.SubjectKind = identity.EntityType;
      this.Domain = collectionHostId.ToString();
      this.IdentityId = !string.IsNullOrWhiteSpace(identity.LocalId) ? Guid.Parse(identity.LocalId) : Guid.Empty;
      SubjectDescriptor? subjectDescriptor = identity.SubjectDescriptor;
      string str3;
      if (!subjectDescriptor.HasValue)
      {
        str3 = "";
      }
      else
      {
        subjectDescriptor = identity.SubjectDescriptor;
        str3 = subjectDescriptor.ToString();
      }
      this.Descriptor = str3;
      this.Scope = identity.ScopeName;
      this.Description = str1;
      this.MailAddress = str2;
      this.EntityId = identity.EntityId;
      this.EntityType = identity.EntityType;
    }
  }
}
