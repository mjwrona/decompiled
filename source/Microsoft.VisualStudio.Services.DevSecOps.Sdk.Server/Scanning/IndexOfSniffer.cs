// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.Scanning.IndexOfSniffer
// Assembly: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AEA81E2B-AB47-44C0-8043-66C0E1018997
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.dll

using System;
using System.IO;
using System.Text;

namespace Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.Scanning
{
  public class IndexOfSniffer : ISniffer
  {
    private const string AdoPatRegex = "[2-7a-z]{52}";
    private const int AdoPatLength = 52;
    private const int Two = 50;
    private const int Seven = 55;
    private const int A = 97;
    private const int Z = 122;
    internal readonly string[] DefaultSignatures = new string[147]
    {
      "Password",
      "password",
      "PASSWORD",
      "Pwd",
      "pwd",
      "PWD",
      "Secret",
      "secret",
      "SECRET",
      "SqlSrv",
      "sqlsrv",
      "SQLSRV",
      ".redis.cache",
      ".documents.azure",
      "AccessKey",
      "AccountKey",
      "xstore",
      ".core.windows",
      "true#",
      "false#",
      "--account-key",
      ".azurewebsites",
      ".azure-devices",
      ".signalr",
      ".communication.azure",
      "access_token",
      "+ASt",
      "AzCa",
      "ACDb",
      "AzSe",
      "/AM7",
      "+ASb",
      "+AEh",
      "xox",
      "ghp_",
      "AKIA",
      "LTAI",
      "oy2",
      "shpat_",
      "shpss_",
      "_auth",
      "npm",
      "PMAK-",
      "npm_",
      ".azure",
      "dapi",
      ".eventgrid",
      ".office.com/webhook",
      "sig=",
      "7Q~",
      "8Q~",
      "AzFu",
      "+ABa",
      "+AMC",
      "+ARm",
      "+ACR",
      "+SQL",
      "AIoT",
      "APIM",
      "amzn1",
      "YXdz",
      "b3JpZ2lu",
      "GOOG",
      "aio_",
      "Asana",
      "asana",
      "Bitbucket",
      "bitbucket",
      "CLOJARS_",
      "dvc_",
      "doo_v",
      "dop_v",
      "dor_v",
      "dos_v",
      "Discord",
      "discord",
      "dp.audit.",
      "dp.ct.",
      "dp.pt.",
      "dp.scim.",
      "dp.st.",
      "sl.",
      "_live",
      "_test_",
      "EZAK",
      "EZTK",
      "fig",
      "FLWSECK",
      "FullStory",
      "fullstory",
      "v1.",
      "gho_",
      "ghr_",
      "ghs_",
      "gh1_",
      "github",
      "ghu_",
      "eyJrIjoi",
      "pat-",
      "dG9rO",
      "ion_",
      "AK",
      "lin_api_",
      "lin_oauth_",
      "lmb_",
      "lma_",
      "Mid-server-",
      "NRIQ-",
      "NRAK-",
      "NRRA-",
      "api_sandbox",
      "sk-",
      "pscale_",
      "pcs_",
      "pcu_",
      "PSK",
      "rdme_xn8s9h",
      "rpa_",
      "samsara_api_",
      "gzUdQrDW:",
      "secret_scanning_ab85fc6f8d7638cf1c11da812da308d43_",
      "SG.",
      "sib-",
      "xapp-",
      "AKID",
      "IKID",
      "tfp_",
      "WISEFlow",
      "wiseflow",
      "sk_",
      "Yandex",
      "yandex",
      "zpka_",
      "ChiefTools",
      "chieftools",
      "hvs.",
      "b.AA",
      "HashiCorp",
      "hashicorp",
      "CiQ",
      "CiR",
      "JFrog",
      "jfrog",
      "waka_",
      "gcntfy-",
      "persona_production_",
      "[2-7a-z]{52}"
    };

    public string[] Signatures { get; }

    public IndexOfSniffer(string[] signatures = null)
    {
      if (signatures != null)
        this.Signatures = signatures;
      else
        this.Signatures = this.DefaultSignatures;
    }

    public bool IsMatch(Stream contentStream)
    {
      if (this.Signatures.Length == 0)
        return true;
      if (contentStream == null)
        return false;
      using (StreamReader streamReader = new StreamReader(contentStream, Encoding.UTF8, true, 1024, true))
      {
        string end = streamReader.ReadToEnd();
        if (end.Length == 0)
          return false;
        foreach (string signature in this.Signatures)
        {
          if (!(signature == "[2-7a-z]{52}") ? this.IsSignatureMatch(signature, end) : this.IsAdoPatMatch(end))
            return true;
        }
      }
      return false;
    }

    internal bool IsSignatureMatch(string signature, string content) => content.IndexOf(signature, StringComparison.Ordinal) >= 0;

    internal bool IsAdoPatMatch(string content) => this.AdoPatIndexOf(content) >= 0;

    private int AdoPatIndexOf(string content, int startIndex = 0)
    {
      int num = 0;
      int length = content.Length;
      for (int index = startIndex; index < length; ++index)
      {
        char ch = content[index];
        if ('2' <= ch && ch <= '7' || 'a' <= ch && ch <= 'z')
        {
          ++num;
          if (num >= 52)
            return index + 1 - 52;
        }
        else
          num = 0;
      }
      return -1;
    }
  }
}
