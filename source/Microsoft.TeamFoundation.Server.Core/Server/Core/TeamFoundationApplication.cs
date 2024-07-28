// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.TeamFoundationApplication
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Xml;

namespace Microsoft.TeamFoundation.Server.Core
{
  public class TeamFoundationApplication : VisualStudioServicesApplication
  {
    private static readonly UTF8Encoding s_soapFaultEncoding = new UTF8Encoding(false);
    private const string c_area = "TeamFoundationApplication";
    private const string c_layer = "WebServices";

    protected override void OnFirstRequest()
    {
      base.OnFirstRequest();
      if (!this.EnsureWebApplicationNodeExists())
        return;
      VisualStudioServicesApplication.s_isWebAppRegistered = true;
    }

    private bool EnsureWebApplicationNodeExists()
    {
      EventLogEntryType level = VisualStudioServicesApplication.s_registrationAttemptsRemaining > 0 ? EventLogEntryType.Warning : EventLogEntryType.Error;
      using (IVssRequestContext systemContext = TeamFoundationApplicationCore.DeploymentServiceHost.CreateSystemContext(false))
      {
        ITeamFoundationCatalogService service = systemContext.GetService<ITeamFoundationCatalogService>();
        List<CatalogNode> catalogNodeList1 = service.QueryNodes(systemContext, (IEnumerable<string>) new string[1]
        {
          CatalogRoots.InfrastructurePath + CatalogConstants.SingleRecurseStar
        }, (IEnumerable<Guid>) new Guid[1]
        {
          CatalogResourceTypes.Machine
        }, (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[1]
        {
          new KeyValuePair<string, string>("MachineName", Environment.MachineName)
        }, CatalogQueryOptions.None);
        if (catalogNodeList1.Count == 0)
        {
          TeamFoundationEventLog.Default.Log(FrameworkResources.RegisterErrorMachineNodeNotFound((object) Environment.MachineName), TeamFoundationEventId.RegisterWebApplicationError, level);
          return false;
        }
        CatalogNode catalogNode1 = catalogNodeList1[0];
        if (catalogNode1.QueryChildren(systemContext, (IEnumerable<Guid>) new Guid[1]
        {
          CatalogResourceTypes.TeamFoundationWebApplication
        }, false, CatalogQueryOptions.None).Count > 0)
          return true;
        try
        {
          List<CatalogNode> catalogNodeList2 = service.QueryNodes(systemContext, (IEnumerable<string>) new string[1]
          {
            CatalogRoots.OrganizationalPath + CatalogConstants.SingleRecurseStar
          }, (IEnumerable<Guid>) new Guid[1]
          {
            CatalogResourceTypes.TeamFoundationServerInstance
          }, CatalogQueryOptions.ExpandDependencies);
          if (catalogNodeList2.Count > 0)
          {
            CatalogNode catalogNode2 = catalogNodeList2[0];
            CatalogNode child = catalogNode1.CreateChild(systemContext, CatalogResourceTypes.TeamFoundationWebApplication, FrameworkResources.TeamFoundationWebApplication());
            catalogNode2.Dependencies.GetDependencySet("WebApplication").Add(child);
            CatalogTransactionContext transactionContext = service.CreateTransactionContext();
            transactionContext.AttachNode(child);
            transactionContext.AttachNode(catalogNode2);
            transactionContext.Save(systemContext, false);
            return true;
          }
          TeamFoundationEventLog.Default.Log(FrameworkResources.RegisterErrorInstanceNodeNotFound(), TeamFoundationEventId.RegisterWebApplicationError, level);
        }
        catch (InvalidCatalogSaveResourceException ex)
        {
          TeamFoundationTracingService.TraceExceptionRaw(60482, nameof (TeamFoundationApplication), "WebServices", (Exception) ex);
          TeamFoundationEventLog.Default.LogException(FrameworkResources.ErrorRegisteringWebApplication(), (Exception) ex, TeamFoundationEventId.RegisterWebApplicationError, level);
        }
        return false;
      }
    }

    public override void AddExceptionHeadersToList(
      List<KeyValuePair<string, string>> headers,
      Exception ex)
    {
      TeamFoundationTracingService.TraceRaw(60362, TraceLevel.Warning, nameof (TeamFoundationApplication), "WebServices", "Starting to write SOAP fault response");
      string str = TeamFoundationApplication.WriteSoapFaultResponse(ex);
      TeamFoundationTracingService.TraceRaw(60363, TraceLevel.Warning, nameof (TeamFoundationApplication), "WebServices", "Finished writing SOAP fault response");
      if (!string.IsNullOrEmpty(str))
        headers.Add(new KeyValuePair<string, string>("X-TFS-SoapException", HttpUtility.UrlEncode(str, (Encoding) TeamFoundationApplication.s_soapFaultEncoding)));
      base.AddExceptionHeadersToList(headers, ex);
    }

    private static string WriteSoapFaultResponse(Exception exceptionToSerialize)
    {
      switch (exceptionToSerialize)
      {
        case SoapSerializableException serializableException:
          pattern_0 = serializableException.ToSoapException();
          break;
        case TeamFoundationServiceException ex:
          pattern_0 = ex.ToSoapException();
          break;
      }
      if (pattern_0 == null)
        return (string) null;
      using (MemoryStream memoryStream = new MemoryStream())
      {
        MemoryStream output = memoryStream;
        using (XmlWriter xmlWriter = XmlWriter.Create((Stream) output, new XmlWriterSettings()
        {
          Encoding = (Encoding) new UTF8Encoding(false),
          CheckCharacters = false
        }))
        {
          xmlWriter.WriteStartDocument();
          xmlWriter.WriteStartElement("soap", "Envelope", "http://www.w3.org/2003/05/soap-envelope");
          xmlWriter.WriteStartElement("Body", "http://www.w3.org/2003/05/soap-envelope");
          xmlWriter.WriteStartElement("Fault", "http://www.w3.org/2003/05/soap-envelope");
          xmlWriter.WriteStartElement("Code", "http://www.w3.org/2003/05/soap-envelope");
          xmlWriter.WriteElementString("Value", "http://www.w3.org/2003/05/soap-envelope", "soap:Receiver");
          xmlWriter.WriteStartElement("Subcode", "http://www.w3.org/2003/05/soap-envelope");
          xmlWriter.WriteElementString("Value", "http://www.w3.org/2003/05/soap-envelope", exceptionToSerialize.GetType().Name);
          xmlWriter.WriteEndElement();
          xmlWriter.WriteEndElement();
          xmlWriter.WriteStartElement("Reason", "http://www.w3.org/2003/05/soap-envelope");
          xmlWriter.WriteStartElement("Text", "http://www.w3.org/2003/05/soap-envelope");
          xmlWriter.WriteAttributeString("xml", "lang", (string) null, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
          xmlWriter.WriteValue(SecretUtility.ScrubSecrets(exceptionToSerialize.Message));
          xmlWriter.WriteEndElement();
          xmlWriter.WriteEndElement();
          if (pattern_0.Detail != null)
            xmlWriter.WriteRaw(pattern_0.Detail.OuterXml);
          xmlWriter.WriteEndDocument();
          xmlWriter.Flush();
          memoryStream.Seek(0L, SeekOrigin.Begin);
          return Encoding.UTF8.GetString(memoryStream.GetBuffer(), 0, (int) memoryStream.Length);
        }
      }
    }
  }
}
