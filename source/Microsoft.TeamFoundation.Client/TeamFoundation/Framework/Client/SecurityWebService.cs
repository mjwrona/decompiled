// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.SecurityWebService
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Client.Channels;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.Web.Services.Protocols;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  internal class SecurityWebService : TfsHttpClient
  {
    public SecurityWebService(TfsConnection connection)
      : base(connection)
    {
    }

    protected override Guid CollectionServiceIdentifier => new Guid("af3178da-1ec3-4bd0-b245-9f5decdc572e");

    protected override string ComponentName => "Framework";

    protected override Guid ConfigurationServiceIdentifier => new Guid("aff1a844-ba7d-4340-8a95-2952524ec778");

    protected override string ServiceType => "SecurityService";

    protected override Exception ConvertException(SoapException exception) => TeamFoundationServiceException.ConvertException(exception);

    public void CreateSecurityNamespace(SecurityNamespaceDescription description) => this.Invoke((TfsClientOperation) new SecurityWebService.CreateSecurityNamespaceClientOperation(), new object[1]
    {
      (object) description
    });

    public void DeleteSecurityNamespace(Guid namespaceId) => this.Invoke((TfsClientOperation) new SecurityWebService.DeleteSecurityNamespaceClientOperation(), new object[1]
    {
      (object) namespaceId
    });

    public bool[] HasPermissionByDescriptorList(
      Guid namespaceId,
      string token,
      IEnumerable<IdentityDescriptor> descriptors,
      int requestedPermissions,
      bool alwaysAllowAdministrators)
    {
      return (bool[]) this.Invoke((TfsClientOperation) new SecurityWebService.HasPermissionByDescriptorListClientOperation(), new object[5]
      {
        (object) namespaceId,
        (object) token,
        (object) descriptors,
        (object) requestedPermissions,
        (object) alwaysAllowAdministrators
      });
    }

    public bool[] HasPermissionByPermissionsList(
      Guid namespaceId,
      string token,
      IdentityDescriptor descriptor,
      IEnumerable<int> requestedPermissions,
      bool alwaysAllowAdministrators)
    {
      return (bool[]) this.Invoke((TfsClientOperation) new SecurityWebService.HasPermissionByPermissionsListClientOperation(), new object[5]
      {
        (object) namespaceId,
        (object) token,
        (object) descriptor,
        (object) requestedPermissions,
        (object) alwaysAllowAdministrators
      });
    }

    public bool[] HasPermissionByTokenList(
      Guid namespaceId,
      IEnumerable<string> tokens,
      IdentityDescriptor descriptor,
      int requestedPermissions,
      bool alwaysAllowAdministrators)
    {
      return (bool[]) this.Invoke((TfsClientOperation) new SecurityWebService.HasPermissionByTokenListClientOperation(), new object[5]
      {
        (object) namespaceId,
        (object) tokens,
        (object) descriptor,
        (object) requestedPermissions,
        (object) alwaysAllowAdministrators
      });
    }

    public bool[] HasWritePermission(
      Guid namespaceId,
      string token,
      IEnumerable<int> requestedPermissions)
    {
      return (bool[]) this.Invoke((TfsClientOperation) new SecurityWebService.HasWritePermissionClientOperation(), new object[3]
      {
        (object) namespaceId,
        (object) token,
        (object) requestedPermissions
      });
    }

    public AccessControlListDetails[] QueryPermissions(
      Guid namespaceId,
      string token,
      IEnumerable<IdentityDescriptor> identities,
      bool includeExtendedInfo,
      bool recurse)
    {
      return (AccessControlListDetails[]) this.Invoke((TfsClientOperation) new SecurityWebService.QueryPermissionsClientOperation(), new object[5]
      {
        (object) namespaceId,
        (object) token,
        (object) identities,
        (object) includeExtendedInfo,
        (object) recurse
      });
    }

    public SecurityNamespaceDescription[] QuerySecurityNamespaces(Guid namespaceId) => (SecurityNamespaceDescription[]) this.Invoke((TfsClientOperation) new SecurityWebService.QuerySecurityNamespacesClientOperation(), new object[1]
    {
      (object) namespaceId
    });

    public bool RemoveAccessControlEntries(
      Guid namespaceId,
      string token,
      IEnumerable<IdentityDescriptor> identities)
    {
      return (bool) this.Invoke((TfsClientOperation) new SecurityWebService.RemoveAccessControlEntriesClientOperation(), new object[3]
      {
        (object) namespaceId,
        (object) token,
        (object) identities
      });
    }

    public bool RemoveAccessControlList(Guid namespaceId, IEnumerable<string> tokens, bool recurse) => (bool) this.Invoke((TfsClientOperation) new SecurityWebService.RemoveAccessControlListClientOperation(), new object[3]
    {
      (object) namespaceId,
      (object) tokens,
      (object) recurse
    });

    public AccessControlEntryDetails RemovePermissions(
      Guid namespaceId,
      string token,
      IdentityDescriptor descriptor,
      int permissions)
    {
      return (AccessControlEntryDetails) this.Invoke((TfsClientOperation) new SecurityWebService.RemovePermissionsClientOperation(), new object[4]
      {
        (object) namespaceId,
        (object) token,
        (object) descriptor,
        (object) permissions
      });
    }

    public void SetAccessControlList(
      Guid namespaceId,
      IEnumerable<AccessControlListDetails> accessControlLists)
    {
      this.Invoke((TfsClientOperation) new SecurityWebService.SetAccessControlListClientOperation(), new object[2]
      {
        (object) namespaceId,
        (object) accessControlLists
      });
    }

    public void SetInheritFlag(Guid namespaceId, string token, bool inherits) => this.Invoke((TfsClientOperation) new SecurityWebService.SetInheritFlagClientOperation(), new object[3]
    {
      (object) namespaceId,
      (object) token,
      (object) inherits
    });

    public AccessControlEntryDetails[] SetPermissions(
      Guid namespaceId,
      string token,
      IEnumerable<AccessControlEntryDetails> accessControlEntries,
      bool merge)
    {
      return (AccessControlEntryDetails[]) this.Invoke((TfsClientOperation) new SecurityWebService.SetPermissionsClientOperation(), new object[4]
      {
        (object) namespaceId,
        (object) token,
        (object) accessControlEntries,
        (object) merge
      });
    }

    internal sealed class CreateSecurityNamespaceClientOperation : TfsClientOperation
    {
      public override string BodyName => "CreateSecurityNamespace";

      public override string SoapAction => "http://microsoft.com/webservices/CreateSecurityNamespace";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        SecurityNamespaceDescription parameter = (SecurityNamespaceDescription) parameters[0];
        if (parameter == null)
          return;
        SecurityNamespaceDescription.ToXml((XmlWriter) writer, "description", parameter);
      }
    }

    internal sealed class DeleteSecurityNamespaceClientOperation : TfsClientOperation
    {
      public override string BodyName => "DeleteSecurityNamespace";

      public override string SoapAction => "http://microsoft.com/webservices/DeleteSecurityNamespace";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        Guid parameter = (Guid) parameters[0];
        if (!(parameter != Guid.Empty))
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "namespaceId", parameter);
      }
    }

    internal sealed class HasPermissionByDescriptorListClientOperation : TfsClientOperation
    {
      public override string BodyName => "HasPermissionByDescriptorList";

      public override bool HasOutputs => true;

      public override string ResultName => "HasPermissionByDescriptorListResult";

      public override string SoapAction => "http://microsoft.com/webservices/HasPermissionByDescriptorList";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) Helper.ZeroLengthArrayOfBoolean;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) Helper.ArrayOfBooleanFromXml(reader, false);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        Guid parameter1 = (Guid) parameters[0];
        if (parameter1 != Guid.Empty)
          XmlUtility.ToXmlElement((XmlWriter) writer, "namespaceId", parameter1);
        string parameter2 = (string) parameters[1];
        if (parameter2 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "token", parameter2);
        IEnumerable<IdentityDescriptor> parameter3 = (IEnumerable<IdentityDescriptor>) parameters[2];
        Helper.ToXml((XmlWriter) writer, "descriptors", parameter3, false, false);
        int parameter4 = (int) parameters[3];
        if (parameter4 != 0)
          XmlUtility.ToXmlElement((XmlWriter) writer, "requestedPermissions", parameter4);
        bool parameter5 = (bool) parameters[4];
        if (!parameter5)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "alwaysAllowAdministrators", parameter5);
      }
    }

    internal sealed class HasPermissionByPermissionsListClientOperation : TfsClientOperation
    {
      public override string BodyName => "HasPermissionByPermissionsList";

      public override bool HasOutputs => true;

      public override string ResultName => "HasPermissionByPermissionsListResult";

      public override string SoapAction => "http://microsoft.com/webservices/HasPermissionByPermissionsList";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) Helper.ZeroLengthArrayOfBoolean;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) Helper.ArrayOfBooleanFromXml(reader, false);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        Guid parameter1 = (Guid) parameters[0];
        if (parameter1 != Guid.Empty)
          XmlUtility.ToXmlElement((XmlWriter) writer, "namespaceId", parameter1);
        string parameter2 = (string) parameters[1];
        if (parameter2 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "token", parameter2);
        IdentityDescriptor parameter3 = (IdentityDescriptor) parameters[2];
        if (parameter3 != null)
          IdentityDescriptor.ToXml((XmlWriter) writer, "descriptor", parameter3);
        IEnumerable<int> parameter4 = (IEnumerable<int>) parameters[3];
        Helper.ToXml((XmlWriter) writer, "requestedPermissions", parameter4, false, false);
        bool parameter5 = (bool) parameters[4];
        if (!parameter5)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "alwaysAllowAdministrators", parameter5);
      }
    }

    internal sealed class HasPermissionByTokenListClientOperation : TfsClientOperation
    {
      public override string BodyName => "HasPermissionByTokenList";

      public override bool HasOutputs => true;

      public override string ResultName => "HasPermissionByTokenListResult";

      public override string SoapAction => "http://microsoft.com/webservices/HasPermissionByTokenList";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) Helper.ZeroLengthArrayOfBoolean;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) Helper.ArrayOfBooleanFromXml(reader, false);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        Guid parameter1 = (Guid) parameters[0];
        if (parameter1 != Guid.Empty)
          XmlUtility.ToXmlElement((XmlWriter) writer, "namespaceId", parameter1);
        IEnumerable<string> parameter2 = (IEnumerable<string>) parameters[1];
        Helper.ToXml((XmlWriter) writer, "tokens", parameter2, false, false);
        IdentityDescriptor parameter3 = (IdentityDescriptor) parameters[2];
        if (parameter3 != null)
          IdentityDescriptor.ToXml((XmlWriter) writer, "descriptor", parameter3);
        int parameter4 = (int) parameters[3];
        if (parameter4 != 0)
          XmlUtility.ToXmlElement((XmlWriter) writer, "requestedPermissions", parameter4);
        bool parameter5 = (bool) parameters[4];
        if (!parameter5)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "alwaysAllowAdministrators", parameter5);
      }
    }

    internal sealed class HasWritePermissionClientOperation : TfsClientOperation
    {
      public override string BodyName => "HasWritePermission";

      public override bool HasOutputs => true;

      public override string ResultName => "HasWritePermissionResult";

      public override string SoapAction => "http://microsoft.com/webservices/HasWritePermission";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) Helper.ZeroLengthArrayOfBoolean;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) Helper.ArrayOfBooleanFromXml(reader, false);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        Guid parameter1 = (Guid) parameters[0];
        if (parameter1 != Guid.Empty)
          XmlUtility.ToXmlElement((XmlWriter) writer, "namespaceId", parameter1);
        string parameter2 = (string) parameters[1];
        if (parameter2 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "token", parameter2);
        IEnumerable<int> parameter3 = (IEnumerable<int>) parameters[2];
        Helper.ToXml((XmlWriter) writer, "requestedPermissions", parameter3, false, false);
      }
    }

    internal sealed class QueryPermissionsClientOperation : TfsClientOperation
    {
      public override string BodyName => "QueryPermissions";

      public override bool HasOutputs => true;

      public override string ResultName => "QueryPermissionsResult";

      public override string SoapAction => "http://microsoft.com/webservices/QueryPermissions";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) Helper.ZeroLengthArrayOfAccessControlListDetails;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) Helper.ArrayOfAccessControlListDetailsFromXml(serviceProvider, reader, false);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        Guid parameter1 = (Guid) parameters[0];
        if (parameter1 != Guid.Empty)
          XmlUtility.ToXmlElement((XmlWriter) writer, "namespaceId", parameter1);
        string parameter2 = (string) parameters[1];
        if (parameter2 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "token", parameter2);
        IEnumerable<IdentityDescriptor> parameter3 = (IEnumerable<IdentityDescriptor>) parameters[2];
        Helper.ToXml((XmlWriter) writer, "identities", parameter3, false, false);
        bool parameter4 = (bool) parameters[3];
        if (parameter4)
          XmlUtility.ToXmlElement((XmlWriter) writer, "includeExtendedInfo", parameter4);
        bool parameter5 = (bool) parameters[4];
        if (!parameter5)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "recurse", parameter5);
      }
    }

    internal sealed class QuerySecurityNamespacesClientOperation : TfsClientOperation
    {
      public override string BodyName => "QuerySecurityNamespaces";

      public override bool HasOutputs => true;

      public override string ResultName => "QuerySecurityNamespacesResult";

      public override string SoapAction => "http://microsoft.com/webservices/QuerySecurityNamespaces";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) Helper.ZeroLengthArrayOfSecurityNamespaceDescription;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) Helper.ArrayOfSecurityNamespaceDescriptionFromXml(serviceProvider, reader, false);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        Guid parameter = (Guid) parameters[0];
        if (!(parameter != Guid.Empty))
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "namespaceId", parameter);
      }
    }

    internal sealed class RemoveAccessControlEntriesClientOperation : TfsClientOperation
    {
      public override string BodyName => "RemoveAccessControlEntries";

      public override bool HasOutputs => true;

      public override string ResultName => "RemoveAccessControlEntriesResult";

      public override string SoapAction => "http://microsoft.com/webservices/RemoveAccessControlEntries";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) false;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) XmlUtility.BooleanFromXmlElement(reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        Guid parameter1 = (Guid) parameters[0];
        if (parameter1 != Guid.Empty)
          XmlUtility.ToXmlElement((XmlWriter) writer, "namespaceId", parameter1);
        string parameter2 = (string) parameters[1];
        if (parameter2 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "token", parameter2);
        IEnumerable<IdentityDescriptor> parameter3 = (IEnumerable<IdentityDescriptor>) parameters[2];
        Helper.ToXml((XmlWriter) writer, "identities", parameter3, false, false);
      }
    }

    internal sealed class RemoveAccessControlListClientOperation : TfsClientOperation
    {
      public override string BodyName => "RemoveAccessControlList";

      public override bool HasOutputs => true;

      public override string ResultName => "RemoveAccessControlListResult";

      public override string SoapAction => "http://microsoft.com/webservices/RemoveAccessControlList";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) false;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) XmlUtility.BooleanFromXmlElement(reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        Guid parameter1 = (Guid) parameters[0];
        if (parameter1 != Guid.Empty)
          XmlUtility.ToXmlElement((XmlWriter) writer, "namespaceId", parameter1);
        IEnumerable<string> parameter2 = (IEnumerable<string>) parameters[1];
        Helper.ToXml((XmlWriter) writer, "tokens", parameter2, false, false);
        bool parameter3 = (bool) parameters[2];
        if (!parameter3)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "recurse", parameter3);
      }
    }

    internal sealed class RemovePermissionsClientOperation : TfsClientOperation
    {
      public override string BodyName => "RemovePermissions";

      public override bool HasOutputs => true;

      public override string ResultName => "RemovePermissionsResult";

      public override string SoapAction => "http://microsoft.com/webservices/RemovePermissions";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) null;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) AccessControlEntryDetails.FromXml(serviceProvider, reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        Guid parameter1 = (Guid) parameters[0];
        if (parameter1 != Guid.Empty)
          XmlUtility.ToXmlElement((XmlWriter) writer, "namespaceId", parameter1);
        string parameter2 = (string) parameters[1];
        if (parameter2 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "token", parameter2);
        IdentityDescriptor parameter3 = (IdentityDescriptor) parameters[2];
        if (parameter3 != null)
          IdentityDescriptor.ToXml((XmlWriter) writer, "descriptor", parameter3);
        int parameter4 = (int) parameters[3];
        if (parameter4 == 0)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "permissions", parameter4);
      }
    }

    internal sealed class SetAccessControlListClientOperation : TfsClientOperation
    {
      public override string BodyName => "SetAccessControlList";

      public override string SoapAction => "http://microsoft.com/webservices/SetAccessControlList";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        Guid parameter1 = (Guid) parameters[0];
        if (parameter1 != Guid.Empty)
          XmlUtility.ToXmlElement((XmlWriter) writer, "namespaceId", parameter1);
        IEnumerable<AccessControlListDetails> parameter2 = (IEnumerable<AccessControlListDetails>) parameters[1];
        Helper.ToXml((XmlWriter) writer, "accessControlLists", parameter2, false, false);
      }
    }

    internal sealed class SetInheritFlagClientOperation : TfsClientOperation
    {
      public override string BodyName => "SetInheritFlag";

      public override string SoapAction => "http://microsoft.com/webservices/SetInheritFlag";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        Guid parameter1 = (Guid) parameters[0];
        if (parameter1 != Guid.Empty)
          XmlUtility.ToXmlElement((XmlWriter) writer, "namespaceId", parameter1);
        string parameter2 = (string) parameters[1];
        if (parameter2 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "token", parameter2);
        bool parameter3 = (bool) parameters[2];
        if (!parameter3)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "inherits", parameter3);
      }
    }

    internal sealed class SetPermissionsClientOperation : TfsClientOperation
    {
      public override string BodyName => "SetPermissions";

      public override bool HasOutputs => true;

      public override string ResultName => "SetPermissionsResult";

      public override string SoapAction => "http://microsoft.com/webservices/SetPermissions";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) Helper.ZeroLengthArrayOfAccessControlEntryDetails;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) Helper.ArrayOfAccessControlEntryDetailsFromXml(serviceProvider, reader, false);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        Guid parameter1 = (Guid) parameters[0];
        if (parameter1 != Guid.Empty)
          XmlUtility.ToXmlElement((XmlWriter) writer, "namespaceId", parameter1);
        string parameter2 = (string) parameters[1];
        if (parameter2 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "token", parameter2);
        IEnumerable<AccessControlEntryDetails> parameter3 = (IEnumerable<AccessControlEntryDetails>) parameters[2];
        Helper.ToXml((XmlWriter) writer, "accessControlEntries", parameter3, false, false);
        bool parameter4 = (bool) parameters[3];
        if (!parameter4)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "merge", parameter4);
      }
    }
  }
}
