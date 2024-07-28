// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.AccountManagement.AccountManagementUtilities
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Client.AccountManagement
{
  internal static class AccountManagementUtilities
  {
    internal const int DefaultCacheVersion = 0;
    internal const string DefaultRegistryRoot = "Software\\Microsoft\\VSCommon";
    internal const string EndpointRegistryOverrideName = "accounturi";
    internal const string VSOEndpointRegistryOverrideName = "uri";
    internal const string ExtraParametersRegistryOverrideName = "ExtraParametersRegistryOverride";
    public static readonly Task<bool> TrueTask = Task.FromResult<bool>(true);
    public static readonly Task<bool> FalseTask = Task.FromResult<bool>(false);

    internal static bool RemoveMidQueue<T>(this Queue<T> queue, T valueToRemove) where T : class
    {
      ArgumentUtility.CheckForNull<Queue<T>>(queue, nameof (queue));
      ArgumentUtility.CheckForNull<T>(valueToRemove, nameof (valueToRemove));
      int count = queue.Count;
      int num = 0;
      bool flag = false;
      while (num < count)
      {
        ++num;
        T obj = queue.Dequeue();
        if (!flag && (object) obj == (object) valueToRemove)
          flag = true;
        else
          queue.Enqueue(obj);
      }
      return flag;
    }

    internal static Uri CreateAzureRMUri(Uri baseUri, string apiVersion, Uri relativeUri) => new UriBuilder(new Uri(baseUri, relativeUri))
    {
      Query = ("api-version=" + apiVersion)
    }.Uri;

    internal static void ExecuteActionInGlobalMutex(string globalMutexName, Action action) => AccountManagementUtilities.ExecuteInGlobalMutex<object>(globalMutexName, (Func<object>) (() =>
    {
      action();
      return (object) null;
    }));

    internal static string SerializeDictionaryAccountProperty(IDictionary<string, string> map) => JsonConvert.SerializeObject((object) map, (JsonConverter) new KeyValuePairConverter());

    internal static int IncrementRegistryValue(RegistryKey rootKey, string registrySentinelValue)
    {
      int num1 = (int) rootKey.GetValue(registrySentinelValue, (object) 0);
      if (num1 == int.MaxValue)
        num1 = 0;
      int num2 = num1 + 1;
      rootKey.SetValue(registrySentinelValue, (object) num2);
      return num2;
    }

    internal static Uri CheckUri(string uriName)
    {
      Uri result;
      return !string.IsNullOrEmpty(uriName) && Uri.TryCreate(uriName, UriKind.Absolute, out result) ? result : (Uri) null;
    }

    internal static bool IsFwlink(Uri url) => (string.Equals(url.Scheme, "https", StringComparison.OrdinalIgnoreCase) || string.Equals(url.Scheme, "http", StringComparison.OrdinalIgnoreCase)) && string.Equals(url.Host, "go.microsoft.com", StringComparison.OrdinalIgnoreCase);

    internal static TResult ExecuteInGlobalMutex<TResult>(
      string globalMutexName,
      Func<TResult> func)
    {
      Mutex mutex = new Mutex(false, globalMutexName);
      bool flag = false;
      try
      {
        try
        {
          flag = mutex.WaitOne();
        }
        catch (AbandonedMutexException ex)
        {
          flag = true;
        }
        return func();
      }
      catch (Exception ex)
      {
        throw new AccountStorageException(ClientResources.AccountManagerProblemExecutingInGlobalMutex((object) globalMutexName), ex);
      }
      finally
      {
        if (flag)
          mutex.ReleaseMutex();
      }
    }

    internal static Uri ResolveFWLinkIfRequired(Uri vsoEndPoint)
    {
      if (AccountManagementUtilities.IsFwlink(vsoEndPoint))
      {
        try
        {
          HttpWebRequest httpWebRequest = (HttpWebRequest) WebRequest.Create(vsoEndPoint);
          httpWebRequest.AllowAutoRedirect = false;
          using (HttpWebResponse response = (HttpWebResponse) httpWebRequest.GetResponse())
          {
            Trace.WriteLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ResolveFWLinkAccountManager statusCode is redirect: '{0}'", (object) (response.StatusCode == HttpStatusCode.Found)));
            Uri uri = new Uri(response.Headers["Location"]);
            Trace.WriteLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ResolveFWLinkAccountManager resovled fwlink from '{0}' to '{1}'", (object) vsoEndPoint, (object) uri));
            return uri;
          }
        }
        catch (Exception ex)
        {
          Trace.WriteLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "There was a problem resolving the FW link '{0}', {1}", (object) vsoEndPoint, (object) ex));
        }
      }
      return vsoEndPoint;
    }

    internal static bool EqualsOrdinalIgnoreCase(this string string1, string string2) => string.Equals(string1, string2, StringComparison.OrdinalIgnoreCase);

    internal static IEnumerable<T> ExceptNull<T>(this IEnumerable<T> source) => source == null ? Enumerable.Empty<T>() : source.Where<T>((Func<T, bool>) (item => (object) item != null));

    public static bool AreSetsEqual<T>(
      this IEnumerable<T> first,
      IEnumerable<T> second,
      IEqualityComparer<T> equalityComparer)
    {
      return first != null && second != null && new HashSet<T>(first, equalityComparer).SetEquals(second);
    }

    internal static Uri GetVSOEndpointRegistryOverride()
    {
      string connectedUserValue = VssClientEnvironment.GetSharedConnectedUserValue<string>("uri");
      return connectedUserValue != null ? AccountManagementUtilities.CheckUri(connectedUserValue) : (Uri) null;
    }

    internal static Uri GetEndpointRegistryOverride()
    {
      string connectedUserValue = VssClientEnvironment.GetSharedConnectedUserValue<string>("accounturi");
      return connectedUserValue != null ? AccountManagementUtilities.CheckUri(connectedUserValue) : (Uri) null;
    }

    internal static string GetExtraParametersRegistryOverride()
    {
      string connectedUserValue = VssClientEnvironment.GetSharedConnectedUserValue<string>("ExtraParametersRegistryOverride");
      return string.IsNullOrEmpty(connectedUserValue) ? (string) null : connectedUserValue;
    }

    internal static byte[] GetDefaultProviderImage()
    {
      using (Bitmap bitmap = (Bitmap) ClientResources.Manager.GetObject("BlueBadgeAccountImage192192", CultureInfo.InvariantCulture))
      {
        using (MemoryStream memoryStream = new MemoryStream())
        {
          bitmap.Save((Stream) memoryStream, ImageFormat.Png);
          return memoryStream.ToArray();
        }
      }
    }

    internal static byte[] GetMSAImage()
    {
      using (Bitmap bitmap = (Bitmap) ClientResources.Manager.GetObject("MicrosoftLogo_128xLG", CultureInfo.InvariantCulture))
      {
        using (MemoryStream memoryStream = new MemoryStream())
        {
          bitmap.Save((Stream) memoryStream, ImageFormat.Png);
          return memoryStream.ToArray();
        }
      }
    }

    internal static Account GetAccountFromKey(this IAccountStore store, AccountKey key)
    {
      ArgumentUtility.CheckForNull<IAccountStore>(store, nameof (store));
      return store.GetAllAccounts().FirstOrDefault<Account>((Func<Account, bool>) (x => AccountKey.KeyComparer.Equals(key, (AccountKey) x)));
    }
  }
}
