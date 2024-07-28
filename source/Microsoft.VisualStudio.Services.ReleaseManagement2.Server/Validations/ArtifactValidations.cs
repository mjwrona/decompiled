// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Validations.ArtifactValidations
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.ServiceEndpoints.Common;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Validations
{
  public static class ArtifactValidations
  {
    public static void Validate(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact artifact,
      IVssRequestContext requestContext,
      Func<string, ArtifactTypeBase> validateArtifactType)
    {
      if (artifact == null)
        throw new ArgumentNullException(nameof (artifact));
      if (validateArtifactType == null)
        throw new ArgumentNullException(nameof (validateArtifactType));
      ArtifactValidations.ValidateArtifact(requestContext, artifact, validateArtifactType);
    }

    public static void ValidateEndpointPermissionsForArtifactsForCreate(
      IVssRequestContext tfsRequestContext,
      Guid projectId,
      ReleaseDefinition serverDefinitionObject)
    {
      if (tfsRequestContext == null)
        throw new ArgumentNullException(nameof (tfsRequestContext));
      if (serverDefinitionObject == null)
        throw new ArgumentNullException(nameof (serverDefinitionObject));
      Dictionary<string, Dictionary<string, InputValue>> dictionary = serverDefinitionObject.LinkedArtifacts.Where<ArtifactSource>((Func<ArtifactSource, bool>) (artifact => artifact.SourceData.ContainsKey("connection"))).ToDictionary<ArtifactSource, string, Dictionary<string, InputValue>>((Func<ArtifactSource, string>) (artifact => artifact.Alias), (Func<ArtifactSource, Dictionary<string, InputValue>>) (artifact => new Dictionary<string, InputValue>((IDictionary<string, InputValue>) artifact.SourceData)
      {
        ["type"] = new InputValue()
        {
          Value = artifact.ArtifactTypeId,
          DisplayValue = (string) null
        }
      }));
      if (!dictionary.Any<KeyValuePair<string, Dictionary<string, InputValue>>>())
        return;
      ArtifactValidations.ValidateEndpointPermissionsForArtifacts(tfsRequestContext, projectId, new Dictionary<string, Dictionary<string, InputValue>>(), dictionary);
    }

    public static void ValidateEndpointPermissionsForArtifactsForUpdate(
      IVssRequestContext tfsRequestContext,
      Guid projectId,
      ReleaseDefinition serverDefinitionObject)
    {
      if (tfsRequestContext == null)
        throw new ArgumentNullException(nameof (tfsRequestContext));
      if (serverDefinitionObject == null)
        throw new ArgumentNullException(nameof (serverDefinitionObject));
      Dictionary<string, Dictionary<string, InputValue>> dictionary1 = tfsRequestContext.GetService<ReleaseDefinitionsService>().GetReleaseDefinition(tfsRequestContext, projectId, serverDefinitionObject.Id).LinkedArtifacts.Where<ArtifactSource>((Func<ArtifactSource, bool>) (artifact => artifact.SourceData.ContainsKey("connection"))).ToDictionary<ArtifactSource, string, Dictionary<string, InputValue>>((Func<ArtifactSource, string>) (artifact => artifact.Alias), (Func<ArtifactSource, Dictionary<string, InputValue>>) (artifact => new Dictionary<string, InputValue>((IDictionary<string, InputValue>) artifact.SourceData)
      {
        ["type"] = new InputValue()
        {
          Value = artifact.ArtifactTypeId,
          DisplayValue = (string) null
        }
      }));
      Dictionary<string, Dictionary<string, InputValue>> dictionary2 = serverDefinitionObject.LinkedArtifacts.Where<ArtifactSource>((Func<ArtifactSource, bool>) (artifact => artifact.SourceData.ContainsKey("connection"))).ToDictionary<ArtifactSource, string, Dictionary<string, InputValue>>((Func<ArtifactSource, string>) (artifact => artifact.Alias), (Func<ArtifactSource, Dictionary<string, InputValue>>) (artifact => new Dictionary<string, InputValue>((IDictionary<string, InputValue>) artifact.SourceData)
      {
        ["type"] = new InputValue()
        {
          Value = artifact.ArtifactTypeId,
          DisplayValue = (string) null
        }
      }));
      if (!dictionary2.Any<KeyValuePair<string, Dictionary<string, InputValue>>>())
        return;
      ArtifactValidations.ValidateEndpointPermissionsForArtifacts(tfsRequestContext, projectId, dictionary1, dictionary2);
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is required for testablity.")]
    [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Null guard is already there.")]
    public static void ValidateEndpointPermissionsForArtifacts(
      IVssRequestContext tfsRequestContext,
      Guid projectId,
      Dictionary<string, Dictionary<string, InputValue>> currentArtifactMapping,
      Dictionary<string, Dictionary<string, InputValue>> newArtifactMapping)
    {
      Dictionary<Guid, Tuple<List<string>, string>> dictionary1 = new Dictionary<Guid, Tuple<List<string>, string>>();
      Dictionary<Guid, Tuple<List<string>, string>> dictionary2 = new Dictionary<Guid, Tuple<List<string>, string>>();
      Dictionary<string, List<string>> descriptorIdsMapping = ArtifactValidations.GetArtifactTypeToEndpointInputDescriptorIdsMapping(tfsRequestContext, projectId);
      foreach (string key1 in newArtifactMapping.Keys)
      {
        string key2 = newArtifactMapping[key1]["type"].Value;
        if (descriptorIdsMapping.ContainsKey(key2))
        {
          foreach (string key3 in descriptorIdsMapping[key2])
          {
            if (newArtifactMapping[key1].ContainsKey(key3) && newArtifactMapping[key1][key3] != null)
            {
              InputValue inputValue = newArtifactMapping[key1][key3];
              string displayValue = inputValue.DisplayValue;
              Guid result;
              if (!Guid.TryParse(inputValue.Value, out result))
              {
                string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Endpoint: '{0}' not a valid Guid", (object) displayValue);
                tfsRequestContext.Trace(1900000, TraceLevel.Info, "ReleaseManagementService", "Service", message);
              }
              else
              {
                if (currentArtifactMapping.ContainsKey(key1))
                {
                  Dictionary<string, InputValue> currentMapData = currentArtifactMapping[key1];
                  Dictionary<string, InputValue> newMapData = newArtifactMapping[key1];
                  bool flag = false;
                  if (newMapData.Keys.Any<string>((Func<string, bool>) (key => !currentMapData.ContainsKey(key) || !ArtifactValidations.CompareInputValues(newMapData[key], currentMapData[key]))))
                    flag = true;
                  if (!flag && currentMapData.Keys.Any<string>((Func<string, bool>) (key => !newMapData.ContainsKey(key))))
                    flag = true;
                  if (flag)
                    dictionary1.AddEndpoint(result, displayValue, key1);
                }
                else
                  dictionary1.AddEndpoint(result, displayValue, key1);
                if (!dictionary1.ContainsKey(result))
                  dictionary2.AddEndpoint(result, displayValue, key1);
              }
            }
          }
        }
      }
      if (!dictionary1.Any<KeyValuePair<Guid, Tuple<List<string>, string>>>() && !dictionary2.Any<KeyValuePair<Guid, Tuple<List<string>, string>>>())
        return;
      ArtifactValidations.CheckEndpointPermissionAndExistence(tfsRequestContext, projectId, dictionary1, dictionary2);
    }

    public static bool IsValidAliasName(string name) => ArtifactValidations.IsValidAliasName(name, string.Empty);

    public static bool IsValidAliasName(string name, string additionalCharacters) => !new Regex(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[{0}{1}]", (object) Regex.Escape(new string(Path.GetInvalidFileNameChars())), (object) additionalCharacters)).IsMatch(name);

    private static void AddEndpoint(
      this Dictionary<Guid, Tuple<List<string>, string>> endpointArtifactsMapForChecks,
      Guid endpointId,
      string endpointName,
      string artifactAlias)
    {
      if (endpointArtifactsMapForChecks.ContainsKey(endpointId))
        endpointArtifactsMapForChecks[endpointId].Item1.Add(artifactAlias);
      else
        endpointArtifactsMapForChecks.Add(endpointId, Tuple.Create<List<string>, string>(new List<string>()
        {
          artifactAlias
        }, endpointName));
    }

    private static Dictionary<string, List<string>> GetArtifactTypeToEndpointInputDescriptorIdsMapping(
      IVssRequestContext tfsRequestContext,
      Guid projectId)
    {
      IEnumerable<ArtifactTypeDefinition> artifactTypeDefinitions = tfsRequestContext.GetService<ArtifactTypeDefinitionService>().GetArtifactTypeDefinitions(tfsRequestContext, projectId);
      Dictionary<string, List<string>> descriptorIdsMapping = new Dictionary<string, List<string>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (ArtifactTypeDefinition artifactTypeDefinition in artifactTypeDefinitions)
      {
        string name = artifactTypeDefinition.Name;
        descriptorIdsMapping[name] = new List<string>();
        foreach (InputDescriptor inputDescriptor in (IEnumerable<InputDescriptor>) artifactTypeDefinition.InputDescriptors)
        {
          if (inputDescriptor.Id.Equals("connection") || !string.IsNullOrEmpty(inputDescriptor.Type) && inputDescriptor.Type.StartsWith("connectedService:", StringComparison.OrdinalIgnoreCase))
            descriptorIdsMapping[name].Add(inputDescriptor.Id);
        }
      }
      return descriptorIdsMapping;
    }

    private static void ValidateArtifact(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact artifact,
      Func<string, ArtifactTypeBase> validateArtifactType)
    {
      ArtifactTypeBase artifactType = validateArtifactType(artifact.Type);
      string definitionIdentifier = artifact.GetDefinitionIdentifier(out bool _);
      ArtifactSourceReference artifactSourceReference;
      if (artifact.DefinitionReference == null || !artifact.DefinitionReference.TryGetValue(definitionIdentifier, out artifactSourceReference) || artifactSourceReference == null || artifactSourceReference.Id.IsNullOrEmpty<char>())
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.ArtifactSourceShouldHaveSourceInfo);
      ArtifactValidations.ValidateArtifactDefaultBranch(requestContext, artifact);
      if (artifactType != null)
        ArtifactValidations.ValidateArtifactInputs(artifactType, artifact);
      ArtifactValidations.ValidateWebApiArtifactAlias(artifact);
    }

    private static void ValidateArtifactDefaultBranch(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact artifact)
    {
      ArtifactSourceReference artifactSourceReference = (ArtifactSourceReference) null;
      if (string.Equals(artifact.Type, "Git", StringComparison.OrdinalIgnoreCase))
        artifact.DefinitionReference.TryGetValue("branches", out artifactSourceReference);
      else if (string.Equals(artifact.Type, "GitHub", StringComparison.OrdinalIgnoreCase) || ArtifactTypeUtility.IsCustomArtifact(requestContext, artifact.Type))
        artifact.DefinitionReference.TryGetValue("branch", out artifactSourceReference);
      else if (string.Equals(artifact.Type, "Build", StringComparison.OrdinalIgnoreCase))
        artifact.DefinitionReference.TryGetValue("defaultVersionBranch", out artifactSourceReference);
      if (artifactSourceReference != null && artifactSourceReference.Id != null && artifactSourceReference.Id.Contains<char>('*'))
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.WildCardNotAllowedInBranch, (object) artifact.Alias));
    }

    private static void ValidateArtifactInputs(ArtifactTypeBase artifactType, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact artifact)
    {
      foreach (InputDescriptor inputDescriptor in (IEnumerable<InputDescriptor>) artifactType.InputDescriptors)
      {
        ArtifactSourceReference defaultVersion;
        bool isKeyPresent = artifact.DefinitionReference.TryGetValue(inputDescriptor.Id, out defaultVersion);
        if (string.Equals(inputDescriptor.Id, "defaultVersionType", StringComparison.OrdinalIgnoreCase))
          ArtifactValidations.ValidateArtifactDefaultVersionType(isKeyPresent, defaultVersion, artifact);
        else if (ArtifactValidations.IsMandatoryArtifactInput(inputDescriptor) && (!isKeyPresent || defaultVersion == null || defaultVersion.Id == null))
          throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.ArtifactInputFieldShouldBePresent, (object) inputDescriptor.Id, (object) artifact.Alias));
        if (isKeyPresent && defaultVersion != null && !string.IsNullOrWhiteSpace(defaultVersion.Id))
        {
          if (inputDescriptor.Validation != null)
          {
            try
            {
              InputDescriptorValidator.GetValidator(inputDescriptor.Validation).Validate(defaultVersion.Id, inputDescriptor.Id);
            }
            catch (ArgumentException ex)
            {
              throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ex.Message));
            }
          }
        }
      }
      foreach (KeyValuePair<string, ArtifactSourceReference> keyValuePair in (IEnumerable<KeyValuePair<string, ArtifactSourceReference>>) artifact.DefinitionReference)
      {
        KeyValuePair<string, ArtifactSourceReference> definitionReference = keyValuePair;
        if (!ArtifactValidations.IsValidAdditionalArtifactInput(definitionReference.Key) && !artifactType.InputDescriptors.Any<InputDescriptor>((Func<InputDescriptor, bool>) (x => string.Equals(x.Id, definitionReference.Key, StringComparison.OrdinalIgnoreCase))))
          throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.InvalidInputFieldForArtifact, (object) definitionReference.Key, (object) artifact.Alias));
      }
    }

    private static bool IsValidAdditionalArtifactInput(string inputId) => string.Equals(inputId, "defaultVersionType", StringComparison.OrdinalIgnoreCase) || string.Equals(inputId, "artifactSourceDefinitionUrl", StringComparison.OrdinalIgnoreCase);

    private static void ValidateArtifactDefaultVersionType(
      bool isKeyPresent,
      ArtifactSourceReference defaultVersion,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact artifact)
    {
      string errorMessage;
      if (isKeyPresent && defaultVersion != null && !defaultVersion.Id.IsNullOrEmpty<char>() && !ArtifactConverter.IsValidDefaultVersionTypeForArtifact(defaultVersion?.Id, artifact.Type, out errorMessage))
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(errorMessage);
    }

    private static bool IsMandatoryArtifactInput(InputDescriptor descriptor)
    {
      if (descriptor.Validation == null || !descriptor.Validation.IsRequired)
        return false;
      return descriptor.Properties == null || !descriptor.Properties.ContainsKey("visibleRule");
    }

    private static void ValidateWebApiArtifactAlias(Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact artifact)
    {
      if (string.IsNullOrEmpty(artifact.Alias))
        return;
      if (artifact.Alias.Length > 256)
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.ReleaseManagementObjectNameLengthExceeded, (object) "Alias", (object) artifact.Alias, (object) 256));
      if (!ArtifactValidations.IsValidFolderName(artifact.Alias))
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.ArtifactAliasCannotHaveReservedFileSystemNames, (object) artifact.Alias, (object) 256));
      if (!ArtifactValidations.IsValidAliasName(artifact.Alias))
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.ArtifactAliasHasInvalidCharacters, (object) artifact.Alias));
    }

    private static bool IsValidFolderName(string name) => !FileSpec.IsReservedName(name);

    private static bool CompareInputValues(InputValue first, InputValue second)
    {
      if (string.IsNullOrEmpty(first.Value) && !string.IsNullOrEmpty(second.Value) || !string.IsNullOrEmpty(first.Value) && string.IsNullOrEmpty(second.Value) || string.IsNullOrEmpty(first.DisplayValue) && !string.IsNullOrEmpty(second.DisplayValue) || !string.IsNullOrEmpty(first.DisplayValue) && string.IsNullOrEmpty(second.DisplayValue) || !string.Equals(first.Value, second.Value, StringComparison.OrdinalIgnoreCase) || !string.Equals(first.DisplayValue, second.DisplayValue, StringComparison.OrdinalIgnoreCase) || first.Data == null && second.Data != null || first.Data != null && second.Data == null)
        return false;
      return first.Data == null || second.Data == null || !first.Data.Except<KeyValuePair<string, object>>((IEnumerable<KeyValuePair<string, object>>) second.Data).Concat<KeyValuePair<string, object>>(second.Data.Except<KeyValuePair<string, object>>((IEnumerable<KeyValuePair<string, object>>) first.Data)).Any<KeyValuePair<string, object>>();
    }

    private static void CheckEndpointPermissionAndExistence(
      IVssRequestContext tfsRequestContext,
      Guid projectId,
      Dictionary<Guid, Tuple<List<string>, string>> endpointArtifactsMapForPermissionAndExistenceCheck,
      Dictionary<Guid, Tuple<List<string>, string>> endpointArtifactsMapForOnlyExistenceCheck)
    {
      List<Guid> endpointIds = new List<Guid>();
      endpointIds.AddRange(endpointArtifactsMapForPermissionAndExistenceCheck.Select<KeyValuePair<Guid, Tuple<List<string>, string>>, Guid>((Func<KeyValuePair<Guid, Tuple<List<string>, string>>, Guid>) (kvp => kvp.Key)));
      endpointIds.AddRange(endpointArtifactsMapForOnlyExistenceCheck.Select<KeyValuePair<Guid, Tuple<List<string>, string>>, Guid>((Func<KeyValuePair<Guid, Tuple<List<string>, string>>, Guid>) (kvp => kvp.Key)));
      List<ServiceEndpoint> source1 = tfsRequestContext.GetService<IServiceEndpointService2>().QueryServiceEndpoints(tfsRequestContext, projectId, (string) null, (IEnumerable<string>) null, (IEnumerable<Guid>) endpointIds, (string) null, true);
      IEnumerable<Guid> source2 = source1.Select<ServiceEndpoint, Guid>((Func<ServiceEndpoint, Guid>) (endpoint => endpoint.Id));
      List<ServiceEndpoint> list = source1.Where<ServiceEndpoint>((Func<ServiceEndpoint, bool>) (endpoint => endpoint.Url != (Uri) null)).ToList<ServiceEndpoint>();
      if (endpointArtifactsMapForPermissionAndExistenceCheck.Any<KeyValuePair<Guid, Tuple<List<string>, string>>>())
      {
        List<Guid> unavailableEndpointIds1 = new List<Guid>();
        List<Guid> unavailableEndpointIds2 = new List<Guid>();
        List<Guid> unavailableEndpointIds3 = new List<Guid>();
        List<Guid> unavailableEndpointIds4 = new List<Guid>();
        foreach (Guid key in endpointArtifactsMapForPermissionAndExistenceCheck.Keys)
        {
          Guid endpointId = key;
          ServiceEndpoint serviceEndpoint = list.FirstOrDefault<ServiceEndpoint>((Func<ServiceEndpoint, bool>) (ep => ep.Id == endpointId));
          if (serviceEndpoint == null)
          {
            if (source2.Contains<Guid>(endpointId))
              unavailableEndpointIds3.Add(endpointId);
            else
              unavailableEndpointIds4.Add(endpointId);
          }
          else if (!serviceEndpoint.IsReady)
            unavailableEndpointIds1.Add(serviceEndpoint.Id);
          else if (serviceEndpoint.IsDisabled)
            unavailableEndpointIds2.Add(serviceEndpoint.Id);
        }
        if (unavailableEndpointIds1.Count > 0)
          ArtifactValidations.ThrowUnavailableEndpointsException(endpointArtifactsMapForPermissionAndExistenceCheck, unavailableEndpointIds1, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.ServiceEndpointOfArtifactInDirtyState);
        if (unavailableEndpointIds2.Count > 0)
          ArtifactValidations.ThrowUnavailableEndpointsException(endpointArtifactsMapForPermissionAndExistenceCheck, unavailableEndpointIds2, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.ServiceEndpointOfArtifactInDisabledState);
        if (unavailableEndpointIds3.Count > 0)
          ArtifactValidations.ThrowUnavailableEndpointsException(endpointArtifactsMapForPermissionAndExistenceCheck, unavailableEndpointIds3, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.ServiceEndpointOfArtifactNotAccessible);
        if (unavailableEndpointIds4.Count > 0)
          ArtifactValidations.ThrowUnavailableEndpointsException(endpointArtifactsMapForPermissionAndExistenceCheck, unavailableEndpointIds4, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.ServiceEndpointOfArtifactNotFound);
      }
      if (!endpointArtifactsMapForOnlyExistenceCheck.Any<KeyValuePair<Guid, Tuple<List<string>, string>>>())
        return;
      List<Guid> guidList = new List<Guid>();
      foreach (Guid key in endpointArtifactsMapForOnlyExistenceCheck.Keys)
      {
        if (!source2.Contains<Guid>(key))
          guidList.Add(key);
      }
      if (!guidList.Any<Guid>())
        return;
      ArtifactValidations.ThrowUnavailableEndpointsException(endpointArtifactsMapForOnlyExistenceCheck, guidList, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.ServiceEndpointOfArtifactNotFound);
    }

    private static void ThrowUnavailableEndpointsException(
      Dictionary<Guid, Tuple<List<string>, string>> endpointArtifactsMap,
      List<Guid> unavailableEndpointIds,
      string exceptionMessage)
    {
      List<string> list = unavailableEndpointIds.Select<Guid, string>((Func<Guid, string>) (endpointId => string.Format((IFormatProvider) CultureInfo.InvariantCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.ArtifactToEndpointDetails, (object) endpointArtifactsMap[endpointId].Item2, (object) string.Join(", ", (IEnumerable<string>) endpointArtifactsMap[endpointId].Item1)))).ToList<string>();
      throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, exceptionMessage, (object) string.Join(", ", (IEnumerable<string>) list)));
    }
  }
}
