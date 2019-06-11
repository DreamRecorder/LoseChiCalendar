using System ;
using System . Collections ;
using System . Collections . Generic ;
using System . Globalization ;
using System . Linq ;
using System . Xml . Linq ;

using DreamRecorder . FoggyConsole ;
using DreamRecorder . FoggyConsole . Controls ;
using DreamRecorder . ToolBox . General ;

namespace LoseChiCalendar . Pages
{

	public sealed class CalendarPage:Page
	{

		private DateTime _currentDateTime=DateTime.Now ;

		public CalendarPage ( ):base(XDocument.Parse(
													typeof(CalendarPage).GetResourceFile(@"CalendarPage.xml")
													).Root)
		{

			YearMonthLabel = Find<Label> ( nameof ( YearMonthLabel ) ) ;
			DateLabel = Find<FIGletLabel>(nameof(DateLabel));
			DayNameLabel= Find<Label>(nameof(DayNameLabel));
			ExitButton = Find <Button> ( nameof ( ExitButton ) ) ;
			MonthCalendarContainer = Find <Container> ( nameof ( MonthCalendarContainer ) ) ;

            ExitButton.Pressed += ExitButton_Pressed;
		}

		private void ExitButton_Pressed(object sender, EventArgs e) {Program.Current. Exit(ProgramExitCode.Success); }

        public Label YearMonthLabel { get ; set ; }

		public FIGletLabel DateLabel { get ; set ; }

		public Label DayNameLabel { get ; set ; }

		public Canvas MainCanvas { get ; set ; }

		public Button ExitButton { get ; set ; }

		public Container MonthCalendarContainer { get; set; }

		public DateTime? MonthCalendarDate
		{
			get => MonthCalendarContainer?.Tag as DateTime?;
            set => MonthCalendarContainer.Tag = value;
		}


		public DateTime CurrentDateTime
		{
			get => _currentDateTime ;
			set
			{
				if (_currentDateTime != value)
				{
					_currentDateTime = value;
					UpdateView ( ) ;
				}
			}
		}

		public void UpdateView ( )
		{
			YearMonthLabel . Text = CurrentDateTime. ToString ( "MMMM yyyy" ) ;
			DateLabel . Text = CurrentDateTime. Day . ToString ( ) ;
			DayNameLabel . Text = CurrentDateTime. ToString ( "dddd" ) ;

			if ( MonthCalendarDate?.Date != CurrentDateTime.Date)
			{
				CultureInfo cultureInfo = new CultureInfo("en-US");

                DateTimeFormatInfo dateTimeFormat = cultureInfo.DateTimeFormat;

				Canvas canvas = new Canvas();

				MonthCalendarContainer.Content=(canvas);

				MonthCalendarDate = CurrentDateTime . Date ;

                int y = 0 ;

                string[] dayNames = dateTimeFormat.AbbreviatedDayNames;

				for (int i = 0; i < 7; i++)
				{
					Label label = new Label()
								{
									Text            = (dayNames[i]),
									HorizontalAlign = ContentHorizontalAlign.Left,
									Width           = 3,
								};

					if (i == 0 || i == 6)
					{
						label.ForegroundColor = ConsoleColor.DarkRed;
					}

					canvas.Items.Add(label);

					canvas[label] = new Point((6 * i) + 1, y);
				}

				y++;

                DateTime firstMonthDay = new DateTime(CurrentDateTime.Year, CurrentDateTime.Month, 1);

                int daysInMonth = DateTime.DaysInMonth(CurrentDateTime.Year, CurrentDateTime.Month);

				int weekday = (int)firstMonthDay.DayOfWeek;

				for (int i = 1; i <= daysInMonth; i++)
				{
					Button button = new Button
									{
										Name            = $"button{i}",
										Text            = $"{i}",
										HorizontalAlign = ContentHorizontalAlign.Right,
										Width           = 5,
									};

					if (i < 11)
					{
						button.KeyBind = i.ToString().Last();
					}

					if (weekday == 0 || weekday == 6)
					{
						button.ForegroundColor = ConsoleColor.Red;
					}

					if ( i== CurrentDateTime.Day)
					{
						button.ForegroundColor = ConsoleColor.Green;
                    }

                    if (new DateTime(CurrentDateTime.Year, CurrentDateTime.Month, i) == DateTime.Today.Date)
					{
						button.ForegroundColor = ConsoleColor.Blue;
					}

					canvas.Items.Add(button);

					canvas[button] = new Point(6 * weekday, y);

					weekday++;

					while (weekday >= 7)
					{
						weekday -= 7;
						y++;
					}
				}
            }


        }

        public override void OnNavigateTo()
		{
			UpdateView ( ) ;
			base . OnNavigateTo ( ) ;
		}

	}

}