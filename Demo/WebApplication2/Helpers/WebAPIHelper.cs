using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

namespace GenerateCourseRecommendation.Helpers
{
    public class WebAPIHelper
    {
        private readonly ILog logger = LogManager.GetLogger(typeof(WebAPIHelper));

        public T Get<T>(string uriBaseAddress, string uriPath, Dictionary<string, string> parameters, Dictionary<string, List<string>> parametersLists, Dictionary<string, string> headers, string mediaType = "application/json")
        {
            try
            {
                using (var client = new HttpClient())
                {
                    //logger.InfoFormat($"[{nameof(Get)}] - ReadyTo: send new GET request to api, uriBaseAddress = '{ uriBaseAddress }', uriPath = '{ uriPath }'");

                    var uriBuilder = new UriBuilder(uriBaseAddress);
                    uriBuilder.Path = uriPath;

                    client.BaseAddress = new Uri(uriBaseAddress);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));

                    // setting headers 
                    if (headers != null && headers.Count() > 0)
                    {
                        foreach (var item in headers)
                            client.DefaultRequestHeaders.Add(item.Key, item.Value);
                    }

                    // the list of query parameters list that will be appended to the URI
                    var queryParams = new List<string>();

                    // adding query parameters to the list
                    if (parameters != null && parameters.Count() > 0)
                    {
                        foreach (var item in parameters)
                            queryParams.Add($"{ item.Key }={ HttpUtility.UrlEncode(item.Value) }");
                    }

                    // adding query parameter lists to the list
                    if (parametersLists != null && parametersLists.Count() > 0)
                    {
                        foreach (var item in parametersLists)
                            foreach (var subItem in item.Value)
                                queryParams.Add($"{ item.Key }={ HttpUtility.UrlEncode(subItem) }");
                    }

                    // appending the list of query parameters to the URI
                    if (queryParams.Count > 0)
                        uriBuilder.Query = String.Join("&", queryParams);

                    // sending the request
                    var response = client.GetAsync(uriBuilder.Uri).Result;

                    // check results if success
                    if (response.IsSuccessStatusCode)
                    {
                        //logger.Info($"[{nameof(Get)}] - Api response with IsSuccess_Code = 'TRUE': Getting response from API and ready to extract content data from response, url = { uriBuilder.Uri }");

                        var result = response.Content.ReadAsStringAsync().Result;

                        if(response.Content.Headers.ContentType.MediaType == "text/plain")
                        {
                            if(string.IsNullOrEmpty(result))
                                throw new Exception($"[{nameof(Get)}] - Failed: Extract content data from api response, Content data = 'NULL', url = { uriBuilder.Uri }");

                            return (T)Convert.ChangeType(result, typeof(T));
                        }

                        var resultData = JsonConvert.DeserializeObject<T>(result);

                        if (resultData != null)
                        {
                            //logger.Info($"[{nameof(Get)}] - Success: Extract content data from api json response, url = { uriBuilder.Uri }");

                            return resultData;
                        }

                        else
                            throw new Exception($"[{nameof(Get)}] - Failed: Extract content data from api json response, Content data = 'NULL', url = { uriBuilder.Uri }");
                    }

                    else
                        throw new Exception($"[{nameof(Get)}] - api response with IsSuccess_Code = 'FALSE', Reason-Phrase = '{ response.ReasonPhrase }', Status-Code = '{ response.StatusCode.ToString() }', url = { uriBuilder.Uri }");
                }
            }
            catch (Exception ex)
            {
                logger.Error($"[{nameof(Get)}] - Error while calling GET request to API, uriBaseAddress = '{ uriBaseAddress }', uriPath = '{ uriPath }', Details = { ex.ToString() }");

                throw new Exception($"[{nameof(Get)}] - Error while calling GET request to API, uriBaseAddress = '{ uriBaseAddress }', uriPath = '{ uriPath }', Details = { ex.ToString() }");
            }
        }

        public T Post<T>(string uriBaseAddress, string uriPath , string requestContent , Dictionary<string,string> headers , string mediaType = "application/json")
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var APIUrl = $"{uriBaseAddress}{uriPath}";

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
                    client.Timeout = TimeSpan.FromMinutes(5);

                    if (headers != null && headers.Count() > 0)
                    {
                        foreach (var item in headers)
                            client.DefaultRequestHeaders.Add(item.Key, item.Value);
                    }


                    using (var request = new HttpRequestMessage(new HttpMethod("POST"), APIUrl))
                    {
                        request.Content = new StringContent(requestContent, Encoding.UTF8, mediaType);

                        var response = client.SendAsync(request).GetAwaiter().GetResult();

                        if (response.IsSuccessStatusCode)
                        {
                            var result = response.Content.ReadAsStringAsync().Result;

                            if (string.IsNullOrWhiteSpace(result))
                                throw new Exception($"empty response returned");

                            var responseData = JsonConvert.DeserializeObject<T>(result);

                            return responseData;
                        }
                        else
                            throw new Exception($"request failed , status code : {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error($"[{nameof(Post)}] - an error occured, ex:{ex} , url: {uriBaseAddress}{uriPath} , request content: {requestContent}");
                throw;
            }
        } 
    }
}
