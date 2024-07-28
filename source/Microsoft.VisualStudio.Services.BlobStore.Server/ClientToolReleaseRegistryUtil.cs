// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.ClientToolReleaseRegistryUtil
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common.Exceptions;
using Microsoft.VisualStudio.Services.BlobStore.WebApi;
using Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  public static class ClientToolReleaseRegistryUtil
  {
    public static void RegisterCurrentVersion(
      IVssRequestContext requestContext,
      ClientTool appName,
      RuntimeIdentifier runtimeIdentifier,
      string version)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(version, nameof (version));
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string currentPath = ClientToolReleaseRegistryUtil.GetCurrentPath(appName, runtimeIdentifier);
      string previousPath = ClientToolReleaseRegistryUtil.GetPreviousPath(appName, runtimeIdentifier);
      string str = service.GetValue(requestContext, (RegistryQuery) currentPath, false, (string) null);
      service.SetValue<string>(requestContext, previousPath, str);
      service.SetValue<string>(requestContext, currentPath, version);
    }

    public static void RevertCurrentVersion(IVssRequestContext requestContext, ClientTool appName)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      foreach (RuntimeIdentifier runtimeIdentifier in Enum.GetValues(typeof (RuntimeIdentifier)))
      {
        string currentPath = ClientToolReleaseRegistryUtil.GetCurrentPath(appName, runtimeIdentifier);
        string previousPath = ClientToolReleaseRegistryUtil.GetPreviousPath(appName, runtimeIdentifier);
        string str = service.GetValue(requestContext, (RegistryQuery) previousPath, false, (string) null);
        service.SetValue<string>(requestContext, currentPath, str);
      }
    }

    public static void RegisterVersion(
      IVssRequestContext requestContext,
      ClientTool appName,
      RuntimeIdentifier runtimeIdentifier,
      string version,
      string fileName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(version, nameof (version));
      ArgumentUtility.CheckStringForNullOrEmpty(fileName, nameof (fileName));
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string versionPath = ClientToolReleaseRegistryUtil.GetVersionPath(appName, runtimeIdentifier, version);
      string str = service.GetValue(requestContext, (RegistryQuery) versionPath, false, (string) null);
      if (string.IsNullOrWhiteSpace(str))
        service.SetValue<string>(requestContext, versionPath, fileName);
      else
        throw new RegistryAlreadyExistsException("Version " + version + " already exists and points to file " + str + ".");
    }

    public static string GetCurrentPath(ClientTool appName, RuntimeIdentifier runtimeIdentifier) => ClientToolReleaseRegistryUtil.GetVersionPath(appName, runtimeIdentifier, "Current");

    public static string GetPreviousPath(ClientTool appName, RuntimeIdentifier runtimeIdentifier) => ClientToolReleaseRegistryUtil.GetVersionPath(appName, runtimeIdentifier, "Previous");

    public static string GetVersionPath(
      ClientTool appName,
      RuntimeIdentifier runtimeIdentifier,
      string version)
    {
      return string.Format("{0}/{1}/{2}/{3}", (object) "/Configuration/ClientToolReleases", (object) appName, (object) RuntimeIdentifierHelper.GetRuntimeName(runtimeIdentifier), (object) version);
    }
  }
}
