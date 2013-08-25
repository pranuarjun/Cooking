using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Text.RegularExpressions;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace GridViewDataBindingDemo
{
	using System.Xml.Linq;
	using Windows.Data.Xml.Dom;
	using Windows.UI.Xaml.Media.Imaging;

	/// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WebViewPage : Page
    {
        public WebViewPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
	        NewsItem item = (NewsItem) e.Parameter;
            // Point the WebView control to the URL passed as a parameter
            //News.Source = new Uri(item.ContentUri);
	        Title.Text = item.Headline;
			LoadContent(item.FileContent);
        }

		private void LoadContent(string content)
		{
			BitmapImage bitmapImage = new BitmapImage();
			bitmapImage.UriSource = new Uri(GetImageFromContent(content));
			Img.Source = bitmapImage;
			IngredientContent.Text = GetIngredients(content);
			DirectionsContent.Text=GetDescriptionContent(content);
		}

		private string GetImageFromContent(string element)
		{
			string htmlContent = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><temp>" + element + "</temp>";
			int start = htmlContent.IndexOf("a href=");
			start = start + 8;
			int end = htmlContent.IndexOf('\"', start);
			string uri = htmlContent.Substring(start, (end - start));
			return uri;
		}

		private string GetIngredients(string content)
		{
			var html = "<html><body>"+ content+"</body></html>";
			const string ingredients = "Ingredients";
			const string directions = "Directions";
			string listofingredients = string.Empty;
			int ingredientsIndex = html.IndexOf(ingredients);
			if (ingredientsIndex < 0) return listofingredients;
			int startindex = ingredientsIndex  + ingredients.Length + 5;
			if (startindex > 0)
			{
				int endindex = html.IndexOf(directions) - 4;

				listofingredients = html.Substring(startindex, (endindex -startindex));
				listofingredients = listofingredients.Replace("<br />", string.Empty);
				listofingredients = ScrubHtmlTags(listofingredients);
			}
			
			
			return listofingredients;
		}

		private string GetDescriptionContent(string content)
		{
			var html = "<html><body>" + content + "</body></html>";
			const string directions = "Directions";
			string directionList = string.Empty;
			int directionsIndex = html.IndexOf(directions);
			if (directionsIndex < 0) return directionList;
			int startindex = directionsIndex + directions.Length + 5;
			if (startindex > 0)
			{
				directionList = html.Substring(startindex);
				directionList = ScrubHtmlTags(directionList);
			}
			return directionList;
		}

		private string ScrubHtmlTags(string html)
		{

			string temp = null;
			if (html != null)
			{
				html = html.Replace("&nbsp;", string.Empty);
				int index = 0;
				temp = html;
				while (true)
				{
					int startindex = temp.IndexOf("<");
					int endindex = temp.IndexOf(">");
					if (startindex < 0) break;
					string before = temp.Substring(0, startindex);
					string after = string.Empty;
					if (endindex > 0)
					{
						after = "\r\n" + temp.Substring(endindex + 1);
					}
					temp = before + after;
				}
				temp = temp.Replace("\r\n\r\n","\r\n");
			}
			return temp;
		}

		
        private void BackButtonClick(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }
    }
}
