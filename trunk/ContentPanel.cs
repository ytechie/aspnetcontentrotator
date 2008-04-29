using System.Web.UI.WebControls;

namespace YTech.WebControls.ContentRotator
{
	/// <summary>
	///     A panel that will have a chance of being randomly
	///     rotated within a <see cref="ServerContentRotator"/>
	/// </summary>
	public class ContentPanel : Panel
	{
		private int _impressions = 1; //Default to 1, so it will show up if in the list
		private string _key = "";

		/// <summary>
		///     An arbitrary number that describes the number of times
		///     to display the content in relation to all of the other
		///     panels in the <see cref="ServerContentRotator"/>.
		/// </summary>
		public int Impressions
		{
			get { return _impressions; }
			set { _impressions = value; }
		}

		/// <summary>
		///     A unique string that identifies this particluar content.  This
		///     is used if you want to be able to track the content that is shown.
		/// </summary>
		public string Key
		{
			get { return _key; }
			set { _key = value; }
		}
	}
}