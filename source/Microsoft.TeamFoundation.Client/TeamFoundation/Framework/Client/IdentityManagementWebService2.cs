// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.IdentityManagementWebService2
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Client.Channels;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.ComponentModel;
using System.Web.Services.Protocols;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class IdentityManagementWebService2 : IdentityManagementWebService
  {
    public IdentityManagementWebService2(TfsConnection connection)
      : base(connection)
    {
    }

    protected override Guid CollectionServiceIdentifier => new Guid("a4ce4577-b38e-49c8-bdb4-b9c53615e0da");

    protected override string ComponentName => "Framework";

    protected override Guid ConfigurationServiceIdentifier => new Guid("6a67ca20-f7b4-4586-b8b6-cb4da7234919");

    protected override string ServiceType => "IdentityManagementService2";

    public void AddRecentUser(Guid teamFoundationId) => this.Invoke((TfsClientOperation) new IdentityManagementWebService2.AddRecentUserClientOperation(), new object[1]
    {
      (object) teamFoundationId
    });

    protected override Exception ConvertException(SoapException exception) => TeamFoundationServiceException.ConvertException(exception);

    public TeamFoundationIdentity[] GetMostRecentlyUsedUsers(int features) => (TeamFoundationIdentity[]) this.Invoke((TfsClientOperation) new IdentityManagementWebService2.GetMostRecentlyUsedUsersClientOperation(), new object[1]
    {
      (object) features
    });

    public FilteredIdentitiesList ReadFilteredIdentities(
      string expression,
      int suggestedPageSize,
      string lastSearchResult,
      bool lookForward,
      int queryMembership,
      int features)
    {
      return (FilteredIdentitiesList) this.Invoke((TfsClientOperation) new IdentityManagementWebService2.ReadFilteredIdentitiesClientOperation(), new object[6]
      {
        (object) expression,
        (object) suggestedPageSize,
        (object) lastSearchResult,
        (object) lookForward,
        (object) queryMembership,
        (object) features
      });
    }

    public void SetCustomDisplayName(string customDisplayName) => this.Invoke((TfsClientOperation) new IdentityManagementWebService2.SetCustomDisplayNameClientOperation(), new object[1]
    {
      (object) customDisplayName
    });

    public void UpdateIdentityExtendedProperties(
      IdentityDescriptor descriptor,
      PropertyValue[] updates,
      PropertyValue[] localUpdates)
    {
      this.Invoke((TfsClientOperation) new IdentityManagementWebService2.UpdateIdentityExtendedPropertiesClientOperation(), new object[3]
      {
        (object) descriptor,
        (object) updates,
        (object) localUpdates
      });
    }

    internal sealed class AddRecentUserClientOperation : TfsClientOperation
    {
      public override string BodyName => "AddRecentUser";

      public override string SoapAction => "http://microsoft.com/webservices/AddRecentUser";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        Guid parameter = (Guid) parameters[0];
        if (!(parameter != Guid.Empty))
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "teamFoundationId", parameter);
      }
    }

    internal sealed class GetMostRecentlyUsedUsersClientOperation : TfsClientOperation
    {
      public override string BodyName => "GetMostRecentlyUsedUsers";

      public override bool HasOutputs => true;

      public override string ResultName => "GetMostRecentlyUsedUsersResult";

      public override string SoapAction => "http://microsoft.com/webservices/GetMostRecentlyUsedUsers";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) Helper.ZeroLengthArrayOfTeamFoundationIdentity;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) Helper.ArrayOfTeamFoundationIdentityFromXml(serviceProvider, reader, false);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        int parameter = (int) parameters[0];
        if (parameter == 0)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "features", parameter);
      }
    }

    internal sealed class ReadFilteredIdentitiesClientOperation : TfsClientOperation
    {
      public override string BodyName => "ReadFilteredIdentities";

      public override bool HasOutputs => true;

      public override string ResultName => "ReadFilteredIdentitiesResult";

      public override string SoapAction => "http://microsoft.com/webservices/ReadFilteredIdentities";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) null;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) FilteredIdentitiesList.FromXml(serviceProvider, reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter1 = (string) parameters[0];
        if (parameter1 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "expression", parameter1);
        int parameter2 = (int) parameters[1];
        if (parameter2 != 0)
          XmlUtility.ToXmlElement((XmlWriter) writer, "suggestedPageSize", parameter2);
        string parameter3 = (string) parameters[2];
        if (parameter3 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "lastSearchResult", parameter3);
        bool parameter4 = (bool) parameters[3];
        if (parameter4)
          XmlUtility.ToXmlElement((XmlWriter) writer, "lookForward", parameter4);
        int parameter5 = (int) parameters[4];
        if (parameter5 != 0)
          XmlUtility.ToXmlElement((XmlWriter) writer, "queryMembership", parameter5);
        int parameter6 = (int) parameters[5];
        if (parameter6 == 0)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "features", parameter6);
      }
    }

    internal sealed class SetCustomDisplayNameClientOperation : TfsClientOperation
    {
      public override string BodyName => "SetCustomDisplayName";

      public override string SoapAction => "http://microsoft.com/webservices/SetCustomDisplayName";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter = (string) parameters[0];
        if (parameter == null)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "customDisplayName", parameter);
      }
    }

    internal sealed class UpdateIdentityExtendedPropertiesClientOperation : TfsClientOperation
    {
      public override string BodyName => "UpdateIdentityExtendedProperties";

      public override string SoapAction => "http://microsoft.com/webservices/UpdateIdentityExtendedProperties";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        IdentityDescriptor parameter1 = (IdentityDescriptor) parameters[0];
        if (parameter1 != null)
          IdentityDescriptor.ToXml((XmlWriter) writer, "descriptor", parameter1);
        PropertyValue[] parameter2 = (PropertyValue[]) parameters[1];
        Helper.ToXml((XmlWriter) writer, "updates", parameter2, false, false);
        PropertyValue[] parameter3 = (PropertyValue[]) parameters[2];
        Helper.ToXml((XmlWriter) writer, "localUpdates", parameter3, false, false);
      }
    }
  }
}
