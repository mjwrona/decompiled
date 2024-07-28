// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.PublisherDomainVerificationService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using DnsClient;
using DnsClient.Protocol;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.Server.Components;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public class PublisherDomainVerificationService : 
    IPublisherDomainVerificationService,
    IVssFrameworkService
  {
    private const string MarketplaceTXTRecordKeyPrefix = "_visual-studio-marketplace-";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public Guid FetchDomainToken(IVssRequestContext requestContext, string publisherName)
    {
      if (new PublisherService().QueryPublisher(requestContext, publisherName, PublisherQueryFlags.None, false) != null)
      {
        using (PublisherComponent component = requestContext.CreateComponent<PublisherComponent>())
        {
          if (component is PublisherComponent8)
            return (component as PublisherComponent8).FetchDomainToken(publisherName);
        }
      }
      return Guid.Empty;
    }

    public void VerifyDomainToken(IVssRequestContext requestContext, string publisherName)
    {
      Publisher publisher = requestContext.GetService<IPublisherService>().QueryPublisher(requestContext, publisherName, PublisherQueryFlags.None);
      PublisherPermissions requestedPermissions = PublisherPermissions.EditSettings;
      GallerySecurity.CheckPublisherPermission(requestContext, publisher, requestedPermissions);
      if (!this.IsDomainTokenInDnsRecords(requestContext, publisher, (string) null))
        throw new TokenNotFoundInDnsTxtRecordsException(GalleryWebApiResources.TokenNotFoundInDnsTxtRecords());
      using (PublisherComponent component = requestContext.CreateComponent<PublisherComponent>())
      {
        if (component is PublisherComponent9)
          (component as PublisherComponent9).UpdateDomainVerificationState(publisher.PublisherId, UpdateVerificationStateOperation.verifyDnsToken, DateTime.UtcNow);
      }
      this.PublishCustomerIntelligenceEventForDomainVerification(requestContext, "VerifyDnsRecord", publisherName, publisher.Domain);
    }

    public void MarkPublisherDomainAsVerified(
      IVssRequestContext requestContext,
      string publisherName)
    {
      this.MarkPublisherDomainState(requestContext, publisherName, UpdateVerificationStateOperation.verifyDomain);
      this.PublishCustomerIntelligenceEventForDomainVerification(requestContext, nameof (MarkPublisherDomainAsVerified), publisherName, (string) null);
    }

    public void MarkPublisherDomainAsUnverified(
      IVssRequestContext requestContext,
      string publisherName)
    {
      this.MarkPublisherDomainState(requestContext, publisherName, UpdateVerificationStateOperation.unverify);
    }

    public List<PublisherDomainModel> FetchVerifiedPublishers(IVssRequestContext requestContext)
    {
      using (PublisherComponent component = requestContext.CreateComponent<PublisherComponent>())
      {
        if (component is PublisherComponent9)
          return (component as PublisherComponent9).FetchVerifiedPublishers();
      }
      return new List<PublisherDomainModel>();
    }

    public bool IsDomainTokenInDnsRecords(
      IVssRequestContext requestContext,
      Publisher publisher,
      string token = null)
    {
      if (string.IsNullOrWhiteSpace(token))
        token = this.FetchDomainToken(requestContext, publisher.PublisherName).ToString();
      LookupClient lookupClient = new LookupClient();
      string str1 = new Uri(publisher.Domain).Host;
      if (str1.Split(new char[1]{ '.' }, StringSplitOptions.RemoveEmptyEntries)[0] == "www")
        str1 = str1.Substring(4);
      string str2 = "_visual-studio-marketplace-" + publisher.PublisherName + "." + str1;
      IReadOnlyList<DnsResourceRecord> answers1 = lookupClient.Query(str2.ToLowerInvariant(), (QueryType) 16, (QueryClass) 1).Answers;
      if (answers1 != null)
      {
        foreach (TxtRecord txtRecord in RecordCollectionExtension.OfRecordType((IEnumerable<DnsResourceRecord>) answers1, (ResourceRecordType) 16))
        {
          if (txtRecord.Text.Count > 0 && string.Equals(token, txtRecord.Text.First<string>(), StringComparison.OrdinalIgnoreCase))
            return true;
        }
      }
      IReadOnlyList<DnsResourceRecord> answers2 = lookupClient.Query(str2, (QueryType) 16, (QueryClass) 1).Answers;
      if (answers2 != null)
      {
        foreach (TxtRecord txtRecord in RecordCollectionExtension.OfRecordType((IEnumerable<DnsResourceRecord>) answers2, (ResourceRecordType) 16))
        {
          if (txtRecord.Text.Count > 0 && string.Equals(token, txtRecord.Text.First<string>(), StringComparison.OrdinalIgnoreCase))
            return true;
        }
      }
      return false;
    }

    private void PublishCustomerIntelligenceEventForDomainVerification(
      IVssRequestContext requestContext,
      string featureName,
      string publisherName,
      string domain)
    {
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add(CustomerIntelligenceProperty.Action, featureName);
      properties.Add("PublisherName", publisherName);
      if (domain != null)
        properties.Add("Domain", domain);
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Microsoft.VisualStudio.Services.Gallery", "Publisher", properties);
    }

    private void MarkPublisherDomainState(
      IVssRequestContext requestContext,
      string publisherName,
      UpdateVerificationStateOperation state)
    {
      IPublisherService service = requestContext.GetService<IPublisherService>();
      Dictionary<Guid, string> publisherIds = service.GetPublisherIds(requestContext, new List<string>(1)
      {
        publisherName
      });
      if (publisherIds.Count == 0)
        throw new PublisherDoesNotExistException(GalleryResources.PublisherDoesNotExist());
      Guid key = publisherIds.First<KeyValuePair<Guid, string>>((Func<KeyValuePair<Guid, string>, bool>) (p => string.Equals(p.Value, publisherName))).Key;
      using (PublisherComponent component = requestContext.CreateComponent<PublisherComponent>())
      {
        if (component is PublisherComponent9)
          (component as PublisherComponent9).UpdateDomainVerificationState(key, state, DateTime.UtcNow);
      }
      service.PublishPublisherUpdatedEvent(requestContext, publisherName);
    }
  }
}
