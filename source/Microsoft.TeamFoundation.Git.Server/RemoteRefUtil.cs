// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.RemoteRefUtil
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class RemoteRefUtil
  {
    public static List<NonLibGit2Reference> GetRefs(
      ITraceRequest tracer,
      Uri gitUri,
      string username,
      string password,
      ClientTraceData ctData)
    {
      if (gitUri == (Uri) null)
        throw new ArgumentNullException(nameof (gitUri));
      foreach (string possibleInfoRefsUri in RemoteRefUtil.ComposePossibleInfoRefsUris(gitUri.ToString()))
      {
        try
        {
          HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, possibleInfoRefsUri);
          request.Headers.Add("User-Agent", "git/vsts-1.0");
          if (!string.IsNullOrEmpty(username) || !string.IsNullOrEmpty(password))
            request.Headers.Add("Authorization", GitServerUtils.GetAuthorizationHeader(username, password));
          using (HttpClient httpClient = new HttpClient())
          {
            HttpResponseMessage result1 = httpClient.SendAsync(request).Result;
            if (result1.Headers.Contains("X-GitHub-OTP"))
              ctData.Add("GitHubSourceBehind2FactorAuth", (object) true);
            string result2 = result1.Content.ReadAsStringAsync().Result;
            if (result1.StatusCode == HttpStatusCode.OK)
            {
              string[] separator = new string[1]{ "\n" };
              List<NonLibGit2Reference> refs = RemoteRefUtil.ConvertToRefs(((IEnumerable<string>) result2.Split(separator, StringSplitOptions.None)).ToList<string>());
              if (refs.Any<NonLibGit2Reference>())
                return refs;
              tracer.Trace(1013696, TraceLevel.Info, GitServerUtils.TraceArea, GitServerUtils.GitImportLayer, "GetRefs Parser failed for Uri ({0}).", (object) possibleInfoRefsUri);
            }
            else
              tracer.Trace(1013696, TraceLevel.Info, GitServerUtils.TraceArea, GitServerUtils.GitImportLayer, "GetRefs failed for Uri ({0}) with reason: {1}.", (object) possibleInfoRefsUri, (object) result1.ReasonPhrase);
          }
        }
        catch (Exception ex)
        {
          tracer.Trace(1013696, TraceLevel.Info, GitServerUtils.TraceArea, GitServerUtils.GitImportLayer, "GetRefs threw for Uri ({0}) with exception {1}.", (object) possibleInfoRefsUri, (object) ex);
        }
      }
      return new List<NonLibGit2Reference>();
    }

    private static List<string> ComposePossibleInfoRefsUris(string sourceUrl)
    {
      List<string> stringList = new List<string>();
      sourceUrl = sourceUrl.TrimEnd('/');
      if (sourceUrl.EndsWith(".git"))
        sourceUrl = sourceUrl.Substring(0, sourceUrl.Length - ".git".Length);
      stringList.Add(sourceUrl + "/info/refs?service=git-upload-pack");
      stringList.Add(sourceUrl + ".git/info/refs?service=git-upload-pack");
      return stringList;
    }

    private static List<NonLibGit2Reference> ConvertToRefs(List<string> refsList)
    {
      List<NonLibGit2Reference> refs1 = new List<NonLibGit2Reference>();
      foreach (string refs2 in refsList)
      {
        string[] source = refs2.Split(Array.Empty<char>(), StringSplitOptions.RemoveEmptyEntries);
        if (((IEnumerable<string>) source).Count<string>() == 2 && source[1].Contains("refs"))
        {
          string targetIdentifier = source[0].Substring(4, source[0].Length - 4);
          string canonicalName = source[1];
          refs1.Add(new NonLibGit2Reference(targetIdentifier, canonicalName));
        }
      }
      return refs1;
    }
  }
}
