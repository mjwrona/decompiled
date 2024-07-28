// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.Authentication.CredentialProvider.PluginCredentialProvider
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Content.Common.Authentication.CredentialProvider
{
  internal class PluginCredentialProvider : ICredentialProvider
  {
    public string Description { get; }

    public string Path { get; }

    public PluginCredentialProvider(string path)
    {
      this.Path = path != null ? path : throw new ArgumentNullException(nameof (path));
      this.Description = System.IO.Path.GetFileName(path);
    }

    public Task<CredentialResponse> GetCredentialsAsync(
      Uri uri,
      bool isRetry,
      bool nonInteractive,
      TimeSpan timeout,
      CancellationToken cancellationToken)
    {
      CredentialResponse result;
      try
      {
        PluginCredentialProvider.PluginCredentialResponse credentialResponse = this.Execute(new PluginCredentialProvider.PluginCredentialRequest()
        {
          Uri = uri.ToString(),
          IsRetry = isRetry,
          NonInteractive = nonInteractive
        }, timeout, cancellationToken);
        result = !credentialResponse.IsValid ? new CredentialResponse(RequestStatus.ProviderNotApplicable) : new CredentialResponse((ICredentials) new NetworkCredential(credentialResponse.Username, credentialResponse.Password), RequestStatus.Success);
      }
      catch (CredentialProviderExeption ex)
      {
        throw;
      }
      catch (Exception ex) when (!(ex is CredentialProviderExeption))
      {
        throw new CredentialProviderExeption(this.Description + " - provider threw exception: " + ex.ToString(), ex);
      }
      return Task.FromResult<CredentialResponse>(result);
    }

    private PluginCredentialProvider.PluginCredentialResponse Execute(
      PluginCredentialProvider.PluginCredentialRequest request,
      TimeSpan timeout,
      CancellationToken cancellationToken)
    {
      StringBuilder stdOut = new StringBuilder();
      StringBuilder stdError = new StringBuilder();
      string str1 = "-uri \"" + request.Uri + "\"" + (request.IsRetry ? " -isRetry" : string.Empty) + (request.NonInteractive ? " -nonInteractive" : string.Empty);
      ProcessStartInfo startInfo = new ProcessStartInfo()
      {
        FileName = this.Path,
        Arguments = str1,
        WindowStyle = ProcessWindowStyle.Hidden,
        UseShellExecute = false,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        StandardOutputEncoding = StrictEncodingWithoutBOM.UTF8,
        StandardErrorEncoding = StrictEncodingWithoutBOM.UTF8,
        ErrorDialog = false
      };
      cancellationToken.ThrowIfCancellationRequested();
      Process process = Process.Start(startInfo);
      if (process == null)
        throw new CredentialProviderExeption("Could not spawn credential provider");
      process.OutputDataReceived += (DataReceivedEventHandler) ((o, e) => stdOut.AppendLine(e.Data));
      process.ErrorDataReceived += (DataReceivedEventHandler) ((o, e) => stdError.AppendLine(e.Data));
      process.BeginOutputReadLine();
      process.BeginErrorReadLine();
      using (cancellationToken.Register((Action) (() => PluginCredentialProvider.Kill(process))))
      {
        if (!process.WaitForExit((int) timeout.TotalMilliseconds))
        {
          PluginCredentialProvider.Kill(process);
          throw new TimeoutException(string.Format("Credential provider took longer {0} secs.", (object) timeout.TotalSeconds));
        }
        process.WaitForExit();
      }
      process.CancelErrorRead();
      process.CancelOutputRead();
      int exitCode = process.ExitCode;
      PluginCredentialProvider.PluginCredentialResponseExitCode responseExitCode = System.Enum.GetValues(typeof (PluginCredentialProvider.PluginCredentialResponseExitCode)).Cast<int>().Contains<int>(exitCode) ? (PluginCredentialProvider.PluginCredentialResponseExitCode) exitCode : throw new CredentialProviderExeption(string.Format("{0} - provider returned invalid exit code '{1}'.  ", (object) this.Description, (object) exitCode) + string.Format("Output: \n{0} \n{1}", (object) stdOut, (object) stdError));
      string str2 = stdOut.ToString();
      PluginCredentialProvider.PluginCredentialResponse credentialResponse;
      try
      {
        credentialResponse = JsonConvert.DeserializeObject<PluginCredentialProvider.PluginCredentialResponse>(str2);
      }
      catch (Exception ex)
      {
        throw new CredentialProviderExeption("${Name} - Deserialize provider response", ex);
      }
      switch (responseExitCode)
      {
        case PluginCredentialProvider.PluginCredentialResponseExitCode.Success:
          return credentialResponse.IsValid ? credentialResponse : throw new CredentialProviderExeption(this.Description + " - returned invalid output: " + str2);
        case PluginCredentialProvider.PluginCredentialResponseExitCode.ProviderNotApplicable:
          credentialResponse.Username = (string) null;
          credentialResponse.Password = (string) null;
          return credentialResponse;
        case PluginCredentialProvider.PluginCredentialResponseExitCode.Failure:
          throw new CredentialProviderExeption(this.Description + " - provider returned failure: " + (credentialResponse != null ? credentialResponse.Message ?? "<empty>" : "<no error details>"));
      }
    }

    private static void Kill(Process p)
    {
      if (p.HasExited)
        return;
      try
      {
        p.Kill();
      }
      catch (InvalidOperationException ex)
      {
      }
    }

    private class PluginCredentialRequest
    {
      public string Uri { get; set; }

      public bool NonInteractive { get; set; }

      public bool IsRetry { get; set; }
    }

    private class PluginCredentialResponse
    {
      public string Username { get; set; }

      public string Password { get; set; }

      public string Message { get; set; }

      public bool IsValid => !string.IsNullOrWhiteSpace(this.Username) || !string.IsNullOrWhiteSpace(this.Password);
    }

    private enum PluginCredentialResponseExitCode
    {
      Success,
      ProviderNotApplicable,
      Failure,
    }
  }
}
