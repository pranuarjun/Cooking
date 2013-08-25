using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Xml;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Connectivity;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace GridViewDataBindingDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            var profile = NetworkInformation.GetInternetConnectionProfile();

            if (profile.GetNetworkConnectivityLevel() != NetworkConnectivityLevel.InternetAccess)
            {
                await new MessageDialog("This app requires Internet access").ShowAsync();
                return;
            }
            
            // Grab ABC's RSS news feed
            var client = new HttpClient();
            client.MaxResponseContentBufferSize = 10 * 1024 * 1024; // Read up to 10 MB of data
			//var response = await client.GetAsync(new Uri("http://feeds.abcnews.com/abcnews/topstories"));

			var response = await client.GetAsync(new Uri("http://feeds.feedburner.com/pranukitchen"));
			var result = await response.Content.ReadAsStringAsync();
            // Use LINQ to XML to extract RSS items
            try
            {
                XDocument doc = XDocument.Parse(result);
	            XNamespace xmlns = "http://www.w3.org/2005/Atom";//"http://search.yahoo.com/mrss/";

				var items = from results in doc.Descendants("item")
                            select new
                            {
                                title = (string)results.Element("title"),
                                link = (string)results.Element("link"),
								image = GetImageFromContent(results.Element("description")),
								blogcontent = (string)results.Element("description")
                            };

                // Create a collection and populate it with NewsItems
                ObservableCollection<NewsItem> feed = new ObservableCollection<NewsItem>();

                foreach (var item in items)
                {
                    string title = item.title.ToLower();

                    // Skip certain types of content
                    if (title.StartsWith("advertisement:") ||
                        title.StartsWith("watch:") ||
                        title.StartsWith("photos:"))
                        continue;

                    // Create a new NewsItem
                    NewsItem rss = new NewsItem();
                    rss.Headline = item.title.Trim();
                    rss.ContentUri = item.link;
	                rss.FileContent = item.blogcontent;

	                if (item.image != null)
		                rss.ImageUri = (string) item.image;/*image.Attribute("url");*/

                    // Add the NewsItem to the collection
                    feed.Add(rss);
                }

                // Bind the GridView to the collection
                NewsGrid.ItemsSource = feed;
            }
            catch (XmlException)
            {
                new MessageDialog("Data unavailable").ShowAsync();
            }
        }

	    private string GetImageFromContent(XElement element)
	    {
		    string htmlContent = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><temp>" +element.Value.ToString() + "</temp>";
			int start = htmlContent.IndexOf("a href=");
		    start = start + 8;
		    int end = htmlContent.IndexOf('\"', start);
		    string uri = htmlContent.Substring(start, (end - start));
		    return uri;
	    }

	    private void NewsGrid_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(WebViewPage), ((NewsItem)e.ClickedItem));
        }
    }
}
