// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Server.ExtensionDataService
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.Server.Components;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Server
{
  public class ExtensionDataService : IVssFrameworkService
  {
    private const string c_defaultScopeType = "Default";
    private const string c_userScopeType = "User";
    private static readonly Guid c_defaultScopeTypeId = Guid.Parse("4CB9C8AE-BB2F-4D78-8DB7-7E48C5E7E67F");
    private static readonly Guid c_userScopeTypeId = Guid.Parse("825A727F-C9D9-4303-87D3-CEE0A8C2273E");
    private const string c_meScopeValue = "Me";
    private const string c_currentScopeValue = "Current";
    private const int m_maxNumberOfRecords = 3000000;

    public void ServiceStart(IVssRequestContext requestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public List<JObject> GetDocuments(
      IVssRequestContext requestContext,
      string collectionName,
      string scopeType,
      string scopeValue,
      string publisherName,
      string extensionName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(collectionName, nameof (collectionName));
      ArgumentUtility.CheckStringForNullOrEmpty(scopeType, nameof (scopeType));
      this.CheckExtensionAuthorization(requestContext, publisherName, extensionName);
      Guid guid = ExtensionDataService.CheckScopeType(requestContext, scopeType);
      scopeValue = ExtensionDataService.CheckScopeValue(requestContext, guid, scopeValue);
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      int maxNumberOfRecords = vssRequestContext.GetService<IVssRegistryService>().GetValue<int>(vssRequestContext, (RegistryQuery) "/Configuration/ExtensionService/MaxNumberOfRecords", 3000000);
      List<ExtensionDataDocument> documents1;
      using (ExtensionDataComponent component = requestContext.CreateComponent<ExtensionDataComponent>())
        documents1 = component.GetDocuments(collectionName, guid, Encoding.UTF8.GetBytes(scopeValue), maxNumberOfRecords, publisherName, extensionName);
      List<JObject> documents2 = new List<JObject>(documents1.Count);
      int documentSize = 0;
      foreach (ExtensionDataDocument edd in documents1)
      {
        int num = documentSize;
        string documentValue = edd.DocumentValue;
        int length = documentValue != null ? documentValue.Length : 0;
        documentSize = num + length;
        documents2.Add(ExtensionDataService.CreateJObjectFromExtensionDataDocument(edd));
      }
      ExtensionDataService.PublishCiEvent(requestContext, nameof (GetDocuments), publisherName, extensionName, collectionName, scopeType, scopeValue, collectionSize: documents2.Count, documentSize: documentSize);
      return documents2;
    }

    public IEnumerable<string> GetDocumentsAsString(
      IVssRequestContext requestContext,
      string collectionName,
      string scopeType,
      string scopeValue,
      string publisherName,
      string extensionName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(collectionName, nameof (collectionName));
      ArgumentUtility.CheckStringForNullOrEmpty(scopeType, nameof (scopeType));
      this.CheckExtensionAuthorization(requestContext, publisherName, extensionName);
      Guid guid = ExtensionDataService.CheckScopeType(requestContext, scopeType);
      scopeValue = ExtensionDataService.CheckScopeValue(requestContext, guid, scopeValue);
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      int maxNumberOfRecords = vssRequestContext.GetService<IVssRegistryService>().GetValue<int>(vssRequestContext, (RegistryQuery) "/Configuration/ExtensionService/MaxNumberOfRecords", 3000000);
      List<ExtensionDataDocument> edds;
      using (ExtensionDataComponent component = requestContext.CreateComponent<ExtensionDataComponent>())
        edds = component.GetDocuments(collectionName, guid, Encoding.UTF8.GetBytes(scopeValue), maxNumberOfRecords, publisherName, extensionName);
      int docSize = 0;
      if (edds.Count > 0)
      {
        foreach (ExtensionDataDocument extensionDataDocument in edds)
        {
          if (extensionDataDocument.DocumentValue != null)
          {
            docSize += extensionDataDocument.DocumentValue.Length;
            yield return extensionDataDocument.DocumentValue;
          }
        }
        ExtensionDataService.PublishCiEvent(requestContext, "GetDocuments", publisherName, extensionName, collectionName, scopeType, scopeValue, collectionSize: edds.Count, documentSize: docSize);
      }
    }

    public JObject GetDocument(
      IVssRequestContext requestContext,
      string collectionName,
      string scopeType,
      string scopeValue,
      string documentId,
      string publisherName,
      string extensionName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(collectionName, nameof (collectionName));
      ArgumentUtility.CheckStringForNullOrEmpty(scopeType, nameof (scopeType));
      ArgumentUtility.CheckStringForNullOrEmpty(documentId, nameof (documentId));
      this.CheckExtensionAuthorization(requestContext, publisherName, extensionName);
      Guid guid = ExtensionDataService.CheckScopeType(requestContext, scopeType);
      scopeValue = ExtensionDataService.CheckScopeValue(requestContext, guid, scopeValue);
      ExtensionDataDocument document;
      using (ExtensionDataComponent component = requestContext.CreateComponent<ExtensionDataComponent>())
        document = component.GetDocument(collectionName, guid, Encoding.UTF8.GetBytes(scopeValue), Encoding.UTF8.GetBytes(documentId), publisherName, extensionName);
      if (document == null)
        throw new DocumentDoesNotExistException(collectionName, scopeType, scopeValue, documentId);
      IVssRequestContext requestContext1 = requestContext;
      string publisherName1 = publisherName;
      string extensionName1 = extensionName;
      string collectionName1 = collectionName;
      string scopeType1 = scopeType;
      string scopeValue1 = scopeValue;
      string documentId1 = documentId;
      string documentValue = document.DocumentValue;
      int length = documentValue != null ? documentValue.Length : 0;
      ExtensionDataService.PublishCiEvent(requestContext1, nameof (GetDocument), publisherName1, extensionName1, collectionName1, scopeType1, scopeValue1, documentId1, documentSize: length);
      return ExtensionDataService.CreateJObjectFromExtensionDataDocument(document);
    }

    public JObject SetDocument(
      IVssRequestContext requestContext,
      string collectionName,
      string scopeType,
      string scopeValue,
      JObject document,
      string publisherName,
      string extensionName,
      long? bodySize = null)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(collectionName, nameof (collectionName));
      ArgumentUtility.CheckStringForNullOrEmpty(scopeType, nameof (scopeType));
      ArgumentUtility.CheckForNull<JObject>(document, nameof (document));
      this.CheckExtensionAuthorization(requestContext, publisherName, extensionName);
      this.CheckCollectionName(collectionName);
      Guid guid = ExtensionDataService.CheckScopeType(requestContext, scopeType);
      scopeValue = ExtensionDataService.CheckScopeValue(requestContext, guid, scopeValue);
      int andUpdateEtag = ExtensionDataService.GetAndUpdateEtag(document);
      string idFromDocument = ExtensionDataService.GetIdFromDocument(document);
      this.SanitizeDocument(document);
      string documentAsString = this.GetDocumentAsString(requestContext, document, bodySize);
      ExtensionDataDocument edd;
      using (ExtensionDataComponent component = requestContext.CreateComponent<ExtensionDataComponent>())
        edd = component.SetDocument(collectionName, guid, Encoding.UTF8.GetBytes(scopeValue), Encoding.UTF8.GetBytes(idFromDocument), documentAsString, andUpdateEtag, publisherName, extensionName);
      IVssRequestContext requestContext1 = requestContext;
      string publisherName1 = publisherName;
      string extensionName1 = extensionName;
      string collectionName1 = collectionName;
      string scopeType1 = scopeType;
      string scopeValue1 = scopeValue;
      string documentId = idFromDocument;
      string documentValue = edd.DocumentValue;
      int length = documentValue != null ? documentValue.Length : 0;
      ExtensionDataService.PublishCiEvent(requestContext1, nameof (SetDocument), publisherName1, extensionName1, collectionName1, scopeType1, scopeValue1, documentId, documentSize: length);
      return ExtensionDataService.CreateJObjectFromExtensionDataDocument(edd);
    }

    public JObject CreateDocument(
      IVssRequestContext requestContext,
      string collectionName,
      string scopeType,
      string scopeValue,
      JObject document,
      string publisherName,
      string extensionName,
      long? bodySize = null)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(collectionName, nameof (collectionName));
      ArgumentUtility.CheckStringForNullOrEmpty(scopeType, nameof (scopeType));
      ArgumentUtility.CheckForNull<JObject>(document, nameof (document));
      this.CheckExtensionAuthorization(requestContext, publisherName, extensionName);
      this.CheckCollectionName(collectionName);
      Guid guid = ExtensionDataService.CheckScopeType(requestContext, scopeType);
      scopeValue = ExtensionDataService.CheckScopeValue(requestContext, guid, scopeValue);
      string idFromDocument = ExtensionDataService.GetIdFromDocument(document, validateId: true);
      document["__etag"] = (JToken) 1;
      this.SanitizeDocument(document);
      string documentAsString = this.GetDocumentAsString(requestContext, document, bodySize);
      ExtensionDataDocument document1;
      using (ExtensionDataComponent component = requestContext.CreateComponent<ExtensionDataComponent>())
        document1 = component.CreateDocument(collectionName, guid, Encoding.UTF8.GetBytes(scopeValue), Encoding.UTF8.GetBytes(idFromDocument), documentAsString, publisherName, extensionName);
      IVssRequestContext requestContext1 = requestContext;
      string publisherName1 = publisherName;
      string extensionName1 = extensionName;
      string collectionName1 = collectionName;
      string scopeType1 = scopeType;
      string scopeValue1 = scopeValue;
      string documentId = idFromDocument;
      string documentValue = document1.DocumentValue;
      int length = documentValue != null ? documentValue.Length : 0;
      ExtensionDataService.PublishCiEvent(requestContext1, nameof (CreateDocument), publisherName1, extensionName1, collectionName1, scopeType1, scopeValue1, documentId, documentSize: length);
      return ExtensionDataService.CreateJObjectFromExtensionDataDocument(document1);
    }

    public JObject UpdateDocument(
      IVssRequestContext requestContext,
      string collectionName,
      string scopeType,
      string scopeValue,
      JObject document,
      string publisherName,
      string extensionName,
      long? bodySize = null)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(collectionName, nameof (collectionName));
      ArgumentUtility.CheckStringForNullOrEmpty(scopeType, nameof (scopeType));
      ArgumentUtility.CheckForNull<JObject>(document, nameof (document));
      this.CheckExtensionAuthorization(requestContext, publisherName, extensionName);
      this.CheckCollectionName(collectionName);
      Guid guid = ExtensionDataService.CheckScopeType(requestContext, scopeType);
      scopeValue = ExtensionDataService.CheckScopeValue(requestContext, guid, scopeValue);
      int andUpdateEtag = ExtensionDataService.GetAndUpdateEtag(document, true);
      string idFromDocument = ExtensionDataService.GetIdFromDocument(document, true);
      this.SanitizeDocument(document);
      string documentAsString = this.GetDocumentAsString(requestContext, document, bodySize);
      ExtensionDataDocument edd;
      using (ExtensionDataComponent component = requestContext.CreateComponent<ExtensionDataComponent>())
        edd = component.UpdateDocument(collectionName, guid, Encoding.UTF8.GetBytes(scopeValue), Encoding.UTF8.GetBytes(idFromDocument), documentAsString, andUpdateEtag, publisherName, extensionName);
      IVssRequestContext requestContext1 = requestContext;
      string publisherName1 = publisherName;
      string extensionName1 = extensionName;
      string collectionName1 = collectionName;
      string scopeType1 = scopeType;
      string scopeValue1 = scopeValue;
      string documentId = idFromDocument;
      string documentValue = edd.DocumentValue;
      int length = documentValue != null ? documentValue.Length : 0;
      ExtensionDataService.PublishCiEvent(requestContext1, nameof (UpdateDocument), publisherName1, extensionName1, collectionName1, scopeType1, scopeValue1, documentId, documentSize: length);
      return ExtensionDataService.CreateJObjectFromExtensionDataDocument(edd);
    }

    public void DeleteDocument(
      IVssRequestContext requestContext,
      string collectionName,
      string scopeType,
      string scopeValue,
      string documentId,
      string publisherName,
      string extensionName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(collectionName, nameof (collectionName));
      ArgumentUtility.CheckStringForNullOrEmpty(scopeType, nameof (scopeType));
      ArgumentUtility.CheckStringForNullOrEmpty(documentId, nameof (documentId));
      this.CheckExtensionAuthorization(requestContext, publisherName, extensionName);
      Guid guid = ExtensionDataService.CheckScopeType(requestContext, scopeType);
      scopeValue = ExtensionDataService.CheckScopeValue(requestContext, guid, scopeValue);
      using (ExtensionDataComponent component = requestContext.CreateComponent<ExtensionDataComponent>())
        component.DeleteDocument(collectionName, guid, Encoding.UTF8.GetBytes(scopeValue), Encoding.UTF8.GetBytes(documentId), publisherName, extensionName);
      ExtensionDataService.PublishCiEvent(requestContext, nameof (DeleteDocument), publisherName, extensionName, collectionName, scopeType, scopeValue, documentId);
    }

    public List<ExtensionDataCollection> QueryCollections(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      ExtensionDataCollectionQuery collectionQuery)
    {
      ArgumentUtility.CheckForNull<ExtensionDataCollectionQuery>(collectionQuery, "CollectionQuery");
      this.CheckExtensionAuthorization(requestContext, publisherName, extensionName);
      List<ExtensionDataCollection> extensionDataCollectionList = new List<ExtensionDataCollection>();
      foreach (ExtensionDataCollection collection in collectionQuery.Collections)
      {
        List<JObject> jobjectList = (List<JObject>) null;
        try
        {
          jobjectList = this.GetDocuments(requestContext, collection.CollectionName, collection.ScopeType, collection.ScopeValue, publisherName, extensionName);
        }
        catch (DocumentCollectionDoesNotExistException ex)
        {
        }
        collection.Documents = jobjectList;
        extensionDataCollectionList.Add(collection);
      }
      return extensionDataCollectionList;
    }

    private void CheckExtensionAuthorization(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName)
    {
      if (requestContext.IsFeatureEnabled(ExtensionManagementFeatureFlags.CheckHostAuthorization) && !requestContext.GetService<IExtensionHostAuthorizationService>().IsRequestAuthorized(requestContext, publisherName, extensionName))
        throw new ExtensionInvalidAuthorizationException(publisherName, extensionName);
    }

    private void CheckCollectionName(string collectionName)
    {
      if (collectionName.Length > 100)
        throw new ArgumentException(ExtensionResources.CollectionNameExceedsLimit((object) collectionName.Length, (object) 100));
    }

    private void SanitizeDocument(JObject document)
    {
      foreach (string propertyName in document.Properties().Where<JProperty>((Func<JProperty, bool>) (p => p.Name.StartsWith("__") && !p.Name.Equals("__etag"))).Select<JProperty, string>((Func<JProperty, string>) (p => p.Name)).ToList<string>())
        document.Remove(propertyName);
    }

    private void CheckMaximumDocumentSize(IVssRequestContext requestContext, long documentSize)
    {
      int maxDocumentSize = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) ExtensionManagementConstants.ExtensionDataMaxDocumentSize, 5244928);
      if (documentSize > (long) maxDocumentSize)
        throw new MaximumDocumentSizeException(maxDocumentSize, documentSize);
    }

    private string GetDocumentAsString(
      IVssRequestContext requestContext,
      JObject document,
      long? bodySize)
    {
      if (bodySize.HasValue)
      {
        this.CheckMaximumDocumentSize(requestContext, bodySize.Value);
        return document.ToString();
      }
      string documentAsString = document.ToString();
      this.CheckMaximumDocumentSize(requestContext, (long) documentAsString.Length);
      return documentAsString;
    }

    private static Guid CheckScopeType(IVssRequestContext requestContext, string scopeType)
    {
      if (scopeType.Equals("Default", StringComparison.OrdinalIgnoreCase))
        return ExtensionDataService.c_defaultScopeTypeId;
      if (scopeType.Equals("User", StringComparison.OrdinalIgnoreCase))
        return ExtensionDataService.c_userScopeTypeId;
      throw new ArgumentException(nameof (scopeType));
    }

    private static string CheckScopeValue(
      IVssRequestContext requestContext,
      Guid scopeTypeId,
      string scopeValue)
    {
      if (scopeTypeId.Equals(ExtensionDataService.c_defaultScopeTypeId))
      {
        if (string.IsNullOrEmpty(scopeValue) || scopeValue.Equals("Current", StringComparison.OrdinalIgnoreCase))
          return "Current";
      }
      else if (scopeTypeId.Equals(ExtensionDataService.c_userScopeTypeId) && (string.IsNullOrEmpty(scopeValue) || scopeValue.Equals("Me", StringComparison.OrdinalIgnoreCase)))
        return requestContext.GetUserId().ToString();
      throw new ArgumentException(nameof (scopeValue));
    }

    private static JObject CreateJObjectFromExtensionDataDocument(ExtensionDataDocument edd)
    {
      JObject extensionDataDocument = JObject.Parse(edd.DocumentValue);
      extensionDataDocument["__etag"] = (JToken) edd.Version;
      return extensionDataDocument;
    }

    private static string GetIdFromDocument(JObject document, bool throwIfNoId = false, bool validateId = false)
    {
      JToken jtoken = document["id"];
      string id;
      if (jtoken != null)
      {
        id = jtoken.ToString();
        if (string.IsNullOrEmpty(id))
          throw new ArgumentNullException("document.id");
        if (validateId && ExtensionDataService.IsInvalidId(id))
          throw new ArgumentException("Document id contais invalid char(s)");
      }
      else
      {
        if (throwIfNoId)
          throw new ArgumentNullException("Document must contain id");
        id = Guid.NewGuid().ToString();
        document["id"] = (JToken) id;
      }
      return id;
    }

    private static bool IsInvalidId(string id) => id.IndexOfAny(Path.GetInvalidFileNameChars()) != -1;

    private static int GetAndUpdateEtag(JObject document, bool throwIfNoEtag = false)
    {
      int result;
      if (document["__etag"] == null || !int.TryParse(document["__etag"].ToString(), out result))
      {
        if (throwIfNoEtag)
          throw new ArgumentNullException("Document must contain integer __etag.");
        result = 1;
        document["__etag"] = (JToken) 1;
      }
      else if (result != -1)
        document["__etag"] = (JToken) (result + 1);
      return result;
    }

    private static void PublishCiEvent(
      IVssRequestContext requestContext,
      string action,
      string publisherName,
      string extensionName,
      string collectionName,
      string scopeType,
      string scopeValue,
      string documentId = null,
      int collectionSize = -1,
      int documentSize = -1)
    {
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add(CustomerIntelligenceProperty.Action, action);
      properties.Add("CollectionName", collectionName);
      properties.Add("PublisherName", publisherName);
      properties.Add("ExtensionName", extensionName);
      properties.Add("ScopeType", scopeType);
      if (collectionSize != -1)
        properties.Add("DocumentCount", (double) collectionSize);
      if (documentSize != -1)
        properties.Add("DocumentSize", (double) documentSize);
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, CustomerIntelligenceArea.Contributions, ExtensionManagementCustomerIntelligenceFeature.ExtensionManagementData, properties);
    }
  }
}
