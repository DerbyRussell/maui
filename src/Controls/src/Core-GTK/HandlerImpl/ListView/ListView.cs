namespace Microsoft.Maui.Controls
{
	public partial class ListView
	{
		public static IPropertyMapper<IListView, ListViewHandler> ControlsListViewMapper =
				new PropertyMapper<ListView, ListViewHandler>(ListViewHandler.Mapper)
				{
				};

		internal static new void RemapForControls()
		{
			// Adjust the mappings to preserve Controls.ScrollView legacy behaviors
			ListViewHandler.Mapper = ControlsListViewMapper;
		}
	}
}
