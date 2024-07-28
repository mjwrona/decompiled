// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.WorkItemUpdateModelBinder
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 74AD14A4-225D-46D2-B154-945941A2D167
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.dll

using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;
using System.Web.Script.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking
{
  public class WorkItemUpdateModelBinder : JsonModelBinder
  {
    public override JsonConverter[] GetConverters() => (JsonConverter[]) new JsonConverterWithCallbacks<WorkItemUpdate>[1]
    {
      new JsonConverterWithCallbacks<WorkItemUpdate>(WorkItemUpdateModelBinder.\u003C\u003EO.\u003C0\u003E__DeserializeJson ?? (WorkItemUpdateModelBinder.\u003C\u003EO.\u003C0\u003E__DeserializeJson = new JsonDeserializationCallback<WorkItemUpdate>(WorkItemUpdateModelBinder.DeserializeJson)))
    };

    public static WorkItemUpdate DeserializeJson(IDictionary<string, object> dictionary)
    {
      if (dictionary == null)
        return (WorkItemUpdate) null;
      try
      {
        int sourceId = 0;
        if (dictionary.ContainsKey("id") && dictionary["id"] != null)
          sourceId = int.Parse(dictionary["id"].ToString());
        if (sourceId == 0 && dictionary.ContainsKey("tempId") && dictionary["tempId"] != null)
          sourceId = int.Parse(dictionary["tempId"].ToString());
        int num = 0;
        if (dictionary.ContainsKey("rev") && dictionary["rev"] != null)
          num = int.Parse(dictionary["rev"].ToString());
        IDictionary<string, object> source = (IDictionary<string, object>) null;
        if (dictionary.ContainsKey("fields") && dictionary["fields"] != null)
          source = JsonConvert.DeserializeObject<IDictionary<string, object>>(dictionary["fields"].ToString());
        if (source != null)
        {
          foreach (KeyValuePair<string, object> keyValuePair in source.Where<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>) (pair => pair.Value?.GetType() == typeof (JObject))).Select<KeyValuePair<string, object>, KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, KeyValuePair<string, object>>) (p => JObject.FromObject((object) p).ToObject<KeyValuePair<string, object>>())).ToArray<KeyValuePair<string, object>>())
          {
            IDictionary<string, object> witIdentityProperties = JObject.FromObject(keyValuePair.Value).ToObject<IDictionary<string, object>>();
            object obj;
            object commentText;
            object format;
            source[keyValuePair.Key] = !witIdentityProperties.TryGetValue("type", out obj) ? (!witIdentityProperties.TryGetValue("text", out commentText) ? WorkItemUpdateModelBinder.DeserializeWitIdentityRef(witIdentityProperties) : (!witIdentityProperties.TryGetValue("format", out format) ? (object) (commentText as string) : (object) new WorkItemCommentUpdate(format as string, commentText as string))) : (object) new ServerDefaultFieldValue((ServerDefaultType) Convert.ToInt32(obj));
          }
        }
        else
          source = (IDictionary<string, object>) new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        List<WorkItemLinkUpdate> linkUpdates = new List<WorkItemLinkUpdate>();
        List<WorkItemResourceLinkUpdate> resourceLinkUpdates = new List<WorkItemResourceLinkUpdate>();
        if (dictionary.ContainsKey("links"))
        {
          IDictionary<string, object> dictionary1 = JsonConvert.DeserializeObject<IDictionary<string, object>>(dictionary["links"].ToString());
          if (dictionary1 != null)
          {
            object obj;
            if (dictionary1.TryGetValue("addedLinks", out obj))
            {
              IEnumerable<IDictionary<string, object>> links = JsonConvert.DeserializeObject<IEnumerable<IDictionary<string, object>>>(obj.ToString());
              if (links != null)
                WorkItemUpdateModelBinder.PopulateLinkUpdates(links, LinkUpdateType.Add, sourceId, linkUpdates, resourceLinkUpdates);
            }
            if (dictionary1.TryGetValue("updatedLinks", out obj))
            {
              IEnumerable<IDictionary<string, object>> links = JsonConvert.DeserializeObject<IEnumerable<IDictionary<string, object>>>(obj.ToString());
              if (links != null)
                WorkItemUpdateModelBinder.PopulateLinkUpdates(links, LinkUpdateType.Update, sourceId, linkUpdates, resourceLinkUpdates);
            }
            if (dictionary1.TryGetValue("deletedLinks", out obj))
            {
              IEnumerable<IDictionary<string, object>> links = JsonConvert.DeserializeObject<IEnumerable<IDictionary<string, object>>>(obj.ToString());
              if (links != null)
                WorkItemUpdateModelBinder.PopulateLinkUpdates(links, LinkUpdateType.Delete, sourceId, linkUpdates, resourceLinkUpdates);
            }
          }
        }
        return new WorkItemUpdate()
        {
          Id = sourceId,
          Rev = num,
          Fields = (IEnumerable<KeyValuePair<string, object>>) source,
          LinkUpdates = (IEnumerable<WorkItemLinkUpdate>) linkUpdates,
          ResourceLinkUpdates = (IEnumerable<WorkItemResourceLinkUpdate>) resourceLinkUpdates
        };
      }
      catch (Exception ex)
      {
        switch (ex)
        {
          case InvalidCastException _:
          case FormatException _:
            throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Invalid JavaScript Object : {0}", (object) ex.Message), ex);
          default:
            throw;
        }
      }
    }

    private static object DeserializeWitIdentityRef(
      IDictionary<string, object> witIdentityProperties)
    {
      string str1 = (string) null;
      object obj;
      witIdentityProperties.TryGetValue("distinctDisplayName", out obj);
      if (obj != null)
        str1 = obj.ToString();
      object o;
      witIdentityProperties.TryGetValue("identityRef", out o);
      if (o != null)
      {
        IDictionary<string, object> dictionary = JObject.FromObject(o).ToObject<IDictionary<string, object>>();
        string str2 = (string) null;
        string subjectDescriptorString = (string) null;
        if (dictionary.ContainsKey("displayName"))
          str2 = dictionary["displayName"]?.ToString();
        if (dictionary.ContainsKey("displayName"))
          subjectDescriptorString = dictionary["descriptor"]?.ToString();
        return (object) new WitIdentityRef()
        {
          DistinctDisplayName = str1,
          IdentityRef = new IdentityRef()
          {
            Descriptor = SubjectDescriptor.FromString(subjectDescriptorString),
            DisplayName = str2
          }
        };
      }
      return (object) new WitIdentityRef()
      {
        DistinctDisplayName = str1,
        IdentityRef = new IdentityRef()
        {
          Descriptor = new SubjectDescriptor(),
          DisplayName = str1
        }
      };
    }

    private static void PopulateLinkUpdates(
      IEnumerable<IDictionary<string, object>> links,
      LinkUpdateType linkUpdateType,
      int sourceId,
      List<WorkItemLinkUpdate> linkUpdates,
      List<WorkItemResourceLinkUpdate> resourceLinkUpdates)
    {
      foreach (IDictionary<string, object> link in links)
      {
        int valueFromDictionary1 = WorkItemUpdateModelBinder.GetValueFromDictionary<int>(link, "FldID");
        string valueFromDictionary2 = WorkItemUpdateModelBinder.GetValueFromDictionary<string>(link, "Comment", true);
        switch (valueFromDictionary1)
        {
          case 37:
            int valueFromDictionary3 = WorkItemUpdateModelBinder.GetValueFromDictionary<int>(link, "ID");
            int valueFromDictionary4 = WorkItemUpdateModelBinder.GetValueFromDictionary<int>(link, "LinkType", true);
            Guid result1;
            Guid.TryParse(WorkItemUpdateModelBinder.GetValueFromDictionary<string>(link, "RemoteHostId", true), out result1);
            Guid result2;
            Guid.TryParse(WorkItemUpdateModelBinder.GetValueFromDictionary<string>(link, "RemoteProjectId", true), out result2);
            bool? nullable = new bool?();
            switch (linkUpdateType)
            {
              case LinkUpdateType.Add:
              case LinkUpdateType.Update:
                if (link.ContainsKey("Lock"))
                {
                  nullable = new bool?(WorkItemUpdateModelBinder.GetValueFromDictionary<bool>(link, "Lock"));
                  break;
                }
                break;
            }
            List<WorkItemLinkUpdate> workItemLinkUpdateList = linkUpdates;
            WorkItemLinkUpdate workItemLinkUpdate = new WorkItemLinkUpdate();
            workItemLinkUpdate.SourceWorkItemId = sourceId;
            workItemLinkUpdate.TargetWorkItemId = valueFromDictionary3;
            workItemLinkUpdate.UpdateType = linkUpdateType;
            workItemLinkUpdate.LinkType = valueFromDictionary4;
            workItemLinkUpdate.Locked = nullable;
            workItemLinkUpdate.Comment = valueFromDictionary2;
            workItemLinkUpdate.RemoteHostId = result1 == Guid.Empty ? new Guid?() : new Guid?(result1);
            workItemLinkUpdate.RemoteProjectId = result2 == Guid.Empty ? new Guid?() : new Guid?(result2);
            workItemLinkUpdateList.Add(workItemLinkUpdate);
            continue;
          case 50:
            if (linkUpdateType == LinkUpdateType.Add)
            {
              string valueFromDictionary5 = WorkItemUpdateModelBinder.GetValueFromDictionary<string>(link, "FilePath");
              string valueFromDictionary6 = WorkItemUpdateModelBinder.GetValueFromDictionary<string>(link, "OriginalName");
              DateTime fromPossibleString1 = WorkItemUpdateModelBinder.GetDateTimeFromPossibleString(link, "CreationDate");
              DateTime fromPossibleString2 = WorkItemUpdateModelBinder.GetDateTimeFromPossibleString(link, "LastWriteDate");
              int valueFromDictionary7 = WorkItemUpdateModelBinder.GetValueFromDictionary<int>(link, "Length");
              List<WorkItemResourceLinkUpdate> resourceLinkUpdateList = resourceLinkUpdates;
              WorkItemResourceLinkUpdate resourceLinkUpdate = new WorkItemResourceLinkUpdate();
              resourceLinkUpdate.SourceWorkItemId = sourceId;
              resourceLinkUpdate.Type = new ResourceLinkType?(ResourceLinkType.Attachment);
              resourceLinkUpdate.UpdateType = linkUpdateType;
              resourceLinkUpdate.Location = valueFromDictionary5;
              resourceLinkUpdate.Name = valueFromDictionary6;
              resourceLinkUpdate.CreationDate = new DateTime?(fromPossibleString1);
              resourceLinkUpdate.LastModifiedDate = new DateTime?(fromPossibleString2);
              resourceLinkUpdate.Length = new int?(valueFromDictionary7);
              resourceLinkUpdate.Comment = valueFromDictionary2;
              resourceLinkUpdateList.Add(resourceLinkUpdate);
              continue;
            }
            int valueFromDictionary8 = WorkItemUpdateModelBinder.GetValueFromDictionary<int>(link, "ExtID");
            List<WorkItemResourceLinkUpdate> resourceLinkUpdateList1 = resourceLinkUpdates;
            WorkItemResourceLinkUpdate resourceLinkUpdate1 = new WorkItemResourceLinkUpdate();
            resourceLinkUpdate1.SourceWorkItemId = sourceId;
            resourceLinkUpdate1.ResourceId = new int?(valueFromDictionary8);
            resourceLinkUpdate1.Type = new ResourceLinkType?(ResourceLinkType.Attachment);
            resourceLinkUpdate1.UpdateType = linkUpdateType;
            resourceLinkUpdate1.Comment = valueFromDictionary2;
            resourceLinkUpdateList1.Add(resourceLinkUpdate1);
            continue;
          case 51:
            List<WorkItemResourceLinkUpdate> resourceLinkUpdateList2 = resourceLinkUpdates;
            WorkItemResourceLinkUpdate resourceLinkUpdate2 = new WorkItemResourceLinkUpdate();
            resourceLinkUpdate2.SourceWorkItemId = sourceId;
            resourceLinkUpdate2.ResourceId = WorkItemUpdateModelBinder.GetValueFromDictionary<int?>(link, "ExtID", true);
            resourceLinkUpdate2.Type = new ResourceLinkType?(ResourceLinkType.Hyperlink);
            resourceLinkUpdate2.UpdateType = linkUpdateType;
            resourceLinkUpdate2.Location = WorkItemUpdateModelBinder.GetValueFromDictionary<string>(link, "FilePath");
            resourceLinkUpdate2.Name = WorkItemUpdateModelBinder.GetValueFromDictionary<string>(link, "OriginalName", true);
            resourceLinkUpdate2.Comment = valueFromDictionary2;
            resourceLinkUpdateList2.Add(resourceLinkUpdate2);
            continue;
          case 58:
            List<WorkItemResourceLinkUpdate> resourceLinkUpdateList3 = resourceLinkUpdates;
            WorkItemResourceLinkUpdate resourceLinkUpdate3 = new WorkItemResourceLinkUpdate();
            resourceLinkUpdate3.SourceWorkItemId = sourceId;
            resourceLinkUpdate3.ResourceId = WorkItemUpdateModelBinder.GetValueFromDictionary<int?>(link, "ExtID", true);
            resourceLinkUpdate3.Type = new ResourceLinkType?(ResourceLinkType.ArtifactLink);
            resourceLinkUpdate3.UpdateType = linkUpdateType;
            resourceLinkUpdate3.Location = WorkItemUpdateModelBinder.GetValueFromDictionary<string>(link, "FilePath");
            resourceLinkUpdate3.Name = WorkItemUpdateModelBinder.GetValueFromDictionary<string>(link, "OriginalName", true);
            resourceLinkUpdate3.Comment = valueFromDictionary2;
            resourceLinkUpdateList3.Add(resourceLinkUpdate3);
            continue;
          default:
            continue;
        }
      }
    }

    private static DateTime GetDateTimeFromPossibleString(
      IDictionary<string, object> dictionary,
      string key)
    {
      try
      {
        return WorkItemUpdateModelBinder.GetValueFromDictionary<DateTime>(dictionary, key);
      }
      catch (InvalidArgumentValueException ex)
      {
      }
      string valueFromDictionary = WorkItemUpdateModelBinder.GetValueFromDictionary<string>(dictionary, key);
      try
      {
        return DateTime.Parse(valueFromDictionary);
      }
      catch (Exception ex)
      {
        throw new InvalidArgumentValueException(key);
      }
    }

    private static T GetValueFromDictionary<T>(
      IDictionary<string, object> dictionary,
      string key,
      bool optional = false)
    {
      object obj;
      if (dictionary.TryGetValue(key, out obj))
      {
        if (obj == null)
          return WorkItemUpdateModelBinder.GetDefaultValue<T>();
        object valueFromDictionary;
        try
        {
          if (typeof (T) == typeof (int) || typeof (T) == typeof (int?))
          {
            int result;
            return int.TryParse(obj.ToString(), out result) ? (T) (ValueType) result : WorkItemUpdateModelBinder.GetDefaultValue<T>();
          }
          if (typeof (T) == typeof (string))
            return (T) obj.ToString();
          if (typeof (T) == typeof (bool))
            return (T) (ValueType) bool.Parse(obj.ToString());
          valueFromDictionary = (object) (T) obj;
        }
        catch (InvalidCastException ex)
        {
          throw new InvalidArgumentValueException(key);
        }
        return (T) valueFromDictionary;
      }
      if (!optional)
        throw new InvalidArgumentValueException(key);
      return WorkItemUpdateModelBinder.GetDefaultValue<T>();
    }

    private static T GetDefaultValue<T>() => (T) (!(typeof (T) == typeof (DateTime)) ? (object) default (T) : (object) SqlDateTime.MinValue.Value);

    public override JavaScriptConverter[] GetJsConverters() => (JavaScriptConverter[]) new JsonConverterJsSerializerWithCallbacks<WorkItemUpdate>[1]
    {
      new JsonConverterJsSerializerWithCallbacks<WorkItemUpdate>(WorkItemUpdateModelBinder.\u003C\u003EO.\u003C1\u003E__DeserializeJsonWithJSSerializer ?? (WorkItemUpdateModelBinder.\u003C\u003EO.\u003C1\u003E__DeserializeJsonWithJSSerializer = new JsonDeserializationJSCallback<WorkItemUpdate>(WorkItemUpdateModelBinder.DeserializeJsonWithJSSerializer)))
    };

    public static WorkItemUpdate DeserializeJsonWithJSSerializer(
      IDictionary<string, object> dictionary,
      JavaScriptSerializer serializer)
    {
      if (dictionary == null)
        return (WorkItemUpdate) null;
      try
      {
        if (serializer == null)
          serializer = new JavaScriptSerializer();
        int sourceId = 0;
        if (dictionary.ContainsKey("id") && dictionary["id"] != null)
          sourceId = (int) dictionary["id"];
        if (sourceId == 0 && dictionary.ContainsKey("tempId") && dictionary["tempId"] != null)
          sourceId = (int) dictionary["tempId"];
        int num = 0;
        if (dictionary.ContainsKey("rev") && dictionary["rev"] != null)
          num = (int) dictionary["rev"];
        IDictionary<string, object> source = (IDictionary<string, object>) null;
        if (dictionary.ContainsKey("fields"))
          source = dictionary["fields"] as IDictionary<string, object>;
        if (source != null)
        {
          foreach (KeyValuePair<string, object> keyValuePair in source.Where<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>) (pair => pair.Value is IDictionary<string, object>)).ToArray<KeyValuePair<string, object>>())
          {
            IDictionary<string, object> witIdentityProperties = (IDictionary<string, object>) keyValuePair.Value;
            object t;
            if (witIdentityProperties.TryGetValue("type", out t))
            {
              source[keyValuePair.Key] = (object) new ServerDefaultFieldValue((ServerDefaultType) t);
            }
            else
            {
              object format;
              object commentText;
              if (witIdentityProperties.TryGetValue("format", out format) && witIdentityProperties.TryGetValue("text", out commentText))
              {
                WorkItemCommentUpdate itemCommentUpdate = new WorkItemCommentUpdate(format as string, commentText as string);
                source[keyValuePair.Key] = (object) itemCommentUpdate;
              }
              else
                source[keyValuePair.Key] = WorkItemUpdateModelBinder.DeserializeWitIdentityRefJSSerializer(witIdentityProperties);
            }
          }
        }
        else
          source = (IDictionary<string, object>) new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        if (dictionary.ContainsKey("isDirty"))
        {
          object obj1 = dictionary["isDirty"];
        }
        List<WorkItemLinkUpdate> linkUpdates = new List<WorkItemLinkUpdate>();
        List<WorkItemResourceLinkUpdate> resourceLinkUpdates = new List<WorkItemResourceLinkUpdate>();
        if (dictionary.ContainsKey("links") && dictionary["links"] is IDictionary<string, object> dictionary1)
        {
          object obj2;
          if (dictionary1.TryGetValue("addedLinks", out obj2))
          {
            IEnumerable<IDictionary<string, object>> type = serializer.ConvertToType<IEnumerable<IDictionary<string, object>>>(obj2);
            if (type != null)
              WorkItemUpdateModelBinder.PopulateLinkUpdatesJSSerializer(type, LinkUpdateType.Add, sourceId, linkUpdates, resourceLinkUpdates);
          }
          if (dictionary1.TryGetValue("updatedLinks", out obj2))
          {
            IEnumerable<IDictionary<string, object>> type = serializer.ConvertToType<IEnumerable<IDictionary<string, object>>>(obj2);
            if (type != null)
              WorkItemUpdateModelBinder.PopulateLinkUpdatesJSSerializer(type, LinkUpdateType.Update, sourceId, linkUpdates, resourceLinkUpdates);
          }
          if (dictionary1.TryGetValue("deletedLinks", out obj2))
          {
            IEnumerable<IDictionary<string, object>> type = serializer.ConvertToType<IEnumerable<IDictionary<string, object>>>(obj2);
            if (type != null)
              WorkItemUpdateModelBinder.PopulateLinkUpdatesJSSerializer(type, LinkUpdateType.Delete, sourceId, linkUpdates, resourceLinkUpdates);
          }
        }
        return new WorkItemUpdate()
        {
          Id = sourceId,
          Rev = num,
          Fields = (IEnumerable<KeyValuePair<string, object>>) source,
          LinkUpdates = (IEnumerable<WorkItemLinkUpdate>) linkUpdates,
          ResourceLinkUpdates = (IEnumerable<WorkItemResourceLinkUpdate>) resourceLinkUpdates
        };
      }
      catch (InvalidCastException ex)
      {
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Invalid JavaScript Object : {0}", (object) ex.Message), (Exception) ex);
      }
    }

    private static object DeserializeWitIdentityRefJSSerializer(
      IDictionary<string, object> witIdentityProperties)
    {
      string str1;
      witIdentityProperties.TryGetValue<string>("distinctDisplayName", out str1);
      IDictionary<string, object> dictionary;
      witIdentityProperties.TryGetValue<IDictionary<string, object>>("identityRef", out dictionary);
      if (dictionary != null)
      {
        string str2;
        dictionary.TryGetValue<string>("displayName", out str2);
        string subjectDescriptorString;
        dictionary.TryGetValue<string>("descriptor", out subjectDescriptorString);
        return (object) new WitIdentityRef()
        {
          DistinctDisplayName = str1,
          IdentityRef = new IdentityRef()
          {
            Descriptor = SubjectDescriptor.FromString(subjectDescriptorString),
            DisplayName = str2
          }
        };
      }
      return (object) new WitIdentityRef()
      {
        DistinctDisplayName = str1,
        IdentityRef = new IdentityRef()
        {
          Descriptor = new SubjectDescriptor(),
          DisplayName = str1
        }
      };
    }

    private static void PopulateLinkUpdatesJSSerializer(
      IEnumerable<IDictionary<string, object>> links,
      LinkUpdateType linkUpdateType,
      int sourceId,
      List<WorkItemLinkUpdate> linkUpdates,
      List<WorkItemResourceLinkUpdate> resourceLinkUpdates)
    {
      foreach (IDictionary<string, object> link in links)
      {
        int dictionaryJsSerializer1 = WorkItemUpdateModelBinder.GetValueFromDictionaryJSSerializer<int>(link, "FldID");
        string dictionaryJsSerializer2 = WorkItemUpdateModelBinder.GetValueFromDictionaryJSSerializer<string>(link, "Comment", true);
        switch (dictionaryJsSerializer1)
        {
          case 37:
            int dictionaryJsSerializer3 = WorkItemUpdateModelBinder.GetValueFromDictionaryJSSerializer<int>(link, "ID");
            int dictionaryJsSerializer4 = WorkItemUpdateModelBinder.GetValueFromDictionaryJSSerializer<int>(link, "LinkType", true);
            Guid result1;
            Guid.TryParse(WorkItemUpdateModelBinder.GetValueFromDictionaryJSSerializer<string>(link, "RemoteHostId", true), out result1);
            Guid result2;
            Guid.TryParse(WorkItemUpdateModelBinder.GetValueFromDictionaryJSSerializer<string>(link, "RemoteProjectId", true), out result2);
            bool? nullable = new bool?();
            switch (linkUpdateType)
            {
              case LinkUpdateType.Add:
              case LinkUpdateType.Update:
                if (link.ContainsKey("Lock"))
                {
                  nullable = new bool?(WorkItemUpdateModelBinder.GetValueFromDictionaryJSSerializer<bool>(link, "Lock"));
                  break;
                }
                break;
            }
            List<WorkItemLinkUpdate> workItemLinkUpdateList = linkUpdates;
            WorkItemLinkUpdate workItemLinkUpdate = new WorkItemLinkUpdate();
            workItemLinkUpdate.SourceWorkItemId = sourceId;
            workItemLinkUpdate.TargetWorkItemId = dictionaryJsSerializer3;
            workItemLinkUpdate.UpdateType = linkUpdateType;
            workItemLinkUpdate.LinkType = dictionaryJsSerializer4;
            workItemLinkUpdate.Locked = nullable;
            workItemLinkUpdate.Comment = dictionaryJsSerializer2;
            workItemLinkUpdate.RemoteHostId = result1 == Guid.Empty ? new Guid?() : new Guid?(result1);
            workItemLinkUpdate.RemoteProjectId = result2 == Guid.Empty ? new Guid?() : new Guid?(result2);
            workItemLinkUpdateList.Add(workItemLinkUpdate);
            continue;
          case 50:
            if (linkUpdateType == LinkUpdateType.Add)
            {
              string dictionaryJsSerializer5 = WorkItemUpdateModelBinder.GetValueFromDictionaryJSSerializer<string>(link, "FilePath");
              string dictionaryJsSerializer6 = WorkItemUpdateModelBinder.GetValueFromDictionaryJSSerializer<string>(link, "OriginalName");
              DateTime stringJsSerializer1 = WorkItemUpdateModelBinder.GetDateTimeFromPossibleStringJSSerializer(link, "CreationDate");
              DateTime stringJsSerializer2 = WorkItemUpdateModelBinder.GetDateTimeFromPossibleStringJSSerializer(link, "LastWriteDate");
              int dictionaryJsSerializer7 = WorkItemUpdateModelBinder.GetValueFromDictionaryJSSerializer<int>(link, "Length");
              List<WorkItemResourceLinkUpdate> resourceLinkUpdateList = resourceLinkUpdates;
              WorkItemResourceLinkUpdate resourceLinkUpdate = new WorkItemResourceLinkUpdate();
              resourceLinkUpdate.SourceWorkItemId = sourceId;
              resourceLinkUpdate.Type = new ResourceLinkType?(ResourceLinkType.Attachment);
              resourceLinkUpdate.UpdateType = linkUpdateType;
              resourceLinkUpdate.Location = dictionaryJsSerializer5;
              resourceLinkUpdate.Name = dictionaryJsSerializer6;
              resourceLinkUpdate.CreationDate = new DateTime?(stringJsSerializer1);
              resourceLinkUpdate.LastModifiedDate = new DateTime?(stringJsSerializer2);
              resourceLinkUpdate.Length = new int?(dictionaryJsSerializer7);
              resourceLinkUpdate.Comment = dictionaryJsSerializer2;
              resourceLinkUpdateList.Add(resourceLinkUpdate);
              continue;
            }
            int dictionaryJsSerializer8 = WorkItemUpdateModelBinder.GetValueFromDictionaryJSSerializer<int>(link, "ExtID");
            List<WorkItemResourceLinkUpdate> resourceLinkUpdateList1 = resourceLinkUpdates;
            WorkItemResourceLinkUpdate resourceLinkUpdate1 = new WorkItemResourceLinkUpdate();
            resourceLinkUpdate1.SourceWorkItemId = sourceId;
            resourceLinkUpdate1.ResourceId = new int?(dictionaryJsSerializer8);
            resourceLinkUpdate1.Type = new ResourceLinkType?(ResourceLinkType.Attachment);
            resourceLinkUpdate1.UpdateType = linkUpdateType;
            resourceLinkUpdate1.Comment = dictionaryJsSerializer2;
            resourceLinkUpdateList1.Add(resourceLinkUpdate1);
            continue;
          case 51:
            List<WorkItemResourceLinkUpdate> resourceLinkUpdateList2 = resourceLinkUpdates;
            WorkItemResourceLinkUpdate resourceLinkUpdate2 = new WorkItemResourceLinkUpdate();
            resourceLinkUpdate2.SourceWorkItemId = sourceId;
            resourceLinkUpdate2.ResourceId = WorkItemUpdateModelBinder.GetValueFromDictionaryJSSerializer<int?>(link, "ExtID", true);
            resourceLinkUpdate2.Type = new ResourceLinkType?(ResourceLinkType.Hyperlink);
            resourceLinkUpdate2.UpdateType = linkUpdateType;
            resourceLinkUpdate2.Location = WorkItemUpdateModelBinder.GetValueFromDictionaryJSSerializer<string>(link, "FilePath");
            resourceLinkUpdate2.Name = WorkItemUpdateModelBinder.GetValueFromDictionaryJSSerializer<string>(link, "OriginalName", true);
            resourceLinkUpdate2.Comment = dictionaryJsSerializer2;
            resourceLinkUpdateList2.Add(resourceLinkUpdate2);
            continue;
          case 58:
            List<WorkItemResourceLinkUpdate> resourceLinkUpdateList3 = resourceLinkUpdates;
            WorkItemResourceLinkUpdate resourceLinkUpdate3 = new WorkItemResourceLinkUpdate();
            resourceLinkUpdate3.SourceWorkItemId = sourceId;
            resourceLinkUpdate3.ResourceId = WorkItemUpdateModelBinder.GetValueFromDictionaryJSSerializer<int?>(link, "ExtID", true);
            resourceLinkUpdate3.Type = new ResourceLinkType?(ResourceLinkType.ArtifactLink);
            resourceLinkUpdate3.UpdateType = linkUpdateType;
            resourceLinkUpdate3.Location = WorkItemUpdateModelBinder.GetValueFromDictionaryJSSerializer<string>(link, "FilePath");
            resourceLinkUpdate3.Name = WorkItemUpdateModelBinder.GetValueFromDictionaryJSSerializer<string>(link, "OriginalName", true);
            resourceLinkUpdate3.Comment = dictionaryJsSerializer2;
            resourceLinkUpdateList3.Add(resourceLinkUpdate3);
            continue;
          default:
            continue;
        }
      }
    }

    private static DateTime GetDateTimeFromPossibleStringJSSerializer(
      IDictionary<string, object> dictionary,
      string key)
    {
      try
      {
        return WorkItemUpdateModelBinder.GetValueFromDictionaryJSSerializer<DateTime>(dictionary, key);
      }
      catch (InvalidArgumentValueException ex)
      {
      }
      string dictionaryJsSerializer = WorkItemUpdateModelBinder.GetValueFromDictionaryJSSerializer<string>(dictionary, key);
      try
      {
        return DateTime.Parse(dictionaryJsSerializer);
      }
      catch (Exception ex)
      {
        throw new InvalidArgumentValueException(key);
      }
    }

    private static T GetValueFromDictionaryJSSerializer<T>(
      IDictionary<string, object> dictionary,
      string key,
      bool optional = false)
    {
      object obj;
      if (dictionary.TryGetValue(key, out obj))
      {
        if (obj == null)
          return WorkItemUpdateModelBinder.GetDefaultValue<T>();
        return typeof (T).IsAssignableFrom(obj.GetType()) ? (T) obj : throw new InvalidArgumentValueException(key);
      }
      if (!optional)
        throw new InvalidArgumentValueException(key);
      return WorkItemUpdateModelBinder.GetDefaultValue<T>();
    }
  }
}
