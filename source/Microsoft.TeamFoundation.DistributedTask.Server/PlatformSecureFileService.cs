// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.PlatformSecureFileService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.DistributedTask.Core.Server.Plugins;
using Microsoft.TeamFoundation.DistributedTask.Expressions;
using Microsoft.TeamFoundation.DistributedTask.Logging;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Server.DataAccess;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal class PlatformSecureFileService : ISecureFileService, IVssFrameworkService
  {
    private readonly LibrarySecurityProvider m_securityProvider;
    private readonly ISecureFileSecretsHelper m_secretsHelper;
    private const string c_layer = "PlatformSecureFileService";

    public PlatformSecureFileService()
      : this((LibrarySecurityProvider) new SecureFileSecurityProvider(), (ISecureFileSecretsHelper) new PlatformSecureFileService.SecureFileSecretsHelper())
    {
    }

    public PlatformSecureFileService(
      LibrarySecurityProvider securityProvider,
      ISecureFileSecretsHelper secretsHelper)
    {
      this.m_securityProvider = securityProvider;
      this.m_secretsHelper = secretsHelper;
    }

    public SecureFile UploadSecureFile(
      IVssRequestContext requestContext,
      Guid projectId,
      string secureFileName,
      Stream content,
      bool authorizePipelines = false)
    {
      using (new MethodScope(requestContext, nameof (PlatformSecureFileService), nameof (UploadSecureFile)))
      {
        SecureFile secureFile1 = new SecureFile()
        {
          Id = Guid.NewGuid(),
          Name = secureFileName
        };
        ArgumentValidation.CheckSecureFile(secureFile1, "secureFile");
        this.m_securityProvider.CheckCreatePermissions(requestContext, new Guid?(projectId), TaskResources.SecureFile());
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
        secureFile1.CreatedBy = new IdentityRef()
        {
          Id = userIdentity.Id.ToString("D")
        };
        SecureFile secureFile2;
        using (SecureFileComponent component = requestContext.CreateComponent<SecureFileComponent>())
          secureFile2 = component.AddSecureFile(projectId, secureFile1);
        if (secureFile2 != null)
        {
          try
          {
            this.m_secretsHelper.StoreSecrets(requestContext, projectId, secureFile2, content);
            this.m_securityProvider.AddLibraryItemCreatorAsItemAdministrator(requestContext, new Guid?(projectId), userIdentity, secureFile2.Id.ToString());
            IdentityService service = requestContext.GetService<IdentityService>();
            secureFile2.CreatedBy = service.GetIdentity(requestContext, secureFile1.CreatedBy.Id).ToIdentityRef(requestContext);
            secureFile2.ModifiedBy = secureFile2.CreatedBy;
            if (authorizePipelines)
              this.SendSecureFileNotificationEvent(requestContext, projectId, secureFile1);
            requestContext.TraceSecureFileEvent(TraceLevel.Verbose, projectId, secureFile2, "UploadSecureFile finished");
          }
          catch (Exception ex)
          {
            requestContext.TraceException(nameof (PlatformSecureFileService), ex);
            requestContext.TraceSecureFileEvent(TraceLevel.Warning, projectId, secureFile1, "UploadSecureFile failed to finish");
            this.DeleteSecureFile(requestContext, projectId, secureFile2.Id);
            throw;
          }
        }
        return secureFile2;
      }
    }

    public Stream DownloadSecureFile(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid secureFileId,
      string downloadTicket)
    {
      using (new MethodScope(requestContext, nameof (PlatformSecureFileService), nameof (DownloadSecureFile)))
      {
        DateTime utcNow = DateTime.UtcNow;
        ArgumentUtility.CheckForEmptyGuid(secureFileId, nameof (secureFileId));
        if (this.m_secretsHelper.ValidateTicket(requestContext, projectId, secureFileId, downloadTicket, utcNow))
        {
          SecureFile secureFile = this.GetSecureFile(requestContext.Elevate(), projectId, secureFileId);
          if (secureFile != null)
          {
            try
            {
              Stream stream = this.m_secretsHelper.ReadSecrets(requestContext, projectId, secureFile.Id);
              if (stream == null)
                requestContext.TraceSecureFileEvent(TraceLevel.Warning, projectId, secureFile, "Failed to access content: null returned");
              else
                requestContext.TraceSecureFileEvent(TraceLevel.Verbose, projectId, secureFile, "Content accessed");
              return stream;
            }
            catch (Exception ex)
            {
              requestContext.TraceException(nameof (PlatformSecureFileService), ex);
              requestContext.TraceSecureFileEvent(TraceLevel.Warning, projectId, secureFile, "Failed to access content: there was an Exception");
              throw;
            }
          }
          else
            requestContext.TraceSecureFileEvent(TraceLevel.Warning, projectId, secureFileId, "Attempted to access content of non-existent file");
        }
        return (Stream) null;
      }
    }

    public async Task<SecureFile> GetSecureFileAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid secureFileId,
      bool includeDownloadTicket = false,
      SecureFileActionFilter actionFilter = SecureFileActionFilter.None)
    {
      SecureFile secureFileAsync;
      using (new MethodScope(requestContext, nameof (PlatformSecureFileService), nameof (GetSecureFileAsync)))
      {
        ArgumentUtility.CheckForEmptyGuid(secureFileId, nameof (secureFileId));
        List<SecureFile> secureFilesAsync;
        using (SecureFileComponent component = requestContext.CreateComponent<SecureFileComponent>())
          secureFilesAsync = await component.GetSecureFilesAsync(projectId, (IEnumerable<Guid>) new Guid[1]
          {
            secureFileId
          });
        SecureFile secureFile = this.FilterResults(requestContext, projectId, (IList<SecureFile>) secureFilesAsync, includeDownloadTicket, actionFilter, false).SingleOrDefault<SecureFile>();
        if (secureFile == null)
          requestContext.TraceSecureFileEvent(TraceLevel.Warning, projectId, secureFileId, "Attempted to Select a DB Row with non-existent ID");
        secureFileAsync = secureFile;
      }
      return secureFileAsync;
    }

    public async Task<IList<SecureFile>> GetSecureFilesAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<Guid> secureFileIds,
      bool includeDownloadTickets = false,
      SecureFileActionFilter actionFilter = SecureFileActionFilter.None)
    {
      using (new MethodScope(requestContext, nameof (PlatformSecureFileService), nameof (GetSecureFilesAsync)))
      {
        List<SecureFile> secureFiles = new List<SecureFile>();
        if (secureFileIds == null || !secureFileIds.Any<Guid>())
          return (IList<SecureFile>) secureFiles;
        foreach (Guid secureFileId in secureFileIds)
          ArgumentUtility.CheckForEmptyGuid(secureFileId, nameof (secureFileIds));
        using (SecureFileComponent component = requestContext.CreateComponent<SecureFileComponent>())
          secureFiles = await component.GetSecureFilesAsync(projectId, secureFileIds);
        return this.FilterResults(requestContext, projectId, (IList<SecureFile>) secureFiles, includeDownloadTickets, actionFilter, false);
      }
    }

    public async Task<IList<SecureFile>> GetSecureFilesAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<string> secureFileNames,
      bool includeDownloadTickets = false,
      SecureFileActionFilter actionFilter = SecureFileActionFilter.None)
    {
      using (new MethodScope(requestContext, nameof (PlatformSecureFileService), nameof (GetSecureFilesAsync)))
      {
        List<SecureFile> secureFiles = new List<SecureFile>();
        if (secureFileNames == null || !secureFileNames.Any<string>())
          return (IList<SecureFile>) secureFiles;
        foreach (string secureFileName in secureFileNames)
          ArgumentUtility.CheckStringForNullOrEmpty(secureFileName, nameof (secureFileNames));
        using (SecureFileComponent component = requestContext.CreateComponent<SecureFileComponent>())
          secureFiles = await component.GetSecureFilesAsync(projectId, secureFileNames);
        return this.FilterResults(requestContext, projectId, (IList<SecureFile>) secureFiles, includeDownloadTickets, actionFilter);
      }
    }

    public IList<SecureFile> GetSecureFiles(
      IVssRequestContext requestContext,
      Guid projectId,
      string secureFileNamePattern,
      bool includeDownloadTickets = false,
      SecureFileActionFilter actionFilter = SecureFileActionFilter.None)
    {
      using (new MethodScope(requestContext, nameof (PlatformSecureFileService), nameof (GetSecureFiles)))
      {
        this.m_securityProvider.CheckAndInitializeLibraryPermissions(requestContext, new Guid?(projectId));
        List<SecureFile> secureFiles;
        using (SecureFileComponent component = requestContext.CreateComponent<SecureFileComponent>())
          secureFiles = component.GetSecureFiles(projectId, secureFileNamePattern);
        return this.FilterResults(requestContext, projectId, (IList<SecureFile>) secureFiles, includeDownloadTickets, actionFilter);
      }
    }

    public void DeleteSecureFile(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid secureFileId)
    {
      using (new MethodScope(requestContext, nameof (PlatformSecureFileService), nameof (DeleteSecureFile)))
      {
        ArgumentUtility.CheckForEmptyGuid(secureFileId, nameof (secureFileId));
        this.m_securityProvider.CheckPermissions(requestContext, new Guid?(projectId), secureFileId.ToString(), 2, false, TaskResources.SecureFileAccessDeniedForAdminOperation());
        SecureFile secureFile = (SecureFile) null;
        using (SecureFileComponent component = requestContext.CreateComponent<SecureFileComponent>())
          secureFile = component.DeleteSecureFile(projectId, secureFileId);
        if (secureFile == null)
        {
          requestContext.TraceSecureFileEvent(TraceLevel.Warning, projectId, secureFileId, "Attempted to delete non-existent file");
        }
        else
        {
          requestContext.TraceSecureFileEvent(TraceLevel.Info, projectId, secureFile, "Deleted");
          this.m_secretsHelper.DeleteSecrets(requestContext, projectId, secureFileId);
          try
          {
            this.m_securityProvider.RemoveAccessControlLists(requestContext, projectId, secureFileId.ToString());
          }
          catch (Exception ex)
          {
            requestContext.TraceException(10015013, "SecureFile", ex);
          }
          try
          {
            IPipelineResourceAuthorizationProxyService authorizationProxyService = requestContext.GetService<IPipelineResourceAuthorizationProxyService>();
            requestContext.RunSynchronously((Func<Task>) (() => authorizationProxyService.DeletePipelinePermissionsForResource(requestContext.Elevate(), projectId, secureFileId.ToString(), "securefile")));
          }
          catch (Exception ex)
          {
            string format = "Deleting pipeline permissions for secure file with ID: {0} failed with the following error: {1}";
            requestContext.TraceError(10015178, "DistributedTask", format, (object) secureFileId.ToString(), (object) ex.Message);
          }
        }
      }
    }

    public SecureFile UpdateSecureFile(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid secureFileId,
      SecureFile secureFile)
    {
      using (new MethodScope(requestContext, nameof (PlatformSecureFileService), nameof (UpdateSecureFile)))
      {
        ArgumentUtility.CheckForEmptyGuid(secureFileId, nameof (secureFileId));
        ArgumentValidation.CheckSecureFile(secureFile, nameof (secureFile));
        this.m_securityProvider.CheckPermissions(requestContext, new Guid?(projectId), secureFileId.ToString(), 2, false, TaskResources.SecureFileAccessDeniedForAdminOperation());
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
        secureFile.ModifiedBy = new IdentityRef()
        {
          Id = userIdentity.Id.ToString("D")
        };
        secureFile.Id = secureFileId;
        SecureFile secureFile1;
        using (SecureFileComponent component = requestContext.CreateComponent<SecureFileComponent>())
          secureFile1 = component.UpdateSecureFile(projectId, secureFile.Id, secureFile);
        requestContext.TraceSecureFileEvent(TraceLevel.Info, projectId, secureFile1, "DB Row Updated. The File may have been renamed.");
        return secureFile1;
      }
    }

    public IList<SecureFile> UpdateSecureFiles(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<SecureFile> secureFiles)
    {
      using (new MethodScope(requestContext, nameof (PlatformSecureFileService), nameof (UpdateSecureFiles)))
      {
        ArgumentUtility.CheckForNull<IList<SecureFile>>(secureFiles, nameof (secureFiles));
        List<SecureFile> secureFileList = new List<SecureFile>(secureFiles.Count);
        foreach (SecureFile secureFile1 in (IEnumerable<SecureFile>) secureFiles)
        {
          ArgumentValidation.CheckSecureFile(secureFile1, "secureFile");
          ArgumentUtility.CheckForEmptyGuid(secureFile1.Id, "Id");
          LibrarySecurityProvider securityProvider = this.m_securityProvider;
          IVssRequestContext requestContext1 = requestContext;
          Guid? projectId1 = new Guid?(projectId);
          Guid id = secureFile1.Id;
          string itemId = id.ToString();
          string errorMessage = TaskResources.SecureFileAccessDeniedForAdminOperation();
          securityProvider.CheckPermissions(requestContext1, projectId1, itemId, 2, false, errorMessage);
          Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
          SecureFile secureFile2 = secureFile1;
          IdentityRef identityRef = new IdentityRef();
          id = userIdentity.Id;
          identityRef.Id = id.ToString("D");
          secureFile2.ModifiedBy = identityRef;
          using (SecureFileComponent component = requestContext.CreateComponent<SecureFileComponent>())
          {
            SecureFile secureFile3 = component.UpdateSecureFile(projectId, secureFile1.Id, secureFile1);
            secureFileList.Add(secureFile3);
          }
        }
        foreach (SecureFile secureFile in secureFileList)
          requestContext.TraceSecureFileEvent(TraceLevel.Info, projectId, secureFile, "DB Row Updated. The File may have been renamed.");
        return (IList<SecureFile>) secureFileList;
      }
    }

    public IList<SecureFile> QuerySecureFilesByProperties(
      IVssRequestContext requestContext,
      Guid projectId,
      string condition,
      string secureFileNamePattern)
    {
      using (new MethodScope(requestContext, nameof (PlatformSecureFileService), nameof (QuerySecureFilesByProperties)))
      {
        IList<SecureFile> source = this.GetSecureFiles(requestContext, projectId, secureFileNamePattern, false, SecureFileActionFilter.None);
        if (source != null && source.Any<SecureFile>())
        {
          DummyTracer tracer = new DummyTracer();
          ExpressionParser expressionParser = new ExpressionParser();
          IFunctionInfo[] functions = new IFunctionInfo[1]
          {
            (IFunctionInfo) new FunctionInfo<PropertyFunctionNode>(PropertyFunctionNode.FunctionName, 1, 1)
          };
          IExpressionNode tree = expressionParser.CreateTree(condition, (ITraceWriter) tracer, (IEnumerable<INamedValueInfo>) null, (IEnumerable<IFunctionInfo>) functions);
          if (tree != null)
            source = (IList<SecureFile>) source.Where<SecureFile>((Func<SecureFile, bool>) (sf => tree.EvaluateBoolean((ITraceWriter) tracer, (ISecretMasker) null, (object) sf.Properties))).ToList<SecureFile>();
        }
        return source;
      }
    }

    private IList<SecureFile> FilterResults(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<SecureFile> secureFiles,
      bool includeDownloadTickets,
      SecureFileActionFilter actionFilter,
      bool removeUnreadableFiles = true)
    {
      List<SecureFile> secureFiles1 = new List<SecureFile>();
      if (secureFiles.Count > 0)
      {
        int requiredPermissions = 0;
        if ((actionFilter & SecureFileActionFilter.Use) == SecureFileActionFilter.Use)
          requiredPermissions |= 16;
        if ((actionFilter & SecureFileActionFilter.Manage) == SecureFileActionFilter.Manage)
          requiredPermissions |= 2;
        foreach (SecureFile secureFile in secureFiles.Select<SecureFile, SecureFile>((Func<SecureFile, SecureFile>) (x => this.FilterSecureFileByViewPermission(requestContext, projectId, x, removeUnreadableFiles))).Where<SecureFile>((Func<SecureFile, bool>) (x => x != null)))
        {
          if (requiredPermissions == 0 || this.m_securityProvider.HasPermissions(requestContext, new Guid?(projectId), secureFile.Id.ToString(), requiredPermissions))
            secureFiles1.Add(secureFile);
        }
        if (includeDownloadTickets)
          this.GenerateDownloadTickets(requestContext, projectId, secureFiles1);
      }
      return (IList<SecureFile>) secureFiles1;
    }

    private SecureFile FilterSecureFileByViewPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      SecureFile secureFile,
      bool removeUnreadableFiles = true)
    {
      ArgumentUtility.CheckForNull<SecureFile>(secureFile, nameof (secureFile));
      if (!this.m_securityProvider.HasPermissions(requestContext, new Guid?(projectId), secureFile.Id.ToString(), 1))
      {
        if (removeUnreadableFiles)
          return (SecureFile) null;
        this.m_securityProvider.CheckFrameworkReadPermissions(requestContext);
        return new SecureFile()
        {
          Id = secureFile.Id,
          Name = secureFile.Name
        };
      }
      this.FillIdentityDetails(requestContext, (IList<SecureFile>) new SecureFile[1]
      {
        secureFile
      });
      return secureFile;
    }

    private void FillIdentityDetails(
      IVssRequestContext requestContext,
      IList<SecureFile> secureFiles)
    {
      if (secureFiles == null || !secureFiles.Any<SecureFile>())
        return;
      IdentityService service = requestContext.GetService<IdentityService>();
      foreach (SecureFile secureFile in (IEnumerable<SecureFile>) secureFiles)
      {
        if (secureFile.CreatedBy != null && !string.IsNullOrWhiteSpace(secureFile.CreatedBy.Id))
          secureFile.CreatedBy = service.GetIdentity(requestContext, secureFile.CreatedBy.Id).ToIdentityRef(requestContext);
        if (secureFile.ModifiedBy != null && !string.IsNullOrWhiteSpace(secureFile.ModifiedBy.Id))
          secureFile.ModifiedBy = service.GetIdentity(requestContext, secureFile.ModifiedBy.Id).ToIdentityRef(requestContext);
      }
    }

    private void GenerateDownloadTickets(
      IVssRequestContext requestContext,
      Guid projectId,
      List<SecureFile> secureFiles)
    {
      if (secureFiles.Count <= 0 || (requestContext.IsSystemContext ? 1 : (ServicePrincipals.IsServicePrincipalThatCanReadSecureFiles(requestContext) ? 1 : 0)) == 0)
        return;
      this.m_secretsHelper.GenerateDownloadTickets(requestContext, projectId, (IEnumerable<SecureFile>) secureFiles);
    }

    public void DeleteTeamProject(IVssRequestContext requestContext, Guid projectId)
    {
      foreach (SecureFile secureFile in (IEnumerable<SecureFile>) this.GetSecureFiles(requestContext, projectId, (string) null, false, SecureFileActionFilter.None))
        new PlatformSecureFileService.SecureFileSecretsHelper().DeleteSecrets(requestContext, projectId, secureFile.Id);
      using (SecureFileComponent component = requestContext.CreateComponent<SecureFileComponent>())
        component.DeleteTeamProject(projectId);
    }

    private void SendSecureFileNotificationEvent(
      IVssRequestContext requestContext,
      Guid projectId,
      SecureFile secureFile)
    {
      this.SendSecureFileNotificationEvent(requestContext, projectId, (IEnumerable<SecureFile>) new SecureFile[1]
      {
        secureFile
      });
    }

    private void SendSecureFileNotificationEvent(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<SecureFile> secureFiles)
    {
      requestContext.GetService<IDistributedTaskEventPublisherService>().NotifySecureFilesEvent(requestContext, "MS.TF.DistributedTask.AuthorizePipelines", secureFiles, projectId);
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext)
    {
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return;
      requestContext.CheckProjectCollectionRequestContext();
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    private class SecureFileSecretsHelper : ISecureFileSecretsHelper
    {
      private const string c_secureFileSettingsPath = "/Service/DistributedTask/Settings/SecureFiles/";
      private const string c_downloadTicketExpirationKey = "/Service/DistributedTask/Settings/SecureFiles/DownloadTicketExpirationInMinutes";

      public void StoreSecrets(
        IVssRequestContext requestContext,
        Guid projectId,
        SecureFile secureFile,
        Stream content)
      {
        IVssRequestContext vssRequestContext = requestContext.Elevate();
        ITeamFoundationStrongBoxService service = vssRequestContext.GetService<ITeamFoundationStrongBoxService>();
        Guid drawerForSecureFile = this.GetDrawerForSecureFile(vssRequestContext, service, projectId, true);
        service.UploadSecureFile(vssRequestContext, drawerForSecureFile, secureFile.Id.ToString(), content);
      }

      public void DeleteSecrets(
        IVssRequestContext requestContext,
        Guid projectId,
        Guid secureFileId)
      {
        IVssRequestContext vssRequestContext = requestContext.Elevate();
        ITeamFoundationStrongBoxService service = vssRequestContext.GetService<ITeamFoundationStrongBoxService>();
        Guid drawerForSecureFile = this.GetDrawerForSecureFile(vssRequestContext, service, projectId);
        if (!(drawerForSecureFile != Guid.Empty))
          return;
        try
        {
          service.DeleteItem(vssRequestContext, drawerForSecureFile, secureFileId.ToString());
        }
        catch (StrongBoxItemNotFoundException ex)
        {
        }
      }

      public Stream ReadSecrets(
        IVssRequestContext requestContext,
        Guid projectId,
        Guid secureFileId)
      {
        IVssRequestContext vssRequestContext = requestContext.Elevate();
        ITeamFoundationStrongBoxService service = vssRequestContext.GetService<ITeamFoundationStrongBoxService>();
        Guid drawerForSecureFile = this.GetDrawerForSecureFile(vssRequestContext, service, projectId);
        return drawerForSecureFile != Guid.Empty ? service.RetrieveFile(vssRequestContext, drawerForSecureFile, secureFileId.ToString(), out long _) : (Stream) null;
      }

      public void GenerateDownloadTickets(
        IVssRequestContext requestContext,
        Guid projectId,
        IEnumerable<SecureFile> secureFiles)
      {
        int num = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/DistributedTask/Settings/SecureFiles/DownloadTicketExpirationInMinutes", 20);
        using (UrlSigner urlSigner = new UrlSigner(requestContext, DateTime.UtcNow.AddMinutes((double) num)))
          urlSigner.SignObject((ISignable) new PlatformSecureFileService.SecureFileSecretsHelper.SecureFileSignable(requestContext, projectId, secureFiles, this));
      }

      public bool ValidateTicket(
        IVssRequestContext requestContext,
        Guid projectId,
        Guid secureFileId,
        string downloadTicket,
        DateTime arrivalTime)
      {
        NameValueCollection queryString;
        try
        {
          queryString = HttpUtility.ParseQueryString(Encoding.UTF8.GetString(Convert.FromBase64String(downloadTicket)));
        }
        catch (Exception ex)
        {
          return false;
        }
        string a = queryString["type"];
        if (string.IsNullOrEmpty(a))
          a = "rsa";
        if (string.Equals(a, "rsa", StringComparison.OrdinalIgnoreCase))
        {
          TicketValidator ticketValidator = new TicketValidator();
          if (!ticketValidator.IsUnexpired(queryString, arrivalTime))
            throw new DownloadTicketValidationException(FrameworkResources.RequestExpired());
          IVssRequestContext vssRequestContext = requestContext.Elevate();
          using (ISigner signer = vssRequestContext.GetService<ITeamFoundationSigningService>().GetSigner(vssRequestContext, ProxyConstants.ProxySigningKey))
          {
            if (ticketValidator.IsValidRsaSignedTicket(signer, queryString))
            {
              if (this.GetSecureFileStrongBoxId(requestContext, projectId, secureFileId) == int.Parse(queryString["fid"]))
                return true;
            }
          }
        }
        return false;
      }

      private int GetSecureFileStrongBoxId(
        IVssRequestContext requestContext,
        Guid projectId,
        Guid secureFileId)
      {
        int secureFileStrongBoxId = 0;
        IVssRequestContext vssRequestContext = requestContext.Elevate();
        ITeamFoundationStrongBoxService service = vssRequestContext.GetService<ITeamFoundationStrongBoxService>();
        Guid drawerForSecureFile = this.GetDrawerForSecureFile(vssRequestContext, service, projectId);
        if (drawerForSecureFile != Guid.Empty)
        {
          StrongBoxItemInfo strongBoxItemInfo = (StrongBoxItemInfo) null;
          try
          {
            strongBoxItemInfo = service.GetItemInfo(vssRequestContext, drawerForSecureFile, secureFileId.ToString());
          }
          catch (StrongBoxItemNotFoundException ex)
          {
          }
          if (strongBoxItemInfo != null)
            secureFileStrongBoxId = strongBoxItemInfo.SecureFileId;
        }
        return secureFileStrongBoxId;
      }

      private Guid GetDrawerForSecureFile(
        IVssRequestContext elevatedRequestContext,
        ITeamFoundationStrongBoxService strongBoxService,
        Guid projectId,
        bool createDrawerIfNotExists = false)
      {
        string name = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Service/DistributedTask/{0}/SecureFile", (object) projectId);
        Guid drawerForSecureFile = strongBoxService.UnlockDrawer(elevatedRequestContext, name, false);
        if (!(drawerForSecureFile != Guid.Empty))
        {
          if (createDrawerIfNotExists)
          {
            try
            {
              return strongBoxService.CreateDrawer(elevatedRequestContext, name);
            }
            catch (StrongBoxDrawerExistsException ex)
            {
              return strongBoxService.UnlockDrawer(elevatedRequestContext, name, true);
            }
          }
        }
        return drawerForSecureFile;
      }

      private class SecureFileSignable : ISignable
      {
        private List<SecureFile> m_files;
        private IVssRequestContext m_requestContext;
        private Guid m_projectId;
        private PlatformSecureFileService.SecureFileSecretsHelper m_secretsHelper;

        public SecureFileSignable(
          IVssRequestContext requestContext,
          Guid projectId,
          IEnumerable<SecureFile> files,
          PlatformSecureFileService.SecureFileSecretsHelper secretsHelper)
        {
          this.m_requestContext = requestContext;
          this.m_projectId = projectId;
          this.m_files = new List<SecureFile>(files);
          this.m_secretsHelper = secretsHelper;
        }

        public int GetDownloadUrlCount() => this.m_files.Count;

        public int GetFileId(int index) => this.m_secretsHelper.GetSecureFileStrongBoxId(this.m_requestContext, this.m_projectId, this.m_files[index].Id);

        public byte[] GetHashValue(int index) => (byte[]) null;

        public void SetDownloadUrl(int index, string downloadUrl) => this.m_files[index].Ticket = Convert.ToBase64String(Encoding.UTF8.GetBytes(downloadUrl));
      }
    }
  }
}
