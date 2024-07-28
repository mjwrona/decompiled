// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TicketGenerationSigningConsumer
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class TicketGenerationSigningConsumer : ISigningServiceConsumer
  {
    private static readonly Guid[] s_wellKnownKeys = new Guid[1]
    {
      ProxyConstants.ProxySigningKey
    };
    private static readonly string s_area = "FileService";
    private static readonly string s_layer = nameof (TicketGenerationSigningConsumer);

    public IEnumerable<Guid> GetSigningKeysInUse(IVssRequestContext requestContext)
    {
      TeamFoundationSigningService signingService = requestContext.GetService<TeamFoundationSigningService>();
      return (IEnumerable<Guid>) ((IEnumerable<Guid>) TicketGenerationSigningConsumer.s_wellKnownKeys).Where<Guid>((Func<Guid, bool>) (x => signingService.GetSigningKey(requestContext, x, createIfNotExists: false) != null)).ToList<Guid>();
    }

    public ReencryptResults Reencrypt(IVssRequestContext requestContext, Guid identifier) => new ReencryptResults();

    public ReencryptResults ConvertKeys(IVssRequestContext requestContext, SigningKeyType keyType)
    {
      ITeamFoundationSigningService service = requestContext.GetService<ITeamFoundationSigningService>();
      ReencryptResults reencryptResults = new ReencryptResults();
      foreach (Guid identifier in this.GetSigningKeysInUse(requestContext))
      {
        try
        {
          if (service.GetSigningKeyType(requestContext, identifier) != keyType)
          {
            requestContext.Trace(108200, TraceLevel.Verbose, TicketGenerationSigningConsumer.s_area, TicketGenerationSigningConsumer.s_layer, "Regenerating TicketGenerationSigningKey Signing Key Id: {0}", (object) identifier);
            service.RegenerateKey(requestContext, identifier, keyType);
            ++reencryptResults.SuccessCount;
          }
        }
        catch (Exception ex)
        {
          requestContext.TraceException(109700, TicketGenerationSigningConsumer.s_area, TicketGenerationSigningConsumer.s_layer, ex);
          ++reencryptResults.FailureCount;
          reencryptResults.Failures.Add(new Exception(string.Format("Error converting signing key {0}: {1}", (object) identifier, (object) ex), ex));
        }
      }
      return reencryptResults;
    }
  }
}
