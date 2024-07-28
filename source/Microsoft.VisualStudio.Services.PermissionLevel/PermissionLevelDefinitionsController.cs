// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PermissionLevel.PermissionLevelDefinitionsController
// Assembly: Microsoft.VisualStudio.Services.PermissionLevel, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 43771064-3FEF-4CA1-8A8B-671AEDB99122
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PermissionLevel.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.PermissionLevel
{
  [ControllerApiVersion(5.1)]
  [VersionedApiControllerCustomName(Area = "PermissionLevel", ResourceName = "PermissionLevelDefinitions")]
  public class PermissionLevelDefinitionsController : TfsApiController
  {
    private const string c_PermissionLevelDefinitionsApis = "Microsoft.VisualStudio.PermissionLevel.PermissionLevelDefinitionApis.Enable";
    private static readonly Dictionary<Type, HttpStatusCode> s_httpExceptions = new Dictionary<Type, HttpStatusCode>()
    {
      {
        typeof (NotSupportedException),
        HttpStatusCode.ServiceUnavailable
      },
      {
        typeof (NotImplementedException),
        HttpStatusCode.NotImplemented
      },
      {
        typeof (UnexpectedHostTypeException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (ArgumentException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (AccessCheckException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (PermissionLevelDefinitionNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (PermissionLevelDefinitionAlreadyExistsException),
        HttpStatusCode.Conflict
      }
    };

    public override string ActivityLogArea => "PermissionLevelDefinition";

    public override string TraceArea => "PermissionLevelDefinition";

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) PermissionLevelDefinitionsController.s_httpExceptions;

    [HttpGet]
    public IDictionary<Guid, Microsoft.VisualStudio.Services.PermissionLevel.Client.PermissionLevelDefinition> GetPermissionLevelDefinitionsById(
      [ClientQueryParameter, ClientParameterAsIEnumerable(typeof (Guid), ',')] string definitionIds = null)
    {
      if (!this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.PermissionLevel.PermissionLevelDefinitionApis.Enable"))
        throw new NotImplementedException();
      IList<Guid> commaSeparatedString = this.ParseCommaSeparatedString(definitionIds);
      return (IDictionary<Guid, Microsoft.VisualStudio.Services.PermissionLevel.Client.PermissionLevelDefinition>) this.TfsRequestContext.GetService<IPermissionLevelDefinitionService>().GetPermissionLevelDefinitions(this.TfsRequestContext, (IEnumerable<Guid>) commaSeparatedString).ToDictionary<KeyValuePair<Guid, Microsoft.TeamFoundation.Framework.Server.PermissionLevel.DataModels.PermissionLevelDefinition>, Guid, Microsoft.VisualStudio.Services.PermissionLevel.Client.PermissionLevelDefinition>((Func<KeyValuePair<Guid, Microsoft.TeamFoundation.Framework.Server.PermissionLevel.DataModels.PermissionLevelDefinition>, Guid>) (x => x.Key), (Func<KeyValuePair<Guid, Microsoft.TeamFoundation.Framework.Server.PermissionLevel.DataModels.PermissionLevelDefinition>, Microsoft.VisualStudio.Services.PermissionLevel.Client.PermissionLevelDefinition>) (x => x.Value.ToClient()));
    }

    [HttpGet]
    public IEnumerable<Microsoft.VisualStudio.Services.PermissionLevel.Client.PermissionLevelDefinition> GetPermissionLevelDefinitionsByScope(
      [ClientQueryParameter] PermissionLevelDefinitionScope definitionScope,
      [ClientQueryParameter] PermissionLevelDefinitionType definitionType)
    {
      if (!this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.PermissionLevel.PermissionLevelDefinitionApis.Enable"))
        throw new NotImplementedException();
      return (IEnumerable<Microsoft.VisualStudio.Services.PermissionLevel.Client.PermissionLevelDefinition>) this.TfsRequestContext.GetService<IPermissionLevelDefinitionService>().GetPermissionLevelDefinitions(this.TfsRequestContext, definitionScope, definitionType).ToClient();
    }

    private IList<Guid> ParseCommaSeparatedString(string inputString)
    {
      List<Guid> commaSeparatedString = new List<Guid>();
      string str = inputString;
      string[] separator = new string[1]{ "," };
      foreach (string input in str.Split(separator, StringSplitOptions.RemoveEmptyEntries))
      {
        Guid result;
        if (!Guid.TryParse(input, out result))
          throw new ArgumentException("Failed to parse the permission level definition IDs list: " + inputString + ".");
        commaSeparatedString.Add(result);
      }
      return (IList<Guid>) commaSeparatedString;
    }
  }
}
