// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.AddDomainDialogDataSource
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client.Channels;
using Microsoft.TeamFoundation.Client.Internal;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Net;

namespace Microsoft.TeamFoundation.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class AddDomainDialogDataSource : BaseDataSource
  {
    private string m_serverName;
    private string m_port;
    private string m_path;
    private bool m_isHttps;
    private Action<TfsConnection, Exception> m_connectionCompleted;
    private BaseDialog m_dialog;
    private const int c_MaxPortNumber = 65535;
    private Dictionary<string, TimeSpan> m_cachedTimeouts = new Dictionary<string, TimeSpan>();
    private BackgroundWorker m_authenticateWorker;

    public AddDomainDialogDataSource(BaseDialog dialog)
    {
      this.m_dialog = dialog;
      this.m_serverName = string.Empty;
      this.m_port = 8080.ToString((IFormatProvider) CultureInfo.CurrentCulture);
      this.m_path = "tfs";
      this.m_isHttps = false;
    }

    public void StartConnect(
      Action<TfsConnection, Exception> connectionCompleted)
    {
      this.StopConnect();
      this.SetTimeouts();
      this.m_connectionCompleted = connectionCompleted;
      List<TfsConnection> tfsConnectionList = new List<TfsConnection>();
      Uri uri1 = this.CreateUri();
      TfsConfigurationServer configurationServer1 = TfsConfigurationServerFactory.GetConfigurationServer(uri1);
      TfsTeamProjectCollection projectCollection1 = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(uri1);
      if (configurationServer1.ClientCredentials.PromptType != CredentialPromptType.PromptIfNeeded)
        configurationServer1.ClientCredentials.PromptType = CredentialPromptType.PromptIfNeeded;
      tfsConnectionList.Add((TfsConnection) configurationServer1);
      tfsConnectionList.Add((TfsConnection) projectCollection1);
      if ((!(!this.IsUrlEntered & string.Equals(this.Path, "tfs", StringComparison.OrdinalIgnoreCase)) ? 0 : (!string.IsNullOrEmpty("tfs") ? 1 : 0)) != 0)
      {
        Uri uri2 = this.CreateUri(false);
        TfsConfigurationServer configurationServer2 = TfsConfigurationServerFactory.GetConfigurationServer(uri2);
        TfsTeamProjectCollection projectCollection2 = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(uri2);
        if (configurationServer2.ClientCredentials.PromptType != CredentialPromptType.PromptIfNeeded)
          configurationServer2.ClientCredentials.PromptType = CredentialPromptType.PromptIfNeeded;
        tfsConnectionList.Add((TfsConnection) configurationServer2);
        tfsConnectionList.Add((TfsConnection) projectCollection2);
      }
      this.m_authenticateWorker = new BackgroundWorker();
      this.m_authenticateWorker.WorkerSupportsCancellation = true;
      this.m_authenticateWorker.DoWork += new DoWorkEventHandler(this.m_authenticateWorker_DoWork);
      this.m_authenticateWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.m_authenticateWorker_RunWorkerCompleted);
      this.m_authenticateWorker.RunWorkerAsync((object) tfsConnectionList);
    }

    public void StopConnect()
    {
      if (this.m_authenticateWorker != null && this.m_authenticateWorker.IsBusy)
        this.m_authenticateWorker.CancelAsync();
      this.RestoreTimeouts();
    }

    public string ServerName
    {
      get => this.m_serverName.Trim();
      set
      {
        if (VssStringComparer.Url.Equals(this.m_serverName, value))
          return;
        this.m_serverName = value;
        this.OnPropertyChanged(nameof (ServerName));
        this.OnPropertyChanged("IsUrlEntered");
        this.OnPropertyChanged("Preview");
        this.OnPropertyChanged("IsInputValid");
        if (this.IsUrlEntered)
          return;
        this.OnPropertyChanged("IsHostedUrl");
      }
    }

    public string Port
    {
      get => this.m_port;
      set
      {
        if (VssStringComparer.Url.Equals(this.m_port, value))
          return;
        this.m_port = value;
        this.OnPropertyChanged(nameof (Port));
        this.OnPropertyChanged("Preview");
        this.OnPropertyChanged("IsInputValid");
      }
    }

    public string Path
    {
      get => this.m_path;
      set
      {
        if (VssStringComparer.Url.Equals(this.m_path, value))
          return;
        this.m_path = value;
        this.OnPropertyChanged(nameof (Path));
        this.OnPropertyChanged("Preview");
        this.OnPropertyChanged("IsInputValid");
      }
    }

    public bool IsHttps
    {
      get => this.m_isHttps;
      set
      {
        if (this.m_isHttps == value)
          return;
        this.m_isHttps = value;
        int result;
        if (int.TryParse(this.Port.Trim(), out result) && result == (this.m_isHttps ? 8080 : 443))
        {
          result = this.m_isHttps ? 443 : 8080;
          this.m_port = result.ToString((IFormatProvider) CultureInfo.CurrentCulture);
          this.OnPropertyChanged("Port");
        }
        this.OnPropertyChanged(nameof (IsHttps));
        this.OnPropertyChanged("Preview");
      }
    }

    public bool IsUrlEntered => this.ServerName.StartsWith(Uri.UriSchemeHttp + Uri.SchemeDelimiter, StringComparison.OrdinalIgnoreCase) || this.ServerName.StartsWith(Uri.UriSchemeHttps + Uri.SchemeDelimiter, StringComparison.OrdinalIgnoreCase);

    public bool IsHostedUrl => TFUtil.IsHostedServer(this.ServerName);

    public string Preview
    {
      get
      {
        string preview;
        this.IsValid(out preview);
        return preview;
      }
    }

    public bool IsInputValid => this.IsValid(out string _);

    private void m_authenticateWorker_DoWork(object sender, DoWorkEventArgs e)
    {
      List<TfsConnection> tfsConnectionList = (List<TfsConnection>) e.Argument;
      AddDomainDialogDataSource.AuthenticateResult[] authenticateResultArray = new AddDomainDialogDataSource.AuthenticateResult[tfsConnectionList.Count];
      for (int index = 0; index < tfsConnectionList.Count; ++index)
      {
        if (e.Cancel)
          return;
        TfsConnection tfsConnection = tfsConnectionList[index];
        AddDomainDialogDataSource.AuthenticateResult result = (AddDomainDialogDataSource.AuthenticateResult) null;
        try
        {
          tfsConnection.EnsureAuthenticated();
          if (tfsConnection.CatalogNode != null)
          {
            if (tfsConnection is TfsTeamProjectCollection projectCollection)
            {
              try
              {
                TfsConfigurationServer configurationServer = projectCollection.ConfigurationServer;
                configurationServer.EnsureAuthenticated();
                CatalogNode catalogNode = configurationServer.CatalogNode;
                result = new AddDomainDialogDataSource.AuthenticateResult((TfsConnection) configurationServer, tfsConnection, true);
              }
              catch (Exception ex)
              {
                TeamFoundationTrace.TraceException(ex);
              }
            }
          }
          if (result == null)
            result = new AddDomainDialogDataSource.AuthenticateResult(tfsConnection, tfsConnection, false);
        }
        catch (TeamFoundationServerUnauthorizedException ex)
        {
          e.Result = (object) new AddDomainDialogDataSource.AuthenticateResult(tfsConnection, tfsConnection, false, (Exception) ex);
          break;
        }
        catch (Exception ex)
        {
          result = new AddDomainDialogDataSource.AuthenticateResult(tfsConnection, tfsConnection, false, ex);
        }
        authenticateResultArray[index] = result;
        if (this.ProcessResult(result))
        {
          e.Result = (object) result;
          break;
        }
      }
      if (e.Result != null)
        return;
      e.Result = (object) authenticateResultArray[0];
    }

    private void m_authenticateWorker_RunWorkerCompleted(
      object sender,
      RunWorkerCompletedEventArgs e)
    {
      this.RestoreTimeouts();
      if (e.Error != null)
      {
        int num = (int) UIHost.ShowException(e.Error);
      }
      else
      {
        if (e.Cancelled)
          return;
        this.HandleConnectionCompleted((AddDomainDialogDataSource.AuthenticateResult) e.Result);
      }
    }

    private void SetTimeouts()
    {
      this.SetTimeout("Framework");
      this.SetTimeout("RegistrationService");
    }

    private void SetTimeout(string componentName)
    {
      TfsRequestSettings settings = TfsRequestSettings.GetSettings(componentName);
      this.m_cachedTimeouts[componentName] = settings.SendTimeout;
      settings.SendTimeout = TimeSpan.FromSeconds(30.0);
    }

    private void RestoreTimeouts()
    {
      this.RestoreTimeout("Framework");
      this.RestoreTimeout("RegistrationService");
    }

    private void RestoreTimeout(string componentName)
    {
      if (!this.m_cachedTimeouts.ContainsKey(componentName))
        return;
      TfsRequestSettings.GetSettings(componentName).SendTimeout = this.m_cachedTimeouts[componentName];
      this.m_cachedTimeouts.Remove(componentName);
    }

    private void HandleConnectionCompleted(
      AddDomainDialogDataSource.AuthenticateResult result)
    {
      this.StopConnect();
      this.OnConnectionCompleted(result.Server, result.Error);
    }

    private void OnConnectionCompleted(TfsConnection server, Exception error)
    {
      this.m_connectionCompleted(server, error);
      this.m_connectionCompleted = (Action<TfsConnection, Exception>) null;
    }

    private bool ProcessResult(
      AddDomainDialogDataSource.AuthenticateResult result)
    {
      TfsConnection server = result.Server;
      if (result.IsAuthenticated)
      {
        if (result.UsedIntanceInsteadOfCollection)
        {
          int num = (int) UIHost.ShowInformation(ClientResources.AddDomainDialogAddingCollection(), (string) null, (string) null);
        }
        return true;
      }
      if (result.Error is TeamFoundationServerNotSupportedException)
        return true;
      if (result.Error != null)
      {
        if (!(result.Error is WebException webException))
          webException = result.Error.InnerException as WebException;
        if (webException != null && (webException.Status == WebExceptionStatus.NameResolutionFailure || webException.Status == WebExceptionStatus.Timeout || webException.Response is HttpWebResponse response && (response.StatusCode == HttpStatusCode.GatewayTimeout || response.StatusCode == HttpStatusCode.RequestTimeout)))
          return true;
      }
      return false;
    }

    private Uri CreateUri() => this.CreateUri(true);

    private Uri CreateUri(bool withPath)
    {
      try
      {
        Uri result1 = (Uri) null;
        if (!this.IsUrlEntered && !this.IsHostedUrl)
        {
          int result2;
          if (!int.TryParse(this.Port.Trim(), out result2))
            return (Uri) null;
          string scheme = this.IsHttps ? Uri.UriSchemeHttps : Uri.UriSchemeHttp;
          string str = withPath ? this.Path.Trim() : string.Empty;
          string serverName = this.ServerName;
          int port = result2;
          string pathValue = str;
          result1 = new UriBuilder(scheme, serverName, port, pathValue).Uri;
        }
        else if (this.IsUrlEntered)
          Uri.TryCreate(this.ServerName, UriKind.Absolute, out result1);
        else
          result1 = new UriBuilder(Uri.UriSchemeHttps, this.ServerName).Uri;
        if (result1 != (Uri) null)
          result1 = UriUtility.NormalizePathSeparators(result1);
        return result1;
      }
      catch
      {
        return (Uri) null;
      }
    }

    private bool IsPortNumberValid()
    {
      try
      {
        string s = this.Port.Trim();
        if (string.IsNullOrEmpty(s))
          return false;
        foreach (char c in s)
        {
          if (!char.IsDigit(c))
            return false;
        }
        int result;
        return int.TryParse(s, out result) && result > 0 && result <= (int) ushort.MaxValue;
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException(ex);
        return false;
      }
    }

    private bool IsValid(out string preview)
    {
      preview = string.Empty;
      if (string.IsNullOrEmpty(this.ServerName))
      {
        preview = ClientResources.ServerNameEmpty();
        return false;
      }
      if (!this.IsUrlEntered && !this.IsPortNumberValid())
      {
        preview = ClientResources.AddDomainDialog_InvalidPort();
        return false;
      }
      Uri uri = this.CreateUri();
      if (uri == (Uri) null)
      {
        preview = ClientResources.AddDomainDialog_InvalidServer();
        return false;
      }
      preview = uri.ToString();
      return true;
    }

    public static class PropertyName
    {
      public const string ServerName = "ServerName";
      public const string Port = "Port";
      public const string Path = "Path";
      public const string IsHttps = "IsHttps";
      public const string IsUrlEntered = "IsUrlEntered";
      public const string IsHostedUrl = "IsHostedUrl";
      public const string Preview = "Preview";
      public const string IsInputValid = "IsInputValid";
    }

    private class AuthenticateResult
    {
      public AuthenticateResult(
        TfsConnection server,
        TfsConnection initialServer,
        bool usedIntanceInsteadOfCollection,
        Exception error = null)
      {
        this.Error = error;
        this.Server = server;
        this.InitialServer = initialServer;
        this.UsedIntanceInsteadOfCollection = usedIntanceInsteadOfCollection;
      }

      public bool UsedIntanceInsteadOfCollection { get; private set; }

      public TfsConnection Server { get; private set; }

      public TfsConnection InitialServer { get; private set; }

      public Exception Error { get; private set; }

      public bool IsAuthenticated => this.Error == null;

      public bool IsInvalidCredentials => this.Error != null && ConnectFailureReason.GetReason(this.Error).Category == ConnectFailureCategory.NotPermitted;

      public string GetErrorMessage() => this.Error == null ? string.Empty : AddDomainDialogDataSource.AddDomainConnectFailureReason.GetReason(this.Error).GetErrorMessage(this.Server.Uri.AbsoluteUri);
    }

    private class AddDomainConnectFailureReason : ConnectFailureReason
    {
      public AddDomainConnectFailureReason(ConnectFailureStatus code, Exception ex)
        : base(code, ex)
      {
      }

      public override string GetErrorMessage(string serverName) => this.Category == ConnectFailureCategory.NotPermitted && this.StatusCode == ConnectFailureStatus.NotAuthorized ? ClientResources.ConnectToTfs_AccessCheck((object) serverName, (object) this._exception.Message) : base.GetErrorMessage(serverName);

      public static AddDomainDialogDataSource.AddDomainConnectFailureReason GetReason(Exception e) => new AddDomainDialogDataSource.AddDomainConnectFailureReason(ConnectFailureReason.GetStatus(e), e);
    }
  }
}
