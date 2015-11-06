using System;
using System.Net;

namespace DailySubsSubtitleDownloader
{
    public class DailySubsCookieWebClient : WebClient
    {
        private WebRequest _request;
        

        public CookieContainer CookieContainer { get; private set; }

        /// <summary>
        /// This will instanciate an internal CookieContainer.
        /// </summary>
        public DailySubsCookieWebClient()
        {
            CookieContainer = new CookieContainer();
        }

        /// <summary>
        /// Use this if you want to control the CookieContainer outside this class.
        /// </summary>
        public DailySubsCookieWebClient(CookieContainer cookieContainer)
        {
            CookieContainer = cookieContainer;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            _request = base.GetWebRequest(address);

            var request = _request as HttpWebRequest;
            if (request != null)
            {
                request.AllowAutoRedirect = true;
                request.CookieContainer = CookieContainer;
            }

            return _request;
        } 

        public HttpStatusCode StatusCode()
        {
            HttpStatusCode result;

            if (_request == null)
            {
                throw (new InvalidOperationException("Unable to retrieve the status code, maybe you haven't made a request yet."));
            }

            var response = GetWebResponse(_request) as HttpWebResponse;

            if (response != null)
            {
                result = response.StatusCode;
            }
            else
            {
                throw (new InvalidOperationException("Unable to retrieve the status code, maybe you haven't made a request yet."));
            }

            return result;
        }
    }
}
