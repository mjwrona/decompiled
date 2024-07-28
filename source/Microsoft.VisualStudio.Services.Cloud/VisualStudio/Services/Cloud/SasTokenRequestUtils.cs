// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.SasTokenRequestUtils
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Azure.Storage.Sas;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public static class SasTokenRequestUtils
  {
    internal const string c_area = "SasTokenRequestUtil";
    internal const string c_layer = "Service";

    public static AccountSasServices ToAccountSasServices(SasRequestServices requestServices)
    {
      AccountSasServices accountSasServices = (AccountSasServices) 0;
      if (requestServices.HasFlag((Enum) SasRequestServices.All))
        return (AccountSasServices) -1;
      if (requestServices.HasFlag((Enum) SasRequestServices.Table))
        accountSasServices = (AccountSasServices) (accountSasServices | 8);
      if (requestServices.HasFlag((Enum) SasRequestServices.Blob))
        accountSasServices = (AccountSasServices) (accountSasServices | 1);
      if (requestServices.HasFlag((Enum) SasRequestServices.Queue))
        accountSasServices = (AccountSasServices) (accountSasServices | 2);
      if (requestServices.HasFlag((Enum) SasRequestServices.File))
        accountSasServices = (AccountSasServices) (accountSasServices | 4);
      return accountSasServices;
    }

    public static SharedAccessAccountServices ToSharedAccessAccountServices(
      SasRequestServices requestServices)
    {
      SharedAccessAccountServices accessAccountServices = SharedAccessAccountServices.None;
      if (requestServices.HasFlag((Enum) SasRequestServices.Table))
        accessAccountServices |= SharedAccessAccountServices.Table;
      if (requestServices.HasFlag((Enum) SasRequestServices.Blob))
        accessAccountServices |= SharedAccessAccountServices.Blob;
      if (requestServices.HasFlag((Enum) SasRequestServices.Queue))
        accessAccountServices |= SharedAccessAccountServices.Queue;
      if (requestServices.HasFlag((Enum) SasRequestServices.File))
        accessAccountServices |= SharedAccessAccountServices.File;
      if (requestServices.HasFlag((Enum) SasRequestServices.All))
        accessAccountServices = SharedAccessAccountServices.Blob | SharedAccessAccountServices.File | SharedAccessAccountServices.Queue | SharedAccessAccountServices.Table;
      return accessAccountServices;
    }

    public static SharedAccessBlobPermissions ToSharedAccessBlobPermissions(
      SasRequestPermissions requestPermissions)
    {
      SharedAccessBlobPermissions accessBlobPermissions = SharedAccessBlobPermissions.None;
      if (requestPermissions.HasFlag((Enum) SasRequestPermissions.Read))
        accessBlobPermissions |= SharedAccessBlobPermissions.Read;
      if (requestPermissions.HasFlag((Enum) SasRequestPermissions.Write))
        accessBlobPermissions |= SharedAccessBlobPermissions.Write;
      if (requestPermissions.HasFlag((Enum) SasRequestPermissions.Delete))
        accessBlobPermissions |= SharedAccessBlobPermissions.Delete;
      if (requestPermissions.HasFlag((Enum) SasRequestPermissions.List))
        accessBlobPermissions |= SharedAccessBlobPermissions.List;
      if (requestPermissions.HasFlag((Enum) SasRequestPermissions.Add))
        accessBlobPermissions |= SharedAccessBlobPermissions.Add;
      if (requestPermissions.HasFlag((Enum) SasRequestPermissions.Create))
        accessBlobPermissions |= SharedAccessBlobPermissions.Create;
      if (requestPermissions.HasFlag((Enum) SasRequestPermissions.All))
        accessBlobPermissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.Delete | SharedAccessBlobPermissions.List | SharedAccessBlobPermissions.Add | SharedAccessBlobPermissions.Create;
      return accessBlobPermissions;
    }

    public static SharedAccessAccountPermissions ToSharedAccessAccountPermissions(
      SasRequestPermissions requestPermissions)
    {
      SharedAccessAccountPermissions accountPermissions = SharedAccessAccountPermissions.None;
      if (requestPermissions.HasFlag((Enum) SasRequestPermissions.Read))
        accountPermissions |= SharedAccessAccountPermissions.Read;
      if (requestPermissions.HasFlag((Enum) SasRequestPermissions.Add))
        accountPermissions |= SharedAccessAccountPermissions.Add;
      if (requestPermissions.HasFlag((Enum) SasRequestPermissions.Write))
        accountPermissions |= SharedAccessAccountPermissions.Write;
      if (requestPermissions.HasFlag((Enum) SasRequestPermissions.List))
        accountPermissions |= SharedAccessAccountPermissions.List;
      if (requestPermissions.HasFlag((Enum) SasRequestPermissions.Create))
        accountPermissions |= SharedAccessAccountPermissions.Create;
      if (requestPermissions.HasFlag((Enum) SasRequestPermissions.Update))
        accountPermissions |= SharedAccessAccountPermissions.Update;
      if (requestPermissions.HasFlag((Enum) SasRequestPermissions.ProcessMessages))
        accountPermissions |= SharedAccessAccountPermissions.ProcessMessages;
      return accountPermissions;
    }

    public static AccountSasPermissions ToAccountSasPermissions(
      SasRequestPermissions requestPermissions)
    {
      AccountSasPermissions accountSasPermissions = (AccountSasPermissions) 0;
      if (requestPermissions.HasFlag((Enum) SasRequestPermissions.Read))
        accountSasPermissions = (AccountSasPermissions) (accountSasPermissions | 1);
      if (requestPermissions.HasFlag((Enum) SasRequestPermissions.Add))
        accountSasPermissions = (AccountSasPermissions) (accountSasPermissions | 16);
      if (requestPermissions.HasFlag((Enum) SasRequestPermissions.Write))
        accountSasPermissions = (AccountSasPermissions) ((AccountSasPermissions) (accountSasPermissions | 2) | 2048);
      if (requestPermissions.HasFlag((Enum) SasRequestPermissions.List))
        accountSasPermissions = (AccountSasPermissions) (accountSasPermissions | 8);
      if (requestPermissions.HasFlag((Enum) SasRequestPermissions.Create))
        accountSasPermissions = (AccountSasPermissions) (accountSasPermissions | 32);
      if (requestPermissions.HasFlag((Enum) SasRequestPermissions.Update))
        accountSasPermissions = (AccountSasPermissions) (accountSasPermissions | 64);
      if (requestPermissions.HasFlag((Enum) SasRequestPermissions.ProcessMessages))
        accountSasPermissions = (AccountSasPermissions) (accountSasPermissions | 128);
      if (requestPermissions.HasFlag((Enum) SasRequestPermissions.Tag))
        accountSasPermissions = (AccountSasPermissions) (accountSasPermissions | 256);
      if (requestPermissions.HasFlag((Enum) SasRequestPermissions.Filter))
        accountSasPermissions = (AccountSasPermissions) (accountSasPermissions | 512);
      return accountSasPermissions;
    }

    public static BlobContainerSasPermissions ToBlobContainerSasPermissions(
      SasRequestPermissions requestPermissions)
    {
      BlobContainerSasPermissions containerSasPermissions = (BlobContainerSasPermissions) 0;
      if (requestPermissions.HasFlag((Enum) SasRequestPermissions.Read))
        containerSasPermissions = (BlobContainerSasPermissions) (containerSasPermissions | 1);
      if (requestPermissions.HasFlag((Enum) SasRequestPermissions.Add))
        containerSasPermissions = (BlobContainerSasPermissions) (containerSasPermissions | 2);
      if (requestPermissions.HasFlag((Enum) SasRequestPermissions.Write))
        containerSasPermissions = (BlobContainerSasPermissions) ((BlobContainerSasPermissions) (containerSasPermissions | 8) | 1024);
      if (requestPermissions.HasFlag((Enum) SasRequestPermissions.List))
        containerSasPermissions = (BlobContainerSasPermissions) (containerSasPermissions | 32);
      if (requestPermissions.HasFlag((Enum) SasRequestPermissions.Create))
        containerSasPermissions = (BlobContainerSasPermissions) (containerSasPermissions | 4);
      if (requestPermissions.HasFlag((Enum) SasRequestPermissions.Delete))
        containerSasPermissions = (BlobContainerSasPermissions) ((BlobContainerSasPermissions) (containerSasPermissions | 16) | 128);
      if (requestPermissions.HasFlag((Enum) SasRequestPermissions.Tag))
        containerSasPermissions = (BlobContainerSasPermissions) (containerSasPermissions | 64);
      return containerSasPermissions;
    }

    public static void CheckRequestPermission(
      IVssRequestContext requestContext,
      SasRequestPermissions permission)
    {
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, HostMigrationSecurityConstants.SasRequestNamespaceId);
      try
      {
        if (permission == SasRequestPermissions.None)
          securityNamespace.ThrowAccessDeniedException(requestContext, "SasRequestPermissions", 0);
        securityNamespace.CheckPermission(requestContext, "SasRequestPermissions", (int) permission, false);
      }
      catch (Exception ex)
      {
        requestContext.TraceAlways(555556, TraceLevel.Error, "SasTokenRequestUtil", "Service", "Sas token request was denied. Exception info: {0}", (object) ex);
        throw;
      }
    }
  }
}
