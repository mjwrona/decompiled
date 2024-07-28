// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.IdentityManagementWebService
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Client.Channels;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Services.Protocols;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class IdentityManagementWebService : TfsHttpClient
  {
    public IdentityManagementWebService(TfsConnection connection)
      : base(connection)
    {
    }

    protected override Guid CollectionServiceIdentifier => new Guid("1e29861e-76b6-4b1e-bf41-5f868aea63fe");

    protected override string ComponentName => "Framework";

    protected override Guid ConfigurationServiceIdentifier => new Guid("3de26348-00be-4b82-8e4a-e5ad004cfecd");

    protected override string ServiceType => "IdentityManagementService";

    public void AddMemberToApplicationGroup(
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor descriptor)
    {
      this.Invoke((TfsClientOperation) new IdentityManagementWebService.AddMemberToApplicationGroupClientOperation(), new object[2]
      {
        (object) groupDescriptor,
        (object) descriptor
      });
    }

    protected override Exception ConvertException(SoapException exception) => TeamFoundationServiceException.ConvertException(exception);

    public IdentityDescriptor CreateApplicationGroup(
      string projectUri,
      string groupName,
      string groupDescription)
    {
      return (IdentityDescriptor) this.Invoke((TfsClientOperation) new IdentityManagementWebService.CreateApplicationGroupClientOperation(), new object[3]
      {
        (object) projectUri,
        (object) groupName,
        (object) groupDescription
      });
    }

    public void DeleteApplicationGroup(IdentityDescriptor groupDescriptor) => this.Invoke((TfsClientOperation) new IdentityManagementWebService.DeleteApplicationGroupClientOperation(), new object[1]
    {
      (object) groupDescriptor
    });

    public string GetScopeName(string scopeId) => (string) this.Invoke((TfsClientOperation) new IdentityManagementWebService.GetScopeNameClientOperation(), new object[1]
    {
      (object) scopeId
    });

    public bool IsMember(IdentityDescriptor groupDescriptor, IdentityDescriptor descriptor) => (bool) this.Invoke((TfsClientOperation) new IdentityManagementWebService.IsMemberClientOperation(), new object[2]
    {
      (object) groupDescriptor,
      (object) descriptor
    });

    public TeamFoundationIdentity[] ListApplicationGroups(
      string projectUri,
      int options,
      int features,
      IEnumerable<string> propertyNameFilters,
      int propertyScope)
    {
      return (TeamFoundationIdentity[]) this.Invoke((TfsClientOperation) new IdentityManagementWebService.ListApplicationGroupsClientOperation(), new object[5]
      {
        (object) projectUri,
        (object) options,
        (object) features,
        (object) propertyNameFilters,
        (object) propertyScope
      });
    }

    public TeamFoundationIdentity[][] ReadIdentities(
      int searchFactor,
      string[] factorValues,
      int queryMembership,
      int options,
      int features,
      IEnumerable<string> propertyNameFilters,
      int propertyScope)
    {
      return (TeamFoundationIdentity[][]) this.Invoke((TfsClientOperation) new IdentityManagementWebService.ReadIdentitiesClientOperation(), new object[7]
      {
        (object) searchFactor,
        (object) factorValues,
        (object) queryMembership,
        (object) options,
        (object) features,
        (object) propertyNameFilters,
        (object) propertyScope
      });
    }

    public TeamFoundationIdentity[] ReadIdentitiesByDescriptor(
      IdentityDescriptor[] descriptors,
      int queryMembership,
      int options,
      int features,
      IEnumerable<string> propertyNameFilters,
      int propertyScope)
    {
      return (TeamFoundationIdentity[]) this.Invoke((TfsClientOperation) new IdentityManagementWebService.ReadIdentitiesByDescriptorClientOperation(), new object[6]
      {
        (object) descriptors,
        (object) queryMembership,
        (object) options,
        (object) features,
        (object) propertyNameFilters,
        (object) propertyScope
      });
    }

    public TeamFoundationIdentity[] ReadIdentitiesById(
      Guid[] teamFoundationIds,
      int queryMembership,
      int features,
      int options,
      IEnumerable<string> propertyNameFilters,
      int propertyScope)
    {
      return (TeamFoundationIdentity[]) this.Invoke((TfsClientOperation) new IdentityManagementWebService.ReadIdentitiesByIdClientOperation(), new object[6]
      {
        (object) teamFoundationIds,
        (object) queryMembership,
        (object) features,
        (object) options,
        (object) propertyNameFilters,
        (object) propertyScope
      });
    }

    public bool RefreshIdentity(IdentityDescriptor descriptor) => (bool) this.Invoke((TfsClientOperation) new IdentityManagementWebService.RefreshIdentityClientOperation(), new object[1]
    {
      (object) descriptor
    });

    public void RemoveMemberFromApplicationGroup(
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor descriptor)
    {
      this.Invoke((TfsClientOperation) new IdentityManagementWebService.RemoveMemberFromApplicationGroupClientOperation(), new object[2]
      {
        (object) groupDescriptor,
        (object) descriptor
      });
    }

    public void UpdateApplicationGroup(
      IdentityDescriptor groupDescriptor,
      int groupProperty,
      string newValue)
    {
      this.Invoke((TfsClientOperation) new IdentityManagementWebService.UpdateApplicationGroupClientOperation(), new object[3]
      {
        (object) groupDescriptor,
        (object) groupProperty,
        (object) newValue
      });
    }

    internal sealed class AddMemberToApplicationGroupClientOperation : TfsClientOperation
    {
      public override string BodyName => "AddMemberToApplicationGroup";

      public override string SoapAction => "http://microsoft.com/webservices/AddMemberToApplicationGroup";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        IdentityDescriptor parameter1 = (IdentityDescriptor) parameters[0];
        if (parameter1 != null)
          IdentityDescriptor.ToXml((XmlWriter) writer, "groupDescriptor", parameter1);
        IdentityDescriptor parameter2 = (IdentityDescriptor) parameters[1];
        if (parameter2 == null)
          return;
        IdentityDescriptor.ToXml((XmlWriter) writer, "descriptor", parameter2);
      }
    }

    internal sealed class CreateApplicationGroupClientOperation : TfsClientOperation
    {
      public override string BodyName => "CreateApplicationGroup";

      public override bool HasOutputs => true;

      public override string ResultName => "CreateApplicationGroupResult";

      public override string SoapAction => "http://microsoft.com/webservices/CreateApplicationGroup";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) null;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) IdentityDescriptor.FromXml(serviceProvider, reader);

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

      public override string SoapAction => "http://microsoft.com/webservices/DeleteApplicationGroup";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        IdentityDescriptor parameter = (IdentityDescriptor) parameters[0];
        if (parameter == null)
          return;
        IdentityDescriptor.ToXml((XmlWriter) writer, "groupDescriptor", parameter);
      }
    }

    internal sealed class GetScopeNameClientOperation : TfsClientOperation
    {
      public override string BodyName => "GetScopeName";

      public override bool HasOutputs => true;

      public override string ResultName => "GetScopeNameResult";

      public override string SoapAction => "http://microsoft.com/webservices/GetScopeName";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) null;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) XmlUtility.StringFromXmlElement(reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter = (string) parameters[0];
        if (parameter == null)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "scopeId", parameter);
      }
    }

    internal sealed class IsMemberClientOperation : TfsClientOperation
    {
      public override string BodyName => "IsMember";

      public override bool HasOutputs => true;

      public override string ResultName => "IsMemberResult";

      public override string SoapAction => "http://microsoft.com/webservices/IsMember";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) false;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) XmlUtility.BooleanFromXmlElement(reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        IdentityDescriptor parameter1 = (IdentityDescriptor) parameters[0];
        if (parameter1 != null)
          IdentityDescriptor.ToXml((XmlWriter) writer, "groupDescriptor", parameter1);
        IdentityDescriptor parameter2 = (IdentityDescriptor) parameters[1];
        if (parameter2 == null)
          return;
        IdentityDescriptor.ToXml((XmlWriter) writer, "descriptor", parameter2);
      }
    }

    internal sealed class ListApplicationGroupsClientOperation : TfsClientOperation
    {
      public override string BodyName => "ListApplicationGroups";

      public override bool HasOutputs => true;

      public override string ResultName => "ListApplicationGroupsResult";

      public override string SoapAction => "http://microsoft.com/webservices/ListApplicationGroups";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) Helper.ZeroLengthArrayOfTeamFoundationIdentity;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) Helper.ArrayOfTeamFoundationIdentityFromXml(serviceProvider, reader, false);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter1 = (string) parameters[0];
        if (parameter1 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "projectUri", parameter1);
        int parameter2 = (int) parameters[1];
        if (parameter2 != 0)
          XmlUtility.ToXmlElement((XmlWriter) writer, "options", parameter2);
        int parameter3 = (int) parameters[2];
        if (parameter3 != 0)
          XmlUtility.ToXmlElement((XmlWriter) writer, "features", parameter3);
        IEnumerable<string> parameter4 = (IEnumerable<string>) parameters[3];
        Helper.ToXml((XmlWriter) writer, "propertyNameFilters", parameter4, false, false);
        int parameter5 = (int) parameters[4];
        if (parameter5 == 0)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "propertyScope", parameter5);
      }
    }

    internal sealed class ReadIdentitiesByDescriptorClientOperation : TfsClientOperation
    {
      public override string BodyName => "ReadIdentitiesByDescriptor";

      public override bool HasOutputs => true;

      public override string ResultName => "ReadIdentitiesByDescriptorResult";

      public override string SoapAction => "http://microsoft.com/webservices/ReadIdentitiesByDescriptor";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) Helper.ZeroLengthArrayOfTeamFoundationIdentity;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) Helper.ArrayOfTeamFoundationIdentityFromXml(serviceProvider, reader, false);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        IdentityDescriptor[] parameter1 = (IdentityDescriptor[]) parameters[0];
        Helper.ToXml((XmlWriter) writer, "descriptors", parameter1, false, false);
        int parameter2 = (int) parameters[1];
        if (parameter2 != 0)
          XmlUtility.ToXmlElement((XmlWriter) writer, "queryMembership", parameter2);
        int parameter3 = (int) parameters[2];
        if (parameter3 != 0)
          XmlUtility.ToXmlElement((XmlWriter) writer, "options", parameter3);
        int parameter4 = (int) parameters[3];
        if (parameter4 != 0)
          XmlUtility.ToXmlElement((XmlWriter) writer, "features", parameter4);
        IEnumerable<string> parameter5 = (IEnumerable<string>) parameters[4];
        Helper.ToXml((XmlWriter) writer, "propertyNameFilters", parameter5, false, false);
        int parameter6 = (int) parameters[5];
        if (parameter6 == 0)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "propertyScope", parameter6);
      }
    }

    internal sealed class ReadIdentitiesByIdClientOperation : TfsClientOperation
    {
      public override string BodyName => "ReadIdentitiesById";

      public override bool HasOutputs => true;

      public override string ResultName => "ReadIdentitiesByIdResult";

      public override string SoapAction => "http://microsoft.com/webservices/ReadIdentitiesById";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) Helper.ZeroLengthArrayOfTeamFoundationIdentity;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) Helper.ArrayOfTeamFoundationIdentityFromXml(serviceProvider, reader, false);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        Guid[] parameter1 = (Guid[]) parameters[0];
        Helper.ToXml((XmlWriter) writer, "teamFoundationIds", parameter1, false, false);
        int parameter2 = (int) parameters[1];
        if (parameter2 != 0)
          XmlUtility.ToXmlElement((XmlWriter) writer, "queryMembership", parameter2);
        int parameter3 = (int) parameters[2];
        if (parameter3 != 0)
          XmlUtility.ToXmlElement((XmlWriter) writer, "features", parameter3);
        int parameter4 = (int) parameters[3];
        if (parameter4 != 0)
          XmlUtility.ToXmlElement((XmlWriter) writer, "options", parameter4);
        IEnumerable<string> parameter5 = (IEnumerable<string>) parameters[4];
        Helper.ToXml((XmlWriter) writer, "propertyNameFilters", parameter5, false, false);
        int parameter6 = (int) parameters[5];
        if (parameter6 == 0)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "propertyScope", parameter6);
      }
    }

    internal sealed class ReadIdentitiesClientOperation : TfsClientOperation
    {
      public override string BodyName => "ReadIdentities";

      public override bool HasOutputs => true;

      public override string ResultName => "ReadIdentitiesResult";

      public override string SoapAction => "http://microsoft.com/webservices/ReadIdentities";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) Helper.ZeroLengthArrayOfArrayOfTeamFoundationIdentity;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) Helper.ArrayOfArrayOfTeamFoundationIdentityFromXml(serviceProvider, reader, false);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        int parameter1 = (int) parameters[0];
        if (parameter1 != 0)
          XmlUtility.ToXmlElement((XmlWriter) writer, "searchFactor", parameter1);
        string[] parameter2 = (string[]) parameters[1];
        Helper.ToXml((XmlWriter) writer, "factorValues", parameter2, false, false);
        int parameter3 = (int) parameters[2];
        if (parameter3 != 0)
          XmlUtility.ToXmlElement((XmlWriter) writer, "queryMembership", parameter3);
        int parameter4 = (int) parameters[3];
        if (parameter4 != 0)
          XmlUtility.ToXmlElement((XmlWriter) writer, "options", parameter4);
        int parameter5 = (int) parameters[4];
        if (parameter5 != 0)
          XmlUtility.ToXmlElement((XmlWriter) writer, "features", parameter5);
        IEnumerable<string> parameter6 = (IEnumerable<string>) parameters[5];
        Helper.ToXml((XmlWriter) writer, "propertyNameFilters", parameter6, false, false);
        int parameter7 = (int) parameters[6];
        if (parameter7 == 0)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "propertyScope", parameter7);
      }
    }

    internal sealed class RefreshIdentityClientOperation : TfsClientOperation
    {
      public override string BodyName => "RefreshIdentity";

      public override bool HasOutputs => true;

      public override string ResultName => "RefreshIdentityResult";

      public override string SoapAction => "http://microsoft.com/webservices/RefreshIdentity";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) false;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) XmlUtility.BooleanFromXmlElement(reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        IdentityDescriptor parameter = (IdentityDescriptor) parameters[0];
        if (parameter == null)
          return;
        IdentityDescriptor.ToXml((XmlWriter) writer, "descriptor", parameter);
      }
    }

    internal sealed class RemoveMemberFromApplicationGroupClientOperation : TfsClientOperation
    {
      public override string BodyName => "RemoveMemberFromApplicationGroup";

      public override string SoapAction => "http://microsoft.com/webservices/RemoveMemberFromApplicationGroup";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        IdentityDescriptor parameter1 = (IdentityDescriptor) parameters[0];
        if (parameter1 != null)
          IdentityDescriptor.ToXml((XmlWriter) writer, "groupDescriptor", parameter1);
        IdentityDescriptor parameter2 = (IdentityDescriptor) parameters[1];
        if (parameter2 == null)
          return;
        IdentityDescriptor.ToXml((XmlWriter) writer, "descriptor", parameter2);
      }
    }

    internal sealed class UpdateApplicationGroupClientOperation : TfsClientOperation
    {
      public override string BodyName => "UpdateApplicationGroup";

      public override string SoapAction => "http://microsoft.com/webservices/UpdateApplicationGroup";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        IdentityDescriptor parameter1 = (IdentityDescriptor) parameters[0];
        if (parameter1 != null)
          IdentityDescriptor.ToXml((XmlWriter) writer, "groupDescriptor", parameter1);
        int parameter2 = (int) parameters[1];
        if (parameter2 != 0)
          XmlUtility.ToXmlElement((XmlWriter) writer, "groupProperty", parameter2);
        string parameter3 = (string) parameters[2];
        if (parameter3 == null)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "newValue", parameter3);
      }
    }
  }
}
