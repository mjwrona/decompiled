// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.XEventDatabaseExtensions
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.Storage.Blob;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CloudConfiguration;
using System;
using System.IO;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public static class XEventDatabaseExtensions
  {
    public static Guid GetXEventSessionContainerId(this ITeamFoundationDatabaseProperties database)
    {
      byte[] byteArray = XEventConstants.SessionBaseContainerId.ToByteArray();
      byte[] bytes = BitConverter.GetBytes(database.DatabaseId);
      Array.Copy((Array) bytes, (Array) byteArray, bytes.Length);
      return new Guid(byteArray);
    }

    public static bool IsXEventSupported(
      this ITeamFoundationDatabaseProperties database,
      IVssRequestContext requestContext)
    {
      if (requestContext.ExecutionEnvironment.IsDevFabricDeployment)
        return true;
      using (TeamFoundationDataTierComponent componentRaw = database.GetDboConnectionInfo().CreateComponentRaw<TeamFoundationDataTierComponent>())
        return componentRaw.IsSqlAzure;
    }

    public static bool IsReadScaleOutXEventSupported(
      this ITeamFoundationDatabaseProperties database,
      IVssRequestContext requestContext)
    {
      return requestContext.IsFeatureEnabled("VisualStudio.FrameworkService.ResourceUtilization2.ReadScaleOutXevents") && !requestContext.ExecutionEnvironment.IsDevFabricDeployment && database.IsXEventSupported(requestContext);
    }

    public static bool ManageXEventSession(
      this ITeamFoundationDatabaseProperties database,
      IVssRequestContext requestContext,
      Action<TeamFoundationDataTierComponent, TeamFoundationDataTierComponent> operation,
      Action preOperation = null,
      Action postOperation = null)
    {
      if (!database.IsXEventSupported(requestContext))
        return false;
      if (preOperation != null)
        preOperation();
      database.ExecuteDataTierOperation(requestContext, operation);
      if (postOperation != null)
        postOperation();
      return true;
    }

    public static XEventSessionSettings GetXEventSessionSettings(
      this ITeamFoundationDatabaseProperties database,
      IVssRequestContext requestContext,
      string storagePath = null,
      string diagnosticsConnectionString = null)
    {
      if (!database.IsXEventSupported(requestContext))
        throw new NotSupportedException("XEvent is not supported on " + database.DatabaseName + ".");
      XEventSessionSettings xeventSessionSettings = new XEventSessionSettings();
      Guid sessionContainerId = database.GetXEventSessionContainerId();
      if (requestContext.ExecutionEnvironment.IsDevFabricDeployment)
      {
        FileSystemProvider fabricBlobProvider = XEventBlobUtils.GetDevFabricBlobProvider(requestContext, storagePath);
        xeventSessionSettings.BlobStoragePath = fabricBlobProvider.GetContainerPath(sessionContainerId, true);
        xeventSessionSettings.PathSeparator = Path.DirectorySeparatorChar;
        xeventSessionSettings.SharedAccessSignature = (string) null;
      }
      else
      {
        CloudBlobContainer cloudBlobContainer = XEventBlobUtils.GetAzureBlobProvider(requestContext, diagnosticsConnectionString).GetCloudBlobContainer(requestContext, sessionContainerId, true, new TimeSpan?());
        xeventSessionSettings.BlobStoragePath = cloudBlobContainer.Uri.AbsoluteUri;
        xeventSessionSettings.PathSeparator = '/';
        SasTokenRequestService service = requestContext.GetService<SasTokenRequestService>();
        string sasToken = service.GetSasToken(requestContext, diagnosticsConnectionString, sessionContainerId, SasRequestPermissions.Read | SasRequestPermissions.Write | SasRequestPermissions.List, SecretRotationConstants.MaxTimeAllowedBetweenRotations);
        string str = service.DecryptToken(requestContext, sasToken);
        xeventSessionSettings.SharedAccessSignature = str.TrimStart('?');
      }
      return xeventSessionSettings;
    }
  }
}
