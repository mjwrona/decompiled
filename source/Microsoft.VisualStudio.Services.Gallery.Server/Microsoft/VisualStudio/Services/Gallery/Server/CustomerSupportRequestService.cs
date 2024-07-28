// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.CustomerSupportRequestService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Gallery.Server.CSR;
using Microsoft.VisualStudio.Services.Gallery.Server.Utility;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public class CustomerSupportRequestService : ICustomerSupportRequestService, IVssFrameworkService
  {
    private const string RegistryPathForSupportEngineerVSID = "/Configuration/Service/Gallery/CustomerSupportRequest/SupportEngineerVSID";
    private const string RegistryPathForGithubRepoURL = "/Configuration/Service/Gallery/CustomerSupportRequest/GitHubRepositoryLink";
    private const string RegistryPathForVendorGitHubUserName = "/Configuration/Service/Gallery/CustomerSupportRequest/SupportEngineerGitHubUserName";
    private const string RegistryPathForVSMAgentGitHubUserName = "/Configuration/Service/Gallery/CustomerSupportRequest/VSMAgentGitHubUserId";
    private const string RegistryPathForMaxCSRThresholdPerDayPerUser = "/Configuration/Service/Gallery/CustomerSupportRequest/CreateCSRThresholdPerDayPerUser";
    private const string RegistryPathForCSRTicketSASTokenExpiryTimeInDays = "/Configuration/Service/Gallery/CustomerSupportRequest/CSRTicketSASTokenExpiryTimeInDays";
    private IMailNotification _mailNotification;
    private IAzureStorage _csrBlobStorage;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public CustomerSupportRequestService()
    {
    }

    public CustomerSupportRequestService(
      IAzureStorage azureStorage,
      IMailNotification mailNotification)
    {
      this._csrBlobStorage = azureStorage;
      this._mailNotification = mailNotification;
    }

    public void EnsureAzureStorageIsInitialized(IVssRequestContext requestContext)
    {
      if (this._csrBlobStorage != null)
        return;
      string valueFromStrongBox = requestContext.Elevate().GetSecretValueFromStrongBox("ConfigurationSecrets", "GalleryCSRStorageConnectionString");
      if (string.IsNullOrWhiteSpace(valueFromStrongBox))
        return;
      this._csrBlobStorage = (IAzureStorage) new AzureStorage(valueFromStrongBox);
    }

    public void EnsureMailNotificationIsInitialized()
    {
      if (this._mailNotification != null)
        return;
      this._mailNotification = (IMailNotification) new MailNotification();
    }

    public async Task CreateCustomerSupportRequestTicket(
      IVssRequestContext requestContext,
      CustomerSupportRequest customerSupportData)
    {
      this.ValidateUserInput(requestContext, customerSupportData);
      int gitHubIssueNumber = 0;
      string errorReason = string.Empty;
      this.EnsureAzureStorageIsInitialized(requestContext);
      this.EnsureMailNotificationIsInitialized();
      string status;
      if (string.Equals(customerSupportData.SourceLink, "footer") && string.Equals(customerSupportData.Reason, "other"))
      {
        status = "Success";
        errorReason = "footer - other";
        this.SendNotificationToSupportEngineer(requestContext, customerSupportData, errorReason);
      }
      else
      {
        HttpResponseMessage csrGitHubIssue = await this.CreateCSRGitHubIssue(requestContext, customerSupportData);
        if (csrGitHubIssue != null && csrGitHubIssue.StatusCode == HttpStatusCode.Created)
        {
          gitHubIssueNumber = requestContext.GetService<GitHubClient>().GetIssueNumberFromGitHubReponseUri(csrGitHubIssue.Headers.Location);
          status = "Success";
        }
        else
        {
          status = "Failed";
          errorReason = csrGitHubIssue == null ? GalleryResources.CSRUploadToBlobFailed() : csrGitHubIssue.ReasonPhrase;
          this.SendNotificationToSupportEngineer(requestContext, customerSupportData, errorReason);
        }
      }
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      if (userIdentity != null)
      {
        Guid id = userIdentity.Id;
        this.SendCSRAcknowledgementNotification(requestContext, userIdentity.Id, customerSupportData.Reason, gitHubIssueNumber);
      }
      this.PublishCustomerIntelligenceEvent(requestContext, customerSupportData, status, gitHubIssueNumber, errorReason);
      errorReason = (string) null;
    }

    private void ValidateUserInput(
      IVssRequestContext requestContext,
      CustomerSupportRequest customerSupportData)
    {
      if (!requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableSupportRequestFeature"))
        throw new HttpException(400, string.Format(GalleryResources.OperationNotSupported((object) nameof (customerSupportData))));
      if (this.isSignedInUser(requestContext))
      {
        ArgumentUtility.CheckStringForNullOrWhiteSpace(customerSupportData.ReporterVSID, "ReporterVSID");
        if (requestContext.GetUserIdentity().Id.ToString() != customerSupportData.ReporterVSID)
          throw new CSRAuthorIdentityMismatchException(GalleryResources.CSRAuthorIdentityMismatchException());
      }
      else if (!string.IsNullOrWhiteSpace(customerSupportData.ReporterVSID))
        throw new HttpException(400, string.Format(GalleryResources.InvalidReporterVSIDException()));
      string reCaptchaToken = customerSupportData.ReCaptchaToken;
      if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableReCaptchaInCreateCSR"))
      {
        if (ReCaptchaUtility.IsReCaptchaTokenValid(requestContext, reCaptchaToken))
        {
          customerSupportData.ReCaptchaToken = string.Empty;
          this.CSRReCaptchaTokenCIForCreateSupportRequest(requestContext, customerSupportData, "Valid");
        }
        else
        {
          this.CSRReCaptchaTokenCIForCreateSupportRequest(requestContext, customerSupportData, "Invalid");
          throw new InvalidReCaptchaTokenException(GalleryResources.InvalidReCaptchaToken());
        }
      }
      customerSupportData.ValidateMandatoryRequestFields();
      customerSupportData.ValidateSpecificRequestFields(requestContext);
    }

    private async Task<HttpResponseMessage> CreateCSRGitHubIssue(
      IVssRequestContext requestContext,
      CustomerSupportRequest customerSupportData)
    {
      string issueTitle = customerSupportData.SourceLink + "-" + customerSupportData.Reason;
      List<string> stringList = new List<string>()
      {
        SupportRequestConstants.labelToReasonMap[customerSupportData.Reason]
      };
      if (!this.isSignedInUser(requestContext))
        stringList.Add("NonSignedIn User");
      string[] issueLabels = stringList.ToArray();
      IVssRegistryService registryService = requestContext.GetService<IVssRegistryService>();
      string[] strArray = new string[1];
      IVssRegistryService registryService1 = registryService;
      IVssRequestContext requestContext1 = requestContext;
      RegistryQuery registryQuery = (RegistryQuery) "/Configuration/Service/Gallery/CustomerSupportRequest/SupportEngineerGitHubUserName";
      ref RegistryQuery local1 = ref registryQuery;
      string empty1 = string.Empty;
      strArray[0] = registryService1.GetValue<string>(requestContext1, in local1, empty1);
      string[] issueAssignees = strArray;
      try
      {
        string payload = JsonConvert.SerializeObject((object) new CSRGitHubIssuePayload(issueTitle, "Please find the blob URL for the ticket: " + await this.UploadCSRContentToBlobStorageAsync(requestContext, customerSupportData), issueAssignees, issueLabels));
        IVssRegistryService registryService2 = registryService;
        IVssRequestContext requestContext2 = requestContext;
        registryQuery = (RegistryQuery) "/Configuration/Service/Gallery/CustomerSupportRequest/GitHubRepositoryLink";
        ref RegistryQuery local2 = ref registryQuery;
        string empty2 = string.Empty;
        string url = registryService2.GetValue<string>(requestContext2, in local2, empty2);
        string valueFromStrongBox = requestContext.Elevate().GetSecretValueFromStrongBox("ConfigurationSecrets", "GalleryCSRGitHubPATToken");
        IVssRegistryService registryService3 = registryService;
        IVssRequestContext requestContext3 = requestContext;
        registryQuery = (RegistryQuery) "/Configuration/Service/Gallery/CustomerSupportRequest/VSMAgentGitHubUserId";
        ref RegistryQuery local3 = ref registryQuery;
        string empty3 = string.Empty;
        string userAgent = registryService3.GetValue<string>(requestContext3, in local3, empty3);
        return await requestContext.GetService<IGitHubClient>().CreateGitHubIssue(payload, url, valueFromStrongBox, userAgent);
      }
      catch (StorageException ex)
      {
        return new HttpResponseMessage()
        {
          StatusCode = (HttpStatusCode) ex.RequestInformation.HttpStatusCode,
          ReasonPhrase = ex.Message
        };
      }
    }

    private void SendCSRAcknowledgementNotification(
      IVssRequestContext requestContext,
      Guid userId,
      string reason,
      int githubIssueNumber)
    {
      string identityDisplayName = requestContext.GetUserIdentityDisplayName();
      CustomerSupportRequestAcknowledgementMailNotification mailNotification = new CustomerSupportRequestAcknowledgementMailNotification();
      mailNotification.UserDisplayName = identityDisplayName;
      if (githubIssueNumber != 0)
        mailNotification.Subject = "#" + githubIssueNumber.ToString();
      mailNotification.Subject = mailNotification.Subject + "- " + reason;
      mailNotification.NotificationContent = string.Empty;
      string g = requestContext.GetService<IVssRegistryService>().GetValue<string>(requestContext, (RegistryQuery) "/Configuration/Service/Gallery/CustomerSupportRequest/SupportEngineerVSID", string.Empty);
      Microsoft.VisualStudio.Services.Identity.Identity identity = new Microsoft.VisualStudio.Services.Identity.Identity();
      identity.Id = new Guid(g);
      ICollection<Microsoft.VisualStudio.Services.Identity.Identity> ccIdentities = (ICollection<Microsoft.VisualStudio.Services.Identity.Identity>) new List<Microsoft.VisualStudio.Services.Identity.Identity>()
      {
        identity
      };
      this._mailNotification.SendMailNotificationToIdentitiesWithCC(requestContext, (MailNotificationEventData) mailNotification, ccIdentities, userId);
    }

    internal async Task<string> UploadCSRContentToBlobStorageAsync(
      IVssRequestContext requestContext,
      CustomerSupportRequest customerSupportData)
    {
      IVssRegistryService registryService = requestContext.GetService<IVssRegistryService>();
      string blobContainerName = "customersupportblobcontainer";
      string blobPrefix = string.IsNullOrWhiteSpace(customerSupportData.ReporterVSID) ? customerSupportData.EmailId.ToString() : customerSupportData.ReporterVSID.ToString();
      string prefix = blobPrefix + "@" + string.Format("{0:yyyy-MM-dd}", (object) DateTime.UtcNow);
      List<IListBlobItem> listBlobItemList = await this._csrBlobStorage.ListAllBlobsWithPrefix(blobContainerName, prefix);
      IVssRegistryService registryService1 = registryService;
      IVssRequestContext requestContext1 = requestContext;
      RegistryQuery registryQuery = (RegistryQuery) "/Configuration/Service/Gallery/CustomerSupportRequest/CreateCSRThresholdPerDayPerUser";
      ref RegistryQuery local1 = ref registryQuery;
      int num = registryService1.GetValue<int>(requestContext1, in local1, 5);
      if (listBlobItemList != null && listBlobItemList.Count >= num)
        throw new CSRCreateThresholdExceededException(GalleryResources.CSRCreateThresholdExceededException());
      string blobName = blobPrefix + "@" + string.Format("{0:s}", (object) DateTime.UtcNow) + ".txt";
      IVssRegistryService registryService2 = registryService;
      IVssRequestContext requestContext2 = requestContext;
      registryQuery = (RegistryQuery) "/Configuration/Service/Gallery/CustomerSupportRequest/CSRTicketSASTokenExpiryTimeInDays";
      ref RegistryQuery local2 = ref registryQuery;
      int csrSasTokenExpiryTimeInDays = registryService2.GetValue<int>(requestContext2, in local2, 30);
      string blobUriWithSasToken;
      using (Stream ms = customerSupportData.ToJsonStream())
        blobUriWithSasToken = (await this._csrBlobStorage.UploadContentToBlobStorageAsync(ms, blobContainerName, blobName)).GetBlobUriWithSasToken(csrSasTokenExpiryTimeInDays);
      registryService = (IVssRegistryService) null;
      blobContainerName = (string) null;
      blobPrefix = (string) null;
      return blobUriWithSasToken;
    }

    public void SendNotificationToSupportEngineer(
      IVssRequestContext requestContext,
      CustomerSupportRequest customerSupportData,
      string failureReason)
    {
      ArgumentUtility.CheckForNull<CustomerSupportRequest>(customerSupportData, nameof (customerSupportData));
      ArgumentUtility.CheckForNull<string>(failureReason, nameof (failureReason));
      CustomerSupportRequestCreationFailedMailNotification mailNotification = new CustomerSupportRequestCreationFailedMailNotification(customerSupportData);
      mailNotification.GithubIssueCreateFailureReason = failureReason;
      mailNotification.NotificationContent = string.Empty;
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      mailNotification.GithubRepoLink = service.GetValue<string>(requestContext, (RegistryQuery) "/Configuration/Service/Gallery/CustomerSupportRequest/GitHubRepositoryLink", string.Empty);
      string g = service.GetValue<string>(requestContext, (RegistryQuery) "/Configuration/Service/Gallery/CustomerSupportRequest/SupportEngineerVSID", string.Empty);
      this._mailNotification.SendMailNotificationToUser(requestContext, new Guid(g), (MailNotificationEventData) mailNotification);
    }

    private void PublishCustomerIntelligenceEvent(
      IVssRequestContext requestContext,
      CustomerSupportRequest customerSupportData,
      string status,
      int githubIssueNo,
      string errorReason)
    {
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add(CustomerIntelligenceProperty.Action, "Created");
      properties.Add("Status", status);
      properties.Add("Publisher", customerSupportData.PublisherName);
      properties.Add("Extension", customerSupportData.ExtensionName);
      properties.Add("VSID", customerSupportData.ReporterVSID);
      properties.Add("Source", customerSupportData.SourceLink);
      properties.Add("Reason", customerSupportData.Reason);
      properties.Add("EmailId", customerSupportData.EmailId);
      properties.Add("ErrorReason", errorReason);
      properties.Add("GithubIssueNo", (double) githubIssueNo);
      if (customerSupportData.SourceLink == "Appeal Review or Q and A")
        properties.Add("Review", (object) customerSupportData.Review);
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Microsoft.VisualStudio.Services.Gallery", "CustomerSupportRequest", properties);
    }

    private void CSRReCaptchaTokenCIForCreateSupportRequest(
      IVssRequestContext requestContext,
      CustomerSupportRequest customerSupportData,
      string featureValidation)
    {
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add(CustomerIntelligenceProperty.Action, "ReCaptchaValidation");
      properties.Add("Source", "CustomerSupportRequest");
      properties.Add("Scenario", "CreateScenario");
      properties.Add("FeatureValidation", featureValidation);
      properties.Add("UserAgent", requestContext.UserAgent);
      properties.Add("Publisher", customerSupportData.PublisherName);
      properties.Add("Extension", customerSupportData.ExtensionName);
      properties.Add("Vsid", customerSupportData.ReporterVSID);
      properties.Add("Source", customerSupportData.SourceLink);
      properties.Add("Reason", customerSupportData.Reason);
      properties.Add("EmailId", customerSupportData.EmailId);
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Microsoft.VisualStudio.Services.Gallery", "ReCaptchaValidation", properties);
    }

    public bool isSignedInUser(IVssRequestContext requestContext)
    {
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      if (userIdentity == null)
        return false;
      Guid id = userIdentity.Id;
      return true;
    }
  }
}
