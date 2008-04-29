using System;
using System.Reflection;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;

namespace YTech.WebControls.ContentRotator
{
	/// <summary>
	///     A panel that is able to rotate server content randomly.
	/// </summary>
	/// <example>
	///     <![CDATA[
	///     <%@ Register Assembly="YTech.General" Namespace="YTech.General.Web.Controls.ContentRotation" TagPrefix="CR" %>
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
		private ICookieDatasource _cookieDS;

		private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
		///     Creates a new instance of the <see cref="ServerContentRotator" /> control.
		/// </summary>
		public ServerContentRotator()
		{
		}

		/// <summary>
		///     The method called when the page is loading.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			_cookieDS = new CookieDatasource(Page.Request.Cookies, Page.Response.Cookies);
		}

		/// <summary>
		///     Creates a new instance of the <see cref="ServerContentRotator" /> control.
		/// </summary>
		/// <remarks>
		///     This constructor is primarily for unit testing.  It allows us to mock
		///     the cookie reading and writing.
		/// </remarks>
		/// <param name="cookieDataSource">
		///     The <see cref="ICookieDatasource"/> to use for reading and writing
		///     persistant cookie values.
		/// </param>
		public ServerContentRotator(ICookieDatasource cookieDataSource)
		{
			_cookieDS = cookieDataSource;
		}

		/// <summary>
		///     Gets or sets the key that will be used to uniquely identify
		///     this content rotator.  Set this property when you wish to
		///     track which content a particular user saw.
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

		/// <summary>
		///     Determines which panel should be displayed, and displays it.
		/// </summary>
		/// <remarks>
		///     This method is primarily a wrapper for the choosePanel
		///     method.  It simply totals the number of impressions for all panels,
		///     and then calls that method.
		/// </remarks>
		private void choosePanel()
		{
			ContentPanel cp;
			int totalImpressions = 0;
			List<ContentPanel> panels;
			bool firstPanel;

			panels = new List<ContentPanel>();
			firstPanel = true;

			foreach (Control currControl in Controls)
			{
				if (currControl.GetType() == typeof(ContentPanel))
				{
					cp = (ContentPanel)currControl;
					cp.Visible = false; //Hide all the panels
					totalImpressions += cp.Impressions;

					//Check if this is the first panel that can actually
					//get displayed.  One impression will be in position 0, so
					//we need to take care of that here.
					if (firstPanel && cp.Impressions > 0)
					{
						totalImpressions -= 1;
						firstPanel = false;
					}

					panels.Add(cp);
				}
			}

			choosePanel(panels, totalImpressions);
		}

		/// <summary>
		///     Determines which panel should be displayed, and displays it.
		/// </summary>
		private void choosePanel(List<ContentPanel> panels, int totalImpressions)
		{
			int randomVal;
			int impressionPointer;
			ContentPanel chosenPanel = null;
			bool previousPanelFound = false;

			randomVal = getRandomValue(totalImpressions);

			if (_rotationMode == RotationModes.AlwaysSame)
			{
				if (_key == null)
				{
					_log.Warn("The content rotator can't always show the same content if a key isn't specified");
				}
				else
				{
					//Check if there is a cookie containing the previous panel
					string savedKey = GetSavedContentKey(_key);
					if (!string.IsNullOrEmpty(savedKey))
						panelChosen(panels, savedKey, out previousPanelFound);

					if (previousPanelFound)
						return;
				}
			}

			impressionPointer = 0;
			foreach (ContentPanel currPanel in panels)
			{
				//See if the random value occurs in the range of the current panel
				if (randomVal >= impressionPointer && randomVal < impressionPointer + currPanel.Impressions)
				{
					chosenPanel = currPanel;
					break;
				}

				impressionPointer += currPanel.Impressions;
			}

			if (chosenPanel != null)
				panelChosen(chosenPanel);
		}

		#region Cookie reading/writing methods

		/// <summary>
		///     Gets the name of the cookie that this content rotator
		///     will use to store the display content key.
		/// </summary>
		/// <returns>
		///     The name of the cookie. Note that the cookie may NOT exist.
		/// </returns>
		public string GetCookieName()
		{
			return getCookieName(_key);
		}

		/// <summary>
		///     Gets the name of the cookie that stores the content key
		///     for the rotator whose key is specified.
		/// </summary>
		/// <param name="rotatorKey">
		///     The key of any content rotator.
		/// </param>
		/// <returns>
		///     The name of the cookie. Note that the cookie may NOT exist.
		/// </returns>
		private static string getCookieName(string rotatorKey)
		{
			return rotatorKey + SAVED_CONTENT_KEY_COOKIE_POSTFIX;
		}

		/// <summary>
		///     Gets the key for the content that was displayed by the content
		///     rotator with the specified rotator key.
		/// </summary>
		/// <remarks>
		///     Basically, you will use this method if you want to know which content
		///     section was displayed to a particular user.  For example, you would use
		///     this if you were doing A/B testing, and wanted to know which version of the
		///     content the current user saw.
		/// </remarks>
		/// <param name="requestCookies">
		///     The request cookies for the current request where the content
		///     key will attempt to be read from.
		/// </param>
		/// <param name="contentRotatorKey">
		///     The key of the content rotator to get the displayed content key for.
		/// </param>
		/// <returns></returns>
		public static string GetSavedContentKey(HttpCookieCollection requestCookies, string contentRotatorKey)
		{
			ICookieDatasource cookieDS;

			cookieDS = new CookieDatasource(requestCookies, null);
			return cookieDS.ReadValue(getCookieName(contentRotatorKey));
		}

		/// <summary>
		///     Gets all of the rotator key/content key combinations stored in
		///     the specified cookie collection.
		/// </summary>
		/// <returns></returns>
		public static Dictionary<string, string> GetSavedContentKeys(HttpCookieCollection cookies)
		{
			int suffixLength;
			Dictionary<string, string> keyPairs;

			suffixLength = SAVED_CONTENT_KEY_COOKIE_POSTFIX.Length;
			keyPairs = new Dictionary<string, string>();

			foreach (string currCookieName in cookies.AllKeys)
			{
				if (currCookieName.EndsWith(SAVED_CONTENT_KEY_COOKIE_POSTFIX))
				{
					string rotatorKey = currCookieName.Substring(0, currCookieName.Length - suffixLength);
					string contentKey = cookies[currCookieName].Value;

					keyPairs.Add(rotatorKey, contentKey);
				}
			}

			return keyPairs;
		}

		/// <summary>
		///     Gets the key for the content that was displayed by the content
		///     rotator with the specified rotator key.
		/// </summary>
		/// <remarks>
		///     Basically, you will use this method if you want to know which content
		///     section was displayed to a particular user.  For example, you would use
		///     this if you were doing A/B testing, and wanted to know which version of the
		///     content the current user saw.
		/// </remarks>
		/// <param name="contentRotatorKey">
		///     The key of the content rotator to get the displayed content key for.
		/// </param>
		/// <returns></returns>
		public string GetSavedContentKey(string contentRotatorKey)
		{
			//If we don't have a key ourselves, we can't look up the content key
			if (string.IsNullOrEmpty(contentRotatorKey))
				return null;

			return _cookieDS.ReadValue(getCookieName(contentRotatorKey));
		}

		/// <summary>
		///     Saves the specified key to a cookie so that it can
		///     be retrieved for this user later.
		/// </summary>
		/// <param name="contentKey">
		///     The unique key of a particular content section.  This would
		///     be the key of the currently displayed content.
		/// </param>
		private void saveContentKey(string contentKey)
		{
			_cookieDS.WriteValue(GetCookieName(), contentKey);
		}

		#endregion

		/// <summary>
		///     Looks though the panels for one that has the specified key
		///     and displays that panel.
		/// </summary>
		private void panelChosen(ContentPanel chosenPanel)
		{
			ContentShownEventArgs args;

			chosenPanel.Visible = true;

			args = new ContentShownEventArgs();
			args.ShownPanel = chosenPanel;

			raiseContentShownEvent(this, args);

			if (!string.IsNullOrEmpty(_key) && !string.IsNullOrEmpty(chosenPanel.Key))
				saveContentKey(chosenPanel.Key);
		}

		/// <summary>
		///     Looks though a panel collection for one that has the specified key
		///     and displays that panel.
		/// </summary>
		/// <param name="panels">
		///     The panel collection to look for a panel witht the specified
		///     content key.
		/// </param>
		/// <param name="contentKey">
		///     The key to look for in the panel collection.
		/// </param>
		/// <param name="panelFound">
		///     Returns a boolean indicating whether or not the panel was found
		///     and selected to be displayed.  This could be false if an invalid
		///     or old contentKey was supplied.
		/// </param>
		private void panelChosen(IEnumerable<ContentPanel> panels, string contentKey, out bool panelFound)
		{
			if (string.IsNullOrEmpty(contentKey))
			{
				panelFound = false;
				return;
			}

			foreach (ContentPanel currContent in panels)
			{
				if (currContent.Key == contentKey)
				{
					panelFound = true;
					panelChosen(currContent);
					return;
				}
			}

			panelFound = false;
		}

		private void raiseContentShownEvent(object s, ContentShownEventArgs args)
		{
			ContentShownDelegate evt;

			evt = ContentShown;

			if (evt != null)
				evt(s, args);
		}

		/// <summary>
		///     Gets a random number lower than the one specified.
		/// </summary>
		/// <remarks>
		///     This method is fairly pointless now, because .NET 2.0 has
		///     a method that makes it easy to get a number within a specified
		///     range.
		/// </remarks>
		/// <param name="totalImpressions">
		///     The maxiumum random value allowed.
		/// </param>
		/// <returns>
		///     A random number between 0 and totalImpressions
		/// </returns>
		private static int getRandomValue(int totalImpressions)
		{
			Random rand;

			rand = new Random();
			return rand.Next(0, totalImpressions + 1);
		}
	}
}