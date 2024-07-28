// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabaseCredentialSigningConsumer
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class DatabaseCredentialSigningConsumer : ISigningServiceConsumer
  {
    private static readonly string s_area = "DatabaseManagement";
    private static readonly string s_layer = "BusinessLogic";

    public IEnumerable<Guid> GetSigningKeysInUse(IVssRequestContext requestContext)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return (IEnumerable<Guid>) Array.Empty<Guid>();
      using (DatabaseCredentialsComponent component = requestContext.CreateComponent<DatabaseCredentialsComponent>())
        return (IEnumerable<Guid>) component.QueryDatabaseCredentialSigningKeys();
    }

    public ReencryptResults Reencrypt(IVssRequestContext requestContext, Guid identifier)
    {
      ReencryptResults reencryptResults = new ReencryptResults();
      ITeamFoundationSigningService service = requestContext.GetService<ITeamFoundationSigningService>();
      requestContext.Trace(99243, TraceLevel.Verbose, DatabaseCredentialSigningConsumer.s_area, DatabaseCredentialSigningConsumer.s_layer, "Re-encrypting database credentials");
      List<TeamFoundationDatabaseCredential> databaseCredentialList;
      using (DatabaseCredentialsComponent component = requestContext.CreateComponent<DatabaseCredentialsComponent>())
        databaseCredentialList = component.QueryDatabaseCredentials();
      foreach (TeamFoundationDatabaseCredential credential in databaseCredentialList)
      {
        try
        {
          if (credential.SigningKeyId != identifier)
          {
            requestContext.Trace(99244, TraceLevel.Verbose, DatabaseCredentialSigningConsumer.s_area, DatabaseCredentialSigningConsumer.s_layer, "Re-encrypting database credential {0}", (object) credential.Id);
            byte[] rawData = service.Decrypt(requestContext, credential.SigningKeyId, credential.PasswordEncrypted, SigningAlgorithm.SHA256);
            try
            {
              credential.PasswordEncrypted = service.Encrypt(requestContext, identifier, rawData, SigningAlgorithm.SHA256);
            }
            finally
            {
              Array.Clear((Array) rawData, 0, rawData.Length);
            }
            credential.SigningKeyId = identifier;
            using (DatabaseCredentialsComponent component = requestContext.CreateComponent<DatabaseCredentialsComponent>())
            {
              if (component is DatabaseCredentialsComponent5)
                ((DatabaseCredentialsComponent5) component).UpdateDatabaseCredential(credential);
              else
                component.UpdateDatabaseCredential(credential, false);
            }
            ++reencryptResults.SuccessCount;
          }
        }
        catch (Exception ex)
        {
          requestContext.TraceException(99245, DatabaseCredentialSigningConsumer.s_area, DatabaseCredentialSigningConsumer.s_layer, ex);
          reencryptResults.Failures.Add(new Exception(string.Format("Error encrypting database credential {0}: {1}", (object) (credential != null ? credential.Id : -1), (object) ex), ex));
          ++reencryptResults.FailureCount;
        }
      }
      return reencryptResults;
    }
  }
}
