// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.GroupSecurityService
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Client.Channels;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Xml;

namespace Microsoft.TeamFoundation.Server
{
  [Obsolete("GroupSecurityService is obsolete.  Please use the IdentityManagementService or SecurityService instead.", false)]
  internal class GroupSecurityService : TfsHttpClient
  {
    public GroupSecurityService(TfsTeamProjectCollection connection)
      : base((TfsConnection) connection)
    {
    }

    protected override Guid CollectionServiceIdentifier => new Guid("d9c3f8ff-8938-4193-919b-7588e81cb730");

    protected override string ServiceType => "CommonStructure";

    public void AddMemberToApplicationGroup(string groupSid, string identitySid) => this.Invoke((TfsClientOperation) new GroupSecurityService.AddMemberToApplicationGroupClientOperation(), new object[2]
    {
      (object) groupSid,
      (object) identitySid
    });

    public string CreateApplicationGroup(
      string projectUri,
      string groupName,
      string groupDescription)
    {
      return (string) this.Invoke((TfsClientOperation) new GroupSecurityService.CreateApplicationGroupClientOperation(), new object[3]
      {
        (object) projectUri,
        (object) groupName,
        (object) groupDescription
      });
    }

    public void DeleteApplicationGroup(string groupSid) => this.Invoke((TfsClientOperation) new GroupSecurityService.DeleteApplicationGroupClientOperation(), new object[1]
    {
      (object) groupSid
    });

    public string GetChangedIdentities(int sequence_id) => (string) this.Invoke((TfsClientOperation) new GroupSecurityService.GetChangedIdentitiesClientOperation(), new object[1]
    {
      (object) sequence_id
    });

    public bool IsIdentityCached(SearchFactor factor, string factorValue) => (bool) this.Invoke((TfsClientOperation) new GroupSecurityService.IsIdentityCachedClientOperation(), new object[2]
    {
      (object) factor,
      (object) factorValue
    });

    public bool IsMember(string groupSid, string identitySid) => (bool) this.Invoke((TfsClientOperation) new GroupSecurityService.IsMemberClientOperation(), new object[2]
    {
      (object) groupSid,
      (object) identitySid
    });

    public Identity[] ListApplicationGroups(string projectUri) => (Identity[]) this.Invoke((TfsClientOperation) new GroupSecurityService.ListApplicationGroupsClientOperation(), new object[1]
    {
      (object) projectUri
    });

    public Identity ReadIdentity(
      SearchFactor factor,
      string factorValue,
      QueryMembership queryMembership)
    {
      return (Identity) this.Invoke((TfsClientOperation) new GroupSecurityService.ReadIdentityClientOperation(), new object[3]
      {
        (object) factor,
        (object) factorValue,
        (object) queryMembership
      });
    }

    public Identity ReadIdentityFromSource(SearchFactor factor, string factorValue) => (Identity) this.Invoke((TfsClientOperation) new GroupSecurityService.ReadIdentityFromSourceClientOperation(), new object[2]
    {
      (object) factor,
      (object) factorValue
    });

    public void RemoveMemberFromApplicationGroup(string groupSid, string identitySid) => this.Invoke((TfsClientOperation) new GroupSecurityService.RemoveMemberFromApplicationGroupClientOperation(), new object[2]
    {
      (object) groupSid,
      (object) identitySid
    });

    public void UpdateApplicationGroup(
      string groupSid,
      ApplicationGroupProperty property,
      string newValue)
    {
      this.Invoke((TfsClientOperation) new GroupSecurityService.UpdateApplicationGroupClientOperation(), new object[3]
      {
        (object) groupSid,
        (object) property,
        (object) newValue
      });
    }

    internal GroupSecurityService(TfsTeamProjectCollection server, string url)
      : base((TfsConnection) server, new Uri(url))
    {
    }

    protected override string ComponentName => "GroupSecurity";

    internal sealed class AddMemberToApplicationGroupClientOperation : TfsClientOperation
    {
      public override string BodyName => "AddMemberToApplicationGroup";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/GroupSecurity/03/AddMemberToApplicationGroup";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/GroupSecurity/03";

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter1 = (string) parameters[0];
        if (parameter1 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "groupSid", parameter1);
        string parameter2 = (string) parameters[1];
        if (parameter2 == null)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "identitySid", parameter2);
      }
    }

    internal sealed class CreateApplicationGroupClientOperation : TfsClientOperation
    {
      public override string BodyName => "CreateApplicationGroup";

      public override bool HasOutputs => true;

      public override string ResultName => "CreateApplicationGroupResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/GroupSecurity/03/CreateApplicationGroup";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/GroupSecurity/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) null;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) XmlUtility.StringFromXmlElement(reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter1 = (string) parameters[0];
        if (parameter1 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "projectUri", parameter1);
        string parameter2 = (string) parameters[1];
        if (parameter2 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "groupName", parameter2);
        string parameter3 = (string) parameters[2];
        if (parameter3 == null)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "groupDescription", parameter3);
      }
    }

    internal sealed class DeleteApplicationGroupClientOperation : TfsClientOperation
    {
      public override string BodyName => "DeleteApplicationGroup";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/GroupSecurity/03/DeleteApplicationGroup";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/GroupSecurity/03";

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter = (string) parameters[0];
        if (parameter == null)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "groupSid", parameter);
      }
    }

    internal sealed class GetChangedIdentitiesClientOperation : TfsClientOperation
    {
      public override string BodyName => "GetChangedIdentities";

      public override bool HasOutputs => true;

      public override string ResultName => "GetChangedIdentitiesResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/GroupSecurity/03/GetChangedIdentities";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/GroupSecurity/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) null;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) XmlUtility.StringFromXmlElement(reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        int parameter = (int) parameters[0];
        if (parameter == 0)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "sequence_id", parameter);
      }
    }

    internal sealed class IsIdentityCachedClientOperation : TfsClientOperation
    {
      public override string BodyName => "IsIdentityCached";

      public override bool HasOutputs => true;

      public override string ResultName => "IsIdentityCachedResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/GroupSecurity/03/IsIdentityCached";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/GroupSecurity/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) false;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) XmlUtility.BooleanFromXmlElement(reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        SearchFactor parameter1 = (SearchFactor) parameters[0];
        if (parameter1 != SearchFactor.None)
          XmlUtility.EnumToXmlElement<SearchFactor>((XmlWriter) writer, "factor", parameter1);
        string parameter2 = (string) parameters[1];
        if (parameter2 == null)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "factorValue", parameter2);
      }
    }

    internal sealed class IsMemberClientOperation : TfsClientOperation
    {
      public override string BodyName => "IsMember";

      public override bool HasOutputs => true;

      public override string ResultName => "IsMemberResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/GroupSecurity/03/IsMember";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/GroupSecurity/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) false;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) XmlUtility.BooleanFromXmlElement(reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter1 = (string) parameters[0];
        if (parameter1 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "groupSid", parameter1);
        string parameter2 = (string) parameters[1];
        if (parameter2 == null)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "identitySid", parameter2);
      }
    }

    internal sealed class ListApplicationGroupsClientOperation : TfsClientOperation
    {
      public override string BodyName => "ListApplicationGroups";

      public override bool HasOutputs => true;

      public override string ResultName => "ListApplicationGroupsResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/GroupSecurity/03/ListApplicationGroups";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/GroupSecurity/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) Helper.ZeroLengthArrayOfIdentity;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) Helper.ArrayOfIdentityFromXml(serviceProvider, reader, false);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter = (string) parameters[0];
        if (parameter == null)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "projectUri", parameter);
      }
    }

    internal sealed class ReadIdentityClientOperation : TfsClientOperation
    {
      public override string BodyName => "ReadIdentity";

      public override bool HasOutputs => true;

      public override string ResultName => "ReadIdentityResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/GroupSecurity/03/ReadIdentity";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/GroupSecurity/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) null;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) Identity.FromXml(serviceProvider, reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        SearchFactor parameter1 = (SearchFactor) parameters[0];
        if (parameter1 != SearchFactor.None)
          XmlUtility.EnumToXmlElement<SearchFactor>((XmlWriter) writer, "factor", parameter1);
        string parameter2 = (string) parameters[1];
        if (parameter2 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "factorValue", parameter2);
        QueryMembership parameter3 = (QueryMembership) parameters[2];
        if (parameter3 == QueryMembership.None)
          return;
        XmlUtility.EnumToXmlElement<QueryMembership>((XmlWriter) writer, "queryMembership", parameter3);
      }
    }

    internal sealed class ReadIdentityFromSourceClientOperation : TfsClientOperation
    {
      public override string BodyName => "ReadIdentityFromSource";

      public override bool HasOutputs => true;

      public override string ResultName => "ReadIdentityFromSourceResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/GroupSecurity/03/ReadIdentityFromSource";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/GroupSecurity/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) null;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) Identity.FromXml(serviceProvider, reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        SearchFactor parameter1 = (SearchFactor) parameters[0];
        if (parameter1 != SearchFactor.None)
          XmlUtility.EnumToXmlElement<SearchFactor>((XmlWriter) writer, "factor", parameter1);
        string parameter2 = (string) parameters[1];
        if (parameter2 == null)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "factorValue", parameter2);
      }
    }

    internal sealed class RemoveMemberFromApplicationGroupClientOperation : TfsClientOperation
    {
      public override string BodyName => "RemoveMemberFromApplicationGroup";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/GroupSecurity/03/RemoveMemberFromApplicationGroup";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/GroupSecurity/03";

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter1 = (string) parameters[0];
        if (parameter1 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "groupSid", parameter1);
        string parameter2 = (string) parameters[1];
        if (parameter2 == null)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "identitySid", parameter2);
      }
    }

    internal sealed class UpdateApplicationGroupClientOperation : TfsClientOperation
    {
      public override string BodyName => "UpdateApplicationGroup";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/GroupSecurity/03/UpdateApplicationGroup";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/GroupSecurity/03";

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter1 = (string) parameters[0];
        if (parameter1 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "groupSid", parameter1);
        ApplicationGroupProperty parameter2 = (ApplicationGroupProperty) parameters[1];
        if (parameter2 != ApplicationGroupProperty.None)
          XmlUtility.EnumToXmlElement<ApplicationGroupProperty>((XmlWriter) writer, "property", parameter2);
        string parameter3 = (string) parameters[2];
        if (parameter3 == null)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "newValue", parameter3);
      }
    }
  }
}
