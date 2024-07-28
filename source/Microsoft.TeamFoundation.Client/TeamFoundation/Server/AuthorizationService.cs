// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.AuthorizationService
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
  internal class AuthorizationService : TfsHttpClient
  {
    public AuthorizationService(TfsTeamProjectCollection connection)
      : base((TfsConnection) connection)
    {
    }

    protected override Guid CollectionServiceIdentifier => new Guid("6373ee32-aad4-4bf9-9ec8-72201ab1c45c");

    protected override string ServiceType => "Authorization";

    public void AddAccessControlEntry(string objectId, AccessControlEntry ace) => this.Invoke((TfsClientOperation) new AuthorizationService.AddAccessControlEntryClientOperation(), new object[2]
    {
      (object) objectId,
      (object) ace
    });

    public void CheckPermission(string objectId, string actionId) => this.Invoke((TfsClientOperation) new AuthorizationService.CheckPermissionClientOperation(), new object[2]
    {
      (object) objectId,
      (object) actionId
    });

    public string GetChangedAccessControlEntries(int sequence_id) => (string) this.Invoke((TfsClientOperation) new AuthorizationService.GetChangedAccessControlEntriesClientOperation(), new object[1]
    {
      (object) sequence_id
    });

    public string GetObjectClass(string objectId) => (string) this.Invoke((TfsClientOperation) new AuthorizationService.GetObjectClassClientOperation(), new object[1]
    {
      (object) objectId
    });

    public bool[] IsPermittedByActionList(string objectId, string[] actionId, string userSid) => (bool[]) this.Invoke((TfsClientOperation) new AuthorizationService.IsPermittedByActionListClientOperation(), new object[3]
    {
      (object) objectId,
      (object) actionId,
      (object) userSid
    });

    public bool[] IsPermittedByObjectList(string[] objectId, string actionId, string userSid) => (bool[]) this.Invoke((TfsClientOperation) new AuthorizationService.IsPermittedByObjectListClientOperation(), new object[3]
    {
      (object) objectId,
      (object) actionId,
      (object) userSid
    });

    public bool[] IsPermittedBySidList(string objectId, string actionId, string[] userSid) => (bool[]) this.Invoke((TfsClientOperation) new AuthorizationService.IsPermittedBySidListClientOperation(), new object[3]
    {
      (object) objectId,
      (object) actionId,
      (object) userSid
    });

    public string[] ListLocalizedActionNames(string objectClassId, string[] actionId) => (string[]) this.Invoke((TfsClientOperation) new AuthorizationService.ListLocalizedActionNamesClientOperation(), new object[2]
    {
      (object) objectClassId,
      (object) actionId
    });

    public string[] ListObjectClassActions(string objectClassId) => (string[]) this.Invoke((TfsClientOperation) new AuthorizationService.ListObjectClassActionsClientOperation(), new object[1]
    {
      (object) objectClassId
    });

    public string[] ListObjectClasses() => (string[]) this.Invoke((TfsClientOperation) new AuthorizationService.ListObjectClassesClientOperation(), Array.Empty<object>());

    public AccessControlEntry[] ReadAccessControlList(string objectId) => (AccessControlEntry[]) this.Invoke((TfsClientOperation) new AuthorizationService.ReadAccessControlListClientOperation(), new object[1]
    {
      (object) objectId
    });

    public AccessControlEntry[][] ReadAccessControlLists(string[] objectId) => (AccessControlEntry[][]) this.Invoke((TfsClientOperation) new AuthorizationService.ReadAccessControlListsClientOperation(), new object[1]
    {
      (object) objectId
    });

    public void RegisterObject(
      string objectId,
      string objectClassId,
      string projectUri,
      string parentObjectId)
    {
      this.Invoke((TfsClientOperation) new AuthorizationService.RegisterObjectClientOperation(), new object[4]
      {
        (object) objectId,
        (object) objectClassId,
        (object) projectUri,
        (object) parentObjectId
      });
    }

    public void RemoveAccessControlEntry(string objectId, AccessControlEntry ace) => this.Invoke((TfsClientOperation) new AuthorizationService.RemoveAccessControlEntryClientOperation(), new object[2]
    {
      (object) objectId,
      (object) ace
    });

    public void ReplaceAccessControlList(string objectId, AccessControlEntry[] acl) => this.Invoke((TfsClientOperation) new AuthorizationService.ReplaceAccessControlListClientOperation(), new object[2]
    {
      (object) objectId,
      (object) acl
    });

    public void ResetInheritance(string objectId, string parentObejctId) => this.Invoke((TfsClientOperation) new AuthorizationService.ResetInheritanceClientOperation(), new object[2]
    {
      (object) objectId,
      (object) parentObejctId
    });

    public void UnregisterObject(string objectId) => this.Invoke((TfsClientOperation) new AuthorizationService.UnregisterObjectClientOperation(), new object[1]
    {
      (object) objectId
    });

    internal AuthorizationService(TfsTeamProjectCollection server, string url)
      : base((TfsConnection) server, new Uri(url))
    {
    }

    protected override string ComponentName => "Authorization";

    internal sealed class AddAccessControlEntryClientOperation : TfsClientOperation
    {
      public override string BodyName => "AddAccessControlEntry";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Authorization/03/AddAccessControlEntry";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Authorization/03";

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter1 = (string) parameters[0];
        if (parameter1 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "objectId", parameter1);
        AccessControlEntry parameter2 = (AccessControlEntry) parameters[1];
        if (parameter2 == null)
          return;
        AccessControlEntry.ToXml((XmlWriter) writer, "ace", parameter2);
      }
    }

    internal sealed class CheckPermissionClientOperation : TfsClientOperation
    {
      public override string BodyName => "CheckPermission";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Authorization/03/CheckPermission";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Authorization/03";

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter1 = (string) parameters[0];
        if (parameter1 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "objectId", parameter1);
        string parameter2 = (string) parameters[1];
        if (parameter2 == null)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "actionId", parameter2);
      }
    }

    internal sealed class GetChangedAccessControlEntriesClientOperation : TfsClientOperation
    {
      public override string BodyName => "GetChangedAccessControlEntries";

      public override bool HasOutputs => true;

      public override string ResultName => "GetChangedAccessControlEntriesResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Authorization/03/GetChangedAccessControlEntries";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Authorization/03";

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

    internal sealed class GetObjectClassClientOperation : TfsClientOperation
    {
      public override string BodyName => "GetObjectClass";

      public override bool HasOutputs => true;

      public override string ResultName => "GetObjectClassResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Authorization/03/GetObjectClass";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Authorization/03";

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
        XmlUtility.ToXmlElement((XmlWriter) writer, "objectId", parameter);
      }
    }

    internal sealed class IsPermittedByActionListClientOperation : TfsClientOperation
    {
      public override string BodyName => "IsPermittedByActionList";

      public override bool HasOutputs => true;

      public override string ResultName => "IsPermittedByActionListResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Authorization/03/IsPermittedByActionList";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Authorization/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) Helper.ZeroLengthArrayOfBoolean;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) Helper.ArrayOfBooleanFromXml(reader, false);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter1 = (string) parameters[0];
        if (parameter1 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "objectId", parameter1);
        string[] parameter2 = (string[]) parameters[1];
        Helper.ToXml((XmlWriter) writer, "actionId", parameter2, false, false);
        string parameter3 = (string) parameters[2];
        if (parameter3 == null)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "userSid", parameter3);
      }
    }

    internal sealed class IsPermittedByObjectListClientOperation : TfsClientOperation
    {
      public override string BodyName => "IsPermittedByObjectList";

      public override bool HasOutputs => true;

      public override string ResultName => "IsPermittedByObjectListResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Authorization/03/IsPermittedByObjectList";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Authorization/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) Helper.ZeroLengthArrayOfBoolean;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) Helper.ArrayOfBooleanFromXml(reader, false);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string[] parameter1 = (string[]) parameters[0];
        Helper.ToXml((XmlWriter) writer, "objectId", parameter1, false, false);
        string parameter2 = (string) parameters[1];
        if (parameter2 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "actionId", parameter2);
        string parameter3 = (string) parameters[2];
        if (parameter3 == null)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "userSid", parameter3);
      }
    }

    internal sealed class IsPermittedBySidListClientOperation : TfsClientOperation
    {
      public override string BodyName => "IsPermittedBySidList";

      public override bool HasOutputs => true;

      public override string ResultName => "IsPermittedBySidListResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Authorization/03/IsPermittedBySidList";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Authorization/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) Helper.ZeroLengthArrayOfBoolean;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) Helper.ArrayOfBooleanFromXml(reader, false);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter1 = (string) parameters[0];
        if (parameter1 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "objectId", parameter1);
        string parameter2 = (string) parameters[1];
        if (parameter2 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "actionId", parameter2);
        string[] parameter3 = (string[]) parameters[2];
        Helper.ToXml((XmlWriter) writer, "userSid", parameter3, false, false);
      }
    }

    internal sealed class ListLocalizedActionNamesClientOperation : TfsClientOperation
    {
      public override string BodyName => "ListLocalizedActionNames";

      public override bool HasOutputs => true;

      public override string ResultName => "ListLocalizedActionNamesResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Authorization/03/ListLocalizedActionNames";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Authorization/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) Helper.ZeroLengthArrayOfString;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) Helper.ArrayOfStringFromXml(reader, false);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter1 = (string) parameters[0];
        if (parameter1 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "objectClassId", parameter1);
        string[] parameter2 = (string[]) parameters[1];
        Helper.ToXml((XmlWriter) writer, "actionId", parameter2, false, false);
      }
    }

    internal sealed class ListObjectClassActionsClientOperation : TfsClientOperation
    {
      public override string BodyName => "ListObjectClassActions";

      public override bool HasOutputs => true;

      public override string ResultName => "ListObjectClassActionsResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Authorization/03/ListObjectClassActions";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Authorization/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) Helper.ZeroLengthArrayOfString;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) Helper.ArrayOfStringFromXml(reader, false);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter = (string) parameters[0];
        if (parameter == null)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "objectClassId", parameter);
      }
    }

    internal sealed class ListObjectClassesClientOperation : TfsClientOperation
    {
      public override string BodyName => "ListObjectClasses";

      public override bool HasOutputs => true;

      public override string ResultName => "ListObjectClassesResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Authorization/03/ListObjectClasses";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Authorization/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) Helper.ZeroLengthArrayOfString;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) Helper.ArrayOfStringFromXml(reader, false);
    }

    internal sealed class ReadAccessControlListClientOperation : TfsClientOperation
    {
      public override string BodyName => "ReadAccessControlList";

      public override bool HasOutputs => true;

      public override string ResultName => "ReadAccessControlListResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Authorization/03/ReadAccessControlList";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Authorization/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) Helper.ZeroLengthArrayOfAccessControlEntry;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) Helper.ArrayOfAccessControlEntryFromXml(serviceProvider, reader, false);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter = (string) parameters[0];
        if (parameter == null)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "objectId", parameter);
      }
    }

    internal sealed class ReadAccessControlListsClientOperation : TfsClientOperation
    {
      public override string BodyName => "ReadAccessControlLists";

      public override bool HasOutputs => true;

      public override string ResultName => "ReadAccessControlListsResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Authorization/03/ReadAccessControlLists";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Authorization/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) Helper.ZeroLengthArrayOfArrayOfAccessControlEntry;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) Helper.ArrayOfArrayOfAccessControlEntryFromXml(serviceProvider, reader, false);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string[] parameter = (string[]) parameters[0];
        Helper.ToXml((XmlWriter) writer, "objectId", parameter, false, false);
      }
    }

    internal sealed class RegisterObjectClientOperation : TfsClientOperation
    {
      public override string BodyName => "RegisterObject";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Authorization/03/RegisterObject";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Authorization/03";

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter1 = (string) parameters[0];
        if (parameter1 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "objectId", parameter1);
        string parameter2 = (string) parameters[1];
        if (parameter2 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "objectClassId", parameter2);
        string parameter3 = (string) parameters[2];
        if (parameter3 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "projectUri", parameter3);
        string parameter4 = (string) parameters[3];
        if (parameter4 == null)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "parentObjectId", parameter4);
      }
    }

    internal sealed class RemoveAccessControlEntryClientOperation : TfsClientOperation
    {
      public override string BodyName => "RemoveAccessControlEntry";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Authorization/03/RemoveAccessControlEntry";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Authorization/03";

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter1 = (string) parameters[0];
        if (parameter1 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "objectId", parameter1);
        AccessControlEntry parameter2 = (AccessControlEntry) parameters[1];
        if (parameter2 == null)
          return;
        AccessControlEntry.ToXml((XmlWriter) writer, "ace", parameter2);
      }
    }

    internal sealed class ReplaceAccessControlListClientOperation : TfsClientOperation
    {
      public override string BodyName => "ReplaceAccessControlList";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Authorization/03/ReplaceAccessControlList";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Authorization/03";

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter1 = (string) parameters[0];
        if (parameter1 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "objectId", parameter1);
        AccessControlEntry[] parameter2 = (AccessControlEntry[]) parameters[1];
        Helper.ToXml((XmlWriter) writer, "acl", parameter2, false, true);
      }
    }

    internal sealed class ResetInheritanceClientOperation : TfsClientOperation
    {
      public override string BodyName => "ResetInheritance";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Authorization/03/ResetInheritance";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Authorization/03";

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter1 = (string) parameters[0];
        if (parameter1 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "objectId", parameter1);
        string parameter2 = (string) parameters[1];
        if (parameter2 == null)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "parentObejctId", parameter2);
      }
    }

    internal sealed class UnregisterObjectClientOperation : TfsClientOperation
    {
      public override string BodyName => "UnregisterObject";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Authorization/03/UnregisterObject";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Authorization/03";

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter = (string) parameters[0];
        if (parameter == null)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "objectId", parameter);
      }
    }
  }
}
