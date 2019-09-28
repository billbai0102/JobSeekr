using System;
using System.Net.Http;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace TTS_Creator
{
    class TextToSpeech
    {
        //This is method is accessable by any class without a constructor. It takes in text which will be 
        //coverted to a .wav file, a file name which it will be saved as, and the name of the voice.
        public static async Task createTTS(string text, string fileName, string voiceName)
        {
            string host = "https://eastus.tts.speech.microsoft.com/cognitiveservices/v1";
            //This sets up the TTS, and also the TTS voice.
            string body = @"<speak version='1.0' xmlns='https://www.w3.org/2001/10/synthesis' xml:lang='en-US'>
              <voice name='Microsoft Server Speech Text to Speech Voice (en-US, " + voiceName + ")'>" +
                          text + "</voice></speak>";

            // Gets an access token
            string accessToken;

            //Uses Authentication class to authenticate TTS API Key.
            Authentication auth = new Authentication("https://eastus.api.cognitive.microsoft.com/sts/v1.0/issueToken", "[redacted API Key] - Contact billbai0102@gmail.com for key, or use your own.");
            try
            {
                accessToken = await auth.FetchTokenAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to obtain an access token.");
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.Message);
                return;
            }

            //Sends a string, gets a .wav file, then saves it in the Resources folder in Assets.
            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage())
                {
                    // Set the HTTP method
                    request.Method = HttpMethod.Post;
                    // Construct the URI
                    request.RequestUri = new Uri(host);
                    // Set the content type header
                    request.Content = new StringContent(body, Encoding.UTF8, "application/ssml+xml");
                    // Set additional header, such as Authorization and User-Agent
                    request.Headers.Add("Authorization", "Bearer " + accessToken);
                    request.Headers.Add("Connection", "Keep-Alive");
                    // Update your resource name
                    request.Headers.Add("User-Agent", "TTS-BillBai-HTN");
                    request.Headers.Add("X-Microsoft-OutputFormat", "riff-24khz-16bit-mono-pcm");
                    using (var response = await client.SendAsync(request).ConfigureAwait(false))
                    {
                        response.EnsureSuccessStatusCode();
                        // Asynchronously read the response
                        using (var dataStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                        {
                            using (var fileStream = new FileStream("../JobSecret3/Assets/Resources" + fileName + ".wav", FileMode.Create, FileAccess.Write, FileShare.Write))
                            {
                                await dataStream.CopyToAsync(fileStream).ConfigureAwait(false);
                                fileStream.Close();
                            }
                        }
                    }
                }
            }
        }
    }
}
