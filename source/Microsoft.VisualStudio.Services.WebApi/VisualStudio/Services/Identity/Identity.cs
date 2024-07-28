// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Identity
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;

namespace Microsoft.VisualStudio.Services.Identity
{
  [DataContract]
  public sealed class Identity : IdentityBase, ISecuredObject
  {
    public Identity()
      : this((PropertiesCollection) null)
    {
    }

    private Identity(PropertiesCollection properties)
      : base(properties)
    {
    }

    public Microsoft.VisualStudio.Services.Identity.Identity Clone(bool includeMemberships)
    {
      Microsoft.VisualStudio.Services.Identity.Identity identity1 = new Microsoft.VisualStudio.Services.Identity.Identity(new PropertiesCollection((IDictionary<string, object>) this.Properties, false));
      identity1.Id = this.Id;
      identity1.Descriptor = new IdentityDescriptor(this.Descriptor);
      identity1.SubjectDescriptor = this.SubjectDescriptor;
      identity1.SocialDescriptor = this.SocialDescriptor;
      identity1.ProviderDisplayName = this.ProviderDisplayName;
      identity1.CustomDisplayName = this.CustomDisplayName;
      identity1.IsActive = this.IsActive;
      identity1.UniqueUserId = this.UniqueUserId;
      identity1.IsContainer = this.IsContainer;
      identity1.ResourceVersion = this.ResourceVersion;
      identity1.MetaTypeId = this.MetaTypeId;
      Microsoft.VisualStudio.Services.Identity.Identity identity2 = identity1;
      if (includeMemberships)
      {
        identity2.Members = Microsoft.VisualStudio.Services.Identity.Identity.CloneDescriptors((IEnumerable<IdentityDescriptor>) this.Members);
        identity2.MemberOf = Microsoft.VisualStudio.Services.Identity.Identity.CloneDescriptors((IEnumerable<IdentityDescriptor>) this.MemberOf);
        Microsoft.VisualStudio.Services.Identity.Identity identity3 = identity2;
        ICollection<Guid> memberIds = this.MemberIds;
        List<Guid> list1 = memberIds != null ? memberIds.ToList<Guid>() : (List<Guid>) null;
        identity3.MemberIds = (ICollection<Guid>) list1;
        Microsoft.VisualStudio.Services.Identity.Identity identity4 = identity2;
        ICollection<Guid> memberOfIds = this.MemberOfIds;
        List<Guid> list2 = memberOfIds != null ? memberOfIds.ToList<Guid>() : (List<Guid>) null;
        identity4.MemberOfIds = (ICollection<Guid>) list2;
      }
      identity2.MasterId = this.MasterId;
      return identity2;
    }

    public Microsoft.VisualStudio.Services.Identity.Identity Clone() => this.Clone(true);

    internal static Microsoft.VisualStudio.Services.Identity.Identity FromXml(
      IServiceProvider serviceProvider,
      XmlReader reader)
    {
      Microsoft.VisualStudio.Services.Identity.Identity identity = new Microsoft.VisualStudio.Services.Identity.Identity();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          switch (reader.Name)
          {
            case "DisplayName":
              identity.ProviderDisplayName = reader.Value;
              continue;
            case "IsActive":
              identity.IsActive = XmlConvert.ToBoolean(reader.Value);
              continue;
            case "IsContainer":
              identity.IsContainer = XmlConvert.ToBoolean(reader.Value);
              continue;
            case "TeamFoundationId":
              identity.Id = XmlConvert.ToGuid(reader.Value);
              continue;
            case "UniqueUserId":
              identity.UniqueUserId = XmlConvert.ToInt32(reader.Value);
              continue;
            default:
              continue;
          }
        }
      }
      reader.Read();
      if (!isEmptyElement)
      {
        while (reader.NodeType == XmlNodeType.Element)
        {
          switch (reader.Name)
          {
            case "Attributes":
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              KeyValueOfStringString[] valueOfStringStringArray = XmlUtility.ArrayOfObjectFromXml<KeyValueOfStringString>(serviceProvider, reader, "KeyValueOfStringString", false, Microsoft.VisualStudio.Services.Identity.Identity.\u003C\u003EO.\u003C0\u003E__FromXml ?? (Microsoft.VisualStudio.Services.Identity.Identity.\u003C\u003EO.\u003C0\u003E__FromXml = new Func<IServiceProvider, XmlReader, KeyValueOfStringString>(KeyValueOfStringString.FromXml)));
              if (valueOfStringStringArray != null && identity.Properties != null)
              {
                foreach (KeyValueOfStringString valueOfStringString in valueOfStringStringArray)
                  identity.Properties[valueOfStringString.Key] = (object) valueOfStringString.Value;
                continue;
              }
              continue;
            case "Descriptor":
              identity.Descriptor = IdentityDescriptor.FromXml(serviceProvider, reader);
              continue;
            case "LocalProperties":
              reader.ReadOuterXml();
              continue;
            case "MemberOf":
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              identity.MemberOf = (ICollection<IdentityDescriptor>) XmlUtility.ArrayOfObjectFromXml<IdentityDescriptor>(serviceProvider, reader, "IdentityDescriptor", false, Microsoft.VisualStudio.Services.Identity.Identity.\u003C\u003EO.\u003C1\u003E__FromXml ?? (Microsoft.VisualStudio.Services.Identity.Identity.\u003C\u003EO.\u003C1\u003E__FromXml = new Func<IServiceProvider, XmlReader, IdentityDescriptor>(IdentityDescriptor.FromXml)));
              continue;
            case "Members":
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              identity.Members = (ICollection<IdentityDescriptor>) XmlUtility.ArrayOfObjectFromXml<IdentityDescriptor>(serviceProvider, reader, "IdentityDescriptor", false, Microsoft.VisualStudio.Services.Identity.Identity.\u003C\u003EO.\u003C1\u003E__FromXml ?? (Microsoft.VisualStudio.Services.Identity.Identity.\u003C\u003EO.\u003C1\u003E__FromXml = new Func<IServiceProvider, XmlReader, IdentityDescriptor>(IdentityDescriptor.FromXml)));
              continue;
            case "Properties":
              reader.ReadOuterXml();
              continue;
            default:
              reader.ReadOuterXml();
              continue;
          }
        }
        reader.ReadEndElement();
      }
      return identity;
    }

    public Guid NamespaceId => GraphSecurityConstants.NamespaceId;

    public int RequiredPermissions => 1;

    public string GetToken() => GraphSecurityConstants.SubjectsToken;

    private static ICollection<IdentityDescriptor> CloneDescriptors(
      IEnumerable<IdentityDescriptor> descriptors)
    {
      return descriptors == null ? (ICollection<IdentityDescriptor>) null : (ICollection<IdentityDescriptor>) descriptors.Select<IdentityDescriptor, IdentityDescriptor>((Func<IdentityDescriptor, IdentityDescriptor>) (item => new IdentityDescriptor(item))).ToList<IdentityDescriptor>();
    }
  }
}
