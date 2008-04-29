namespace YTech.WebControls.ContentRotator
{
	/// <summary>
	///     The different methods to rotate the content in
	///     the <see cref="ServerContentRotator"/>.
	/// </summary>
	public enum RotationModes
	{
		/// <summary>
		///     Each user will always see the same content they
		///     previously viewed.
		/// </summary>
		AlwaysSame,
		/// <summary>
		///     Each users should see different content from what they
		///     had previously used. (Not yet supported)
		/// </summary>
		AlwaysDifferent,
		/// <summary>
		///     The content is randomly selected.
		/// </summary>
		Random
	}
}