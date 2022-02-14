using System ;
using System . Collections ;
using System . Collections . Generic ;
using System . Globalization ;
using System . Linq ;
using System . Xml . Linq ;

using DreamRecorder . FoggyConsole ;
using DreamRecorder . FoggyConsole . Controls ;
using DreamRecorder . ToolBox . General ;

using Pcg ;

namespace DreamRecorder . LoseChiCalendar . Pages
{

	public sealed class CalendarPage : Page
	{

		private DateTime _currentDateTime = DateTime . Now . Date ;

		/// <summary>
		/// Things to do, current preset value is based on previous Mizunara's suggestion. These location can be found at UESTC in Chengdu, PRC.
		/// </summary>
		private List <string> ThingsTodo { get ; } = new List <string>
													{
														"study in the Library" ,
														"study in dormitory" ,
														"study in the Pinxue Building" ,
														"study in the Liren Building" ,
														"swim" ,
														"practice playing piano" ,
														"practice playing viola" ,
														"practice playing violin" ,
														"do homework" ,
														"watch ducks" ,
														"watch PornHub" ,
														"watch Youtube"
													} ;

		public Label YearMonthLabel { get ; set ; }

		public Label YearMonthLabel2 { get ; set ; }

		public FIGletLabel DateLabel { get ; set ; }

		public Label DayNameLabel { get ; set ; }

		public Button TodayButton { get ; set ; }

		public Button ExitButton { get ; set ; }

		public Button PreviousMonthButton { get ; set ; }

		public Button NextMonthButton { get ; set ; }

		public Container MonthCalendarContainer { get ; set ; }

		public HorizontalStackPanel ProperThingsTodoContainer { get ; set ; }

		public HorizontalStackPanel ImproperThingsTodoContainer { get ; set ; }

		public DateTime ? MonthCalendarDate
		{
			get => MonthCalendarContainer ? . Tag as DateTime ? ;
			set
			{
				if ( MonthCalendarContainer != null )
				{
					MonthCalendarContainer . Tag = value ;
				}
			}
		}

		public DateTime CurrentDateTime
		{
			get => _currentDateTime ;
			set
			{
				if ( _currentDateTime != value )
				{
					_currentDateTime = value . Date ;
					UpdateView ( ) ;
				}
			}
		}

		public CalendarPage ( ) : base (
										XDocument . Load (
														typeof ( CalendarPage ) . GetResourceFileStream (
														@"CalendarPage.xml" ) ) .
													Root )
		{
			YearMonthLabel              = Find <Label> ( nameof ( YearMonthLabel ) ) ;
			YearMonthLabel2             = Find <Label> ( nameof ( YearMonthLabel2 ) ) ;
			DateLabel                   = Find <FIGletLabel> ( nameof ( DateLabel ) ) ;
			DayNameLabel                = Find <Label> ( nameof ( DayNameLabel ) ) ;
			ExitButton                  = Find <Button> ( nameof ( ExitButton ) ) ;
			MonthCalendarContainer      = Find <Container> ( nameof ( MonthCalendarContainer ) ) ;
			PreviousMonthButton         = Find <Button> ( nameof ( PreviousMonthButton ) ) ;
			NextMonthButton             = Find <Button> ( nameof ( NextMonthButton ) ) ;
			TodayButton                 = Find <Button> ( nameof ( TodayButton ) ) ;
			ProperThingsTodoContainer   = Find <HorizontalStackPanel> ( nameof ( ProperThingsTodoContainer ) ) ;
			ImproperThingsTodoContainer = Find <HorizontalStackPanel> ( nameof ( ImproperThingsTodoContainer ) ) ;

			ExitButton . Pressed          += ExitButton_Pressed ;
			PreviousMonthButton . Pressed += PreviousMonthButton_Pressed ;
			NextMonthButton . Pressed     += NextMonthButton_Pressed ;
			TodayButton . Pressed         += TodayButton_Pressed ;
		}

		private void TodayButton_Pressed ( object sender , EventArgs e ) { CurrentDateTime = DateTime . Now . Date ; }

		private void NextMonthButton_Pressed ( object sender , EventArgs e )
		{
			DateTime currentDateTime = CurrentDateTime ;
			CurrentDateTime = currentDateTime . AddMonths ( 1 ) ;
		}

		private void PreviousMonthButton_Pressed ( object sender , EventArgs e )
		{
			DateTime currentDateTime = CurrentDateTime ;
			CurrentDateTime = currentDateTime . AddMonths ( - 1 ) ;
		}

		private void ExitButton_Pressed ( object sender , EventArgs e )
		{
			Program . Current . Exit ( ProgramExitCode . Success ) ;
		}

		public void UpdateView ( )
		{
			ViewRoot ? . PauseRedraw ( ) ;

			CultureInfo cultureInfo = new CultureInfo ( "en-US" ) ;

			DateTimeFormatInfo dateTimeFormat = cultureInfo . DateTimeFormat ;

			YearMonthLabel . Text  = CurrentDateTime . ToString ( "MMMM yyyy" , cultureInfo ) ;
			YearMonthLabel2 . Text = CurrentDateTime . ToString ( "MMMM yyyy" , cultureInfo ) ;
			DateLabel . Text       = CurrentDateTime . Day . ToString ( ) ;
			DayNameLabel . Text    = CurrentDateTime . ToString ( "dddd" , cultureInfo ) ;

			if ( MonthCalendarDate ? . Date != CurrentDateTime . Date )
			{
				#region Update Month Calendar

				bool changeFocus = ViewRoot ? . Application . FocusManager . FocusedControl ? . Container ? . Container
									== MonthCalendarContainer ;


				Canvas canvas = new Canvas ( ) ;

				if ( MonthCalendarContainer != null )
				{
					MonthCalendarContainer . Content = canvas ;
				}

				MonthCalendarDate = CurrentDateTime . Date ;

				int y = 0 ;

				string [ ] dayNames = dateTimeFormat . AbbreviatedDayNames ;

				for ( int i = 0 ; i < 7 ; i++ )
				{
					Label label = new Label
								{
									Text = dayNames [ i ] , HorizontalAlign = ContentHorizontalAlign . Left , Width = 3
								} ;

					if ( i   == 0
						|| i == 6 )
					{
						label . ForegroundColor = ConsoleColor . Red ;
					}

					canvas . Items . Add ( label ) ;

					canvas [ label ] = new Point ( 6 * i + 1 , y ) ;
				}

				y++ ;

				DateTime firstMonthDay = new DateTime ( CurrentDateTime . Year , CurrentDateTime . Month , 1 ) ;

				int daysInMonth = DateTime . DaysInMonth ( CurrentDateTime . Year , CurrentDateTime . Month ) ;

				int weekday = ( int )firstMonthDay . DayOfWeek ;

				DateTime prevMonth = CurrentDateTime . AddMonths ( - 1 ) ;

				int daysInPrevMonth = DateTime . DaysInMonth ( prevMonth . Year , prevMonth . Month ) ;

				int daysBefore = weekday ;

				bool newLine = ( daysBefore == 0 ) ;

				if ( newLine )
				{
					daysBefore = 7 ;
				}

				for ( int i = 0 ; i < daysBefore ; i++ )
				{
					int day = daysInPrevMonth - ( daysBefore - i ) + 1 ;

					Button button = new Button
									{
										Name            = $"buttonPrev{day}" ,
										Text            = $"{day}" ,
										HorizontalAlign = ContentHorizontalAlign . Right ,
										Width           = 4 ,
										Tag             = new DateTime ( prevMonth . Year , prevMonth . Month , day ) ,
										ForegroundColor = ConsoleColor . DarkGray
									} ;

					button . Pressed += DateButton_Pressed ;

					canvas . Items . Add ( button ) ;

					canvas [ button ] = new Point ( 6 * i , y ) ;
				}

				if ( newLine )
				{
					y++ ;
				}

				for ( int i = 1 ; i <= daysInMonth ; i++ )
				{
					Button button = new Button
									{
										Name = $"button{i}" ,
										Text = $"{i}" ,
										HorizontalAlign = ContentHorizontalAlign . Right ,
										Width = 4 ,
										Tag = new DateTime ( CurrentDateTime . Year , CurrentDateTime . Month , i )
									} ;

					button . Pressed += DateButton_Pressed ;

					if ( i <= 10 )
					{
						button . KeyBind = i . ToString ( ) . Last ( ) ;
					}

					if ( weekday   == 0
						|| weekday == 6 )
					{
						button . ForegroundColor = ConsoleColor . Cyan ;
					}

					if ( i == CurrentDateTime . Day )
					{
						button . ForegroundColor = ConsoleColor . Green ;
						if ( changeFocus )
						{
							if ( ViewRoot ? . Application . FocusManager != null )
							{
								ViewRoot . Application . FocusManager . FocusedControl = button ;
							}
						}
					}

					if ( ( button . Tag as DateTime ? ) ? . Date == DateTime . Today . Date )
					{
						button . ForegroundColor = ConsoleColor . Blue ;
					}

					canvas . Items . Add ( button ) ;

					canvas [ button ] = new Point ( 6 * weekday , y ) ;

					weekday++ ;

					while ( weekday >= 7 )
					{
						weekday -= 7 ;
						y++ ;
					}
				}

				DateTime nextMonth = CurrentDateTime . AddMonths ( 1 ) ;

				for ( int i = weekday ; i < 7 ; i++ )
				{
					int day = i - weekday + 1 ;

					Button button = new Button
									{
										Name            = $"buttonNext{day}" ,
										Text            = $"{day}" ,
										HorizontalAlign = ContentHorizontalAlign . Right ,
										Width           = 4 ,
										Tag             = new DateTime ( nextMonth . Year , nextMonth . Month , day ) ,
										ForegroundColor = ConsoleColor . DarkGray
									} ;

					button . Pressed += DateButton_Pressed ;

					canvas . Items . Add ( button ) ;

					canvas [ button ] = new Point ( 6 * i , y ) ;
				}

				if ( y < 6 )
				{
					y++ ;

					for ( int i = 0 ; i < 7 ; i++ )
					{
						int day = i - weekday + 8 ;

						Button button = new Button
										{
											Name = $"buttonNext{day}" ,
											Text = $"{day}" ,
											HorizontalAlign = ContentHorizontalAlign . Right ,
											Width = 4 ,
											Tag = new DateTime ( nextMonth . Year , nextMonth . Month , day ) ,
											ForegroundColor = ConsoleColor . DarkGray
										} ;

						button . Pressed += DateButton_Pressed ;

						canvas . Items . Add ( button ) ;

						canvas [ button ] = new Point ( 6 * i , y ) ;
					}
				}

				#endregion

				PcgRandom random = new PcgRandom ( CurrentDateTime . Date . Ticks . GetHashCode ( ) ) ;

				int properThingsCount = random . Next ( 1 , 3 ) ;

				int improperThingsCount = random . Next ( 1 , 3 ) ;

				int thingsTodoCount = properThingsCount + improperThingsCount ;

				List <string> thingsTodo =
					ThingsTodo . RandomUniqueChoose ( thingsTodoCount , ( PcgRandomWrapper )random ) ;

				ProperThingsTodoContainer . Items . Clear ( ) ;

				for ( int i = 0 ; i < properThingsCount ; i++ )
				{
					if ( i > 0 )
					{
						Label orLabel = new Label { Text = " or " } ;

						ProperThingsTodoContainer . Items . Add ( orLabel ) ;
					}

					Label thingLabel = new Label { Text = thingsTodo [ i ] , ForegroundColor = ConsoleColor . Blue } ;

					ProperThingsTodoContainer . Items . Add ( thingLabel ) ;
				}

				ImproperThingsTodoContainer . Items . Clear ( ) ;

				for ( int i = 0 ; i < improperThingsCount ; i++ )
				{
					if ( i > 0 )
					{
						Label orLabel = new Label { Text = " or " } ;

						ImproperThingsTodoContainer . Items . Add ( orLabel ) ;
					}

					Label thingLabel = new Label
										{
											Text            = thingsTodo [ i + properThingsCount ] ,
											ForegroundColor = ConsoleColor . Blue
										} ;

					ImproperThingsTodoContainer . Items . Add ( thingLabel ) ;
				}

				ViewRoot ? . ResumeRedraw ( ) ;
			}
		}

		private void DateButton_Pressed ( object sender , EventArgs e )
		{
			CurrentDateTime = ( ( sender as Button ) ? . Tag as DateTime ? ) ? . Date ?? CurrentDateTime ;
		}

		public override void OnNavigateTo ( )
		{
			UpdateView ( ) ;
			base . OnNavigateTo ( ) ;
		}

	}

}
