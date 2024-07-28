// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Users.Server.InternalUpdateUserParameters
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Users.Server
{
  [DataContract]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class InternalUpdateUserParameters : UpdateUserParameters
  {
    private IdentityDescriptor m_identityDescriptor;
    private SocialDescriptor m_socialDescriptor;

    public InternalUpdateUserParameters()
    {
    }

    public InternalUpdateUserParameters(UpdateUserParameters copy)
      : base(copy)
    {
      if (!(copy is InternalUpdateUserParameters updateUserParameters))
        return;
      this.IdentityDescriptor = updateUserParameters.IdentityDescriptor;
      this.SocialDescriptor = updateUserParameters.SocialDescriptor;
      this.ProfileLastSynced = updateUserParameters.ProfileLastSynced;
    }

    [IgnoreDataMember]
    public IdentityDescriptor IdentityDescriptor
    {
      set
      {
        this.m_identityDescriptor = value;
        this.Properties[nameof (IdentityDescriptor)] = (object) value.ToString();
      }
      get
      {
        if (this.m_identityDescriptor == (IdentityDescriptor) null)
        {
          string identityDescriptorString = this.Properties.GetValue<string>(nameof (IdentityDescriptor), (string) null);
          if (identityDescriptorString != null)
            this.m_identityDescriptor = IdentityDescriptor.FromString(identityDescriptorString);
        }
        return this.m_identityDescriptor;
      }
    }

    [IgnoreDataMember]
    public SocialDescriptor SocialDescriptor
    {
      set
      {
        this.m_socialDescriptor = value;
        this.Properties[nameof (SocialDescriptor)] = (object) value.ToString();
      }
      get
      {
        if (this.m_socialDescriptor == new SocialDescriptor())
        {
          string socialDescriptorString = this.Properties.GetValue<string>(nameof (SocialDescriptor), (string) null);
          if (socialDescriptorString != null)
            this.m_socialDescriptor = SocialDescriptor.FromString(socialDescriptorString);
        }
        return this.m_socialDescriptor;
      }
    }

    [IgnoreDataMember]
    public DateTimeOffset? ProfileLastSynced { get; set; }

    public bool UpdateSocialDescriptor => this.Properties.TryGetValue("SocialDescriptor", out object _);
  }
}
