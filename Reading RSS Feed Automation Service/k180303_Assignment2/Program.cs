using k180303_Assignment2.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Xml;
using System.Xml.Serialization;

namespace k180303_Assignment2
{
    public class RssFeed
    {
        public List<NewsItem> FeedNewsList = new List<NewsItem>();
        public List<NewsItem> SortedFeedNewsList = new List<NewsItem>();

        public void FetchRSSData(string URL)
        {
            try
            {
                NewsItem newsModel;
                XmlReader reader = XmlReader.Create(URL);
                SyndicationFeed feed = SyndicationFeed.Load(reader);
                reader.Close();
                foreach (SyndicationItem item in feed.Items)
                {
                    newsModel = new NewsItem();
                    newsModel.Title = item.Title.Text;
                    newsModel.Description = item.Summary.Text;
                    newsModel.PublishedDate = item.PublishDate.DateTime;
                    newsModel.NewsChannel = item.Id.Split('/')[2];
                    FeedNewsList.Add(newsModel);

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex);
                return;
            }
        }

        public void SortNewsItems()
        {
            var query =
                from m in FeedNewsList
                orderby m.PublishedDate descending
                select m;
            
            foreach(NewsItem item in query)
            {
                SortedFeedNewsList.Add(item);
            }

            
        }

        public void GenerateXML()
        {
            string FilePath = ConfigurationManager.AppSettings["XMLLocation"];

            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);
            }

            string FileName = FilePath + "\\News.xml";

            XmlSerializer serialiser = new XmlSerializer(typeof(List<NewsItem>));

            TextWriter Filestream = new StreamWriter(FileName);

            serialiser.Serialize(Filestream, SortedFeedNewsList);

            Filestream.Close();
        }
        static void Main(string[] args)
        {

            string link1 = ConfigurationManager.AppSettings["NewsLink1"];
            string link2 = ConfigurationManager.AppSettings["NewsLink2"];
            RssFeed rssFeed = new RssFeed();
            rssFeed.FetchRSSData(link1);
            rssFeed.FetchRSSData(link2);
            rssFeed.SortNewsItems();
            rssFeed.GenerateXML();



        }
    }
}
