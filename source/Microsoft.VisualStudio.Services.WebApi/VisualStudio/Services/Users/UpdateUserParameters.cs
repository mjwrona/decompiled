// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Users.UpdateUserParameters
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Users
{
  [DataContract]
  public class UpdateUserParameters
  {
    public UpdateUserParameters() => this.Properties = new PropertiesCollection();

    public UpdateUserParameters(UpdateUserParameters copy)
    {
      this.Descriptor = copy.Descriptor;
      this.Properties = new PropertiesCollection((IDictionary<string, object>) copy.Properties);
      this.LastModified = copy.LastModified;
      this.Revision = copy.Revision;
    }

    [IgnoreDataMember]
    public SubjectDescriptor Descriptor { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public PropertiesCollection Properties { get; private set; }

    [IgnoreDataMember]
    public string DisplayName
    {
      set => this.Properties[nameof (DisplayName)] = (object) value;
      get => this.Properties.GetValue<string>(nameof (DisplayName), (string) null);
    }

    [IgnoreDataMember]
    public string Mail
    {
      set => this.Properties[nameof (Mail)] = (object) value;
      get => this.Properties.GetValue<string>(nameof (Mail), (string) null);
    }

    [IgnoreDataMember]
    public string UnconfirmedMail
    {
      set => this.Properties[nameof (UnconfirmedMail)] = (object) value;
      get => this.Properties.GetValue<string>(nameof (UnconfirmedMail), (string) null);
    }

    [IgnoreDataMember]
    public string Country
    {
      set => this.Properties[nameof (Country)] = (object) value;
      get => this.Properties.GetValue<string>(nameof (Country), (string) null);
    }

    [IgnoreDataMember]
    public string Region
    {
      set => this.Properties[nameof (Region)] = (object) value;
      get => this.Properties.GetValue<string>(nameof (Region), (string) null);
    }

    [IgnoreDataMember]
    public string Bio
    {
      set => this.Properties[nameof (Bio)] = (object) value;
      get => this.Properties.GetValue<string>(nameof (Bio), (string) null);
    }

    [IgnoreDataMember]
    public string Blog
    {
      set => this.Properties[nameof (Blog)] = (object) value;
      get => this.Properties.GetValue<string>(nameof (Blog), (string) null);
    }

    [IgnoreDataMember]
    public string Company
    {
      set => this.Properties[nameof (Company)] = (object) value;
      get => this.Properties.GetValue<string>(nameof (Company), (string) null);
    }

    [IgnoreDataMember]
    internal DateTimeOffset LastModified { get; set; }

    [IgnoreDataMember]
    internal int Revision { get; set; }

    internal UpdateUserParameters Clone() => new UpdateUserParameters()
    {
      Descriptor = this.Descriptor,
      Properties = new PropertiesCollection((IDictionary<string, object>) this.Properties),
      Revision = this.Revision
    };

    internal virtual User ToUser()
    {
      User user = new User();
      user.Descriptor = this.Descriptor;
      user.LastModified = this.LastModified;
      user.Revision = this.Revision;
      user.UpdateWith(this);
      return user;
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "UpdateUserParameters\r\n[\r\nDescriptor:     {0}\r\nRevision:       {1}\r\nLastModified:   {2}\r\n{3}\r\n]", (object) this.Descriptor, (object) this.Revision, (object) this.LastModified, (object) string.Join("\r\n", this.Properties.Select<KeyValuePair<string, object>, string>((Func<KeyValuePair<string, object>, string>) (kvp => string.Format("{0}:{1}", (object) kvp.Key, kvp.Value)))));
  }
}
