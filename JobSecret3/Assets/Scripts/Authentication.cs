using System;
using System.Net.Http;
using System.Text;
using System.IO;
using System.Threading.Tasks;

public class Authentication
{
    private string subscriptionKey; //Subscription key
    private string tokenFetchUri; //URI to get token

    //This is the constructor, it will set token uri & subscription key
    public Authentication(string tokenFetchUri, string subscriptionKey)
    {
        //If statement that checks for invalid keys/ uris
        if (string.IsNullOrWhiteSpace(tokenFetchUri))
        {
            throw new ArgumentNullException(nameof(tokenFetchUri));
        }
        if (string.IsNullOrWhiteSpace(subscriptionKey))
        {
            throw new ArgumentNullException(nameof(subscriptionKey));
        }
        this.tokenFetchUri = tokenFetchUri;
        this.subscriptionKey = subscriptionKey;
    }

    //Authentication
    public async Task<string> FetchTokenAsync()
    {
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", this.subscriptionKey);
            UriBuilder uriBuilder = new UriBuilder(this.tokenFetchUri);

            var result = await client.PostAsync(uriBuilder.Uri.AbsoluteUri, null).ConfigureAwait(false);
            return await result.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
    }
}