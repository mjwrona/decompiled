// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Users.User
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Users
{
  [DataContract]
  public class User
  {
    public User()
    {
    }

    public User(User copy)
    {
      this.Descriptor = copy.Descriptor;
      this.UserName = copy.UserName;
      this.DisplayName = copy.DisplayName;
      this.Mail = copy.Mail;
      this.UnconfirmedMail = copy.UnconfirmedMail;
      this.Bio = copy.Bio;
      this.Blog = copy.Blog;
      this.Company = copy.Company;
      this.Country = copy.Country;
      this.DateCreated = copy.DateCreated;
      this.Links = copy.Links;
      this.LastModified = copy.LastModified;
      this.ProfileLastSynced = copy.ProfileLastSynced;
      this.Revision = copy.Revision;
      this.State = copy.State;
    }

    [DataMember(IsRequired = true)]
    public SubjectDescriptor Descriptor { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string UserName { get; set; }

    [DataMember(IsRequired = false)]
    public string DisplayName { get; set; }

    [DataMember(IsRequired = false)]
    public string Mail { get; set; }

    [DataMember(IsRequired = false)]
    public string UnconfirmedMail { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Bio { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Blog { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Company { get; set; }

    [DataMember(IsRequired = false)]
    public string Country { get; set; }

    [DataMember(IsRequired = false)]
    public DateTimeOffset DateCreated { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ReferenceLinks Links { get; internal set; }

    [DataMember(IsRequired = false)]
    public DateTimeOffset LastModified { get; internal set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTimeOffset? ProfileLastSynced { get; internal set; }

    [DataMember(IsRequired = false)]
    public int Revision { get; internal set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public User.UserState State { get; internal set; }

    public static implicit operator UpdateUserParameters(User user) => new UpdateUserParameters()
    {
      Descriptor = user.Descriptor,
      DisplayName = user.DisplayName,
      Mail = user.Mail,
      UnconfirmedMail = user.UnconfirmedMail,
      Country = user.Country,
      Bio = user.Bio,
      Blog = user.Blog,
      Company = user.Company,
      LastModified = user.LastModified,
      Revision = user.Revision
    };

    internal virtual void UpdateWith(UpdateUserParameters userParameters)
    {
      ArgumentUtility.CheckForNull<UpdateUserParameters>(userParameters, nameof (userParameters));
      foreach (string key in userParameters.Properties.Keys)
      {
        string property = userParameters.Properties[key] as string;
        if (key != null)
        {
          switch (key.Length)
          {
            case 3:
              if (key == "Bio")
              {
                this.Bio = property;
                continue;
              }
              continue;
            case 4:
              switch (key[0])
              {
                case 'B':
                  if (key == "Blog")
                  {
                    this.Blog = property;
                    continue;
                  }
                  continue;
                case 'M':
                  if (key == "Mail")
                  {
                    this.Mail = property;
                    continue;
                  }
                  continue;
                default:
                  continue;
              }
            case 7:
              switch (key[2])
              {
                case 'm':
                  if (key == "Company")
                  {
                    this.Company = property;
                    continue;
                  }
                  continue;
                case 'u':
                  if (key == "Country")
                  {
                    this.Country = property;
                    continue;
                  }
                  continue;
                default:
                  continue;
              }
            case 11:
              if (key == "DisplayName")
              {
                this.DisplayName = property;
                continue;
              }
              continue;
            case 15:
              if (key == "UnconfirmedMail")
              {
                this.UnconfirmedMail = property;
                continue;
              }
              continue;
            case 17:
              if (key == "ProfileLastSynced")
              {
                DateTimeOffset result;
                this.ProfileLastSynced = DateTimeOffset.TryParse(property, out result) ? new DateTimeOffset?(result) : new DateTimeOffset?();
                continue;
              }
              continue;
            default:
              continue;
          }
        }
      }
    }

    [DataContract]
    public enum UserState
    {
      Wellformed,
      PendingProfileCreation,
      Deleted,
    }
  }
}
