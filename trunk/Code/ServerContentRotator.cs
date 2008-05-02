using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace YTech.WebControls.ContentRotator
{
	/// <summary>
	///     A panel that is able to rotate server content randomly.
	/// </summary>
	/// <example>
	///     <![CDATA[
	///     <%@ Register Assembly="YTech.WebControls.ContentRotator" Namespace="YTech.WebControls.ContentRotator" TagPrefix="CR" %>
	///     
	///     <CR:ServerContentRotator runat="server" Key="Rotator1">
	///         <CR:ContentPanel runat="server" Impressions="50" Key="Content1">
	///             Content 1
	///         </CR:ContentPanel>
	///         <CR:ContentPanel runat="server" Impressions="50" Key="Content2">
	///             Content 2
	///         </CR:ContentPanel>
	///     </CR:ServerContentRotator>
	///     ]]>
	/// </example>
	public class ServerContentRotator : Panel
	{
		/// <summary>
		///     The delegate for the <see cref="ContentShown"/> event.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="e"></param>
		public delegate void ContentShownDelegate(object s, ContentShownEventArgs e);

		/// <summary>
		///     The event raises when a content section is displayed.  This can be used
		///     for tracking purposes.
		/// </summary>
		public event ContentShownDelegate ContentShown;

		/// <summary>
		///     The postfix that is attached to the key of the content
		///     rotator that is used as the cookie name to load the
		///     key of the content that was last displayed.
		/// </summary>
		private const string SAVED_CONTENT_KEY_COOKIE_POSTFIX = "_SCR_ContentKey";

		private RotationModes _rotationMode = RotationModes.Random;
		private string _key = null;

		///	<summary>
		///		Gets or sets the key that will be used to uniquely identify
		///		this content rotator.  Set this property when you wish to
		///		track which content a particular user saw.
		/// </summary>
		public string Key
		{
			get { return _key; }
			set { _key = value; }
		}

		/// <summary>
		///     Gets or sets the rotation mode that is used to determine
		///     which content panel will be displayed.
		/// </summary>
		public RotationModes RotationMode
		{
			get { return _rotationMode; }
			set { _rotationMode = value; }
		}

		/// <summary>
		///     Renders the chile controls to the writer.
		/// </summary>
		/// <param name="writer"></param>
		protected override void RenderChildren(HtmlTextWriter writer)
		{
			choosePanel();

			base.RenderChildren(writer);
		}

		private void choosePanel()
		{
			List<ContentPanel> panels = getContentPanels();
			Dictionary<string, int> itemWeights = new Dictionary<string, int>();

			foreach(ContentPanel currPanel in panels)
				itemWeights.Add(currPanel.Key, currPanel.Impressions);

			string displayKey = ChooseKey(itemWeights, _rotationMode, getPreviousKey());
			
			//_log.DebugFormat("Key '{0}' is being displayed", displayKey);

			//display the chosen content panel
			foreach(ContentPanel currPanel in panels)
				currPanel.Visible = currPanel.Key == displayKey;

			//Raise the content chosen event
			ContentShownEventArgs args = new ContentShownEventArgs();
			args.ShownPanel = getPanelByKey(panels, displayKey);
			raiseContentShownEvent(this, args);

			//Save the key value for next time
			saveKey(displayKey);
		}

		private static ContentPanel getPanelByKey(IEnumerable<ContentPanel> panels, string key)
		{
			foreach (ContentPanel currPanel in panels)
			{
				if (currPanel.Key == key)
					return currPanel;
			}

			return null;
		}

		private string getPreviousKey()
		{
			HttpCookie keyCookie = HttpContext.Current.Request.Cookies[_key + SAVED_CONTENT_KEY_COOKIE_POSTFIX];
			if (keyCookie == null)
				return null;
			else
				return keyCookie.Value;
		}

		private void saveKey(string key)
		{
			HttpCookie keyCookie = new HttpCookie(_key + SAVED_CONTENT_KEY_COOKIE_POSTFIX);
			keyCookie.Value = key;
			keyCookie.Expires = DateTime.Now.AddDays(10);

			HttpContext.Current.Response.Cookies.Add(keyCookie);
		}

		private List<ContentPanel> getContentPanels()
		{
			List<ContentPanel> panels = new List<ContentPanel>();

			foreach (Control currControl in Controls)
			{
				if (currControl.GetType() == typeof (ContentPanel))
					panels.Add((ContentPanel) currControl);
			}

			return panels;
		}

		public static T ChooseKey<T>(IDictionary<T, int> itemWeights)
		{
			int totalImpressions = 0;
			foreach (T currKey in itemWeights.Keys)
				totalImpressions += itemWeights[currKey];

			Random rand = new Random();
			int targetPosition = rand.Next(0, totalImpressions - 1);

			int position = 0;
			foreach(T currKey in itemWeights.Keys)
			{
				int min = position;
				int max = position + itemWeights[currKey];

				if (targetPosition >= min && targetPosition < max)
					return currKey;

				position += itemWeights[currKey];
			}

			//This should never happen, but we put it here to avoid warnings
			return default(T);
		}

		public static T ChooseKey<T>(IDictionary<T, int> itemWeights, RotationModes rotationMode, T previousKey)
		{
			if (rotationMode == RotationModes.AlwaysSame)
			{
				if ((object)previousKey != null)
				{
					//This is easy
					return previousKey;
				}
			}
			else if (rotationMode == RotationModes.AlwaysDifferent)
			{
				if ((object)previousKey != null)
				{
					//Allow any key except the previous
					itemWeights.Remove(previousKey);
				}
			}

			return ChooseKey(itemWeights);
		}

		private void raiseContentShownEvent(object s, ContentShownEventArgs args)
		{
			ContentShownDelegate evt = ContentShown;

			if (evt != null)
				evt(s, args);
		}

		public override void RenderBeginTag(HtmlTextWriter writer)
		{
			//We don't want a start tag, we only want the content
		}

		public override void RenderEndTag(HtmlTextWriter writer)
		{
			//We don't want an end tag, we only want the content
		}
	}
}