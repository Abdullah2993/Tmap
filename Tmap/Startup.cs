using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Owin;
using Tweetinvi;
using Tweetinvi.Core.Credentials;
using Tweetinvi.Core.Enum;
using Tweetinvi.Core.Events.EventArguments;
using Tweetinvi.Core.Interfaces.Streaminvi;

namespace Tmap
{
    public class Startup
    {
        private static readonly ISampleStream tStream;
        static Startup()
        {
            Auth.ApplicationCredentials = new TwitterCredentials(ConfigurationManager.AppSettings["consumerKey"],
                ConfigurationManager.AppSettings["consumerSecret"],
                ConfigurationManager.AppSettings["accessToken"], ConfigurationManager.AppSettings["accessTokenSecret"]);
            tStream = Stream.CreateSampleStream();
            tStream.AddTweetLanguageFilter(Language.English);
            tStream.TweetReceived += tStream_TweetReceived;
            tStream.StreamStopped += tStream_StreamStopped;
     
            tStream.StallWarnings = true;
            tStream.StartStreamAsync();
        }

        private static void tStream_StreamStopped(object sender, StreamExceptionEventArgs e)
        {
            tStream.StartStream();
        }

        private static void tStream_TweetReceived(object sender, TweetReceivedEventArgs e)
        {
            var t = e.Tweet;
            //Debug.WriteIf(t.Coordinates==null,"Cords null\n");
            //Debug.WriteIf(t.Place==null,"Place null\n");
            
            if(t.Coordinates==null) return;
            GlobalHost.ConnectionManager.GetHubContext<TweetHub>()
                .Clients.All.sendTweet(t.Coordinates.Longitude, t.Coordinates.Latitude, t.CreatedBy.ScreenName,
                    t.CreatedAt.ToShortTimeString(), t.Text);
        }

        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
