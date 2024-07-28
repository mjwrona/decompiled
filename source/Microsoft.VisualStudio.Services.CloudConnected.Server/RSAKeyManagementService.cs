// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CloudConnected.RSAKeyManagementService
// Assembly: Microsoft.VisualStudio.Services.CloudConnected.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6AAFC756-39E6-4247-9102-7DC33B035E4F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CloudConnected.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Newtonsoft.Json.Linq;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.VisualStudio.Services.CloudConnected
{
  public class RSAKeyManagementService : IRSAKeyManagementService, IVssFrameworkService
  {
    private const string c_connectedServiceDrawerName = "/Service/Gallery/ConnectedService";
    private static readonly RegistryQuery s_keySizeQuery = new RegistryQuery("/Service/Commerce/ConnectedServer/KeySize");
    private int m_keySize = 2048;
    private byte[] m_protectedRsaJson;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (systemRequestContext.ServiceHost.IsProduction)
        systemRequestContext.CheckOnPremisesDeployment();
      this.LoadSettings(systemRequestContext);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public RSACryptoServiceProvider GetKey()
    {
      RSAParameters rsaParameters = RSAKeyManagementService.ConvertJsonStringToRsaParameters(Encoding.UTF8.GetString(ProtectedData.Unprotect(this.m_protectedRsaJson, (byte[]) null, DataProtectionScope.CurrentUser)));
      RSACryptoServiceProvider key = new RSACryptoServiceProvider(this.m_keySize);
      key.PersistKeyInCsp = false;
      key.ImportParameters(rsaParameters);
      return key;
    }

    private static RSAParameters ConvertJsonStringToRsaParameters(string strongBoxValue)
    {
      JObject jobject = JObject.Parse(strongBoxValue);
      return new RSAParameters()
      {
        D = jobject["D"].ToObject<byte[]>(),
        DP = jobject["DP"].ToObject<byte[]>(),
        DQ = jobject["DQ"].ToObject<byte[]>(),
        Exponent = jobject["Exponent"].ToObject<byte[]>(),
        InverseQ = jobject["InverseQ"].ToObject<byte[]>(),
        Modulus = jobject["Modulus"].ToObject<byte[]>(),
        P = jobject["P"].ToObject<byte[]>(),
        Q = jobject["Q"].ToObject<byte[]>()
      };
    }

    private static string ConvertRsaToJsonString(RSACryptoServiceProvider rsa)
    {
      RSAParameters rsaParameters = rsa.ExportParameters(true);
      return new JObject()
      {
        ["D"] = ((JToken) rsaParameters.D),
        ["DP"] = ((JToken) rsaParameters.DP),
        ["DQ"] = ((JToken) rsaParameters.DQ),
        ["Exponent"] = ((JToken) rsaParameters.Exponent),
        ["InverseQ"] = ((JToken) rsaParameters.InverseQ),
        ["Modulus"] = ((JToken) rsaParameters.Modulus),
        ["P"] = ((JToken) rsaParameters.P),
        ["Q"] = ((JToken) rsaParameters.Q)
      }.ToString();
    }

    public void DeleteKey(IVssRequestContext requestContext)
    {
      ITeamFoundationStrongBoxService service = requestContext.GetService<ITeamFoundationStrongBoxService>();
      string name = "/Service/Gallery/ConnectedService";
      Guid drawerId = service.UnlockDrawer(requestContext, name, false);
      if (drawerId.Equals(Guid.Empty))
        return;
      string lookupKey = requestContext.ServiceHost.InstanceId.ToString();
      service.DeleteItem(requestContext, drawerId, lookupKey);
      this.LoadSettings(requestContext);
    }

    private void LoadSettings(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
      int num = vssRequestContext.GetService<IVssRegistryService>().GetValue<int>(vssRequestContext, in RSAKeyManagementService.s_keySizeQuery, 2048);
      if (num < 2048 || num > 16384 || num % 8 != 0)
        num = 2048;
      this.m_keySize = num;
      requestContext = requestContext.Elevate();
      ITeamFoundationStrongBoxService service = requestContext.GetService<ITeamFoundationStrongBoxService>();
      string name = "/Service/Gallery/ConnectedService";
      Guid drawerId = service.UnlockDrawer(requestContext, name, false);
      if (drawerId.Equals(Guid.Empty))
      {
        try
        {
          drawerId = service.CreateDrawer(requestContext, name);
        }
        catch (StrongBoxDrawerExistsException ex)
        {
          drawerId = service.UnlockDrawer(requestContext, name, false);
        }
      }
      string str = requestContext.ServiceHost.InstanceId.ToString();
      string jsonString;
      if (!this.TryGetStrongBoxValue(requestContext, service, drawerId, str, out jsonString))
      {
        jsonString = RSAKeyManagementService.ConvertRsaToJsonString(new RSACryptoServiceProvider(this.m_keySize)
        {
          PersistKeyInCsp = false
        });
        service.AddString(requestContext, drawerId, str, jsonString);
      }
      this.m_protectedRsaJson = ProtectedData.Protect(Encoding.UTF8.GetBytes(jsonString), (byte[]) null, DataProtectionScope.CurrentUser);
    }

    private bool TryGetStrongBoxValue(
      IVssRequestContext requestContext,
      ITeamFoundationStrongBoxService strongBoxService,
      Guid drawerId,
      string key,
      out string value)
    {
      try
      {
        value = strongBoxService.GetString(requestContext, drawerId, key);
        return true;
      }
      catch (StrongBoxItemNotFoundException ex)
      {
        value = (string) null;
        return false;
      }
    }
  }
}
