using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowerBotLibCore;
using Tweetinvi;
using Tweetinvi.Models;

namespace TowerBotConsole
{
    public class TwitterManager
    {
       
        public void PostMessage(Radar radar, string message)
        {
            if (radar != null && !String.IsNullOrEmpty(radar.TwitterConsumerKey))
            {
                if (message.Length > 139)
                {
                    message = message.Substring(0, 139);
                }

                    var creds = new TwitterCredentials(radar.TwitterConsumerKey, radar.TwitterConsumerSecret,radar.TwitterAccessToken,radar.TwitterAccessTokenSecret);

                    var tweet = Auth.ExecuteOperationWithCredentials(creds, () =>
                    {
                        return Tweet.PublishTweet(message);
                    });      
                
            }
        }
    }
}
