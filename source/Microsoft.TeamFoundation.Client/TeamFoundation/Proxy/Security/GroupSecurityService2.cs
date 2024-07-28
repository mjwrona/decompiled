// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Proxy.Security.GroupSecurityService2
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Client.Channels;
using Microsoft.TeamFoundation.Server;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Web.Services.Protocols;
using System.Xml;

namespace Microsoft.TeamFoundation.Proxy.Security
{
  internal sealed class GroupSecurityService2 : TfsHttpClient
  {
    private const string c_soapAction = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/GroupSecurity/03/GetIdentityChanges";
    private const string c_soapNamespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/GroupSecurity/03";

    public GroupSecurityService2(TfsTeamProjectCollection tfs)
      : base((TfsConnection) tfs)
    {
    }

    protected override string ComponentName => "Services";

    protected override string ServiceType => "GroupSecurity2";

    protected override Guid CollectionServiceIdentifier => IntegrationServiceIdentifiers.GroupSecurity2;

    public int GetIdentityChanges(
      IClientContext clientContext,
      int sequenceId,
      out IResultCollection<Identity> identities)
    {
      identities = (IResultCollection<Identity>) new ResultCollection<Identity, Identity>();
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      TfsMessage message = this.Channel.Request(TfsMessage.CreateMessage("http://schemas.microsoft.com/TeamFoundation/2005/06/Services/GroupSecurity/03/GetIdentityChanges", new TfsBodyWriter(nameof (GetIdentityChanges), "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/GroupSecurity/03", new object[1]
      {
        (object) sequenceId
      }, GroupSecurityService2.\u003C\u003EO.\u003C0\u003E__WriteParameters ?? (GroupSecurityService2.\u003C\u003EO.\u003C0\u003E__WriteParameters = new Action<XmlDictionaryWriter, object[]>(GroupSecurityService2.WriteParameters)))));
      if (message.IsFault)
      {
        Exception e = message.CreateException();
        e = e is SoapException ? this.ConvertException((SoapException) e) : throw e;
      }
      else
      {
        int identityChanges = 0;
        bool flag = true;
        XmlDictionaryReader dictionaryReader = (XmlDictionaryReader) null;
        try
        {
          dictionaryReader = message.GetBodyReader();
          int num = dictionaryReader.IsEmptyElement ? 1 : 0;
          dictionaryReader.Read();
          if (num == 0)
          {
            if (string.Equals(dictionaryReader.LocalName, "GetIdentityChangesResult", StringComparison.Ordinal) && string.Equals(dictionaryReader.NamespaceURI, "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/GroupSecurity/03", StringComparison.Ordinal))
              identityChanges = XmlUtility.Int32FromXmlElement((XmlReader) dictionaryReader);
            TfsRequestContext context = new TfsRequestContext(clientContext, (TfsHttpClientBase) this, message, dictionaryReader, nameof (GetIdentityChanges));
            identities = (IResultCollection<Identity>) new ResultCollection<Identity, Identity>(context, nameof (identities));
          }
          flag = false;
        }
        finally
        {
          if (flag)
          {
            dictionaryReader?.Close();
            message?.Close();
          }
        }
        return identityChanges;
      }
    }

    private static void WriteParameters(XmlDictionaryWriter writer, object[] parameters) => XmlUtility.ToXmlElement((XmlWriter) writer, "sequenceId", (int) parameters[0]);
  }
}
