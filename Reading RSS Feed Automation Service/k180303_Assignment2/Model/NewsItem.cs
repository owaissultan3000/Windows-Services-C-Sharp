using System;
using System.Collections.Generic;
using System.Text;

namespace k180303_Assignment2.Model
{
    public class NewsItem
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime PublishedDate { get; set; }
        public string NewsChannel { get; set; }

        public NewsItem()
        {

        }
        public NewsItem(NewsItem newsModel)
        {
            Title = newsModel.Title;
            Description = newsModel.Description;
            PublishedDate = newsModel.PublishedDate;
            NewsChannel = newsModel.NewsChannel;

        }
    }
}
