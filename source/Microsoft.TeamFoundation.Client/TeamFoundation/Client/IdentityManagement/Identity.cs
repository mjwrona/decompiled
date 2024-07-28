// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.IdentityManagement.Identity
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Client.IdentityManagement
{
  [DataContract]
  internal sealed class Identity
  {
    [DataMember(Name = "entityId")]
    public string EntityId { get; set; }

    [DataMember(Name = "entityType")]
    public string EntityType { get; set; }

    [DataMember(Name = "originDirectory")]
    public string OriginDirectory { get; set; }

    [DataMember(Name = "originId")]
    public string OriginId { get; set; }

    [DataMember(Name = "localDirectory")]
    public string LocalDirectory { get; set; }

    [DataMember(Name = "localId")]
    public string LocalId { get; set; }

    [DataMember(Name = "displayName")]
    public string DisplayName { get; set; }

    [DataMember(Name = "scopeName")]
    public string ScopeName { get; set; }

    [DataMember(Name = "samAccountName")]
    public string SamAccountName { get; set; }

    [DataMember(Name = "active")]
    public bool? Active { get; set; }

    [DataMember(Name = "subjectDescriptor")]
    public Microsoft.VisualStudio.Services.Common.SubjectDescriptor? SubjectDescriptor { get; set; }

    [DataMember(Name = "signInAddress")]
    public string SignInAddress { get; set; }

    [DataMember(Name = "isMru")]
    public bool? IsMru { get; set; }
  }
}
