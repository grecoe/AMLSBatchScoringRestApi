using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace BatchScoring
{
    /// <summary>
    /// Response object from the rest call, whatever it was. 
    /// </summary>
    class RestResponse
    {
        public int StatusCode { get; set; }
        public String Content { get; set; }
    }

    /// <summary>
    /// Static class to make a request to the AMLS Batch Scoring pipeline
    /// </summary>
    class RestRequest
    {
        /// <summary>
        /// Start an experiment and get the results.
        /// </summary>
        /// <typeparam name="T">Type of object to retrieve</typeparam>
        /// <param name="requestInfo">Info for retrieving one or many run results</param>
        /// <param name="authToken">AAD Token for SP</param>
        /// <param name="payload">Job start body - {"ExperimentName" : "NAME"} </param>
        /// <returns>Results of the call as a T or null on failure. </returns>
        public static T StartBatchJob<T>(PipelineRequest requestInfo, string authToken, string payload) where T : class, new()
        {
            RestResponse rr = RestRequest.MakeRequest(requestInfo, authToken, payload, "application/json").Result;

            if (rr.Content != null && rr.Content.Length > 0)
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(rr.Content);
            }
            return null;
        }

        /// <summary>
        /// Get the results of a run or batch of runs
        /// </summary>
        /// <typeparam name="T">List<RunResult> or RunResult</RunResult></typeparam>
        /// <param name="requestInfo">Info for retrieving one or many run results</param>
        /// <param name="authToken">AAD Token for SP</param>
        /// <returns>Results of the call as a T or null on failure. </returns>
        public static T GetRunResults<T>(PipelineRequest requestInfo, string authToken) where T: class, new()
        {
            RestResponse rr = RestRequest.MakeRequest(requestInfo, authToken).Result;

            if (rr.Content != null && rr.Content.Length > 0)
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(rr.Content);
            }
            return null;
        }


        /// <summary>
        /// Generic call to an endpoint. 
        /// </summary>
        /// <param name="requestInfo">Request method and URL</param>
        /// <param name="authToken">AAD Token for SP</param>
        /// <param name="payload">Optional payload parameter</param>
        /// <param name="payloadType">Optional payload type parameter, i.e. "application/json"</param>
        /// <returns></returns>
        private static async Task<RestResponse> MakeRequest(PipelineRequest requestInfo, string authToken, string payload = null, string payloadType = null)
        {
            RestResponse returnResponse = null;

            using (HttpClient httpClient = new HttpClient())
            {
                String bearer = String.Format("Bearer {0}", authToken);

                httpClient.DefaultRequestHeaders.Add("Authorization", bearer);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.BaseAddress = new Uri(requestInfo.Url);

                var httpRequest = new HttpRequestMessage(requestInfo.Method, string.Empty);

                if (requestInfo.Method == HttpMethod.Post)
                {
                    httpRequest.Content = new StringContent(payload);
                    httpRequest.Content.Headers.ContentType = new MediaTypeHeaderValue(payloadType);
                }

                HttpResponseMessage res = await httpClient.SendAsync(httpRequest);

                returnResponse = new RestResponse()
                {
                    StatusCode = (int)res.StatusCode
                };

                if (res.IsSuccessStatusCode)
                {
                    HttpContent content = res.Content;
                    returnResponse.Content = await content.ReadAsStringAsync().ConfigureAwait(false);
                }
            }
            return returnResponse;
        }
    }
}
