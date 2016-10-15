using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Models;
//using Tweetinvi;

namespace TowerBotLibCore
{
    public class TwitterManager
    {
       
        public void PostMessage(Radar radar, string message)
        {
            if (radar != null)
            {
                if (message.Length > 139)
                {
                    message = message.Substring(0, 139);
                }

                if (radar.Name == "BSB")
                {
                    
                    var creds = new TwitterCredentials("3r8wBciRbW7wniT7DYIofy60G", "ozfqugyE2hihws5AkGw8yXVvuZMqY5u9rpIOjdKxxHjqo3KM5T", "3087708189-bkr12ClOMZyBeiHmw7i9EZeXlnSNAjx3QjKnxe4", "cVL2s1kCzJl3nAydDXkIz1fVY07g1XWnUGByjb92ZO8wj");

                    var tweet = Auth.ExecuteOperationWithCredentials(creds, () =>
                    {
                        return Tweet.PublishTweet(message);
                    });

                }
                
            }
        }
    }
}
