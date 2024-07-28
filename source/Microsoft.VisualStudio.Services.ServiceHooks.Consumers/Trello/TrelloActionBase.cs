// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Trello.TrelloActionBase
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.CSharp.RuntimeBinder;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceHooks.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Trello
{
  public abstract class TrelloActionBase : ConsumerActionImplementation
  {
    protected const int c_boardIdLengthMin = 8;
    protected const int c_boardIdLengthMax = 64;
    protected const int c_labelsLengthMin = 3;
    protected const int c_labelsLengthMax = 35;
    protected const int c_listIdLengthMin = 9;
    protected const int c_listIdLengthMax = 64;
    protected const string c_cardPositionTop = "top";
    protected const string c_cardPositionBottom = "bottom";
    protected const string c_listPositionTop = "top";
    protected const string c_listPositionBottom = "bottom";
    protected const string c_defaultPropertyValueFalse = "False";
    protected const string c_contentTypeJson = "application/json";
    public const string ListIdInputId = "listId";
    public const string LabelsInputId = "labels";
    public const string BoardIdInputId = "boardId";
    public const string AddToTopInputId = "addToTop";
    public const string AddToBottomInputId = "addToBottom";
    public const string UrlFormatCreateList = "https://api.trello.com/1/lists?key={0}&token={1}";
    public const string UrlFormatCreateCard = "https://api.trello.com/1/cards?key={0}&token={1}";
    public const string UrlFormatQueryLists = "https://api.trello.com/1/boards/{0}/lists?key={1}&token={2}";
    public const string UrlFormatQueryBoards = "https://api.trello.com/1/members/me/boards?key={0}&token={1}";
    public const string RegistryPathUrlFormatCreateList = "/Service/ServiceHooks/TrelloConsumer/ListCreateAction/UrlFormatCreateList";
    public const string RegistryPathUrlFormatCreateCard = "/Service/ServiceHooks/TrelloConsumer/CardCreateAction/UrlFormatCreateCard";
    public const string RegistryPathUrlFormatQueryLists = "/Service/ServiceHooks/TrelloConsumer/CardCreateAction/UrlFormatQueryLists";
    public const string RegistryPathUrlFormatQueryBoards = "/Service/ServiceHooks/TrelloConsumer/CardCreateAction/UrlFormatQueryBoards";

    protected IList<InputValue> GetPossibleValues(
      IVssRequestContext requestContext,
      string inputId,
      IDictionary<string, string> currentConsumerInputValues,
      out string errorMessage)
    {
      IList<InputValue> possibleValues = (IList<InputValue>) new List<InputValue>();
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      errorMessage = (string) null;
      string str1;
      if (service != null && currentConsumerInputValues.TryGetValue("userToken", out str1))
      {
        HttpRequestMessage request = (HttpRequestMessage) null;
        switch (inputId)
        {
          case "boardId":
            request = new HttpRequestMessage(HttpMethod.Get, string.Format(service.GetValue<string>(requestContext, (RegistryQuery) "/Service/ServiceHooks/TrelloConsumer/CardCreateAction/UrlFormatQueryBoards", true, "https://api.trello.com/1/members/me/boards?key={0}&token={1}"), (object) TrelloConsumer.ApplicationKey, (object) str1));
            break;
          case "listId":
            string str2;
            if (currentConsumerInputValues.TryGetValue("boardId", out str2))
            {
              request = new HttpRequestMessage(HttpMethod.Get, string.Format(service.GetValue<string>(requestContext, (RegistryQuery) "/Service/ServiceHooks/TrelloConsumer/CardCreateAction/UrlFormatQueryLists", true, "https://api.trello.com/1/boards/{0}/lists?key={1}&token={2}"), (object) str2, (object) TrelloConsumer.ApplicationKey, (object) str1));
              break;
            }
            break;
        }
        if (request != null)
        {
          try
          {
            using (HttpClientHandler handler = new HttpClientHandler())
            {
              using (HttpClient client = new HttpClient((HttpMessageHandler) handler)
              {
                Timeout = new TimeSpan(0, 0, 30)
              })
              {
                HttpResponseMessage httpResponseMessage = client.PauseTimerAndSendResult(requestContext, request, HttpCompletionOption.ResponseHeadersRead);
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                  Task<string> task = httpResponseMessage.Content.ReadAsStringAsync();
                  task.Wait();
                  object obj1 = (object) JArray.Parse(task.Result);
                  // ISSUE: reference to a compiler-generated field
                  if (TrelloActionBase.\u003C\u003Eo__25.\u003C\u003Ep__4 == null)
                  {
                    // ISSUE: reference to a compiler-generated field
                    TrelloActionBase.\u003C\u003Eo__25.\u003C\u003Ep__4 = CallSite<Func<CallSite, object, IEnumerable>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (IEnumerable), typeof (TrelloActionBase)));
                  }
                  // ISSUE: reference to a compiler-generated field
                  // ISSUE: reference to a compiler-generated field
                  foreach (object obj2 in TrelloActionBase.\u003C\u003Eo__25.\u003C\u003Ep__4.Target((CallSite) TrelloActionBase.\u003C\u003Eo__25.\u003C\u003Ep__4, obj1))
                  {
                    IList<InputValue> inputValueList = possibleValues;
                    InputValue inputValue1 = new InputValue();
                    InputValue inputValue2 = inputValue1;
                    // ISSUE: reference to a compiler-generated field
                    if (TrelloActionBase.\u003C\u003Eo__25.\u003C\u003Ep__1 == null)
                    {
                      // ISSUE: reference to a compiler-generated field
                      TrelloActionBase.\u003C\u003Eo__25.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (TrelloActionBase)));
                    }
                    // ISSUE: reference to a compiler-generated field
                    Func<CallSite, object, string> target1 = TrelloActionBase.\u003C\u003Eo__25.\u003C\u003Ep__1.Target;
                    // ISSUE: reference to a compiler-generated field
                    CallSite<Func<CallSite, object, string>> p1 = TrelloActionBase.\u003C\u003Eo__25.\u003C\u003Ep__1;
                    // ISSUE: reference to a compiler-generated field
                    if (TrelloActionBase.\u003C\u003Eo__25.\u003C\u003Ep__0 == null)
                    {
                      // ISSUE: reference to a compiler-generated field
                      TrelloActionBase.\u003C\u003Eo__25.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "id", typeof (TrelloActionBase), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                      {
                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                      }));
                    }
                    // ISSUE: reference to a compiler-generated field
                    // ISSUE: reference to a compiler-generated field
                    object obj3 = TrelloActionBase.\u003C\u003Eo__25.\u003C\u003Ep__0.Target((CallSite) TrelloActionBase.\u003C\u003Eo__25.\u003C\u003Ep__0, obj2);
                    string str3 = target1((CallSite) p1, obj3);
                    inputValue2.Value = str3;
                    InputValue inputValue3 = inputValue1;
                    // ISSUE: reference to a compiler-generated field
                    if (TrelloActionBase.\u003C\u003Eo__25.\u003C\u003Ep__3 == null)
                    {
                      // ISSUE: reference to a compiler-generated field
                      TrelloActionBase.\u003C\u003Eo__25.\u003C\u003Ep__3 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (TrelloActionBase)));
                    }
                    // ISSUE: reference to a compiler-generated field
                    Func<CallSite, object, string> target2 = TrelloActionBase.\u003C\u003Eo__25.\u003C\u003Ep__3.Target;
                    // ISSUE: reference to a compiler-generated field
                    CallSite<Func<CallSite, object, string>> p3 = TrelloActionBase.\u003C\u003Eo__25.\u003C\u003Ep__3;
                    // ISSUE: reference to a compiler-generated field
                    if (TrelloActionBase.\u003C\u003Eo__25.\u003C\u003Ep__2 == null)
                    {
                      // ISSUE: reference to a compiler-generated field
                      TrelloActionBase.\u003C\u003Eo__25.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "name", typeof (TrelloActionBase), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                      {
                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                      }));
                    }
                    // ISSUE: reference to a compiler-generated field
                    // ISSUE: reference to a compiler-generated field
                    object obj4 = TrelloActionBase.\u003C\u003Eo__25.\u003C\u003Ep__2.Target((CallSite) TrelloActionBase.\u003C\u003Eo__25.\u003C\u003Ep__2, obj2);
                    string str4 = target2((CallSite) p3, obj4);
                    inputValue3.DisplayValue = str4;
                    InputValue inputValue4 = inputValue1;
                    inputValueList.Add(inputValue4);
                  }
                }
                else
                  errorMessage = httpResponseMessage.StatusCode != HttpStatusCode.Unauthorized ? string.Format(TrelloConsumerResources.QueryResponseFailureFormat, (object) httpResponseMessage.ReasonPhrase) : TrelloConsumerResources.SuppliedTokenNotAuthorized;
              }
            }
          }
          catch (Exception ex)
          {
            errorMessage = string.Format(TrelloConsumerResources.QueryExceptionFormat, (object) ex.Message);
          }
        }
      }
      return possibleValues;
    }
  }
}
