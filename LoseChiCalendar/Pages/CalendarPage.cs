using System ;
using System . Collections ;
using System . Collections . Generic ;
using System . Linq ;
using System . Xml . Linq ;

using DreamRecorder . FoggyConsole . Controls ;
using DreamRecorder . ToolBox . General ;

namespace LoseChiCalendar . Pages
{

	public sealed class CalendarPage:Page
	{

		public CalendarPage ( ):base(XDocument.Parse(
													typeof(CalendarPage).GetResourceFile(@"CalendarPage.xml")
													).Root)
		{


		}

		public Label YearMonthLabel { get ; set ; } 

		public Canvas MainCanvas { get ; set ; }

		public override void OnNavigateTo() => base.OnNavigateTo();
	}

}