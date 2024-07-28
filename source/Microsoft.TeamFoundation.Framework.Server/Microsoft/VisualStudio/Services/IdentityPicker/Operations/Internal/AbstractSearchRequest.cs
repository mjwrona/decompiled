// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal.AbstractSearchRequest
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal
{
  public abstract class AbstractSearchRequest
  {
    [JsonProperty("identityType")]
    [JsonConverter(typeof (StringEnumConverter))]
    internal IdentityTypeEnum IdentityType { get; set; }

    [JsonProperty("operationScope")]
    [JsonConverter(typeof (StringEnumConverter))]
    internal OperationScopeEnum OperationScope { get; set; }

    [JsonProperty("query")]
    public string Query { get; set; }

    [JsonProperty("queryTypeHint")]
    public QueryTypeHintEnum QueryTypeHint { get; set; }

    [JsonProperty("options")]
    internal Dictionary<string, object> Options { get; set; }

    [JsonProperty("requestProperties")]
    internal HashSet<string> RequestProperties { get; set; }

    [JsonProperty("filterByAncestorEntityIds")]
    internal HashSet<string> FilterByAncestorEntityIds { get; set; }

    [JsonProperty("filterByEntityIds")]
    internal HashSet<string> FilterByEntityIds { get; set; }

    protected AbstractSearchRequest(
      string query,
      string queryTypeHint,
      IList<string> identityTypes,
      IList<string> operationScopes,
      IList<string> requestProperties,
      IList<string> filterByAncestorEntityIds,
      IList<string> filterByEntityIds)
    {
      try
      {
        if (identityTypes == null || identityTypes.Count == 0)
          identityTypes = (IList<string>) new List<string>();
        if (operationScopes == null || operationScopes.Count == 0)
          operationScopes = (IList<string>) new List<string>();
        this.IdentityType = IdentityOperationHelper.ParseIdentityTypes(identityTypes);
        this.OperationScope = IdentityOperationHelper.ParseOperationScopes(operationScopes);
        this.Query = query;
        this.QueryTypeHint = IdentityOperationHelper.ParseQueryTypeHint(queryTypeHint);
        this.RequestProperties = requestProperties != null ? new HashSet<string>((IEnumerable<string>) requestProperties, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) : new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        this.FilterByAncestorEntityIds = filterByAncestorEntityIds != null ? new HashSet<string>((IEnumerable<string>) filterByAncestorEntityIds, (IEqualityComparer<string>) VssStringComparer.DirectoryKeyStringComparer) : new HashSet<string>((IEqualityComparer<string>) VssStringComparer.DirectoryKeyStringComparer);
        this.FilterByEntityIds = filterByEntityIds != null ? new HashSet<string>((IEnumerable<string>) filterByEntityIds, (IEqualityComparer<string>) VssStringComparer.DirectoryKeyStringComparer) : new HashSet<string>((IEqualityComparer<string>) VssStringComparer.DirectoryKeyStringComparer);
      }
      catch (Exception ex)
      {
        throw new IdentityPickerArgumentException("Invalid arguments", ex);
      }
    }

    protected AbstractSearchRequest(
      IdentityTypeEnum identityType,
      OperationScopeEnum scope,
      Dictionary<string, object> options,
      string query,
      HashSet<string> requestProperties,
      HashSet<string> filterByAncestorEntityIds,
      HashSet<string> filterByEntityIds)
    {
      this.IdentityType = identityType;
      this.OperationScope = scope;
      this.Options = options != null ? new Dictionary<string, object>((IDictionary<string, object>) options, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) : new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.Query = query;
      this.RequestProperties = requestProperties != null ? new HashSet<string>((IEnumerable<string>) requestProperties, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) : new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.FilterByAncestorEntityIds = filterByAncestorEntityIds != null ? new HashSet<string>((IEnumerable<string>) filterByAncestorEntityIds, (IEqualityComparer<string>) VssStringComparer.DirectoryKeyStringComparer) : new HashSet<string>((IEqualityComparer<string>) VssStringComparer.DirectoryKeyStringComparer);
      this.FilterByEntityIds = filterByEntityIds != null ? new HashSet<string>((IEnumerable<string>) filterByEntityIds, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) : new HashSet<string>((IEqualityComparer<string>) VssStringComparer.DirectoryKeyStringComparer);
    }

    protected AbstractSearchRequest()
    {
      this.IdentityType = IdentityTypeEnum.None;
      this.OperationScope = OperationScopeEnum.None;
      this.Options = new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.Query = string.Empty;
      this.RequestProperties = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.FilterByAncestorEntityIds = new HashSet<string>((IEqualityComparer<string>) VssStringComparer.DirectoryKeyStringComparer);
      this.FilterByEntityIds = new HashSet<string>((IEqualityComparer<string>) VssStringComparer.DirectoryKeyStringComparer);
    }

    public virtual void Validate(IVssRequestContext requestContext)
    {
      IdentityOperationHelper.ValidateRequestContext(requestContext);
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Application) && !requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new IdentityPickerValidateException("Application or ProjectCollection host required.");
      if (string.IsNullOrEmpty(this.Query))
        throw new IdentityPickerArgumentException("Query (required parameter) is null or empty");
      if (this.IdentityType == IdentityTypeEnum.None)
        throw new IdentityPickerArgumentException("Invalid IdentityType (required parameter)");
      if (this.OperationScope == OperationScopeEnum.None)
        throw new IdentityPickerArgumentException("Invalid OperationScope (required parameter)");
      if (this.Options == null || this.Options.Keys.Count <= 0)
        return;
      SearchOptions.Validate(this.Options);
    }
  }
}
