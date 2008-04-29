using System;
using YTech.WebControls.ContentRotator;

namespace YTech.WebControls.ContentRotator
{
	/// <summary>
	///		EventArgs for the event raised when content is displayed in the content rotator.
	/// </summary>
	public class ContentShownEventArgs : EventArgs
	{
		private ContentPanel _shownPanel;

		/// <summary>
		///     Gets or sets the <see cref="ContentPanel"/> that was
		///     displayed by the <see cref="ServerContentRotator"/>.
		/// </summary>
		public ContentPanel ShownPanel
		{
			get { return _shownPanel; }
			set { _shownPanel = value; }
		}
	}
}